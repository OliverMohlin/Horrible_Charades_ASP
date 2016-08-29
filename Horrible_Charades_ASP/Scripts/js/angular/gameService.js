(function () {
    "use strict";

    //Obtains the existing module
    angular.module("mainContent")
        .service("gameService", gameService);

    function gameService() {
        var self = this;
        self.game = {};
        var gameStateClient;
        console.log(self.game);

    }


})();