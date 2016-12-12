(function () {
    "use strict";


    angular.module("mainContent")
        .controller("lobbyController", lobbyController, ['signalRService', '$mdDialog']);
    function lobbyController(signalRService, $mdDialog) {

        var vm = this;
        var hub = $.connection.gameHub;
        vm.roundsToPlay;

        vm.game = signalRService.game;
        vm.team = signalRService.team;
        vm.rounds = 2;

        vm.openModal = function () {
            $mdDialog.show({
                controller: DialogController,
                templateUrl: 'Scripts/js/views/LobbyOptionsModal.tpl.html',
                parent: angular.element(document.body),
                clickOutsideToClose: true,
            })
        };
        vm.leaveLobby = function () {
            vm.roundOptions.forEach(vm.checkRoundsToPlay);
            hub.server.getRuleChanger(vm.game.GameCode, vm.roundsToPlay);
        };

        $.connection.hub.start().done(function () {
            console.log(vm.team)
        });

        function DialogController(signalRService) {
            //$scope.hide = function () {
            //    $mdDialog.hide();
            //};

            //$scope.cancel = function () {
            //    $mdDialog.cancel();
            //};

            //$scope.answer = function (answer) {
            //    $mdDialog.hide(answer);
            //};
        };

        vm.roundOptions = [{
                name: 'three',
                value: 3,
                checked: true
            },
            {
                name: 'four',
                value: 4,
                checked: false
            },
            {
                name: 'five',
                value: 5,
                checked: false
            }
        ];

        vm.updateSelection = function (position, r) {
            angular.forEach(r, function (r, index) {
                if (position !== index)
                    r.checked = false;
            });
        };

        vm.checkRoundsToPlay = function (r) {
            
            if (r.checked === true) {
                vm.roundsToPlay = r.value;
            } 
        };
    };
})();