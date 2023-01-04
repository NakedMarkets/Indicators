using IndicatorInterfaceCSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AC
{
    public class AcceleratorOscillator : IndicatorInterface
    {
        [Separator(Label = "Common")]
        public string Separator1;
        [Input(Name = "Period F")]
        public int PeriodF = 5;
        [Input(Name = "Period S")]
        public int PeriodS = 34;
        [Input(Name = "Period AC")]
        public int PeriodSAC = 5;
        [Input(Name = "MA Type")]
        public MA_Method MAType;
        [Input(Name = "Apply to price")]
        public Applied_Price ApplytoPrice;

        public IndicatorBuffer SMA = new IndicatorBuffer();
        public IndicatorBuffer SMAF = new IndicatorBuffer();
        public IndicatorBuffer SMAS = new IndicatorBuffer();
        public IndicatorBuffer SMAAC = new IndicatorBuffer();
        public IndicatorBuffer SMA1 = new IndicatorBuffer();
        public IndicatorBuffer SMA2 = new IndicatorBuffer();

        public override void OnInit()
        {
            SetIndicatorShortName("Accelerator Oscillator (B. Williams)");
            Indicator_Separate_Window = true;
            SetIndexBuffer(0, SMA1);
            SetIndexStyle(0, DrawingStyle.DRAW_HISTOGRAM, Color.Lime, LineStyle.STYLE_SOLID, 3);
            SetIndexLabel(0, "AC 1");
            SetIndexBuffer(1, SMA2);
            SetIndexStyle(1, DrawingStyle.DRAW_HISTOGRAM, Color.Red, LineStyle.STYLE_SOLID, 3);
            SetIndexLabel(1, "AC 2");
            SetLevel(0, Color.Black, LineStyle.STYLE_DOT, 1);
        }

        public override void OnCalculate(int index)
        {
            double sum, current, prev;

            if (index + PeriodF >= Bars() || index + PeriodS >= Bars())
                return;

            SMAF[index] = iMA(Symbol(), Period(), PeriodF, 0, MAType, ApplytoPrice, index);
            SMAS[index] = iMA(Symbol(), Period(), PeriodS, 0, MAType, ApplytoPrice, index);
            SMA[index] = SMAF[index] - SMAS[index];
            sum = 0;

            for (int i = index; i < index + PeriodSAC; i++)
                sum = sum + SMA[i];

            SMAAC[index] = sum / PeriodSAC;

            current = SMA[index] - SMAAC[index];
            prev = SMA[index + 1] - SMAAC[index + 1];

            if (current < prev)
            {
                SMA2[index] = current;
                SMA1[index] = 0;
            }
            else
            {
                SMA1[index] = current;
                SMA2[index] = 0;
            }
        }
    }
}
