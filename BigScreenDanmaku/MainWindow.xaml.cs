using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.IO;
using System.Xml;
using StriveEngine;
using StriveEngine.Core;
using StriveEngine.Tcp.Server;
namespace BigScreenDanmaku
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private ITcpServerEngine tcpServerEngine;
        ObservableCollection<object> ObservableObj;
        DanmakuWindow danmakuwindow = new DanmakuWindow();
        SettingsWindow settingsWindow;
        Danmaku danmaku;
        public MainWindow()
        {
            InitializeComponent();
            InitialConfig();

        }

        #region tcpServerEngineEvent
        private void ServerControl_Checked(object sender, RoutedEventArgs e)
        {
            try
            {

                //CheckForIllegalCrossThreadCalls = false;
                //初始化并启动服务端引擎（TCP、文本协议）
                this.tcpServerEngine = NetworkEngineFactory.CreateTextTcpServerEngine(int.Parse(this.textBox_port.Text), new DefaultTextContractHelper("\0"));//DefaultTextContractHelper是StriveEngine内置的ITextContractHelper实现。使用UTF-8对EndToken进行编码。 
                this.tcpServerEngine.ClientCountChanged += new CbDelegate<int>(tcpServerEngine_ClientCountChanged);
                this.tcpServerEngine.ClientConnected += new CbDelegate<System.Net.IPEndPoint>(tcpServerEngine_ClientConnected);
                this.tcpServerEngine.ClientDisconnected += new CbDelegate<System.Net.IPEndPoint>(tcpServerEngine_ClientDisconnected);
                this.tcpServerEngine.MessageReceived += new CbDelegate<IPEndPoint, byte[]>(tcpServerEngine_MessageReceived);
                this.tcpServerEngine.Initialize();

                MessageBox.Show("开始监听");
                this.textBox_port.IsReadOnly = false;
                this.ServerControl.IsEnabled = false;
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        private void ServerControl_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!this.tcpServerEngine.Disposed)
            {
                tcpServerEngine.ChangeListenerState(false);
                GC.GetTotalMemory(true);
                MessageBox.Show("停止监听");
                this.textBox_port.IsReadOnly = true;
            }


        }

        void tcpServerEngine_ClientCountChanged(int count)
        {
            this.ShowConnectionCount(count);
        }

        void tcpServerEngine_ClientConnected(System.Net.IPEndPoint ipe)
        {
            if (this.CheckAccess())
            {
                textBox_Log.AppendText(DateTime.Now.ToString("hh:mm:ss") + "\t" + ipe.ToString() + "\t上线\n");
            }
            else
                this.Dispatcher.BeginInvoke(new CbDelegate<System.Net.IPEndPoint>(this.tcpServerEngine_ClientConnected), ipe);

        }

        void tcpServerEngine_ClientDisconnected(System.Net.IPEndPoint ipe)
        {
            if (this.CheckAccess())
            {
                textBox_Log.AppendText(DateTime.Now.ToString("hh:mm:ss") +"\t" + ipe.ToString() + "\t离线\n");
            }
            else
                this.Dispatcher.BeginInvoke(new CbDelegate<System.Net.IPEndPoint>(this.tcpServerEngine_ClientDisconnected), ipe);
        }

        void tcpServerEngine_MessageReceived(IPEndPoint client, byte[] bMsg)
        {
            string bundle = System.Text.Encoding.UTF8.GetString(bMsg); //消息使用UTF-8编码
            try
            {
                bundle = bundle.Substring(0, bundle.Length - 1); //将结束标记"\0"剔除
                this.ShowClientMsg(client, bundle);
            }
            catch(Exception e){
                Console.WriteLine(e.Message);
            }
        }



        private void ShowClientMsg(IPEndPoint client, string bundle)
        {
            if (this.CheckAccess())
            {
                ObservableObj = new ObservableCollection<object>();
                danmaku = new Danmaku(client, bundle);
                this.listView_ClientDanmaku.Items.Add(danmaku);
                danmakuwindow.createDanmaku(danmaku);
            }
            else
            {
                this.Dispatcher.BeginInvoke(new CbDelegate<IPEndPoint, string>(this.ShowClientMsg), client, bundle);
            }
        }

        private void ShowConnectionCount(int clientCount)
        {
            if (this.CheckAccess())
            {
                this.ClientNumb.Content = clientCount.ToString();
            }
            else
            {
                this.Dispatcher.BeginInvoke(new CbDelegate<int>(this.ShowConnectionCount), clientCount);
            }
        }

        #endregion

        #region controlEvent

        private void Window_Closed(object sender, EventArgs e)
        {
            danmakuwindow.Close();
        }

        private void MenuItem_SaveDanmaku_Click(object sender, RoutedEventArgs e)
        {
            //var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            //saveFileDialog.Title = "选择保存位置";
            //saveFileDialog.Filter = "Xml Files (*.xml)|*.xml";
            //saveFileDialog.ShowDialog();
            //Stream fileStream = saveFileDialog.OpenFile();
            //if (fileStream == null)
            //{
            //    this.textBox_Log.AppendText(DateTime.Now.ToString("hh:mm:ss") + "\t未保存成功，文件名为空！\n");
            //    return;
            //}
            //else
            //{
                

            //    }
            XmlDocument xmlDoc = new XmlDocument();
            XmlDeclaration dec = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlNode root = xmlDoc.CreateElement("DanmakuList");
            xmlDoc.AppendChild(dec);

            var saveDanmaku = listView_ClientDanmaku.Items.SourceCollection;
            foreach (Danmaku CurrentDanmaku in saveDanmaku)
            {
                XmlNode current = xmlDoc.CreateElement("Danmaku");

                XmlAttribute Ip = xmlDoc.CreateAttribute("Ip");
                Ip.InnerText = CurrentDanmaku.ip;
                XmlAttribute Time = xmlDoc.CreateAttribute("Time");
                Time.InnerText = CurrentDanmaku.time;
                XmlAttribute Color = xmlDoc.CreateAttribute("Color");
                Color.InnerText = CurrentDanmaku.color;
                XmlAttribute Location = xmlDoc.CreateAttribute("Location");
                Location.InnerText = CurrentDanmaku.location;
                current.Attributes.Append(Ip);
                current.Attributes.Append(Time);
                current.Attributes.Append(Color);
                current.Attributes.Append(Location);

                current.InnerText = CurrentDanmaku.text;
                root.AppendChild(current);
                xmlDoc.AppendChild(root);
                xmlDoc.Save("D:/danmaku.xml");
                this.textBox_Log.AppendText(DateTime.Now.ToString("hh:mm:ss") + "\t保存弹幕成功！\n");
            }
        }

        private void MenuItem_ShowDanmakuWindow_Checked(object sender, RoutedEventArgs e)
        {
            danmakuwindow.Show();
            Danmaku testdanmaku = new Danmaku();
            danmakuwindow.createDanmaku(testdanmaku);
        }

        private void MenuItem_ShowDanmakuWindow_Unchecked(object sender, RoutedEventArgs e)
        {
            danmakuwindow.Hide();
        }

        private void MenuItem_ShowSettingsWindow_Click(object sender, RoutedEventArgs e)
        {
            settingsWindow = new SettingsWindow();
            settingsWindow.Show();
        }
        #endregion

        private void InitialConfig()
        {
            if (!File.Exists("Config.dat"))
            {

                this.textBox_Log.AppendText(DateTime.Now.ToString("hh:mm:ss") + "\t配置文件不存在，正在创建...\n");
                XmlDocument xmlDoc = new XmlDocument();
                XmlDeclaration dec = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                XmlNode root = xmlDoc.CreateElement("Settings");
                xmlDoc.AppendChild(dec);

                XmlNode DANMAKU_FONTSIZE = xmlDoc.CreateElement("DANMAKU_FONTSIZE");
                DANMAKU_FONTSIZE.InnerText = "30";

                XmlNode DANMAKU_DURATION = xmlDoc.CreateElement("DANMAKU_DURATION");
                DANMAKU_DURATION.InnerText = "6000";

                XmlNode DANMAKU_OPACITY = xmlDoc.CreateElement("DANMAKU_OPACITY");
                DANMAKU_OPACITY.InnerText = "1.0";

                XmlNode DANMAKU_SHADOW = xmlDoc.CreateElement("DANMAKU_SHADOW");
                DANMAKU_SHADOW.InnerText = "True";

                XmlNode SHADOW_BLURRADIUS = xmlDoc.CreateElement("SHADOW_BLURRADIUS");
                SHADOW_BLURRADIUS.InnerText = "5";

                root.AppendChild(DANMAKU_FONTSIZE);
                root.AppendChild(DANMAKU_DURATION);
                root.AppendChild(DANMAKU_OPACITY);
                root.AppendChild(DANMAKU_SHADOW);
                root.AppendChild(SHADOW_BLURRADIUS);
                xmlDoc.AppendChild(root);

                xmlDoc.Save(@"Config.dat");


                this.textBox_Log.AppendText(DateTime.Now.ToString("hh:mm:ss") + "\t配置文件创建成功！\n");
            }
            else
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load("Config.dat");
                XmlNode root = xmlDoc.GetElementsByTagName("Settings")[0];
                XmlNodeList NodeList = root.ChildNodes;
                GlobalVariables.DANMAKU_FONTSIZE = Convert.ToInt32(NodeList[0].InnerText);
                GlobalVariables.DANMAKU_DURATION = Convert.ToInt32(NodeList[1].InnerText);
                GlobalVariables.DANMAKU_OPACITY = Convert.ToDouble(NodeList[2].InnerText);
                GlobalVariables.DANMAKU_SHADOW = NodeList[3].InnerText == "True" ? true : false;
                GlobalVariables.SHADOW_BLURRADIUS = Convert.ToInt32(NodeList[4].InnerText);
                this.textBox_Log.AppendText(DateTime.Now.ToString("hh:mm:ss") + "\t配置文件加载成功！\n");

            }
        }

        private void MenuItem_ClearDanmaku_Click(object sender, RoutedEventArgs e)
        {
            this.listView_ClientDanmaku.Items.Clear();
        }

        private void MenuItem_ClearLog_Click(object sender, RoutedEventArgs e)
        {
            this.textBox_Log.Text = "";
        }

        private void MenuItem_Game_Click(object sender, RoutedEventArgs e)
        {
            new GameWindow().Show();
        }

    }
}
