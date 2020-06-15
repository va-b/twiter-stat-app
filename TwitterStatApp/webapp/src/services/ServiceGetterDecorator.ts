import Vue from "vue";

Object.defineProperty(window, '_servicesCache', { value: {} });

type ServiceGetterDecoratorParams =  {singleton?: boolean, observable?: boolean};

export function GetSrv<T extends string>(param: ServiceGetterDecoratorParams = {})
{
	let {singleton = true, observable = false} = param;
	return function (target, propertyKey: T, descriptor: TypedPropertyDescriptor<(typeof target)[T]>): void
	{
		const key = `${target.constructor.name}.${propertyKey}`;
		const getter = descriptor.get;

		descriptor.get = function(): (typeof target)[T]
		{
			let self = this;
			if(!!window._servicesCache[key]) return window._servicesCache[key];

			let res: (typeof target)[T] = getter!.apply(self);
			if(observable)
			{
				res = Vue.observable(res);
			}
			if(singleton)
			{
				window._servicesCache[key] = res;
			}
			return res;
		};
	}
}

declare global
{
	interface Window
	{
		readonly _servicesCache: {[key in string]: unknown};
	}
}
