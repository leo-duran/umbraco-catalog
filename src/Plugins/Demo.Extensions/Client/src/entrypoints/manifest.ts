export const manifests: Array<UmbExtensionManifest> = [
  {
    name: "Demo Extensions Entrypoint",
    alias: "Demo.Extensions.Entrypoint",
    type: "backofficeEntryPoint",
    js: () => import("./entrypoint.js"),
  },
];
