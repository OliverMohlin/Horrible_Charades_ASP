(function () {
    "use strict";

    angular.module("mainContent")
        .controller("charadePartiController", charadePartiController, ['signalRService', '$scope', '$interval']);

    function charadePartiController(signalRService, $scope, $interval) {

        var vm = this;
        var hub = $.connection.gameHub;

        vm.game = signalRService.game;
        vm.promies;
        vm.timeLeft = signalRService.charadeTime;
        vm.guessed = false;
        vm.alternatives = ["a", "b", "c"];

        // Gets incorrect fromAnswers to Display from 
        vm.getIncorrectAnswers = function () {
            hub.server.getIncorrectAnswers(vm.game.GameCode);
            console.log("trying to call getIncorrectAnswers");
        };

        vm.submitGuess = function () {
            console.log("Inne i submit guess");
            vm.guessed = true;
            var guess = $("#hidden").text() + " ";//Lägger vi till fler ord måste vi lägga till en splitchar
            $("#submit").append(" " + guess);
            var timeLeft = $(".timer").text();
            console.log(timeLeft);
            hub.server.calculateScoreP(vm.game.GameCode, timeLeft, guess);
            console.log("har anropat calculateScoreP");
        };

        vm.pointCounter = function () {
            $interval.cancel(vm.promise);
            console.log("You're in pointcounter");
            var timeLeft = $(".timer").text();
            hub.server.pointCounter(vm.game.GameCode, timeLeft);
        };


        ///HÄR ÄR TIMER

        //Starts timer on CharadeActor
        vm.startTimer = function () {

            console.log("vm.startTimer har börjat köra");
            $(".timer").text(vm.timeLeft);
            vm.promise = $interval(timer, 1000);
        };

        function timer() {
            vm.timeLeft = $(".timer").text();
            vm.timeLeft--;
            $(".timer").text(vm.timeLeft);
            if (vm.timeLeft <= 0) {
                $interval.cancel(vm.promise);
                vm.pointCounter();
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