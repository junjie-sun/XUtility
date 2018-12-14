// Copyright (c) junjie sun. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MessagePack;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XUtility.Web.Extensions
{
    /// <summary>
    /// Session扩展类
    /// </summary>
    public static class SessionExtensions
    {
        /// <summary>
        /// 设置Session值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="session"></param>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        public static void Set<T>(this ISession session, string key, T obj)
        {
            byte[] buf = Serialize(obj);
            session.Set(key, buf);
        }

        /// <summary>
        /// 设置Session值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="session"></param>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async static Task SetAsync<T>(this ISession session, string key, T obj)
        {
            byte[] buf = await SerializeAsync(obj);
            session.Set(key, buf);
        }

        /// <summary>
        /// 获取Session值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="session"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Get<T>(this ISession session, string key)
        {
            var buf = session.Get(key);
            if (buf == null)
            {
                return default(T);
            }

            return Deserialize<T>(buf);
        }

        /// <summary>
        /// 获取Session值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="session"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public async static Task<T> GetAsync<T>(this ISession session, string key)
        {
            var buf = session.Get(key);
            if (buf == null)
            {
                return default(T);
            }

            return await DeserializeAsync<T>(buf);
        }

        private static byte[] Serialize<T>(T obj)
        {
            return MessagePackSerializer.Serialize(obj);
        }

        private static T Deserialize<T>(byte[] buf)
        {
            return MessagePackSerializer.Deserialize<T>(buf);
        }

        private async static Task<byte[]> SerializeAsync<T>(T obj)
        {
            byte[] buf = null;

            using (var stream = new System.IO.MemoryStream())
            {
                await MessagePackSerializer.SerializeAsync(stream, obj);
                buf = stream.ToArray();
            }

            return buf;
        }

        private static Task<T> DeserializeAsync<T>(byte[] buf)
        {
            using (var stream = new System.IO.MemoryStream(buf))
            {
                return MessagePackSerializer.DeserializeAsync<T>(stream);
            }
        }
    }
}
