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
        public Noun GetNoun()
        {
          
            Noun qResult = null;
            do
            {
                var nounID = RandomUtils.ReturnValue(_charadeContext.Nouns.Count() - 1);
                qResult = _charadeContext.Nouns
               .SingleOrDefault(n => n.ID == nounID);

            } while (qResult == null);

            return qResult;
        }

        /// <summary>
        /// Hämtar ett Adjective baserat på framslumpat ID.
        /// </summary>
        /// <returns>random Word</returns>
        public Adjective GetAdjective()
        {
      
            Adjective qResult = null;
            do
            {
                var adjectiveID = RandomUtils.ReturnValue(_charadeContext.Adjectives.Count() - 1);
                qResult = _charadeContext.Adjectives
            .SingleOrDefault(n => n.ID == adjectiveID);
            } while (qResult == null);

            return qResult;
        }

        /// <summary>
        /// Hämtar ett Verb baserat på framslumpat ID.
        /// </summary>
        /// <returns>random Word</returns>
        public Verb GetVerb()
        {
          
            Verb qResult = null;
            do
            {
                var verbID = RandomUtils.ReturnValue(_charadeContext.Verbs.Count() - 1);
                qResult = _charadeContext.Verbs
            .SingleOrDefault(n => n.ID == verbID);
            } while (qResult == null);
            
            return qResult;
        }

        /// <summary>
        /// This method retrieves incorrect alternatives to show in opponents' view during a charade + the right one.
        /// </summary>
        /// <param name="inputWord"></param>
        /// <returns></returns>
        public List<Word> GetIncorrectAnswers(Word inputWord)
        {
            List<Word> tmpList = new List<Word>();
            List<Word> newTmpList = new List<Word>();

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
                newTmpList.Add(tmpList[randomID]);
                tmpList.Remove(tmpList[randomID]);
            }
            newTmpList.Add(inputWord);
            //newTmpList.Sort(t => RandomUtils.rnd.Next()).Select(o => o.ID);
            newTmpList.Shuffle();
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

        public RuleChanger GetRuleChangerByType(string type)
        {
            var tmpList = _charadeContext.RuleChangers.Where(r => r.Type == type).ToList();
            int modifierID = RandomUtils.ReturnValue(tmpList.Count()-1);
            return tmpList[modifierID];
        }

        public RuleChanger GetRandomRuleChanger()
        {
            var tmpList = _charadeContext.RuleChangers.Where(r => r.ID > 0).ToList();

            int modifierID = RandomUtils.ReturnValue(tmpList.Count()-1);
            return tmpList[modifierID];
        }
    }
}
