using System.Collections.Generic;

namespace Charlie.OpenIam.Common
{
    /// <summary>
    /// 是否包含 上下级
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IHasParentIdAndChildren<T>
       where T : class
    {
        /// <summary>
        /// 编号
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// 父级编号
        /// </summary>
        string ParentId { get; set; }

        /// <summary>
        /// 子级
        /// </summary>
        List<T> Children { get; set; }
    }
}
