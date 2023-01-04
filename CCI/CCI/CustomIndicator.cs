using IndicatorInterfaceCSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CCI
{
    public class CCI : IndicatorInterface
    {
        [Separator(Label = "Common")]
        public string Separator_1;
        [Input(Name = "CCI Period")]
        public int CCIPeriod = 14;

        public IndicatorBuffer CCIBuffer = new IndicatorBuffer();
        public IndicatorBuffer MAbuff = new IndicatorBuffer();
        double sum;
        int i;

        public override void OnInit()
        {
            SetIndicatorShortName("Commodity Channel Index (CCI)");
            Indicator_Separate_Window = true;
            SetIndexBuffer(0, CCIBuffer);
            SetIndexStyle(0, DrawingStyle.DRAW_LINE, Color.FromArgb(0x1E, 0x90, 0xFF), LineStyle.STYLE_SOLID, 1);
            SetIndexLabel(0, "CCI");
            SetLevel(-100, Color.Black, LineStyle.STYLE_DOT, 1);
            SetLevel(100, Color.Black, LineStyle.STYLE_DOT, 1);
            SetLevel(0, Color.Black, LineStyle.STYLE_SOLID, 1);
        }

        public override void OnCalculate(int index)
        {
            sum = 0;
            for (int i = 0; i <= CCIPeriod - 1; i++)
                sum = sum + TPrice(index + i);

            MAbuff[index] = sum / CCIPeriod;

            if (index + CCIPeriod >= Bars())
                return;

            sum = 0;
            for (int i = 0; i <= CCIPeriod - 1; i++)
                sum = sum + Math.Abs(TPrice(index + i) - MAbuff[index]);

            if (sum == 0)
                CCIBuffer[index] = CCIBuffer[index + 1];
            else
                CCIBuffer[index] = (TPrice(index) - MAbuff[index]) / 0.015 / (sum / CCIPeriod);
        }

        public double TPrice(int i)
        {
            return (High(i) + Low(i) + Close(i)) / 3;
        }

    }
}
