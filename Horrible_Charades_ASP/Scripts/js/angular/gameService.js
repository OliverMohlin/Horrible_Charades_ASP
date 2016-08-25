(function () {
    "use strict";

    //Getting the existing module
    angular.module("mainContent")
        .service("gameService", gameService);

    function gameService() {

        this.gameCode = "hello"
    }
    
    
})();