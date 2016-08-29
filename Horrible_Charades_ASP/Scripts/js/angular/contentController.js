(function () {
    "use strict";

    //Getting the existing module
    angular.module("mainContent")
        .controller("contentController", contentController);

    function contentController(gameService, signalRService) {
        var vm = this;
        var hub = $.connection.gameHub; //Saves connection in "hub"-variable
        console.log(gameService);
        console.log("gameservice");
        
        vm.gameData = gameService.game;
        console.log(vm.gameData);
        console.log("gameData");

        //Calls CreateGame function on Server-Side when CreateTeamHost is loaded
        vm.createGame = function () {
            hub.server.createGame();
        };

        //Calls JoinGame function on Server-Side when a teamName and GameCode is submitted in CreateTeamGuest
        vm.joinGame = function () {
            //gameService.gameCode = $("#GameCode").val();
            hub.server.joinGame($("#GameCode").val(), $("#TeamName").val());
        };

        //Calls CreateTeam function on Server-Side when a teamName in CreateTeamHost is submitted
        vm.createTeam = function () {
            gameService.game.GameCode = $("#GameCode").text();
            console.log("requesting to create team");
            hub.server.createTeam(gameService.game.GameCode, $("#TeamName").val());
        };

        //Calls GetCharade function on Server-Side when PreCharadeActor is loaded
        vm.getNoun = function () {
            console.log("initiating getNoun");
            hub.server.getNoun(gameService.game.GameCode);
        };

        //// Bara javascript variant
        //$("#charade").load = function () {
        //    console.log("initiating getNoun");
        //    hub.server.getNoun(gameService.gameCode);
        //};

        ////jQuery variant
        //$("#charade").load(function () {
        //    console.log("initiating getNoun");
        //    hub.server.getNoun(gameService.gameCode);
        //});

        //Calls UpdateCharade function on Server-Side when "Get Adjective"-button is pressed
        vm.getAdjective = function () {
            console.log("initiating getAdjective");
            hub.server.updateCharade("adjective", gameService.gameCode);
        };

        //Calls UpdateCharade function on Server-Side when "Get Verb"-button is pressed
        vm.getVerb = function () {
            console.log("initiating getVerb");
            hub.server.updateCharade("verb", gameService.gameCode);
        };

    }
})();