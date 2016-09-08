(function () {
    "use strict";

    angular.module("mainContent")
        .controller("preCharadePartiController", preCharadePartiController, ['signalRService', '$scope', '$interval']);

    function preCharadePartiController(signalRService, $scope, $interval) {

        var vm = this;
        var hub = $.connection.gameHub;
        vm.game = signalRService.game;
        vm.teamName = signalRService.teamName;
        vm.timeLeft = 65;
        vm.promise;
        vm.time = signalRService.timeLeft;

        $.connection.hub.start().done(function () {
        });

        // Sends FunkUp's towards the acting team when a matching button is pressed 
        vm.activateFunkUp = function (Id) {
            if (Id === 3) {
                hub.server.affectCharadeTime(signalRService.game.GameCode, "minus");
            }
            if (Id === 4) {
                hub.server.updateCharade("adjective", signalRService.game.GameCode);

            }
            if (Id === 5) {
                hub.server.updateCharade("verb", signalRService.game.GameCode);
            }
        };

        // Redirects to Charade View
        vm.redirectToCharade = function () {
            hub.server.redirectToCharade(vm.game.GameCode, vm.teamName);
        };

        ///HÄR ÄR TIMER

        //Starts timer on CharadeActor
        vm.startTimer = function () {

            console.log("vm.startTimer har börjat köra");
            $(".timer").text(15);
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

        // Called upon when redirecting to PreCharade or Charade view. Starts the Timer for the View.
        //hub.client.startTimer = function () {
        //    console.log("calling vm.startTimer");
        //    vm.stopTimer();
        //    vm.startTimer();
        //};

        //// Stops the timer. Called from startTimer.
        //vm.stopTimer = function () {
        //    $interval.cancel(vm.promise);
        //};
    };
})();