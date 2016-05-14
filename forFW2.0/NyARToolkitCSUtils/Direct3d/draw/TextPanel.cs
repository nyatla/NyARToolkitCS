using System;
using System.Collections.Generic;
using System.Text;
#if NyartoolkitCS_FRAMEWORK_CFW
using Microsoft.WindowsMobile.DirectX.Direct3D;
using Microsoft.WindowsMobile.DirectX;
#else
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
#endif
namespace NyARToolkitCSUtils.Direct3d
{
    public class TextPanel
    {
        private Device _device;
        private System.Drawing.Font _font;
        public TextPanel(Device i_device, int i_size)
        {
            this._device = i_device;
            this._font = new System.Drawing.Font("System", i_size);
            return;
        }
        public void draw(String i_str, float i_scale)
        {
            Mesh m = Mesh.TextFromFont(this._device, this._font, i_str, 5.0f, 0.1f);

            m.DrawSubset(0);
            m.Dispose();
            return;
        }
    }
}
