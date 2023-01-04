using IndicatorInterfaceCSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Volume
{
    public class Volume : IndicatorInterface
    {
        public IndicatorBuffer VolumeBuffer = new IndicatorBuffer();
        public override void OnInit()
        {
            SetIndicatorShortName("Volume");
            Indicator_Separate_Window = true;
            SetIndexBuffer(0, VolumeBuffer);
            SetIndexStyle(0, DrawingStyle.DRAW_HISTOGRAM, Color.Green);
            SetIndexLabel(0, "Volume");
        }

        public override void OnCalculate(int index)
        {
            VolumeBuffer[index] = Volume(index);
        }

    }
}
