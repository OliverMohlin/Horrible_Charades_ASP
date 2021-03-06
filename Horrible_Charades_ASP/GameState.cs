﻿using Horrible_Charades_ASP.Database;
using Horrible_Charades_ASP.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Horrible_Charades_ASP
{
    public class GameState
    {

        /// <summary>
        /// Medförde viss problematik med att låta flera klienter prata med databasen samtidigt.
        /// Löstes genom att lägga till (MARS) "MultipleActiveResultSets=true" i ConnectionString.
        /// MARS låter flera göra Queryes hos databasen samtidigt. 
        /// </summary>
        private static readonly Lazy<GameState> _instance = new Lazy<GameState>(            //Lazy = Skapas inte förän klassen accessas
            () => new GameState(GlobalHost.ConnectionManager.GetHubContext<GameHub>()));    //Skickar in vår Gamehub till konstruktorn

        private readonly ConcurrentDictionary<string, Team> _teams =
            new ConcurrentDictionary<string, Team>(StringComparer.OrdinalIgnoreCase);

        private readonly ConcurrentDictionary<string, Game> _games =
            new ConcurrentDictionary<string, Game>(StringComparer.OrdinalIgnoreCase);

        public IHubConnectionContext<dynamic> Clients { get; set; }
        public IGroupManager Groups { get; set; }                           //Används för att hålla koll på SignalR grupper

        DatabaseUtils _dbUtils = new DatabaseUtils(new CharadeContext());

        public GameState(IHubContext context)
        {
            Clients = context.Clients;
            Groups = context.Groups;
        }

        public static GameState Instance
        {
            get { return _instance.Value; }
        }
        /// <summary>
        /// Checks if a game with the speicific gamecode already has a team with the requested name
        /// </summary>
        /// <param name="teamName"></param>
        /// <returns></returns>
        internal Team GetTeam(string teamName, string gameCode)
        {
            return _teams.Values.FirstOrDefault(t => t.Name == teamName && t.GameCode == gameCode);
        }

        internal int GetTeam(Game game, string connectionId)
        {
            var index = game.Teams.FindIndex(t => t.ConnectionID == connectionId);

            return index;
        }
        /// <summary>
        /// Skapar ett team som kopplas till ConnectionId
        /// Creates a team, add to game and SignalRGroup
        /// </summary>
        /// <param name="teamName"></param>
        /// <returns></returns>
        public Game CreateTeam(string teamName, string gameCode, string conId, out Team team) //Todo: koppla till connectionstring istället för Teamname?
        {
            Game game = GetGame(gameCode);
            team = new Team(teamName, conId, gameCode);

            _teams[team.Name] = team;//Todo: Fundera på vad vi ska koppla Team till, GetMD5Hash för att göra en safe connectionId
            game.Teams.Add(team);

            Task add = Groups.Add(team.ConnectionID, team.GameCode);
            add.Wait();
            game.GameState = 2;
            return game;
        }

        /// <summary>
        /// Assigns Who's turn in game
        /// </summary>
        /// <param name="game"></param>
        internal void AssignWhosTurn(Game game)
        {
            if (game.Turn >= game.TurnOrder.Count())
            {
                game.Turn = 0;
                game.Round++;
            }

            foreach (Team team in game.Teams)
            {
                if (team.ConnectionID == game.TurnOrder[game.Turn])
                {
                    game.WhosTurn = team;
                }
            }
            game.GameState = 3; //Kanske inte bra att ändra gamestatet här
        }

        /// <summary>
        /// Return a game with a specific gameCode
        /// </summary>
        /// <param name="gameCode"></param>
        /// <returns></returns>
        internal Game GetGame(string gameCode)
        {
            var game = _games.FirstOrDefault(g => g.Key == gameCode);
            return game.Value;//.Values.FirstOrDefault(t => t.Name == teamName);
        }
        /// <summary>
        /// Skapar ett team som kopplas till ConnectionId
        /// </summary>
        /// <param name="teamName"></param>
        /// <returns></returns>
        public Game CreateGame() //Todo: koppla till connectionstring istället för Teamname
        {
            Game game = new Game();
            _games[game.GameCode] = game; //Todo: Fundera på vad vi ska koppla Team till, GetMD5Hash för att göra en safe connectionId
            return game;
        }

        // Todo: Se över hur vi ska hämta ut och lämna över listorna med felaktiga gissningar
        internal Noun GetNoun(List<Charade> usedCharades)
        {
            Noun noun = _dbUtils.GetNoun(usedCharades);
            return noun;
        }

        internal Game GiveAllTeamsRuleChanger(string connectionId, string gameCode, int roundsToPlay)
        {
            Game game = GetGame(gameCode);

            if (game.Round == 0)
            {
                game.RoundsToPlay = roundsToPlay;
                ShuffleTurnOrder(game);
            }
            foreach (Team team in game.Teams)
            {
                GetRuleChangers(team, roundsToPlay);
            }
            game.CurrentCharade.Noun = GetNoun(game.UsedCharades);
            AssignWhosTurn(game);
            return game;
        }

        private void getRuleChanger(Team team)
        {
            if (team.PowerUps.Count >= 2 && team.FunkUps.Count >= 2)
            {
                return;
            }

            RuleChanger ruleChanger = new RuleChanger();

            ruleChanger = _dbUtils.GetRandomRuleChanger();

            if (ruleChanger.Type == "PowerUp" && team.PowerUps.Count < 2)
            {
                team.PowerUps.Add(ruleChanger);
            }
            else if (ruleChanger.Type == "FunkUp" && team.FunkUps.Count < 2)
            {
                if (ruleChanger.ID == 4)
                {
                    ruleChanger.Description = "Adjective";
                }
                else if (ruleChanger.ID == 5)
                {
                    ruleChanger.Description = "Verb";
                }
                team.FunkUps.Add(ruleChanger);
            }
            else if (ruleChanger.Type == "PowerUp" && team.FunkUps.Count < 2)
            {
                ruleChanger = _dbUtils.GetRuleChangerByType("FunkUp");
                if (ruleChanger.ID == 4)
                {
                    ruleChanger.Description = "Adjective";
                }
                else if (ruleChanger.ID == 5)
                {
                    ruleChanger.Description = "Verb";
                }
                team.FunkUps.Add(ruleChanger);
            }
            else
            {
                ruleChanger = _dbUtils.GetRuleChangerByType("PowerUp");
                team.PowerUps.Add(ruleChanger);
            }

        }

        /// <summary>
        /// Takes in roundsToPlay and subtract two to know how many rulechangers to give
        /// </summary>
        /// <param name="team"></param>
        /// <param name="roundsToPlay"></param>
        internal void GetRuleChangers(Team team, int roundsToPlay)
        {
            for (int i = 0; i < roundsToPlay - 2; i++)
            {
                getRuleChanger(team);
            }
        }
        internal Game RemoveRuleChanger(string gameCode, string connectionId, int ruleChangerId)
        {
            Game game = GetGame(gameCode);
            Team team = game.Teams.SingleOrDefault(t => t.ConnectionID == connectionId);

            if (ruleChangerId < 3)
            {
                team.PowerUps.Remove(team.PowerUps.FirstOrDefault(r => r.ID == ruleChangerId));
            }
            else
            {
                team.FunkUps.Remove(team.FunkUps.FirstOrDefault(r => r.ID == ruleChangerId));
            }
            return game;
        }

        private void ShuffleTurnOrder(Game game)
        {
            game.TurnOrder = game.Teams.OrderBy(t => RandomUtils.rnd.Next()).Select(o => o.ConnectionID).ToArray();
        }

        internal Game GetAdjective(string gameCode, string connectionId, int ruleChangerId)
        {
            Game game = GetGame(gameCode);
            if (game.CurrentCharade.Adjective.Count >= 2)
            {
                return game;
            }

            game.CurrentCharade.Adjective.Add(_dbUtils.GetAdjective(game.UsedCharades));
            game = RemoveRuleChanger(gameCode, connectionId, ruleChangerId);
            return game;
        }

        internal Game GetVerb(string gameCode, string connectionId, int ruleChangerId)
        {
            Game game = GetGame(gameCode);
            if (game.CurrentCharade.Verb.Count >= 2)
            {
                return game;
            }
            game.CurrentCharade.Verb.Add(_dbUtils.GetVerb(game.UsedCharades));
            game = RemoveRuleChanger(gameCode, connectionId, ruleChangerId);

            return game;
        }

        internal List<List<Word>> GetIncorrectAnswers(string gameCode)
        {
            Game game = GetGame(gameCode);
            List<List<Word>> tmpList = new List<List<Word>>();

            foreach (Adjective adjective in game.CurrentCharade.Adjective)
            {
                tmpList.Add(_dbUtils.GetIncorrectAnswers(adjective));
            };

            tmpList.Add(_dbUtils.GetIncorrectAnswers(game.CurrentCharade.Noun));

            foreach (Verb verb in game.CurrentCharade.Verb)
            {
                tmpList.Add(_dbUtils.GetIncorrectAnswers(verb));
            };

            return tmpList;
        }

        internal Game ShuffleCharade(string gameCode, string connectionId, int ruleChangerId)
        {
            Game game = GetGame(gameCode);

            var newNoun = game.CurrentCharade.Noun;
            var adjectiveList = game.CurrentCharade.Adjective;
            adjectiveList = ShuffleAdjectivesToCurrentCharade(game);
            var verbList = game.CurrentCharade.Verb;
            verbList = ShuffleVerbsToCurrentCharade(game);

            do
            {
                newNoun = _dbUtils.GetNoun();
            } while (newNoun.Description == game.CurrentCharade.Noun.Description);
            game.CurrentCharade.Noun = newNoun;
            game.CurrentCharade.Adjective = adjectiveList;
            game.CurrentCharade.Verb = verbList;

            game = RemoveRuleChanger(gameCode, connectionId, ruleChangerId);
            return game;
        }

        internal Game EmptyTurnScores(string gameCode)
        {
            Game game = GetGame(gameCode);
            foreach (Team team in game.Teams)
            {
                team.TurnPoint = 0;
            }

            game.CurrentCharade.Adjective.Clear();
            game.CurrentCharade.Verb.Clear();
            game.CurrentCharade.Noun = GetNoun(game.UsedCharades);

            return game;
        }

        internal Game PrepareNewRound(Game game, string conId)
        {

            game.Turn++;
            AssignWhosTurn(game);

            return game;

        }

        private List<Verb> ShuffleVerbsToCurrentCharade(Game game)
        {
            var verbList = new List<Verb>();

            for (int i = 0; i < game.CurrentCharade.Verb.Count(); i++)
            {
                Verb lastVerb = game.CurrentCharade.Verb[i];
                Verb tmpNewVerb = _dbUtils.GetVerb(game.UsedCharades);

                if (tmpNewVerb.Description == lastVerb.Description)
                {
                    tmpNewVerb = _dbUtils.GetVerb(game.UsedCharades);
                }
                verbList.Add(tmpNewVerb);
            }
            return verbList;
        }

        private List<Adjective> ShuffleAdjectivesToCurrentCharade(Game game)
        {
            var adjectiveList = new List<Adjective>();

            for (int i = 0; i < game.CurrentCharade.Adjective.Count(); i++)
            {
                Adjective lastAdjective = game.CurrentCharade.Adjective[i];
                Adjective tmpNewAdjective = _dbUtils.GetAdjective(game.UsedCharades);

                if (tmpNewAdjective.Description == lastAdjective.Description)
                {
                    tmpNewAdjective = _dbUtils.GetAdjective(game.UsedCharades);
                }
                adjectiveList.Add(tmpNewAdjective);
            }
            return adjectiveList;
        }

        internal Game AssignPoints(string gameCode, int timeLeft, string conId, int points)
        {
            Game game = GetGame(gameCode);
            Team team = game.Teams.SingleOrDefault(t => t.ConnectionID == conId);
            int charadewords = game.CurrentCharade.Adjective.Count() + game.CurrentCharade.Verb.Count() + 1;

            if (timeLeft > 45)
            {
                team.TurnPoint = points * charadewords;
            }
            else if (timeLeft > 30)
            {
                team.TurnPoint = (int)(points * charadewords * 0.75);
            }
            else if (timeLeft > 15)
            {
                team.TurnPoint = (int)(points * charadewords * 0.50);
            }
            else if (timeLeft > 0)
            {
                team.TurnPoint = (int)(points * charadewords * 0.25);
            }

            if (team.TurnPoint > 0)
            {
                getRuleChanger(team);
            }

            //Todo: Ska det här vara här eller när man går mellan Score och TotalScore
            if (game.WhosTurn.ConnectionID == conId)
            {
                foreach (var loopTeams in game.Teams)
                {
                    loopTeams.TotalPoints += loopTeams.TurnPoint;
                }
            }

            game.GameState = 6;
            return game;
        }
        internal void AssignPoints(string gameCode, int timeLeft, string conId, int points, string guess)
        {
            Game game = GetGame(gameCode);
            List<string> CurCharade = new List<string>();

            CurCharade.Add(game.CurrentCharade.Noun.Description);

            int correctwords = 0;
            foreach (Word word in game.CurrentCharade.Adjective)
            {
                CurCharade.Add(word.Description);
            }

            foreach (Word word in game.CurrentCharade.Verb)
            {
                CurCharade.Add(word.Description);
            }

            foreach (string word in CurCharade)
            {
                if (guess.Contains(word))
                {
                    correctwords++;
                }
            }

            if (correctwords == CurCharade.Count())
            {
                AssignPoints(gameCode, timeLeft, conId, points);
            }
        }

        internal Game AssignWinner(string gameCode)
        {
            Game game = GetGame(gameCode);

            int? maxVal = null;
            for (int i = 0; i < game.Teams.Count; i++)
            {
                int thisNum = game.Teams[i].TotalPoints;
                if (!maxVal.HasValue || thisNum > maxVal.Value)
                {
                    maxVal = thisNum;
                }
            }

            foreach (var team in game.Teams)
            {
                if (team.TotalPoints == maxVal)
                {
                    game.Winner.Add(team);

                }
            }

            return game;
        }

    }
}