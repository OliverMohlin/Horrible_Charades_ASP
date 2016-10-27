
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
        /// This function is called when a user clicks "New Game"
        /// </summary>
        public void CreateGame()
        {
            Game game = GameState.Instance.CreateGame();
            Clients.Caller.redirectToView(game, "/#/CreateTeamHost");
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
        /// Creates a new team in the specific fame
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
                Game game = GameState.Instance.CreateTeam(teamName, gameCode, Context.ConnectionId, out team);
                if (game.Teams.Count == 1)
                {
                    Clients.Caller.setTeam(team, game, "/#/LobbyHost");
                }
                else
                {
                    Clients.Group(game.GameCode).pushToTeamList(teamName);
                    Clients.Caller.setTeam(team, game, "/#/LobbyGuest");
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
            string trimmedCode = gameCode.ToUpper().Trim();
            Game game = GameState.Instance.GetGame(trimmedCode);
            if (game == null)
                Clients.Caller.DisplayMessage("No such game exist. Revise your GameCode");
            else
                CreateTeam(trimmedCode, teamName);
        }

        /// <summary>
        /// Serverside activation of Getting RuleChangers. Called when leaving Waiting room.
        /// </summary>
        /// <param name="gameCode"></param>
        public void GetRuleChanger(string gameCode, int roundsToPlay)
        {
            //TODO: Flytta in hela getRuleChanger in i StartCharade(?) och flytta vissa grejer till start charade från GiveAllTeamsRuleChanger
            Game game = GameState.Instance.GiveAllTeamsRuleChanger(Context.ConnectionId, gameCode, roundsToPlay);
            StartCharade(game);
        }

        /// <summary>
        /// Shuffles and assigns which Team is going to do the charade and redirects the client
        /// </summary>
        /// <param name="gameCode"></param>
        public void StartCharade(Game game)
        {
            Clients.Client(game.WhosTurn.ConnectionID).redirectToView(game, "/#/WaitingRoomActor");
            Clients.Group(game.GameCode, game.WhosTurn.ConnectionID).redirectToView(game, "/#/WaitingRoomOpponent");

        }

        /// <summary>
        /// Redirects the client to PreCharadeActor and PreCharadeParticipant
        /// </summary>
        /// <param name="gameCode"></param>
        /// <param name="teamName"></param>
        /// 

        public void RedirectToPreCharade(string gameCode, string teamName)
        {
            Game game = GameState.Instance.GetGame(gameCode);
            //Team myTeam = game.Teams.FirstOrDefault(t => t.Name == teamName);
            game.GameState = 4;
            Clients.Client(game.WhosTurn.ConnectionID).setTeam(game.WhosTurn, game, "/#/PreCharadeActor");
            foreach (Team team in game.Teams)
            {
                if (team != game.WhosTurn)
                {
                    Clients.Client(team.ConnectionID).setTeam(team, game, "/#/PreCharadeParticipant");
                }

            }
        }

        /// <summary>
        /// Updates the current charade serverSide with either adjective or verb.
        /// Called upon when charade:Opponenet uses a the respective FunkUp
        /// </summary>
        /// <param name="typeOfWord"></param>
        /// <param name="gameCode"></param>
        public void UpdateCharade(string typeOfWord, string gameCode, int ruleChangerId)
        {
            if (typeOfWord == "adjective")
            {
                Game game = GameState.Instance.GetAdjective(gameCode, Context.ConnectionId, ruleChangerId);
                Clients.Group(game.GameCode).InsertCharadeHTML(game, "adjective");
                // TODO: behöver inte vara eget hubanrop. kan stoppas in i signalRservice(insertcharadeHtml)
            }

            if (typeOfWord == "verb")
            {
                Game game = GameState.Instance.GetVerb(gameCode, Context.ConnectionId, ruleChangerId);
                Clients.Group(game.GameCode).InsertCharadeHTML(game, "verb");
            }
        }

        /// <summary>
        /// Shuffles the active Charades. Activated when Charade:Actor uses the respectrive PowerUp
        /// </summary>
        /// <param name="gameCode"></param>
        public void ShuffleCharade(string gameCode, int ruleChangerId)
        {
            Game game = GameState.Instance.ShuffleCharade(gameCode, Context.ConnectionId, ruleChangerId);
            Clients.Caller.InsertCharadeHTML(game, "noun");
            Clients.Caller.InsertCharadeHTML(game, "adjective");
            Clients.Caller.InsertCharadeHTML(game, "verb");
            Clients.Group(gameCode).shuffleCharadeGameUpdate(game);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameCode"></param>
        /// <param name="direction"></param>
        public void AffectCharadeTime(string gameCode, string direction, int ruleChangerId)
        {
            Game game = GameState.Instance.RemoveRuleChanger(gameCode, Context.ConnectionId, ruleChangerId);
            Clients.Group(gameCode).affectCharadeTime(direction, game);
        }


        public void RedirectToCharade(string gameCode, string teamName)
        {
            Game game = GameState.Instance.GetGame(gameCode);
            Team myTeam = game.Teams.FirstOrDefault(t => t.Name == teamName);
            game.GameState = 5;

            if (myTeam.ConnectionID == game.WhosTurn.ConnectionID)
            {
                Clients.Caller.redirectToView(game, "/#/CharadeActor");
            }
            else
            {
                Clients.Caller.redirectToView(game, "/#/CharadeParticipant");
            }
        }

        /// <summary>
        /// Gets a Noun from Database Table Nouns and Converts it into a Charade.
        ///  Pushes to Client-side
        /// </summary>
        public void GetNoun(string gameCode)
        {
            Game game = GameState.Instance.GetGame(gameCode);
            Clients.Group(gameCode).InsertCharadeHTML(game, "noun");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameCode"></param>
        public void GetIncorrectAnswers(string gameCode)
        {
            Game game = GameState.Instance.GetGame(gameCode);
            List<List<Word>> inCorrectAnswers = new List<List<Word>>();
            inCorrectAnswers = GameState.Instance.GetIncorrectAnswers(gameCode);
            Clients.Caller.DisplayAlternatives(inCorrectAnswers);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameCode"></param>
        /// <param name="timeLeft"></param>
        public void CalculateScore(string gameCode, int timeLeft)
        {
            Game game = GameState.Instance.AssignPoints(gameCode, timeLeft, Context.ConnectionId, 200);

            Clients.Group(game.GameCode).redirectToView(game, "/#/Score");
        }
        public void CalculateScoreP(string gameCode, int timeLeft, string guess) //SubmitGuess(?)
        {
            GameState.Instance.AssignPoints(gameCode, timeLeft, Context.ConnectionId, 70, guess);
            //Todo: Skriva att den har blivit submittad
            //Clients.Caller.guessSubmitted();
        }

        public void RedirectToTotalScore(string gameCode)
        {
            Game game = GameState.Instance.GetGame(gameCode);
            game.GameState = 7;
            Clients.Group(gameCode).redirectToView(game, "/#/TotalScore");
        }

        public void playAgain(string gameCode)
        {
            Game game = GameState.Instance.GetGame(gameCode);
             Clients.Caller.redirectToView(game, "/#/MainMenu");
        }

        public void StartNextCharade(string gameCode)
        {
            Game game = GameState.Instance.GetGame(gameCode);

            game = GameState.Instance.EmptyTurnScores(game.GameCode);
            game = GameState.Instance.PrepareNewRound(game, Context.ConnectionId);

            if (game.Round == game.RoundsToPlay)
            {
                game = GameState.Instance.AssignWinner(game.GameCode);
                Clients.Group(gameCode).redirectToView(game, "/#/GameOver");
            }
            else
            {
                Clients.Group(gameCode, game.WhosTurn.ConnectionID).redirectToView(game, "/#/WaitingRoomOpponent");
                Clients.Group(gameCode).resetCharadeTimer();
                Clients.Client(game.WhosTurn.ConnectionID).redirectToView(game, "/#/WaitingRoomActor");
            }

        }
    }
}