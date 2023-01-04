using IndicatorInterfaceCSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Keltner
{
    public class Keltner : IndicatorInterface
    {
        [Separator(Label = "Common")]
        public string Separator_Common;
        [Input(Name = "Period")]
        public int KeltnerPeriod = 10;

        public IndicatorBuffer Upper = new IndicatorBuffer();
        public IndicatorBuffer Middle = new IndicatorBuffer();
        public IndicatorBuffer Lower = new IndicatorBuffer();
        public override void OnInit()
        {
            SetIndicatorShortName("Keltner Channels");
            Indicator_Separate_Window = false;
            SetIndexBuffer(0, Upper);
            SetIndexStyle(0, DrawingStyle.DRAW_LINE, Color.Lime);
            SetIndexLabel(0, "Upper band");
            SetIndexBuffer(1, Middle);
            SetIndexStyle(1, DrawingStyle.DRAW_LINE, Color.Red);
            SetIndexLabel(1, "Middle band");
            SetIndexBuffer(2, Lower);
            SetIndexStyle(2, DrawingStyle.DRAW_LINE, Color.Lime);
            SetIndexLabel(2, "Lower band");
        }

        public override void OnCalculate(int index)
        {
            double average;

            if (index + KeltnerPeriod >= Bars())
                return;

            Middle[index] = GetMA(Symbol(), Period(), index, 0, KeltnerPeriod, MA_Method.MODE_SMA, Applied_Price.PRICE_WEIGHTED, Middle[index + 1]);
            average = CalculateAverage(index);

            Upper[index] = Middle[index] + average;
            Lower[index] = Middle[index] - average;

        }

        public double CalculateAverage(int value)
        {
            double sum = 0;
            for(int i = value; i < value + KeltnerPeriod; i++)            
                sum = sum + High(i) - Low(i);

            return sum / KeltnerPeriod;
        }

    }
}
