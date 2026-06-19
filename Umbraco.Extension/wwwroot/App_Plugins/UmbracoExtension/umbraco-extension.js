const n = [
  {
    name: "Umbraco Extension Entrypoint",
    alias: "Umbraco.Extension.Entrypoint",
    type: "backofficeEntryPoint",
    js: () => import("./entrypoint-BSlTz4-p.js")
  }
], a = [
  {
    name: "Umbraco Extension Dashboard",
    alias: "Umbraco.Extension.Dashboard",
    type: "dashboard",
    js: () => import("./booking.dashboard-DRQ753Ke.js"),
    meta: {
      label: "Bookings",
      pathname: "umbraco-extension-dashboard"
    },
    conditions: [
      {
        alias: "Umb.Condition.SectionAlias",
        match: "Umb.Section.Content"
      }
    ]
  },
  {
    name: "Umbraco Extension Events Dashboard",
    alias: "Umbraco.Extension.EventsDashboard",
    type: "dashboard",
    js: () => import("./events.dashboard-Dh0TLhhj.js"),
    meta: {
      label: "Events",
      pathname: "umbraco-extension-events-dashboard"
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
//# sourceMappingURL=umbraco-extension.js.map
