using IndicatorInterfaceCSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MovingAverage
{
    public class MovingAverage : IndicatorInterface
    {
        [Separator(Label = "Common")]
        public string Separator_Common;
        [Input(Name = "Period")]
        public int period = 8;
        [Input(Name = "HShift")]
        public int HShift = 0;
        [Input(Name = "MA Type")]
        public MA_Method MAType;
        [Input(Name = "Apply to price")]
        public Applied_Price ApplytoPriceParameter;

        public IndicatorBuffer MA = new IndicatorBuffer();

        public override void OnInit()
        {
            SetIndicatorShortName("Moving average");
            Indicator_Separate_Window = false;
            SetIndexBuffer(0, MA);
            SetIndexStyle(0, DrawingStyle.DRAW_LINE, Color.Yellow, LineStyle.STYLE_SOLID, 2);
            SetIndexLabel(0, "MA");
        }

        public override void OnCalculate(int index)
        {
            if (index + period >= Bars())
                return;

            MA[index] = GetMA(Symbol(), Period(), index, HShift, period, MAType, ApplytoPriceParameter, MA[index + 1]);
        }        

    }
}
