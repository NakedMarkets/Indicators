using IndicatorInterfaceCSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sessions
{
    public class Sessions : IndicatorInterface
    {
        [Separator(Label = "Common")]
        public string Separator_Common;

        [Input(Name = "Number of days")]
        public int NumberOfDays = 3;
        [Separator(Label = "Tokyo parameters")]
        public string Separator_Tokyo;
        [Input(Name = "Tokyo start hour")]
        public HoursOfDay TokyoStartHour = HoursOfDay.Hour_0;
        [Input(Name = "Tokyo start minute")]
        public MinutesOfDay TokyoStartMinute = MinutesOfDay.Minute_0;
        [Input(Name = "Tokyo end hour")]
        public HoursOfDay TokyoEndHour = HoursOfDay.Hour_8;
        [Input(Name = "Tokyo end minute")]
        public MinutesOfDay TokyoEndMinute = MinutesOfDay.Minute_0;
        [Input(Name = "Tokyo color")]
        public Color TokyoColor = Color.Yellow;
        [Separator(Label = "London parameters")]
        public string Separator_London;
        [Input(Name = "London start hour")]
        public HoursOfDay LondonStartHour = HoursOfDay.Hour_7;
        [Input(Name = "London start minute")]
        public MinutesOfDay LondonStartMinute = MinutesOfDay.Minute_0;
        [Input(Name = "London end hour")]
        public HoursOfDay LondonEndHour = HoursOfDay.Hour_16;
        [Input(Name = "London end minute")]
        public MinutesOfDay LondonEndMinute = MinutesOfDay.Minute_0;
        [Input(Name = "London color")]
        public Color LondonColor = Color.Blue;
        [Separator(Label = "New-York parameters")]
        public string Separator_USA;
        [Input(Name = "New-York start hour")]
        public  HoursOfDay NYStartHour = HoursOfDay.Hour_12;
        [Input(Name = "New-York start minute")]
        public MinutesOfDay NYStartMinute = MinutesOfDay.Minute_0;
        [Input(Name = "New-York end hour")]
        public HoursOfDay NYEndHour = HoursOfDay.Hour_20;
        [Input(Name = "New-York end minute")]
        public MinutesOfDay NYEndMinute = MinutesOfDay.Minute_0;
        [Input(Name = "New-York color")]
        public Color NYColor = Color.Green;
        [Separator(Label = "Sydney parameters")]
        public string Separator_Sydney;
        [Input(Name = "Sydney start hour")]
        public HoursOfDay SydneyStartHour = HoursOfDay.Hour_9;
        [Input(Name = "Sydney start minute")]
        public MinutesOfDay SydneyStartMinute = MinutesOfDay.Minute_0;
        [Input(Name = "Sydney end hour")]
        public HoursOfDay SydneyEndHour = HoursOfDay.Hour_18;
        [Input(Name = "Sydney start minute")]
        public MinutesOfDay SydneyEndMinute = MinutesOfDay.Minute_0;
        [Input(Name = "Sydney color")]
        public Color SydneyColor = Color.Red;

        [Separator(Label = "Sessions parameters")]
        public string Separator_visibility;
        [Input(Name = "Show Tokyo session")]
        public bool ShowTokyo = true;
        [Input(Name = "Show London session")]
        public bool ShowLondon = true;
        [Input(Name = "Show New-York session")]
        public bool ShowNY = true;
        [Input(Name = "Show Sydney session")]
        public bool ShowSydney = false;

        public override void OnInit()
        {
            SetIndicatorShortName("Trading sessions");
            Indicator_Separate_Window = false;
        }

        public override void OnAttach()
        {
            DeleteObjects();

            // The colors are set to be semi-transparent by changing the Alpha channel
            TokyoColor = Color.FromArgb(100, TokyoColor);
            LondonColor = Color.FromArgb(100, LondonColor);
            NYColor = Color.FromArgb(100, NYColor);
            SydneyColor = Color.FromArgb(100, SydneyColor);

            for (int i = 0; i < NumberOfDays; i++)
            {
                if(ShowTokyo)
                    CreateObjects("TO" + i, TokyoColor);

                if(ShowLondon)
                    CreateObjects("LO" + i, LondonColor);

                if(ShowNY)
                    CreateObjects("NY" + i, NYColor);

                if(ShowSydney)
                    CreateObjects("SY" + i, SydneyColor);
            }

            OnCalculate(0);
        }

        public override void OnCalculate(int index)
        {
            if (index != 0 || Period() >= (int)Timeframe.PERIOD_D1)
                return;

            DateTime CurrentTime = Time(0);

            for (int i = 0; i < NumberOfDays; i++)
            {
                if (ShowTokyo)
                    DrawObjects(CurrentTime, "TO" + i, TokyoStartHour, TokyoStartMinute, TokyoEndHour, TokyoEndMinute);

                if (ShowLondon)
                    DrawObjects(CurrentTime, "LO" + i, LondonStartHour, LondonStartMinute, LondonEndHour, LondonEndMinute);

                if (ShowNY)
                    DrawObjects(CurrentTime, "NY" + i, NYStartHour, NYStartMinute, NYEndHour, NYEndMinute);

                if (ShowSydney)
                    DrawObjects(CurrentTime, "SY" + i, SydneyStartHour, SydneyStartMinute, SydneyEndHour, SydneyEndMinute);

                CurrentTime = CurrentTime.AddDays(-1);
                // We skip the weekends
                while (CurrentTime.DayOfWeek == DayOfWeek.Saturday || CurrentTime.DayOfWeek == DayOfWeek.Sunday)
                    CurrentTime = CurrentTime.AddDays(-1);
            }
        }

        public void DeleteObjects()
        {
            for (int i = 0; i < NumberOfDays; i++)
            {
                ObjectDelete("TO" + i);
                ObjectDelete("LO" + i);
                ObjectDelete("NY" + i);
                ObjectDelete("SY" + i);
            }
        }

        public void CreateObjects(string ObjectName, Color ObjectColor)
        {
            ObjectCreate(ObjectName, ObjectType.OBJ_RECTANGLE, DateTime.MinValue, 0, DateTime.MinValue, 0);
            ObjectSet(ObjectName, ObjectProperty.OBJPROP_STYLE, LineStyle.STYLE_SOLID);
            ObjectSet(ObjectName, ObjectProperty.OBJPROP_COLOR, ObjectColor);
        }

        public void DrawObjects(DateTime CurrentDate, string ObjectName, HoursOfDay HourStart, MinutesOfDay MinuteStart, HoursOfDay HourEnd, MinutesOfDay MinuteEnd)
        {
            DateTime SessionStart, SessionEnd;
            int SessionStartIndex, SessionEndIndex;

            SessionStart = new DateTime(CurrentDate.Year, CurrentDate.Month, CurrentDate.Day).AddHours((int)HourStart).AddMinutes((int)MinuteStart);
            SessionEnd = new DateTime(CurrentDate.Year, CurrentDate.Month, CurrentDate.Day).AddHours((int)HourEnd).AddMinutes((int)MinuteEnd);

            SessionStartIndex = iBarShift(Symbol(), Period(), SessionStart, false);
            SessionEndIndex = iBarShift(Symbol(), Period(), SessionEnd, false);

            double HighestValue = Highest(Symbol(), Period(), Series.MODE_HIGH, SessionStartIndex - SessionEndIndex, SessionEndIndex);
            double LowestValue = Lowest(Symbol(), Period(), Series.MODE_LOW, SessionStartIndex - SessionEndIndex, SessionEndIndex);            

            ObjectSet(ObjectName, ObjectProperty.OBJPROP_TIME1, SessionStart);
            ObjectSet(ObjectName, ObjectProperty.OBJPROP_PRICE1, HighestValue);
            ObjectSet(ObjectName, ObjectProperty.OBJPROP_TIME2, SessionEnd);
            ObjectSet(ObjectName, ObjectProperty.OBJPROP_PRICE2, LowestValue);            
        }

        public enum HoursOfDay
        {
            [Description("0H")]
            Hour_0 = 0,
            [Description("1H")]
            Hour_1 = 1,
            [Description("2H")]
            Hour_2 = 2,
            [Description("3H")]
            Hour_3 = 3,
            [Description("4H")]
            Hour_4 = 4,
            [Description("5H")]
            Hour_5 = 5,
            [Description("6H")]
            Hour_6 = 6,
            [Description("7H")]
            Hour_7 = 7,
            [Description("8H")]
            Hour_8 = 8,
            [Description("9H")]
            Hour_9 = 9,
            [Description("10H")]
            Hour_10 = 10,
            [Description("11H")]
            Hour_11 = 11,
            [Description("12H")]
            Hour_12 = 12,
            [Description("13H")]
            Hour_13 = 13,
            [Description("14H")]
            Hour_14 = 14,
            [Description("15H")]
            Hour_15 = 15,
            [Description("16H")]
            Hour_16 = 16,
            [Description("17H")]
            Hour_17 = 17,
            [Description("18H")]
            Hour_18 = 18,
            [Description("19H")]
            Hour_19 = 19,
            [Description("20H")]
            Hour_20 = 20,
            [Description("21H")]
            Hour_21 = 21,
            [Description("22H")]
            Hour_22 = 22,
            [Description("23H")]
            Hour_23 = 23,
        }

        public enum MinutesOfDay
        {
            [Description("00")]
            Minute_0 = 0,
            [Description("01")]
            Minute_1 = 1,
            [Description("02")]
            Minute_2 = 2,
            [Description("03")]
            Minute_3 = 3,
            [Description("04")]
            Minute_4 = 4,
            [Description("05")]
            Minute_5 = 5,
            [Description("06")]
            Minute_6 = 6,
            [Description("07")]
            Minute_7 = 7,
            [Description("08")]
            Minute_8 = 8,
            [Description("09")]
            Minute_9 = 9,
            [Description("10")]
            Minute_10 = 10,
            [Description("11")]
            Minute_11 = 11,
            [Description("12")]
            Minute_12 = 12,
            [Description("13")]
            Minute_13 = 13,
            [Description("14")]
            Minute_14 = 14,
            [Description("15")]
            Minute_15 = 15,
            [Description("16")]
            Minute_16 = 16,
            [Description("17")]
            Minute_17 = 17,
            [Description("18")]
            Minute_18 = 18,
            [Description("19")]
            Minute_19 = 19,
            [Description("20")]
            Minute_20 = 20,
            [Description("21")]
            Minute_21 = 21,
            [Description("22")]
            Minute_22 = 22,
            [Description("23")]
            Minute_23 = 23,
            [Description("24")]
            Minute_24 = 24,
            [Description("25")]
            Minute_25 = 25,
            [Description("26")]
            Minute_26 = 26,
            [Description("27")]
            Minute_27 = 27,
            [Description("28")]
            Minute_28 = 28,
            [Description("29")]
            Minute_29 = 29,
            [Description("30")]
            Minute_30 = 30,
            [Description("31")]
            Minute_31 = 31,
            [Description("32")]
            Minute_32 = 32,
            [Description("33")]
            Minute_33 = 33,
            [Description("34")]
            Minute_34 = 34,
            [Description("35")]
            Minute_35 = 35,
            [Description("36")]
            Minute_36 = 36,
            [Description("37")]
            Minute_37 = 37,
            [Description("38")]
            Minute_38 = 38,
            [Description("39")]
            Minute_39 = 39,
            [Description("40")]
            Minute_40 = 40,
            [Description("41")]
            Minute_41 = 41,
            [Description("42")]
            Minute_42 = 42,
            [Description("43")]
            Minute_43 = 43,
            [Description("44")]
            Minute_44 = 44,
            [Description("45")]
            Minute_45 = 45,
            [Description("46")]
            Minute_46 = 46,
            [Description("47")]
            Minute_47 = 47,
            [Description("48")]
            Minute_48 = 48,
            [Description("49")]
            Minute_49 = 49,
            [Description("50")]
            Minute_50 = 50,
            [Description("51")]
            Minute_51 = 51,
            [Description("52")]
            Minute_52 = 52,
            [Description("53")]
            Minute_53 = 53,
            [Description("54")]
            Minute_54 = 54,
            [Description("55")]
            Minute_55 = 55,
            [Description("56")]
            Minute_56 = 56,
            [Description("57")]
            Minute_57 = 57,
            [Description("58")]
            Minute_58 = 58,
            [Description("59")]
            Minute_59 = 59,
        }

    }
}
