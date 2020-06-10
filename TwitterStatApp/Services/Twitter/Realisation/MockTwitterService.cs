using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitterStatApp.Model;
using TwitterStatApp.Services.Twitter.Abstraction;
using TwitterStatApp.Services.Twitter.Model;

namespace TwitterStatApp.Services.Twitter.Realisation
{
    public sealed class MockTwitterService : ITwitterService
    {
        private readonly RandomUsersService _randomUsers;

        public MockTwitterService(RandomUsersService randomUsers)
        {
            _randomUsers = randomUsers;
        }

        public async Task<IEnumerable<TwitterUser>> FindUsers(string username)
        {
            var users = await _randomUsers.GetUsers();
            return string.IsNullOrEmpty(username) 
                ? users.Take(5).ToList() 
                : users.Where(x => x.Name.Contains(username, StringComparison.InvariantCultureIgnoreCase)).Take(5).ToList();
        }

        public async Task<IEnumerable<Tweet>> GetTweetsAsync(long userId)
        {
            var random = new Random((int)(userId % 1000));
            var tweetsCount = random.Next(10, 1000);
            var tweets = new List<Tweet>(tweetsCount);

            var humanSleepTime = random.Next(16, 24);
            
            for (var i = 0; i <= tweetsCount; i++) tweets.Add(new Tweet
            {
                LikesCount = random.Next(0, 1000),
                PostingDate =  DateTime.Now.AddHours(random.Next(0, 24) % humanSleepTime)
            });

            await Task.CompletedTask;
            return tweets;

        }
    }
}