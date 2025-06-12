export const manifests: Array<UmbExtensionManifest> = [
  {
    name: "Suggestions Property Editor UI",
    alias: "Catalog.SuggestionsPropertyEditorUi",
    type: "propertyEditorUi",
    js: () => import("./suggestions-property-editor-ui.element.js"),
    meta: {
      label: "Suggestions",
      icon: "icon-list",
      group: "common",
      propertyEditorSchemaAlias: "Umbraco.Plain.String",
    }
  },
];