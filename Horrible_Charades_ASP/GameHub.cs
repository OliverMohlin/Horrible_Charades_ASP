using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace Horrible_Charades_ASP
{
    public class GameHub : Hub
    {
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
    }
}