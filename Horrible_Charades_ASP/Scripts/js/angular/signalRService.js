/// <reference path="signalRService.js" />
(function () {
    "use strict";

    //Getting the existing module
    angular.module("mainContent")
        .service("signalRService", signalRService);

    function signalRService($compile) {

        var self = this;
        self.game = {};
        self.team = {};
        self.charadeTime = 60;

        //Saves connection in "hub"-variable
        var hub = $.connection.gameHub;                 

        //Submit Options
        self.submitOptions = function (rounds) {
            console.log("Submit options in signalR is called")
        }

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
        hub.client.setTeam = function (team, game, nextView) {
            self.team = team;
            hub.client.redirectToView(game, nextView);
        };

        //Redirects to next view
        hub.client.redirectToView = function (game, nextView) {
            self.game = game;
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
        // When a FunkUp/PowerUp is sent to increase or decrease Charade Time. 
        hub.client.affectCharadeTime = function (direction, game) {
            if (direction === "plus") {
                self.charadeTime += 15;
                $(".timer").text(10);
            }
            else {
                self.charadeTime -= 15;
                $(".timer").text(10);
            }
            self.game = game;
        };

        // Receives a call to reset the Timer in Clients Browsers.
        hub.client.resetTimer = function () {
            $(".timer").text(10);
        };

        hub.client.resetCharadeTimer = function () {
            self.charadeTime = 60;
        };

        hub.client.shuffleCharadeGameUpdate = function (game) {
            $(".timer").text(10);
            self.game = game;
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
                $(".timer").text(10);
            }
        };

        hub.client.displayAlternatives = function (alternatives) {
            var tmpstr = "";
            for (var i = 0; i < alternatives.length; i++) {
                tmpstr += "<div id='" + i + "' class='chosenAlt'></div><div id='myDiv" + i + "'> <ul>";
                for (var j = 0; j < alternatives[i].length; j++) {
                    tmpstr += "<li id='" + [i][j] + "'><button class='altButton' name='" +
                    alternatives[i][j].Description + i + "' onclick='checkBtn(event)'>" + alternatives[i][j].Description + "</button> </li>";
                };
                tmpstr += "</ul> </div> </br></br>";
            }
            $('#alternatives').append(tmpstr);
        };



        $.connection.hub.start().done(function () {                         //Opens connection to the Hub
        });

    }
})();