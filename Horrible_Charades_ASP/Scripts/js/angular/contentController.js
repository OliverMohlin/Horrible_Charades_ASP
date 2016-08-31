(function () {
    "use strict";

    //Obtains the existing module
    angular.module("mainContent")
        .controller("contentController", contentController);

    function contentController(signalRService, $interval) {
        var vm = this;
        var hub = $.connection.gameHub; //Saves connection in "hub"-variable

        // Updates contentController to fit the locally persisted data in gameService. 
        vm.gameData = signalRService.game;
        vm.myTeam = signalRService.myTeam;
        vm.timeLeft = 3000;
        vm.promise;
        vm.time = signalRService.time;

        //Starts timer on CharadeActor
        vm.startTimer = function (time) {

            if (signalRService.game.GameState === 4) {
                $(".timer").text(time);
            } else {
                $(".timer").text(vm.time);
            }
            vm.promise = $interval(timer, 1000);
        };

        function timer() {
            vm.timeLeft = $(".timer").text();
            vm.timeLeft--;
            $(".timer").text(vm.timeLeft);
            if (vm.timeLeft <= 0) {
                $interval.cancel(vm.promise);
                if (signalRService.game.GameState === 4) {
                    vm.redirectToCharade();
                } else if (signalRService.game.GameState === 5) {
                    vm.pointCounter(0);

                }
            }
        }
        vm.stopTimer = function () {
            $interval.cancel(promise);
        };
        //Calls CreateGame function on Server-Side when CreateTeamHost is loaded
        vm.createGame = function () {
            hub.server.createGame();
        };

        //Calls JoinGame function on Server-Side when a teamName and GameCode is submitted in CreateTeamGuest
        vm.joinGame = function () {
            hub.server.joinGame($("#GameCode").val(), $("#TeamName").val());
        };

        //Calls StartCharade on Server-Side when a the host presses start
        vm.startCharade = function () {
            hub.server.startCharade(signalRService.game.GameCode);
            hub.server.getRuleChanger(signalRService.game.GameCode);
        };
        //Calls CreateTeam function on Server-Side when a teamName in CreateTeamHost is submitted
        vm.createTeam = function () {
            signalRService.game.GameCode = $("#GameCode").text();
            hub.server.createTeam(signalRService.game.GameCode, $("#TeamName").val());
        };

        //Redirects to nextView
        vm.redirectToPreCharade = function () {
            console.log("Redirecting to PreCharade");
            hub.server.redirectToPreCharade(signalRService.game.GameCode);
        };

        vm.redirectToCharade = function () {
            console.log("Redirecting to Charade");
            hub.server.redirectToCharade(signalRService.game.GameCode);
        };

        //Calls GetCharade function on Server-Side when PreCharadeActor is loaded
        vm.getNoun = function () {
            hub.server.getNoun(signalRService.game.GameCode);
        };

        //Calls shuffleCharade function on Server-side when "shufflecharade-button" is pressed by the actor
        vm.shuffleCharade = function () {
            $("#charadeContainer").html('');
            hub.server.shuffleCharade(signalRService.game.GameCode);
        };
        //Calls UpdateCharade function on Server-Side when "Get Adjective"-button is pressed
        vm.getAdjective = function () {
            hub.server.updateCharade("adjective", signalRService.game.GameCode);
        };

        //Calls UpdateCharade function on Server-Side when "Get Verb"-button is pressed
        vm.getVerb = function () {
            hub.server.updateCharade("verb", signalRService.game.GameCode);
        };

        // Receives a call to reset the Timer in Clients Browsers.
        hub.client.resetTimer = function (reset) {
            // Allt fungerar förutom att sätta tiden till 10 !!!
            $(".timer").text(reset);
            console.log("timeLeft");
            console.log(vm.timeLeft);
        };
        // Call GetRuleChanger on server-side to get RuleChangers from Database when "Start Game" button is pressed. 
        vm.getRuleChanger = function () {
            console.log("initiating getRuleChanger");
            hub.server.getRuleChanger(signalRService.game.GameCode);
        };
        
        // Sends FunkUp's towards the acting team when a matching button is pressed 
        vm.activateFunkUp = function (Id) {
            console.log("initiating activateFunkUp");
            console.log(Id);
            if (Id === 3) {
                signalRService.time -= 15;
            }
            if (Id === 4) {
                vm.getAdjective();
            }
            if (Id === 5) {
                vm.getVerb();
            }
        };

        // Sends FunkUp's towards the acting team when a matching button is pressed 
        vm.activatePowerUp = function (Id) {
            console.log("initiating activateFunkUp");
            console.log(Id);
            if (Id === 1) {
                signalRService.time += 15;
            }
            if (Id === 2) {
                vm.shuffleCharade();
            }
        };

        vm.getIncorrectAnswers = function () {
            hub.server.getIncorrectAnswers(signalRService.game.GameCode);
        };

        vm.pointCounter = function (timeLeft) {
            $interval.cancel(vm.promise);
            console.log("You're in pointcounter");
            hub.server.pointCounter(signalRService.game.GameCode, timeLeft);
        };
        //vm.printCharade = function () {
            
        //    for (var i = 0; i < gameService.game.CurrentCharade.Adjective.length; i++) {
        //        $("#charade").append("<li>" + gameService.game.CurrentCharade.Adjective[i].Description + "</li>");
        //    }

        //    $("#charade").append("<li>" + gameService.game.CurrentCharade.Noun.Description + "</li>");

        //    for (var i = 0; i < gameService.game.CurrentCharade.Verb.length; i++) {
        //        $("#charade").append("<li>" + gameService.game.CurrentCharade.Verb[i].Description + "</li>");
        //    }
        //};
        $.connection.hub.start().done(function () {                         //Opens connection to the Hub              
        });
    }

})();