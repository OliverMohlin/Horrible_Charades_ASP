using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Horrible_Charades_ASP.Models
{
    public class Team
    {
        public Team(string name/*, string hash*/)
        {
            Name = name;
            //Hash = hash;
            Id = Guid.NewGuid().ToString("d");
            PowerUps = new List<RuleChanger>();
            FunkUps = new List<RuleChanger>();
            GameState.Instance.GetRuleChanger(this);
            TotalPoints = 0;
            TurnPoint = 0;
        }

        public string ConnectionID { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string GameCode { get; set; }
        public string Hash { get; set; }
        public string Group { get; set; }
        public bool isPlaying { get; set; }
        public int TotalPoints { get; set; }
        public int TurnPoint { get; set; }
        public List<RuleChanger> PowerUps { get; set; }
        public List<RuleChanger> FunkUps { get; set; }

    }
}