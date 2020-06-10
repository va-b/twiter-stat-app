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

            await foreach (var (userId, tweets) in TweetsForMultipleUsers(userIds))
            {
                var hourRange = Enumerable
                    .Range(0, 24)
                    .Select(x =>
                    {
                        return tweets
                            .Where(y => y.PostingDate.Hour == x)
                            .Select(y => y.LikesCount)
                            .DefaultIfEmpty(0).Sum();
                    })
                    .ToArray();
                
                //Пока наивная реализация, потом мб взять MathNet.Numeric
                var medianArr = tweets.OrderBy(x => x.LikesCount).Select(x => x.LikesCount).ToArray();
                var median = tweets.Count % 2 != 0
                    ? medianArr[tweets.Count / 2]
                    : Math.Round((medianArr[tweets.Count / 2] + medianArr[tweets.Count / 2 + 1]) / 2.0, 2);

                res.Add(new TweetStatistic
                {
                    UserId = userId,
                    Median = median,
                    LikesTimeRange = hourRange,
                    TotalLikes = hourRange.Sum(),
                    TotalTweets = tweets.Count
                });
            }
            return res;
        }

        private async IAsyncEnumerable<(long userId, IList<Tweet> tweets)> TweetsForMultipleUsers(IEnumerable<long> userIds)
        {
            foreach (var u in userIds)
            {
                var res = await _cache.GetOrCreateAsync(u, ce =>
                {
                    ce.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                    return _twitterService.GetTweetsAsync(u);
                });
                yield return (u, res.ToArray());
            }
        }
    }
}