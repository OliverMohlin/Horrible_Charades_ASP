using Horrible_Charades_ASP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Horrible_Charades_ASP.Database    //Todo: när vi byter namn på mapp byt namespace
{
    public class DatabaseUtils
    {
        NounContext _nounContext;
        public DatabaseUtils(NounContext nouncontext)
        {
            _nounContext = nouncontext;
        }
        public string GetNoun()
        {
            var qResult = _nounContext.Nouns
                .SingleOrDefault(n => n.ID == 1);

            return qResult.Description;
            //Todo: Kolla databas efter ett adjektiv och returnerar den
            //return "temp";
        }
        public string GetAdjective()
        {
            //Todo: Kolla databas efter ett adjektiv och returnerar den
            return "temp";
        }
        public string GetVerb()
        {
            //Todo: Kolla databas efter ett verb och returnerar den
            return "temp";
        }
    }
}