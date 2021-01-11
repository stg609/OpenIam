import { ApiResponseError } from "./ApiResponseError";
import ApiResult from "./ApiResult";
import IamBasicOptions from "./IamBasicOptions";
import { UserBasicInfoDto, UserNewDto, UserNewRespDto, UserUpdateDto } from "./UserDtos";

export class IdsApi {

    private _iamOptions: IamBasicOptions;

    constructor(settings: IamBasicOptions) {
        this._iamOptions = settings;
    }

    /**
     * 判断用户是否拥有权限
     * @param permissionKey 权限的Key
     * @param isAdminRequired 是否要求是管理员
     */
    public hasPermission(accessToken: string, permissionKey: string, isAdminRequired: boolean = false): Promise<ApiResult<boolean>> {
        return this.invokeInternalAsync<boolean>(`${this._iamOptions.Authority}/api/user/permcheck?permkey=${permissionKey}&isadmin=${isAdminRequired}`, 'get', null, accessToken);
    }

    /**
     * 获取当前登录用户的基本信息
     */
    public async getCurrentUserBasicInfoAsync(accessToken: string): Promise<ApiResult<UserBasicInfoDto>> {
        return this.invokeInternalAsync<UserBasicInfoDto>(`${this._iamOptions.Authority}/api/user`, 'get', null, accessToken);
    }

    /**
     * 添加新用户
     * @param model 用于新增用户的数据模型
     * @param accessToken 当前登陆用户的 access token
     */
    public async createUserAsync(model: UserNewDto, accessToken: string): Promise<ApiResult<UserNewRespDto>> {
        return this.invokeInternalAsync<UserNewRespDto>(`${this._iamOptions.Authority}/admin/api/users`, 'post', model, accessToken);
    }

    /**
    * 更新用户
    * @param userId 用户编号
    * @param model 用于更新用户的数据模型
    * @param accessToken 当前登陆用户的 access token
    */
    public async updateUserAsync(userId: string, model: UserUpdateDto, accessToken: string): Promise<ApiResult> {
        return this.invokeInternalAsync(`${this._iamOptions.Authority}/admin/api/users/${userId}`, 'put', model, accessToken);
    }

    /**
     * 内部调用
     * @param url 
     * @param method 
     * @param body 
     * @param accessToken 
     */
    private async invokeInternalAsync<TData = void>(url: string, method: string, body: any, accessToken: string): Promise<ApiResult<TData>> {
        if (accessToken == null) {
            return <ApiResult<TData>>{
                IsSucceed: false,
                Message: 'Access token 不能为空'
            }
        }

        var req = new Request(url, {
            method: method,
            body: body != null ? JSON.stringify(body) : null,
            headers: {
                "Authorization": `Bearer ${accessToken}`,
                "Content-type": "application/json; charset=UTF-8"
            }
        });

        try {
            let resp = await fetch(req);
            if (resp.ok) {
                return <ApiResult<TData>>{
                    IsSucceed: true,
                    StatusCode: resp.status,
                    Data: <TData>await resp.json()
                };
            }
            else {
                let error = <ApiResponseError>await resp.json();
                return <ApiResult<TData>>{
                    IsSucceed: false,
                    StatusCode: resp.status,
                    Message: error.detail
                };
            }
        } catch (exception) {
            // 发生网络错误
            return <ApiResult<TData>>{
                IsSucceed: false,
                Message: exception
            };
        }
    }
}
