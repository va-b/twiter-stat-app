import { CustomJsonParser } from "@/utils/JsonHelpers";
import IHttpClient from "@/services/IHttpClient";

export default class Fetcher implements IHttpClient
{
    private readonly fetchParams: RequestInit;

    constructor()
    {
        let headers = new Headers();
        headers.set('Content-type', 'application/json');
        //headers.set("Cache-Control", "no-cache");

        this.fetchParams = {
            credentials: "same-origin",
            headers: headers
        };
    }

    private async handleResponse (response: Response): Promise<any>
    {
        if(!response.ok)
        {
            let msg: string;
            switch (response.status)
            {
                case 401:
                    msg = "NOT_AUTHORIZED";
                    break;
                case 400:
                    msg = "FRONTEND_ERROR";
                    break;
                case 404:
                    msg = "NOT_FOUND_ERROR";
                    break;
                case 500:
                    msg = "SERVER_ERROR";
                    break;
                case 502:
                    msg = "CONNECTION_ERROR";
                    break;
                default:
                    msg = JSON.stringify(response, CustomJsonParser);
            }
            throw msg;
        }
        else
        {
            let contentType = response.headers.get("content-type");
            if(contentType?.includes("application/json"))
            {
                return JSON.parse(await response.text(), CustomJsonParser);
            }
            else
            {
                return response.text();
            }
        }
    }



    public async delete<T>( url: string ): Promise<T>
    {
        let params: RequestInit = { method: "DELETE", ...this.fetchParams };
        let resp = await fetch(url, params);
        let res = await this.handleResponse(resp);
        return res as T;
    }

    public async get<T>( url: string ): Promise<T>
    {
        let params: RequestInit = { method: "GET", ...this.fetchParams };
        let resp = await fetch(url, params);
        let res = await this.handleResponse(resp);
        return res as T;
    }

    public async patch<T>( url: string, data: any ): Promise<T>
    {
        let params: RequestInit = { method: "PATCH", body: data, ...this.fetchParams };
        let resp = await fetch(url, params);
        let res = await this.handleResponse(resp);
        return res as T;
    }

    public async post<T>( url: string, data: any ): Promise<T>
    {
        let params: RequestInit = { method: "POST", body: data, ...this.fetchParams };

        if(data instanceof FormData && params.headers !== undefined)
        {
            delete params.headers;
        }

        let resp = await fetch(url, params);
        let res = await this.handleResponse(resp);
        return res as T;
    }

    public async put<T>( url: string, data: any ): Promise<T>
    {
        let params: RequestInit = { method: "PUT", body: data, ...this.fetchParams };
        let resp = await fetch(url, params);
        let res = await this.handleResponse(resp);
        return res as T;
    }
}