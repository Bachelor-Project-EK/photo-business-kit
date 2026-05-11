export const manifests: Array<UmbExtensionManifest> = [
  {
    name: "Umbraco Extension Entrypoint",
    alias: "Umbraco.Extension.Entrypoint",
    type: "backofficeEntryPoint",
    js: () => import("./entrypoint.js"),
  },
];
