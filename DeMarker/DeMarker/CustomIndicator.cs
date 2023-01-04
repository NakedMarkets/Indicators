using IndicatorInterfaceCSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DeMarker
{
    public class DeMarker : IndicatorInterface
    {
        [Separator(Label = "Common")]
        public string Separator_Common;
        [Input(Name = "DeMarker Period")]
        public int DeMarkerPeriod = 14;

        public IndicatorBuffer DeMarkerBuffer = new IndicatorBuffer();
        public IndicatorBuffer ExtMaxBuffer = new IndicatorBuffer();
        public IndicatorBuffer ExtMinBuffer = new IndicatorBuffer();
        public IndicatorBuffer dNUM1 = new IndicatorBuffer();
        public IndicatorBuffer dNUM2 = new IndicatorBuffer();
        public override void OnInit()
        {
            SetIndicatorShortName("DeMarker");
            Indicator_Separate_Window = true;
            SetIndexBuffer(0, DeMarkerBuffer);
            SetIndexStyle(0, DrawingStyle.DRAW_LINE, Color.DarkGreen);
            SetIndexLabel(0, "DeMarker");
        }

        public override void OnCalculate(int index)
        {
            double dNum;

            if (DeMarkerPeriod + 2 > Bars())
                return;

            ExtMaxBuffer[index] = 0;
            ExtMinBuffer[index] = 0;

            dNum = High(index) - High(index + 1);
            if (dNum < 0)
                dNum = 0;
            ExtMaxBuffer[index] = dNum;

            dNum = Low(index + 1) - Low(index);
            if (dNum < 0)
               dNum = 0;
            ExtMinBuffer[index] = dNum;

            dNUM1[index] = MAOnArray(ExtMaxBuffer, DeMarkerPeriod, index);
            dNUM2[index] = MAOnArray(ExtMinBuffer, DeMarkerPeriod, index);
            dNum = dNUM1[index] + dNUM2[index];

            if(dNum != 0)
                DeMarkerBuffer[index] = MAOnArray(ExtMaxBuffer, DeMarkerPeriod, index) / dNum;
            else
                DeMarkerBuffer[index] = 0;

        }

        public double MAOnArray(IndicatorBuffer Array, int Period, int index)
        {
            double sum = 0;
            double result = 0;
            
            for (int i = index; i < Period + index; i++)
            {
                sum = sum + Array[i];
                result = sum / Period;
            }

            return result;
        }

    }
}
