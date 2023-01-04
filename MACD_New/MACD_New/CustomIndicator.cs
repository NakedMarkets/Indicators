using IndicatorInterfaceCSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MACD_New
{
    public class MACD_New : IndicatorInterface
    {
        [Separator(Label = "Common")]
        public string Separator_Common;
        [Input(Name = "Fast EMA period")]
        public int FastEMAPeriod = 5;
        [Input(Name = "Slow EMA period")]
        public int SlowEMAPeriod = 13;
        [Input(Name = "SMA period")]
        public int SMAPeriod = 3;
        [Input(Name = "Apply to Price")]
        public Applied_Price ApplyToPrice;

        public IndicatorBuffer FastEMA = new IndicatorBuffer();
        public IndicatorBuffer SlowEMA = new IndicatorBuffer();
        public IndicatorBuffer _MACD = new IndicatorBuffer();
        public IndicatorBuffer SMA = new IndicatorBuffer();
        public IndicatorBuffer Diff = new IndicatorBuffer();

        public override void OnInit()
        {
            SetIndicatorShortName("MACD (new version)");
            Indicator_Separate_Window = true;

            SetLevel(0, Color.DarkGray, LineStyle.STYLE_DOT);
            SetIndexBuffer(0, _MACD);
            SetIndexStyle(0, DrawingStyle.DRAW_LINE, Color.Blue);
            SetIndexLabel(0, "MACD");
            SetIndexBuffer(1, SMA);
            SetIndexStyle(1, DrawingStyle.DRAW_LINE, Color.Red, LineStyle.STYLE_DOT);
            SetIndexLabel(1, "Signal Line");
            SetIndexBuffer(2, Diff);
            SetIndexStyle(2, DrawingStyle.DRAW_HISTOGRAM, Color.DarkGray);
            SetIndexLabel(2, "MACD - Signal");
        }

        public override void OnCalculate(int index)
        {
            double k, sum;

            FastEMA[index] = GetMA(Symbol(), Period(), index, 0, FastEMAPeriod, MA_Method.MODE_EMA, ApplyToPrice, FastEMA[index + 1]);
            SlowEMA[index] = GetMA(Symbol(), Period(), index, 0, SlowEMAPeriod, MA_Method.MODE_EMA, ApplyToPrice, SlowEMA[index + 1]);
            _MACD[index] = FastEMA[index] - SlowEMA[index];

            sum = 0;
            for (int i = index; i < index + SMAPeriod; i++)
                sum = sum + _MACD[i];

            SMA[index] = sum / SMAPeriod;
            Diff[index] = _MACD[index] - SMA[index];
        }

    }
}
