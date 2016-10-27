(function () {
    "use strict";

    angular.module("mainContent")
        .controller("gameOverController", gameOverController);

    function gameOverController(signalRService) {

        var vm = this;
        var hub = $.connection.gameHub;
        vm.game = signalRService.game;

        $.connection.hub.start().done(function () {
        });

        vm.playAgain = function () {
            hub.server.playAgain(vm.game.GameCode);
        }

    };
})();