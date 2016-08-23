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

        public void Hello(string textToWrite)
        {
            Clients.All.hello(textToWrite);
            int i = GameState.Instance.ReturnNumberOfClients();
        }
        public void createTeam(string teamName)
        {
            var team = GameState.Instance.CreateTeam(teamName);
            team.ConnectionID = Context.ConnectionId;
            int connectedClients = GameState.Instance.ReturnNumberOfClients();
            Clients.All.hello(team.Name, team.ConnectionID, connectedClients);
        }

        public void getNoun()
        {
            string noun = _dbUtils.GetNoun();
            Clients.All.hello(noun);

        }
    }
}