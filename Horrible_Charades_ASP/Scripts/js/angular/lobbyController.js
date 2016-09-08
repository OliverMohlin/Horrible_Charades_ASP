(function () {
    "use strict";


    angular.module("mainContent")
        .controller("lobbyController", lobbyController, ['$scope', 'signalRService']);
        function lobbyController(signalRService, $scope) {

        var vm = this;
        var hub = $.connection.gameHub;

        vm.game = signalRService.game;
        vm.teamName = signalRService.teamName;

        vm.leaveLobby = function () {
            console.log("leaveLobby in lobbyController");
            hub.server.getRuleChanger(signalRService.game.GameCode);
        };

        $.connection.hub.start().done(function () {
            console.log("lobbyController loaded");
        });
    };
})();