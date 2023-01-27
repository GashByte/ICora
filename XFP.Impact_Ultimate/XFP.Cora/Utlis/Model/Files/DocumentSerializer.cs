using System.Xml.Serialization;

namespace XFP.ICora.Utlis.Model.Files
{
    public class DocumentSerializer
    {
        /// <summary>
        /// 序列化数据
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="t">对象</param>
        /// <returns></returns>
        public string Serializer<T>(T t)
        {
            using (StringWriter sw = new())
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(sw, t);
                return sw.ToString();
            }
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <param name="document">序列化后的数据</param>
        /// <returns></returns>
        public object Deserialize(Type type, string document)
        {
            using (StringReader sr = new(document))
            {
                XmlSerializer serializer = new XmlSerializer(type);
                return serializer.Deserialize(sr);
            }
        }
    }
}
