(function () {
    "use strict";


    angular.module("mainContent")
        .controller("lobbyController", lobbyController, ['$scope', 'signalRService']);
        function lobbyController(signalRService, $scope) {

        var vm = this;
        var hub = $.connection.gameHub;

        vm.game = signalRService.game;
        //vm.teamName = signalRService.teamName;
        vm.team = signalRService.team;

        vm.leaveLobby = function () {
            hub.server.getRuleChanger(vm.game.GameCode);
        };

        $.connection.hub.start().done(function () {
        });
    };
})();