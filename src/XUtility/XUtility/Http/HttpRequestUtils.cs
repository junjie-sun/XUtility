// Copyright (c) junjie sun. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace XUtility.Http
{
    /// <summary>
    /// Http请求工具类
    /// </summary>
    public static class HttpRequestUtils
    {
        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="paramList"></param>
        /// <param name="options"></param>
        /// <param name="clientSetting"></param>
        /// <returns></returns>
        public async static Task<string> GetAsync(string url, IDictionary<string, string> paramList = null, HttpGetOptions options = null, Action<HttpClient> clientSetting = null)
        {
            options = options ?? new HttpGetOptions();

            using (var client = CreateHttpClient(options.Accept, options.AcceptCharset))
            {
                clientSetting?.Invoke(client);

                url = AppendParams(url, paramList);

                var response = await client.GetAsync(url);

                response.EnsureSuccessStatusCode();

                return options.Encoding.GetString(await response.Content.ReadAsByteArrayAsync());
            }
        }

        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <param name="paramList"></param>
        /// <param name="options"></param>
        /// <param name="clientSetting"></param>
        /// <returns></returns>
        public async static Task<string> PostAsync(string url, string body, IDictionary<string, string> paramList = null, HttpPostOptions options = null, Action<HttpClient> clientSetting = null)
        {
            options = options ?? new HttpPostOptions();

            using (var client = CreateHttpClient(options.Accept, options.AcceptCharset))
            {
                clientSetting?.Invoke(client);

                url = AppendParams(url, paramList);

                StringContent content = null;
                if (body != null)
                {
                    content = new StringContent(body, options.Encoding, options.ContentType);
                }

                var response = await client.PostAsync(url, content);

                response.EnsureSuccessStatusCode();

                return options.Encoding.GetString(await response.Content.ReadAsByteArrayAsync());
            }
        }

        /// <summary>
        /// PostForm请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="form"></param>
        /// <param name="paramList"></param>
        /// <param name="options"></param>
        /// <param name="clientSetting"></param>
        /// <returns></returns>
        public async static Task<string> PostFormAsync(string url, IDictionary<string, string> form, IDictionary<string, string> paramList = null, HttpPostOptions options = null, Action<HttpClient> clientSetting = null)
        {
            options = options ?? new HttpPostOptions();

            using (var client = CreateHttpClient(options.Accept, options.AcceptCharset))
            {
                clientSetting?.Invoke(client);

                url = AppendParams(url, paramList);

                var content = new FormUrlEncodedContent(form);

                var response = await client.PostAsync(url, content);

                response.EnsureSuccessStatusCode();

                return options.Encoding.GetString(await response.Content.ReadAsByteArrayAsync());
            }
        }

        /// <summary>
        /// PostMultipartForm请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="form"></param>
        /// <param name="filePaths"></param>
        /// <param name="fileFormName"></param>
        /// <param name="paramList"></param>
        /// <param name="options"></param>
        /// <param name="clientSetting"></param>
        /// <returns></returns>
        public async static Task<string> PostMultipartFormAsync(string url, IDictionary<string, string> form, IList<string> filePaths, string fileFormName, IDictionary<string, string> paramList = null, HttpPostOptions options = null, Action<HttpClient> clientSetting = null)
        {
            options = options ?? new HttpPostOptions();

            using (var client = CreateHttpClient(options.Accept, options.AcceptCharset))
            {
                clientSetting?.Invoke(client);

                url = AppendParams(url, paramList);

                using (var content = new MultipartFormDataContent())
                {
                    if (form != null)
                    {
                        foreach (var key in form.Keys)
                        {
                            content.Add(new StringContent(form[key]), key);
                        }
                    }

                    if (filePaths != null)
                    {
                        foreach (var path in filePaths)
                        {
                            var file = Path.GetFileName(path);

                            FileStream stream = File.OpenRead(path);
                            StreamContent streamContent = new StreamContent(stream);
                            streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
                            streamContent.Headers.ContentDisposition.Name = fileFormName ?? "files";
                            streamContent.Headers.ContentDisposition.FileName = file;
                            streamContent.Headers.ContentLength = stream.Length;
                            streamContent.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(file));
                            content.Add(streamContent);
                        }
                    }

                    var response = await client.PostAsync(url, content);

                    response.EnsureSuccessStatusCode();

                    return options.Encoding.GetString(await response.Content.ReadAsByteArrayAsync());
                }
            }
        }

        /// <summary>
        /// 以body为载体上传文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="filePath"></param>
        /// <param name="paramList"></param>
        /// <param name="options"></param>
        /// <param name="clientSetting"></param>
        /// <returns></returns>
        public async static Task<string> PostFileAsync(string url, string filePath, IDictionary<string, string> paramList = null, HttpPostOptions options = null, Action<HttpClient> clientSetting = null)
        {
            options = options ?? new HttpPostOptions();

            var file = Path.GetFileName(filePath);

            using (var client = CreateHttpClient(options.Accept, options.AcceptCharset))
            {
                clientSetting?.Invoke(client);

                url = AppendParams(url, paramList);

                FileStream stream = File.OpenRead(filePath);
                StreamContent streamContent = new StreamContent(stream);
                streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                streamContent.Headers.ContentDisposition.FileName = file;
                streamContent.Headers.ContentLength = stream.Length;
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(file));

                var response = await client.PostAsync(url, streamContent);

                response.EnsureSuccessStatusCode();

                return options.Encoding.GetString(await response.Content.ReadAsByteArrayAsync());

                #region
                //foreach (var path in filePaths)
                //{
                //    var fileContent = new ByteArrayContent(File.ReadAllBytes(path));
                //    fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                //    {
                //        FileName = Path.GetFileName(path),
                //        Name = "files"
                //    };
                //    content.Add(fileContent);
                //}
                #endregion
            }
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="savePath"></param>
        /// <param name="paramList"></param>
        /// <param name="saveFileName"></param>
        /// <param name="clientSetting"></param>
        /// <returns></returns>
        public async static Task<string> DownloadAsync(string url, string savePath, IDictionary<string, string> paramList = null, string saveFileName = null, Action<HttpClient> clientSetting = null)
        {
            string fileName = null;

            using (var client = new HttpClient())
            {
                clientSetting?.Invoke(client);

                url = AppendParams(url, paramList);

                var response = await client.GetAsync(url);

                response.EnsureSuccessStatusCode();

                fileName = response.Content.Headers.ContentDisposition.FileName;

                if (!string.IsNullOrEmpty(saveFileName))
                {
                    var ext = Path.GetExtension(saveFileName);
                    if (string.IsNullOrEmpty(ext))
                    {
                        fileName = saveFileName + Path.GetExtension(fileName);
                    }
                    else
                    {
                        fileName = saveFileName;
                    }
                }

                var filePath = savePath.TrimEnd('/').TrimEnd('\\') + "/" + fileName;

                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    using (var fs = new FileStream(filePath, FileMode.Create))
                    {
                        await stream.CopyToAsync(fs);
                    }
                }
            }

            return fileName;
        }

        /// <summary>
        /// 将参数添加到url的QueryString中
        /// </summary>
        /// <param name="url"></param>
        /// <param name="paramList"></param>
        /// <returns></returns>
        public static string AppendParams(string url, IDictionary<string, string> paramList)
        {
            if (string.IsNullOrEmpty(url))
            {
                return string.Empty;
            }

            if (paramList == null || paramList.Count == 0)
            {
                return url;
            }

            StringBuilder sb = new StringBuilder();

            sb.Append(url);
            if (url.Contains("?"))
            {
                sb.Append("&");
            }
            else
            {
                sb.Append("?");
            }

            foreach (var key in paramList.Keys)
            {
                if (paramList[key] == null)
                {
                    continue;
                }

                sb.Append(key);
                sb.Append("=");
                sb.Append(Uri.EscapeDataString(paramList[key]));
                sb.Append("&");
            }

            return sb.ToString().TrimEnd('&');
        }

        private static HttpClient CreateHttpClient(string accept, string acceptCharset)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(accept));
            client.DefaultRequestHeaders.AcceptCharset.Add(new StringWithQualityHeaderValue(acceptCharset));

            return client;
        }
    }

    /// <summary>
    /// HttpGet方法参数
    /// </summary>
    public class HttpGetOptions
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public HttpGetOptions()
        {
            Accept = "application/json";
            AcceptCharset = "utf-8";
            Encoding = Encoding.UTF8;
        }

        /// <summary>
        /// Accept
        /// </summary>
        public string Accept { get; set; }

        /// <summary>
        /// AcceptCharset
        /// </summary>
        public string AcceptCharset { get; set; }

        /// <summary>
        /// Encoding
        /// </summary>
        public Encoding Encoding { get; set; }
    }

    /// <summary>
    /// HttpPost方法参数
    /// </summary>
    public class HttpPostOptions
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public HttpPostOptions()
        {
            Accept = "application/json";
            AcceptCharset = "utf-8";
            Encoding = Encoding.UTF8;
            ContentType = "application/json";
        }

        /// <summary>
        /// Accept
        /// </summary>
        public string Accept { get; set; }

        /// <summary>
        /// AcceptCharset
        /// </summary>
        public string AcceptCharset { get; set; }

        /// <summary>
        /// Encoding
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// ContentType
        /// </summary>
        public string ContentType { get; set; }
    }
}
