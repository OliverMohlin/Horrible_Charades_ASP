(function () {
    "use strict";

    //Obtains the existing module
    angular.module("mainContent")
        .controller("contentController", contentController);

    function contentController(signalRService, $interval, $timeout, $compile, $scope) {
        var vm = this;
        var hub = $.connection.gameHub; //Saves SignalR connection in "hub"-variable

        vm.gameData = signalRService.game;
        vm.myTeam = signalRService.myTeam;


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