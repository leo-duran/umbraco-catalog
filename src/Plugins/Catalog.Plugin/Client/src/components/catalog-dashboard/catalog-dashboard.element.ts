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
import { ContentTypeDto } from "../../api/types.gen.js";

function getBearerToken() {
  const token = localStorage.getItem("umb:userAuthTokenResponse");
  return token ? `Bearer ${JSON.parse(token)?.access_token}` : null;
}

@customElement("catalog-dashboard")
export class CatalogDashboardElement extends UmbElementMixin(LitElement) {
    @state()
    private message?: string = "Ping!";

    @state()
    private contentTypes: ContentTypeDto[] = [];

    @state()
    private isLoading = false;
  
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

    #onClickGetContentTypes = async (ev: Event) => {
        const buttonElement = ev.target as UUIButtonElement;
        buttonElement.state = "waiting";
        this.isLoading = true;
        
        try {
            const { data, error } = await CatalogPluginService.getContentTypes({
                credentials: 'include',
                mode: 'cors',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                    'Authorization': getBearerToken()
                }
            });
            
            if (error) {
                console.error("Error fetching content types:", error);
                buttonElement.state = "failed";
                return;
            }
            
            if (data) {
                this.contentTypes = data;
                buttonElement.state = "success";
            } else {
                buttonElement.state = "failed";
            }
        } catch (error) {
            console.error("Error fetching content types:", error);
            buttonElement.state = "failed";
        } finally {
            this.isLoading = false;
        }
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

                <uui-box headline="Content Types" class="wide">
                    <div slot="header">[Custom API]</div>
                    <uui-button
                      color="default"
                      look="primary"
                      @click="${this.#onClickGetContentTypes}"
                    >
                        Get Content Types
                    </uui-button>
                    
                    ${this.isLoading 
                        ? html`<p>Loading content types...</p>` 
                        : this.contentTypes.length > 0 
                            ? html`
                                <p>Found ${this.contentTypes.length} content types:</p>
                                <div class="custom-table">
                                    <div class="table-header">
                                        <div class="table-cell name-col">Name</div>
                                        <div class="table-cell alias-col">Alias</div>
                                        <div class="table-cell icon-col">Icon</div>
                                        <div class="table-cell desc-col">Description</div>
                                        <div class="table-cell props-col">Properties</div>
                                    </div>
                                    ${this.contentTypes.map(contentType => html`
                                        <div class="table-row">
                                            <div class="table-cell name-col">${contentType.name}</div>
                                            <div class="table-cell alias-col">${contentType.alias}</div>
                                            <div class="table-cell icon-col">
                                                <uui-icon name="${contentType.icon || 'icon-document'}"></uui-icon>
                                            </div>
                                            <div class="table-cell desc-col">${contentType.description || '-'}</div>
                                            <div class="table-cell props-col">
                                                ${contentType.propertyGroups?.reduce((count, group) => 
                                                    count + (group.propertyTypes?.length || 0), 0) || 0} properties
                                            </div>
                                        </div>
                                    `)}
                                </div>
                            `
                            : html`<p>No content types found. Click the button to load them.</p>`
                    }
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

            .custom-table {
                width: 100%;
                margin-top: var(--uui-size-space-4);
                border: 1px solid var(--uui-color-border);
                border-radius: 3px;
                overflow: hidden;
            }

            .table-header {
                display: flex;
                background-color: var(--uui-color-surface);
                border-bottom: 2px solid var(--uui-color-border);
                font-weight: bold;
            }

            .table-row {
                display: flex;
                border-bottom: 1px solid var(--uui-color-border-subtle);
            }

            .table-row:last-child {
                border-bottom: none;
            }

            .table-cell {
                padding: 12px;
                overflow: hidden;
                text-overflow: ellipsis;
            }

            .name-col {
                width: 20%;
            }

            .alias-col {
                width: 20%;
            }

            .icon-col {
                width: 10%;
                text-align: center;
                display: flex;
                align-items: center;
                justify-content: center;
            }

            .desc-col {
                width: 35%;
            }

            .props-col {
                width: 15%;
                text-align: center;
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