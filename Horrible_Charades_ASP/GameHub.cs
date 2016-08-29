using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Horrible_Charades_ASP.Database;
using Horrible_Charades_ASP.Models;
using System.Threading;

namespace Horrible_Charades_ASP
{
    public class GameHub : Hub
    {
        DatabaseUtils _dbUtils = new DatabaseUtils(
            new Models.CharadeContext());
        Charade _charade = new Charade();

        /// <summary>
        /// This function calls the method Hello on Client-Side, with something to write out
        /// </summary>
        /// <param name="textToWrite"></param>
        public void Hello(string textToWrite)
        {
            Clients.All.hello(textToWrite);
        }

        /// <summary>
        /// This function is called when a user clicks "New Game"
        /// </summary>
        public void CreateGame()
        {
            Game game = GameState.Instance.CreateGame();
            Clients.Caller.printGameCode(game); //Todo: skapa printGameCode på klientsidan
        }
        /// <summary>
        /// This function calls the method InsertCharadeHTML on Client-Side, which inserts provided string in a <div>-tag
        /// </summary>
        /// <param name="htmlToWrite"></param>
        public void InsertCharadeHTML(string htmlToWrite)
        {
            Clients.All.insertCharadeHTML(htmlToWrite);
        }

        /// <summary>
        /// Creates a new team if the device don't have a team. 
        /// </summary>
        /// <param name="teamName"></param>
        public void CreateTeam(string gameCode, string teamName) //To-do: validera team-name
        {
            Clients.Caller.foo($"code:{gameCode} tam:{teamName}" );
            Team team = GameState.Instance.GetTeam(teamName, gameCode);
            Clients.Caller.foo($"Possible team retrieved from DB");

            if (team != null)
            {
                //Clients.Group(team.GameCode).UpdateGameState(GameState.Instance.GetGame(gameCode));
                //Clients.All.teamsJoined(game);
                Clients.Caller.displayMessage("There is already a team in this game with that name");
            }
            else
            {
                Clients.Caller.foo($"creating team");

                Game game = GameState.Instance.CreateTeam(teamName, gameCode, Context.ConnectionId);
                Clients.Caller.foo($"team created");

                if (game.Teams.Count == 1)
                {
                    Clients.Caller.foo($"first team in game");
                    Clients.Group(game.GameCode).updateGameState(game, "/#/LobbyHost");
                    //Clients.Caller.redirectToView("/#/LobbyHost");
                }
                else
                {
                    Clients.Caller.foo($"already 1 team in game");
                    Clients.Group(game.GameCode).updateGameState(game, "/#/LobbyGuest");
                    //Clients.Caller.redirectToView("/#/LobbyGuest");
                }

            }
        }
        /// <summary>
        /// Takes in a gameCode and TeamName from a joining team, looks for a Game with matching gameCode and adds the team into the game.
        /// </summary>
        /// <param name="gameCode"></param>
        /// <param name="teamName"></param>
        public void JoinGame(string gameCode, string teamName)
        {
            Game game = GameState.Instance.GetGame(gameCode);
            if (game == null)
            {
                //Clients.Caller.NoGameExist(false);
                Clients.Caller.DisplayMessage("No such game exist. Revise your GameCode");
            }
            else
            {
                CreateTeam(gameCode, teamName);
                //Clients.Caller.NoGameExist(true);
            }

        }
        /// <summary>
        /// Gets a Noun from Database Table Nouns and Converts it into a Charade.
        ///  Pushes to Client-side
        /// </summary>
        public void GetNoun(string gameCode)
        {
            Clients.Caller.foo("initiating getNoun on serverside");
            Game game = GameState.Instance.GetNoun(gameCode);
            Clients.Caller.foo($"Have found a noun: {game.CurrentCharade.Noun}");
            //Game game = GameState.Instance.GetGame(gameCode);
            //Clients.Caller.foo($"Have found a game: {game} - ready to update gam");
            //game.CurrentCharade.Noun = noun;
            Clients.Caller.foo("Game updated on serverside");
            // Ska inte skickas till All - enbart Aktören för charaden.
            Clients.All.InsertCharadeHTML(game, "noun");
        }

        public void UpdateCharade(string typeOfWord, string gameCode)
        {

            if (typeOfWord == "adjective")
            {
                string adjective = GameState.Instance.GetAdjective(gameCode);
                Clients.All.InsertCharadeHTML(adjective, "adjective");
            }
            if (typeOfWord == "verb")
            {
                string verb = GameState.Instance.GetVerb(gameCode);
                Clients.All.InsertCharadeHTML(verb, "verb");
            }
        }
        #region gamla ordhämtningar
        /// <summary>
        /// Hämtar ett slumpat Substantiv från Databasen
        /// Hämtar även 3 felaktiga alternativ för gissande
        /// </summary>
        //public void GetNoun(string gameCode)
        //{
        //    string noun = GameState.Instance.GetNoun(gameCode);

        //    //Clients.All.incorrectGuesses(tmpList);
        //    Clients.All.InsertCharadeHTML(noun, "noun");
        //}
        /// <summary>
        /// Hämtar slumpat Adjective från databasen
        ///  + 3 felaktiga alternativ för gissande
        /// </summary>
        //public void GetAdjective()
        //{
        //    //string adjective = GameState.Instance.GetAdjective();
        //    ////Clients.All.incorrectGuesses(tmpList);
        //    //Clients.All.InsertCharadeHTML(adjective, "adjective");
        //}
        ///// <summary>
        ///// Hämtar slumpat verb från databasen
        /////  + 3 felaktiga alternativ för gissande
        ///// </summary>
        //public void GetVerb()
        //{
        //    string verb = GameState.Instance.GetVerb();
        //    //Clients.All.incorrectGuesses(tmpList);
        //    Clients.All.InsertCharadeHTML(verb, "verb");
        //}
        #endregion

    }
}