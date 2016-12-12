(function () {
    "use strict";

    angular.module("mainContent")
        .controller("waitingRoomController", waitingRoomController);

    function waitingRoomController(signalRService) {

        var vm = this;
        var hub = $.connection.gameHub;
        vm.game = signalRService.game;
        vm.team = signalRService.team;

        $.connection.hub.start().done(function () {
            var gameCode = localStorage.getItem('gameCode');
            var teamName = localStorage.getItem('teamName');
            var connectionId = localStorage.getItem('connectionId');
            console.log(gameCode + ' | ' + teamName + ' | ' + connectionId);
            hub.server.reconnectToGame(teamName, gameCode, "/#/WaitingRoomActor");
        });

        $.connection.hub.disconnected(function () {
            setTimeout(function () {
                console.log("disconnected");
                $.connection.hub.start();
            }, 1000);
        });

        $.connection.hub.reconnected(function () {
            console.log("reconnected. Win!");
        });

        //Redirects to PreCharade View
        vm.redirectToPreCharade = function () {
            hub.server.redirectToPreCharade(vm.game.GameCode, vm.teamName);
        };
    }
})();