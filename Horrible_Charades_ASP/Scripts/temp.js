$(function () {
    var hub = $.connection.gameHub;                 //Saves connection in "hub"-variable
    hub.client.hello = function (textToWrite) {     //Assigns hello() to client
        $("#result").append("<li>" + textToWrite + "</li>");
    };

    hub.client.teamsJoined = function (teamName, conId, connectedClients) {
        $("#teams").append("TeamName: " + teamName + "<br /> ConnectionId: " + conId + "<br /> ConnectedClients: " + connectedClients + "<br /> <br />");
    };

    //Gets charade from GameHub and pushes out to #Charade
    hub.client.printCharade = function (charade) {
        $("#Charade").append(charade.Noun !== null ? charade.Noun + " " : charade.Adjective[0] + " ")

    };

    hub.client.printGameCode = function (game) {
        console.log(game)
        //vm.Team.GameCode = game.GameCode
        $("#GameCode").append(game.GameCode)
    };

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
    });
});