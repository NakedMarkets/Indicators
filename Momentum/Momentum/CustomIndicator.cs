using IndicatorInterfaceCSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Momentum
{
    public class Momentum : IndicatorInterface
    {
        [Separator(Label = "Common")]
        public string Separator_Common;
        [Input(Name = "Period")]
        public int period = 14;
        [Input(Name = "Apply to Price")]
        public Applied_Price ApplyToPriceParameter;

        public IndicatorBuffer MomentumBuffer = new IndicatorBuffer();
        public override void OnInit()
        {
            SetIndicatorShortName("Momentum");
            Indicator_Separate_Window = true;
            SetIndexBuffer(0, MomentumBuffer);
            SetIndexStyle(0, DrawingStyle.DRAW_LINE, Color.BlueViolet);
            SetIndexLabel(0, "Momentum");
        }

        public override void OnCalculate(int index)
        {
            if (index + period >= Bars())
                return;

            MomentumBuffer[index] = GetAppliedPrice(Symbol(), Period(), index, ApplyToPriceParameter) * 100 / GetAppliedPrice(Symbol(), Period(), index + period, ApplyToPriceParameter);
        }

    }
}
