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
            new Models.NounContext());

        /// <summary>
        /// This function calls the method Hello on Client-Side, with something to write out
        /// </summary>
        /// <param name="textToWrite"></param>
        public void Hello(string textToWrite)
        {
            Clients.All.hello(textToWrite);
        }
        /// <summary>
        /// Creates a new team if the device don't have a team. 
        /// </summary>
        /// <param name="teamName"></param>
        public void createTeam(string teamName)
        {
            var team = GameState.Instance.GetTeam(Context.ConnectionId);
            if (team != null)
            {
                int connectedClients = GameState.Instance.ReturnNumberOfClients();
                Clients.All.teamsJoined(team.Name, team.ConnectionID, connectedClients);
            }
            else
            {
                team = GameState.Instance.CreateTeam(Context.ConnectionId, teamName);
                int connectedClients = GameState.Instance.ReturnNumberOfClients();
                Clients.All.teamsJoined(team.Name, team.ConnectionID, connectedClients);

            }
        }

        public void getNoun()
        {
            string noun = _dbUtils.GetNoun();
            Clients.All.hello(noun);

        }
    }
}