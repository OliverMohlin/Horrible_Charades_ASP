(function () {
    "use strict";

    //Obtains the existing module
    angular.module("mainContent")
        .service("gameService", gameService);

    function gameService() {

        this.gameCode = "hello"
    }
    
    
})();