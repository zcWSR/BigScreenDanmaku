using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;

namespace BigScreenDanmaku
{
    /// <summary>
    /// GameWindow.xaml 的交互逻辑
    /// </summary>
    public partial class GameWindow : Window
    {
        public GameWindow()
        {
            InitializeComponent();
            random();
        }

        private void random()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("D:/danmaku.xml");
            XmlNode root = xmlDoc.GetElementsByTagName("DanmakuList")[0];
            XmlNodeList NodeList = root.ChildNodes;
            int i=-1;
            foreach(XmlNode node in NodeList){
                i++;
            }
            Random ran=new Random();
            for (int a = 0; a < 10; a++)
            {
                int RandKey = ran.Next(0, i);
                TextBox_log.AppendText(NodeList[RandKey].InnerText.ToString()+"\n");
            }
        }
    }
}
