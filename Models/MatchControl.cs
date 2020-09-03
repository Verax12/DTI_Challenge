using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DTI_Challenge.Models
{
    public class MatchControl
    {
        public Guid Id { get; set; }
        public Player playerOne
        {
            get
            {
                return new Player
                {
                    PlayerLetter = "X",
                    PlayerId = 1
                };
            }
        }
        public Player playerTwo
        {
            get
            {
                return new Player
                {
                    PlayerLetter = "O",
                    PlayerId = 2
                };
            }
        }

        public Random Random { get; set; }
        public string FirstPlayer { get; set; }

        public string LastPlayer { get; set; }

        public Dictionary<int, Dictionary<int, int>> PlayerXPositions { get; set; }
        public Dictionary<int, Dictionary<int, int>> PlayerOPositions { get; set; }




        public EndGame Winner { get; set; }
    }

}
