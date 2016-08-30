(function () {
    "use strict";

    //Obtains the existing module
    angular.module("mainContent")
        .service("gameService", gameService);

    function gameService() {
        var self = this;
        self.game = {};
        self.myTeam = {};
        var gameStateClient;
        console.log(self.game);

    }


})();