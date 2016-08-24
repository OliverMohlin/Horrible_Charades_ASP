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
        /// <param name="connectionId"></param>
        /// <returns></returns>
        internal Team GetTeam(string connectionId)
        {
            return _teams.Values.FirstOrDefault(t => t.ConnectionID == connectionId);
        }
        /// <summary>
        /// Skapar ett team som kopplas till ConnectionId
        /// </summary>
        /// <param name="con_str"></param>
        /// <param name="teamName"></param>
        /// <returns></returns>
        public Team CreateTeam(string con_str, string teamName) //Todo: koppla till connectionstring istället för Teamname
        {
            var team = new Team(teamName);
            team.ConnectionID = con_str;
            _teams[con_str] = team;
            return team;
        } 
    }
}