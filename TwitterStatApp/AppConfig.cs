namespace TwitterStatApp
{
    public sealed class AppConfig
    {
        public TwitterConfig Twitter { get; set; }
        public string RandomNamesApi { get; set; }
    }

    public sealed class TwitterConfig
    {
        public string OAuthConsumerKey { get; set; }
        public string OAuthConsumerSecret { get; set; }
    }
}