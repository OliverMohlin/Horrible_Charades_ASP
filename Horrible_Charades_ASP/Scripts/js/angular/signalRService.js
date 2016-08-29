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
        }

        //Write out and append new words to a charade in Pre-Charade(?)
        hub.client.InsertCharadeHTML = function (game, typeOfWord) {
            if (typeOfWord === "noun") {
                console.log("printing noun");
                $("#charadeContainer").append("<div id='noun' style='display:inline''>" + game.CurrentCharade.Noun + "</div>");
            }
            if (typeOfWord === "adjective") {
                $("#noun").prepend("<div class='adjective' style='display:inline'>" + charadeWord + " " + "</div>");
                console.log("printing adjective");
            }

            if (typeOfWord === "verb") {
                if ($('.verb').length) {
                    $("#noun").append("<div class='verb' style='display:inline'>" + " while " + charadeWord + "</div>");
                }
                else {
                    $("#noun").append("<div class='verb' style='display:inline'>" + "  " + charadeWord + "</div>");
                }
            }
        };
            $("#charade").onload = function () {
            console.log("initiating getNoun");
            hub.server.GetNoun(gameService.gameCode);
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