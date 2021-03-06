﻿(function () {
    "use strict";

    //obtains existing module
    angular.module("mainContent")
        .controller("cthController", cthController);

    function cthController(signalRService, $scope) {

        var vm = this;
        var hub = $.connection.gameHub; //Saves connection in "hub"-variable

        $.connection.hub.start().done(function () {                         //Opens connection to the Hub

        });
        vm.printGameCode = function () {
            $("#GameCode").append(signalRService.game.GameCode);
        };

        vm.pressEnter = function (keyEvent) {
            if (keyEvent.which === 13)
                vm.createTeam();
        };

        vm.createTeam = function () {

            var teamNameInput = $("#TeamName").val().trim();

            if (teamNameInput.length >= 20) {
                $(".errorMessage").html("Your team name is too long, Max length is 20 characters");
                setTimeout(function () {
                    $(".errorMessage").html("");
                }, 3000);
            }
            else if (teamNameInput.length < 1) {
                $(".errorMessage").html("Enter a team name!");
                setTimeout(function () {
                    $(".errorMessage").html("");
                }, 3000);
            }
            else {
                signalRService.game.GameCode = $("#GameCode").text();
                hub.server.createTeam($("#GameCode").text(), $("#TeamName").val());
            }
        };
    }
})();