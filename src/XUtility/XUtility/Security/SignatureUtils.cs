// Copyright (c) junjie sun. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XUtility.Security
{
    /// <summary>
    /// 签名工具类
    /// </summary>
    public static class SignatureUtils
    {
        /// <summary>
        /// 获取时间戳，1970-1-1至今的秒数
        /// </summary>
        /// <returns></returns>
        public static long GetTimestamp()
        {
            DateTime theDate = DateTime.Now;
            DateTime d1 = new DateTime(1970, 1, 1).ToUniversalTime();
            DateTime d2 = theDate.ToUniversalTime();
            TimeSpan ts = d2 - d1;
            return (long)ts.TotalSeconds;
        }

        /// <summary>
        /// 生成签名
        /// 签名格式：paramKey1=paramValue分隔符paramKey2=paramValue分隔符paramKey3=paramValuePwd
        /// paramKey以升序排列
        /// </summary>
        /// <param name="signatureParams"></param>
        /// <param name="pwd"></param>
        /// <param name="splitStr"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static string GenerateSignature(IDictionary<string, string> signatureParams, string pwd = null, string splitStr = null, SignatureOptions options = null)
        {
            options = options ?? SignatureOptions.GetDefaultOptions();

            if (!string.IsNullOrWhiteSpace(pwd) && options.IsEncryptPwd)
            {
                pwd = Encrypt(pwd, options.PwdEncryptMode);
            }

            SortedDictionary<string, string> data = new SortedDictionary<string, string>();
            if (signatureParams != null)
            {
                foreach (var key in signatureParams.Keys)
                {
                    var param = signatureParams[key];
                    data.Add(key, param);
                }
            }
            var signStr = ComboString(data, pwd, splitStr);
            switch (options.Method)
            {
                case SignatureMethod.MD5:
                    return CryptographyUtils.MD5Encrypt(signStr);
                case SignatureMethod.SHA1:
                    return CryptographyUtils.SHA1Encrypt(signStr);
                default:
                    return CryptographyUtils.MD5Encrypt(signStr);
            }
        }

        /// <summary>
        /// 验证签名是否合法
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="signatureParams"></param>
        /// <param name="pwd"></param>
        /// <param name="splitStr"></param>
        /// <param name="options"></param>
        /// <param name="signatureHandler"></param>
        /// <returns></returns>
        public static bool ValidateSignature(string signature,
            IDictionary<string, string> signatureParams,
            string pwd = null,
            string splitStr = null,
            SignatureValidateOptions options = null,
            Func<string, IDictionary<string, string>, SignatureOptions, string> signatureHandler = null)
        {
            options = options ?? SignatureValidateOptions.GetDefaultOptions();

            if (options.IsCheckTimestamp)
            {
                if (signatureParams == null || !signatureParams.ContainsKey(options.TimestampParamName))
                {
                    throw new ArgumentException("未提供时间戳");
                }
                long timestamp;
                if (!long.TryParse(signatureParams[options.TimestampParamName], out timestamp))
                {
                    throw new ArgumentException("时间戳不合法");
                }

                var curTimestamp = GetTimestamp();
                var timeSpan = curTimestamp - timestamp;
                if (Math.Abs(timeSpan) > (long)options.TimestampError.TotalSeconds)
                {
                    return false;
                }
            }

            var curSignature = signatureHandler == null ? GenerateSignature(signatureParams, pwd, splitStr, options.SignatureOptions) : signatureHandler(pwd, signatureParams, options.SignatureOptions);

            if (curSignature != signature)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 将signatureParams进行组合
        /// </summary>
        /// <param name="signatureParams"></param>
        /// <param name="pwd"></param>
        /// <param name="splitStr"></param>
        /// <returns></returns>
        private static string ComboString(IDictionary<string, string> signatureParams, string pwd = null, string splitStr = null)
        {
            StringBuilder sb = new StringBuilder();

            int i = 0;
            foreach (var key in signatureParams.Keys)
            {
                var param = signatureParams[key];
                var isLast = i == signatureParams.Count - 1;
                if (string.IsNullOrEmpty(splitStr) || isLast)
                {
                    sb.Append(key + "=" + param);
                }
                else
                {
                    sb.Append(key + "=" + param + splitStr);
                }
                i++;
            }

            return string.IsNullOrWhiteSpace(pwd) ? sb.ToString() : sb.ToString() + pwd;
        }

        private static string Encrypt(string text, EncryptMode mode)
        {
            switch(mode)
            {
                case EncryptMode.MD5:
                    return CryptographyUtils.MD5Encrypt(text);
                case EncryptMode.SHA1:
                    return CryptographyUtils.SHA1Encrypt(text);
                case EncryptMode.SHA256:
                    return CryptographyUtils.SHA256Encrypt(text);
                case EncryptMode.SHA384:
                    return CryptographyUtils.SHA384Encrypt(text);
                case EncryptMode.SHA512:
                    return CryptographyUtils.SHA512Encrypt(text);
                default:
                    return text;
            }
        }
    }

    /// <summary>
    /// 签名方法
    /// </summary>
    public enum SignatureMethod
    {
        /// <summary>
        /// MD5
        /// </summary>
        MD5,

        /// <summary>
        /// SHA1
        /// </summary>
        SHA1
    }

    /// <summary>
    /// 加密模式
    /// </summary>
    public enum EncryptMode
    {
        /// <summary>
        /// MD5
        /// </summary>
        MD5,

        /// <summary>
        /// SHA1
        /// </summary>
        SHA1,

        /// <summary>
        /// SHA256
        /// </summary>
        SHA256,

        /// <summary>
        /// SHA384
        /// </summary>
        SHA384,

        /// <summary>
        /// SHA384
        /// </summary>
        SHA512
    }

    /// <summary>
    /// 签名Options
    /// </summary>
    public class SignatureOptions
    {
        /// <summary>
        /// 签名方法
        /// </summary>
        public SignatureMethod Method { get; set; }

        /// <summary>
        /// 是否对Pwd进行加密
        /// </summary>
        public bool IsEncryptPwd { get; set; }

        /// <summary>
        /// isEncryptPwd=true时有效，Pwd的加密方式
        /// </summary>
        public EncryptMode PwdEncryptMode { get; set; }

        /// <summary>
        /// 获取默认Options
        /// </summary>
        /// <returns></returns>
        public static SignatureOptions GetDefaultOptions()
        {
            return new SignatureOptions()
            {
                Method = SignatureMethod.MD5,
                IsEncryptPwd = false,
                PwdEncryptMode = EncryptMode.SHA1
            };
        }
    }

    /// <summary>
    /// 签名验证Options
    /// </summary>
    public class SignatureValidateOptions
    {
        /// <summary>
        /// 是否检查时间戳，单位：秒
        /// </summary>
        public bool IsCheckTimestamp { get; set; }

        /// <summary>
        /// 当IsCheckTimestamp=true时有效，时间戳所在的参数名
        /// </summary>
        public string TimestampParamName { get; set; }

        /// <summary>
        /// 当IsCheckTimestamp=true时有效，时间戳与服务器时间允许的最大误差值
        /// </summary>
        public TimeSpan TimestampError { get; set; }

        /// <summary>
        /// 签名选项
        /// </summary>
        public SignatureOptions SignatureOptions { get; set; }

        /// <summary>
        /// 获取默认Options
        /// </summary>
        /// <returns></returns>
        public static SignatureValidateOptions GetDefaultOptions()
        {
            return new SignatureValidateOptions()
            {
                SignatureOptions = SignatureOptions.GetDefaultOptions(),
                IsCheckTimestamp = false,
                TimestampError = new TimeSpan(0, 5, 0),
                TimestampParamName = "timestamp"
            };
        }
    }
}
