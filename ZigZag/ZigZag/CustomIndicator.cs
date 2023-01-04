using IndicatorInterfaceCSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZigZag
{
    public class ZigZag : IndicatorInterface
    {
        [Separator(Label = "Common")]
        public string Separator_Common;
        [Input(Name = "Depth")]
        public int Depth = 12;

        public IndicatorBuffer zz = new IndicatorBuffer();
        public IndicatorBuffer zzH = new IndicatorBuffer();
        public IndicatorBuffer zzL = new IndicatorBuffer();
        public IndicatorBuffer zzHPos = new IndicatorBuffer();
        public IndicatorBuffer zzLPos = new IndicatorBuffer();
        public override void OnInit()
        {
            SetIndicatorShortName("ZigZag");
            Indicator_Separate_Window = false;
            SetIndexBuffer(0, zz);
            SetIndexStyle(0, DrawingStyle.DRAW_LINE, Color.Yellow, LineStyle.STYLE_SOLID, 2);
            SetIndexLabel(0, "ZigZag");
        }

        public override void OnCalculate(int index)
        {
            int cpt, Pos, LastHighPos, LastLowPos, CurHighPos, CurLowPos;
            double CurLow, CurHigh, LastHigh, LastLow, Cmin, Cmax;

            if (index + Depth >= Bars() || index != 0)
                return;

            cpt = 0;
            Pos = Bars() - 1;
            for(int i = 0; i < Bars(); i++)
            {
                if(zz[i] != 0)
                {
                    cpt++;
                    if(cpt == 2)
                    {
                        Pos = i - 1;
                        break;
                    }
                }
            }

            if( Pos == Bars() - 1)
            {
                LastHighPos = Pos;
                LastLowPos = Pos;
            }
            else
            {
                LastHighPos = (int)Math.Round(zzHPos[Pos + 1]);
                LastLowPos = (int)Math.Round(zzLPos[Pos + 1]);
            }

            LastLow = Low(LastLowPos);
            LastHigh = High(LastHighPos);

            for(int i = Pos; i >= 0; i--)
            {
                zz[i] = 0;
                zzH[i] = 0;
                zzL[i] = 0;
                CurLowPos = iLowest(Symbol(), Period(), Series.MODE_LOW, Depth + 1, i);
                CurLow = Low(CurLowPos);
                CurHighPos = iHighest(Symbol(), Period(), Series.MODE_HIGH, Depth + 1, i);
                CurHigh = High(CurHighPos);


                if (CurLow >= LastLow)
                    LastLow = CurLow;
                else
                {
                    if(LastHighPos > CurLowPos)
                    {
                        zzL[CurLowPos] = CurLow;
                        Cmin = Double.MaxValue;
                        Pos = LastHighPos;

                        for(int j = LastHighPos; j >= CurLowPos; j--)
                        {
                            if (zzL[j] == 0)
                                continue;
                            if(zzL[j] < Cmin)
                            {
                                Cmin = zzL[j];
                                Pos = j;
                            }
                            zz[j] = 0;
                        }
                        zz[Pos] = Cmin;
                    }
                    LastLowPos = CurLowPos;
                    LastLow = CurLow;
                }

                if (CurHigh <= LastHigh)
                    LastHigh = CurHigh;
                else
                {
                    if (LastLowPos > CurHighPos)
                    {
                        zzH[CurHighPos] = CurHigh;
                        Cmax = Double.MinValue;
                        Pos = LastLowPos;

                        for (int j = LastLowPos; j >= CurHighPos; j--)
                        {
                            if (zzH[j] == 0)
                                continue;
                            if (zzH[j] > Cmax)
                            {
                                Cmax = zzH[j];
                                Pos = j;
                            }
                            zz[j] = 0;
                        }
                        zz[Pos] = Cmax;
                    }
                    LastHighPos = CurHighPos;
                    LastHigh = CurHigh;
                }

                zzHPos[i] = LastHighPos;
                zzLPos[i] = LastLowPos;

            }
        }

    }
}
