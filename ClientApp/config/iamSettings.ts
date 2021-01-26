import { UserManagerSettings } from 'oidc-client';

// 从配置中加载
const AppSettings = window['REACT_APP_IAM_SETTINGS'];

export const PathBasePrefix: string = AppSettings.PathBasePrefix ?? "";
export const SiginPathSegment: string = AppSettings.SiginPathSegment ?? "signin";

export default <UserManagerSettings>{
    authority: AppSettings.Authority ?? "",
    client_id: AppSettings.ClientId ?? "",
    redirect_uri: `${AppSettings.FrontEndHost}/${SiginPathSegment}`,
    response_type: "code",
    scope: "openid profile iamApi",
    post_logout_redirect_uri: `${AppSettings.FrontEndHost}/signout-callback-oidc`,
    silent_redirect_uri: `${AppSettings.FrontEndHost}/silent-renew`
};