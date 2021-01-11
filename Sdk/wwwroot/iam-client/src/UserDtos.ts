/**
 * 用户基本信息
 */
export class UserBasicInfoDto {

     /**
     * 用户编号
     */
    public Id: string;

    /**
     * 工号
     */
    public Saler: string;  

    /**
     * 姓名
     */
    public Name: string;

    /**
     * 手机号
     */
    public Phone: string;
}

/**
 * 新增用户的视图模型
 */
export class UserNewDto {
    /**
     * 工号
     */
    public Saler: string;

    /**
     * 用户名
     */
    public Username: string;

    /**
     * 邮箱
     */
    public Email: string;

    /**
     * 姓名
     */
    public Name: string;

    /**
     * 性别
     */
    public Sex: number

    /**
     * 职务
     */
    public Position: string;

    /**
     * 手机号
     */
    public Phone: string;

    /**
     * 身份证号
     */
    public IdCard: string;

    /**
     * 家庭住址
     */
    public HomeAddress: string;

    /**
     * 初始密码
     */
    public Password: string;

    /**
     * 用户所属的组织机构编号，多个组织用逗号分隔
     */
    public OrgIds: string;

    /**
     * 是否激活
     */
    public IsActive: boolean
}

/**
 * 新增用户返回的结果
 */
export class UserNewRespDto {
    /**
     * 用户编号
     */
    public Id: string;   
}

/**
 * 更新用户的视图模型
 */
export class UserUpdateDto {
    /**
     * 工号
     */
    public Saler: string;

    /**
     * 邮箱
     */
    public Email: string;

    /**
     * 姓名
     */
    public Name: string;

    /**
     * 性别
     */
    public Sex?: number

    /**
     * 职务
     */
    public Position: string;

    /**
     * 手机号
     */
    public Phone: string;

    /**
     * 身份证号
     */
    public IdCard: string;

    /**
     * 家庭住址
     */
    public HomeAddress: string;

    /**
     * 初始密码
     */
    public Password: string;

    /**
     * 用户所属的组织机构编号，多个组织用逗号分隔
     */
    public OrgIds: string;

    /**
     * 是否激活
     */
    public IsActive?: boolean
}