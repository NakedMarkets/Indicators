using IndicatorInterfaceCSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ROC
{
    public class ROC : IndicatorInterface
    {
        [Separator(Label = "Common")]
        public string Separator_Common;
        [Input(Name = "Period")]
        public int ROCPeriod = 14;
        [Input(Name = "Apply to price")]
        public Applied_Price ApplytoPriceParameter;

        public IndicatorBuffer ROCBuffer = new IndicatorBuffer();
        public override void OnInit()
        {
            SetIndicatorShortName("Rate of Change");
            Indicator_Separate_Window = true;
            SetIndexBuffer(0, ROCBuffer);
            SetIndexStyle(0, DrawingStyle.DRAW_LINE, Color.DarkMagenta, LineStyle.STYLE_SOLID, 1);
            SetIndexLabel(0, "ROC");
            SetLevel(0, Color.DarkGray, LineStyle.STYLE_DOT);
        }

        public override void OnCalculate(int index)
        {
            if (index + ROCPeriod >= Bars())
                return;

            try
            {
                ROCBuffer[index] = ((GetAppliedPrice(Symbol(), Period(), index, ApplytoPriceParameter) - GetAppliedPrice(Symbol(), Period(), index + ROCPeriod, ApplytoPriceParameter)) / GetAppliedPrice(Symbol(), Period(), index + ROCPeriod, ApplytoPriceParameter)) * 100;
            }
            catch (Exception)
            {
                ROCBuffer[index] = 0;
            }
        }

    }
}
