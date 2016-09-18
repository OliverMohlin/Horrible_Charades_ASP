(function () {
    "use strict";

    //obtains existing module
    angular.module("mainContent")
        .controller("ctgController", ctgController);

    function ctgController(signalRService) {

        var vm = this;
        var hub = $.connection.gameHub;
        vm.game = signalRService.game;

        $.connection.hub.start().done(function () {
        });

        vm.pressEnter = function (keyEvent) {
            if (keyEvent.which === 13)
                vm.joinGame();
        };

        vm.joinGame = function () {
            hub.server.joinGame($("#GameCodeGuest").val(), $("#TeamName").val());
        };


    }
})();