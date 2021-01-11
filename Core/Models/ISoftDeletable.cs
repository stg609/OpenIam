namespace Charlie.OpenIam.Core.Models
{
    /// <summary>
    /// 是否支持软删除
    /// </summary>
    /// <remarks>
    /// 这个接口只是个 marker。如果支持，则 DbContext 会自动处理删除、查询操作
    /// </remarks>
    public interface ISoftDeletable
    {
    }
}
