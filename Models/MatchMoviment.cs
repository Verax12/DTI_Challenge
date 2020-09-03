using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DTI_Challenge.Models
{
    public class MatchMoviment
    {
        public Guid Id { get; set; }
        public string Player { get; set; }
        public int Round { get; set; }
        public Position Position { get; set; }
    }

    public class Position
    {
        public int x { get; set; }
        public int y { get; set; }

    }
}
