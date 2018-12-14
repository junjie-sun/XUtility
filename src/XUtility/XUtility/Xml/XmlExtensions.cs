// Copyright (c) junjie sun. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace XUtility.Xml
{
    /// <summary>
    /// XML扩展类
    /// </summary>
    public static class XmlExtensions
    {
        /// <summary>
        /// XML反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T FromXml<T>(this string xml)
        {
            if (string.IsNullOrEmpty(xml)) return default(T);
            try
            {
                using (MemoryStream stream = new MemoryStream(StringToUTF8ByteArray(xml)))
                using (new XmlTextWriter(stream, new UTF8Encoding(false)))
                {
                    return (T)new XmlSerializer(typeof(T)).Deserialize(stream);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("xml解析错误：" + ex.Message);
            }
        }

        /// <summary>
        /// XML反序列化
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object FromXml(this string xml, Type type)
        {
            if (string.IsNullOrEmpty(xml)) return null;
            try
            {
                using (MemoryStream stream = new MemoryStream(StringToUTF8ByteArray(xml)))
                using (new XmlTextWriter(stream, new UTF8Encoding(false)))
                {
                    return new XmlSerializer(type).Deserialize(stream);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("xml解析错误：" + ex.Message);
            }
        }

        /// <summary>
        /// XML序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToXml(this Object obj)
        {
            if (obj == null) return string.Empty;
            try
            {
                using (MemoryStream stream = new MemoryStream())
                using (XmlTextWriter xml = new XmlTextWriter(stream, new UTF8Encoding(false)))
                {
                    XmlSerializer xs = new XmlSerializer(obj.GetType());
                    xs.Serialize(xml, obj);
                    return UTF8ByteArrayToString(((MemoryStream)xml.BaseStream).ToArray());
                }
            }
            catch (Exception ex)
            {
                throw new Exception("xml解析错误：" + ex.Message);
            }
        }

        private static byte[] StringToUTF8ByteArray(string xml)
        {
            return Encoding.UTF8.GetBytes(xml);
        }

        private static string UTF8ByteArrayToString(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
