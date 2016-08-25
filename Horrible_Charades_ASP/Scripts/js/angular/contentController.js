(function () {
    "use strict";

    //Getting the existing module
    angular.module("mainContent")
        .controller("contentController", contentController);

    function contentController(gameService) {
        var vm = this;
        var hub = $.connection.gameHub; //Saves connection in "hub"-variable
        console.log(gameService);
        vm.Team = {};
        vm.createGame = function () {
            hub.server.createGame();
        };
        vm.joinGame = function () {
            gameService.gameCode = $("#GameCode").val();
            alert("Join Game in contentCtrl");
            console.log(gameService);
            hub.server.joinGame(vm.Team.Name, gameService.gameCode);
        };
        vm.createTeam = function () {
            gameService.gameCode = $("#GameCode").text();
            console.log(gameService.gameCode);
            hub.server.createTeam(vm.Team.Name, gameService.gameCode);
        };
        vm.getCharade = function () {
            console.log(vm.Team);
            hub.server.getCharade(gameService.gameCode);
        };

        vm.getAdjective = function () {
            hub.server.updateCharade("adjective", gameService.gameCode);
        };

        vm.getVerb = function () {
            hub.server.updateCharade("verb");
        };

        function writeInConsole() {
            $("#initiate");
            console.log(vm.Team)
        }

    }
})();