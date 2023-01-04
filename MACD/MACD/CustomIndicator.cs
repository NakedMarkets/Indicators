using IndicatorInterfaceCSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MACD
{
    public class MACD : IndicatorInterface
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

        public override void OnInit()
        {
            SetIndicatorShortName("MACD");
            Indicator_Separate_Window = true;

            SetLevel(0, Color.DarkGray, LineStyle.STYLE_DOT);
            SetIndexBuffer(0, _MACD);
            SetIndexStyle(0, DrawingStyle.DRAW_HISTOGRAM, Color.DarkGray);
            SetIndexLabel(0, "MACD");
            SetIndexBuffer(1, SMA);
            SetIndexStyle(1, DrawingStyle.DRAW_LINE, Color.Red, LineStyle.STYLE_DOT);
            SetIndexLabel(1, "Signal Line");
        }

        public override void OnCalculate(int index)
        {
            double k, sum;

            // calculate fast EMA
            k = (double)2 / (FastEMAPeriod + 1);

            if (index == Bars() - 1)
                FastEMA[index] = GetAppliedPrice(Symbol(), Period(), index, ApplyToPrice);
            else
                FastEMA[index] = FastEMA[index + 1] + k * (GetAppliedPrice(Symbol(), Period(), index, ApplyToPrice) - FastEMA[index + 1]);

            // calculate slow EMA
            k = (double)2 / (SlowEMAPeriod + 1);

            if (index == Bars() - 1)
                SlowEMA[index] = GetAppliedPrice(Symbol(), Period(), index, ApplyToPrice);
            else
                SlowEMA[index] = SlowEMA[index + 1] + k * (GetAppliedPrice(Symbol(), Period(), index, ApplyToPrice) - SlowEMA[index + 1]);

            // calculate MACD
            _MACD[index] = FastEMA[index] - SlowEMA[index];

            // calculate SMA
            sum = 0;
            for (int i = index; i < index + SMAPeriod; i++)
                sum = sum + _MACD[i];

            SMA[index] = sum / SMAPeriod;
        }

    }
}
