import IHttpClient from "@/services/IHttpClient";
import TweetStatisticModel from "@/model/TweetStatisticModel";

export default class TwitterStatAppService
{
    constructor(private readonly client: IHttpClient)
    {}
    
    public GetStatistic(usernames: string[]): Promise<TweetStatisticModel[]>
    {
        let url = new URL(location.origin + "/api/GetStatistic");
        usernames.forEach(x => url.searchParams.append('username', x));
        return this.client.get(url.toString());
    }
    
    public async FindUsers(name: string): Promise<string[]>
    {
        let url = new URL(location.origin + "/api/FindUsers");
        url.searchParams.append('name', name);
        return this.client.get(url.toString());
    }
}