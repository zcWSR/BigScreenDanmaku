using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows;

namespace BigScreenDanmaku
{
    class GlobalVariables
    {
        public static int DANMAKU_FONTSIZE = 30;
        public static int DANMAKU_DURATION = 6000;
        public static double DANMAKU_OPACITY = 1.0;
        public static bool DANMAKU_SHADOW = true;
        public static int SHADOW_BLURRADIUS = 5;


        public static int _maxRow;
        public static bool[] _rowList;
        public static ArrayList _rowListArray;
        public static double ScreeHeight = SystemParameters.FullPrimaryScreenHeight;
        public static double ScreeWidth = SystemParameters.FullPrimaryScreenWidth;
    }
}
