using Horrible_Charades_ASP.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Horrible_Charades_ASP.Database    //Todo: när vi byter namn på mapp byt namespace
{
    public class DatabaseUtils
    {
        CharadeContext _charadeContext;
        public DatabaseUtils(CharadeContext nouncontext)
        {
            _charadeContext = nouncontext;
        }
        /// <summary>
        /// Hämtar ut ett en slumpad siffra bland antalet
        /// </summary>
        /// <returns></returns>
        public string GetNoun()
        {
            var nounCount = RandomUtils.ReturnValue(_charadeContext.Nouns.Count()+1);
            var qResult = _charadeContext.Nouns
                .SingleOrDefault(n => n.ID == nounCount);

            return qResult.Description;

        }
        public string GetAdjective()
        {
            var adjectiveCount = RandomUtils.ReturnValue(_charadeContext.Adjectives.Count() + 1);
            var qResult = _charadeContext.Adjectives
            .SingleOrDefault(n => n.ID == adjectiveCount);

            return qResult.Description;
        }
        public string GetVerb()
        {
            var verbCount = RandomUtils.ReturnValue(_charadeContext.Verbs.Count() + 1);
            var qResult = _charadeContext.Verbs
            .SingleOrDefault(n => n.ID == verbCount);

            return qResult.Description;

        }
    }
}