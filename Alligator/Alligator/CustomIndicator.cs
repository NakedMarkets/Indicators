using IndicatorInterfaceCSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Alligator
{
    public class Alligator : IndicatorInterface
    {
        [Separator(Label = "Common")]
        public string Separator_Common;
        [Input(Name = "Jaws period")]
        public int JawsPeriod = 13;
        [Input(Name = "Jaws shift")]
        public int JawsShift = 8;
        [Input(Name = "Teeth period")]
        public int TeethPeriod = 8;
        [Input(Name = "Teeth shift")]
        public int TeethShift = 5;
        [Input(Name = "Lips period")]
        public int LipsPeriod = 5;
        [Input(Name = "Lips shift")]
        public int LipsShift = 3;
        [Input(Name = "MA Type")]
        public MA_Method MAType;
        [Input(Name = "Apply to price")]
        public Applied_Price ApplyToPriceParameter;

        public IndicatorBuffer Jaws = new IndicatorBuffer();
        public IndicatorBuffer Teeth = new IndicatorBuffer();
        public IndicatorBuffer Lips = new IndicatorBuffer();

        public override void OnInit()
        {
            SetIndicatorShortName("Alligator (B. Williams)");
            Indicator_Separate_Window = false;
            SetIndexBuffer(0, Jaws);
            SetIndexStyle(0, DrawingStyle.DRAW_LINE, Color.Blue, LineStyle.STYLE_SOLID, 1);
            SetIndexLabel(0, "Jaws");
            SetIndexBuffer(1, Teeth);
            SetIndexStyle(1, DrawingStyle.DRAW_LINE, Color.Red, LineStyle.STYLE_SOLID, 1);
            SetIndexLabel(1, "Teeth");
            SetIndexBuffer(2, Lips);
            SetIndexStyle(2, DrawingStyle.DRAW_LINE, Color.Lime, LineStyle.STYLE_SOLID, 1);
            SetIndexLabel(2, "Lips");
        }

        public override void OnAttach()
        {
            SetIndexShift(0, JawsShift);
            SetIndexShift(1, TeethShift);
            SetIndexShift(2, LipsShift);
        }

        public override void OnCalculate(int index)
        {
            Jaws[index] = GetMA(Symbol(), Period(), index, 0, JawsPeriod, MAType, ApplyToPriceParameter, Jaws[index + 1]);
            Teeth[index] = GetMA(Symbol(), Period(), index, 0, TeethPeriod, MAType, ApplyToPriceParameter, Teeth[index + 1]);
            Lips[index] = GetMA(Symbol(), Period(), index, 0, LipsPeriod, MAType, ApplyToPriceParameter, Lips[index + 1]);
        }

    }
}
