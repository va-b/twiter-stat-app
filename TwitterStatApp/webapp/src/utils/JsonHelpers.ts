export function CustomJsonParser(key: string, val: any): any
{
	let type = typeof (val);
	let dateRegExp: RegExp = /\d{4}-[01]\d-[0-3]\d(T[0-2]\d:[0-5]\d:[0-5]\d([+-][0-2]\d:[0-5]\d|Z|))?/;


	if (type === "string" && dateRegExp.test(val))
	{
		let r = new Date(val);
		return isNaN(r.getTime()) ? val : r;
	}
	else return val;
}

export function CustomJsonStringify(this: any, key: string, val: any): any
{
	let item = this[key];
	if(item instanceof Date)
	{
		let r = new Date(item);
		r.setHours(r.getHours() - r.getTimezoneOffset()/60);
		r.setMinutes(r.getMinutes() - r.getTimezoneOffset()%60);
		r.setMilliseconds(0);
		return r.toISOString();
	}
	return val;
}