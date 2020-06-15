using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitterStatApp.Services.Twitter.Abstraction;
using TwitterStatApp.Services.Twitter.Model;

namespace TwitterStatApp.Services.Twitter.Realisation
{
    public sealed class TwitterService : ITwitterService
    {
        private const int MaxTweetsCount = 2000;
        private const int MinTweetsCount = 0;
        private readonly RandomUsersService _randomUsers;
        
        private readonly ConcurrentDictionary<string, TwitterThread> _threads = new ConcurrentDictionary<string, TwitterThread>();

        public TwitterService(RandomUsersService randomUsers)
        {
            _randomUsers = randomUsers;
        }

        public async Task<IEnumerable<string>> FindUsers(string username)
        {
            var users = await _randomUsers.GetUsers();
            return string.IsNullOrEmpty(username) 
                ? users.Take(5).ToList() 
                : users.Where(x => x.Contains(username, StringComparison.InvariantCultureIgnoreCase)).Take(5).ToList();
        }

        public async Task<IEnumerable<Tweet>> GetTweetsAsync(long userId)
        {
            var random = new Random((int) userId % 10000);
            var tweetsCount = random.Next(MinTweetsCount, MaxTweetsCount);
            var tweets = new List<Tweet>(tweetsCount);

            var humanSleepTime = random.Next(16, 24);
            
            for (var i = 0; i <= tweetsCount; i++) tweets.Add(new Tweet
            {
                LikesCount = random.Next(MinTweetsCount, MaxTweetsCount),
                PostingDate =  DateTime.Now.AddHours(random.Next(0, 24) % humanSleepTime)
            });

            await Task.CompletedTask;
            return tweets;

        }

        public TwitterThread GetThread(string username) => 
            _threads.GetOrAdd(username, key => new TwitterThread(key.GetHashCode()));
    }
}