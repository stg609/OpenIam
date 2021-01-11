using System;

namespace Charlie.OpenIam.Web.Infra
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class UnitOfWorkAttribute : Attribute
    {
        public UnitOfWorkAttribute()
        {

        }

        public UnitOfWorkAttribute(Type dbContextType)
        {
            DbContextType = dbContextType;
        }

        public Type DbContextType { get; }
    }
}
