using IndicatorInterfaceCSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OBV
{
    public class OBV : IndicatorInterface
    {
        [Separator(Label = "Common")]
        public string Separator_Common;
        [Input(Name = "Apply to price")]
        public Applied_Price ApplytoPriceParameter;

        public IndicatorBuffer OBVBuffer = new IndicatorBuffer();

        public override void OnInit()
        {
            SetIndicatorShortName("On Balance Volume");
            Indicator_Separate_Window = true;
            SetIndexBuffer(0, OBVBuffer);
            SetIndexStyle(0, DrawingStyle.DRAW_LINE, Color.DeepSkyBlue, LineStyle.STYLE_SOLID, 2);
            SetIndexLabel(0, "OBV");
        }

        public override void OnCalculate(int index)
        {
            double price1, price2;

            if (index >= Bars())
                return;

            price1 = GetAppliedPrice(Symbol(), Period(), index, ApplytoPriceParameter);
            price2 = GetAppliedPrice(Symbol(), Period(), index + 1, ApplytoPriceParameter);

            if (price1 > price2)
                OBVBuffer[index] = OBVBuffer[index + 1] + Volume(index);
            else
            if(price1 < price2)
                OBVBuffer[index] = OBVBuffer[index + 1] - Volume(index);
            else
                OBVBuffer[index] = OBVBuffer[index + 1];
        }

    }
}
