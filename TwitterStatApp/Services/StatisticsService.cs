using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using TwitterStatApp.Services.Twitter.Abstraction;
using TwitterStatApp.Services.Twitter.Model;
using TwitterStatApp.Model;

namespace TwitterStatApp.Services
{
    public sealed class StatisticsService
    {
        private readonly ITwitterService _twitterService;
        private readonly IMemoryCache _cache;

        public StatisticsService(ITwitterService twitterService, IMemoryCache cache)
        {
            _twitterService = twitterService;
            _cache = cache;
        }

        public async Task<IEnumerable<TweetStatistic>> GetTweetLikesStatisticByUsers(IReadOnlyCollection<long> userIds)
        {
            var res = new List<TweetStatistic>(userIds.Count);
            
            await foreach (var item in TweetsForMultipleUsers(userIds))
            {
                res.Add(new TweetStatistic
                {
                    UserId = item.userId,
                    TotalTweets = item.tweets.Count(),
                });
            }

            return res;
        }

        private async IAsyncEnumerable<(long userId, IEnumerable<Tweet> tweets)> TweetsForMultipleUsers(IReadOnlyCollection<long> userIds)
        {
            foreach (var u in userIds)
            {
                var res = await _cache.GetOrCreateAsync(u, ce =>
                {
                    ce.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                    return _twitterService.GetTweetsAsync(u);
                });
                yield return (u, res);
            }
        }

    }
}