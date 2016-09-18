(function () {
    "use strict";

    angular.module("mainContent")
        .controller("charadePartiController", charadePartiController, ['signalRService', '$scope', '$interval']);

    function charadePartiController(signalRService, $scope, $interval) {

        var vm = this;
        var hub = $.connection.gameHub;

        vm.game = signalRService.game;
        vm.promise;
        vm.timeLeft = signalRService.charadeTime;
        vm.guessed = false;
        vm.alternatives = ["a", "b", "c"];

        // Gets incorrect fromAnswers to Display from 
        vm.getIncorrectAnswers = function () {
            hub.server.getIncorrectAnswers(vm.game.GameCode);
        };

        vm.submitGuess = function () {
            vm.guessed = true;
            var guess = $("#hidden").text() + " ";//Lägger vi till fler ord måste vi lägga till en splitchar
            $("#submit").append(" " + guess);
            var timeLeft = $(".timer").text();
            console.log("guess submitted")
            hub.server.calculateScoreP(vm.game.GameCode, timeLeft, guess);
        };

        $("#submitGuessButton").click(function () {
            $("#submitGuessButton").hide();
            $("#enter").hide();
        });

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
            };
        };


        //// Is not used right now. Could be used in the future if placed in a Service..
        //// Called upon when redirecting to PreCharade or Charade view. Starts the Timer for the View.
        //hub.client.startTimer = function () {
        //    console.log("calling vm.startTimer");
        //    vm.stopTimer();
        //    vm.startTimer();
        //};
        //// Stops the timer. Called from hub.client.startTimer.
        //vm.stopTimer = function () {
        //    $interval.cancel(vm.promise);
        //};

    };
})();