using IndicatorInterfaceCSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DonchianChannel
{
    public class DonchianChannel : IndicatorInterface
    {
        [Separator(Label = "Common")]
        public string Separator_Common;
        [Input(Name = "Period")]
        public int period = 21;
        [Input(Name = "Show Mid Level")]
        public bool ShowMidLevel = true;

        public IndicatorBuffer UPDonchianBuf = new IndicatorBuffer();
        public IndicatorBuffer DNDonchianBuf = new IndicatorBuffer();
        public IndicatorBuffer MidDonchianBuf = new IndicatorBuffer();

        public override void OnInit()
        {
            SetIndicatorShortName("Donchian Channel");
            Indicator_Separate_Window = false;
            SetIndexBuffer(0, UPDonchianBuf);
            SetIndexStyle(0, DrawingStyle.DRAW_LINE, Color.IndianRed);
            SetIndexLabel(0, "Upper Donchian");
            SetIndexBuffer(1, DNDonchianBuf);
            SetIndexStyle(1, DrawingStyle.DRAW_LINE, Color.IndianRed);
            SetIndexLabel(1, "Lower Donchian");
            SetIndexBuffer(2, MidDonchianBuf);
            SetIndexStyle(2, DrawingStyle.DRAW_LINE, Color.IndianRed);
            SetIndexLabel(2, "MidLine Donchian");
        }

        public override void OnCalculate(int index)
        {
            UPDonchianBuf[index] = High(iHighest(Symbol(), Period(), Series.MODE_HIGH, period, index));
            DNDonchianBuf[index] = Low(iLowest(Symbol(), Period(), Series.MODE_LOW, period, index));

            if(ShowMidLevel)
                MidDonchianBuf[index] = (UPDonchianBuf[index] + DNDonchianBuf[index]) / 2;
        }
    }
}
