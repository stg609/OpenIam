using System.Collections.Generic;

namespace Charlie.OpenIam.Common
{
	/// <summary>
    /// 含分页的Dto
    /// </summary>
    public class PaginatedDto<TData>
    {
		/// <summary>
        /// 分页数据
        /// </summary>
        public IEnumerable<TData> Data { get; set; }

        /// <summary>
        /// 总数
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 分页大小
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 页码，以1为起始
        /// </summary>
        public int PageIndex { get; set; }
    }
}
