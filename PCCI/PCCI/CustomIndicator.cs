using IndicatorInterfaceCSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PCCI
{
    public class PCCI : IndicatorInterface
    {
        public IndicatorBuffer PCCIBuffer = new IndicatorBuffer();
        public override void OnInit()
        {
            SetIndicatorShortName("PCCI");
            Indicator_Separate_Window = true;
            SetIndexBuffer(0, PCCIBuffer);
            SetIndexStyle(0, DrawingStyle.DRAW_LINE, Color.Lime, LineStyle.STYLE_SOLID, 2);
            SetIndexLabel(0, "PCCI");
        }

        public override void OnCalculate(int index)
        {
            PCCIBuffer[index] =
               0.2447098565978 * Close(index + 0)
               + 0.2313977400697 * Close(index + 1)
               + 0.2061379694732 * Close(index + 2)
               + 0.1716623034064 * Close(index + 3)
               + 0.1314690790360 * Close(index + 4)
               + 0.0895038754956 * Close(index + 5)
               + 0.0496009165125 * Close(index + 6)
               + 0.01502270569607 * Close(index + 7)
               - 0.01188033734430 * Close(index + 8)
               - 0.02989873856137 * Close(index + 9)
               - 0.0389896710490 * Close(index + 10)
               - 0.0401411362639 * Close(index + 11)
               - 0.0351196808580 * Close(index + 12)
               - 0.02611613850342 * Close(index + 13)
               - 0.01539056955666 * Close(index + 14)
               - 0.00495353651394 * Close(index + 15)
               + 0.00368588764825 * Close(index + 16)
               + 0.00963614049782 * Close(index + 17)
               + 0.01265138888314 * Close(index + 18)
               + 0.01307496106868 * Close(index + 19)
               + 0.01169702291063 * Close(index + 20)
               + 0.00974841844086 * Close(index + 21)
               + 0.00898900012545 * Close(index + 22)
               - 0.00649745721156 * Close(index + 23);
        }

    }
}
