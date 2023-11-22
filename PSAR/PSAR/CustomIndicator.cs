using IndicatorInterfaceCSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PSAR
{
    public class PSAR : IndicatorInterface
    {
        [Separator(Label = "Common")]
        public string Separator_Common;
        [Input(Name = "Start")]
        public double StartAccelerationFactor = 0.02;
        [Input(Name = "Step")]
        public double Step = 0.02;
        [Input(Name = "Maximum")]
        public double Maximum = 0.2;

        public IndicatorBuffer PSarBuffer = new IndicatorBuffer();
        public IndicatorBuffer UpBuffer = new IndicatorBuffer();
        public IndicatorBuffer DownBuffer = new IndicatorBuffer();

        int Trend = 1;        
        double AccelerationFactor = 0.02;
        double ExtremePoint;
        List<double> HighPriceTrend;
        List<double> LowPriceTrend;

        public override void OnInit()
        {
            SetIndicatorShortName("Parabolic SAR");
            Indicator_Separate_Window = false;
            SetIndexBuffer(0, PSarBuffer);
            SetIndexStyle(0, DrawingStyle.DRAW_ARROW, Color.Black, LineStyle.STYLE_SOLID, 1);
            SetIndexArrow(0, 158, -3, 0);
            SetIndexLabel(0, "PSAR Value");
        }

        public override void OnAttach()
        {
            HighPriceTrend = new List<double>();
            LowPriceTrend = new List<double>();
            AccelerationFactor = StartAccelerationFactor;
        }

        public override void OnCalculate(int index)
        {
            if (Bars() >= 3)
                CalculatePSARValue(index);            
            else
                InitPSARValue(index);

            UpdatePSARValue(index);
        }

        public void InitPSARValue(int index)
        {
            if(Bars() <= 1)
            {
                Trend = -1;
                ExtremePoint = High(index);
                return;
            }

            if(High(index) < High(index + 1))
            {
                Trend = 1;
                PSarBuffer[index] = Math.Min(Low(index + 1), Low(index + 2));
                ExtremePoint = Math.Max(High(index + 1), High(index + 2));
            }
            else
            {
                Trend = 0;
                PSarBuffer[index] = Math.Max(High(index + 1), High(index + 2));
                ExtremePoint = Math.Min(Low(index + 1), Low(index + 2));
            }
        }

        public void CalculatePSARValue(int index)
        {
            double PreviousPSAR = PSarBuffer[index + 1];
            double PSAR = 0;

            if(Trend == 1) // UP Trend
            {
                PSAR = PreviousPSAR + AccelerationFactor * (ExtremePoint - PreviousPSAR);
                PSAR = Math.Min(PSAR, Math.Min(Low(index + 1), Low(index + 2)));
            }
            else // DOWN Trend
            {
                PSAR = PreviousPSAR - AccelerationFactor * (PreviousPSAR - ExtremePoint);
                PSAR = Math.Max(PSAR, Math.Max(High(index + 1), High(index + 2)));
            }

            PSarBuffer[index] = PSAR;
        }

        public void UpdatePSARValue(int index)
        {
            if(Trend == 1)
            {
                HighPriceTrend.Add(High(index));
                UpBuffer[index] = 1;
                DownBuffer[index] = 0;
            }                
            else
            if(Trend == 0)
            {
                LowPriceTrend.Add(Low(index));
                UpBuffer[index] = 0;
                DownBuffer[index] = 1;
            }            

            TrendReversal(index);
        }

        public void TrendReversal(int index)
        {
            bool Reversal = false;

            if(Trend == 1 && PSarBuffer[index] > Low(index))
            {
                Trend = 0;
                PSarBuffer[index] = HighPriceTrend.Max();
                ExtremePoint = Low(index);
                Reversal = true;      
            }
            else
            if(Trend == 0 && PSarBuffer[index] < High(index))
            {
                Trend = 1;
                PSarBuffer[index] = LowPriceTrend.Min();
                ExtremePoint = High(index);
                Reversal = true;
            }

            if (Reversal)
            {
                AccelerationFactor = StartAccelerationFactor;
                HighPriceTrend.Clear();
                LowPriceTrend.Clear();
            }
            else
            {
                if(High(index) > ExtremePoint && Trend == 1)
                {
                    AccelerationFactor = Math.Min(AccelerationFactor + Step, Maximum);
                    ExtremePoint = High(index);
                }
                else
                if (Low(index) < ExtremePoint && Trend == 0)
                {
                    AccelerationFactor = Math.Min(AccelerationFactor + Step, Maximum);
                    ExtremePoint = Low(index);
                }
            }      
        }
    }
}
