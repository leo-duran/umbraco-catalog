export const manifests: Array<UmbExtensionManifest> = [
  {
    name: "Catalog Plugin Entrypoint",
    alias: "Catalog.Plugin.Entrypoint",
    type: "backofficeEntryPoint",
    js: () => import("./entrypoint.js"),
  },
];
