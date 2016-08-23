using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace Horrible_Charades_ASP
{
    public class GameHub : Hub
    {
        public void Hello(string textToWrite)
        {
            Clients.All.hello(textToWrite);
        }
    }
}