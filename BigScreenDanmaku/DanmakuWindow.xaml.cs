using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Collections;

namespace BigScreenDanmaku
{

    /// <summary>
    /// DanmakuWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DanmakuWindow : Window
    {
        Random ra = new Random();
        private int i=0;
        //prevent Cover


        public DanmakuWindow()
        {
            InitializeComponent();
            GlobalVariables._maxRow = (int)(GlobalVariables.ScreeHeight / 30);
            GlobalVariables._rowList = new Boolean[GlobalVariables._maxRow - 3];
        }

        #region Danmaku

        public void createDanmaku(Danmaku _danmaku) 
        {
            TextBlock _singleDanmaku = new TextBlock();

            _singleDanmaku.Text = _danmaku.text;
            _singleDanmaku.FontFamily = (FontFamily)new FontFamilyConverter().ConvertFromString("Microsoft YaHei");
            _singleDanmaku.FontSize = GlobalVariables.DANMAKU_FONTSIZE;
            _singleDanmaku.FontWeight = FontWeights.Bold;        
            _singleDanmaku.Opacity = GlobalVariables.DANMAKU_OPACITY;

            int targetRow = getAvailableRow();
            double rowHeight = GlobalVariables.DANMAKU_FONTSIZE + 5;
            _singleDanmaku.SetValue(Canvas.TopProperty, (double)targetRow * rowHeight);
            //颜色
            _singleDanmaku.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString((string)_danmaku.color));

            //阴影
            if (GlobalVariables.DANMAKU_SHADOW ==true)
            {
                DropShadowEffect _ef = new DropShadowEffect();

                _ef.RenderingBias = RenderingBias.Performance;
                _ef.Opacity = (double)100;
                _ef.ShadowDepth = (double)0;
                _ef.BlurRadius = GlobalVariables.SHADOW_BLURRADIUS;

                if (_singleDanmaku.Foreground == new SolidColorBrush(Color.FromRgb(0,0,0)))
                {
                    _ef.Color = Color.FromRgb(255, 255, 255);
                }
                else
                {
                    _ef.Color = Color.FromRgb(0, 0, 0);
                }

                _singleDanmaku.Effect = _ef;
            }

            _singleDanmaku.Loaded += delegate(object o, RoutedEventArgs e) { doAnimation(_singleDanmaku, GlobalVariables.DANMAKU_DURATION, targetRow); };

            danmakuRender.Children.Add(_singleDanmaku);

            lockRow(targetRow);
        }

        private void doAnimation(TextBlock _singleDanmaku, int _duration, int _row)
        {
            TextBlock _targetDanmaku = _singleDanmaku;

            double _danmakuWidth = _targetDanmaku.ActualWidth;
            DoubleAnimation _doubleAnimation = new DoubleAnimation(GlobalVariables.ScreeWidth, -_danmakuWidth, new Duration(TimeSpan.FromMilliseconds(_duration)), FillBehavior.Stop);
            
            _doubleAnimation.Completed += delegate(object o, EventArgs e) { removeOutdateDanmaku(_singleDanmaku, _row); };
            _targetDanmaku.BeginAnimation(Canvas.LeftProperty, _doubleAnimation);
            //Storyboard _sb = new Storyboard();
            //Storyboard.SetTarget(_doubleAnimation, _targetDanmaku);
            //Storyboard.SetTargetProperty(_doubleAnimation, new PropertyPath("(Canvas.Left)"));

            //_sb.Completed += delegate(object o, EventArgs e) { removeOutdateDanmaku(_singleDanmaku, _row); };

            //_sb.Children.Add(_doubleAnimation);
            //_sb.Begin();

        }

        private void removeOutdateDanmaku(TextBlock _singleDanmaku, int _row)
        {
            TextBlock ready2remove = _singleDanmaku;
            if (ready2remove != null)
            {
                danmakuRender.Children.Remove(ready2remove);
                ready2remove = null;


                unlockRow(_row);
            }
            else
            {
                Console.WriteLine("Remove Danmaku Error.");
            }
        }

        private int getAvailableRow()
        {

            int i = 0;
            int j = 0;
            foreach (bool a in GlobalVariables._rowList)
            {
                if (a == false)
                {
                    i++;
                }
            }
            if (i==0)
            {
                unlockRow();             
                //int ret = ra.Next(0, _maxRow - 1);
                //return ret;
                //debug
                Console.WriteLine("All Rows Full,unlock all rows.");

            }

            foreach (bool a in GlobalVariables._rowList)
            {
                    if (a == false)
                {
                    break;
                }
                    j++;
            }
            return j;     
        }

        private void lockRow(int _row)
        {
            GlobalVariables._rowList[_row] = true;
        }

        private void unlockRow(int _row = -1)
        {
            if (_row == -1)
            {
                //for (int i = 0; i <= _rowList.Length - 1; i++)
                //{
                //    _rowList[i] = false;
                //}
                //重置所有行
                GlobalVariables._rowList = new bool[GlobalVariables._maxRow - 1];
            }
            else
            {
                if (!(_row > GlobalVariables._rowList.Length - 1))
                {
                    GlobalVariables._rowList[_row] = false;
                }
            }
        }

        
        #endregion

        private void Window_Deactivated(object sender, EventArgs e)
        {
            this.Topmost = true;
        }


    }


}


