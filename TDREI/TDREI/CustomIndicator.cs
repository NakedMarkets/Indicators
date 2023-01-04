using IndicatorInterfaceCSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TDREI
{
    public class TDREI : IndicatorInterface
    {
        [Separator(Label = "Common")]
        public string Separator_Common;
        [Input(Name = "Period")]
        public int Period = 5;

        public IndicatorBuffer Buffer_1 = new IndicatorBuffer();
        public IndicatorBuffer Buffer_2 = new IndicatorBuffer();
        public IndicatorBuffer Buffer_3 = new IndicatorBuffer();
        public override void OnInit()
        {
            SetIndicatorShortName("TD Range Expansions Index");
            Indicator_Separate_Window = true;
            SetIndexBuffer(0, Buffer_1);
            SetIndexStyle(0, DrawingStyle.DRAW_LINE, Color.Red, LineStyle.STYLE_SOLID, 2);
            SetIndexLabel(0, "Tom DeMark REI");

            SetLevel(45, Color.DarkGray, LineStyle.STYLE_DOT);
            SetLevel(-45, Color.DarkGray, LineStyle.STYLE_DOT);
        }

        public override void OnCalculate(int index)
        {
            double sum1, sum2;

            if (index + 8 >= Bars())
                return;

            if ((High(index) >= Low(index + 5) || High(index) >= Low(index + 6) &&
                (Low(index) <= High(index + 5)) || (Low(index) <= High(index + 6))) ||
                (High(index + 2) >= Close(index + 7) || (High(index + 2) >= Close(index + 8)) &&
                (Low(index + 2) <= Close(index + 7)) || (Low(index + 2) <= Close(index + 8))))
            {
                Buffer_2[index] = High(index) - High(index + 2) + Low(index) - Low(index + 2);
                Buffer_3[index] = Math.Abs(High(index) - High(index + 2)) + Math.Abs(Low(index) - Low(index + 2));
            }
            else
            {
                Buffer_2[index] = 0;
                Buffer_3[index] = 0;
            }

            sum1 = 0;
            sum2 = 0;

            for (int i = 0; i < Period; i++)
            {
                sum1 = sum1 + Buffer_2[index + i];
                sum2 = sum2 + Buffer_3[index + i];
            }

            try
            {
                Buffer_1[index] = sum1 / sum2 * 100;
            }
            catch (Exception)
            {
                Buffer_1[index] = 0;
            }
        }
    }
}
