using IndicatorInterfaceCSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BollingerBands
{
    public class BollingerBands : IndicatorInterface
    {
        [Separator(Label = "Common")]
        public string Separator_Common;
        [Input(Name = "Period")]
        public int period = 8;
        [Input(Name = "Shift")]
        public int Shift = 0;
        [Input(Name = "Deviation")]
        public double Deviation = 2;
        [Input(Name = "MAType")]
        public MA_Method MAType;
        [Input(Name = "Apply to price")]
        public Applied_Price ApplyToPriceParameter;

        public IndicatorBuffer UpBand = new IndicatorBuffer();
        public IndicatorBuffer MA = new IndicatorBuffer();
        public IndicatorBuffer DownBand = new IndicatorBuffer();
        public override void OnInit()
        {
            SetIndicatorShortName("Bollinger Bands");
            Indicator_Separate_Window = false;
            SetIndexBuffer(0, UpBand);
            SetIndexStyle(0, DrawingStyle.DRAW_LINE, Color.DarkOrange);
            SetIndexLabel(0, "Up Band");
            SetIndexBuffer(1, MA);
            SetIndexStyle(1, DrawingStyle.DRAW_LINE, Color.DarkOrange);
            SetIndexLabel(1, "MA");
            SetIndexBuffer(2, DownBand);
            SetIndexStyle(2, DrawingStyle.DRAW_LINE, Color.DarkOrange);
            SetIndexLabel(2, "Down Band");
        }

        public override void OnCalculate(int index)
        {
            double sum, value, sd;

            if (index + period >= Bars())
                return;

            value = GetMA(Symbol(), Period(), index, 0, period, MAType, ApplyToPriceParameter, MA[index + 1]);
            MA[index] = value;

            sum = 0;
            for (int i = index; i < index + period; i++)
                sum = sum + Math.Pow(Math.Abs(GetAppliedPrice(Symbol(), Period(), i, ApplyToPriceParameter) - value), 2);

            sd = Math.Sqrt(sum / period) * Deviation;
            UpBand[index] = value + sd;
            DownBand[index] = value - sd;
        }
    }
}
