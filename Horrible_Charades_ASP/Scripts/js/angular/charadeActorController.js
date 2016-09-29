/// <reference path="signalRService.js" />
(function () {
    "use strict";

    angular.module("mainContent")
        .controller("charadeActorController", charadeActorController, ['signalRService', '$scope', '$interval']);

    function charadeActorController(signalRService, $scope, $interval) {

        var vm = this;
        var hub = $.connection.gameHub;

        vm.game = signalRService.game;
        vm.promise;
        vm.timeLeft = signalRService.charadeTime;

        vm.printCharade = function () {

            for (var i = 0; i < signalRService.game.CurrentCharade.Adjective.length; i++) {
                $("#charade").append("<li>" + signalRService.game.CurrentCharade.Adjective[i].Description + " </li>");
            }
            $("#charade").append("<li>" + signalRService.game.CurrentCharade.Noun.Description + " </li>");

            for (i = 0; i < signalRService.game.CurrentCharade.Verb.length; i++) {
                $("#charade").append("<li>" + signalRService.game.CurrentCharade.Verb[i].Description + "</li>");
            }
        };

        ///HÄR ÄR TIMER

        //Starts timer on CharadeActor
        vm.startTimer = function () {
            $(".timer").text(vm.timeLeft);
            vm.promise = $interval(timer, 1000);
        };

        function timer() {
            vm.timeLeft = $(".timer").text();
            vm.timeLeft--;
            $(".timer").text(vm.timeLeft);
            if (vm.timeLeft <= 0) {
                $interval.cancel(vm.promise);
                hub.server.calculateScore(vm.game.GameCode, 0);
            };
        };

        vm.pointCounter = function () {
            $interval.cancel(vm.promise);
            var timeLeft = $(".timer").text();
            hub.server.calculateScore(vm.game.GameCode, timeLeft);
        };


    };
})();