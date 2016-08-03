using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using StriveEngine;
using StriveEngine.Core;
using StriveEngine.Tcp.Server;
using System.Xml;
using System.Windows.Media;
namespace BigScreenDanmaku
{
    
    public class Danmaku
    {
        XmlDocument Doc;
        public Danmaku(String Text = "欢迎使用，这是一条测试弹幕" )
        {
            this.time = DateTime.Now.ToString("hh:mm:ss");
            this.ip = "172.0.0.1";
            this.text = Text;
            this.color = "#FFFFFF";
            this.location = "1";
        }
        public Danmaku(IPEndPoint ip, string bundle)
        {
            List<object> all=analyseDanmaku(bundle);
            this.time = DateTime.Now.ToString("hh:mm:ss");
            this.ip = ip.ToString();
            this.text = (String)all[0];
            this.color = (String)all[1];
            this.location = (String)all[2];
        }
        //弹幕发送时间
        public String time { get; set; }
        //来源ip
        public String ip { get; set; }
        //弹幕内容
        public String text { get; set; }
        //弹幕颜色
        public String color { get; set; }
        //弹幕位置：从左，从右
        public String location { get; set; }

        public List<object> analyseDanmaku(String msg)
        {
            Doc = new XmlDocument();
             XmlNode node = null;
             String danmaku = "";
             String color = "#FFFFFF";
             String location = "1";
            try
            {
                Doc.LoadXml("<load>" + msg + "</load>");
                node= Doc.GetElementsByTagName("text")[0];
                danmaku = node.InnerText;
                color = node.Attributes["color"].Value;
                location = node.Attributes["location"].Value;
            }
            catch (Exception e)
            {
                return new List<object> { danmaku, color, location };
            }

            return new List<object>{danmaku,color,location};
        }
    }
}
