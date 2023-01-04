using IndicatorInterfaceCSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SupportResistance
{
    public class SupportResistance : IndicatorInterface
    {
        /* Be careful, this indicator is repainting */

        [Separator(Label = "Common")]
        public string Separator_Common;
        [Input(Name = "Max Num Bars")]
        public int MaxNumBars = 500;

        public IndicatorBuffer Buffer_Resistance = new IndicatorBuffer();
        public IndicatorBuffer Buffer_Support = new IndicatorBuffer();
        public override void OnInit()
        {
            SetIndicatorShortName("Support and Resistance");
            Indicator_Separate_Window = false;
            SetIndexBuffer(0, Buffer_Resistance);
            SetIndexStyle(0, DrawingStyle.DRAW_ARROW, Color.Red);
            SetIndexArrow(0, 119);
            SetIndexLabel(0, "Resistance");
            SetIndexBuffer(1, Buffer_Support);
            SetIndexStyle(1, DrawingStyle.DRAW_ARROW, Color.Blue);
            SetIndexArrow(1, 119);
            SetIndexLabel(1, "Support");

        }

        public override void OnCalculate(int index)
        {
            if (index > MaxNumBars)
                return;

            double ResistanceVal = GetFractals(index, 1);
            
            if(ResistanceVal > 0)
            {
                Buffer_Resistance[index] = High(index + 3);
                Buffer_Resistance[index + 1] = High(index + 3);
                Buffer_Resistance[index + 2] = High(index + 3);
                Buffer_Resistance[index + 3] = High(index + 3);
            }
            else
                Buffer_Resistance[index] = Buffer_Resistance[index + 1];

            double SupportVal = GetFractals(index, 2);

            if (SupportVal > 0)
            {
                Buffer_Support[index] = Low(index + 3);
                Buffer_Support[index + 1] = Low(index + 3);
                Buffer_Support[index + 2] = Low(index + 3);
                Buffer_Support[index + 3] = Low(index + 3);
            }
            else
                Buffer_Support[index] = Buffer_Support[index + 1];
        }

        public double GetFractals(int index, int mode)
        {
            double UpBuff, DownBuff;

            bool Condition1 =
                (High(index + 3) > High(index + 5)) &&
                (High(index + 3) > High(index + 4)) &&
                (High(index + 3) > High(index + 2)) &&
                (High(index + 3) > High(index + 1));

            bool Condition2 =
                (High(index + 4) > High(index + 6)) &&
                (High(index + 4) > High(index + 5)) &&
                (High(index + 4) == High(index + 3)) &&
                (High(index + 3) > High(index + 2)) &&
                (High(index + 3) > High(index + 1));

            bool Condition3 =
                (High(index + 5) > High(index + 7)) &&
                (High(index + 5) > High(index + 6)) &&
                (High(index + 3) >= High(index + 5)) &&
                (High(index + 4) <= High(index + 5)) &&
                (High(index + 3) >= High(index + 4)) &&
                (High(index + 3) > High(index + 2)) &&
                (High(index + 3) > High(index + 1));

            bool Condition4 =
                (High(index + 3) == High(index + 4)) &&
                (High(index + 4) == High(index + 5)) &&
                (High(index + 5) == High(index + 6)) &&
                (High(index + 6) > High(index + 7)) &&
                (High(index + 6) > High(index + 8)) &&
                (High(index + 3) > High(index + 2)) &&
                (High(index + 3) > High(index + 1));

            bool Condition5 =
                (High(index + 3) == High(index + 4)) &&
                (High(index + 4) == High(index + 5)) &&
                (High(index + 5) == High(index + 6)) &&
                (High(index + 6) == High(index + 7)) &&
                (High(index + 7) > High(index + 8)) &&
                (High(index + 7) > High(index + 9)) &&
                (High(index + 3) > High(index + 2)) &&
                (High(index + 3) > High(index + 1));

            if (Condition1 || Condition2 || Condition3 || Condition4 || Condition5)
                UpBuff = High(index + 3);
            else
                UpBuff = 0;

            Condition1 =
                (Low(index + 3) < Low(index + 5)) &&
                (Low(index + 3) < Low(index + 4)) &&
                (Low(index + 3) < Low(index + 2)) &&
                (Low(index + 3) < Low(index + 1));

            Condition2 =
                (Low(index + 4) < Low(index + 6)) &&
                (Low(index + 4) < Low(index + 5)) &&
                (Low(index + 4) == Low(index + 3)) &&
                (Low(index + 3) < Low(index + 2)) &&
                (Low(index + 3) < Low(index + 1));

            Condition3 =
                (Low(index + 5) < Low(index + 7)) &&
                (Low(index + 5) < Low(index + 6)) &&
                (Low(index + 3) <= Low(index + 5)) &&
                (Low(index + 4) <= Low(index + 5)) &&
                (Low(index + 3) <= Low(index + 4)) &&
                (Low(index + 3) < Low(index + 2)) &&
                (Low(index + 3) < Low(index + 1));

            Condition4 =
                (Low(index + 3) == Low(index + 4)) &&
                (Low(index + 4) == Low(index + 5)) &&
                (Low(index + 5) == Low(index + 6)) &&
                (Low(index + 6) < Low(index + 7)) &&
                (Low(index + 6) < Low(index + 8)) &&
                (Low(index + 3) < Low(index + 2)) &&
                (Low(index + 3) < Low(index + 1));

            Condition5 =
                (Low(index + 3) == Low(index + 4)) &&
                (Low(index + 4) == Low(index + 5)) &&
                (Low(index + 5) == Low(index + 6)) &&
                (Low(index + 6) == Low(index + 7)) &&
                (Low(index + 7) < Low(index + 8)) &&
                (Low(index + 7) < Low(index + 9)) &&
                (Low(index + 3) < Low(index + 2)) &&
                (Low(index + 3) < Low(index + 1));

            if (Condition1 || Condition2 || Condition3 || Condition4 || Condition5)
                DownBuff = Low(index + 3);
            else
                DownBuff = 0;

            if (mode == 1)
                return UpBuff;
            else
                return DownBuff;
        }
    }
}
