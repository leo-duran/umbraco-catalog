const n = [
  {
    name: "Demo Extensions Entrypoint",
    alias: "Demo.Extensions.Entrypoint",
    type: "backofficeEntryPoint",
    js: () => import("./entrypoint-B0iyVXaJ.js")
  }
], a = [
  {
    name: "Demo Extensions Dashboard",
    alias: "Demo.Extensions.Dashboard",
    type: "dashboard",
    js: () => import("./dashboard.element-B0RMT2Vj.js"),
    meta: {
      label: "Example Dashboard",
      pathname: "example-dashboard"
    },
    conditions: [
      {
        alias: "Umb.Condition.SectionAlias",
        match: "Umb.Section.Content"
      }
    ]
  }
], o = [
  ...n,
  ...a
];
export {
  o as manifests
};
//# sourceMappingURL=demo-extensions.js.map
