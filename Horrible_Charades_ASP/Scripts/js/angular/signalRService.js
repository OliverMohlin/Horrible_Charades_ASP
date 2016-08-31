/// <reference path="signalRService.js" />
(function () {
    "use strict";

    //Getting the existing module
    angular.module("mainContent")
        .service("signalRService", signalRService);

    function signalRService() {

        var self = this;
        self.time = 60;
        self.game = {};
        self.myTeam = {};
        //var vm = contentController;
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
        // Update the Local players index in games List of Teams 
        hub.client.updateMyTeam = function (index) {
            self.myTeam = index;
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
                signalRService.time += 15;
            }
            if (dicretion === "minus") {
                signalRService.time -= 15;
            }
        };

        //Write out and append new words to a charade in Pre-Charade(?)
        hub.client.InsertCharadeHTML = function (game, typeOfWord) {
            if (typeOfWord === "noun") {
                $("#charadeContainer").append("<div id='noun' style='display:inline''>" + game.CurrentCharade.Noun.Description + "</div>");
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

        hub.client.displayAlternatives = function (alternatives) {
            for (var i = 0; i < alternatives.length; i++) {
                var tmpstr = "<div id='" + i + "'></div><div id='myDiv" + i + "'> <ul>";
                for (var j = 0; j < alternatives[i].length; j++) {
                    tmpstr += "<li> <button name='" + alternatives[i][j].Description + i + "' data-ng-click='vm.hideDiv()'>" + alternatives[i][j].Description + "</button> </li>"
                };
                tmpstr += "</ul> </div> </br></br>";
                angular.element(document.getElementById('alternatives')).append($compile(tmpstr)($scope));
                //$("#alternatives").append(tmpstr);
            };
        };
     
        //hub.client.displayAlternatives = function (alternatives) {
        //    for (var i = 0; i < alternatives.length; i++) {
        //        var tmpstr = "<ul>";
        //        for (var j = 0; j < alternatives[i].length; j++) {
        //            tmpstr += "<li id='" + alternatives[i][j].Description + "'><input type='radio' id='" + alternatives[i][j].Description + "' name='selector'><label for='" + alternatives[i][j].Description + "'>" + alternatives[i][j].Description + "</label></input></li>";
        //        }
        //        tmpstr += "</ul></br></br>";
        //        $("#alternatives").append(tmpstr);
        //    }
        //};
        //$("#alternatives").click(function () {
        //    console.log("hiding div");
        //    var i = event.target.name[event.target.name.length - 1]
        //    $("#myDiv" + i).hide()
        //    var str = event.target.name.substring(0, event.target.name.length - 1);
        //    $("#" + i).append(str)
        //});

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
        });

    }
})();