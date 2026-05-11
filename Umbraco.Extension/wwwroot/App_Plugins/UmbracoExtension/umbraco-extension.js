const a = [
  {
    name: "Umbraco Extension Entrypoint",
    alias: "Umbraco.Extension.Entrypoint",
    type: "backofficeEntryPoint",
    js: () => import("./entrypoint-BSlTz4-p.js")
  }
], n = [
  {
    name: "Umbraco Extension Dashboard",
    alias: "Umbraco.Extension.Dashboard",
    type: "dashboard",
    js: () => import("./dashboard.element-D6h9S4mK.js"),
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
  ...a,
  ...n
];
export {
  o as manifests
};
//# sourceMappingURL=umbraco-extension.js.map
