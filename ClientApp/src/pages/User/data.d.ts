export type UserDto = {
    id: string;
    firstName?: string;
    lastName?: string;
    jobNo?: string;
    username: string;
    phone?: string;
    idCard?: string;
    position?: string;
    homeAddress?: string;
    organizations?: string[];
    isActive: boolean;
    lastIp: string;
    lastLoginAt: string;
    createdAt: string;
    password?: string;
}