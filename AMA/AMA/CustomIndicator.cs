using IndicatorInterfaceCSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AMA
{
    public class AMA : IndicatorInterface
    {
        [Separator(Label = "Common")]
        public string Separator_Common;
        [Input(Name = "Fast MA Period")]
        public int FastMA = 3;
        [Input(Name = "Slow MA Period")]
        public int SlowMA = 100;
        [Input(Name = "AMA Period")]
        public int AMAPeriod = 10;
        [Input(Name = "AMA Shift")]
        public int Shift = 0;
        [Input(Name = "ApplyToPrice")]
        public Applied_Price ApplyToPrice;

        public IndicatorBuffer AMABuf = new IndicatorBuffer();

        public override void OnInit()
        {
            SetIndicatorShortName("Adaptive Moving Average (AMA)");
            Indicator_Separate_Window = false;

            SetIndexBuffer(0, AMABuf);
            SetIndexStyle(0, DrawingStyle.DRAW_LINE, Color.Green, LineStyle.STYLE_SOLID, 1);
            SetIndexLabel(0, "AMA");
        }

        public override void OnCalculate(int index)
        {
            double SSC, ER, Volatility;
            double k1, k2;

            if (index + AMAPeriod >= Bars())
                return;

            if(AMABuf[index + 1] == 0)
            {
                AMABuf[index] = GetPrice(index);
                return;
            }

            Volatility = 0;
            for(int i = 0; i < AMAPeriod; i++)
                Volatility = Volatility + Math.Abs(GetPrice(index + i) - GetPrice(index + i + 1));

            if (Volatility != 0)
                ER = (GetPrice(index) - GetPrice(index + AMAPeriod)) / Volatility;
            else
                ER = 0;

            k1 = 2.0 / (FastMA + 1);
            k2 = 2.0 / (SlowMA + 1);

            SSC = ER * (k1 - k2) + k2;

            AMABuf[index] = AMABuf[index + 1] + SSC * SSC * (GetPrice(index) - AMABuf[index + 1]);
        }

        public double GetPrice(int index)
        {
            return GetAppliedPrice(Symbol(), Period(), index, ApplyToPrice);
        }

    }
}
