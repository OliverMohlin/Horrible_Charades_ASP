(function () {
    "use strict";

    angular.module("mainContent")
        .controller("waitingRoomController", waitingRoomController);

    function waitingRoomController(signalRService) {

        var vm = this;
        var hub = $.connection.gameHub;
        vm.game = signalRService.game;

        $.connection.hub.start().done(function () {
            console.log("waitingRoomController loaded and connected to GameHub");
            console.log(vm);
        });

        //Redirects to PreCharade View
        vm.redirectToPreCharade = function () {
            console.log("Redirect to pre charade from waitingRoomController");
            hub.server.redirectToPreCharade(signalRService.game.GameCode, signalRService.teamName);
        };
    };
})();