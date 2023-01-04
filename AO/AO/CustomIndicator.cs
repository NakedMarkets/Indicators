using IndicatorInterfaceCSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AO
{
    public class AO : IndicatorInterface
    {
        [Separator(Label = "Common")]
        public string Separator_Common;
        [Input(Name = "periodF")]
        public int periodF = 5;
        [Input(Name = "periodS")]
        public int periodS = 34;
        [Input(Name = "MA Type")]
        public MA_Method MAType;
        [Input(Name = "Apply to price")]
        public Applied_Price ApplyToPriceParameter;

        public IndicatorBuffer SMA1 = new IndicatorBuffer();
        public IndicatorBuffer SMA2 = new IndicatorBuffer();
        public IndicatorBuffer SMAF = new IndicatorBuffer();
        public IndicatorBuffer SMAS = new IndicatorBuffer();
        public IndicatorBuffer SMA = new IndicatorBuffer();
        public override void OnInit()
        {
            SetIndicatorShortName("Awesome Oscillator (B. Williams)");
            Indicator_Separate_Window = true;
            SetLevel(0, Color.Gray, LineStyle.STYLE_DOT);     
            
            SetIndexBuffer(0, SMA1);
            SetIndexStyle(0, DrawingStyle.DRAW_HISTOGRAM, Color.Lime, LineStyle.STYLE_SOLID, 3);
            SetIndexLabel(0, "SMA 1");
            SetIndexBuffer(1, SMA2);
            SetIndexStyle(1, DrawingStyle.DRAW_HISTOGRAM, Color.Red, LineStyle.STYLE_SOLID, 3);
            SetIndexLabel(1, "SMA 2");
        }

        public override void OnCalculate(int index)
        {
            double current, prev;

            if (index + periodF >= Bars() || index + periodS >= Bars())
                return;

            SMAF[index] = GetMA(Symbol(), Period(), index, 0, periodF, MAType, ApplyToPriceParameter, SMAF[index + 1]);
            SMAS[index] = GetMA(Symbol(), Period(), index, 0, periodS, MAType, ApplyToPriceParameter, SMAS[index + 1]);

            SMA[index] = SMAF[index] - SMAS[index];
            current = SMA[index];
            prev = SMA[index + 1];

            if(current < prev)
            {
                SMA2[index] = current;
                SMA1[index] = 0;
            }

            if(current > prev)
            {
                SMA1[index] = current;
                SMA2[index] = 0;
            }

        }

    }
}
