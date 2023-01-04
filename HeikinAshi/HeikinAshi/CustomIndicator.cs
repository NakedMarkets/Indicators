using IndicatorInterfaceCSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeikinAshi
{
    public class HeikenAshi : IndicatorInterface
    {
        public IndicatorBuffer buff1 = new IndicatorBuffer();
        public IndicatorBuffer buff2 = new IndicatorBuffer();
        public IndicatorBuffer buff3 = new IndicatorBuffer();
        public IndicatorBuffer buff4 = new IndicatorBuffer();
        public override void OnInit()
        {
            SetIndicatorShortName("Heikin Ashi");
            Indicator_Separate_Window = false;
            SetIndexBuffer(0, buff1);
            SetIndexStyle(0, DrawingStyle.DRAW_NONE, Color.Blue);
            SetIndexLabel(0, "Up bar color");
            SetIndexBuffer(1, buff2);
            SetIndexStyle(1, DrawingStyle.DRAW_NONE, Color.Blue);
            SetIndexLabel(1, "Up bar fill color");
            SetIndexBuffer(2, buff3);
            SetIndexStyle(2, DrawingStyle.DRAW_NONE, Color.Red);
            SetIndexLabel(2, "Down bar color");
            SetIndexBuffer(3, buff4);
            SetIndexStyle(3, DrawingStyle.DRAW_CANDLES, Color.Red);
            SetIndexLabel(3, "Down bar fill color");
            SetVisibleChart(false);
        }

        public override void OnCalculate(int index)
        {
            double HaOpen, HaClose, HaHigh, HaLow;

            if (index == Bars() - 1)
            {
                HaOpen = Open(index);
                HaHigh = High(index);
                HaLow = Low(index);
                HaClose = Close(index);
            }
            else
            {
                HaOpen = (buff1[index + 1] + buff4[index + 1]) / 2;
                HaClose = (Open(index) + High(index) + Low(index) + Close(index)) / 4;
                HaHigh = Math.Max(High(index), Math.Max(HaOpen, HaClose));
                HaLow = Math.Min(Low(index), Math.Min(HaOpen, HaClose));
            }

            buff1[index] = HaOpen;
            buff2[index] = HaHigh;
            buff3[index] = HaLow;
            buff4[index] = HaClose;
        }



    }
}
