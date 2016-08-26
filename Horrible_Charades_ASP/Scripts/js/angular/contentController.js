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

        //Calls CreateGame function on Server-Side when CreateTeamHost is loaded
        vm.createGame = function () {
            hub.server.createGame();
        };

        //Calls JoinGame function on Server-Side when a teamName and GameCode is submitted in CreateTeamGuest
        vm.joinGame = function () {
            gameService.gameCode = $("#GameCode").val();
            alert("Join Game in contentCtrl");
            console.log(gameService);
            hub.server.joinGame(vm.Team.Name, gameService.gameCode);
        };

        //Calls CreateTeam function on Server-Side when a teamName in CreateTeamHost is submitted
        vm.createTeam = function () {
            gameService.gameCode = $("#GameCode").text();
            console.log(gameService.gameCode);
            hub.server.createTeam(vm.Team.Name, gameService.gameCode);
        };

        //Calls GetCharade function on Server-Side when PreCharadeActor is loaded
        vm.getCharade = function () {
            console.log(vm.Team);
            hub.server.getCharade(gameService.gameCode);
        };
        
        //Calls UpdateCharade function on Server-Side when "Get Adjective"-button is pressed
        vm.getAdjective = function () {
            hub.server.updateCharade("adjective", gameService.gameCode);
        };

        //Calls UpdateCharade function on Server-Side when "Get Verb"-button is pressed
        vm.getVerb = function () {
            hub.server.updateCharade("verb", gameService.gameCode);
        };

        function writeInConsole() {
            $("#initiate");
            console.log(vm.Team)
        }

    }
})();