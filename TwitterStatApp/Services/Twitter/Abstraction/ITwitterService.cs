using System.Collections.Generic;
using System.Threading.Tasks;
using TwitterStatApp.Model;
using TwitterStatApp.Services.Twitter.Model;

namespace TwitterStatApp.Services.Twitter.Abstraction
{
    public interface ITwitterService
    {
        public Task<IEnumerable<TwitterUser>> FindUsers(string username);
        public Task<IEnumerable<Tweet>> GetTweetsAsync(long userId);
    }
}