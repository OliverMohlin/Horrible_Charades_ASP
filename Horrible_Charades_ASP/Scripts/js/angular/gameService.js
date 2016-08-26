(function () {
    "use strict";

    //Getting the existing module
    angular.module("mainContent")
        .service("gameService", gameService);

    function gameService() {
        var self = this;
        self.game = {
            team : "",
            gameCode : ""
        };
        var gameStateClient;
        console.log(self.game);

    }


})();