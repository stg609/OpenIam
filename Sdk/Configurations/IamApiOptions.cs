namespace Charlie.OpenIam.Sdk.Configurations
{
    /// <summary>
    /// Iam 配置
    /// </summary>
    public class IamApiOptions : IamBasicOptions
    {
        /// <summary>
        /// Api Resource 名称
        /// </summary>
       public string Audience { get; set; }
    }
}
