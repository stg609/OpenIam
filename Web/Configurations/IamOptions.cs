namespace Charlie.OpenIam.Web.Configurations
{
    public class IamOptions
    {
        /// <summary>
        /// 部署的地址
        /// </summary>
        public string Host { get; set; }

        public string[] ValidIssuers { get; set; }
    }
}
