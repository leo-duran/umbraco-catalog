export const manifests: Array<UmbExtensionManifest> = [
  {
    name: "Catalog Dashboard",
    alias: "Catalog.Dashboard",
    type: "dashboard",
    js: () => import("./catalog-dashboard.element.js"),
    meta: {
      label: "Catalog Dashboard",
      pathname: "catalog-dashboard",
    },
    conditions: [
      {
        alias: "Umb.Condition.SectionAlias",
        match: "Umb.Section.Content",
      },
    ],
  },
];