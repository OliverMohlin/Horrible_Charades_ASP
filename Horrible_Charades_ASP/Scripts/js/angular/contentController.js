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
            alert("Create game in ContentCtrl ")
            hub.server.createGame();
        }
        vm.getCharade = function () {
            hub.server.getCharade();
        }

        vm.getAdjective = function () {
            hub.server.updateCharade();
        }

        vm.createTeam = function () {
            console.log(vm.Team.Name);
            hub.server.createTeam(vm.Team.Name);
        };
    }
})();