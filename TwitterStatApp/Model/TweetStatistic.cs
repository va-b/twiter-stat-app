namespace TwitterStatApp.Model
{
    public sealed class TweetStatistic
    {
        public string UserName { get; set; }
        public int[] LikesTimeRange { get; set; }
        public double[] MedianTimeRange { get; set; }
        public int[] TweetsTimeRange { get; set; }
    }
}