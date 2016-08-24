using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Horrible_Charades_ASP.Models
{
    public class Noun
    {
        public int ID { get; set; }
        public string Description { get; set; }
        public int CategoryID{ get; set; }
    }
}