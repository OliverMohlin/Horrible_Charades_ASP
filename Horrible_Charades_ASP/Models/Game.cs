using Horrible_Charades_ASP.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Horrible_Charades_ASP.Models
{
    public class Game
    {
        public Game()
        {
            GameCode = GenerateCode();
            Teams = new List<Team>();
            CurrentCharade = new Charade();
            GameState = 1;
            Round = 0;
            RoundsToPlay = 2; //Hårdkodat in RoundsToPlay nu. ska kunna vara valbart senare
            Winner = new List<Team>();
            
        }

        public List<Team> Teams { get; set; }
        public string[] TurnOrder { get; set; }
        public int Turn { get; set; }
        public int Round { get; set; }
        public int RoundsToPlay { get; set; }
        public Charade CurrentCharade { get; set; }
        public Team WhosTurn { get; set; }
        public string GameCode { get; set; }
        public int GameState { get; set; }
        public int ID { get; set; }
        public List<Team> Winner { get; set; }

        //Genererar en kod som användare skriver in för att komma in på samma spel
        private string GenerateCode()
        {
            string gameCode = "";
            for (int i = 0; i < 5; i++) //Antal tecken
            {
                if (RandomUtils.ReturnValue(2) == 0) //50-50 om det blir en siffra eller bokstav
                {
                    int num = RandomUtils.ReturnValue(26);
                    char let = (Char)('A' + num);
                    gameCode += let;
                }
                else
                {
                    string nr = Convert.ToString(RandomUtils.ReturnValue(10));
                    gameCode += nr;
                }
            }

            return gameCode.ToUpper();
        }
    }
}