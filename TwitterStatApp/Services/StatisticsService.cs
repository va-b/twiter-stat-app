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

        public IEnumerable<TweetStatistic> GetTweetLikesStatisticByUsers(IReadOnlyCollection<string> userNames)
        {
            var res = new List<TweetStatistic>(userNames.Count);
            Parallel.ForEach(userNames, username =>
            {
                res.Add(GetTweetLikesStatisticForSingleUser(username));
            });
            return res;
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
            
            const int summaryElement = 24;

            var stat = new TweetStatistic
            {
                UserName = username,
                LikesTimeRange = new int[summaryElement + 1],
                MedianTimeRange = new double[summaryElement + 1],
                TweetsTimeRange = new int[summaryElement + 1]
            };



            stat.LikesTimeRange[summaryElement] = 0;
            stat.TweetsTimeRange[summaryElement] = 0;
            for (var i = 0; i < summaryElement; i++)
            {
                var tweetsForTime = tweets
                    .Where(y => y.PostingDate.Hour == i)
                    .OrderBy(x => x.LikesCount)
                    .ToArray();
                stat.LikesTimeRange[i] = tweetsForTime.Sum(x => x.LikesCount);
                stat.TweetsTimeRange[i] = tweetsForTime.Length;
                stat.MedianTimeRange[i] = tweetsForTime.Length == 0 ? 0.0 
                    : tweetsForTime.Length % 2 != 0 ? tweetsForTime[tweetsForTime.Length / 2].LikesCount
                    : (tweetsForTime[tweetsForTime.Length / 2 - 1].LikesCount + tweetsForTime[tweetsForTime.Length / 2].LikesCount) / 2.0;
                stat.LikesTimeRange[summaryElement] += stat.LikesTimeRange[i];
                stat.TweetsTimeRange[summaryElement] += stat.TweetsTimeRange[i];
            }

            var medianArr = tweets.OrderBy(x => x.LikesCount).Select(x => x.LikesCount).ToArray();
            stat.MedianTimeRange[summaryElement] = tweets.Count % 2 != 0
                ? medianArr[tweets.Count / 2]
                : Math.Round((medianArr[tweets.Count / 2] + medianArr[tweets.Count / 2 + 1]) / 2.0, 2);

            return stat;
        }
    }
}