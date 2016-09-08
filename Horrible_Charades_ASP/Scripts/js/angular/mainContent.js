
//Creates the module
(function () {
    angular.module("mainContent", ["ngRoute", "ngSanitize"])
    .config(function ($routeProvider) {

        $routeProvider
            .when("/", {
                controller: "mainMenuController",
                controllerAs: "vm",
                templateUrl: "Scripts/js/views/MainMenu.tpl.html"
            })

        .when("/CreateTeamHost", {
            controller: "cthController",
            controllerAs: "vm",
            templateUrl: "Scripts/js/views/CreateTeamHost.tpl.html"
        })
        .when("/CreateTeamGuest", {
            controller: "ctgController",
            controllerAs: "vm",
            templateUrl: "Scripts/js/views/CreateTeamGuest.tpl.html"
        })

        .when("/LobbyHost", {
            controller: "lobbyController",
            controllerAs: "vm",
            templateUrl: "Scripts/js/views/LobbyHost.tpl.html"
        })

        .when("/LobbyGuest", {
            controller: "lobbyController",
            controllerAs: "vm",
            templateUrl: "Scripts/js/views/LobbyGuest.tpl.html"
        })

        .when("/WaitingRoomActor", {
            controller: "waitingRoomController",
            controllerAs: "vm",
            templateUrl: "Scripts/js/views/WaitingRoomActor.tpl.html"
        })

         .when("/WaitingRoomOpponent", {
             controller: "waitingRoomController",
             controllerAs: "vm",
             templateUrl: "Scripts/js/views/WaitingRoomOpponent.tpl.html"
         })


        .when("/PreCharadeActor", {
            controller: "preCharadeActorController",
            controllerAs: "vm",
            templateUrl: "Scripts/js/views/PreCharadeActor.tpl.html"
        })

         .when("/PreCharadeParticipant", {
             controller: "preCharadePartiController",
             controllerAs: "vm",
             templateUrl: "Scripts/js/views/PreCharadeParticipant.tpl.html"
         })

        .when("/CharadeActor", {
            controller: "charadeActorController",
            controllerAs: "vm",
            templateUrl: "Scripts/js/views/CharadeActor.tpl.html"
        })

         .when("/CharadeParticipant", {
             controller: "charadePartiController",
             controllerAs: "vm",
             templateUrl: "Scripts/js/views/CharadeParticipant.tpl.html"
         })

        .when("/Score", {
            controller: "contentController",
            controllerAs: "vm",
            templateUrl: "Scripts/js/views/Score.tpl.html"
        })

        .when("/TotalScore", {
            controller: "contentController",
            controllerAs: "vm",
            templateUrl: "Scripts/js/views/TotalScore.tpl.html"
        });

        $routeProvider.otherwise({ redirectTo: "/" });
    });
})();