import { UserManagerSettings } from 'oidc-client';

export const PathBasePrefix: string = "https://localhost:5002";
export const SiginPathSegment: string = "signin";

export default <UserManagerSettings>{
    authority: `https://localhost:5002/`,
    client_id: "d1e4f462-67ac-4fdf-b978-a1193a36d28d",
    client_secret: "U0KJ4S21YIN9FW6GRHWS3HRRUEK2G8",
    redirect_uri: `http://localhost:8000/${SiginPathSegment}`,
    response_type: "code",
    scope: "openid profile iamApi",
    post_logout_redirect_uri: "http://localhost:8000/signout-callback-oidc",
    silent_redirect_uri: "http://localhost:8000/silent-renew"
};