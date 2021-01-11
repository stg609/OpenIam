using System;
using System.Net.Http;
using System.Threading.Tasks;
using Charlie.OpenIam.Common.Exceptions;
using Newtonsoft.Json;

namespace Charlie.OpenIam.Common.Helpers
{
    /// <summary>
    /// 帮助类
    /// </summary>
    public static partial class Helper
    {
        /// <summary>
        /// 对返回值进行预处理
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="response"></param>
        /// <param name="onResponseSuccess"></param>
        /// <returns></returns>
        public static async Task<TResult> WhenResponseSuccess<TResult>(this HttpResponseMessage response, Func<string, TResult> onResponseSuccess)
        {
            string resp = await response.WhenResponseSuccess();
            if (resp != null)
            {
                return onResponseSuccess(resp);
            }
            else
            {
                return default(TResult);
            }
        }

        /// <summary>
        /// 对返回值进行预处理
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="response"></param>
        /// <param name="onResponseSuccess"></param>
        /// <returns></returns>
        public static async Task WhenResponseSuccess(this HttpResponseMessage response, Action<string> onResponseSuccess)
        {
            string resp = await response.WhenResponseSuccess();
            if (resp != null)
            {
                onResponseSuccess(resp);
            }
        }

        /// <summary>
        /// 对返回值进行预处理
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static async Task<string> WhenResponseSuccess(this HttpResponseMessage response)
        {
            string jsonResponse = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                SimpleProblemDetailsData problemDetails;
                try
                {
                    problemDetails = JsonConvert.DeserializeObject<SimpleProblemDetailsData>(jsonResponse);
                }
                catch (JsonReaderException ex)
                {
                    // 返回的可能并非是一个 Json，比如一个 Html
                    throw new GeneralHttpRequestException(System.Net.HttpStatusCode.InternalServerError, $"访问 {response.RequestMessage.RequestUri.PathAndQuery} 失败 {response.StatusCode} {jsonResponse}。 返回值无法解析成 Json:{ex.Message}.");
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    //if (problemDetails != null && !String.IsNullOrWhiteSpace(problemDetails.Type) && _h400ErrorRegex.IsMatch(problemDetails.Type))
                    //{
                    //    // 对于 404 有2种可能，1. URL 不正确。2. REST 中的资源不存在。
                    //    // 如果是资源不存在这种错误，则直接返回 null
                    //    return null;
                    //}
                    //else
                    //{
                        // 不建议直接通过 EnsureSuccessStatusCode 方式抛出异常，因为这个抛出的是 HttpRequestException，会隐藏 StatusCode 等信息
                        throw new GeneralHttpRequestException(response.StatusCode, jsonResponse, problemDetails?.Type, problemDetails?.Title, problemDetails?.Detail);
                    //}
                }
                else
                {
                    throw new GeneralHttpRequestException(response.StatusCode, jsonResponse, problemDetails?.Type, problemDetails?.Title, problemDetails?.Detail);
                }
            }

            return jsonResponse;
        }
    }
}
