using IndicatorInterfaceCSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WPR
{
    public class WPR : IndicatorInterface
    {
        [Separator(Label = "Common")]
        public string Separator_Common;
        [Input(Name = "Period")]
        public int WPRPeriod = 14;
        [Input(Name = "Apply to price")]
        public Applied_Price ApplytoPriceParameter;

        public IndicatorBuffer WPRBuffer = new IndicatorBuffer();
        public override void OnInit()
        {
            SetIndicatorShortName("Williams Percent Range");
            Indicator_Separate_Window = true;
            SetIndexBuffer(0, WPRBuffer);
            SetIndexStyle(0, DrawingStyle.DRAW_LINE, Color.Red, LineStyle.STYLE_SOLID, 1);
            SetIndexLabel(0, "%R");
            SetLevel(-20, Color.DarkGray, LineStyle.STYLE_DOT);
            SetLevel(-80, Color.DarkGray, LineStyle.STYLE_DOT);
        }

        public override void OnCalculate(int index)
        {
            double dMaxHigh, dMinLow;

            dMaxHigh = Highest(Symbol(), Period(), Series.MODE_HIGH, WPRPeriod, index);
            dMinLow = Lowest(Symbol(), Period(), Series.MODE_LOW, WPRPeriod, index);

            if (dMaxHigh - dMinLow == 0)
                WPRBuffer[index] = 0;
            else
                WPRBuffer[index] = -100 * (dMaxHigh - GetAppliedPrice(Symbol(), Period(), index, ApplytoPriceParameter)) / (dMaxHigh - dMinLow);
        }

    }
}
