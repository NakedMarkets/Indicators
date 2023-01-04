using IndicatorInterfaceCSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ATR
{
    public class ATR : IndicatorInterface
    {
        [Separator(Label = "Common")]
        public string Separator_Common;
        [Input(Name = "Period")]
        public int period = 14;

        public IndicatorBuffer ATRBuffer = new IndicatorBuffer();
        public IndicatorBuffer TempBuffer = new IndicatorBuffer();

        public override void OnInit()
        {
            SetIndicatorShortName("Average True Range");
            Indicator_Separate_Window = true;

            SetIndexBuffer(0, ATRBuffer);
            SetIndexStyle(0, DrawingStyle.DRAW_LINE, Color.Red);
            SetIndexLabel(0, "ATR");
        }

        public override void OnCalculate(int index)
        {
            double P_High, P_Low, prevclose, sum;

            P_High = High(index);
            P_Low = Low(index);

            if (index == Bars() - 1)
                TempBuffer[index] = P_High - P_Low;
            else
            {
                prevclose = Close(index + 1);
                TempBuffer[index] = Math.Max(P_High, prevclose) - Math.Min(P_Low, prevclose);
            }

            sum = 0;
            for(int i = 0; i < period; i++)
                sum = sum + TempBuffer[index + i];

            ATRBuffer[index] = sum / period;
        }

    }
}
