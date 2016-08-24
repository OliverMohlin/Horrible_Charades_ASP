﻿
//Creating the module
(function () {
    angular.module("mainContent", ["ngRoute"])
    .config(function ($routeProvider) {

        $routeProvider
            .when("/", {
                controller: "contentController",
                controllerAs: "vm",
                templateUrl: "Scripts/js/views/MainMenu.tpl.html"
            })

        .when("/CreateTeam", {
            controller: "contentController",
            controllerAs: "vm",
            templateUrl: "Scripts/js/views/CreateTeam.tpl.html"
        })

        .when("/Lobby", {
            controller: "contentController",
            controllerAs: "vm",
            templateUrl: "Scripts/js/views/Lobby.tpl.html"
        })

        .when("/WaitingRoom", {
            controller: "contentController",
            controllerAs: "vm",
            templateUrl: "Scripts/js/views/WaitingRoom.tpl.html"
        })

        .when("/PreCharadeActor", {
            controller: "contentController",
            controllerAs: "vm",
            templateUrl: "Scripts/js/views/PreCharadeActor.tpl.html"
        })

         .when("/PreCharadeParticipant", {
             controller: "contentController",
             controllerAs: "vm",
             templateUrl: "Scripts/js/views/PreCharadeParticipant.tpl.html"
         })

        .when("/CharadeActor", {
            controller: "contentController",
            controllerAs: "vm",
            templateUrl: "Scripts/js/views/CharadeActor.tpl.html"
        })

         .when("/CharadeParticipant", {
             controller: "contentController",
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