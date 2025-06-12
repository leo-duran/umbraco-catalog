import { LitElement, html, css, customElement, property } from '@umbraco-cms/backoffice/external/lit';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';
import { UmbTextStyles } from '@umbraco-cms/backoffice/style';

@customElement('my-suggestions-property-editor-ui')
export default class MySuggestionsPropertyEditorUIElement extends LitElement implements UmbPropertyEditorUiElement {
    @property({ type: String })
    public value = '';

    override render() {
        return html`
          <uui-input
            id="suggestion-input"
            class="element"
            label="text input"
            .value=${this.value || ""}
          >
          </uui-input>
          <div id="wrapper">
            <uui-button
              id="suggestion-button"
              class="element"
              look="primary"
              label="give me suggestions"
            >
              Give me suggestions!
            </uui-button>
            <uui-button
              id="suggestion-trimmer"
              class="element"
              look="outline"
              label="Trim text"
            >
              Trim text
            </uui-button>
          </div>
        `;
    }

    static override readonly styles = [
        UmbTextStyles,
        css`
          #wrapper {
            margin-top: 10px;
            display: flex;
            gap: 10px;
          }
          .element {
            width: 100%;
          }
        `,
      ];

}


declare global {
    interface HTMLElementTagNameMap {
        'my-suggestions-property-editor-ui': MySuggestionsPropertyEditorUIElement;
    }
}