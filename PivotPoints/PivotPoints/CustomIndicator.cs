using IndicatorInterfaceCSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;

namespace PivotPoints
{
    public class PivotPoints : IndicatorInterface
    {
        [Separator(Label = "Common")]
        public string Separator_Common;
        [Input(Name = "Pivot Type")]
        public PivotType PivotTypeParameter;
        [Input(Name = "Continuous")]
        public bool Continuous;
        [Input(Name = "Show Mid Points")]
        public bool ShowMidpoints;

        public IndicatorBuffer PP = new IndicatorBuffer();
        public IndicatorBuffer R1 = new IndicatorBuffer();
        public IndicatorBuffer R2 = new IndicatorBuffer();
        public IndicatorBuffer R3 = new IndicatorBuffer();
        public IndicatorBuffer S1 = new IndicatorBuffer();
        public IndicatorBuffer S2 = new IndicatorBuffer();
        public IndicatorBuffer S3 = new IndicatorBuffer();
        public IndicatorBuffer M1 = new IndicatorBuffer();
        public IndicatorBuffer M2 = new IndicatorBuffer();
        public IndicatorBuffer M3 = new IndicatorBuffer();
        public IndicatorBuffer M4 = new IndicatorBuffer();

        public override void OnInit()
        {
            SetIndicatorShortName("Pivot Points");
            Indicator_Separate_Window = false;

            SetIndexBuffer(0, R3);
            SetIndexStyle(0, DrawingStyle.DRAW_LINE, Color.Green, LineStyle.STYLE_SOLID, 1);
            SetIndexLabel(0, "R3");

            SetIndexBuffer(1, R2);
            SetIndexStyle(1, DrawingStyle.DRAW_LINE, Color.Blue, LineStyle.STYLE_DASHDOT, 1);
            SetIndexLabel(1, "R2");

            SetIndexBuffer(2, R1);
            SetIndexStyle(2, DrawingStyle.DRAW_LINE, Color.Red, LineStyle.STYLE_DOT, 1);
            SetIndexLabel(2, "R1");

            SetIndexBuffer(3, PP);
            SetIndexStyle(3, DrawingStyle.DRAW_LINE, Color.Maroon, LineStyle.STYLE_DOT, 1);
            SetIndexLabel(3, "PP");

            SetIndexBuffer(4, S1);
            SetIndexStyle(4, DrawingStyle.DRAW_LINE, Color.Red, LineStyle.STYLE_DOT, 1);
            SetIndexLabel(4, "S1");

            SetIndexBuffer(5, S2);
            SetIndexStyle(5, DrawingStyle.DRAW_LINE, Color.Blue, LineStyle.STYLE_DASHDOT, 1);
            SetIndexLabel(5, "S2");

            SetIndexBuffer(6, S3);
            SetIndexStyle(6, DrawingStyle.DRAW_LINE, Color.Green, LineStyle.STYLE_DASHDOT, 1);
            SetIndexLabel(6, "S3");

            SetIndexBuffer(7, M1);
            SetIndexStyle(7, DrawingStyle.DRAW_LINE, Color.Gray, LineStyle.STYLE_DOT, 1);
            SetIndexLabel(7, "M1");

            SetIndexBuffer(8, M2);
            SetIndexStyle(8, DrawingStyle.DRAW_LINE, Color.Gray, LineStyle.STYLE_DOT, 1);
            SetIndexLabel(8, "M2");

            SetIndexBuffer(9, M3);
            SetIndexStyle(9, DrawingStyle.DRAW_LINE, Color.Gray, LineStyle.STYLE_DOT, 1);
            SetIndexLabel(9, "M3");

            SetIndexBuffer(10, M4);
            SetIndexStyle(10, DrawingStyle.DRAW_LINE, Color.Gray, LineStyle.STYLE_DOT, 1);
            SetIndexLabel(10, "M4");
        }

        public override void OnCalculate(int index)
        {
            DateTime time, time1;
            int day, week, day1, week1, month, month1, year, year1, i;
            double h, l, c, _pp, _R1, _S1, _R2, _S2;
            bool NewPeriod = false;

            if(index >= Bars() - 2)
            {
                SetPivotPoints(index, 0);
                return;
            }

            time = Time(index);
            time1 = Time(index + 1);
            day = time.Day;
            day1 = time1.Day;
            week = GetWeekNumber(time);
            week1 = GetWeekNumber(time1);
            month = time.Month;
            month1 = time1.Month;
            year = time.Year;
            year1 = time1.Year;

            switch (PivotTypeParameter)
            {
                case PivotType.Daily:
                    if (Period() >= (int)Timeframe.PERIOD_D1) return;
                    NewPeriod = day != day1;
                    break;
                case PivotType.Weekly:
                    if (Period() >= (int)Timeframe.PERIOD_W1) return;
                    NewPeriod = week != week1;
                    break;
                case PivotType.Monthly:
                    if (Period() >= (int)Timeframe.PERIOD_MN1) return;
                    NewPeriod = month != month1;
                    break;
                case PivotType.Yearly:
                    NewPeriod = year != year1;
                    break;
            }

            if (!NewPeriod)
            {
                PP[index] = PP[index + 1];
                R1[index] = R1[index + 1];
                R2[index] = R2[index + 1];
                R3[index] = R3[index + 1];
                S1[index] = S1[index + 1];
                S2[index] = S2[index + 1];
                S3[index] = S3[index + 1];
                M1[index] = M1[index + 1];
                M2[index] = M2[index + 1];
                M3[index] = M3[index + 1];
                M4[index] = M4[index + 1];
                return;
            }

            i = index + 1;
            _pp = PP[i + 1];

            if (!Continuous)
                SetPivotPoints(index + 1, 0);

            h = High(i);
            l = Low(i);
            c = Close(i);
            i++;

            while(PP[i] == _pp && i < Bars())
            {
                h = Math.Max(High(i), h);
                l = Math.Min(Low(i), l);
                i++;
            }

            _pp = (h + l + c) / 3;
            _R1 = 2 * _pp - l;
            _S1 = 2 * _pp - h;
            _R2 = _pp + (_R1 - _S1);
            _S2 = _pp - (_R1 - _S1);
            PP[index] = _pp;
            R1[index] = _R1;
            R2[index] = _R2;
            R3[index] = h + 2 * (_pp - l);
            S1[index] = _S1;
            S2[index] = _S2;
            S3[index] = l - 2 * (h - _pp);

            if (ShowMidpoints)
            {
                M1[index] = (_S1 + _S2) / 2;
                M2[index] = (_S1 + _pp) / 2;
                M3[index] = (_R1 + _pp) / 2;
                M4[index] = (_R1 + _R2) / 2;
            }
        }

        public void SetPivotPoints(int index, double value)
        {
            PP[index] = value;
            R1[index] = value;
            R2[index] = value;
            R3[index] = value;
            S1[index] = value;
            S2[index] = value;
            S3[index] = value;
            M1[index] = value;
            M2[index] = value;
            M3[index] = value;
            M4[index] = value;
        }

        public int GetWeekNumber(DateTime Date)
        {
            CultureInfo ciCurr = CultureInfo.CurrentCulture;
            int weekNum = ciCurr.Calendar.GetWeekOfYear(Date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            return weekNum;
        }

        public enum PivotType
        {
            Daily,
            Weekly,
            Monthly,
            Yearly
        }
    }
}
