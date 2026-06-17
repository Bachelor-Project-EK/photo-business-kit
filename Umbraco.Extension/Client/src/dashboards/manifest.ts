export const manifests: Array<UmbExtensionManifest> = [
  {
    name: "Umbraco Extension Dashboard",
    alias: "Umbraco.Extension.Dashboard",
    type: "dashboard",
    js: () => import("./booking.dashboard.js"),
    meta: {
      label: "Bookings",
      pathname: "umbraco-extension-dashboard",
    },
    conditions: [
      {
        alias: "Umb.Condition.SectionAlias",
        match: "Umb.Section.Content",
      },
    ],
  },
];
