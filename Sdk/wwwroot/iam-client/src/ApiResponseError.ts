/**
 * Api 请求错误，ProblemDetails 格式
 */
export interface ApiResponseError {
    type: string;
    title: string;
    detail: string;
}
