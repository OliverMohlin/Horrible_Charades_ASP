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
            Adjective = new List<string>();
            Verb = new List<string>();
            //Todo: sätt noun direkt med ett substantiv från databasen.
            //Todo: sätt ID på ett vettigt sätt :) (vet inte hur vi ska använda detta än)
        }
   
        public int Id { get; set; }
        public string Noun { get; set; }
        public List<string> Adjective { get; set; }
        public List<string> Verb { get; set; }

    }
}