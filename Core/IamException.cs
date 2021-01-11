using System;
using System.Net;
using Microsoft.Extensions.Localization;

namespace Charlie.OpenIam.Core
{
    [Serializable]
    public class IamException : Exception
    {
        public HttpStatusCode StatusCode { get; private set; }
        public string ErrCode { get; private set; }

        private readonly string _message;
        private readonly IStringLocalizer _localizer;
        private readonly string[] _args;

        public IamException(HttpStatusCode statusCode, IStringLocalizer localizer, string errCode, params string[] args)
          : base()
        {
            StatusCode = statusCode;
            ErrCode = errCode;
            _localizer = localizer;
            _args = args;
        }

        public IamException(HttpStatusCode statusCode, string message, Exception ex = null)
         : base(message, ex)
        {
            StatusCode = statusCode;
            _message = message;
        }


        public override string Message
        {
            get
            {
                if (_localizer != null)
                {
                    return _localizer[ErrCode, _args];
                }
                else
                {
                    return _message;
                }
            }
        }
    }
}
