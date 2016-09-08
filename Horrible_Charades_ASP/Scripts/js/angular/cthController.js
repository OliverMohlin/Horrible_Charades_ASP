(function () {
    "use strict";

    //obtains existing module
    angular.module("mainContent")
        .controller("cthController", cthController);

    function cthController(signalRService, $scope) {

        var vm = this;
        var hub = $.connection.gameHub; //Saves connection in "hub"-variable

        $.connection.hub.start().done(function () {                         //Opens connection to the Hub

        });

        vm.createGame = function () {
            hub.server.createGame();
        };

        vm.createTeam = function () {
            signalRService.game.GameCode = $("#GameCode").text();
            hub.server.createTeam($("#GameCode").text(), $("#TeamName").val());
        };
    };
})();