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
import { ContentTypeDto, ContentTypeDetailDto } from "../../api/types.gen.js";

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
    private selectedContentType: ContentTypeDetailDto | null = null;

    @state()
    private isLoading = false;

    @state()
    private isLoadingDetails = false;

    @state()
    private detailsError: string | null = null;
  
    constructor() {
        super();
        this.attachShadow({ mode: 'open' });
    }
    
    connectedCallback() {
        super.connectedCallback();
        // Load content types when component is connected to the DOM
        this.#loadContentTypes();
    }
    
    #loadContentTypes = async () => {
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
                return;
            }
            
            if (data) {
                this.contentTypes = data;
            }
        } catch (error) {
            console.error("Error fetching content types:", error);
        } finally {
            this.isLoading = false;
        }
    };
    
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
        
        await this.#loadContentTypes();
        
        buttonElement.state = "success";
    };

    #onViewContentTypeDetails = async (alias: string) => {
        this.isLoadingDetails = true;
        this.detailsError = null;
        
        try {
            const { data, error } = await CatalogPluginService.getContentTypeByAlias(alias, {
                credentials: 'include',
                mode: 'cors',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                    'Authorization': getBearerToken()
                },
                path: { alias }
            });
            
            if (error) {
                console.error(`Error fetching content type details for ${alias}:`, error);
                this.detailsError = `Failed to load details for ${alias}`;
                return;
            }
            
            if (data) {
                this.selectedContentType = data;
            } else {
                this.detailsError = `No details found for ${alias}`;
            }
        } catch (error) {
            console.error(`Error fetching content type details for ${alias}:`, error);
            this.detailsError = `Error loading details for ${alias}`;
        } finally {
            this.isLoadingDetails = false;
        }
    };

    #onCloseDetails = () => {
        this.selectedContentType = null;
        this.detailsError = null;
    };

    #renderContentTypeDetails() {
        if (this.isLoadingDetails) {
            return html`<div class="loading-details">Loading content type details...</div>`;
        }
        
        if (this.detailsError) {
            return html`
                <div class="details-error">
                    <p>${this.detailsError}</p>
                    <uui-button look="secondary" @click="${this.#onCloseDetails}">Close</uui-button>
                </div>
            `;
        }
        
        if (!this.selectedContentType) return null;
        
        const ct = this.selectedContentType;
        
        return html`
            <div class="content-type-details">
                <div class="details-header">
                    <div class="details-title">
                        <uui-icon name="${ct.icon || 'icon-document'}"></uui-icon>
                        <h2>${ct.name}</h2>
                    </div>
                    <uui-button look="secondary" @click="${this.#onCloseDetails}">Close</uui-button>
                </div>
                
                <div class="details-meta">
                    <div class="meta-item">
                        <strong>Alias:</strong> ${ct.alias}
                    </div>
                    <div class="meta-item">
                        <strong>Description:</strong> ${ct.description || 'No description'}
                    </div>
                    <div class="meta-item">
                        <strong>Created:</strong> ${new Date(ct.createDate).toLocaleString()}
                    </div>
                    <div class="meta-item">
                        <strong>Updated:</strong> ${new Date(ct.updateDate).toLocaleString()}
                    </div>
                    <div class="meta-item">
                        <strong>Element Type:</strong> ${ct.isElement ? 'Yes' : 'No'}
                    </div>
                    <div class="meta-item">
                        <strong>Allowed at Root:</strong> ${ct.allowedAsRoot ? 'Yes' : 'No'}
                    </div>
                    <div class="meta-item">
                        <strong>Default Template:</strong> ${ct.defaultTemplate || 'None'}
                    </div>
                </div>
                
                ${ct.compositions && ct.compositions.length > 0 ? html`
                    <div class="details-section">
                        <h3>Compositions</h3>
                        <div class="compositions-list">
                            ${ct.compositions.map(comp => html`
                                <div class="composition-item">
                                    <uui-icon name="${comp.icon || 'icon-document'}"></uui-icon>
                                    <span>${comp.name}</span>
                                </div>
                            `)}
                        </div>
                    </div>
                ` : ''}
                
                ${ct.allowedContentTypes && ct.allowedContentTypes.length > 0 ? html`
                    <div class="details-section">
                        <h3>Allowed Child Content Types</h3>
                        <ul class="allowed-types-list">
                            ${ct.allowedContentTypes.map(act => html`
                                <li>${act.alias || `Content Type #${act.id}`}</li>
                            `)}
                        </ul>
                    </div>
                ` : ''}
                
                <div class="details-section">
                    <h3>Properties</h3>
                    ${ct.propertyGroups.map(group => html`
                        <div class="property-group">
                            <h4>${group.name}</h4>
                            <div class="properties-table">
                                <div class="property-row header">
                                    <div class="property-cell">Name</div>
                                    <div class="property-cell">Alias</div>
                                    <div class="property-cell">Data Type</div>
                                    <div class="property-cell">Required</div>
                                </div>
                                ${group.propertyTypes.map(prop => html`
                                    <div class="property-row">
                                        <div class="property-cell">${prop.name}</div>
                                        <div class="property-cell">${prop.alias}</div>
                                        <div class="property-cell">${prop.dataType?.name || 'Unknown'}</div>
                                        <div class="property-cell">${prop.mandatory ? 'Yes' : 'No'}</div>
                                    </div>
                                `)}
                            </div>
                        </div>
                    `)}
                </div>
            </div>
        `;
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
                        Let's play ping pong!
                    </p>
                </uui-box>

                <uui-box headline="Content Types" class="wide">
                    <div slot="header">[Custom API]</div>
                    
                    ${this.selectedContentType ? 
                        this.#renderContentTypeDetails() : 
                        html`
                            <uui-button
                              color="default"
                              look="primary"
                              @click="${this.#onClickGetContentTypes}"
                            >
                                Refresh
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
                                                <div class="table-cell action-col">Actions</div>
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
                                                    <div class="table-cell action-col">
                                                        <uui-button 
                                                            look="outline" 
                                                            label="View Details" 
                                                            compact
                                                            @click="${() => this.#onViewContentTypeDetails(contentType.alias)}"
                                                        >
                                                            Details
                                                        </uui-button>
                                                    </div>
                                                </div>
                                            `)}
                                        </div>
                                    `
                                    : html`<p>No content types found. Click the button to load them.</p>`
                            }
                        `
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
                width: 18%;
            }

            .alias-col {
                width: 18%;
            }

            .icon-col {
                width: 8%;
                text-align: center;
                display: flex;
                align-items: center;
                justify-content: center;
            }

            .desc-col {
                width: 32%;
            }

            .props-col {
                width: 12%;
                text-align: center;
            }

            .action-col {
                width: 12%;
                display: flex;
                align-items: center;
                justify-content: center;
            }

            .content-type-details {
                margin-top: var(--uui-size-space-4);
            }

            .details-header {
                display: flex;
                justify-content: space-between;
                align-items: center;
                margin-bottom: var(--uui-size-space-4);
            }

            .details-title {
                display: flex;
                align-items: center;
                gap: var(--uui-size-space-3);
            }

            .details-title h2 {
                margin: 0;
            }

            .details-meta {
                display: grid;
                grid-template-columns: 1fr 1fr;
                gap: var(--uui-size-space-3);
                margin-bottom: var(--uui-size-space-4);
                padding: var(--uui-size-space-4);
                background-color: var(--uui-color-surface);
                border-radius: 3px;
            }

            .meta-item {
                margin-bottom: var(--uui-size-space-2);
            }

            .details-section {
                margin-bottom: var(--uui-size-space-5);
            }

            .compositions-list {
                display: flex;
                flex-wrap: wrap;
                gap: var(--uui-size-space-3);
            }

            .composition-item {
                display: flex;
                align-items: center;
                gap: var(--uui-size-space-2);
                padding: var(--uui-size-space-2) var(--uui-size-space-3);
                background-color: var(--uui-color-surface);
                border-radius: 3px;
            }

            .allowed-types-list {
                margin: 0;
                padding-left: var(--uui-size-space-4);
            }

            .allowed-types-list li {
                margin-bottom: var(--uui-size-space-2);
            }

            .property-group {
                margin-bottom: var(--uui-size-space-4);
            }

            .properties-table {
                width: 100%;
                border: 1px solid var(--uui-color-border);
                border-radius: 3px;
                overflow: hidden;
            }

            .property-row {
                display: flex;
                border-bottom: 1px solid var(--uui-color-border-subtle);
            }

            .property-row.header {
                background-color: var(--uui-color-surface);
                font-weight: bold;
            }

            .property-row:last-child {
                border-bottom: none;
            }

            .property-cell {
                padding: 8px 12px;
                overflow: hidden;
                text-overflow: ellipsis;
            }

            .property-cell:nth-child(1) {
                width: 30%;
            }

            .property-cell:nth-child(2) {
                width: 30%;
            }

            .property-cell:nth-child(3) {
                width: 30%;
            }

            .property-cell:nth-child(4) {
                width: 10%;
                text-align: center;
            }

            .loading-details, .details-error {
                padding: var(--uui-size-space-4);
                text-align: center;
            }

            .details-error {
                color: var(--uui-color-danger);
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