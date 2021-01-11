using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Charlie.OpenIam.Common.Helpers;
using Charlie.OpenIam.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Charlie.OpenIam.Web.Infra
{
    public class UnitOfWorkActionFilter : IAsyncActionFilter
    {
        private readonly IServiceProvider _sp;

        //private readonly IUnitOfWork _uow;
        private readonly ILogger<UnitOfWorkActionFilter> _logger;

        public UnitOfWorkActionFilter(IServiceProvider sp, ILogger<UnitOfWorkActionFilter> logger)
        {
            //_uow = uow;

            _sp = sp;
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (controllerActionDescriptor == null)
            {
                await next.Invoke();
                return;
            }

            bool uowExistedOnMethodLevel = true;
            var uowAttr = controllerActionDescriptor.MethodInfo.GetCustomAttributes(typeof(UnitOfWorkAttribute), inherit: true);
            if (!uowAttr.Any())
            {
                uowExistedOnMethodLevel = false;
                uowAttr = controllerActionDescriptor.ControllerTypeInfo.GetCustomAttributes(typeof(UnitOfWorkAttribute), inherit: true);
            }
            var ignoreUowAttr = controllerActionDescriptor.MethodInfo.GetCustomAttributes(typeof(IgnoreUnitOfWorkAttribute), inherit: true);
            if (ignoreUowAttr.Any() || !uowAttr.Any())
            {
                // 如果 action 带有 ignoreUoW 或者 没有找到任何 UnitOfWork 则不启用 transaction
                await next.Invoke();
                return;
            }

            if (!uowExistedOnMethodLevel && context.HttpContext.Request.Method == HttpMethods.Get)
            {
                // 如果 action 级别没有任何 uow attribute，则默认情况下，对于 GET 也不进行 UOW
                await next.Invoke();
                return;
            }

            var uow = (UnitOfWorkAttribute)uowAttr.First();
            IUnitOfWork _uow = null;
            if (uow.DbContextType == null)
            {
                _uow = _sp.GetRequiredService<IUnitOfWork>();
            }
            else
            {
                _uow = _sp.GetRequiredService(uow.DbContextType) as IUnitOfWork;
            }

            string transactionId = null;
            var trans = await _uow.BeginAsync();
            transactionId = trans.TransactionId;

            using (trans.Transaction)
            {
                _logger.LogDebug(Helper.FormatLog($"Begin Transaction for {GetActionInfo(controllerActionDescriptor, context.ActionArguments)}."));
                context.HttpContext.Request.Headers.Add("X-TransactionId", transactionId);
                var executedContext = await next.Invoke();

                if (executedContext.Exception == null)
                {
                    _logger.LogDebug(Helper.FormatLog($"Start Commit Transaction."));
                    await _uow.CommitAsync(transactionId);
                    _logger.LogDebug(Helper.FormatLog($"Committed Transaction."));
                }
                else
                {
                    _uow.Rollback();
                    _logger.LogWarning(Helper.FormatLog($"Rolled back Transaction Finished of exception {executedContext.Exception.Message}.", logLevel: LogLevel.Warning));
                }
            }
        }

        private string GetActionInfo(ControllerActionDescriptor controllerActionDescriptor, IDictionary<string, object> arguments)
        {
            string parameters = String.Empty;
            if (arguments != null && arguments.Any())
            {
                parameters = String.Join(",", arguments.Select(itm => itm.Key + ":" + itm.Value));
            }
            return $"{controllerActionDescriptor.ControllerName}.{controllerActionDescriptor.ActionName}({parameters})";
        }
    }
}
