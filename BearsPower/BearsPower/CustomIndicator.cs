﻿using IndicatorInterfaceCSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BearsPower
{
    public class BearsPower : IndicatorInterface
    {
        [Separator(Label = "Common")]
        public string Separator_Common;
        [Input(Name = "Period")]
        public int period = 13;

        public IndicatorBuffer Buffer = new IndicatorBuffer();
        public IndicatorBuffer Bears = new IndicatorBuffer();

        public override void OnInit()
        {
            SetIndicatorShortName("Bears Power");
            Indicator_Separate_Window = true;
            SetLevel(0, Color.Gray, LineStyle.STYLE_DOT);
            SetIndexBuffer(0, Bears);
            SetIndexStyle(0, DrawingStyle.DRAW_HISTOGRAM, Color.Red);
            SetIndexLabel(0, "Bears Power");
        }

        public override void OnCalculate(int index)
        {
            Buffer[index] = GetMA(Symbol(), Period(), index, 0, period, MA_Method.MODE_EMA, Applied_Price.PRICE_CLOSE, Buffer[index + 1]);
            Bears[index] = Low(index) - Buffer[index];
        }

    }
}
