﻿(function () {
    "use strict";

    //obtains existing module
    angular.module("mainContent")
        .controller("ctgController", ctgController);

    function ctgController(signalRService) {

        var vm = this;
        var hub = $.connection.gameHub;
        vm.game = signalRService.game;

        $.connection.hub.start().done(function () {
        });

        vm.pressEnter = function (keyEvent) {
            if (keyEvent.which === 13)
                vm.joinGame();
        };

        vm.joinGame = function () {

            var teamNameInput = $("#TeamName").val().trim();

            if (teamNameInput.length >= 20) {
                $(".errorMessage").html("Your team name is too long, Max length is 20 characters");
                setTimeout(function () {
                    $(".errorMessage").html("");
                }, 3000);
            }
            else if(teamNameInput.length < 1) {
                $(".errorMessage").html("Enter a team name!");
                setTimeout(function () {
                    $(".errorMessage").html("");
                }, 3000);
            }
            else {
                hub.server.joinGame($("#GameCodeGuest").val(), $("#TeamName").val());
            }
        };


    }
})();