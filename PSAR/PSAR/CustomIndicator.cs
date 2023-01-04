using IndicatorInterfaceCSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PSAR
{
    public class PSAR : IndicatorInterface
    {
        [Separator(Label = "Common")]
        public string Separator_Common;
        [Input(Name = "Step")]
        public double Step = 0.02;
        [Input(Name = "Maximum")]
        public double Maximum = 0.2;

        public IndicatorBuffer SarBuffer = new IndicatorBuffer();
        public IndicatorBuffer UpBuffer = new IndicatorBuffer();
        public IndicatorBuffer DownBuffer = new IndicatorBuffer();

        int save_lastreverse;
        bool first, save_dirlong;
        double save_start, save_last_high, save_last_low, save_ep, save_sar;
        public override void OnInit()
        {
            SetIndicatorShortName("Parabolic SAR");
            Indicator_Separate_Window = false;
            SetIndexBuffer(0, SarBuffer);
            SetIndexStyle(0, DrawingStyle.DRAW_ARROW, Color.Black, LineStyle.STYLE_SOLID, 1);
            SetIndexArrow(0, 158, -3, 0);
            SetIndexLabel(0, "PSAR Value");
        }

        public override void OnCalculate(int index)
        {
            bool dirlong;
            double start, last_high, last_low, ep, sar, price_low, price_high, price;            
            int i;

            if (Bars() < 3 || index != 0)
                return;

            sar = 0;
            i = Bars() - 2;
            first = false;
            dirlong = true;
            start = Step;
            last_high = Double.MinValue;
            last_low = Double.MaxValue;

            while(i > index)
            {
                save_lastreverse = i;
                price_low = Low(i);
                price_high = High(i);
                last_low = Math.Min(Low(i), last_low);
                last_high = Math.Max(High(i), last_high);
                
                if (High(i) > High(i + 1) && Low(i) > Low(i + 1))
                    break;

                if (High(i) < High(i + 1) && Low(i) < Low(i + 1))
                {
                    dirlong = false;
                    break;
                } 

                i--;
            }

            if (dirlong)
            {
                SarBuffer[i] = Low(i + 1);
                ep = High(i);
            }
            else
            {
                SarBuffer[i] = High(i + 1);
                ep = Low(i);
            }

            while (i >= index)
            {
                price_low = Low(i);
                price_high = High(i);

                if (dirlong && price_low < SarBuffer[i + 1])
                {
                    SaveLastReverse(i, true, start, price_low, last_high, ep, sar);
                    start = Step; 
                    dirlong = false;
                    ep = price_low; 
                    last_low = price_low;
                    SarBuffer[i] = last_high;
                    i--;
                    continue;
                }

                if (!dirlong && price_high > SarBuffer[i + 1])
                {
                    SaveLastReverse(i, false, start, last_low, price_high, ep, sar);
                    start = Step;
                    dirlong = true;
                    ep = price_high;
                    last_high = price_high;
                    SarBuffer[i] = last_low;
                    i--;
                    continue;
                }

                price = SarBuffer[i + 1];
                sar = price + start * (ep - price);

                if (dirlong)
                {
                    if (ep < price_high && start + Step <= Maximum)
                        start = start + Step;

                    if (price_high < High(i + 1) && i == Bars() - 2)
                        sar = SarBuffer[i + 1];

                    price = Low(i + 1);
                    if (sar > price)
                        sar = price;
                    price = Low(i + 2);
                    if (sar > price)
                        sar = price;

                    if(sar > price_low)
                    {
                        SaveLastReverse(i, true, start, price_low, last_high, ep, sar);
                        start = Step; 
                        dirlong = false; 
                        ep = price_low;
                        last_low = price_low;
                        SarBuffer[i] = last_high;
                        i--;
                        continue;
                    }

                    if(ep < price_high)
                    {
                        last_high = price_high; 
                        ep = price_high;
                    }
                }
                else
                {
                    if (ep > price_low && start + Step <= Maximum)
                        start = start + Step;

                    if (price_low < Low(i + 1) && i == Bars() - 2)
                        sar = SarBuffer[i + 1];

                    price = High(i + 1);
                    if (sar < price)
                        sar = price;
                    price = High(i + 2);
                    if (sar < price)
                        sar = price;

                    if (sar < price_high)
                    {
                        SaveLastReverse(i, false, start, last_low, price_high, ep, sar);
                        start = Step;
                        dirlong = true;
                        ep = price_high;
                        last_high = price_high;
                        SarBuffer[i] = last_low;
                        i--;
                        continue;
                    }

                    if (ep > price_low)
                    {
                        last_low = price_low;
                        ep = price_low;
                    }
                }

                SarBuffer[i] = sar;
                i--;
            }
        }

        public void SaveLastReverse(int last, bool dir, double start, double low, double high, double ep, double sar)
        {
            save_lastreverse = last;
            save_dirlong = dir;
            save_start = start;
            save_last_low = low;
            save_last_high = high;
            save_ep = ep;
            save_sar = sar;

            if (save_dirlong)
            {
                UpBuffer[last] = 1;
                DownBuffer[last] = 0;
            }
            else
            {
                UpBuffer[last] = 0;
                DownBuffer[last] = 1;
            }
        }

    }
}
