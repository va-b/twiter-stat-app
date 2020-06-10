using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TwitterStatApp.Services.Twitter.Abstraction;
using TwitterStatApp.Services.Twitter.Model;
using TwitterStatApp.Model;
using TwitterStatApp.Services;

namespace TwitterStatApp.Controllers
{
    [Route("api/[action]")]
    public class HomeController : ControllerBase
    {
        private readonly ITwitterService _twitterService;
        private readonly StatisticsService _statistics;

        public HomeController(ITwitterService twitterService, StatisticsService statistics)
        {
            _twitterService = twitterService;
            _statistics = statistics;
        }


        [HttpGet]
        public ActionResult<string> Ping()
        {
            return Ok("Pong");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tweet>>> GetTweets(long userId)
        {
            var res = await _twitterService.GetTweetsAsync(userId);
            return Ok(res);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TweetStatistic>>> GetStatistic([FromQuery] long[] userId)
        {
            var res = await _statistics.GetTweetLikesStatisticByUsers(userId);
            return Ok(res);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TwitterUser>>> FindUsers(string name)
        {
            var res = await _twitterService.FindUsers(name);
            return Ok(res);
        }
        
        
    }
}