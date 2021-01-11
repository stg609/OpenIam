using System;
using System.Runtime.CompilerServices;

using Microsoft.Extensions.Logging;

namespace Charlie.OpenIam.Common.Helpers
{
    /// <summary>
    /// 日志帮助类
    /// </summary>
    public static partial class Helper
    {
        /// <summary>
        /// 格式化日志输出
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="correlationId">事务 id</param>
        /// <param name="callerMemberName">调用方</param>
        /// <param name="sourceFilePath">调用方路径</param>
        /// <param name="sourceLineNumber">调用方行号</param>
        /// <returns></returns>
        public static string FormatLog(string message, string correlationId = "",
            LogLevel logLevel = LogLevel.Information,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0
            )
        {
            string dbg = logLevel <= LogLevel.Information ? "" : $"[Source File Path: {sourceFilePath}, line no: {sourceLineNumber}]";
            if (String.IsNullOrWhiteSpace(correlationId))
            {
                return $"----- {callerMemberName}: {message} {dbg}";
            }
            return $"----- {callerMemberName}({correlationId}): {message} {dbg}";
        }
    }
}
