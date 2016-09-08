(function () {
    "use strict";

    angular.module("mainContent")
        .controller("totalScoreController", totalScoreController, ['signalRService', '$scope', '$interval']);

    function totalScoreController(signalRService, $scope, $interval) {

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
                hub.server.startNextCharade(vm.game.GameCode);
            };
        };

    };
})();