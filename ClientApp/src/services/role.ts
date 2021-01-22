import type { PermissionDto } from "@/pages/Permission/data";
import { ThinRequestAsync } from "@/utils/utils";
import { PathBasePrefix } from "../../config/iamSettings";

export async function queryAsync(params: {
  access_token: string, name?: string,
  clientId?: string, withPerms?: boolean, excludeOrgId?: string,
  excludeUserId?: string, pageIndex?: number, pageSize?: number
}): Promise<API.PaginatedDto<API.RoleDto>> {
  const queryString = [
    `name=${params.name ?? ""}`,
    `clientId=${params.clientId ?? ""}`,
    `withPerms=${params.withPerms ?? false}`,
    `excludeOrgId=${params.excludeOrgId ?? ""}`,
    `excludeUserId=${params.excludeUserId ?? ""}`,
    `pageIndex=${params.pageIndex ?? 1}`,
    `pageSize=${params.pageSize ?? 10}`,
  ].join('&');

  const resp = await ThinRequestAsync({
    access_token: params.access_token,
    url: `${PathBasePrefix}/admin/api/roles?${queryString}`,
  });

  return (await resp.json()) as API.PaginatedDto<API.RoleDto>;
}

export async function createAsync(access_token: string, role: API.RoleDto): Promise<void> {
  await ThinRequestAsync({
    access_token,
    url: `${PathBasePrefix}/admin/api/roles/`,
    method: 'post',
    body: JSON.stringify(role),
  });
}

export async function batchDeleteAsync(access_token: string, ids: string[]): Promise<void> {
  await ThinRequestAsync({
    access_token,
    url: `${PathBasePrefix}/admin/api/roles/`,
    method: 'delete',
    body: JSON.stringify({ "ids": ids }),
  });
}

export async function updateAsync(access_token: string, id: string, role: API.RoleDto): Promise<void> {
  await ThinRequestAsync({
    access_token,
    url: `${PathBasePrefix}/admin/api/roles/${id}`,
    method: 'put',
    body: JSON.stringify(role),
  });
}

export async function getPermsAsync(access_token: string, id: string): Promise<{ data: PermissionDto[] }> {
  const resp = await ThinRequestAsync({
    access_token,
    url: `${PathBasePrefix}/admin/api/roles/${id}/permissions`,
  });

  return {
    data: (await resp.json()) as PermissionDto[]
  }
}

export async function addPermsAsync(access_token: string, id: string, ids: string[]): Promise<void> {
  await ThinRequestAsync({
    access_token,
    url: `${PathBasePrefix}/admin/api/roles/${id}/permissions`,
    method: 'post',
    body: JSON.stringify({ permissionIds: ids }),
  });
}

export async function removePermsAsync(access_token: string, id: string, ids: string[]): Promise<void> {
  await ThinRequestAsync({
    access_token,
    url: `${PathBasePrefix}/admin/api/roles/${id}/permissions`,
    method: 'delete',
    body: JSON.stringify({ permissionIds: ids }),
  });
}