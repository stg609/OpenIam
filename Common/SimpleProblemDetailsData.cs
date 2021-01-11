namespace Charlie.OpenIam.Common
{
    /// <summary>
    /// 简单的 ProblemDetails 类型
    /// </summary>
    /// <remarks>微软的 ProblemDetails 属于 Mvc namespace 下，所以新建一个简单的类型用于 responseHelper 处理。</remarks>
    public class SimpleProblemDetailsData
    {
        /// <summary>
        /// 错误类型
        /// </summary>
        public string Type
        {
            get;
            set;
        }

        /// <summary>
        /// 错误标题
        /// </summary>
        public string Title
        {
            get;
            set;
        }

        /// <summary>
        /// 错误详情
        /// </summary>
        public string Detail
        {
            get;
            set;
        }

    }
}
