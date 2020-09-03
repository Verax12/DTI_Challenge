using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DTI_Challenge.Application;
using DTI_Challenge.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace DTI_Challenge.Controllers
{
    [Route("api/game")]
    [ApiController]
    public class PlayController : ControllerBase
    {
        public IMatchApplication _matchApplication { get; set; }
        public IMemoryCache _memoryCache { get; set; }
        public PlayController(IMatchApplication matchApplication, IMemoryCache memoryCache)
        {
            _matchApplication = matchApplication;
            _memoryCache = memoryCache;
        }


        [HttpPost]
        [Route("new-game")]
        public async Task<ActionResult> Game()
        {
            MatchResume match = _matchApplication.Start();

            _memoryCache.Set(match.Id.ToString(), match);
            _memoryCache.Set("Round", 1);

            return Ok(JsonConvert.SerializeObject(match));
        }

        [HttpPost]
        [Route("{id}/movement")]
        public async Task<ActionResult> Movement(MatchMoviment match)
        {
            string msg = _matchApplication.MatchMoviment(match);

            return Ok(msg);

        }
    }
}
