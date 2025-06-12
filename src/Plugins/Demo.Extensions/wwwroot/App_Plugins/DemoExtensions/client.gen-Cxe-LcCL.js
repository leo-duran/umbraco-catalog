var C = Object.defineProperty;
var _ = (e, r, t) => r in e ? C(e, r, { enumerable: !0, configurable: !0, writable: !0, value: t }) : e[r] = t;
var g = (e, r, t) => _(e, typeof r != "symbol" ? r + "" : r, t);
var U = async (e, r) => {
  let t = typeof r == "function" ? await r(e) : r;
  if (t) return e.scheme === "bearer" ? `Bearer ${t}` : e.scheme === "basic" ? `Basic ${btoa(t)}` : t;
}, I = { bodySerializer: (e) => JSON.stringify(e, (r, t) => typeof t == "bigint" ? t.toString() : t) }, T = (e) => {
  switch (e) {
    case "label":
      return ".";
    case "matrix":
      return ";";
    case "simple":
      return ",";
    default:
      return "&";
  }
}, A = (e) => {
  switch (e) {
    case "form":
      return ",";
    case "pipeDelimited":
      return "|";
    case "spaceDelimited":
      return "%20";
    default:
      return ",";
  }
}, z = (e) => {
  switch (e) {
    case "label":
      return ".";
    case "matrix":
      return ";";
    case "simple":
      return ",";
    default:
      return "&";
  }
}, x = ({ allowReserved: e, explode: r, name: t, style: n, value: l }) => {
  if (!r) {
    let s = (e ? l : l.map((i) => encodeURIComponent(i))).join(A(n));
    switch (n) {
      case "label":
        return `.${s}`;
      case "matrix":
        return `;${t}=${s}`;
      case "simple":
        return s;
      default:
        return `${t}=${s}`;
    }
  }
  let o = T(n), a = l.map((s) => n === "label" || n === "simple" ? e ? s : encodeURIComponent(s) : b({ allowReserved: e, name: t, value: s })).join(o);
  return n === "label" || n === "matrix" ? o + a : a;
}, b = ({ allowReserved: e, name: r, value: t }) => {
  if (t == null) return "";
  if (typeof t == "object") throw new Error("Deeply-nested arrays/objects aren’t supported. Provide your own `querySerializer()` to handle these.");
  return `${r}=${e ? t : encodeURIComponent(t)}`;
}, $ = ({ allowReserved: e, explode: r, name: t, style: n, value: l }) => {
  if (l instanceof Date) return `${t}=${l.toISOString()}`;
  if (n !== "deepObject" && !r) {
    let s = [];
    Object.entries(l).forEach(([d, f]) => {
      s = [...s, d, e ? f : encodeURIComponent(f)];
    });
    let i = s.join(",");
    switch (n) {
      case "form":
        return `${t}=${i}`;
      case "label":
        return `.${i}`;
      case "matrix":
        return `;${t}=${i}`;
      default:
        return i;
    }
  }
  let o = z(n), a = Object.entries(l).map(([s, i]) => b({ allowReserved: e, name: n === "deepObject" ? `${t}[${s}]` : s, value: i })).join(o);
  return n === "label" || n === "matrix" ? o + a : a;
}, E = /\{[^{}]+\}/g, W = ({ path: e, url: r }) => {
  let t = r, n = r.match(E);
  if (n) for (let l of n) {
    let o = !1, a = l.substring(1, l.length - 1), s = "simple";
    a.endsWith("*") && (o = !0, a = a.substring(0, a.length - 1)), a.startsWith(".") ? (a = a.substring(1), s = "label") : a.startsWith(";") && (a = a.substring(1), s = "matrix");
    let i = e[a];
    if (i == null) continue;
    if (Array.isArray(i)) {
      t = t.replace(l, x({ explode: o, name: a, style: s, value: i }));
      continue;
    }
    if (typeof i == "object") {
      t = t.replace(l, $({ explode: o, name: a, style: s, value: i }));
      continue;
    }
    if (s === "matrix") {
      t = t.replace(l, `;${b({ name: a, value: i })}`);
      continue;
    }
    let d = encodeURIComponent(s === "label" ? `.${i}` : i);
    t = t.replace(l, d);
  }
  return t;
}, q = ({ allowReserved: e, array: r, object: t } = {}) => (n) => {
  let l = [];
  if (n && typeof n == "object") for (let o in n) {
    let a = n[o];
    if (a != null) if (Array.isArray(a)) {
      let s = x({ allowReserved: e, explode: !0, name: o, style: "form", value: a, ...r });
      s && l.push(s);
    } else if (typeof a == "object") {
      let s = $({ allowReserved: e, explode: !0, name: o, style: "deepObject", value: a, ...t });
      s && l.push(s);
    } else {
      let s = b({ allowReserved: e, name: o, value: a });
      s && l.push(s);
    }
  }
  return l.join("&");
}, D = (e) => {
  var t;
  if (!e) return "stream";
  let r = (t = e.split(";")[0]) == null ? void 0 : t.trim();
  if (r) {
    if (r.startsWith("application/json") || r.endsWith("+json")) return "json";
    if (r === "multipart/form-data") return "formData";
    if (["application/", "audio/", "image/", "video/"].some((n) => r.startsWith(n))) return "blob";
    if (r.startsWith("text/")) return "text";
  }
}, N = async ({ security: e, ...r }) => {
  for (let t of e) {
    let n = await U(t, r.auth);
    if (!n) continue;
    let l = t.name ?? "Authorization";
    switch (t.in) {
      case "query":
        r.query || (r.query = {}), r.query[l] = n;
        break;
      case "cookie":
        r.headers.append("Cookie", `${l}=${n}`);
        break;
      case "header":
      default:
        r.headers.set(l, n);
        break;
    }
    return;
  }
}, j = (e) => k({ baseUrl: e.baseUrl, path: e.path, query: e.query, querySerializer: typeof e.querySerializer == "function" ? e.querySerializer : q(e.querySerializer), url: e.url }), k = ({ baseUrl: e, path: r, query: t, querySerializer: n, url: l }) => {
  let o = l.startsWith("/") ? l : `/${l}`, a = (e ?? "") + o;
  r && (a = W({ path: r, url: a }));
  let s = t ? n(t) : "";
  return s.startsWith("?") && (s = s.substring(1)), s && (a += `?${s}`), a;
}, v = (e, r) => {
  var n;
  let t = { ...e, ...r };
  return (n = t.baseUrl) != null && n.endsWith("/") && (t.baseUrl = t.baseUrl.substring(0, t.baseUrl.length - 1)), t.headers = S(e.headers, r.headers), t;
}, S = (...e) => {
  let r = new Headers();
  for (let t of e) {
    if (!t || typeof t != "object") continue;
    let n = t instanceof Headers ? t.entries() : Object.entries(t);
    for (let [l, o] of n) if (o === null) r.delete(l);
    else if (Array.isArray(o)) for (let a of o) r.append(l, a);
    else o !== void 0 && r.set(l, typeof o == "object" ? JSON.stringify(o) : o);
  }
  return r;
}, w = class {
  constructor() {
    g(this, "_fns");
    this._fns = [];
  }
  clear() {
    this._fns = [];
  }
  getInterceptorIndex(e) {
    return typeof e == "number" ? this._fns[e] ? e : -1 : this._fns.indexOf(e);
  }
  exists(e) {
    let r = this.getInterceptorIndex(e);
    return !!this._fns[r];
  }
  eject(e) {
    let r = this.getInterceptorIndex(e);
    this._fns[r] && (this._fns[r] = null);
  }
  update(e, r) {
    let t = this.getInterceptorIndex(e);
    return this._fns[t] ? (this._fns[t] = r, e) : !1;
  }
  use(e) {
    return this._fns = [...this._fns, e], this._fns.length - 1;
  }
}, P = () => ({ error: new w(), request: new w(), response: new w() }), H = q({ allowReserved: !1, array: { explode: !0, style: "form" }, object: { explode: !0, style: "deepObject" } }), J = { "Content-Type": "application/json" }, O = (e = {}) => ({ ...I, headers: J, parseAs: "auto", querySerializer: H, ...e }), B = (e = {}) => {
  let r = v(O(), e), t = () => ({ ...r }), n = (a) => (r = v(r, a), t()), l = P(), o = async (a) => {
    let s = { ...r, ...a, fetch: a.fetch ?? r.fetch ?? globalThis.fetch, headers: S(r.headers, a.headers) };
    s.security && await N({ ...s, security: s.security }), s.body && s.bodySerializer && (s.body = s.bodySerializer(s.body)), (s.body === void 0 || s.body === "") && s.headers.delete("Content-Type");
    let i = j(s), d = { redirect: "follow", ...s }, f = new Request(i, d);
    for (let u of l.request._fns) u && (f = await u(f, s));
    let R = s.fetch, c = await R(f);
    for (let u of l.response._fns) u && (c = await u(c, f, s));
    let h = { request: f, response: c };
    if (c.ok) {
      if (c.status === 204 || c.headers.get("Content-Length") === "0") return { data: {}, ...h };
      let u = (s.parseAs === "auto" ? D(c.headers.get("Content-Type")) : s.parseAs) ?? "json";
      if (u === "stream") return { data: c.body, ...h };
      let m = await c[u]();
      return u === "json" && (s.responseValidator && await s.responseValidator(m), s.responseTransformer && (m = await s.responseTransformer(m))), { data: m, ...h };
    }
    let y = await c.text();
    try {
      y = JSON.parse(y);
    } catch {
    }
    let p = y;
    for (let u of l.error._fns) u && (p = await u(y, c, f, s));
    if (p = p || {}, s.throwOnError) throw p;
    return { error: p, ...h };
  };
  return { buildUrl: j, connect: (a) => o({ ...a, method: "CONNECT" }), delete: (a) => o({ ...a, method: "DELETE" }), get: (a) => o({ ...a, method: "GET" }), getConfig: t, head: (a) => o({ ...a, method: "HEAD" }), interceptors: l, options: (a) => o({ ...a, method: "OPTIONS" }), patch: (a) => o({ ...a, method: "PATCH" }), post: (a) => o({ ...a, method: "POST" }), put: (a) => o({ ...a, method: "PUT" }), request: o, setConfig: n, trace: (a) => o({ ...a, method: "TRACE" }) };
};
const V = B(O({
  baseUrl: "https://localhost:44389"
}));
export {
  V as c
};
//# sourceMappingURL=client.gen-Cxe-LcCL.js.map
