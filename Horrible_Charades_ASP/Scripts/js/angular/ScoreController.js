(function () {
    "use strict";

    angular.module("mainContent")
        .controller("scoreController", scoreController, ['signalRService', '$scope', '$interval']);

    function scoreController(signalRService, $scope, $interval) {

        var vm = this;
        var hub = $.connection.gameHub;

        vm.game = signalRService.game;
        vm.team = signalRService.team;
        vm.promise;
        vm.timeLeft = 5;

        ///HÄR ÄR TIMER

        //Starts timer on CharadeActor
        vm.startTimer = function () {
            vm.promise = $interval(timer, 1000);
        };

        function timer() {
            vm.timeLeft--;
            if (vm.timeLeft <= 0 && vm.game.WhosTurn.Name === vm.team.Name) {
                console.log("endTimer");
                $interval.cancel(vm.promise);
                hub.server.redirectToTotalScore(vm.game.GameCode);
            };
        };

    };
})();