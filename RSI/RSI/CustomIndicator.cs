using IndicatorInterfaceCSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RSI
{
    public class RSIIndicator : IndicatorInterface
    {
        [Separator(Label = "Common")]
        public string Separator_Common;
        [Input(Name = "Period")]
        public int period = 14;
        [Input(Name = "Apply to price")]
        public Applied_Price ApplyToPriceParameter;

        public IndicatorBuffer RSI = new IndicatorBuffer();
        public IndicatorBuffer AvGain = new IndicatorBuffer();
        public IndicatorBuffer AvLoss = new IndicatorBuffer();

        double diff, gain, loss;

        public override void OnInit()
        {
            SetIndicatorShortName("RSI");
            Indicator_Separate_Window = true;
            SetLevel(30, Color.Gray, LineStyle.STYLE_DOT);
            SetLevel(70, Color.Gray, LineStyle.STYLE_DOT);            
            SetIndexBuffer(0, RSI);
            SetIndexStyle(0, DrawingStyle.DRAW_LINE, Color.DarkOrange);
            SetIndexLabel(0, "RSI");
        }
        public override void OnCalculate(int index)
        {
            if (Bars() - index <= period)
                return;

            if (Bars() - index == period + 1)
            {
                gain = 0;
                loss = 0;
                for (int i = index; i <= index + period - 1; i++)
                {
                    diff = Close(i) - Close(i + 1);
                    if (diff > 0)
                        gain += diff;
                    else
                        loss -= diff;
                }
                AvGain[index] = gain / period;
                AvLoss[index] = loss / period;
            }
            else
            {
                gain = 0;
                loss = 0;
                diff = Close(index) - Close(index + 1);
                if (diff > 0)
                    gain = diff;
                else
                    loss = -diff;
                gain = (AvGain[index + 1] * (period - 1) + gain) / period;
                loss = (AvLoss[index + 1] * (period - 1) + loss) / period;
                AvGain[index] = gain;
                AvLoss[index] = loss;

                if (loss == 0)
                    RSI[index] = 105;
                else
                    RSI[index] = 100 - 100 / (1 + gain / loss);
            }

        }
    }
}
