using IndicatorInterfaceCSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PriceChannel
{
    public class PriceChannel : IndicatorInterface
    {
        [Separator(Label = "Common")]
        public string Separator_Common;
        [Input(Name = "Period")]
        public int ChannelPeriod = 12;

        public IndicatorBuffer UpBuff = new IndicatorBuffer();
        public IndicatorBuffer DownBuff = new IndicatorBuffer();
        public override void OnInit()
        {
            SetIndicatorShortName("Price channel");
            Indicator_Separate_Window = false;
            SetIndexBuffer(0, UpBuff);
            SetIndexStyle(0, DrawingStyle.DRAW_LINE, Color.Red);
            SetIndexLabel(0, "Up Band");
            SetIndexBuffer(1, DownBuff);
            SetIndexStyle(1, DrawingStyle.DRAW_LINE, Color.Red);
            SetIndexLabel(1, "Down Band");
        }

        public override void OnCalculate(int index)
        {
            UpBuff[index] =   Highest(Symbol(), Period(), Series.MODE_HIGH, ChannelPeriod, index);
            DownBuff[index] = Lowest(Symbol(), Period(), Series.MODE_LOW, ChannelPeriod, index);
        }

    }
}
