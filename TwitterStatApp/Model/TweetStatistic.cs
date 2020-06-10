namespace TwitterStatApp.Model
{
    public sealed class TweetStatistic
    {
        public long UserId { get; set; }
        public int[] LikesTimeRange { get; set; }
        public int TotalTweets { get; set; }
        public int TotalLikes { get; set; }
        public double Median { get; set; }
    }
}