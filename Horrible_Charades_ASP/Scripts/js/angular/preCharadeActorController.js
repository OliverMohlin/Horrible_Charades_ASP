﻿(function () {
    "use strict";

    angular.module("mainContent")
        .controller("preCharadeActorController", preCharadeActorController, ['signalRService', '$scope', '$interval']);

    function preCharadeActorController(signalRService, $scope, $interval) {

        var vm = this;
        var hub = $.connection.gameHub;
        vm.game = signalRService.game;
        vm.team = signalRService.team;
        vm.timeLeft = 10;
        vm.promise;

        $.connection.hub.start().done(function () {
        });

        //Calls GetCharade function on Server-Side when PreCharadeActor is loaded
        vm.getNoun = function () {
            hub.server.getNoun(vm.game.GameCode);
        };

        // Sends PowerUp's towards the acting team when a matching button is pressed 
        vm.activatePowerUp = function (clickedPowerUp) {
            if (clickedPowerUp.ID === 1) {
                hub.server.affectCharadeTime(signalRService.game.GameCode, "plus", clickedPowerUp.ID, vm.team);
                clickedPowerUp.hide = true;
            }
            if (clickedPowerUp.ID === 2) {
                hub.server.shuffleCharade(vm.game.GameCode, clickedPowerUp.ID, vm.team);
                clickedPowerUp.hide = true;
            }
        };

        // Redirects to Charade View
        vm.redirectToCharade = function () {
            hub.server.redirectToCharade(vm.game.GameCode, vm.team.Name);
        };

        $(".powerup").click(function () {
            $(".powerup").hide();
        });

        ///HÄR ÄR TIMER

        //Starts timer on CharadeActor
        vm.startTimer = function () {

            $(".timer").text(10);
            vm.promise = $interval(timer, 1000);
        };

        function timer() {
            vm.timeLeft = $(".timer").text();
            vm.timeLeft--;
            $(".timer").text(vm.timeLeft);
            if (vm.timeLeft <= 0) {
                $interval.cancel(vm.promise);
                vm.redirectToCharade();
            };
        };

        //// Called upon when redirecting to PreCharade or Charade view. Starts the Timer for the View.
        //hub.client.startTimer = function () {
        //    vm.stopTimer();
        //    vm.startTimer();
        //};

        //// Stops the timer. Called from startTimer.
        //vm.stopTimer = function () {
        //    $interval.cancel(vm.promise);
        //};
    };
})();