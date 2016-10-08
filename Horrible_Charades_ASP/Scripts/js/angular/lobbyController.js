(function () {
    "use strict";


    angular.module("mainContent")
        .controller("lobbyController", lobbyController, ['signalRService', '$mdDialog']);
        function lobbyController(signalRService, $mdDialog) {

        var vm = this;
        var hub = $.connection.gameHub;


        vm.game = signalRService.game;
        vm.teamName = signalRService.teamName;
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
            hub.server.getRuleChanger(vm.game.GameCode);
        };

        $.connection.hub.start().done(function () {
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
        }
    };
})();