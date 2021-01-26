import type { OrgDto } from "@/pages/Org/data";
import type { UserDto } from "@/pages/User/data";
import { ThinRequestAsync } from "@/utils/utils";
import { PathBasePrefix } from "../../config/iamSettings";

export async function queryAsync(access_token: string, name: string | null = null): Promise<{ data: OrgDto[] }> {
  const requestHeaders: HeadersInit = new Headers();
  requestHeaders.set('Authorization', `Bearer ${access_token}`);

  const queryString = [
    `name=${name ?? ""}`
  ].join('&');

  const resp = await ThinRequestAsync({
    access_token,
    url: `${PathBasePrefix}/admin/api/organizations?${queryString}`,
  });

  return {
    data: (await resp.json()) as OrgDto[]
  }
}

export async function createAsync(access_token: string, org: OrgDto): Promise<void> {
  await ThinRequestAsync({
    access_token,
    url: `${PathBasePrefix}/admin/api/organizations/`,
    method: 'post',
    body: JSON.stringify(org)
  });
}

export async function batchDeleteAsync(access_token: string, ids: string[]): Promise<void> {
  await ThinRequestAsync({
    access_token,
    url: `${PathBasePrefix}/admin/api/organizations/`,
    method: 'delete',
    body: JSON.stringify({ "ids": ids }),
  });
}

export async function updateAsync(access_token: string, id: string, org: OrgDto): Promise<void> {
  await ThinRequestAsync({
    access_token,
    url: `${PathBasePrefix}/admin/api/organizations/${id}`,
    method: 'put',
    body: JSON.stringify(org),
  });
}

export async function getRolesAsync(access_token: string, id: string): Promise<{ data: API.RoleDto[] }> {
  const resp = await ThinRequestAsync({
    access_token,
    url: `${PathBasePrefix}/admin/api/organizations/${id}/roles`
  });

  return {
    data: (await resp.json()) as API.RoleDto[]
  }
}

export async function addRolesAsync(params: { access_token: string, id: string, ids: string[] }): Promise<void> {
  await ThinRequestAsync({
    access_token: params.access_token,
    url: `${PathBasePrefix}/admin/api/organizations/${params.id}/roles`,
    method: 'post',
    body: JSON.stringify({ roleIds: params.ids }),
  });
}

export async function removeRolesAsync(params: { access_token: string, id: string, ids: string[] }): Promise<void> {
  await ThinRequestAsync({
    access_token: params.access_token,
    url: `${PathBasePrefix}/admin/api/organizations/${params.id}/roles`,
    method: 'delete',
    body: JSON.stringify({ roleIds: params.ids }),
  });
}

export async function addUsersAsync(params: { access_token: string, id: string, ids: string[] }): Promise<void> {
  await ThinRequestAsync({
    access_token: params.access_token,
    url: `${PathBasePrefix}/admin/api/organizations/${params.id}/users`,
    method: 'post',
    body: JSON.stringify({ userIds: params.ids }),
  });
}

export async function removeUsersAsync(params: { access_token: string, id: string, ids: string[] }): Promise<void> {
  await ThinRequestAsync({
    access_token: params.access_token,
    url: `${PathBasePrefix}/admin/api/organizations/${params.id}/users`,
    method: 'delete',
    body: JSON.stringify({ userIds: params.ids }),
  });
}

export async function getUsersAsync(access_token: string, id: string): Promise<{ data: UserDto[] }> {
  const resp = await ThinRequestAsync({
    access_token,
    url: `${PathBasePrefix}/admin/api/organizations/${id}/users`
  });

  return {
    data: (await resp.json()) as UserDto[]
  }
}