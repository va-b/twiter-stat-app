using System.Collections.Generic;
using System.Threading.Tasks;

namespace TwitterStatApp.Services.Twitter.Abstraction
{
    public interface ITwitterService
    {
        public Task<IEnumerable<string>> FindUsers(string username);
        public TwitterThread GetThread(string username);
    }
}