$(function () {
    var hub = $.connection.gameHub; //Saves connection in "hub"-variable
    hub.client.hello = function (textToWrite) {     //Assigns hello() to client
        $("#result").append("<li>" + textToWrite + "</li>");
    };

    hub.client.teamsJoined = function (teamName, conId, connectedClients) {
        $("#teams").append("TeamName: " + teamName + "<br /> ConnectionId: " + conId + "<br /> ConnectedClients: " + connectedClients + "<br /> <br />");
    }

    $("#getNounButton").click(function () {
        hub.server.getNoun();
    });

    $("#getAdjectiveButton").click(function () {
        hub.server.getAdjective();
    });

    $("#getVerbButton").click(function () {
        hub.server.getVerb();
    });

    $.connection.hub.start().done(function () {                         //Opens connection to the Hub
        hub.server.hello("Welcome to Horrible Charades");               //Calls hello() from Hub
    })
})