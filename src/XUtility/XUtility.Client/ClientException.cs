using System;
using System.Collections.Generic;
using System.Text;

namespace XUtility.Client
{
    /// <summary>
    /// Client异常
    /// </summary>
    public class ClientException : Exception
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ClientException(int code, string message, Exception innerException) : base(message, innerException)
        {
            Code = code;
        }

        /// <summary>
        /// 错误码
        /// </summary>
        public int Code { get; }
    }
}
