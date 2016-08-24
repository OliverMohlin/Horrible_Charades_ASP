using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Horrible_Charades_ASP.Database;
using Horrible_Charades_ASP.Models;

namespace Horrible_Charades_ASP
{
    public class GameHub : Hub
    {
        // TODO: Kolla varför GameHub säger att _dbUtils alltid kommer att vara null ?!
        DatabaseUtils _dbUtils = new DatabaseUtils(
            new Models.CharadeContext());

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
        public void createTeam(string teamName) //To-do: validera team-name
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
        /// <summary>
        /// Gets a Noun from Database Table Nouns and Converts it into a Charade.
        ///  Pushes to Client-side
        /// </summary>
        public void GetCharade()
        {
            Word noun = _dbUtils.GetNoun();
            Charade charade = new Charade(noun.Description);
            Clients.All.printCharade(charade);
        }
        /// <summary>
        /// Hämtar ett slumpat Substantiv från Databasen
        /// Hämtar även 3 felaktiga alternativ för gissande
        /// </summary>
        public void GetNoun()
        {
            var noun = _dbUtils.GetNoun();
            List<string> tmpList = _dbUtils.GetIncorrectAnswers(noun);
            Clients.All.incorrectGuesses(tmpList);
            Clients.All.hello(noun.Description);
        }
        /// <summary>
        /// Hämtar slumpat Adjective från databasen
        ///  + 3 felaktiga alternativ för gissande
        /// </summary>
        public void GetAdjective()
        {
            var adjective = _dbUtils.GetAdjective();
            List<string> tmpList = _dbUtils.GetIncorrectAnswers(adjective);
            Clients.All.incorrectGuesses(tmpList);
            Clients.All.hello(adjective.Description);
        }
        /// <summary>
        /// Hämtar slumpat verb från databasen
        ///  + 3 felaktiga alternativ för gissande
        /// </summary>
        public void GetVerb()
        {
            var verb = _dbUtils.GetVerb();
            List<string> tmpList = _dbUtils.GetIncorrectAnswers(verb);
            Clients.All.incorrectGuesses(tmpList);
            Clients.All.hello(verb.Description);
        }
    }
}