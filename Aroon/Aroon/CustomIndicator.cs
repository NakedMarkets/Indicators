using IndicatorInterfaceCSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aroon
{
    public class Aroon : IndicatorInterface
    {
        [Separator(Label = "Common")]
        public string Separator_Common;
        [Input(Name = "Period")]
        public int period = 14;

        public IndicatorBuffer Buffer1 = new IndicatorBuffer();
        public IndicatorBuffer Buffer2 = new IndicatorBuffer();

        public override void OnInit()
        {
            SetIndicatorShortName("Aroon");
            Indicator_Separate_Window = true;
            SetLevel(30, Color.Gray, LineStyle.STYLE_DOT);
            SetLevel(70, Color.Gray, LineStyle.STYLE_DOT);
            SetIndexBuffer(0, Buffer1);
            SetIndexStyle(0, DrawingStyle.DRAW_LINE, Color.Yellow);
            SetIndexLabel(0, "Buffer 1");
            SetIndexBuffer(1, Buffer2);
            SetIndexStyle(1, DrawingStyle.DRAW_LINE, Color.DodgerBlue);
            SetIndexLabel(1, "Buffer 2");
        }

        public override void OnCalculate(int index)
        {
            int nHigh,nLow, k;
            double Max, Min, Num;

            if (index + period >= Bars())
                return;

            Max = Double.MinValue;
            Min = Double.MaxValue;
            nHigh = -1;
            nLow = -1;

            for(int i = index; i < index + period; i++)
            {
                Num = Close(i);
                if(Num > Max)
                {
                    Max = Num;
                    nHigh = i;
                }

                if(Num< Min)
                {
                    Min = Num;
                    nLow = i;
                }
            }

            Buffer1[index] = 100.0 * (period - (nHigh - index)) / period;
            Buffer2[index] = 100.0 * (period - (nLow - index)) / period;

        }

    }
}
