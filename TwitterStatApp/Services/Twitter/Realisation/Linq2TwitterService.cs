using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using TwitterStatApp.Model;
using TwitterStatApp.Services.Twitter.Abstraction;
using TwitterStatApp.Services.Twitter.Model;

namespace TwitterStatApp.Services.Twitter.Realisation
{
    public sealed class Linq2TwitterService : ITwitterService
    {
        private readonly IOptions<AppConfig> _config;

        public Linq2TwitterService(IOptions<AppConfig> config)
        {
            _config = config;
        }
            
        public async Task<IEnumerable<TwitterUser>> FindUsers(string username)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IEnumerable<Tweet>> GetTweetsAsync(long userId)
        {
            throw new System.NotImplementedException();
        }
    }
}