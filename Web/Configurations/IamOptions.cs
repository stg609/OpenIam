namespace Charlie.OpenIam.Web.Configurations
{
    /// <summary>
    /// Iam Web 的基础配置
    /// </summary>
    public class IamOptions
    {
        /// <summary>
        /// 部署的地址，当自己作为一个 Api Resource 时，所指向的 Iam Server 地址
        /// </summary>
        public string Host { get; set; }

        public string[] ValidIssuers { get; set; }

        /// <summary>
        /// 基地址
        /// </summary>
        /// <remarks>
        /// 如果想部署到 nginx 的 子目录中，比如 foo 这个目录，那此时 url 为 /foo/api/user。
        /// 但是 .net core 处理时需要去掉 foo，此时可以使用 PathBase，结合 app.UsePathBase 方法
        /// </remarks>
        public string PathBase { get; set; }
    }
}
