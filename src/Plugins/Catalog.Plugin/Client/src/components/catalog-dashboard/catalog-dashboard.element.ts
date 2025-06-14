import {
    LitElement,
    css,
    html,
    customElement,
    state,
  } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from '@umbraco-cms/backoffice/element-api';
import { UUIButtonElement } from "@umbraco-cms/backoffice/external/uui";
import { CatalogPluginService } from "../../api/sdk.gen.js";

function getBearerToken() {
  const token = localStorage.getItem("umb:userAuthTokenResponse");
  return token ? `Bearer ${JSON.parse(token)?.access_token}` : null;
}

@customElement("catalog-dashboard")
export class CatalogDashboardElement extends UmbElementMixin(LitElement) {
    @state()
    private message?: string = "Ping!";

  
    constructor() {
        super();
        this.attachShadow({ mode: 'open' });
    }
    
    #onClickWhatsMyName = async (ev: Event) => {
        const buttonElement = ev.target as UUIButtonElement;
        buttonElement.state = "waiting";
    
        const { data, error } = await CatalogPluginService.ping({
          credentials: 'include',
          mode: 'cors',
          headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': getBearerToken()
          }
        });
    
        if (error) {
          buttonElement.state = "failed";
          console.error(error);
          return;
        }
    
        this.message = data;
        buttonElement.state = "success";
      };

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
                
                <uui-box headline="Catalog Plugin Dashboard Woot woot">
                    <p>Welcome to your custom dashboard!</p>
                    <p>Last update: ${new Date().toLocaleString()}</p>
                </uui-box>

                <uui-box headline="Ping!">
                    <div slot="header">[Server]</div>
                    <h2><uui-icon name="icon-user"></uui-icon> ${this.message}</h2>
                    <uui-button
                        color="default"
                        look="primary"
                        @click="${this.#onClickWhatsMyName}"
                    >
                        Ping!
                    </uui-button>
                    <p>
                        This endpoint has a forced delay to show the button 'waiting' state for a few seconds before completing the request.
                    </p>
                </uui-box>

            `
    }

    static styles = [
        css`
            :host {
            display: grid;
            gap: var(--uui-size-layout-1);
            padding: var(--uui-size-layout-1);
            grid-template-columns: 1fr 1fr 1fr;
            }

            uui-box {
            margin-bottom: var(--uui-size-layout-1);
            }

            h2 {
            margin-top: 0;
            }

            .wide {
            grid-column: span 3;
            }
        `,
        ];
    
}

// This is required for module federation to properly load the element
export default CatalogDashboardElement;

declare global {
    interface HTMLElementTagNameMap {
      "catalog-dashboard": CatalogDashboardElement;
    }
  }