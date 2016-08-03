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
using System.Collections;


namespace BigScreenDanmaku
{
    /// <summary>
    /// SettingsWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public static int temp_DANMAKU_FONTSIZE;
        public static int temp_DANMAKU_DURATION;
        public static double temp_DANMAKU_OPACITY;
        public static bool temp_DANMAKU_SHADOW;
        public static int temp_SHADOW_BLURRADIUS;
        public SettingsWindow()
        {
            temp_DANMAKU_FONTSIZE = GlobalVariables.DANMAKU_FONTSIZE;
            temp_DANMAKU_DURATION = GlobalVariables.DANMAKU_DURATION;
            temp_DANMAKU_OPACITY = GlobalVariables.DANMAKU_OPACITY;
            temp_DANMAKU_SHADOW = GlobalVariables.DANMAKU_SHADOW;
            temp_SHADOW_BLURRADIUS = GlobalVariables.SHADOW_BLURRADIUS;
            InitializeComponent();
            Initial();
        }

        private void Initial()
        {
            for (int i = 15; i <= 40; i++)
            {
                this.ComboBox_FontSize.Items.Add(i);
            }
            this.ComboBox_FontSize.SelectedItem = temp_DANMAKU_FONTSIZE;
            this.Slider_DanmakuDuration.Value = temp_DANMAKU_DURATION;
            this.Slider_DanmakuOpacity.Value = temp_DANMAKU_OPACITY;
            this.CheckBox_DanmakuShadow.IsChecked = temp_DANMAKU_SHADOW;
            this.Slider_DanmakuShadowBlurRadius.Value = temp_SHADOW_BLURRADIUS;
        }

        private void SaveConfig()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("Config.dat");
            XmlNode root = xmlDoc.GetElementsByTagName("Settings")[0];
            XmlNodeList NodeList = root.ChildNodes;
            NodeList[0].InnerText = GlobalVariables.DANMAKU_FONTSIZE.ToString();
            NodeList[1].InnerText = GlobalVariables.DANMAKU_DURATION.ToString();
            NodeList[2].InnerText = GlobalVariables.DANMAKU_OPACITY.ToString();
            NodeList[3].InnerText = GlobalVariables.DANMAKU_SHADOW.ToString();
            NodeList[4].InnerText = GlobalVariables.SHADOW_BLURRADIUS.ToString();
            xmlDoc.Save("Config.dat");
        }

        private void ResetRowList(Danmaku _danmaku)
        {
            int _maxRowTemp = (int)(GlobalVariables.ScreeHeight / GlobalVariables.DANMAKU_FONTSIZE) - 3;
            GlobalVariables._rowListArray = new ArrayList(GlobalVariables._rowList);
            if (_maxRowTemp >= GlobalVariables._maxRow)
            {
                for (int i = 0; i < _maxRowTemp - GlobalVariables._maxRow; i++)
                {
                    GlobalVariables._rowListArray.Add(false);
                }
                GlobalVariables._rowList = (Boolean[])GlobalVariables._rowListArray.ToArray(typeof(Boolean));
            }
            else
            {
                for (int i = 0; i < GlobalVariables._maxRow - _maxRowTemp; i++)
                {
                    GlobalVariables._rowListArray.RemoveAt(GlobalVariables._rowListArray.Count - 1);
                }
                GlobalVariables._rowList = (Boolean[])GlobalVariables._rowListArray.ToArray(typeof(Boolean));
            }
        }
        #region Event
        private void ComboBox_FontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GlobalVariables.DANMAKU_FONTSIZE = (int)this.ComboBox_FontSize.SelectedValue;
            ResetRowList(new Danmaku());
        }

        private void Slider_DanmakuDuration_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            GlobalVariables.DANMAKU_DURATION = (int)this.Slider_DanmakuDuration.Value;
        }

        private void Slider_DanmakuOpacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            GlobalVariables.DANMAKU_OPACITY = this.Slider_DanmakuOpacity.Value;
        }

        private void CheckBox_DanmakuShadow_Checked(object sender, RoutedEventArgs e)
        {
            GlobalVariables.DANMAKU_SHADOW = true;
        }

        private void CheckBox_DanmakuShadow_Unchecked(object sender, RoutedEventArgs e)
        {
            GlobalVariables.DANMAKU_SHADOW = false;
        }

        private void Slider_DanmakuShadowBlurRadius_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double>e)
        {
            GlobalVariables.SHADOW_BLURRADIUS = (int)this.Slider_DanmakuShadowBlurRadius.Value;
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            SaveConfig();
            this.Close();
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            GlobalVariables.DANMAKU_FONTSIZE = temp_DANMAKU_FONTSIZE;
            GlobalVariables.DANMAKU_DURATION = temp_DANMAKU_DURATION;
            GlobalVariables.DANMAKU_OPACITY = temp_DANMAKU_OPACITY;
            GlobalVariables.DANMAKU_SHADOW = temp_DANMAKU_SHADOW;
            GlobalVariables.SHADOW_BLURRADIUS = temp_SHADOW_BLURRADIUS;
            this.Close();
        }
        #endregion



    }
}
