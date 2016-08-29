(function () {
    "use strict";

    //Getting the existing module
    angular.module("mainContent")
        .service("signalRService", signalRService);

    function signalRService(gameService) {

        var self = this;
        var hub = $.connection.gameHub;                 //Saves connection in "hub"-variable
        hub.client.hello = function (textToWrite) {     //Assigns hello() to client
            $("#result").append("<li>" + textToWrite + "</li>");
        };

        //Write out the GameCode in CreateTeamHost
        hub.client.printGameCode = function (game) {
            $("#GameCode").append(game.GameCode);
        };

        hub.client.displayMessage = function (message) {
            alert(message);
        };

        hub.client.updateGameState = function (game, nextView) {
            console.log("Updating Gamestate");
            gameService.game = game;
            console.log(gameService.game);
            //console.log("GamestateUpdated");
            //console.log("signalRService updategameState");
            hub.client.redirectToView(nextView);
        };
        
        hub.client.redirectToView = function (nextView) {
            console.log("redirecting to view")
            window.location.href = nextView;
        };

        hub.client.foo = function (message) {
            console.log(message);
        };

        $.connection.hub.start().done(function () {                         //Opens connection to the Hub
            hub.server.hello("Welcome to Horrible Charades");               //Calls hello() from Hub
        });

    }
})();