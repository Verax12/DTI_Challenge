using DTI_Challenge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DTI_Challenge.Application
{
    public interface IMatchApplication
    {
        MatchResume Start();
        string MatchMoviment(MatchMoviment matchMoviment);
        //MatchMoviment MatchMoviment(MatchMoviment, string);
    }
}
