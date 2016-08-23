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

        public int ReturnNumberOfClients()
        {
            return _teams.Count();
        }
        public Team CreateTeam(string teamName) //Todo: koppla till connectionstring istället för Teamname
        {
            var team = new Team(teamName);
            _teams[teamName] = team;
            return team;
        } 
    }
}