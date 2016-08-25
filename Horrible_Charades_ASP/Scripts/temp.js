$(function () {
    var hub = $.connection.gameHub;                 //Saves connection in "hub"-variable
    hub.client.hello = function (textToWrite) {     //Assigns hello() to client
        $("#result").append("<li>" + textToWrite + "</li>");
    };

    hub.client.teamsJoined = function (game) {
        console.log(game)
        $("#teams").append("TeamName: " + game.Teams[0].Name + "<br /> ConnectionId: " + game.Teams[0].ConnectionID + "<br /> ConnectedClients: " + game.Teams.length + "<br /> <br />");
    };

    //Gets charade from GameHub and pushes out to #Charade
    //hub.client.printCharade = function (charade) {
    //    $("#Charade").append(charade.Noun !== null ? charade.Noun + " " : charade.Adjective[0] + " ");

    //};

    hub.client.InsertCharadeHTML = function (charade, typeOfWord) {
        //if (typeOfWord === "noun") {
        //    console.log("printing noun");
        //    $("#Charade").append("<div id='noun style='display:inline''>" + charade.Noun + "</div>");
        //}

        //if (typeOfWord === "adjective") {
        //    $("#noun").prepend("<div class='adjective' style='display:inline'>" + charade.Adjective[0] + "</div>");
        //    console.log("printing adjective");
        //}

        //if (typeOfWord === "verb") {
        //    console.log("printing verb");
        //    $("#noun").append("<div class='verb' style='display:inline'>" + charade.Verb[0] + "</div>");
        //}

        if (typeOfWord === "noun") {
            console.log("printing noun");
            $("#Charade").append("<div id='noun' style='display:inline''>" + charade.Noun + "</div>");
        }

        if (typeOfWord === "adjective") {
            $("#noun").prepend("<div class='adjective' style='display:inline'>" + charade.Adjective[0]+ " " + "</div>");
            console.log("printing adjective");
        }

        if (typeOfWord === "verb") {
            console.log("printing verb");
            $("#noun").append("<div class='verb' style='display:inline'>" + " " + charade.Verb[0] + "</div>");
        }

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