using IndicatorInterfaceCSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PFE
{
    public class PFE : IndicatorInterface
    {
        [Separator(Label = "Common")]
        public string Separator_Common;
        [Input(Name = "PFE Period")]
        public int PFEPeriod = 14;
        [Input(Name = "Apply to price")]
        public Applied_Price ApplytoPriceParameter;

        public IndicatorBuffer PFEBuffer = new IndicatorBuffer();
        public IndicatorBuffer EMABuffer = new IndicatorBuffer();
        public override void OnInit()
        {
            SetIndicatorShortName("Polarized Fractal Efficiency");
            Indicator_Separate_Window = true;
            SetIndexBuffer(0, EMABuffer);
            SetIndexStyle(0, DrawingStyle.DRAW_LINE, Color.Red, LineStyle.STYLE_SOLID, 2);
            SetIndexLabel(0, "PFE");

            SetLevel(0, Color.DarkGray, LineStyle.STYLE_DOT, 1);
            SetLevel(50, Color.DarkGray, LineStyle.STYLE_DOT, 1);
            SetLevel(-50, Color.DarkGray, LineStyle.STYLE_DOT, 1);
        }

        public override void OnCalculate(int index)
        {
            double a, b, k, prev;

            if (index + PFEPeriod + 1 >= Bars())
                return;

            a = Math.Abs(GetROC(index, PFEPeriod));
            b = Math.Abs(GetROC(index, 1));

            try
            {
                double PFE = Math.Sqrt(Math.Pow(a, 2) + 100) / (Math.Sqrt(Math.Pow(b, 2) + 1) + PFEPeriod);
                PFEBuffer[index] = GetPrice(index) > GetPrice(index + PFEPeriod) ? PFE : -PFE;                
            }
            catch (Exception)
            {
                PFEBuffer[index] = 0;
            }

            PFEBuffer[index] = PFEBuffer[index] * 100;
            k = (double) 2 / (PFEPeriod + 1);
            prev = EMABuffer[index + 1];

            EMABuffer[index] = prev == 0 ? PFEBuffer[index] : prev + k * (PFEBuffer[index] - prev);
        }

        public double GetROC(int index, int count)
        {
            double result;
            try
            {
                result = ((GetPrice(index) - GetPrice(index + count)) / GetPrice(index + count)) * 100;
            }
            catch (Exception)
            {
                result = 0;
            }

            return result;
        }

        public double GetPrice(int index)
        {
            return GetAppliedPrice(Symbol(), Period(), index, ApplytoPriceParameter);
        }

    }
}
