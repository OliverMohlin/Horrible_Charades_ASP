$(function () {
    //Saves connection in "hub"-variable
    var hub = $.connection.gameHub;

    //Assigns hello() to client
    hub.client.hello = function (textToWrite) {
        $("#result").append("<li>" + textToWrite + "</li>");
    };

    //Print TeamName, ConnectionId and connected users in CreateTeamHost
    hub.client.teamsJoined = function (game) {
        console.log(game);
        $("#teams").append("TeamName: " + game.Teams[0].Name + "<br /> ConnectionId: " + game.Teams[0].ConnectionID + "<br /> ConnectedClients: " + game.Teams.length + "<br /> <br />");
    };

    //Print and append new words to a charade in Pre-Charade(?)
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

    //Print the GameCode in CreateTeamHost
    hub.client.printGameCode = function (game) {
        console.log(game);
        $("#GameCode").append(game.GameCode);
    };

    //Displays a noun in a charade
    $("#getNounButton").click(function () {
        hub.server.getNoun();
    });

    //Displays an adjective in a charade
    $("#getAdjectiveButton").click(function () {
        hub.server.getAdjective();
    });

    //Displays a verb in a charade
    $("#getVerbButton").click(function () {
        hub.server.getVerb();
    });

    //Opens connection to the Hub and calls hello()
    $.connection.hub.start().done(function () {
        hub.server.hello("Welcome to Horrible Charades");
    });
});