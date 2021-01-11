using System;
using System.Collections.Generic;
using System.Linq;

namespace Charlie.OpenIam.Common.Helpers
{
    public static partial class Helper
    {
        /// <summary>
        /// 获取树形数据结构
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="itms"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static IEnumerable<TObject> GetTreeLayout<TObject>(this IEnumerable<TObject> itms, TObject parent = null)
          where TObject : class, IHasParentIdAndChildren<TObject>
        {
            var roots = itms.Where(itm => parent == null ? String.IsNullOrEmpty(itm.ParentId) : itm.ParentId == parent?.Id);
            foreach (var org in roots)
            {
                org.Children.AddRange(GetTreeLayout(itms, org));
            }
            return roots;
        }

        /// <summary>
        /// 对 Tree 进行过滤
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="itms"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public static IEnumerable<TObject> FilterTree<TObject>(this IEnumerable<TObject> itms, IEnumerable<string> ids)
           where TObject : class, IHasParentIdAndChildren<TObject>
        {
            if (itms == null)
            {
                return Enumerable.Empty<TObject>();
            }

            if (ids == null || !ids.Any())
            {
                return itms;
            }

            var result = new List<TObject>();
            foreach (var itm in itms)
            {
                if (ids.Contains(itm.Id))
                {
                    result.Add(itm);
                }

                result.AddRange(FilterTree(itm.Children, ids));
            }

            return result;
        }

        public static bool IsItemExisted<TObject>(this TObject itm, string id)
           where TObject : class, IHasParentIdAndChildren<TObject>
        {
            if (itm == null)
            {
                return false;
            }

            if (String.IsNullOrWhiteSpace(id))
            {
                return false;
            }

            if (itm.Id == id)
            {
                return true;
            }

            if (itm.Children == null || !itm.Children.Any())
            {
                return false;
            }

            foreach (var child in itm.Children)
            {
                if (child.IsItemExisted(id))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
