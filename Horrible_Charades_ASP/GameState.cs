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
            }

            foreach (Team team in game.Teams)
            {
                if (team.Id == game.TurnOrder[game.Turn])
                {
                    game.WhosTurn = team;
                }
            }
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

        internal Game GetRuleChanger(Game game, int index)
        {
            for (int i = 0; i < 6; i++)
            {

                RuleChanger ruleChanger = _dbUtils.GetRuleChanger();

                if (ruleChanger.Type == "PowerUp")
                {

                    //RuleChanger modifier = _dbUtils.GetRuleChanger(type);
                    game.Teams[index].PowerUps.Add(ruleChanger);
                    //return game;
                }
                else if (ruleChanger.Type == "FunkUp")
                {
                    //RuleChanger modifier = _dbUtils.GetRuleChanger(type);
                    game.Teams[index].FunkUps.Add(ruleChanger);
                    //return game;
                }
            }
            return game;

        }
        internal Game GetAdjective(string gameCode)
        {
            var adjective = _dbUtils.GetAdjective();
            //List<string> tmpList = _dbUtils.GetIncorrectAnswers(adjective);
            Game game = GetGame(gameCode);
            game.CurrentCharade.Adjective.Add(_dbUtils.GetAdjective());
            return game;
        }

        internal Game GetVerb(string gameCode)
        {
            //var verb = _dbUtils.GetVerb();
            Game game = GetGame(gameCode);
            game.CurrentCharade.Verb.Add(_dbUtils.GetVerb());
            //List<string> tmpList = _dbUtils.GetIncorrectAnswers(verb);

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

        internal Game GiveAllTeamsRuleChanger(string connectionId, string gameCode, out int index)
        {
            Game game = GetGame(gameCode);
            index = GetTeam(game, connectionId);
            GetRuleChanger(game, index);

            return game;
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
            if (team.ConnectionID == conId)
            {
                team.TurnPoint *= 4;
            }

            return game;
        }
    }
}