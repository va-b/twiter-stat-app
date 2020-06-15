using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace TwitterStatApp.Services
{
    public sealed class RandomUsersService
    {
        private const int UsersCount = 100;
        private readonly IOptions<AppConfig> _config;
        private readonly IMemoryCache _cache;

        public RandomUsersService(IOptions<AppConfig> config, IMemoryCache cache)
        {
            _config = config;
            _cache = cache;
        }

        public Task<IEnumerable<string>> GetUsers()
            => _cache.GetOrCreateAsync("RandomUsersServiceCache", ce =>
            {
                ce.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);
                return GetUsersInternal();
            });


        private async Task<IEnumerable<string>> GetUsersInternal()
        {
            var users = new List<string>(UsersCount);

            using var client = new HttpClient();
            await using var respStream = await client.GetStreamAsync(_config.Value.RandomNamesApi);
            using var streamReader = new StreamReader(respStream);
            while (!streamReader.EndOfStream)
            {
                var username = await streamReader.ReadLineAsync();
                users.Add(username);
            }

            return users;
        }
    }
}