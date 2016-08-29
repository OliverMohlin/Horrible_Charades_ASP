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

        hub.client.displayMessage = function (message) {
            alert(message);
        };

        hub.client.updateGameState = function (game) {
            gameService.game = game;
        };

        hub.client.redirectToView = function (nextView) {
            window.location.href = nextView;
        };

        hub.client.debugMessage = function (message) {
            console.log(message);
        };

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

        hub.client.InsertModifierHTML = function (game, categoryID) {
            console.log("preparing to show Modifier");
            if (categoryID === 1) {
                console.log("printing funkUp");
                $("#funkUpContainer").append("<div id='funkUp' style='display:inline''>" + game.FunkUps[0] + "</div>");
                console.log("FunkUp printed");
                hub.client.updateGameState(game);
            }
            if (categoryID === 2) {
                console.log("printing PowerUp");
                $("#powerUpContainer").prepend("<div class='powerUp' style='display:inline'>" + game.PowerUps[0] + " " + "</div>");
                console.log("PowerUp printed");
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