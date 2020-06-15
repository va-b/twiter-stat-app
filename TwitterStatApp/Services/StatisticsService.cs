using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitterStatApp.Services.Twitter.Abstraction;
using TwitterStatApp.Services.Twitter.Model;
using TwitterStatApp.Model;

namespace TwitterStatApp.Services
{
    public sealed class StatisticsService
    {
        private readonly ITwitterService _twitterService;

        public StatisticsService(ITwitterService twitterService)
        {
            _twitterService = twitterService;
        }

        public async Task<IEnumerable<TweetStatistic>> GetTweetLikesStatisticByUsers(IReadOnlyCollection<string> userNames)
        {
            var getStatTasks = userNames
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => Task<TweetStatistic>.Factory.StartNew(() => GetTweetLikesStatisticForSingleUser(x)))
                .ToList();
            await Task.WhenAll(getStatTasks);
            return getStatTasks.Select(x => x.Result);
        }

        private TweetStatistic GetTweetLikesStatisticForSingleUser(string username)
        {
            var tweets = new HashSet<Tweet>();
            var thread = _twitterService.GetThread(username);
            for (var i = 0; i <= 2000;)
            {
                var resp = thread.GetTweets(i, 2000).ToList();
                if (resp.Count == 0) break;
                i += resp.Count;
                resp.ForEach(x => tweets.Add(x));
            }

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
                
            var medianArr = tweets.OrderBy(x => x.LikesCount).Select(x => x.LikesCount).ToArray();
            var median = tweets.Count % 2 != 0
                ? medianArr[tweets.Count / 2]
                : Math.Round((medianArr[tweets.Count / 2] + medianArr[tweets.Count / 2 + 1]) / 2.0, 2);

            return new TweetStatistic
            {
                UserName = username,
                Median = median,
                LikesTimeRange = hourRange,
                TotalLikes = hourRange.Sum(),
                TotalTweets = tweets.Count
            };
        }
    }
}