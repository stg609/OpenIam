namespace Charlie.OpenIam.Sdk.Services.Dtos
{
    /// <summary>
    /// Api 调用结果
    /// </summary>
    public class ApiResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSucceed { get; set; }

        /// <summary>
        /// Api 请求返回的状态码
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// 请求返回的消息，多用于当请求失败的信息
        /// </summary>
        public string Message { get; set; }
    }

    /// <summary>
    /// Api 调用结果
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public class ApiResult<TData>
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSucceed { get; set; }

        /// <summary>
        /// Api 请求返回的状态码
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Data 表示请求成功后的数据
        /// </summary>
        public TData Data { get; set; }

        /// <summary>
        /// 请求返回的消息，多用于当请求失败的信息
        /// </summary>
        public string Message { get; set; }
    }
}
