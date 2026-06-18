import { LitElement as e, css as t, html as n } from "@umbraco-cms/backoffice/external/lit";
import { umbHttpClient as r } from "@umbraco-cms/backoffice/http-client";
//#region src/api/core/bodySerializer.gen.ts
var i = { bodySerializer: (e) => JSON.stringify(e, (e, t) => typeof t == "bigint" ? t.toString() : t) };
Object.entries({
	$body_: "body",
	$headers_: "headers",
	$path_: "path",
	$query_: "query"
});
//#endregion
//#region src/api/core/serverSentEvents.gen.ts
var a = ({ onRequest: e, onSseError: t, onSseEvent: n, responseTransformer: r, responseValidator: i, sseDefaultRetryDelay: a, sseMaxRetryAttempts: o, sseMaxRetryDelay: s, sseSleepFn: c, url: l, ...u }) => {
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
}, o = (e) => {
	switch (e) {
		case "label": return ".";
		case "matrix": return ";";
		case "simple": return ",";
		default: return "&";
	}
}, s = (e) => {
	switch (e) {
		case "form": return ",";
		case "pipeDelimited": return "|";
		case "spaceDelimited": return "%20";
		default: return ",";
	}
}, c = (e) => {
	switch (e) {
		case "label": return ".";
		case "matrix": return ";";
		case "simple": return ",";
		default: return "&";
	}
}, l = ({ allowReserved: e, explode: t, name: n, style: r, value: i }) => {
	if (!t) {
		let t = (e ? i : i.map((e) => encodeURIComponent(e))).join(s(r));
		switch (r) {
			case "label": return `.${t}`;
			case "matrix": return `;${n}=${t}`;
			case "simple": return t;
			default: return `${n}=${t}`;
		}
	}
	let a = o(r), c = i.map((t) => r === "label" || r === "simple" ? e ? t : encodeURIComponent(t) : u({
		allowReserved: e,
		name: n,
		value: t
	})).join(a);
	return r === "label" || r === "matrix" ? a + c : c;
}, u = ({ allowReserved: e, name: t, value: n }) => {
	if (n == null) return "";
	if (typeof n == "object") throw Error("Deeply-nested arrays/objects aren’t supported. Provide your own `querySerializer()` to handle these.");
	return `${t}=${e ? n : encodeURIComponent(n)}`;
}, d = ({ allowReserved: e, explode: t, name: n, style: r, value: i, valueOnly: a }) => {
	if (i instanceof Date) return a ? i.toISOString() : `${n}=${i.toISOString()}`;
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
	let o = c(r), s = Object.entries(i).map(([t, i]) => u({
		allowReserved: e,
		name: r === "deepObject" ? `${n}[${t}]` : t,
		value: i
	})).join(o);
	return r === "label" || r === "matrix" ? o + s : s;
}, f = /\{[^{}]+\}/g, p = ({ path: e, url: t }) => {
	let n = t, r = t.match(f);
	if (r) for (let t of r) {
		let r = !1, i = t.substring(1, t.length - 1), a = "simple";
		i.endsWith("*") && (r = !0, i = i.substring(0, i.length - 1)), i.startsWith(".") ? (i = i.substring(1), a = "label") : i.startsWith(";") && (i = i.substring(1), a = "matrix");
		let o = e[i];
		if (o == null) continue;
		if (Array.isArray(o)) {
			n = n.replace(t, l({
				explode: r,
				name: i,
				style: a,
				value: o
			}));
			continue;
		}
		if (typeof o == "object") {
			n = n.replace(t, d({
				explode: r,
				name: i,
				style: a,
				value: o,
				valueOnly: !0
			}));
			continue;
		}
		if (a === "matrix") {
			n = n.replace(t, `;${u({
				name: i,
				value: o
			})}`);
			continue;
		}
		let s = encodeURIComponent(a === "label" ? `.${o}` : o);
		n = n.replace(t, s);
	}
	return n;
}, m = ({ baseUrl: e, path: t, query: n, querySerializer: r, url: i }) => {
	let a = i.startsWith("/") ? i : `/${i}`, o = (e ?? "") + a;
	t && (o = p({
		path: t,
		url: o
	}));
	let s = n ? r(n) : "";
	return s.startsWith("?") && (s = s.substring(1)), s && (o += `?${s}`), o;
};
function h(e) {
	let t = e.body !== void 0;
	if (t && e.bodySerializer) return "serializedBody" in e ? e.serializedBody !== void 0 && e.serializedBody !== "" ? e.serializedBody : null : e.body === "" ? null : e.body;
	if (t) return e.body;
}
//#endregion
//#region src/api/core/auth.gen.ts
var g = async (e, t) => {
	let n = typeof t == "function" ? await t(e) : t;
	if (n) return e.scheme === "bearer" ? `Bearer ${n}` : e.scheme === "basic" ? `Basic ${btoa(n)}` : n;
}, _ = ({ allowReserved: e, array: t, object: n } = {}) => (r) => {
	let i = [];
	if (r && typeof r == "object") for (let a in r) {
		let o = r[a];
		if (o != null) if (Array.isArray(o)) {
			let n = l({
				allowReserved: e,
				explode: !0,
				name: a,
				style: "form",
				value: o,
				...t
			});
			n && i.push(n);
		} else if (typeof o == "object") {
			let t = d({
				allowReserved: e,
				explode: !0,
				name: a,
				style: "deepObject",
				value: o,
				...n
			});
			t && i.push(t);
		} else {
			let t = u({
				allowReserved: e,
				name: a,
				value: o
			});
			t && i.push(t);
		}
	}
	return i.join("&");
}, v = (e) => {
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
}, y = (e, t) => t ? !!(e.headers.has(t) || e.query?.[t] || e.headers.get("Cookie")?.includes(`${t}=`)) : !1, b = async ({ security: e, ...t }) => {
	for (let n of e) {
		if (y(t, n.name)) continue;
		let e = await g(n, t.auth);
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
}, x = (e) => m({
	baseUrl: e.baseUrl,
	path: e.path,
	query: e.query,
	querySerializer: typeof e.querySerializer == "function" ? e.querySerializer : _(e.querySerializer),
	url: e.url
}), S = (e, t) => {
	let n = {
		...e,
		...t
	};
	return n.baseUrl?.endsWith("/") && (n.baseUrl = n.baseUrl.substring(0, n.baseUrl.length - 1)), n.headers = w(e.headers, t.headers), n;
}, C = (e) => {
	let t = [];
	return e.forEach((e, n) => {
		t.push([n, e]);
	}), t;
}, w = (...e) => {
	let t = new Headers();
	for (let n of e) {
		if (!n) continue;
		let e = n instanceof Headers ? C(n) : Object.entries(n);
		for (let [n, r] of e) if (r === null) t.delete(n);
		else if (Array.isArray(r)) for (let e of r) t.append(n, e);
		else r !== void 0 && t.set(n, typeof r == "object" ? JSON.stringify(r) : r);
	}
	return t;
}, T = class {
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
}, E = () => ({
	error: new T(),
	request: new T(),
	response: new T()
}), D = _({
	allowReserved: !1,
	array: {
		explode: !0,
		style: "form"
	},
	object: {
		explode: !0,
		style: "deepObject"
	}
}), O = { "Content-Type": "application/json" }, k = (e = {}) => ({
	...i,
	headers: O,
	parseAs: "auto",
	querySerializer: D,
	...e
}), A = ((e = {}) => {
	let t = S(k(), e), n = () => ({ ...t }), r = (e) => (t = S(t, e), n()), i = E(), o = async (e) => {
		let n = {
			...t,
			...e,
			fetch: e.fetch ?? t.fetch ?? globalThis.fetch,
			headers: w(t.headers, e.headers),
			serializedBody: void 0
		};
		return n.security && await b({
			...n,
			security: n.security
		}), n.requestValidator && await n.requestValidator(n), n.body !== void 0 && n.bodySerializer && (n.serializedBody = n.bodySerializer(n.body)), (n.body === void 0 || n.serializedBody === "") && n.headers.delete("Content-Type"), {
			opts: n,
			url: x(n)
		};
	}, s = async (e) => {
		let { opts: t, url: n } = await o(e), r = {
			redirect: "follow",
			...t,
			body: h(t)
		}, a = new Request(n, r);
		for (let e of i.request.fns) e && (a = await e(a, t));
		let s = t.fetch, c = await s(a);
		for (let e of i.response.fns) e && (c = await e(c, a, t));
		let l = {
			request: a,
			response: c
		};
		if (c.ok) {
			let e = (t.parseAs === "auto" ? v(c.headers.get("Content-Type")) : t.parseAs) ?? "json";
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
		let f = d ?? u, p = f;
		for (let e of i.error.fns) e && (p = await e(f, c, a, t));
		if (p ||= {}, t.throwOnError) throw p;
		return t.responseStyle === "data" ? void 0 : {
			error: p,
			...l
		};
	}, c = (e) => (t) => s({
		...t,
		method: e
	}), l = (e) => async (t) => {
		let { opts: n, url: r } = await o(t);
		return a({
			...n,
			body: n.body,
			headers: n.headers,
			method: e,
			onRequest: async (e, t) => {
				let r = new Request(e, t);
				for (let e of i.request.fns) e && (r = await e(r, n));
				return r;
			},
			url: r
		});
	};
	return {
		buildUrl: x,
		connect: c("CONNECT"),
		delete: c("DELETE"),
		get: c("GET"),
		getConfig: n,
		head: c("HEAD"),
		interceptors: i,
		options: c("OPTIONS"),
		patch: c("PATCH"),
		post: c("POST"),
		put: c("PUT"),
		request: s,
		setConfig: r,
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
})(((e) => ({
	...e,
	...r.getConfig()
}))(k({ baseUrl: "https://localhost:5000,https://localhost:44353" }))), j = class {
	static CreateBooking(e) {
		return (e?.client ?? A).post({
			security: [{
				scheme: "bearer",
				type: "http"
			}],
			url: "/bookings",
			...e
		});
	}
	static ChangeBookingStatus(e) {
		return (e?.client ?? A).put({
			security: [{
				scheme: "bearer",
				type: "http"
			}],
			url: "/bookings/{bookingId}/{action}",
			...e
		});
	}
	static GetBookings(e) {
		return (e?.client ?? A).get({
			security: [{
				scheme: "bearer",
				type: "http"
			}],
			url: "/bookings",
			...e
		});
	}
	static whatsMyName(e) {
		return (e?.client ?? A).get({
			security: [{
				scheme: "bearer",
				type: "http"
			}],
			url: "/umbraco/umbracoextension/api/v1/whatsMyName",
			...e
		});
	}
	static whatsTheTimeMrWolf(e) {
		return (e?.client ?? A).get({
			security: [{
				scheme: "bearer",
				type: "http"
			}],
			url: "/umbraco/umbracoextension/api/v1/whatsTheTimeMrWolf",
			...e
		});
	}
	static whoAmI(e) {
		return (e?.client ?? A).get({
			security: [{
				scheme: "bearer",
				type: "http"
			}],
			url: "/umbraco/umbracoextension/api/v1/whoAmI",
			...e
		});
	}
}, M = class extends e {
	constructor(...e) {
		super(...e), this.email = "", this.startDate = "", this.endDate = "", this.comment = "", this.photoPackageId = "", this.loading = !1;
	}
	static {
		this.properties = {
			email: { state: !0 },
			startDate: { state: !0 },
			endDate: { state: !0 },
			comment: { state: !0 },
			photoPackageId: { state: !0 },
			loading: { state: !0 },
			error: { state: !0 }
		};
	}
	async createBooking() {
		if (this.error = void 0, !this.email || !this.startDate || !this.endDate) {
			this.error = "E-mail, start date and end date are required";
			return;
		}
		this.loading = !0;
		let { error: e } = await j.CreateBooking({
			query: { email: this.email },
			body: {
				startDate: new Date(this.startDate).toISOString(),
				endDate: new Date(this.endDate).toISOString(),
				comment: this.comment,
				photoPackageId: this.photoPackageId
			}
		});
		if (this.loading = !1, e) {
			this.error = "Could not create booking";
			return;
		}
		this.email = "", this.startDate = "", this.endDate = "", this.comment = "", this.photoPackageId = "", this.dispatchEvent(new CustomEvent("booking-created", {
			bubbles: !0,
			composed: !0
		}));
	}
	render() {
		return n`
      <uui-box headline="Create booking">
        ${this.error ? n`<p class="error">${this.error}</p>` : null}

        <div class="form">
          <label>
            E-mail
            <input
              type="email"
              .value=${this.email}
              @input=${(e) => this.email = e.target.value}
            />
          </label>

          <label>
            Start date
            <input
              type="datetime-local"
              .value=${this.startDate}
              @input=${(e) => this.startDate = e.target.value}
            />
          </label>

          <label>
            End date
            <input
              type="datetime-local"
              .value=${this.endDate}
              @input=${(e) => this.endDate = e.target.value}
            />
          </label>

          <label>
            Comment
            <textarea
              .value=${this.comment}
              @input=${(e) => this.comment = e.target.value}
            ></textarea>
          </label>
          <label>
            Photo package ID
            <input
              type="text"
              .value=${this.photoPackageId}
              @input=${(e) => this.photoPackageId = e.target.value}
            />
          </label>

          <uui-button
            look="primary"
            color="positive"
            label="Create booking"
            ?disabled=${this.loading}
            @click=${this.createBooking}
          >
            ${this.loading ? "Creating..." : "Create booking"}
          </uui-button>
        </div>
      </uui-box>
    `;
	}
	static {
		this.styles = t`
    .form {
      display: grid;
      gap: 16px;
      max-width: 420px;
    }

    label {
      display: grid;
      gap: 6px;
      font-weight: 600;
    }

    input,
    textarea {
      padding: 8px;
      border: 1px solid #d8d7d9;
      border-radius: 3px;
      font: inherit;
    }

    textarea {
      min-height: 80px;
    }

    .error {
      color: #d42054;
    }
  `;
	}
};
customElements.define("create-booking", M);
//#endregion
//#region src/dashboards/booking.dashboard.ts
var N = class extends e {
	constructor(...e) {
		super(...e), this.bookings = [], this.loading = !0, this.selectedStatus = "propose", this.savingStatus = !1, this.statuses = [
			"approve",
			"propose",
			"reject"
		];
	}
	static {
		this.properties = {
			bookings: { state: !0 },
			loading: { state: !0 },
			error: { state: !0 },
			selectedBooking: { state: !0 },
			selectedStatus: { state: !0 },
			savingStatus: { state: !0 }
		};
	}
	connectedCallback() {
		super.connectedCallback(), this.loadBookings();
	}
	async loadBookings() {
		let { data: e, error: t } = await j.GetBookings();
		if (this.loading = !1, t) {
			this.error = "Could not load bookings";
			return;
		}
		this.bookings = e ?? [];
	}
	openStatusPopup(e) {
		this.selectedBooking = e, this.selectedStatus = e.status ?? "propose";
	}
	closeStatusPopup() {
		this.savingStatus || (this.selectedBooking = void 0, this.selectedStatus = "propose");
	}
	async saveStatus() {
		if (!this.selectedBooking) return;
		this.savingStatus = !0;
		let e = this.selectedBooking.id, t = this.selectedStatus, { error: n } = await j.ChangeBookingStatus({ path: {
			bookingId: e,
			action: t
		} });
		if (this.savingStatus = !1, n) {
			this.error = "Could not update booking status";
			return;
		}
		this.bookings = this.bookings.map((n) => n.id === e ? {
			...n,
			status: t
		} : n), this.closeStatusPopup();
	}
	render() {
		return this.loading ? n`
        <uui-box headline="Bookings">
          <uui-loader></uui-loader>
        </uui-box>
      ` : this.error ? n`
        <uui-box headline="Bookings">
          <p>${this.error}</p>
        </uui-box>
      ` : n`
      <create-booking @booking-created=${this.loadBookings}></create-booking>

      <uui-box headline="Bookings">
        <uui-table>
          <uui-table-head>
            <uui-table-head-cell>Status</uui-table-head-cell>
            <uui-table-head-cell>Package</uui-table-head-cell>
            <uui-table-head-cell>Start</uui-table-head-cell>
            <uui-table-head-cell>End</uui-table-head-cell>
            <uui-table-head-cell>Comment</uui-table-head-cell>
            <uui-table-head-cell>Action</uui-table-head-cell>
          </uui-table-head>

          ${this.bookings.map((e) => n`
              <uui-table-row>
                <uui-table-cell>${e.status}</uui-table-cell>

                <uui-table-cell>
                  ${e.photoPackage?.name ?? "-"}
                </uui-table-cell>

                <uui-table-cell>
                  ${this.formatDate(e.startDate)}
                </uui-table-cell>

                <uui-table-cell>
                  ${this.formatDate(e.endDate)}
                </uui-table-cell>

                <uui-table-cell>${e.comment}</uui-table-cell>

                <uui-table-cell>
                  <uui-button
                    look="secondary"
                    label="Change status"
                    @click=${() => this.openStatusPopup(e)}
                  >
                    Change status
                  </uui-button>
                </uui-table-cell>
              </uui-table-row>
            `)}
        </uui-table>
      </uui-box>

      ${this.renderStatusPopup()}
    `;
	}
	renderStatusPopup() {
		return this.selectedBooking ? n`
      <div class="modal-backdrop">
        <div class="modal">
          <uui-box headline="Change booking status">
            <p>
              Current status:
              <strong>${this.selectedBooking.status}</strong>
            </p>

            <label>Status</label>

            <select
              .value=${this.selectedStatus}
              @change=${(e) => this.selectedStatus = e.target.value}
            >
              ${this.statuses.map((e) => n`
                  <option value=${e}>${e}</option>
                `)}
            </select>

            <div class="actions">
              <uui-button
                look="secondary"
                label="Cancel"
                ?disabled=${this.savingStatus}
                @click=${this.closeStatusPopup}
              >
                Cancel
              </uui-button>

              <uui-button
                look="primary"
                color="positive"
                label="Save"
                ?disabled=${this.savingStatus}
                @click=${this.saveStatus}
              >
                ${this.savingStatus ? "Saving..." : "Save"}
              </uui-button>
            </div>
          </uui-box>
        </div>
      </div>
    ` : null;
	}
	formatDate(e) {
		return new Date(e).toLocaleString();
	}
	static {
		this.styles = t`
    uui-table {
      width: 100%;
    }

    .modal-backdrop {
      position: fixed;
      inset: 0;
      background: rgba(0, 0, 0, 0.35);
      display: flex;
      align-items: center;
      justify-content: center;
      z-index: 9999;
    }

    .modal {
      width: 420px;
      max-width: calc(100vw - 32px);
    }

    label {
      display: block;
      margin-bottom: 6px;
      font-weight: 600;
    }

    select {
      width: 100%;
      padding: 8px;
      border: 1px solid #d8d7d9;
      border-radius: 3px;
      margin-bottom: 16px;
    }

    .actions {
      display: flex;
      justify-content: flex-end;
      gap: 8px;
      margin-top: 16px;
    }
  `;
	}
};
customElements.define("booking-dashboard", N);
//#endregion
export { N as default };

//# sourceMappingURL=booking.dashboard-xbCd0DWu.js.map