using IndicatorInterfaceCSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SolarWind
{
    public class SolarWind : IndicatorInterface
    {
        [Separator(Label = "Common")]
        public string Separator_Common;
        [Input(Name = "Period")]
        public int Period = 10;

        public IndicatorBuffer Buffer_1 = new IndicatorBuffer();
        public IndicatorBuffer Buffer_2 = new IndicatorBuffer();
        public IndicatorBuffer Buffer_3 = new IndicatorBuffer();
        public IndicatorBuffer Buffer_4 = new IndicatorBuffer();
        public override void OnInit()
        {
            SetIndicatorShortName("Solar Wind");
            Indicator_Separate_Window = true;
            SetIndexBuffer(0, Buffer_3);
            SetIndexStyle(0, DrawingStyle.DRAW_HISTOGRAM, Color.Green, LineStyle.STYLE_SOLID, 1);
            SetIndexLabel(0, "Up buff");
            SetIndexBuffer(1, Buffer_4);
            SetIndexStyle(1, DrawingStyle.DRAW_HISTOGRAM, Color.Red, LineStyle.STYLE_SOLID, 1);
            SetIndexLabel(1, "Down buff");
            SetLevel(0, Color.DarkGray, LineStyle.STYLE_DOT);
        }

        public override void OnCalculate(int index)
        {
            double MaxH, MinL, price, value, current;

            if (index + Period >= Bars())
                return;

            try
            {
                MaxH = Highest(Symbol(), Period(), Series.MODE_HIGH, Period, index);
                MinL = Lowest(Symbol(), Period(), Series.MODE_LOW, Period, index);
                price = (High(index) + Low(index)) / 2;
                value = 0.33 * 2 * ((price - MinL) / (MaxH - MinL) - 0.5) + 0.67 * Buffer_2[index + 1];
                value = Math.Min(Math.Max(value, -0.999), 0.999);
                Buffer_2[index] = value;
                Buffer_1[index] = 0.5 * Math.Log((1 + value) / (1 - value)) + 0.5 * Buffer_1[index + 1];

                current = Buffer_1[index];

                if(current < 0)
                {
                    Buffer_4[index] = current;
                    Buffer_3[index] = 0;
                }
                else
                {
                    Buffer_3[index] = current;
                    Buffer_4[index] = 0;
                }

            }
            catch (Exception)
            {
                Buffer_2[index] = Buffer_2[index + 1];
                Buffer_3[index] = Buffer_3[index + 1];
                Buffer_4[index] = Buffer_4[index + 1];
            }

        }

    }
}
