using System;

namespace Charlie.OpenIam.Core.Models
{
    /// <summary>
    /// 是否提供审计字段
    /// </summary>
    public interface IAuditable
    {
        /// <summary>
        /// 创建者
        /// </summary>
        string CreatedBy { get; }

        /// <summary>
        /// 创建时间
        /// </summary>
        DateTime CreatedAt { get; }

        /// <summary>
        /// 最后更新的用户
        /// </summary>
        string LastUpdatedBy { get; }

        /// <summary>
        /// 更新时间
        /// </summary>
        DateTime LastUpdatedAt { get; }
    }
}
