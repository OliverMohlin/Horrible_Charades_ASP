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
            console.log("ctgController loaded and connected to GameHub");  // verkar köras 2 ggr just nu..
            console.log(vm);
        });

        vm.joinGame = function () {
            console.log("vm.joinGame initiated");
            hub.server.joinGame($("#GameCodeGuest").val(), $("#TeamName").val());
        };


    };
})();