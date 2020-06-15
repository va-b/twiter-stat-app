using System;
using System.Collections.Generic;
using System.Linq;
using TwitterStatApp.Services.Twitter.Model;

namespace TwitterStatApp.Services.Twitter
{
    public sealed class TwitterThread
    {
        private const int MaxTweetsCount = 2000;
        private const int MaxTweetsInResponse = 200;
        
        private readonly int _userId;
        private readonly int _totalTweetsCount;
        private readonly int _humanSleepTime;
        private readonly DateTime _timeBase = DateTime.Now;
        
        public TwitterThread(int userId)
        {
            _userId = userId;
            var rand = new Random(_userId);
            _totalTweetsCount = rand.Next(0, MaxTweetsCount);
            _humanSleepTime = rand.Next(16, 24);
        }
        
        public IEnumerable<Tweet> GetTweets(int skip, int count)
        {
            if(skip > _totalTweetsCount) return Enumerable.Empty<Tweet>();

            var rand = new Random(_userId);
            var tweetsToReturn = rand.Next(0, count > MaxTweetsInResponse ? MaxTweetsInResponse : count);
            return GetAllTweets().Skip(skip).Take(tweetsToReturn);
        }

        private IEnumerable<Tweet> GetAllTweets()
        {
            var rand = new Random(_userId);
            for (var i = 0; i <= _totalTweetsCount; i++)
            {
                yield return new Tweet
                {
                    LikesCount = rand.Next(0, 1000),
                    PostingDate =  _timeBase.AddHours(rand.Next(0, 24) % _humanSleepTime)
                };
                
            }
        }
    }
}