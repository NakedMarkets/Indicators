using IndicatorInterfaceCSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TDI
{
    public class TDI : IndicatorInterface
    {
        [Separator(Label = "Common")]
        public string Separator_Common;
        [Input(Name = "RSI Period")]
        public int RSI_Period = 13;
        [Input(Name = "RSI Price")]
        public Applied_Price RSI_Price;
        [Input(Name = "Volatility Band")]
        public int Volatility_Band = 34;
        [Input(Name = "RSI Price Line")]
        public int RSI_Price_Line = 2;
        [Input(Name = "Trade Signal Line")]
        public int Trade_Signal_Line = 7;
        [Input(Name = "Max Bars count")]
        public int MaxBarsCount = 1000;

        public IndicatorBuffer _RSI = new IndicatorBuffer();
        public IndicatorBuffer AvGain = new IndicatorBuffer();
        public IndicatorBuffer AvLoss = new IndicatorBuffer();
        public IndicatorBuffer RSIBuf = new IndicatorBuffer();
        public IndicatorBuffer RSI = new IndicatorBuffer();
        public IndicatorBuffer UpZone = new IndicatorBuffer();
        public IndicatorBuffer MdZone = new IndicatorBuffer();
        public IndicatorBuffer DnZone = new IndicatorBuffer();
        public IndicatorBuffer MaBuf = new IndicatorBuffer();
        public IndicatorBuffer MbBuf = new IndicatorBuffer();
        public override void OnInit()
        {
            SetIndicatorShortName("Traders Dynamic Index");
            Indicator_Separate_Window = true;
            SetLevel(50, Color.DarkGray, LineStyle.STYLE_DOT);
            SetLevel(68, Color.DarkGray, LineStyle.STYLE_DOT);
            SetLevel(32, Color.DarkGray, LineStyle.STYLE_DOT);

            SetIndexBuffer(0, UpZone);
            SetIndexStyle(0, DrawingStyle.DRAW_LINE, Color.Blue, LineStyle.STYLE_SOLID, 1);
            SetIndexLabel(0, "VB High");
            SetIndexBuffer(1, MdZone);
            SetIndexStyle(1, DrawingStyle.DRAW_LINE, Color.Yellow, LineStyle.STYLE_SOLID, 2);
            SetIndexLabel(1, "Market Base Line");
            SetIndexBuffer(2, DnZone);
            SetIndexStyle(2, DrawingStyle.DRAW_LINE, Color.Blue, LineStyle.STYLE_SOLID, 1);
            SetIndexLabel(2, "VB Low");
            SetIndexBuffer(3, MaBuf);
            SetIndexStyle(3, DrawingStyle.DRAW_LINE, Color.Green, LineStyle.STYLE_SOLID, 2);
            SetIndexLabel(3, "RSI Price Line");
            SetIndexBuffer(4, MbBuf);
            SetIndexStyle(4, DrawingStyle.DRAW_LINE, Color.Red, LineStyle.STYLE_SOLID, 2);
            SetIndexLabel(4, "Trade Signal Line");
        }

        public override void OnCalculate(int index)
        {
            double MA_Value;

            if (index > MaxBarsCount)
                return;

            RSIBuf[index] = GetRSI(RSI_Period, RSI_Price, index);
            MA_Value = 0;

            for(int i = index; i < index + Volatility_Band; i++)
            {
                RSI[i - index] = RSIBuf[i];
                if (Volatility_Band != 0)
                    MA_Value = MA_Value + RSIBuf[i] / Volatility_Band;
            }

            UpZone[index] = MA_Value + (1.6185 * GetStandardDeviation(RSI, Volatility_Band));
            DnZone[index] = MA_Value - (1.6185 * GetStandardDeviation(RSI, Volatility_Band));
            MdZone[index] = (UpZone[index] + DnZone[index]) / 2;
            MaBuf[index] = MAOnArray(RSIBuf, RSI_Price_Line, index);
            MbBuf[index] = MAOnArray(RSIBuf, Trade_Signal_Line, index);

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

        public double GetRSI(int period, Applied_Price PriceType, int index)
        {
            double diff, gain, loss;

            if (Bars() - index <= period || period == 0)
                return 0;

            if (Bars() - index == period + 1)
            {
                gain = 0;
                loss = 0;
                for (int i = index; i <= index + period - 1; i++)
                {
                    diff = GetAppliedPrice(Symbol(), Period(), i, PriceType) - GetAppliedPrice(Symbol(), Period(), i + 1, PriceType);
                    if (diff > 0)
                        gain += diff;
                    else
                        loss -= diff;
                }
                AvGain[index] = gain / period;
                AvLoss[index] = loss / period;
            }
            else
            {
                gain = 0;
                loss = 0;
                diff = GetAppliedPrice(Symbol(), Period(), index, PriceType) - GetAppliedPrice(Symbol(), Period(), index + 1, PriceType);
                if (diff > 0)
                    gain = diff;
                else
                    loss = -diff;
                gain = (AvGain[index + 1] * (period - 1) + gain) / period;
                loss = (AvLoss[index + 1] * (period - 1) + loss) / period;
                AvGain[index] = gain;
                AvLoss[index] = loss;

                if (loss == 0)
                    _RSI[index] = 105;
                else
                    _RSI[index] = 100 - 100 / (1 + gain / loss);
            }

            return _RSI[index];
        }

        public double GetStandardDeviation(IndicatorBuffer Array, int Count)
        {
            double sum = 0, ssum = 0, variance = 0;

            for(int i = 0; i < Count; i++)
            {
                sum += Array[i];
                ssum += Math.Pow(Array[i],2);
            }

            if(Count*(Count-1) != 0)
                variance = (ssum * Count - sum*sum) / (Count*(Count-1));

            return Math.Sqrt(variance);
        }
    }
}
