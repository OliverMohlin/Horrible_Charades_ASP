using Horrible_Charades_ASP.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Horrible_Charades_ASP
{
    public class GameState
    {
        private static readonly Lazy<GameState> _instance = new Lazy<GameState>(            //Lazy = Skapas inte förän klassen accessas
            () => new GameState(GlobalHost.ConnectionManager.GetHubContext<GameHub>()));    //Skickar in vår Gamehub till konstruktorn

        private readonly ConcurrentDictionary<string, Team> _teams =
            new ConcurrentDictionary<string, Team>(StringComparer.OrdinalIgnoreCase);

        private readonly ConcurrentDictionary<string, Game> _games =
            new ConcurrentDictionary<string, Game>(StringComparer.OrdinalIgnoreCase);

        public IHubConnectionContext<dynamic> Clients { get; set; }         //Todo: Funkar detta?
        public IGroupManager Groups { get; set; }                           //Används för att hålla koll på SignalR grupper

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
        /// Returnerar en int som visar hur många devices som är connectade
        /// </summary>
        /// <returns></returns>
        public int ReturnNumberOfClients()
        {
            return _teams.Count();
        }
        /// <summary>
        /// Returnerar ett team från connectade teams
        /// </summary>
        /// <param name="teamName"></param>
        /// <returns></returns>
        internal Team GetTeam(string teamName)
        {
            return _teams.Values.FirstOrDefault(t => t.Name == teamName);
        }
        /// <summary>
        /// Skapar ett team som kopplas till ConnectionId
        /// </summary>
        /// <param name="teamName"></param>
        /// <returns></returns>
        public Game CreateTeam(string teamName, string gameCode, string conId) //Todo: koppla till connectionstring istället för Teamname
        {
            Game game = GetGame(gameCode);
            Team team = new Team(teamName);
            team.ConnectionID = conId;
            team.GameCode = gameCode;

            _teams[team.Name] = team;//Todo: Fundera på vad vi ska koppla Team till, GetMD5Hash för att göra en safe connectionId
            game.Teams.Add(team);
            _games[game.GameCode] = game;
            Groups.Add(team.ConnectionID, team.GameCode);
            return game; //Todo: Ta in gameCode. Lägga till laget i game och i en grupp
        }
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
        public void CreateGame(Game game) //Todo: koppla till connectionstring istället för Teamname
        {
            _games[game.GameCode] = game; //Todo: Fundera på vad vi ska koppla Team till, GetMD5Hash för att göra en safe connectionId
        }
    }
}