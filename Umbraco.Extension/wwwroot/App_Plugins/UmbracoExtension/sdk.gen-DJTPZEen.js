import { umbHttpClient as F } from "@umbraco-cms/backoffice/http-client";
const _ = {
  bodySerializer: (t) => JSON.stringify(
    t,
    (e, r) => typeof r == "bigint" ? r.toString() : r
  )
}, Q = ({
  onRequest: t,
  onSseError: e,
  onSseEvent: r,
  responseTransformer: o,
  responseValidator: n,
  sseDefaultRetryDelay: u,
  sseMaxRetryAttempts: c,
  sseMaxRetryDelay: a,
  sseSleepFn: i,
  url: f,
  ...s
}) => {
  let d;
  const k = i ?? ((l) => new Promise((p) => setTimeout(p, l)));
  return { stream: async function* () {
    let l = u ?? 3e3, p = 0;
    const x = s.signal ?? new AbortController().signal;
    for (; !x.aborted; ) {
      p++;
      const z = s.headers instanceof Headers ? s.headers : new Headers(s.headers);
      d !== void 0 && z.set("Last-Event-ID", d);
      try {
        const j = {
          redirect: "follow",
          ...s,
          body: s.serializedBody,
          headers: z,
          signal: x
        };
        let m = new Request(f, j);
        t && (m = await t(f, j));
        const b = await (s.fetch ?? globalThis.fetch)(m);
        if (!b.ok)
          throw new Error(
            `SSE failed: ${b.status} ${b.statusText}`
          );
        if (!b.body) throw new Error("No body in SSE response");
        const g = b.body.pipeThrough(new TextDecoderStream()).getReader();
        let A = "";
        const I = () => {
          try {
            g.cancel();
          } catch {
          }
        };
        x.addEventListener("abort", I);
        try {
          for (; ; ) {
            const { done: L, value: M } = await g.read();
            if (L) break;
            A += M;
            const O = A.split(`

`);
            A = O.pop() ?? "";
            for (const G of O) {
              const J = G.split(`
`), T = [];
              let $;
              for (const y of J)
                if (y.startsWith("data:"))
                  T.push(y.replace(/^data:\s*/, ""));
                else if (y.startsWith("event:"))
                  $ = y.replace(/^event:\s*/, "");
                else if (y.startsWith("id:"))
                  d = y.replace(/^id:\s*/, "");
                else if (y.startsWith("retry:")) {
                  const P = Number.parseInt(
                    y.replace(/^retry:\s*/, ""),
                    10
                  );
                  Number.isNaN(P) || (l = P);
                }
              let E, B = !1;
              if (T.length) {
                const y = T.join(`
`);
                try {
                  E = JSON.parse(y), B = !0;
                } catch {
                  E = y;
                }
              }
              B && (n && await n(E), o && (E = await o(E))), r?.({
                data: E,
                event: $,
                id: d,
                retry: l
              }), T.length && (yield E);
            }
          }
        } finally {
          x.removeEventListener("abort", I), g.releaseLock();
        }
        break;
      } catch (j) {
        if (e?.(j), c !== void 0 && p >= c)
          break;
        const m = Math.min(
          l * 2 ** (p - 1),
          a ?? 3e4
        );
        await k(m);
      }
    }
  }() };
}, K = (t) => {
  switch (t) {
    case "label":
      return ".";
    case "matrix":
      return ";";
    case "simple":
      return ",";
    default:
      return "&";
  }
}, X = (t) => {
  switch (t) {
    case "form":
      return ",";
    case "pipeDelimited":
      return "|";
    case "spaceDelimited":
      return "%20";
    default:
      return ",";
  }
}, Y = (t) => {
  switch (t) {
    case "label":
      return ".";
    case "matrix":
      return ";";
    case "simple":
      return ",";
    default:
      return "&";
  }
}, D = ({
  allowReserved: t,
  explode: e,
  name: r,
  style: o,
  value: n
}) => {
  if (!e) {
    const a = (t ? n : n.map((i) => encodeURIComponent(i))).join(X(o));
    switch (o) {
      case "label":
        return `.${a}`;
      case "matrix":
        return `;${r}=${a}`;
      case "simple":
        return a;
      default:
        return `${r}=${a}`;
    }
  }
  const u = K(o), c = n.map((a) => o === "label" || o === "simple" ? t ? a : encodeURIComponent(a) : v({
    allowReserved: t,
    name: r,
    value: a
  })).join(u);
  return o === "label" || o === "matrix" ? u + c : c;
}, v = ({
  allowReserved: t,
  name: e,
  value: r
}) => {
  if (r == null)
    return "";
  if (typeof r == "object")
    throw new Error(
      "Deeply-nested arrays/objects aren’t supported. Provide your own `querySerializer()` to handle these."
    );
  return `${e}=${t ? r : encodeURIComponent(r)}`;
}, W = ({
  allowReserved: t,
  explode: e,
  name: r,
  style: o,
  value: n,
  valueOnly: u
}) => {
  if (n instanceof Date)
    return u ? n.toISOString() : `${r}=${n.toISOString()}`;
  if (o !== "deepObject" && !e) {
    let i = [];
    Object.entries(n).forEach(([s, d]) => {
      i = [
        ...i,
        s,
        t ? d : encodeURIComponent(d)
      ];
    });
    const f = i.join(",");
    switch (o) {
      case "form":
        return `${r}=${f}`;
      case "label":
        return `.${f}`;
      case "matrix":
        return `;${r}=${f}`;
      default:
        return f;
    }
  }
  const c = Y(o), a = Object.entries(n).map(
    ([i, f]) => v({
      allowReserved: t,
      name: o === "deepObject" ? `${r}[${i}]` : i,
      value: f
    })
  ).join(c);
  return o === "label" || o === "matrix" ? c + a : a;
}, Z = /\{[^{}]+\}/g, ee = ({ path: t, url: e }) => {
  let r = e;
  const o = e.match(Z);
  if (o)
    for (const n of o) {
      let u = !1, c = n.substring(1, n.length - 1), a = "simple";
      c.endsWith("*") && (u = !0, c = c.substring(0, c.length - 1)), c.startsWith(".") ? (c = c.substring(1), a = "label") : c.startsWith(";") && (c = c.substring(1), a = "matrix");
      const i = t[c];
      if (i == null)
        continue;
      if (Array.isArray(i)) {
        r = r.replace(
          n,
          D({ explode: u, name: c, style: a, value: i })
        );
        continue;
      }
      if (typeof i == "object") {
        r = r.replace(
          n,
          W({
            explode: u,
            name: c,
            style: a,
            value: i,
            valueOnly: !0
          })
        );
        continue;
      }
      if (a === "matrix") {
        r = r.replace(
          n,
          `;${v({
            name: c,
            value: i
          })}`
        );
        continue;
      }
      const f = encodeURIComponent(
        a === "label" ? `.${i}` : i
      );
      r = r.replace(n, f);
    }
  return r;
}, te = ({
  baseUrl: t,
  path: e,
  query: r,
  querySerializer: o,
  url: n
}) => {
  const u = n.startsWith("/") ? n : `/${n}`;
  let c = (t ?? "") + u;
  e && (c = ee({ path: e, url: c }));
  let a = r ? o(r) : "";
  return a.startsWith("?") && (a = a.substring(1)), a && (c += `?${a}`), c;
};
function re(t) {
  const e = t.body !== void 0;
  if (e && t.bodySerializer)
    return "serializedBody" in t ? t.serializedBody !== void 0 && t.serializedBody !== "" ? t.serializedBody : null : t.body !== "" ? t.body : null;
  if (e)
    return t.body;
}
const se = async (t, e) => {
  const r = typeof e == "function" ? await e(t) : e;
  if (r)
    return t.scheme === "bearer" ? `Bearer ${r}` : t.scheme === "basic" ? `Basic ${btoa(r)}` : r;
}, H = ({
  allowReserved: t,
  array: e,
  object: r
} = {}) => (n) => {
  const u = [];
  if (n && typeof n == "object")
    for (const c in n) {
      const a = n[c];
      if (a != null)
        if (Array.isArray(a)) {
          const i = D({
            allowReserved: t,
            explode: !0,
            name: c,
            style: "form",
            value: a,
            ...e
          });
          i && u.push(i);
        } else if (typeof a == "object") {
          const i = W({
            allowReserved: t,
            explode: !0,
            name: c,
            style: "deepObject",
            value: a,
            ...r
          });
          i && u.push(i);
        } else {
          const i = v({
            allowReserved: t,
            name: c,
            value: a
          });
          i && u.push(i);
        }
    }
  return u.join("&");
}, ae = (t) => {
  if (!t)
    return "stream";
  const e = t.split(";")[0]?.trim();
  if (e) {
    if (e.startsWith("application/json") || e.endsWith("+json"))
      return "json";
    if (e === "multipart/form-data")
      return "formData";
    if (["application/", "audio/", "image/", "video/"].some(
      (r) => e.startsWith(r)
    ))
      return "blob";
    if (e.startsWith("text/"))
      return "text";
  }
}, ne = (t, e) => e ? !!(t.headers.has(e) || t.query?.[e] || t.headers.get("Cookie")?.includes(`${e}=`)) : !1, ie = async ({
  security: t,
  ...e
}) => {
  for (const r of t) {
    if (ne(e, r.name))
      continue;
    const o = await se(r, e.auth);
    if (!o)
      continue;
    const n = r.name ?? "Authorization";
    switch (r.in) {
      case "query":
        e.query || (e.query = {}), e.query[n] = o;
        break;
      case "cookie":
        e.headers.append("Cookie", `${n}=${o}`);
        break;
      default:
        e.headers.set(n, o);
        break;
    }
  }
}, N = (t) => te({
  baseUrl: t.baseUrl,
  path: t.path,
  query: t.query,
  querySerializer: typeof t.querySerializer == "function" ? t.querySerializer : H(t.querySerializer),
  url: t.url
}), U = (t, e) => {
  const r = { ...t, ...e };
  return r.baseUrl?.endsWith("/") && (r.baseUrl = r.baseUrl.substring(0, r.baseUrl.length - 1)), r.headers = R(t.headers, e.headers), r;
}, oe = (t) => {
  const e = [];
  return t.forEach((r, o) => {
    e.push([o, r]);
  }), e;
}, R = (...t) => {
  const e = new Headers();
  for (const r of t) {
    if (!r)
      continue;
    const o = r instanceof Headers ? oe(r) : Object.entries(r);
    for (const [n, u] of o)
      if (u === null)
        e.delete(n);
      else if (Array.isArray(u))
        for (const c of u)
          e.append(n, c);
      else u !== void 0 && e.set(
        n,
        typeof u == "object" ? JSON.stringify(u) : u
      );
  }
  return e;
};
class q {
  constructor() {
    this.fns = [];
  }
  clear() {
    this.fns = [];
  }
  eject(e) {
    const r = this.getInterceptorIndex(e);
    this.fns[r] && (this.fns[r] = null);
  }
  exists(e) {
    const r = this.getInterceptorIndex(e);
    return !!this.fns[r];
  }
  getInterceptorIndex(e) {
    return typeof e == "number" ? this.fns[e] ? e : -1 : this.fns.indexOf(e);
  }
  update(e, r) {
    const o = this.getInterceptorIndex(e);
    return this.fns[o] ? (this.fns[o] = r, e) : !1;
  }
  use(e) {
    return this.fns.push(e), this.fns.length - 1;
  }
}
const ce = () => ({
  error: new q(),
  request: new q(),
  response: new q()
}), ue = H({
  allowReserved: !1,
  array: {
    explode: !0,
    style: "form"
  },
  object: {
    explode: !0,
    style: "deepObject"
  }
}), le = {
  "Content-Type": "application/json"
}, V = (t = {}) => ({
  ..._,
  headers: le,
  parseAs: "auto",
  querySerializer: ue,
  ...t
}), fe = (t = {}) => {
  let e = U(V(), t);
  const r = () => ({ ...e }), o = (f) => (e = U(e, f), r()), n = ce(), u = async (f) => {
    const s = {
      ...e,
      ...f,
      fetch: f.fetch ?? e.fetch ?? globalThis.fetch,
      headers: R(e.headers, f.headers),
      serializedBody: void 0
    };
    s.security && await ie({
      ...s,
      security: s.security
    }), s.requestValidator && await s.requestValidator(s), s.body !== void 0 && s.bodySerializer && (s.serializedBody = s.bodySerializer(s.body)), (s.body === void 0 || s.serializedBody === "") && s.headers.delete("Content-Type");
    const d = N(s);
    return { opts: s, url: d };
  }, c = async (f) => {
    const { opts: s, url: d } = await u(f), k = {
      redirect: "follow",
      ...s,
      body: re(s)
    };
    let S = new Request(d, k);
    for (const h of n.request.fns)
      h && (S = await h(S, s));
    const C = s.fetch;
    let l = await C(S);
    for (const h of n.response.fns)
      h && (l = await h(l, S, s));
    const p = {
      request: S,
      response: l
    };
    if (l.ok) {
      const h = (s.parseAs === "auto" ? ae(l.headers.get("Content-Type")) : s.parseAs) ?? "json";
      if (l.status === 204 || l.headers.get("Content-Length") === "0") {
        let g;
        switch (h) {
          case "arrayBuffer":
          case "blob":
          case "text":
            g = await l[h]();
            break;
          case "formData":
            g = new FormData();
            break;
          case "stream":
            g = l.body;
            break;
          default:
            g = {};
            break;
        }
        return s.responseStyle === "data" ? g : {
          data: g,
          ...p
        };
      }
      let b;
      switch (h) {
        case "arrayBuffer":
        case "blob":
        case "formData":
        case "json":
        case "text":
          b = await l[h]();
          break;
        case "stream":
          return s.responseStyle === "data" ? l.body : {
            data: l.body,
            ...p
          };
      }
      return h === "json" && (s.responseValidator && await s.responseValidator(b), s.responseTransformer && (b = await s.responseTransformer(b))), s.responseStyle === "data" ? b : {
        data: b,
        ...p
      };
    }
    const x = await l.text();
    let z;
    try {
      z = JSON.parse(x);
    } catch {
    }
    const j = z ?? x;
    let m = j;
    for (const h of n.error.fns)
      h && (m = await h(j, l, S, s));
    if (m = m || {}, s.throwOnError)
      throw m;
    return s.responseStyle === "data" ? void 0 : {
      error: m,
      ...p
    };
  }, a = (f) => (s) => c({ ...s, method: f }), i = (f) => async (s) => {
    const { opts: d, url: k } = await u(s);
    return Q({
      ...d,
      body: d.body,
      headers: d.headers,
      method: f,
      onRequest: async (S, C) => {
        let l = new Request(S, C);
        for (const p of n.request.fns)
          p && (l = await p(l, d));
        return l;
      },
      url: k
    });
  };
  return {
    buildUrl: N,
    connect: a("CONNECT"),
    delete: a("DELETE"),
    get: a("GET"),
    getConfig: r,
    head: a("HEAD"),
    interceptors: n,
    options: a("OPTIONS"),
    patch: a("PATCH"),
    post: a("POST"),
    put: a("PUT"),
    request: c,
    setConfig: o,
    sse: {
      connect: i("CONNECT"),
      delete: i("DELETE"),
      get: i("GET"),
      head: i("HEAD"),
      options: i("OPTIONS"),
      patch: i("PATCH"),
      post: i("POST"),
      put: i("PUT"),
      trace: i("TRACE")
    },
    trace: a("TRACE")
  };
}, de = (t) => ({
  ...t,
  ...F.getConfig()
}), w = fe(de(V({
  baseUrl: "https://localhost:5000,https://localhost:44353"
})));
class pe {
  static GetAllPhotoPackages(e) {
    return (e?.client ?? w).get({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/umbracoextension/api/v1/photopackages",
      ...e
    });
  }
  static CreatePhotoPackage(e) {
    return (e?.client ?? w).post({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/umbracoextension/api/v1/photopackages",
      ...e
    });
  }
  static CreateEventType(e) {
    return (e?.client ?? w).post({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/umbracoextension/api/v1/eventtypes",
      ...e
    });
  }
  static GetEventTypes(e) {
    return (e?.client ?? w).get({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/umbracoextension/api/v1/eventtypes",
      ...e
    });
  }
  static CreateBooking(e) {
    return (e?.client ?? w).post({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/umbracoextension/api/v1/bookings",
      ...e
    });
  }
  static ChangeBookingStatus(e) {
    return (e?.client ?? w).put({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/umbracoextension/api/v1/bookings/{bookingId}/{action}",
      ...e
    });
  }
  static GetBookings(e) {
    return (e?.client ?? w).get({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/umbracoextension/api/v1/bookings",
      ...e
    });
  }
  static whatsMyName(e) {
    return (e?.client ?? w).get({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/umbracoextension/api/v1/whatsMyName",
      ...e
    });
  }
  static whatsTheTimeMrWolf(e) {
    return (e?.client ?? w).get({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/umbracoextension/api/v1/whatsTheTimeMrWolf",
      ...e
    });
  }
  static whoAmI(e) {
    return (e?.client ?? w).get({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/umbracoextension/api/v1/whoAmI",
      ...e
    });
  }
}
export {
  pe as U
};
//# sourceMappingURL=sdk.gen-DJTPZEen.js.map
