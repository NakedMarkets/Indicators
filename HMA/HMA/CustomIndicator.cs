using IndicatorInterfaceCSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HMA
{
    public class HMA : IndicatorInterface
    {
        [Separator(Label = "Common")]
        public string Separator_Common;
        [Input(Name = "Period")]
        public int period = 21;

        public IndicatorBuffer WMA1 = new IndicatorBuffer();
        public IndicatorBuffer WMA2 = new IndicatorBuffer();
        public IndicatorBuffer WMADiff = new IndicatorBuffer();
        public IndicatorBuffer HMA1 = new IndicatorBuffer();
        public IndicatorBuffer HMA2 = new IndicatorBuffer();
        public override void OnInit()
        {
            SetIndicatorShortName("Hull Moving Average");
            Indicator_Separate_Window = false;
            SetIndexBuffer(0, HMA1);
            SetIndexStyle(0, DrawingStyle.DRAW_LINE, Color.Blue);
            SetIndexLabel(0, "HMA up");
            SetIndexBuffer(1, HMA2);
            SetIndexStyle(1, DrawingStyle.DRAW_LINE, Color.Red);
            SetIndexLabel(1, "HMA down");
        }

        public override void OnCalculate(int index)
        {
            int HalfPeriod, SqrtPeriod, weight;
            double sum, NewValue, PreviousValue;            

            if (index + period >= Bars())
                return;

            HalfPeriod = (int)Math.Truncate((double)period / 2);
            SqrtPeriod = (int)Math.Truncate(Math.Sqrt(period));

            WMA1[index] = GetWMA(HalfPeriod, index) * 2;
            WMA2[index] = GetWMA(period, index);
            WMADiff[index] = WMA1[index] - WMA2[index];

            sum = 0;
            weight = 0;
            for(int i = 0; i < SqrtPeriod; i++)
            {
                sum = sum + WMADiff[index + i] * (SqrtPeriod - i);
                weight = weight + (SqrtPeriod - i);
            }

            NewValue = sum / weight;

            if(HMA1[index + 1] != 0)
                PreviousValue = HMA1[index + 1];
            else
                PreviousValue = HMA2[index + 1];

            if (NewValue >= PreviousValue)
                HMA1[index] = NewValue;
            else
                HMA2[index] = NewValue;

        }

        public double GetWMA(int period, int index)
        {
            double sum = 0;
            int weight = 0;
            for(int i = 0; i < period; i++)
            {
                sum = sum + Close(index + i) * (period - i);
                weight = weight + (period - i);
            }
            return sum / weight;
        }

    }
}
