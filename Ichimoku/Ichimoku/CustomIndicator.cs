using IndicatorInterfaceCSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ichimoku
{
    public class Ichimoku : IndicatorInterface
    {
        [Separator(Label = "Common")]
        public string Separator_Common;
        [Input(Name = "Tenkan-sen period")]
        public int TenkanSenPeriod = 9;
        [Input(Name = "Kijun-sen period")]
        public int KijunSenPeriod = 26;
        [Input(Name = "Senkou Span period")]
        public int SenkouSpanPeriod = 52;

        public IndicatorBuffer TenkanSen = new IndicatorBuffer();
        public IndicatorBuffer KijunSen = new IndicatorBuffer();
        public IndicatorBuffer ChinkouSpan = new IndicatorBuffer();
        public IndicatorBuffer SenkouSpanA = new IndicatorBuffer();
        public IndicatorBuffer SenkouSpanB = new IndicatorBuffer();

        public override void OnInit()
        {
            SetIndicatorShortName("Ichimoku");
            Indicator_Separate_Window = false;
            SetIndexBuffer(0, TenkanSen);
            SetIndexStyle(0, DrawingStyle.DRAW_LINE, Color.Red);
            SetIndexLabel(0, "Tenkan Sen");
            SetIndexBuffer(1, KijunSen);
            SetIndexStyle(1, DrawingStyle.DRAW_LINE, Color.Blue);
            SetIndexLabel(1, "Kijun Sen");
            SetIndexBuffer(2, ChinkouSpan);
            SetIndexStyle(2, DrawingStyle.DRAW_LINE, Color.Green);
            SetIndexLabel(2, "Chinkou Span");
            SetIndexBuffer(3, SenkouSpanA);
            SetIndexStyle(3, DrawingStyle.DRAW_LINE, Color.FromArgb(0xFF, 0xAB, 0x79));
            SetIndexLabel(3, "Senkou Span A");
            SetIndexBuffer(4, SenkouSpanB);
            SetIndexStyle(4, DrawingStyle.DRAW_FILL, Color.FromArgb(0x9E, 0x9E, 0xFF));
            SetIndexLabel(4, "Senkou Span B");
        }

        public override void OnAttach()
        {
            SetIndexShift(2, -KijunSenPeriod);
            SetIndexShift(3, KijunSenPeriod);
            SetIndexShift(4, KijunSenPeriod);
        }

        public override void OnCalculate(int index)
        {
            double LocalHigh, LocalLow;

            // calculate tenkan-sen
            LocalHigh = High(index);
            LocalLow = Low(index);

            for(int i = index + 1; i < index + TenkanSenPeriod ; i++)
            {
                LocalHigh = Math.Max(High(i), LocalHigh);
                LocalLow = Math.Min(Low(i), LocalLow);
            }

            TenkanSen[index] = (LocalHigh + LocalLow) / 2;

            // calculate kijun-sen
            LocalHigh = High(index);
            LocalLow = Low(index);

            for (int i = index + 1; i < index + KijunSenPeriod; i++)
            {
                LocalHigh = Math.Max(High(i), LocalHigh);
                LocalLow = Math.Min(Low(i), LocalLow);
            }

            KijunSen[index] = (LocalHigh + LocalLow) / 2;
            // calculate chinkou-span
            ChinkouSpan[index] = Close(index);
            // calculate Senkou Span A
            SenkouSpanA[index] = (TenkanSen[index] + KijunSen[index]) / 2;

            // calculate Senkou Span B
            LocalHigh = High(index);
            LocalLow = Low(index);

            for (int i = index + 1; i < index + SenkouSpanPeriod; i++)
            {
                LocalHigh = Math.Max(High(i), LocalHigh);
                LocalLow = Math.Min(Low(i), LocalLow);
            }
            SenkouSpanB[index] = (LocalHigh + LocalLow) / 2;

        }

    }
}
