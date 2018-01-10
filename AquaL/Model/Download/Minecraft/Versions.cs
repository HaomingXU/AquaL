using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace AquaL.Model.Download.Minecraft
{
    /// <summary>
    /// Minecraft 版本获取
    /// </summary>
    class Versions
    {
        /// <summary>
        /// 获取全部版本
        /// </summary>
        /// <param name="api">API前置地址，例如http://launchermeta.mojang.com/</param>
        /// <returns>反序列化后的Json</returns>
        public static VersionsModel GetAll(string api)
        {
            WebClient wc = new WebClient();
            wc.Credentials = CredentialCache.DefaultCredentials;
            Encoding enc = Encoding.GetEncoding("UTF-8");
            Byte[] pageData = wc.DownloadData(api + "//mc//game//version_manifest.json");
            string json = enc.GetString(pageData);
            VersionsModel model;
            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(VersionsModel));
                model = (VersionsModel)deseralizer.ReadObject(ms);// //反序列化ReadObject
            }
            return model;
        }
    }

    /// <summary>
    /// 服务器返回内容 反序列化用模具
    /// </summary>
    [DataContract]
    class VersionsModel
    {
        /// <summary>
        /// 包含两个string（最新正式版和测试版的版本名）
        /// </summary>
        [DataMember]
        public VersionsLatest latest { get; set; }
        /// <summary>
        /// 全部版本的数组
        /// </summary>
        [DataMember]
        public VersionsInfo[] versions { get; set; }
    }
    /// <summary>
    /// 最新版本的反序列化
    /// </summary>
    [DataContract]
    class VersionsLatest
    {
        /// <summary>
        /// 最新测试版版本名
        /// </summary>
        [DataMember]
        public string snapshot { get; set; }
        /// <summary>
        /// 最新正式版版本名
        /// </summary>
        [DataMember]
        public string release { get; set; }
    }
    /// <summary>
    /// 版本信息
    /// </summary>
    [DataContract]
    class VersionsInfo
    {
        /// <summary>
        /// 版本名，如1.12.1，17w47a
        /// </summary>
        [DataMember]
        public string id { get; set; }
        /// <summary>
        /// 版本类型，如snapshot、release
        /// </summary>
        [DataMember]
        public string type { get; set; }
        /// <summary>
        /// 编译时间？
        /// </summary>
        [DataMember]
        public string time { get; set; }
        /// <summary>
        /// 发布时间
        /// </summary>
        [DataMember]
        public string releaseTime { get; set; }
        /// <summary>
        /// 下载地址
        /// </summary>
        [DataMember]
        public string url { get; set; }

    }
}
