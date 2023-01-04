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
        [Separator(Label = "Asia parameters")]
        public string Separator_Asia;
        [Input(Name = "Asia start hour")]
        public HoursOfDay AsiaStart = HoursOfDay.Hour_0;
        [Input(Name = "Asia end hour")]
        public HoursOfDay AsiaEnd = HoursOfDay.Hour_8;
        [Input(Name = "Asia color")]
        public Color AsiaColor = Color.Yellow;
        [Separator(Label = "Europe parameters")]
        public string Separator_Europe;
        [Input(Name = "Europ start hour")]
        public HoursOfDay EurStart = HoursOfDay.Hour_7;
        [Input(Name = "Europ end hour")]
        public HoursOfDay EurEnd = HoursOfDay.Hour_16;
        [Input(Name = "Europe color")]
        public Color EurColor = Color.Blue;
        [Separator(Label = "USA parameters")]
        public string Separator_USA;
        [Input(Name = "USA start hour")]
        public  HoursOfDay USAStart = HoursOfDay.Hour_12;
        [Input(Name = "USA end hour")]
        public HoursOfDay USAEnd = HoursOfDay.Hour_20;
        [Input(Name = "USA color")]
        public Color USAColor = Color.Green;

        public override void OnInit()
        {
            SetIndicatorShortName("Trading sessions");
            Indicator_Separate_Window = false;
        }

        public override void OnAttach()
        {
            DeleteObjects();

            // The colors are set to be semi-transparent by changing the Alpha channel
            AsiaColor = Color.FromArgb(100, AsiaColor);
            EurColor = Color.FromArgb(100, EurColor);
            USAColor = Color.FromArgb(100, USAColor);

            for (int i = 0; i < NumberOfDays; i++)
            {
                CreateObjects("AS" + i, AsiaColor);
                CreateObjects("EU" + i, EurColor);
                CreateObjects("US" + i, USAColor);
            }
        }

        public override void OnCalculate(int index)
        {
            if (index != 0 || Period() >= (int)Timeframe.PERIOD_D1)
                return;

            DateTime CurrentTime = Time(0);

            for (int i = 0; i < NumberOfDays; i++)
            {
                DrawObjects(CurrentTime, "AS" + i, AsiaStart, AsiaEnd);
                DrawObjects(CurrentTime, "EU" + i, EurStart, EurEnd);
                DrawObjects(CurrentTime, "US" + i, USAStart, USAEnd);
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
                ObjectDelete("AS" + i);
                ObjectDelete("EU" + i);
                ObjectDelete("US" + i);
            }

            ObjectDelete("ASup");
            ObjectDelete("ASdn");
            ObjectDelete("EUup");
            ObjectDelete("EUdn");
            ObjectDelete("USup");
            ObjectDelete("USdn");
        }

        public void CreateObjects(string ObjectName, Color ObjectColor)
        {
            ObjectCreate(ObjectName, ObjectType.OBJ_RECTANGLE, DateTime.MinValue, 0, DateTime.MinValue, 0);
            ObjectSet(ObjectName, ObjectProperty.OBJPROP_STYLE, LineStyle.STYLE_SOLID);
            ObjectSet(ObjectName, ObjectProperty.OBJPROP_COLOR, ObjectColor);
        }

        public void DrawObjects(DateTime CurrentDate, string no, HoursOfDay HourStart, HoursOfDay HourEnd)
        {
            DateTime SessionStart, SessionEnd;
            int SessionStartIndex, SessionEndIndex;

            SessionStart = new DateTime(CurrentDate.Year, CurrentDate.Month, CurrentDate.Day).AddHours((int)HourStart);
            SessionEnd = new DateTime(CurrentDate.Year, CurrentDate.Month, CurrentDate.Day).AddHours((int)HourEnd);

            SessionStartIndex = iBarShift(Symbol(), Period(), SessionStart, false);
            SessionEndIndex = iBarShift(Symbol(), Period(), SessionEnd, false);

            double HighestValue = Highest(Symbol(), Period(), Series.MODE_HIGH, SessionStartIndex - SessionEndIndex, SessionEndIndex);
            double LowestValue = Lowest(Symbol(), Period(), Series.MODE_LOW, SessionStartIndex - SessionEndIndex, SessionEndIndex);            

            ObjectSet(no, ObjectProperty.OBJPROP_TIME1, SessionStart);
            ObjectSet(no, ObjectProperty.OBJPROP_PRICE1, HighestValue);
            ObjectSet(no, ObjectProperty.OBJPROP_TIME2, SessionEnd);
            ObjectSet(no, ObjectProperty.OBJPROP_PRICE2, LowestValue);            
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

    }
}
