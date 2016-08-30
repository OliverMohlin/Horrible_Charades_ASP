(function () {
    "use strict";

    //Getting the existing module
    angular.module("mainContent")
        .service("signalRService", signalRService);

    function signalRService(gameService) {

        var self = this;
        var hub = $.connection.gameHub;                 //Saves connection in "hub"-variable
        hub.client.hello = function (textToWrite) {     //Assigns hello() to client
            $("#result").append("<li>" + textToWrite + "</li>");
        };

        //Write out the GameCode in CreateTeamHost
        hub.client.printGameCode = function (game) {
            $("#GameCode").append(game.GameCode);
        };
        //Write out a message
        hub.client.displayMessage = function (message) {
            alert(message);
        };
        //Updates game on clientSide
        hub.client.updateGameState = function (game) {
            gameService.game = game;
        };
        //Redirects to next view
        hub.client.redirectToView = function (nextView) {
            window.location.href = nextView;
        };
        //Use this to print out a message to console.log
        hub.client.debugMessage = function (message) {
            console.log(message);
        };
        //Show join game for everbody in the game
        hub.client.pushToTeamList = function (teamName) {
            $("#teamList").append("<li>" + teamName + "</li>");
        };

        //Write out and append new words to a charade in Pre-Charade(?)
        hub.client.InsertCharadeHTML = function (game, typeOfWord) {
            console.log("preparing to Print new Word");
            if (typeOfWord === "noun") {
                $("#charadeContainer").append("<div id='noun' style='display:inline''>" + game.CurrentCharade.Noun + "</div>");
                hub.client.updateGameState(game);
            }
            if (typeOfWord === "adjective") {
                console.log("printing adjective");
                $("#noun").prepend("<div class='adjective' style='display:inline'>" + game.CurrentCharade.Adjective[0] + " " + "</div>");
                console.log("adjective printed");
                hub.client.updateGameState(game);
            }
            if (typeOfWord === "verb") {
                if ($('.verb').length) {
                    $("#noun").append("<div class='verb' style='display:inline'>" + " while " + game.CurrentCharade.Verb[1] + "</div>");
                    hub.client.updateGameState(game);
                }
                else {
                    $("#noun").append("<div class='verb' style='display:inline'>" + "  " + game.CurrentCharade.Verb[0] + "</div>");
                    hub.client.updateGameState(game);
                }
            }
        };
        $("#charade").onload = function () {
            console.log("initiating getNoun");
            hub.server.GetNoun(gameService.gameCode);
        };

        hub.client.InsertRuleChangerHTML = function (game, type) {
            console.log("preparing to show RuleChanger");
            if (type === "PowerUp") {
                console.log("printing PowerUp");
                $("#powerUpContainer").append("<div class='powerUp' style='display:inline''>" + game.PowerUps[0].Description + "</div>");
                console.log(game.PowerUps[0]);
                hub.client.updateGameState(game);
            }
            if (type === "FunkUp") {
                console.log("printing FunkupUp");
                $("#funkUpContainer").prepend("<div class='funkUp' style='display:inline'>" + game.FunkUps[0].Description + " " + "</div>");
                console.log(game.FunkUps[0]);
                hub.client.updateGameState(game);
            }
        };

        //Adds an adjective to a charade
        $("#getAdjectiveButton").click(function () {
            hub.server.getAdjective();
        });
        //Adds a verb to a charade
        $("#getVerbButton").click(function () {
            hub.server.getVerb();
        });

        $.connection.hub.start().done(function () {                         //Opens connection to the Hub
            hub.server.hello("Welcome to Horrible Charades");               //Calls hello() from Hub
        });

    }
})();