using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using XUtility.Http;
using XUtility.Json;

namespace XUtility.Client
{
    /// <summary>
    /// Client基类
    /// </summary>
    public abstract class ClientBase
    {
        private string _baseApiUrl;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="baseApiUrl"></param>
        public ClientBase(string baseApiUrl)
        {
            _baseApiUrl = baseApiUrl;
        }

        /// <summary>
        /// 日志对像
        /// </summary>
        protected abstract ILogger Logger { get; }

        /// <summary>
        /// Client名称
        /// </summary>
        protected abstract string Name { get; }

        /// <summary>
        /// 添加API验证参数
        /// </summary>
        /// <param name="apiName"></param>
        /// <param name="data"></param>
        /// <param name="ip"></param>
        protected abstract void AppendApiAuthParam(string apiName, IDictionary<string, string> data, string ip = null);

        /// <summary>
        /// 执行请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="apiName"></param>
        /// <param name="req"></param>
        /// <param name="method"></param>
        /// <param name="ip"></param>
        /// <param name="isFromAttr"></param>
        /// <param name="httpGetOptions"></param>
        /// <param name="httpPostOptions"></param>
        /// <param name="clientSetting"></param>
        /// <returns></returns>
        protected virtual async Task<T> DoRequest<T>(string apiName, object req, string method, string ip = null, bool isFromAttr = false, HttpGetOptions httpGetOptions = null, HttpPostOptions httpPostOptions = null, Action<HttpClient> clientSetting = null) where T : class, new()
        {
            string ret = await DoRequest(apiName, req, method, ip, isFromAttr, httpGetOptions, httpPostOptions, clientSetting);

            return ParseResponse<T>(ret);
        }

        /// <summary>
        /// 执行请求
        /// </summary>
        /// <param name="apiName"></param>
        /// <param name="req"></param>
        /// <param name="method"></param>
        /// <param name="ip"></param>
        /// <param name="isFromAttr"></param>
        /// <param name="httpGetOptions"></param>
        /// <param name="httpPostOptions"></param>
        /// <param name="clientSetting"></param>
        /// <returns></returns>
        protected virtual async Task<string> DoRequest(string apiName, object req, string method, string ip = null, bool isFromAttr = false, HttpGetOptions httpGetOptions = null, HttpPostOptions httpPostOptions = null, Action<HttpClient> clientSetting = null)
        {
            string url = _baseApiUrl;
            url += apiName;
            var data = new Dictionary<string, string>();
            FillData(req, data, isFromAttr);
            AppendApiAuthParam(apiName, data, ip);

            var paramStr = GetParamStr(data);
            Logger.LogDebug($"[{Name}]开始调用Http接口，Url={url}，Param={paramStr}");

            string ret = "";
            try
            {
                if (method.ToLower() == "post")
                {
                    ret = await HttpRequestUtils.PostFormAsync(url, data, null, httpPostOptions, clientSetting);
                }
                else if (method.ToLower() == "postjson")
                {
                    ret = await HttpRequestUtils.PostAsync(url, req.ToJson(), null, httpPostOptions, clientSetting);
                }
                else
                {
                    ret = await HttpRequestUtils.GetAsync(url, data, httpGetOptions, clientSetting);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.InnerException == null ? ex : ex.InnerException, $"[{Name}]调用Http接口出现错误，Url={url}，Param={paramStr}");
                throw ex;
            }

            Logger.LogDebug($"[{Name}]Http接口调用完成，Url={url}，Param={paramStr}，Response={ret}");
            return ret;
        }

        /// <summary>
        /// 填充数据
        /// </summary>
        /// <param name="req"></param>
        /// <param name="data"></param>
        /// <param name="isFromAttr"></param>
        /// <param name="ignoreNullOrEmptyValue"></param>
        protected virtual void FillData(object req, IDictionary<string, string> data, bool isFromAttr = false, bool ignoreNullOrEmptyValue = false)
        {
            if (req == null)
            {
                return;
            }

            var reqType = req.GetType();
            var props = reqType.GetProperties();
            foreach (var prop in props)
            {
                if (isFromAttr)
                {
                    var attrs = prop.GetCustomAttributes(typeof(System.Runtime.Serialization.DataMemberAttribute), true);
                    if (attrs != null && attrs.Length > 0)
                    {
                        var attr = attrs[0] as System.Runtime.Serialization.DataMemberAttribute;
                        var name = attr.Name;
                        var val = prop.GetValue(req, null);
                        if (prop.PropertyType.IsEnum)
                        {
                            val = Convert.ChangeType(val, Enum.GetUnderlyingType(prop.PropertyType));
                        }
                        if (!data.ContainsKey(name) && (!ignoreNullOrEmptyValue || val != null && !string.IsNullOrWhiteSpace(val.ToString())))
                        {
                            data.Add(name, val == null ? string.Empty : val.ToString());
                        }
                    }
                }
                else
                {
                    var name = prop.Name;
                    var val = prop.GetValue(req, null);
                    if (prop.PropertyType.IsEnum)
                    {
                        val = Convert.ChangeType(val, Enum.GetUnderlyingType(prop.PropertyType));
                    }
                    if (!data.ContainsKey(name) && (!ignoreNullOrEmptyValue || val != null && !string.IsNullOrWhiteSpace(val.ToString())))
                    {
                        data.Add(name, val == null ? string.Empty : val.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// 解析响应信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonRet"></param>
        /// <returns></returns>
        protected virtual T ParseResponse<T>(string jsonRet) where T : class, new()
        {
            if (string.IsNullOrWhiteSpace(jsonRet))
            {
                return null;
            }

            try
            {
                return jsonRet.FromJson<T>();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, string.Format($"[{Name}]解析Json时出错，Message={ex.Message}"));
                throw new ClientException(ClientErrorCode.JSON_PARSE_ERROR_CODE, ex.Message, ex);
            }
        }

        /// <summary>
        /// 获取参数字符串
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected virtual string GetParamStr(IDictionary<string, string> data)
        {
            var paramStr = new StringBuilder();

            if (data == null || data.Count == 0)
            {
                return string.Empty;
            }

            foreach (var key in data.Keys)
            {
                paramStr.Append($"{key}={data[key]}&");
            }

            return paramStr.ToString().TrimEnd('&');
        }
    }
}
