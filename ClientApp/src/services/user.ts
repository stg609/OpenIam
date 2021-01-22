import type { UserDto } from '@/pages/User/data';
import type { PermissionDto } from '@/pages/Permission/data';
import { PathBasePrefix } from "../../config/iamSettings";
import { ThinRequestAsync } from '@/utils/utils';
import { UserManager } from 'oidc-client';
import * as IamSettings from '../../config/iamSettings';

export async function queryAsync(params: {
  access_token: string, firstname?: string, lastname?: string, jobNo?: string,
  idCard?: string, phone?: string, email?: string,
  excludeOrgId?: string, isActive?: boolean,
  pageIndex?: number, pageSize?: number
}): Promise<API.PaginatedDto<UserDto>> {

  const queryString = [
    `firstname=${params.firstname ?? ""}`,
    `lastname=${params.lastname ?? ""}`,
    `jobNo=${params.jobNo ?? ""}`,
    `idCard=${params.idCard ?? ""}`,
    `phone=${params.phone ?? ""}`,
    `email=${params.email ?? ""}`,
    `excludeOrgId=${params.excludeOrgId ?? ""}`,
    `isActive=${params.isActive ?? ""}`,
    `pageIndex=${params.pageIndex ?? ""}`,
    `pageSize=${params.pageSize ?? ""}`,
  ].join('&');

  const resp = await ThinRequestAsync({
    access_token: params.access_token,
    url: `${PathBasePrefix}/admin/api/users?${queryString}`,
  });

  return (await resp.json()) as API.PaginatedDto<UserDto>;
}

export async function createAsync(access_token: string, model: UserDto): Promise<void> {
  await ThinRequestAsync({
    access_token,
    url: `${PathBasePrefix}/admin/api/users/`,
    method: 'post',
    body: JSON.stringify(model),
  });
}

export async function deleteAsync(access_token: string, id: string): Promise<void> {
  await ThinRequestAsync({
    access_token,
    url: `${PathBasePrefix}/admin/api/users/${id}`,
    method: 'delete'
  });
}

export async function updateAsync(access_token: string, id: string, model: UserDto): Promise<void> {
  await ThinRequestAsync({
    url: `${PathBasePrefix}/admin/api/users/${id}`,
    access_token,
    method: 'put',
    body: JSON.stringify(model)
  });
}

export async function inActiveAsync(access_token: string, id: string, isActive: boolean): Promise<void> {
  await ThinRequestAsync({
    url: `${PathBasePrefix}/admin/api/users/${id}/activity`,
    access_token,
    method: 'put',
    body: JSON.stringify({ isActive })
  });
}

export async function resetPwdAsync(access_token: string, id: string): Promise<string> {
  const resp = await ThinRequestAsync({
    url: `${PathBasePrefix}/admin/api/users/${id}/pwd`,
    access_token,
    method: 'post'
  });

  return resp.text();
}

export async function queryCurrentPermissionsAsync(access_token: string): Promise<{ isSuperAdmin: boolean, isAdmin: boolean } | null | undefined> {
  const resp = await ThinRequestAsync({ url: `${PathBasePrefix}/api/user/permissions`, access_token });
  const perms = (await resp.json()) as API.UserRolePermissionDto;
  return {
    isSuperAdmin: perms.roles?.some(role => role.isSuperAdmin) ?? false,
    isAdmin: perms.roles?.some(role => role.isAdmin) ?? false,
  }
}

export async function getPermsAsync(access_token: string, id: string): Promise<{ data: PermissionDto[] }> {
  const resp = await ThinRequestAsync({
    access_token,
    url: `${PathBasePrefix}/admin/api/users/${id}/permissions`,
  });

  return {
    data: (await resp.json()) as PermissionDto[]
  }
}

export async function addPermsAsync(access_token: string, id: string, ids: string[]): Promise<void> {
  await ThinRequestAsync({
    access_token,
    url: `${PathBasePrefix}/admin/api/users/${id}/permissions`,
    method: 'post',
    body: JSON.stringify({ permissionIds: ids })
  });
}

export async function removePermsAsync(access_token: string, id: string, ids: string[]): Promise<void> {
  await ThinRequestAsync({
    access_token,
    url: `${PathBasePrefix}/admin/api/users/${id}/permissions`,
    method: 'delete',
    body: JSON.stringify({ permissionIds: ids }),
  });
}


export async function getRolesAsync(access_token: string, id: string): Promise<{ data: API.RoleDto[] }> {
  const resp = await ThinRequestAsync({
    access_token,
    url: `${PathBasePrefix}/admin/api/users/${id}/roles`,
  });

  return {
    data: (await resp.json()) as API.RoleDto[]
  }
}

export async function addRolesAsync(access_token: string, id: string, ids: string[]): Promise<void> {
  await ThinRequestAsync({
    access_token,
    url: `${PathBasePrefix}/admin/api/users/${id}/roles`,
    method: 'post',
    body: JSON.stringify({ roleIds: ids }),
  });

}

export async function removeRolesAsync(access_token: string, id: string, ids: string[]): Promise<void> {
  await ThinRequestAsync({
    access_token,
    url: `${PathBasePrefix}/admin/api/users/${id}/roles`,
    method: 'delete',
    body: JSON.stringify({ roleIds: ids }),
  });

}

export async function logoutAsync(id_token: string) {
  await new UserManager(IamSettings.default).signoutRedirect({ id_token_hint: id_token, post_logout_redirect_uri: IamSettings.default.post_logout_redirect_uri });
}