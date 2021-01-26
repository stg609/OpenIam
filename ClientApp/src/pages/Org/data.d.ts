export type OrgDto = {
    id: string;
    name: string;
    desc: string;
    isEnabled: boolean;    
    mobile?: string;
    address?: string;
    parentId?: string;    
}