/**
 * Api 调用结果
 */
export default class ApiResult<TData = void> {

    /**
     * 是否成功
     */
    public IsSucceed: boolean;

    /**
     * Api 请求返回的状态码
     */
    public StatusCode?: number;

    /**
     * 请求返回的消息，多用于当请求失败的信息
     */
    public Message?: string;

    /**
     * 数据
     */
    public Data?: TData;
}

// export class ApiResultWithoutData {

//     /**
//      * 是否成功
//      */
//     public IsSucceed: boolean;

//     /**
//      * Api 请求返回的状态码
//      */
//     public StatusCode?: number;

//     /**
//      * 请求返回的消息，多用于当请求失败的信息
//      */
//     public Message?: string;
// }