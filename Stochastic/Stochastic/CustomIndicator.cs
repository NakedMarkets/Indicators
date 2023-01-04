using IndicatorInterfaceCSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stochastic
{
    public class Stochastic : IndicatorInterface
    {
        [Separator(Label = "Common")]
        public string Separator_Common;
        [Input(Name = "%K period")]
        public int KPeriod = 9;
        [Input(Name = "%D period")]
        public int DPeriod = 3;
        [Input(Name = "Slowing")]
        public int Slowing = 3;
        [Input(Name = "Apply to price")]
        public AppliedPriceType ApplytoPriceParameter;

        public IndicatorBuffer Kfast = new IndicatorBuffer();
        public IndicatorBuffer Kslow = new IndicatorBuffer();
        public IndicatorBuffer Dline = new IndicatorBuffer();
        public IndicatorBuffer HighesBuffer = new IndicatorBuffer();
        public IndicatorBuffer LowesBuffer = new IndicatorBuffer();
        public override void OnInit()
        {
            SetIndicatorShortName("Stochastic");
            Indicator_Separate_Window = true;
            SetIndexBuffer(0, Kslow);
            SetIndexStyle(0, DrawingStyle.DRAW_LINE, Color.Red);
            SetIndexLabel(0, "K Line");
            SetIndexBuffer(1, Dline);
            SetIndexStyle(1, DrawingStyle.DRAW_LINE, Color.DeepSkyBlue, LineStyle.STYLE_SOLID, 1);
            SetIndexLabel(1, "D Line");
            SetLevel(20, Color.DarkGray, LineStyle.STYLE_DOT);
            SetLevel(80, Color.DarkGray, LineStyle.STYLE_DOT);
        }

        public override void OnCalculate(int index)
        {
            double sumlow, sumhigh, Plow, Phigh;

            if (Bars() < KPeriod || Bars() < Slowing || Bars() < DPeriod)
                return;

            Phigh = Double.MinValue;
            Plow = Double.MaxValue;

            if (ApplytoPriceParameter == AppliedPriceType.LowHigh)
            {
                for(int i = 0; i < KPeriod; i++)
                {
                    Phigh = Math.Max(Phigh, High(index + i));
                    Plow = Math.Min(Plow, Low(index + i));
                }
            }
            else
            {
                for (int i = 0; i < KPeriod; i++)
                {
                    Phigh = Math.Max(Phigh, Close(index + i));
                    Plow = Math.Min(Plow, Close(index + i));
                }
            }

            HighesBuffer[index] = Phigh;
            LowesBuffer[index] = Plow;
            sumlow = 0;
            sumhigh = 0;

            for(int i = index + Slowing - 1; i >= index; i--)
            {
                sumlow = sumlow + Close(i) - LowesBuffer[i];
                sumhigh = sumhigh + HighesBuffer[i] - LowesBuffer[i];
            }

            Kslow[index] = sumhigh == 0 ? 100 : sumlow / sumhigh * 100;
            Dline[index] = MAOnArray(Kslow, DPeriod, index);
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

        public enum AppliedPriceType
        {
            [Description("Low/High")]
            LowHigh,
            [Description("Close/Close")]
            CloseClose
        }
    }
}
