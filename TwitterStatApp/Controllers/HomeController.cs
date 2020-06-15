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
        public ActionResult<IEnumerable<Tweet>> GetTweets(string username, int skip = 0, int count = 10)
        {
            var res = _twitterService.GetThread(username).GetTweets(skip, count);
            return Ok(res);
        }

        [HttpGet]
        public ActionResult<IEnumerable<TweetStatistic>> GetStatistic([FromQuery] string[] username)
        {
            var res = _statistics.GetTweetLikesStatisticByUsers(username);
            return Ok(res);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> FindUsers(string name)
        {
            var res = await _twitterService.FindUsers(name);
            return Ok(res);
        }
        
        
    }
}