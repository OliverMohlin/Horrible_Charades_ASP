(function () {
    "use strict";


    angular.module("mainContent")
        .controller("lobbyController", lobbyController, ['signalRService', '$mdDialog']);
        function lobbyController(signalRService, $mdDialog) {

        var vm = this;
        var hub = $.connection.gameHub;

        vm.game = signalRService.game;
        //vm.teamName = signalRService.teamName;
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

        vm.entities = [{
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
        ]

        vm.updateSelection = function (position, entities) {
            angular.forEach(entities, function (subscription, index) {
                if (position != index)
                    subscription.checked = false;
            });
        }

        //vm.checkRoundsToPlay = function (entities) {
        //    //default-value is 3
        //    var roundsToPlay = 3;
        //    for (var i = 0; i < entities.length; i++) {
        //        if (entities[i].checked === true)
        //            roundsToPlay = entities[i].value;
        //    }
        //    return roundsToPlay;
        //}
    };
})();