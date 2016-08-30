using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Horrible_Charades_ASP.Models
{
    public class Charade
    {
        public Charade()
        {
            Adjective = new List<Adjective>();
            Verb = new List<Verb>();
            //Todo: sätt noun direkt med ett substantiv från databasen.
            //Todo: sätt ID på ett vettigt sätt :) (vet inte hur vi ska använda detta än)
        }
   
        public int Id { get; set; }
        public int Time { get; set; }
        public Noun Noun { get; set; }
        public List<Adjective> Adjective { get; set; }
        public List<Verb> Verb { get; set; }

    }
}