import type {
  UmbEntryPointOnInit,
  UmbEntryPointOnUnload,
} from "@umbraco-cms/backoffice/extension-api";
import { CatalogPluginService } from "../api/sdk.gen";

function getBearerToken() {
  const token = localStorage.getItem("umb:userAuthTokenResponse");
  return token ? `Bearer ${JSON.parse(token)?.access_token}` : null;
}

// load up the manifests here
export const onInit: UmbEntryPointOnInit = async (_host, _extensionRegistry) => {
  console.log("Hello from my extension... on init 🎉");
  
  try {
    const response = await CatalogPluginService.ping({
      credentials: 'include',
      mode: 'cors',
      headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json',
        'Authorization': getBearerToken()
      }
    });
    console.log("Ping response:", response);
  } catch (error) {
    console.error("Error pinging service:", error);
    if (error instanceof Error) {
      console.error("Error details:", error.message);
    }
  }
};

export const onUnload: UmbEntryPointOnUnload = (_host, _extensionRegistry) => {
  console.log("Goodbye from my extension 👋");
};
