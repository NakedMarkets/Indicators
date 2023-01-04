using IndicatorInterfaceCSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PriceDailyRange
{
    public class PriceDailyRange : IndicatorInterface
    {
        [Separator(Label = "Common")]
        public string Separator_Common;
        [Input(Name = "Delta points")]
        public int DeltaParam = 0;
        [Input(Name = "Start of day shift (hours)")]
        public int TimeShiftParam = 0;
        [Input(Name = "Apply to price")]
        public Applied_Price ApplytoPrice;

        public IndicatorBuffer DailyPriceHiBuf = new IndicatorBuffer();
        public IndicatorBuffer DailyPriceLoBuf = new IndicatorBuffer();
        public override void OnInit()
        {
            SetIndicatorShortName("Price Daily Range");
            Indicator_Separate_Window = false;
            SetIndexBuffer(0, DailyPriceHiBuf);
            SetIndexStyle(0, DrawingStyle.DRAW_LINE, Color.Blue);
            SetIndexLabel(0, "Up Band");
            SetIndexBuffer(1, DailyPriceLoBuf);
            SetIndexStyle(1, DrawingStyle.DRAW_LINE, Color.Red);
            SetIndexLabel(1, "Down Band");
        }

        public override void OnCalculate(int index)
        {
            bool IsChangedHi, IsChangedLo;
            double NewHi, NewLo, NowDate;
            int i;

            //DateTime TimeShift = new DateTime()
            if (index == 0)
                return;

            if (index >= Bars() || index < 0)
            {
                DailyPriceLoBuf[index] = 0;
                DailyPriceHiBuf[index] = 0;
            } else
            if (DailyPriceLoBuf[index] == 0)
            {
                IsChangedHi = false;
                IsChangedLo = false;
                NewHi = GetPriceHigh(index);
                NewLo = GetPriceLow(index);

                i = index + 1;

                if(ShiftedDate(index, TimeShiftParam) == ShiftedDate(i, TimeShiftParam) && i < Bars())
                {
                    if (DailyPriceHiBuf[i] < NewHi)
                        IsChangedHi = true;

                    if (DailyPriceLoBuf[i] > NewLo)
                        IsChangedLo = true;

                    i = index;

                    if(IsChangedHi && IsChangedLo)
                    {
                        while(ShiftedDate(index, TimeShiftParam) == ShiftedDate(i, TimeShiftParam) && i < Bars())
                        {
                            if (IsChangedHi)
                                DailyPriceHiBuf[i] = NewHi;
                            if (IsChangedLo)
                                DailyPriceLoBuf[i] = NewLo;
                            i = i + 1;
                        }
                    }

                    DailyPriceHiBuf[index] = DailyPriceHiBuf[index + 1];
                    DailyPriceLoBuf[index] = DailyPriceLoBuf[index + 1];
                }
                else
                {
                    DailyPriceHiBuf[index] = NewHi;
                    DailyPriceLoBuf[index] = NewLo;
                }
            }
        }

        public double GetPriceHigh(int index)
        {
            double CurOpen, CurClose;

            switch (ApplytoPrice)
            {
                case Applied_Price.PRICE_HIGH:
                    CurOpen = Open(index);
                    CurClose = Close(index);
                    return CurOpen > CurClose ? CurOpen + (DeltaParam * Point()) : CurClose + (DeltaParam * Point());
                case Applied_Price.PRICE_LOW:
                    return High(index) + (DeltaParam * Point());
                default:
                    return 0;
            }
        }

        public double GetPriceLow(int index)
        {
            double CurOpen, CurClose;

            switch (ApplytoPrice)
            {
                case Applied_Price.PRICE_HIGH:
                    CurOpen = Open(index);
                    CurClose = Close(index);
                    return CurOpen < CurClose ? CurOpen - (DeltaParam * Point()) : CurClose - (DeltaParam * Point());
                case Applied_Price.PRICE_LOW:
                    return Low(index) - (DeltaParam * Point());
                default:
                    return 0;
            }
        }

        public DateTime ShiftedDate(int index, double shift)
        {
            return Time(index).AddHours(shift);
        }
    }
}
