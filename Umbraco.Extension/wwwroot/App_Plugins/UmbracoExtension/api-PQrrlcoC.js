import { umbHttpClient as e } from "@umbraco-cms/backoffice/http-client";
//#region src/api/core/bodySerializer.gen.ts
var t = { bodySerializer: (e) => JSON.stringify(e, (e, t) => typeof t == "bigint" ? t.toString() : t) };
Object.entries({
	$body_: "body",
	$headers_: "headers",
	$path_: "path",
	$query_: "query"
});
//#endregion
//#region src/api/core/serverSentEvents.gen.ts
var n = ({ onRequest: e, onSseError: t, onSseEvent: n, responseTransformer: r, responseValidator: i, sseDefaultRetryDelay: a, sseMaxRetryAttempts: o, sseMaxRetryDelay: s, sseSleepFn: c, url: l, ...u }) => {
	let d, f = c ?? ((e) => new Promise((t) => setTimeout(t, e)));
	return { stream: async function* () {
		let c = a ?? 3e3, p = 0, m = u.signal ?? new AbortController().signal;
		for (; !m.aborted;) {
			p++;
			let a = u.headers instanceof Headers ? u.headers : new Headers(u.headers);
			d !== void 0 && a.set("Last-Event-ID", d);
			try {
				let t = {
					redirect: "follow",
					...u,
					body: u.serializedBody,
					headers: a,
					signal: m
				}, o = new Request(l, t);
				e && (o = await e(l, t));
				let s = await (u.fetch ?? globalThis.fetch)(o);
				if (!s.ok) throw Error(`SSE failed: ${s.status} ${s.statusText}`);
				if (!s.body) throw Error("No body in SSE response");
				let f = s.body.pipeThrough(new TextDecoderStream()).getReader(), p = "", h = () => {
					try {
						f.cancel();
					} catch {}
				};
				m.addEventListener("abort", h);
				try {
					for (;;) {
						let { done: e, value: t } = await f.read();
						if (e) break;
						p += t;
						let a = p.split("\n\n");
						p = a.pop() ?? "";
						for (let e of a) {
							let t = e.split("\n"), a = [], o;
							for (let e of t) if (e.startsWith("data:")) a.push(e.replace(/^data:\s*/, ""));
							else if (e.startsWith("event:")) o = e.replace(/^event:\s*/, "");
							else if (e.startsWith("id:")) d = e.replace(/^id:\s*/, "");
							else if (e.startsWith("retry:")) {
								let t = Number.parseInt(e.replace(/^retry:\s*/, ""), 10);
								Number.isNaN(t) || (c = t);
							}
							let s, l = !1;
							if (a.length) {
								let e = a.join("\n");
								try {
									s = JSON.parse(e), l = !0;
								} catch {
									s = e;
								}
							}
							l && (i && await i(s), r && (s = await r(s))), n?.({
								data: s,
								event: o,
								id: d,
								retry: c
							}), a.length && (yield s);
						}
					}
				} finally {
					m.removeEventListener("abort", h), f.releaseLock();
				}
				break;
			} catch (e) {
				if (t?.(e), o !== void 0 && p >= o) break;
				await f(Math.min(c * 2 ** (p - 1), s ?? 3e4));
			}
		}
	}() };
}, r = (e) => {
	switch (e) {
		case "label": return ".";
		case "matrix": return ";";
		case "simple": return ",";
		default: return "&";
	}
}, i = (e) => {
	switch (e) {
		case "form": return ",";
		case "pipeDelimited": return "|";
		case "spaceDelimited": return "%20";
		default: return ",";
	}
}, a = (e) => {
	switch (e) {
		case "label": return ".";
		case "matrix": return ";";
		case "simple": return ",";
		default: return "&";
	}
}, o = ({ allowReserved: e, explode: t, name: n, style: a, value: o }) => {
	if (!t) {
		let t = (e ? o : o.map((e) => encodeURIComponent(e))).join(i(a));
		switch (a) {
			case "label": return `.${t}`;
			case "matrix": return `;${n}=${t}`;
			case "simple": return t;
			default: return `${n}=${t}`;
		}
	}
	let c = r(a), l = o.map((t) => a === "label" || a === "simple" ? e ? t : encodeURIComponent(t) : s({
		allowReserved: e,
		name: n,
		value: t
	})).join(c);
	return a === "label" || a === "matrix" ? c + l : l;
}, s = ({ allowReserved: e, name: t, value: n }) => {
	if (n == null) return "";
	if (typeof n == "object") throw Error("Deeply-nested arrays/objects aren’t supported. Provide your own `querySerializer()` to handle these.");
	return `${t}=${e ? n : encodeURIComponent(n)}`;
}, c = ({ allowReserved: e, explode: t, name: n, style: r, value: i, valueOnly: o }) => {
	if (i instanceof Date) return o ? i.toISOString() : `${n}=${i.toISOString()}`;
	if (r !== "deepObject" && !t) {
		let t = [];
		Object.entries(i).forEach(([n, r]) => {
			t = [
				...t,
				n,
				e ? r : encodeURIComponent(r)
			];
		});
		let a = t.join(",");
		switch (r) {
			case "form": return `${n}=${a}`;
			case "label": return `.${a}`;
			case "matrix": return `;${n}=${a}`;
			default: return a;
		}
	}
	let c = a(r), l = Object.entries(i).map(([t, i]) => s({
		allowReserved: e,
		name: r === "deepObject" ? `${n}[${t}]` : t,
		value: i
	})).join(c);
	return r === "label" || r === "matrix" ? c + l : l;
}, l = /\{[^{}]+\}/g, u = ({ path: e, url: t }) => {
	let n = t, r = t.match(l);
	if (r) for (let t of r) {
		let r = !1, i = t.substring(1, t.length - 1), a = "simple";
		i.endsWith("*") && (r = !0, i = i.substring(0, i.length - 1)), i.startsWith(".") ? (i = i.substring(1), a = "label") : i.startsWith(";") && (i = i.substring(1), a = "matrix");
		let l = e[i];
		if (l == null) continue;
		if (Array.isArray(l)) {
			n = n.replace(t, o({
				explode: r,
				name: i,
				style: a,
				value: l
			}));
			continue;
		}
		if (typeof l == "object") {
			n = n.replace(t, c({
				explode: r,
				name: i,
				style: a,
				value: l,
				valueOnly: !0
			}));
			continue;
		}
		if (a === "matrix") {
			n = n.replace(t, `;${s({
				name: i,
				value: l
			})}`);
			continue;
		}
		let u = encodeURIComponent(a === "label" ? `.${l}` : l);
		n = n.replace(t, u);
	}
	return n;
}, d = ({ baseUrl: e, path: t, query: n, querySerializer: r, url: i }) => {
	let a = i.startsWith("/") ? i : `/${i}`, o = (e ?? "") + a;
	t && (o = u({
		path: t,
		url: o
	}));
	let s = n ? r(n) : "";
	return s.startsWith("?") && (s = s.substring(1)), s && (o += `?${s}`), o;
};
function f(e) {
	let t = e.body !== void 0;
	if (t && e.bodySerializer) return "serializedBody" in e ? e.serializedBody !== void 0 && e.serializedBody !== "" ? e.serializedBody : null : e.body === "" ? null : e.body;
	if (t) return e.body;
}
//#endregion
//#region src/api/core/auth.gen.ts
var p = async (e, t) => {
	let n = typeof t == "function" ? await t(e) : t;
	if (n) return e.scheme === "bearer" ? `Bearer ${n}` : e.scheme === "basic" ? `Basic ${btoa(n)}` : n;
}, m = ({ allowReserved: e, array: t, object: n } = {}) => (r) => {
	let i = [];
	if (r && typeof r == "object") for (let a in r) {
		let l = r[a];
		if (l != null) if (Array.isArray(l)) {
			let n = o({
				allowReserved: e,
				explode: !0,
				name: a,
				style: "form",
				value: l,
				...t
			});
			n && i.push(n);
		} else if (typeof l == "object") {
			let t = c({
				allowReserved: e,
				explode: !0,
				name: a,
				style: "deepObject",
				value: l,
				...n
			});
			t && i.push(t);
		} else {
			let t = s({
				allowReserved: e,
				name: a,
				value: l
			});
			t && i.push(t);
		}
	}
	return i.join("&");
}, h = (e) => {
	if (!e) return "stream";
	let t = e.split(";")[0]?.trim();
	if (t) {
		if (t.startsWith("application/json") || t.endsWith("+json")) return "json";
		if (t === "multipart/form-data") return "formData";
		if ([
			"application/",
			"audio/",
			"image/",
			"video/"
		].some((e) => t.startsWith(e))) return "blob";
		if (t.startsWith("text/")) return "text";
	}
}, g = (e, t) => t ? !!(e.headers.has(t) || e.query?.[t] || e.headers.get("Cookie")?.includes(`${t}=`)) : !1, _ = async ({ security: e, ...t }) => {
	for (let n of e) {
		if (g(t, n.name)) continue;
		let e = await p(n, t.auth);
		if (!e) continue;
		let r = n.name ?? "Authorization";
		switch (n.in) {
			case "query":
				t.query ||= {}, t.query[r] = e;
				break;
			case "cookie":
				t.headers.append("Cookie", `${r}=${e}`);
				break;
			default:
				t.headers.set(r, e);
				break;
		}
	}
}, v = (e) => d({
	baseUrl: e.baseUrl,
	path: e.path,
	query: e.query,
	querySerializer: typeof e.querySerializer == "function" ? e.querySerializer : m(e.querySerializer),
	url: e.url
}), y = (e, t) => {
	let n = {
		...e,
		...t
	};
	return n.baseUrl?.endsWith("/") && (n.baseUrl = n.baseUrl.substring(0, n.baseUrl.length - 1)), n.headers = x(e.headers, t.headers), n;
}, b = (e) => {
	let t = [];
	return e.forEach((e, n) => {
		t.push([n, e]);
	}), t;
}, x = (...e) => {
	let t = new Headers();
	for (let n of e) {
		if (!n) continue;
		let e = n instanceof Headers ? b(n) : Object.entries(n);
		for (let [n, r] of e) if (r === null) t.delete(n);
		else if (Array.isArray(r)) for (let e of r) t.append(n, e);
		else r !== void 0 && t.set(n, typeof r == "object" ? JSON.stringify(r) : r);
	}
	return t;
}, S = class {
	constructor() {
		this.fns = [];
	}
	clear() {
		this.fns = [];
	}
	eject(e) {
		let t = this.getInterceptorIndex(e);
		this.fns[t] && (this.fns[t] = null);
	}
	exists(e) {
		let t = this.getInterceptorIndex(e);
		return !!this.fns[t];
	}
	getInterceptorIndex(e) {
		return typeof e == "number" ? this.fns[e] ? e : -1 : this.fns.indexOf(e);
	}
	update(e, t) {
		let n = this.getInterceptorIndex(e);
		return this.fns[n] ? (this.fns[n] = t, e) : !1;
	}
	use(e) {
		return this.fns.push(e), this.fns.length - 1;
	}
}, C = () => ({
	error: new S(),
	request: new S(),
	response: new S()
}), w = m({
	allowReserved: !1,
	array: {
		explode: !0,
		style: "form"
	},
	object: {
		explode: !0,
		style: "deepObject"
	}
}), T = { "Content-Type": "application/json" }, E = (e = {}) => ({
	...t,
	headers: T,
	parseAs: "auto",
	querySerializer: w,
	...e
}), D = ((e = {}) => {
	let t = y(E(), e), r = () => ({ ...t }), i = (e) => (t = y(t, e), r()), a = C(), o = async (e) => {
		let n = {
			...t,
			...e,
			fetch: e.fetch ?? t.fetch ?? globalThis.fetch,
			headers: x(t.headers, e.headers),
			serializedBody: void 0
		};
		return n.security && await _({
			...n,
			security: n.security
		}), n.requestValidator && await n.requestValidator(n), n.body !== void 0 && n.bodySerializer && (n.serializedBody = n.bodySerializer(n.body)), (n.body === void 0 || n.serializedBody === "") && n.headers.delete("Content-Type"), {
			opts: n,
			url: v(n)
		};
	}, s = async (e) => {
		let { opts: t, url: n } = await o(e), r = {
			redirect: "follow",
			...t,
			body: f(t)
		}, i = new Request(n, r);
		for (let e of a.request.fns) e && (i = await e(i, t));
		let s = t.fetch, c = await s(i);
		for (let e of a.response.fns) e && (c = await e(c, i, t));
		let l = {
			request: i,
			response: c
		};
		if (c.ok) {
			let e = (t.parseAs === "auto" ? h(c.headers.get("Content-Type")) : t.parseAs) ?? "json";
			if (c.status === 204 || c.headers.get("Content-Length") === "0") {
				let n;
				switch (e) {
					case "arrayBuffer":
					case "blob":
					case "text":
						n = await c[e]();
						break;
					case "formData":
						n = new FormData();
						break;
					case "stream":
						n = c.body;
						break;
					default:
						n = {};
						break;
				}
				return t.responseStyle === "data" ? n : {
					data: n,
					...l
				};
			}
			let n;
			switch (e) {
				case "arrayBuffer":
				case "blob":
				case "formData":
				case "json":
				case "text":
					n = await c[e]();
					break;
				case "stream": return t.responseStyle === "data" ? c.body : {
					data: c.body,
					...l
				};
			}
			return e === "json" && (t.responseValidator && await t.responseValidator(n), t.responseTransformer && (n = await t.responseTransformer(n))), t.responseStyle === "data" ? n : {
				data: n,
				...l
			};
		}
		let u = await c.text(), d;
		try {
			d = JSON.parse(u);
		} catch {}
		let p = d ?? u, m = p;
		for (let e of a.error.fns) e && (m = await e(p, c, i, t));
		if (m ||= {}, t.throwOnError) throw m;
		return t.responseStyle === "data" ? void 0 : {
			error: m,
			...l
		};
	}, c = (e) => (t) => s({
		...t,
		method: e
	}), l = (e) => async (t) => {
		let { opts: r, url: i } = await o(t);
		return n({
			...r,
			body: r.body,
			headers: r.headers,
			method: e,
			onRequest: async (e, t) => {
				let n = new Request(e, t);
				for (let e of a.request.fns) e && (n = await e(n, r));
				return n;
			},
			url: i
		});
	};
	return {
		buildUrl: v,
		connect: c("CONNECT"),
		delete: c("DELETE"),
		get: c("GET"),
		getConfig: r,
		head: c("HEAD"),
		interceptors: a,
		options: c("OPTIONS"),
		patch: c("PATCH"),
		post: c("POST"),
		put: c("PUT"),
		request: s,
		setConfig: i,
		sse: {
			connect: l("CONNECT"),
			delete: l("DELETE"),
			get: l("GET"),
			head: l("HEAD"),
			options: l("OPTIONS"),
			patch: l("PATCH"),
			post: l("POST"),
			put: l("PUT"),
			trace: l("TRACE")
		},
		trace: c("TRACE")
	};
})(((t) => ({
	...t,
	...e.getConfig()
}))(E({ baseUrl: "https://localhost:5000,https://localhost:44353" }))), O = class {
	static GetAllPhotoPackages(e) {
		return (e?.client ?? D).get({
			security: [{
				scheme: "bearer",
				type: "http"
			}],
			url: "/umbraco/umbracoextension/api/v1/photopackages",
			...e
		});
	}
	static CreatePhotoPackage(e) {
		return (e?.client ?? D).post({
			security: [{
				scheme: "bearer",
				type: "http"
			}],
			url: "/umbraco/umbracoextension/api/v1/photopackages",
			...e
		});
	}
	static CreateEventType(e) {
		return (e?.client ?? D).post({
			security: [{
				scheme: "bearer",
				type: "http"
			}],
			url: "/umbraco/umbracoextension/api/v1/eventtypes",
			...e
		});
	}
	static GetEventTypes(e) {
		return (e?.client ?? D).get({
			security: [{
				scheme: "bearer",
				type: "http"
			}],
			url: "/umbraco/umbracoextension/api/v1/eventtypes",
			...e
		});
	}
	static CreateBooking(e) {
		return (e?.client ?? D).post({
			security: [{
				scheme: "bearer",
				type: "http"
			}],
			url: "/bookings",
			...e
		});
	}
	static ChangeBookingStatus(e) {
		return (e?.client ?? D).put({
			security: [{
				scheme: "bearer",
				type: "http"
			}],
			url: "/bookings/{bookingId}/{action}",
			...e
		});
	}
	static GetBookings(e) {
		return (e?.client ?? D).get({
			security: [{
				scheme: "bearer",
				type: "http"
			}],
			url: "/bookings",
			...e
		});
	}
	static whatsMyName(e) {
		return (e?.client ?? D).get({
			security: [{
				scheme: "bearer",
				type: "http"
			}],
			url: "/umbraco/umbracoextension/api/v1/whatsMyName",
			...e
		});
	}
	static whatsTheTimeMrWolf(e) {
		return (e?.client ?? D).get({
			security: [{
				scheme: "bearer",
				type: "http"
			}],
			url: "/umbraco/umbracoextension/api/v1/whatsTheTimeMrWolf",
			...e
		});
	}
	static whoAmI(e) {
		return (e?.client ?? D).get({
			security: [{
				scheme: "bearer",
				type: "http"
			}],
			url: "/umbraco/umbracoextension/api/v1/whoAmI",
			...e
		});
	}
};
//#endregion
export { O as t };

//# sourceMappingURL=api-PQrrlcoC.js.map