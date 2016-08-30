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
            Team team = GameState.Instance.GetTeam(teamName, gameCode);

            if (team != null)
            {
                Clients.Caller.displayMessage("There is already a team in this game with that name");
            }
            else
            {
                Game game = GameState.Instance.CreateTeam(teamName, gameCode, Context.ConnectionId);
                if (game.Teams.Count == 1)
                {
                    Clients.Group(game.GameCode).updateGameState(game);
                    Clients.Caller.redirectToView("/#/LobbyHost");
                }
                else {
                    Clients.Group(game.GameCode).updateGameState(game);
                    Clients.Group(game.GameCode).pushToTeamList(teamName);
                    Clients.Caller.redirectToView("/#/LobbyGuest");
                }
    
            }
        }
        /// <summary>
        /// Shuffles and assigns which Team is going to do the charade and redirects the client
        /// </summary>
        /// <param name="gameCode"></param>
        public void startCharade(string gameCode)
        {
            Game game = GameState.Instance.GetGame(gameCode);

            GameState.Instance.AssignWhosTurn(game);

            Clients.Group(gameCode).updateGameState(game);
            Clients.Client(game.WhosTurn.ConnectionID).redirectToView("/#/WaitingRoomActor");
            Clients.Group(game.GameCode, game.WhosTurn.ConnectionID).redirectToView("/#/WaitingRoomOpponent");
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
        /// Redirects the client to PreCharadeActor and PreCharadeParticipant
        /// </summary>
        /// <param name="gameCode"></param>
        public void RedirectFromWaitingRoom(string gameCode)
        {
            Game game = GameState.Instance.GetGame(gameCode);
            Clients.Group(gameCode).updateGameState(game);

            //game = GameState.Instance.GiveAllTeamsRuleChanger(Context.ConnectionId, gameCode);
            Clients.Client(game.WhosTurn.ConnectionID).redirectToView("/#/PreCharadeActor");
            Clients.Group(game.GameCode, game.WhosTurn.ConnectionID).redirectToView("/#/PreCharadeParticipant");
        }

        /// <summary>
        /// Gets a Noun from Database Table Nouns and Converts it into a Charade.
        ///  Pushes to Client-side
        /// </summary>
        public void GetNoun(string gameCode)
        {
            Clients.Caller.debugMessage("initiating getNoun on serverside");
            Game game = GameState.Instance.GetNoun(gameCode);
            Clients.Caller.debugMessage($"Have found a noun: {game.CurrentCharade.Noun} and updated serverside Game");
            Clients.Group(game.GameCode).InsertCharadeHTML(game, "noun");
        }

        public void UpdateCharade(string typeOfWord, string gameCode)
        {
            Clients.Caller.debugMessage("initiating UpdateCharade on serverside");
            if (typeOfWord == "adjective")
            {
                Clients.Caller.debugMessage("starting to find adjective");
                Game game = GameState.Instance.GetAdjective(gameCode);
                Clients.Caller.debugMessage($"Have found an adjective: {game.CurrentCharade.Adjective[0]} and updated serverside Game");
                Clients.Group(game.GameCode).InsertCharadeHTML(game, "adjective");
            }
            if (typeOfWord == "verb")
            {
                Game game = GameState.Instance.GetVerb(gameCode);
                Clients.Group(game.GameCode).InsertCharadeHTML(game, "verb");
            }
        }

        public void GetRuleChanger(string gameCode)
        {
            int index = 5;
            Clients.Caller.debugMessage("initiating getModifier on serverside");
            Game game = GameState.Instance.GiveAllTeamsRuleChanger(Context.ConnectionId, gameCode,out index);
            Clients.Caller.debugMessage($"Found a Modifier and updated serverside Game");
            Clients.Caller.updateMyTeam(index);
            Clients.Caller.debugMessage(game);
            Clients.Group(gameCode).updateGameState(game);
        }

        public void GetIncorrectAnswers(string gameCode)
        {
            Game game = GameState.Instance.GetGame(gameCode);
            List<List<Word>> inCorrectAnswers = new List<List<Word>>();
            inCorrectAnswers = GameState.Instance.GetIncorrectAnswers(gameCode);

            Clients.Caller.DisplayAlternatives(inCorrectAnswers);
        }
        public void PointCounter(string gameCode, int timeLeft)
        {
            //Kolla så att
            Game game = GameState.Instance.AssignPoints(gameCode, timeLeft, Context.ConnectionId);
            Clients.Group(game.GameCode).updateGameState(game);
            Clients.Group(game.GameCode).redirectToView("/#/Score");
        }
    }
}