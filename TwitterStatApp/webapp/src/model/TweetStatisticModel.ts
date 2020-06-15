export default interface TweetStatisticModel
{
    userName: string;
    likesTimeRange: number[];
    totalTweets: number;
    totalLikes: number;
    median: number;
}