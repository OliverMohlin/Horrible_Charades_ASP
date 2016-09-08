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