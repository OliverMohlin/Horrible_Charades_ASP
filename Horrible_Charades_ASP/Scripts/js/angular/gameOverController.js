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

        //Redirects to PreCharade View
        //vm.redirectToPreCharade = function () {
        //    console.log(vm.game);
        //    hub.server.redirectToPreCharade(vm.game.GameCode, vm.teamName);
        //};
    };
})();