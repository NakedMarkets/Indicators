using IndicatorInterfaceCSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ADX
{
    public class ADX : IndicatorInterface
    {
        [Separator(Label = "Common")]
        public string Separator_Common;
        [Input(Name = "ADX Period")]
        public int ADXPeriod = 14;
        [Input(Name = "Apply to price")]
        public Applied_Price ApplytoPrice;

        public IndicatorBuffer ADXBuffer = new IndicatorBuffer();
        public IndicatorBuffer PlusDiBuffer = new IndicatorBuffer();
        public IndicatorBuffer MinusDiBuffer = new IndicatorBuffer();
        public IndicatorBuffer PlusSdiBuffer = new IndicatorBuffer();
        public IndicatorBuffer MinusSdiBuffer = new IndicatorBuffer();
        public IndicatorBuffer TempBuffer = new IndicatorBuffer();
        public override void OnInit()
        {            
            SetIndicatorShortName("Average Directional Movement Index (ADX)");
            Indicator_Separate_Window = true;
            SetIndexBuffer(0, ADXBuffer);
            SetIndexStyle(0, DrawingStyle.DRAW_LINE, Color.Yellow, LineStyle.STYLE_SOLID, 1);
            SetIndexLabel(0, "ADX");
            SetIndexBuffer(1, PlusDiBuffer);
            SetIndexStyle(1, DrawingStyle.DRAW_LINE, Color.Green, LineStyle.STYLE_SOLID, 1);
            SetIndexLabel(1, "+Di");
            SetIndexBuffer(2, MinusDiBuffer);
            SetIndexStyle(2, DrawingStyle.DRAW_LINE, Color.Blue, LineStyle.STYLE_SOLID, 1);
            SetIndexLabel(2, "-Di");                        
        }

        public override void OnCalculate(int index)
        {
            double divide, pdm, mdm, tr, price_high, price_low, k;
            double num1, num2, num3;

            price_low = Low(index);
            price_high = High(index);

            pdm = price_high - High(index + 1);
            mdm = Low(index + 1) - price_low;

            if (pdm < 0) pdm = 0;
            if (mdm < 0) mdm = 0;

            if(pdm == mdm)
            {
                pdm = 0;
                mdm = 0;
            }
            else
            {
                if (pdm < mdm)
                    pdm = 0;
                else
                if(mdm < pdm)
                    mdm = 0;
            }

            //---- calculate real interval
            num1 = Math.Abs(price_high - price_low);
            num2 = Math.Abs(price_high - GetAppliedPrice(Symbol(), Period(), index + 1, ApplytoPrice));
            num3 = Math.Abs(price_low - GetAppliedPrice(Symbol(), Period(), index + 1, ApplytoPrice));

            tr = Math.Max(num1, num2);
            tr = Math.Max(tr, num3);

            //---- counting plus/minus direction
            if (tr == 0)
            {
                PlusSdiBuffer[index] = 0;
                MinusSdiBuffer[index] = 0;
            }
            else
            {
                PlusSdiBuffer[index] = 100.0 * pdm / tr;
                MinusSdiBuffer[index] = 100.0 * mdm / tr;
            }

            k = (double)2 / (ADXPeriod + 1);

            //---- apply EMA to +DI
            if (index == Bars() - 2)
                PlusDiBuffer[index + 1] = PlusSdiBuffer[index + 1];
            PlusDiBuffer[index] = PlusSdiBuffer[index] * k + PlusDiBuffer[index + 1] * (1 - k);

            //---- apply EMA to -DI
            if (index == Bars() - 2)
                MinusDiBuffer[index + 1] = MinusSdiBuffer[index + 1];
            MinusDiBuffer[index] = MinusSdiBuffer[index] * k + MinusDiBuffer[index + 1] * (1 - k);

            //---- Directional Movement (DX)
            divide = Math.Abs(PlusDiBuffer[index] + MinusDiBuffer[index]);
            TempBuffer[index] = divide == 0 ? 0 : 100 * (Math.Abs(PlusDiBuffer[index] - MinusDiBuffer[index]) / divide);

            //---- ADX is exponential moving average on DX
            if (index == Bars() - 2)
                ADXBuffer[index + 1] = TempBuffer[index + 1];
            ADXBuffer[index] = TempBuffer[index] * k + ADXBuffer[index + 1] * (1 - k);
        }

    }
}
