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
        public DatabaseUtils(CharadeContext context)
        {
            _charadeContext = context;
            _charadeContext.Database.Initialize(false);
        }
        /// <summary>
        /// Hämtar ett Noun baserat på framslumpat ID.
        /// </summary>
        /// <returns>random Word</returns>
        public Word GetNoun()
        {
            var nounID = RandomUtils.ReturnValue(_charadeContext.Nouns.Count() - 1);
            var qResult = _charadeContext.Nouns
                .SingleOrDefault(n => n.ID == nounID);
            return qResult;
        }

        /// <summary>
        /// Hämtar ett Adjective baserat på framslumpat ID.
        /// </summary>
        /// <returns>random Word</returns>
        public Word GetAdjective()
        {
            var adjectiveID = RandomUtils.ReturnValue(_charadeContext.Adjectives.Count() - 1);
            var qResult = _charadeContext.Adjectives
            .SingleOrDefault(n => n.ID == adjectiveID);
            return qResult;
        }

        /// <summary>
        /// Hämtar ett Verb baserat på framslumpat ID.
        /// </summary>
        /// <returns>random Word</returns>
        public Word GetVerb()
        {
            var verbID = RandomUtils.ReturnValue(_charadeContext.Verbs.Count() - 1);
            var qResult = _charadeContext.Verbs
            .SingleOrDefault(n => n.ID == verbID);
            return qResult;
        }

        /// <summary>
        /// This method retrieves incorrect alternatives to show in opponents' view during a charade.
        /// </summary>
        /// <param name="inputWord"></param>
        /// <returns></returns>
        public List<string> GetIncorrectAnswers(Word inputWord)
        {
            List<Word> tmpList = new List<Word>();
            List<String> newTmpList = new List<String>();

            if (inputWord is Noun)
            {
                tmpList.AddRange(GetAllNounWords(inputWord.CategoryID));
            }
            else if (inputWord is Adjective)
            {
                tmpList.AddRange(GetAllAdjectiveWords(inputWord.CategoryID));
            }
            else
                tmpList.AddRange(GetAllVerbWords(inputWord.CategoryID));

            tmpList.Remove(inputWord);
            for (int i = 0; i < 3; i++)
            {
                int randomID = RandomUtils.ReturnValue(tmpList.Count - 1);
                newTmpList.Add(tmpList[randomID].Description);
                tmpList.Remove(tmpList[randomID]);
            }
            return newTmpList;
        }

        /// <summary>
        /// Retrieves all nouns within a given category.
        /// </summary>
        /// <param name="categoryID"></param>
        /// <returns></returns>
        private List<Word> GetAllNounWords(int categoryID)
        {
            return _charadeContext.Nouns.Where(n => n.CategoryID == categoryID).ToList<Word>();
        }

        /// <summary>
        /// Retrieves all adjectives within a given category.
        /// </summary>
        /// <param name="categoryID"></param>
        /// <returns></returns>
        private List<Word> GetAllAdjectiveWords(int categoryID)
        {
            return _charadeContext.Adjectives.Where(n => n.CategoryID == categoryID).ToList<Word>();
        }

        /// <summary>
        /// Retrieves all verbs.
        /// </summary>
        /// <param name="categoryID"></param>
        /// <returns></returns>
        private List<Word> GetAllVerbWords(int categoryID)
        {
            return _charadeContext.Verbs.Where(n => n.CategoryID == categoryID).ToList<Word>();
        }

        public RuleChanger GetRuleChanger(string type)
        {

            var tmpList = _charadeContext.RuleChangers.Where(r => r.Type == type).ToList();

            int modifierID = RandomUtils.ReturnValue(tmpList.Count() - 1);
            return tmpList[modifierID];
        }
        public RuleChanger GetRuleChanger()
        {
            var tmpList = new List<RuleChanger>();
            tmpList = _charadeContext.RuleChangers.ToList();

            int modifierID = RandomUtils.ReturnValue(tmpList.Count() - 1);
            return tmpList[modifierID];
        }
    }
}
