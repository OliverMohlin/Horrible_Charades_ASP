/// <reference path="signalRService.js" />
(function () {
    "use strict";

    //Getting the existing module
    angular.module("mainContent")
        .service("signalRService", signalRService);

    function signalRService($compile) {

        var self = this;
        self.timeLeft = 60;
        self.game = {};
        self.teamName;
        self.charadeTime = 60;

        var hub = $.connection.gameHub;                 //Saves connection in "hub"-variable

        //Write out the GameCode in CreateTeamHost
        hub.client.printGameCode = function (game) {
            $("#GameCode").append(game.GameCode);
        };
        //Write out a message in Alert Popup
        hub.client.displayMessage = function (message) {
            alert(message);
        };
        //Updates game on clientSide
        hub.client.updateGameState = function (game) {
            self.game = game;
        };
        //Updates this players teamName
        hub.client.setTeamName = function (teamName) {
            self.teamName = teamName;
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
            $(".teamList").append("<li class='lobby-teamList'>" + teamName + "</li>");
        };
        hub.client.affectCharadeTime = function (direction) {
            console.log(direction);
            if (direction === "plus") {
                self.charadeTime += 15;
            }
            if (dicretion === "minus") {
                self.charadeTime -= 15;
            }
        };

        // Receives a call to reset the Timer in Clients Browsers.
        hub.client.resetTimer = function () {
            console.log("SignalRService changing timeLeft to 10");
            $(".timer").text(10);
        };

        //Write out and append new words to a charade in Pre-Charade(?)
        hub.client.InsertCharadeHTML = function (game, typeOfWord) {
            if (typeOfWord === "noun") {
                $(".charadeContainer").html("<div id='noun' style='display:inline''>" + game.CurrentCharade.Noun.Description + "</div>");
                hub.client.updateGameState(game);
            }
            if (typeOfWord === "adjective") {
                $(".adjective").remove();

                for (var i = 0; i < game.CurrentCharade.Adjective.length; i++) {
                    $("#noun").prepend("<div class='adjective' style='display:inline'>" + game.CurrentCharade.Adjective[i].Description + " " + "</div>");
                }
                hub.client.updateGameState(game);
            }
            if (typeOfWord === "verb") {

                $(".verb").remove();

                for (i = 0; i < game.CurrentCharade.Verb.length; i++) {

                    if (i === 0) {
                        $("#noun").append("<div class='verb' style='display:inline'>" + "  " + game.CurrentCharade.Verb[i].Description + "</div>");

                    } else {
                        $("#noun").append("<div class='verb' style='display:inline'>" + " while " + game.CurrentCharade.Verb[i].Description + "</div>");

                    }
                }
                hub.client.updateGameState(game);

            }
        };


        $("#charade").onload = function () {
            console.log("initiating getNoun");
            hub.server.GetNoun(gameService.gameCode);
        };

        $(".led").click(function () {
            alert("jQuery-click works contentController");
        });

        //Adds an adjective to a charade
        $("#getAdjectiveButton").click(function () {
            hub.server.getAdjective();
        });
        //Adds a verb to a charade
        $("#getVerbButton").click(function () {
            hub.server.getVerb();
        });

        $.connection.hub.start().done(function () {                         //Opens connection to the Hub
        });

    }
})();