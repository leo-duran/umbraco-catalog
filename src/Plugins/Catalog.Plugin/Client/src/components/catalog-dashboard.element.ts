import {
    LitElement,
    html,
    customElement,
  } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from '@umbraco-cms/backoffice/element-api';

@customElement("catalog-dashboard")
export class CatalogDashboardElement extends UmbElementMixin(LitElement) {
    constructor() {
        super();
        this.attachShadow({ mode: 'open' });
    }

    render() {
        return html`
                <style>
                    :host {
                        display: block;
                        padding: 20px;
                    }
                    h2 {
                        margin-top: 0;
                    }
                </style>
                <section>
                    <h2>Catalog Plugin Dashboard</h2>
                    <p>Welcome to your custom dashboard!</p>
                    <p>Last update: ${new Date().toLocaleString()}</p>
                </section>
            `
    }
}

// This is required for module federation to properly load the element
export default CatalogDashboardElement;

declare global {
    interface HTMLElementTagNameMap {
      "catalog-dashboard": CatalogDashboardElement;
    }
  }