import Fetcher from "@/services/Fetcher";
import { PluginFunction, VueConstructor } from "vue";
import IHttpClient from "@/services/IHttpClient";
import {GetSrv} from "@/services/ServiceGetterDecorator";
import TwitterStatAppService from "@/services/TwitterStatAppService";

declare module 'vue/types/vue'
{
	interface Vue
	{
		readonly $sf: ServicesFactory;
	}
}
declare global
{
	interface Window
	{
		readonly $sf: ServicesFactory;
	}
}

export default class ServicesFactory
{
	public static install:  PluginFunction<object> = (Vue: VueConstructor) =>
	{
		Object.defineProperty(window, '$sf', {
			get: function get() { return new ServicesFactory(Vue) }
		});
		Object.defineProperty(Vue.prototype, '$sf', {
			get: function get() { return window.$sf }
		});
	};

	constructor(private readonly Vue: VueConstructor) {}
	
	@GetSrv() public get HttpClient(): IHttpClient
	{
		return new Fetcher();
	}

	@GetSrv() public get Tsa(): TwitterStatAppService
	{
		return new TwitterStatAppService(this.HttpClient);
	}
}