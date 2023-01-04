using IndicatorInterfaceCSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Envelopes
{
    public class Envelopes : IndicatorInterface
    {
        [Separator(Label = "Common")]
        public string Separator_Common;
        [Input(Name = "Period")]
        public int period = 14;
        [Input(Name = "Deviation")]
        public double Deviation = 0.1;
        [Input(Name = "MA Type")]
        public MA_Method MAType;
        [Input(Name = "Apply to price")]
        public Applied_Price ApplyToPriceParameter;

        public IndicatorBuffer Env_UP = new IndicatorBuffer();
        public IndicatorBuffer Env_DOWN = new IndicatorBuffer();
        public IndicatorBuffer MA = new IndicatorBuffer();
        public override void OnInit()
        {
            SetIndicatorShortName("Envelopes");
            Indicator_Separate_Window = false;
            SetIndexBuffer(0, Env_UP);
            SetIndexStyle(0, DrawingStyle.DRAW_LINE, Color.Blue);
            SetIndexLabel(0, "Envelope Up");
            SetIndexBuffer(1, Env_DOWN);
            SetIndexStyle(1, DrawingStyle.DRAW_LINE, Color.Blue);
            SetIndexLabel(1, "Envelope Down");
        }

        public override void OnCalculate(int index)
        {
            MA[index] = GetMA(Symbol(), Period(), index, 0, period, MAType, ApplyToPriceParameter,  MA[index + 1]);
            Env_UP[index] = (1 + Deviation / 100) * MA[index];
            Env_DOWN[index] = (1 - Deviation / 100) * MA[index];
        }

    }
}
