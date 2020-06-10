using System;

namespace TwitterStatApp.Services.Twitter.Model
{
    public sealed class Tweet
    {
        public int LikesCount { get; internal set; }
        public DateTime PostingDate { get; internal set; }
    }
}