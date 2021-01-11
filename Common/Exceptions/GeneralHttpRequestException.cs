using System;
using System.Net;

namespace Charlie.OpenIam.Common.Exceptions
{
    /// <summary>
    /// 通用的 http 请求错误，含状态码
    /// </summary>
    [Serializable]
    public class GeneralHttpRequestException : Exception
    {
        /// <summary>
        /// Http 的状态码
        /// </summary>
        public HttpStatusCode StatusCode
        {
            get;
        }

        /// <summary>
        /// 错误类型
        /// </summary>
        public string Type
        {
            get;
        }

        /// <summary>
        /// 错误标题
        /// </summary>
        public string Title
        {
            get;
        }

        /// <summary>
        /// 错误描述
        /// </summary>
        public string Detail
        {
            get;
        }

        public GeneralHttpRequestException(HttpStatusCode statusCode, string message, string type = null, string title = null, string detail = null)
            : base(message)
        {
            Type = type;
            Title = title;
            Detail = detail;
            StatusCode = statusCode;
        }
    }
}
