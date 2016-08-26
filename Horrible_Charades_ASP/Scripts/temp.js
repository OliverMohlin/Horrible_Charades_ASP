$(function () {
    var hub = $.connection.gameHub;                 //Saves connection in "hub"-variable
    hub.client.hello = function (textToWrite) {     //Assigns hello() to client
        $("#result").append("<li>" + textToWrite + "</li>");
    };

    //Write out TeamName, ConnectionId and connected users in CreateTeamHost
    //Todo: If successful redirect user to lobby
    hub.client.teamsJoined = function (game) {
        console.log(game);
        $("#teams").append("TeamName: " + game.Teams[0].Name + "<br /> ConnectionId: " + game.Teams[0].ConnectionID + "<br /> ConnectedClients: " + game.Teams.length + "<br /> <br />");
    };

    //Gets charade from GameHub and pushes out to #Charade
    //hub.client.printCharade = function (charade) {
    //    $("#Charade").append(charade.Noun !== null ? charade.Noun + " " : charade.Adjective[0] + " ");

    //};

    //Write out and append new words to a charade in Pre-Charade(?)
    hub.client.InsertCharadeHTML = function (charadeWord, typeOfWord) {
        if (typeOfWord === "noun") {
            console.log("printing noun");
            $("#Charade").append("<div id='noun' style='display:inline''>" + charadeWord + "</div>");
        }

        if (typeOfWord === "adjective") {
            $("#noun").prepend("<div class='adjective' style='display:inline'>" + charadeWord + " " + "</div>");
            console.log("printing adjective");
        }

        if (typeOfWord === "verb") {
            if ($('.verb').length) {
                $("#noun").append("<div class='verb' style='display:inline'>" + " while " + charadeWord + "</div>");
            }
            else {
                $("#noun").append("<div class='verb' style='display:inline'>" + "  " + charadeWord + "</div>");
            }
        }

    };

    //Write out the GameCode in CreateTeamHost
    hub.client.printGameCode = function (game) {
        console.log(game);
        //vm.Team.GameCode = game.GameCode
        $("#GameCode").append(game.GameCode);
    };

    //Adds a Noun to a charade(?)
    $("#getNounButton").click(function () {
        hub.server.getNoun();
    });

    //Adds an adjective to a charade
    $("#getAdjectiveButton").click(function () {
        hub.server.getAdjective();
    });
    //Adds a verb to a charade
    $("#getVerbButton").click(function () {
        hub.server.getVerb();
    });

    $.connection.hub.start().done(function () {                         //Opens connection to the Hub
        hub.server.hello("Welcome to Horrible Charades");               //Calls hello() from Hub
    });
});