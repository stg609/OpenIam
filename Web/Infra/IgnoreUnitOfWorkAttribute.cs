using System;

namespace Charlie.OpenIam.Web.Infra
{
    [AttributeUsage(AttributeTargets.Method)]
    public class IgnoreUnitOfWorkAttribute : Attribute
    {
    }
}
