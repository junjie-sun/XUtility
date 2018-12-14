// Copyright (c) junjie sun. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using MessagePack;
using Microsoft.Extensions.Caching.Distributed;

namespace XUtility.Cache
{
    /// <summary>
    /// Cache扩展类
    /// </summary>
    public static class CacheExtentions
    {
        /// <summary>
        /// 设置缓存值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="options"></param>
        public static void Set<T>(this IDistributedCache cache, string key, T obj, DistributedCacheEntryOptions options = null)
        {
            byte[] buf = Serialize(obj);
            if (options == null)
            {
                options = new DistributedCacheEntryOptions();
            }
            cache.Set(key, buf, options);
        }

        /// <summary>
        /// 设置缓存值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async static Task SetAsync<T>(this IDistributedCache cache, string key, T obj, DistributedCacheEntryOptions options = null)
        {
            byte[] buf = await SerializeAsync(obj);
            if (options == null)
            {
                options = new DistributedCacheEntryOptions();
            }
            await cache.SetAsync(key, buf, options);
        }

        /// <summary>
        /// 获取缓存值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Get<T>(this IDistributedCache cache, string key)
        {
            var buf = cache.Get(key);
            if (buf == null)
            {
                return default(T);
            }

            return Deserialize<T>(buf);
        }

        /// <summary>
        /// 获取缓存值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public async static Task<T> GetAsync<T>(this IDistributedCache cache, string key)
        {
            var buf = await cache.GetAsync(key);
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
