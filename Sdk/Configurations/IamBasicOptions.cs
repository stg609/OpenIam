using System.ComponentModel.DataAnnotations;

namespace Charlie.OpenIam.Sdk.Configurations
{
    /// <summary>
    /// Iam 基础配置
    /// </summary>
    public class IamBasicOptions
    {
        /// <summary>
        /// OpenIam 地址
        /// </summary>
        [Required]
        public string Authority { get; set; }

        public string[] ValidIssuers { get; set; }
    }
}
