(function () {
    "use strict";

    //Obtains the existing module
    angular.module("mainContent")
        .controller("contentController", contentController);

    function contentController(signalRService, $interval, $timeout, $compile, $scope) {
        var vm = this;
        var hub = $.connection.gameHub; //Saves SignalR connection in "hub"-variable

        // Updates contentController to fit the locally persisted data in signalRService. 
        vm.gameData = signalRService.game;
        vm.myTeam = signalRService.myTeam;
        //vm.timeLeft = 65;
        //vm.promise;
        vm.hide = false;
        //vm.time = signalRService.time;
        vm.guessed = false;
        vm.alternatives = ["a", "b", "c"];

        //Starts timer on CharadeActor
        vm.startTimer = function (time) {

            if (signalRService.game.GameState === 4) {
                $(".timer").text(10);
            } else if (signalRService.game.GameState === 5) {
                $(".timer").text(6000);
            } else if (signalRService.game.GameState === 6){
                $(".timer").text(5);
            } else {
                $(".timer").text(5);
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
                }
                else if (signalRService.game.GameState === 5) {
                    vm.pointCounter(0);
                    vm.playTut();
                }
                else if (signalRService.game.GameState === 6) {
                    hub.server.redirectToTotalScore(signalRService.game.GameCode);
                } else if (signalRService.game.GameState === 7) {
                    vm.leaveLobby(signalRService.game.GameCode);
                }
                else {
                    alert("Your gameState is wrong. Swish $('300') for netflix and chill with Bernie Lo <3 ");
                }
            }
        };


        // Called upon when redirecting to PreCharade or Charade view. Starts the Timer for the View.
        //hub.client.startTimer = function () {
        //    console.log("calling vm.startTimer");
        //    vm.stopTimer();
        //    vm.startTimer();
        //};

        // Stops the timer. Called from startTimer.
        //vm.stopTimer = function () {
        //    $interval.cancel(vm.promise);
        //};

        //Calls StartCharade on Server-Side when a the host presses start
        hub.client.startCharade = function () {
            hub.server.startCharade(signalRService.game.GameCode);
        };

        // Redirects to Charade View
        vm.redirectToCharade = function () {
            console.log("Redirecting to Charade");
            console.log(signalRService.teamName);
            hub.server.redirectToCharade(signalRService.game.GameCode, signalRService.teamName);
        };

        // Receives a call to reset the Timer in Clients Browsers.
        hub.client.resetTimer = function (reset) {
            // Allt fungerar förutom att sätta tiden till 10 !!!
            $(".timer").text(reset);
            console.log("timeLeft");
            console.log(vm.timeLeft);
        };

        //// Sends FunkUp's towards the acting team when a matching button is pressed 
        //vm.activateFunkUp = function (Id) {
        //    console.log("initiating activateFunkUp");
        //    console.log(Id);
        //    if (Id === 3) {
        //        hub.server.affectCharadeTime(signalRService.game.GameCode, "minus");
        //    }
        //    if (Id === 4) {
        //        hub.server.updateCharade("adjective", signalRService.game.GameCode);

        //    }
        //    if (Id === 5) {
        //        hub.server.updateCharade("verb", signalRService.game.GameCode);
        //    }
        //};



        vm.getIncorrectAnswers = function () {
            hub.server.getIncorrectAnswers(signalRService.game.GameCode);
        };

        vm.pointCounter = function () {
            $interval.cancel(vm.promise);
            console.log("You're in pointcounter");
            var timeLeft = $(".timer").text();
            hub.server.pointCounter(signalRService.game.GameCode, timeLeft);
        };

        vm.printCharade = function () {

            for (var i = 0; i < signalRService.game.CurrentCharade.Adjective.length; i++) {
                $("#charade").append("<li>" + signalRService.game.CurrentCharade.Adjective[i].Description + " </li>");
            }

            $("#charade").append("<li>" + signalRService.game.CurrentCharade.Noun.Description + " </li>");

            for (i = 0; i < signalRService.game.CurrentCharade.Verb.length; i++) {
                $("#charade").append("<li>" + signalRService.game.CurrentCharade.Verb[i].Description + "</li>");
            }
        };

        hub.client.displayAlternatives = function (alternatives) {
            var tmpstr = "";
            for (var i = 0; i < alternatives.length; i++) {
                tmpstr += "<div id='" + i + "' class='chosenAlt'></div><div id='myDiv" + i + "'> <ul>";

                for (var j = 0; j < alternatives[i].length; j++) {
                    tmpstr += "<li id='" + [i][j] + "'><button class='altButton' name='" + alternatives[i][j].Description + i + "' onclick='checkBtn(event)'>" + alternatives[i][j].Description + "</button> </li>"
                };

                tmpstr += "</ul> </div> </br></br>";

            }
            $('#alternatives').append(tmpstr);
        };

        vm.submitGuess = function () {
            console.log("Inne i submit guess");
            vm.guessed = true;
            var guess = $("#hidden").text() + " ";//Lägger vi till fler ord måste vi lägga till en splitchar
            $("#submit").append(" " + guess);
            var timeLeft = $(".timer").text();
            console.log(timeLeft);
            hub.server.calculateScoreP(signalRService.game.GameCode, timeLeft, guess);
            console.log("efter calculateScoreP");
            //hub.server.calculateScoreP(signalRService.game.GameCode);
        };

        vm.showTeams = function () {
            console.log("inne i showTeams", signalRService.game.Teams);

            //hub.server.getTeams() 
        };

        vm.playKnappJoin = function () {
            var audio = new Audio("sounds/Knappjoin.mp3");
            audio.play();
        };

        vm.playKnappvalj = function () {
            var audio = new Audio("sounds/Knappvalj.mp3");
            audio.play();
        };

        vm.playJubel = function () {
            var audio = new Audio("sounds/Jubel.mp3");
            audio.play();
        };

        vm.playLightsThunder = function () {
            var audio = new Audio("sounds/lightsthunder.mp3");
            audio.play();
        };

        vm.playTickTack = function () {
            var audio = new Audio("sounds/Ticktack.mp3");
            audio.play();
        };

        vm.playTut = function () {
            var audio = new Audio("sounds/Tut.mp3");
            audio.play();
        };

        $.connection.hub.start().done(function () {                         //Opens connection to the Hub              
        });
    };


})();