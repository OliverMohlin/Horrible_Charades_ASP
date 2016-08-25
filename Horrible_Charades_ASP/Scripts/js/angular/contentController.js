(function () {
    "use strict";

    //Getting the existing module
    angular.module("mainContent")
        .controller("contentController", contentController);

    function contentController() {
        var vm = this;
        var hub = $.connection.gameHub; //Saves connection in "hub"-variable

        vm.Team = {};
        vm.createGame = function () {
            hub.server.createGame();
        };
        vm.joinGame = function () {
            vm.Team.gameCode = $("#GameCode").val();
            alert("Join Game in contentCtrl");
            console.log(vm.Team);
            hub.server.joinGame(vm.Team.Name, vm.Team.gameCode);
        };
        vm.createTeam = function () {
            vm.Team.gameCode = $("#GameCode").text();
            console.log(vm.Team);
            hub.server.createTeam(vm.Team.Name, vm.Team.gameCode);
        };
        vm.getCharade = function () {
            console.log(vm.Team);
            hub.server.getCharade(vm.Team.gameCode);
        };

        vm.getAdjective = function () {
            hub.server.updateCharade("adjective", vm.Team.gameCode);
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