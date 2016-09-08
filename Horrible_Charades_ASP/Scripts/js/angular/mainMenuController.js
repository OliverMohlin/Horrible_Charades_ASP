(function () {
    "use strict";

    // obtains existing module
    angular.module("mainContent")
        .controller("mainMenuController", mainMenuController);

    function mainMenuController(signalRService, $scope) {

        var mainMenu = this;
        var hub = $.connection.gameHub; //Saves connection in "hub"-variable


        $.connection.hub.start().done(function () {                         //Opens connection to the Hub
        });
    };
})();