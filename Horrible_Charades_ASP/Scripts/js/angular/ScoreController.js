(function () {
    "use strict";

    angular.module("mainContent")
        .controller("scoreController", scoreController, ['signalRService', '$scope', '$interval']);

    function scoreController(signalRService, $scope, $interval) {

        var vm = this;
        var hub = $.connection.gameHub;

        vm.game = signalRService.game;
        vm.promise;
        vm.timeLeft = 5;

        ///HÄR ÄR TIMER

        //Starts timer on CharadeActor
        vm.startTimer = function () {
            console.log("vm.startTimer har börjat köra");
            vm.promise = $interval(timer, 1000);
        };

        function timer() {
            vm.timeLeft--;
            if (vm.timeLeft <= 0) {
                $interval.cancel(vm.promise);
                hub.server.redirectToTotalScore(vm.game.GameCode);
            };
        };

    };
})();