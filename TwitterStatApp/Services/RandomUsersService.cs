using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using TwitterStatApp.Model;
using TwitterStatApp.Services.Twitter.Model;

namespace TwitterStatApp.Services
{
    public sealed class RandomUsersService
    {
        private const int UsersCount = 1000;
        private readonly IOptions<AppConfig> _config;
        private readonly IMemoryCache _cache;

        public RandomUsersService(IOptions<AppConfig> config, IMemoryCache cache)
        {
            _config = config;
            _cache = cache;
        }

        public Task<IEnumerable<TwitterUser>> GetUsers()
            => _cache.GetOrCreateAsync("RandomUsersServiceCache", ce =>
            {
                ce.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);
                return GetUsersInternal();
            });


        private async Task<IEnumerable<TwitterUser>> GetUsersInternal()
        {
            var users = new List<TwitterUser>(UsersCount);

            using var client = new HttpClient();
            await using var respStream = await client.GetStreamAsync(_config.Value.RandomNamesApi);
            using var streamReader = new StreamReader(respStream);
            int i = 0;
            while (!streamReader.EndOfStream)
            {
                users.Add(new TwitterUser
                {
                    Id = ++i, 
                    Name = await streamReader.ReadLineAsync()
                });
            }

            return users;
        }
    }
}