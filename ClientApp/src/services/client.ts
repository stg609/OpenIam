import { ThinRequestAsync } from "@/utils/utils";
import { PathBasePrefix } from "../../config/iamSettings";

export async function queryAsync(access_token: string, clientName: string | null = null, clientId: string | null = null,
  pageIndex: number = 1, pageSize: number = 10): Promise<API.PaginatedDto<API.ClientDto>> {
  const requestHeaders: HeadersInit = new Headers();
  requestHeaders.set('Authorization', `Bearer ${access_token}`);

  const queryString = [
    `clientName=${clientName ?? ""}`,
    `clientId=${clientId ?? ""}`,
    `pageSize=${pageSize ?? ""}`,
    `pageIndex=${pageIndex ?? ""}`,
  ].join('&');

  const resp = await ThinRequestAsync({
    url: `${PathBasePrefix}/admin/api/clients?${queryString}`,
    access_token
  });

  return await (resp.json()) as API.PaginatedDto<API.ClientDto>;
}

export async function enableAsync(access_token: string, clientId: string, isEnabled: boolean): Promise<void> {
  await ThinRequestAsync({
    access_token,
    url: `${PathBasePrefix}/admin/api/clients/${clientId}/enabled`,
    method: 'put',
    body: JSON.stringify({ "isEnabled": isEnabled }),
  });
}

export async function refreshSecretsAsync(access_token: string, clientId: string): Promise<string | null> {
  const resp = await ThinRequestAsync({
    access_token,
    url: `${PathBasePrefix}/admin/api/clients/${clientId}/secrets`,
    method: 'post',
  });

  return await resp.text();

}

export async function createAsync(access_token: string, client: API.ClientDto): Promise<API.ClientCreateRespDto | null> {
  const resp = await ThinRequestAsync({
    access_token,
    url: `${PathBasePrefix}/admin/api/clients/`,
    method: 'post',
    body: JSON.stringify(client),
  });

  return <API.ClientCreateRespDto>(await resp.json());
}

export async function batchDeleteAsync(access_token: string, clientIds: string[]): Promise<void> {
  await ThinRequestAsync({
    access_token,
    url: `${PathBasePrefix}/admin/api/clients/`,
    method: 'delete',
    body: JSON.stringify({ "clientIds": clientIds }),
  });
}

export async function updateAsync(access_token: string, clientId: string, client: API.ClientDto): Promise<void> {
  let allowedScopes = "";
  if (client.allowedScopes.join) {
    allowedScopes = client.allowedScopes.join();
  }
  else {
    allowedScopes = <string><unknown>client.allowedScopes;
  }

  await ThinRequestAsync({
    access_token,
    url: `${PathBasePrefix}/admin/api/clients/${clientId}`,
    method: 'put',
    body: JSON.stringify({ ...client, allowedScopes }),
  });
}