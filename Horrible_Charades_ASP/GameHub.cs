using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Horrible_Charades_ASP.Database;

namespace Horrible_Charades_ASP
{
    public class GameHub : Hub
    {
        // TODO: Kolla varför GameHub säger att _dbUtils alltid kommer att vara null ?!
        DatabaseUtils _dbUtils = new DatabaseUtils(
            new Models.CharadeContext());

        public void Hello(string textToWrite)
        {
            Clients.All.hello(textToWrite);
            int i = GameState.Instance.ReturnNumberOfClients();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="teamName"></param>
        public void createTeam(string teamName)
        {
            var team = GameState.Instance.CreateTeam(teamName);
            team.ConnectionID = Context.ConnectionId;
            int connectedClients = GameState.Instance.ReturnNumberOfClients();
            Clients.All.teamsJoined(team.Name, team.ConnectionID, connectedClients);
        }
        /// <summary>
        /// Hämtar ett slumpat Substantiv från Databasen
        /// </summary>
        public void GetNoun()
        {
            string noun = _dbUtils.GetNoun();
            Clients.All.hello(noun);
        }
        public void GetAdjective()
        {
            string adjective = _dbUtils.GetAdjective();
            Clients.All.hello(adjective);
        }
        public void GetVerb()
        {
            string verb = _dbUtils.GetVerb();
            Clients.All.hello(verb);
        }
    }
}