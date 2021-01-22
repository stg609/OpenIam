import type { PermissionDto } from "@/pages/Permission/data";
import { ThinRequestAsync } from "@/utils/utils";
import { PathBasePrefix } from "../../config/iamSettings";

export async function queryAsync(params: {
  access_token: string, name?: string,
  key?: string, url?: string, clientId?: string,
  type?: number, excludeRoleId?: string, excludeUserId?: string
}): Promise<{ data: PermissionDto[] }> {
  const requestHeaders: HeadersInit = new Headers();
  requestHeaders.set('Authorization', `Bearer ${params.access_token}`);

  const finalType = params.type == -1 ? null : params.type;
  const queryString = [
    `name=${params.name ?? ""}`,
    `targetClientId=${params.clientId ?? ""}`,
    `key=${params.key ?? ""}`,
    `url=${params.url ?? ""}`,
    `excludeRoleId=${params.excludeRoleId ?? ""}`,
    `excludeUserId=${params.excludeUserId ?? ""}`,
    `type=${finalType ?? ""}`,
  ].join('&');

  const resp = await ThinRequestAsync({ url: `${PathBasePrefix}/admin/api/permissions?${queryString}`, access_token: params.access_token });
  return {
    data: (await resp.json()) as PermissionDto[]
  }
}

export async function createAsync(access_token: string, permission: PermissionDto): Promise<void> {
  await ThinRequestAsync({
    access_token,
    url: `${PathBasePrefix}/admin/api/permissions/`,
    method: 'post',
    body: JSON.stringify(permission),
  });
}

export async function batchDeleteAsync(access_token: string, ids: string[]): Promise<void> {
  await ThinRequestAsync({
    access_token,
    url: `${PathBasePrefix}/admin/api/permissions/`,
    method: 'delete',
    body: JSON.stringify({ "ids": ids }),
  });
}


export async function updateAsync(access_token: string, id: string, permission: PermissionDto): Promise<void> {
  await ThinRequestAsync({
    access_token,
    url: `${PathBasePrefix}/admin/api/permissions/${id}`,
    method: 'put',
    body: JSON.stringify(permission),
  });
}