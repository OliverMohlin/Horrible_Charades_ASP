using Horrible_Charades_ASP.Database;
using Horrible_Charades_ASP.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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

        public IHubConnectionContext<dynamic> Clients { get; set; }         //Todo: Funkar detta?
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
        public Game CreateTeam(string teamName, string gameCode, string conId) //Todo: koppla till connectionstring istället för Teamname?
        {
            Game game = GetGame(gameCode);
            Team team = new Team(teamName);
            team.ConnectionID = conId;
            team.GameCode = gameCode;

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
            if (game.Turn == 0)
            {
                game.TurnOrder = game.Teams.OrderBy(t => RandomUtils.rnd.Next()).Select(o => o.Id).ToArray();
                game.Turn++;
            }
            else if (game.Turn >= game.TurnOrder.Count())
            {
                game.Turn = 0;
                game.TurnOrder = game.Teams.OrderBy(t => RandomUtils.rnd.Next()).Select(o => o.Id).ToArray();
            }

            foreach (Team team in game.Teams)
            {
                if (team.Id == game.TurnOrder[game.Turn])
                {
                    game.WhosTurn = team;
                }
            }
            game.GameState = 3;
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
        internal Game GetNoun(string gameCode)
        {
            Noun noun = _dbUtils.GetNoun();
            //List<string> tmpList = _dbUtils.GetIncorrectAnswers(noun);
            Game game = GetGame(gameCode);
            game.CurrentCharade.Noun = noun;

            return game;
        }
        internal Game GiveAllTeamsRuleChanger(string connectionId, string gameCode)
        {
            Game game = GetGame(gameCode);

            // We are now hardcoding the rulechangers at start. 
            //GetRuleChanger(game, index);
            game.CurrentCharade.Adjective.Clear();
            game.CurrentCharade.Verb.Clear();
            GetNoun(gameCode);
            AssignWhosTurn(game);
            return game;
        }


        internal void getRuleChangers(Team team)
        {

            team.PowerUps.Add(_dbUtils.GetRuleChanger("Shuffle"));
            team.FunkUps.Add(_dbUtils.GetRuleChanger("Add Adjective"));
            team.FunkUps.Add(_dbUtils.GetRuleChanger("Add Activity"));

        }

        internal Game GetRuleChanger(Game game, int index)
        {
            foreach (var team in game.Teams)
            {
                //Ändra för olika antal funkups
                for (int i = 0; i < 3; i++)
                {
                    RuleChanger ruleChanger = new RuleChanger();
                    do
                    {
                        ruleChanger = _dbUtils.GetRuleChanger();
                    } while (ruleChanger.ID == 1 || ruleChanger.ID == 3);

                    if (ruleChanger.Type == "PowerUp")
                    {
                        team.PowerUps.Add(ruleChanger);
                    }
                    else if (ruleChanger.Type == "FunkUp")
                    {
                        if (ruleChanger.Description == "Add Adjective")
                        {
                            ruleChanger.HTMLString = "<div class='btn funkup add-adjective' data-ng-click='vm.activateFunkUp(FunkUp.ID)'><p class='funkup-text'><span class='add'>Add</span><br />Adjective</p></div>";
                        }
                        else if (ruleChanger.Description == "Add Verb")
                        {
                            ruleChanger.HTMLString = "<div class='btn funkup add-verb' data-ng-click='vm.activateFunkUp(FunkUp.ID)'><p class='funkup-text'><span class='add'>Add</span> <br />Verb</p></div>";
                        }
                        else
                        {
                            ruleChanger.HTMLString = "<div class='btn funkup reduce-time' data-ng-click='vm.activateFunkUp(FunkUp.ID)'>+ <br />15 Seconds</div>";
                        }
                        team.FunkUps.Add(ruleChanger);
                    }
                }
            }
            return game;
        }
        internal Game GetAdjective(string gameCode)
        {
            Game game = GetGame(gameCode);
            if (game.CurrentCharade.Adjective.Count >= 2)
            {
                return game;
            }


            game.CurrentCharade.Adjective.Add(_dbUtils.GetAdjective());
            return game;
        }

        internal Game GetVerb(string gameCode)
        {
            Game game = GetGame(gameCode);
            if (game.CurrentCharade.Verb.Count >= 2)
            {
                return game;
            }
            game.CurrentCharade.Verb.Add(_dbUtils.GetVerb());

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

        internal Game ShuffleCharade(string gameCode)
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

            return game;
        }

        internal Game PrepareNewRound(string gameCode, string conId)
        {
            Game game = GetGame(gameCode);
            Team team = game.Teams.SingleOrDefault(t => t.ConnectionID == conId);

            team.TurnPoint = 0;
            AssignWhosTurn(game);
            return game;
        }

        private List<Verb> ShuffleVerbsToCurrentCharade(Game game)
        {
            var verbList = new List<Verb>();

            for (int i = 0; i < game.CurrentCharade.Verb.Count(); i++)
            {
                Verb lastVerb = game.CurrentCharade.Verb[i];
                Verb tmpNewVerb = _dbUtils.GetVerb();

                if (tmpNewVerb.Description == lastVerb.Description)
                {
                    tmpNewVerb = _dbUtils.GetVerb();
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
                Adjective tmpNewAdjective = _dbUtils.GetAdjective();

                if (tmpNewAdjective.Description == lastAdjective.Description)
                {
                    tmpNewAdjective = _dbUtils.GetAdjective();
                }
                adjectiveList.Add(tmpNewAdjective);
            }
            return adjectiveList;
        }

        internal Game AssignPoints(string gameCode, int timeLeft, string conId)
        {
            Game game = GetGame(gameCode);
            Team team = game.Teams.SingleOrDefault(t => t.ConnectionID == conId);
            int charadewords = game.CurrentCharade.Adjective.Count() + game.CurrentCharade.Verb.Count() + 1;

            if (timeLeft > 45)
            {
                team.TurnPoint = 400 * charadewords;
            }
            else if (timeLeft > 30)
            {
                team.TurnPoint = 300 * charadewords;
            }
            else if (timeLeft > 15)
            {
                team.TurnPoint = 200 * charadewords;
            }
            else if (timeLeft > 0)
            {
                team.TurnPoint = 100 * charadewords;
            }
            if (game.WhosTurn.ConnectionID == conId)
            {
                team.TurnPoint *= 2;
                foreach (var loopTeams in game.Teams)
                {
                    loopTeams.TotalPoints += loopTeams.TurnPoint;
                }
            }

            game.GameState = 6;
            return game;
        }
        internal Game AssignPoints(string gameCode, int timeLeft, string conId, string guess)
        {
            Game game = GetGame(gameCode);
            Team team = game.Teams.SingleOrDefault(t => t.ConnectionID == conId);

            List<string> charade = new List<string>();

            charade.Add(game.CurrentCharade.Noun.Description);

            int correctwords = 0;
            foreach (Word word in game.CurrentCharade.Adjective)
            {
                charade.Add(word.Description);
            }

            foreach (Word word in game.CurrentCharade.Verb)
            {
                charade.Add(word.Description);
            }

            foreach (string word in charade)
            {
                if (guess.Contains(word))
                {
                    correctwords++;
                }
            }

            if (correctwords == charade.Count())
            {
                int charadewords = game.CurrentCharade.Adjective.Count() + game.CurrentCharade.Verb.Count() + 1;
                if (timeLeft > 45)
                {
                    team.TurnPoint = 100 * charadewords;
                }
                else if (timeLeft > 30)
                {
                    team.TurnPoint = 80 * charadewords;
                }
                else if (timeLeft > 15)
                {
                    team.TurnPoint = 60 * charadewords;
                }
                else if (timeLeft > 0)
                {
                    team.TurnPoint = 40 * charadewords;
                }
                if (game.WhosTurn.ConnectionID == conId)
                {
                    team.TurnPoint *= 2;
                }
            }


            return game;
        }
    }
}