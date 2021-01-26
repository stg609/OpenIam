export type PermissionDto = {
    id: string;
    name: string;
    desc: string;
    key: string;
    type: number;
    clientId?: string;
    parentId?: string;
    url?: string;
    icon?: string;
    order?: number;
}