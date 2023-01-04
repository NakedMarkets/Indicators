using IndicatorInterfaceCSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fractals
{
    public class Fractals : IndicatorInterface
    {
        public IndicatorBuffer UpBuff = new IndicatorBuffer();
        public IndicatorBuffer DownBuff = new IndicatorBuffer();

        int Range = 3;

        public override void OnInit()
        {
            SetIndicatorShortName("Fractals");
            Indicator_Separate_Window = false;

            SetIndexBuffer(0, UpBuff);
            SetIndexStyle(0, DrawingStyle.DRAW_ARROW, Color.Green);
            SetIndexArrow(0, 217, 0, 10);
            SetIndexLabel(0, "Fractal Up");
            SetIndexBuffer(1, DownBuff);
            SetIndexStyle(1, DrawingStyle.DRAW_ARROW, Color.Red);
            SetIndexArrow(1, 218, 0, 0);
            SetIndexLabel(1, "Fractal Down");
        }

        public override void OnCalculate(int index)
        {
            int center = 3;
            double cur = 0;
            bool found;

            // Fractal Up
            cur = High(index + center - 1);
            if (cur > High(index) && cur >= High(index + 4) && cur > High(index + 1) && cur >= High(index + 3))
                found = true;
            else
                found = false;

            if (found && cur == High(index + 4) && !IsHigh(cur, index + 4))
                found = false;

            if (found && cur == High(index + 3) && !IsHigh(cur, index + 3))
                found = false;

            if (!found)
            {
                if (cur > High(index) && cur > High(index + 1) && cur == High(index + 3) && cur == High(index + 4))
                    found = true;
                if (found && !IsHigh(cur, index + 4))
                    found = false;
            }

            if (found)
            {
                UpBuff[index + center - 1] = High(index + center - 1);
                UpBuff[index + center] = 0;
            }
            else
            {
                UpBuff[index + center - 1] = 0;
            }

            if (UpBuff[index + 1] > 0)
                UpBuff[index + 1] = 0;


            // Fractal down
            cur = Low(index + center - 1);
            if (cur < Low(index) && cur <= Low(index + 4) && cur < Low(index + 1) && cur <= Low(index + 3))
                found = true;
            else
                found = false;

            if (found && cur == Low(index + 4) && !IsLow(cur, index + 4))
                found = false;

            if (found && cur == Low(index + 3) && !IsLow(cur, index + 3))
                found = false;

            if (!found)
            {
                if (cur < Low(index) && cur < Low(index + 1) && cur == Low(index + 3) && cur == Low(index + 4))
                    found = true;
                if (found && !IsLow(cur, index + 4))
                    found = false;
            }

            if (found)
            {
                DownBuff[index + center - 1] = Low(index + center - 1);
                DownBuff[index + center] = 0;
            }
            else
            {
                DownBuff[index + center - 1] = 0;
            }

            if (DownBuff[index + 1] > 0)
                DownBuff[index + 1] = 0;
        }

        public bool IsSubLow(double Lowest, int IndexLowest, double MLowest)
        {
            bool Flag = true;
            bool Found = false;

            for (int i = IndexLowest + 1; i < IndexLowest + Range; i++)
            {
                if(Flag && !Found)
                {
                    if (Low(i) >= Lowest)
                        Found = true;
                    if (Low(i) < Lowest && Low(i) > MLowest)
                        Found = true;
                    if (Low(i) <= MLowest)
                        Flag = false;
                }
            }
            return Found;
        }

        public bool IsLow(double Lowest, int IndexLowest)
        {
            bool Flag = true;
            bool Found = false;

            for (int i = IndexLowest + 1; i < IndexLowest + Range; i++)
            {
                if (Flag && !Found)
                {
                    if (Low(i) > Lowest)
                        Found = IsSubLow(Low(i), i, Lowest);

                    if (Low(i) < Lowest)
                        Flag = false;
                }
            }
            return Found;
        }

        public bool IsSubHigh(double Highest, int IndexHighest, double MHighest)
        {
            bool Flag = true;
            bool Found = false;

            for (int i = IndexHighest + 1; i < IndexHighest + Range; i++)
            {
                if (Flag && !Found)
                {
                    if (High(i) <= Highest)
                        Found = true;
                    if (High(i) > Highest && High(i) < MHighest)
                        Found = true;
                    if (High(i) >= MHighest)
                        Flag = false;
                }
            }
            return Found;
        }

        public bool IsHigh(double Highest, int IndexHighest)
        {
            bool Flag = true;
            bool Found = false;

            for (int i = IndexHighest + 1; i < IndexHighest + Range; i++)
            {
                if (Flag && !Found)
                {
                    if (High(i) < Highest)
                        Found = IsSubHigh(High(i), i, Highest);

                    if (High(i) > Highest)
                        Flag = false;
                }
            }
            return Found;
        }

    }
}
