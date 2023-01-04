using IndicatorInterfaceCSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UltimateOscillator
{
    public class UltimateOscillator : IndicatorInterface
    {
        [Separator(Label = "Common")]
        public string Separator_Common;
        [Input(Name = "Period 1")]
        public int Period_1 = 7;
        [Input(Name = "Period 2")]
        public int Period_2 = 14;
        [Input(Name = "Period 3")]
        public int Period_3 = 28;

        public IndicatorBuffer UOBuffer = new IndicatorBuffer();

        public override void OnInit()
        {
            SetIndicatorShortName("Ultimate Oscillator");
            Indicator_Separate_Window = true;
            SetIndexBuffer(0, UOBuffer);
            SetIndexStyle(0, DrawingStyle.DRAW_LINE, Color.Red, LineStyle.STYLE_SOLID, 2);
            SetIndexLabel(0, "Ultimate Oscillator");

            SetLevel(30, Color.DarkGray, LineStyle.STYLE_DOT);
            SetLevel(50, Color.Gray, LineStyle.STYLE_DOT);
            SetLevel(70, Color.DarkGray, LineStyle.STYLE_DOT);
        }

        public override void OnCalculate(int index)
        {
            double c = 0, h = 0, l = 0, a1 = 0, a2 = 0, a3 = 0, b1 = 0, b2 = 0, b3 = 0;

            if (index + Period_3 >= Bars())
                return;

            int UpperBound = Math.Max(Math.Max(Period_1, Period_2), Period_3);

            for(int i = 0; i < UpperBound; i++)
            {
                c = Close(index + i);
                h = High(index + i);
                l = Low(index + i);

                if( i < Period_1)
                {
                    a1 = a1 + (c - l);
                    b1 = b1 + (h - l);
                }

                if (i < Period_2)
                {
                    a2 = a2 + (c - l);
                    b2 = b2 + (h - l);
                }

                if (i < Period_3)
                {
                    a3 = a3 + (c - l);
                    b3 = b3 + (h - l);
                }
            }

            try
            {
                UOBuffer[index] = (a1 / b1 * 4 + a2 / b2 * 2 + a3 / b3) / 7 * 100;
            }
            catch (Exception)
            {
                UOBuffer[index] = 50;
            }
        }

    }
}
