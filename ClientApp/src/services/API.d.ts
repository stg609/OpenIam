declare namespace API {

  export type PaginatedDto<T> = {
    data: T[];
    total: number;
    pageSize: number;
    pageIndex: number;
  }

  export type ClientDto = {
    alwaysSendClientClaims: boolean;
    alwaysIncludeUserClaimsInIdToken: boolean;
    accessTokenLifetime: number;
    allowedScopes: string[];
    clientId: string;
    clientName: string;
    clientUri: string;
    description: string;
    isEnabled: boolean;
  }

  export type ClientCreateRespDto = {
    clientId: string;
    plainSecret: string;
    clientName: string;
    description: string;
  }

  export type RoleDto = {
    id: string;
    name: string;
    isAdmin: boolean;
    isSuperAdmin: boolean;
    // desc?: string;
    // clientId?: string;
    // createdAt: string;
  }

  export type PermissionDto = {
    id: string;
    name: string;
    key: string;
  };

  export type UserRolePermissionDto = {
    roles?: RoleDto[];
    permissions?: PermissionDto[];
  };

  export type CurrentUser = {
    avatar?: string;
    name?: string;
    userid?: string;
    isSuperAdmin: boolean = false;
    isAdmin: boolean = false;
    permissions?: string[],
    accessToken?: string;
    idToken?: string;
  };

  export type LoginStateType = {
    status?: 'ok' | 'error';
    type?: string;
  };

  export type NoticeIconData = {
    id: string;
    key: string;
    avatar: string;
    title: string;
    datetime: string;
    type: string;
    read?: boolean;
    description: string;
    clickClose?: boolean;
    extra: any;
    status: string;
  };


}
