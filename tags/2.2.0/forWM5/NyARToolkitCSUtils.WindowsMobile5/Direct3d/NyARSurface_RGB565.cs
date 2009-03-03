/* 
 * PROJECT: NyARToolkitCSUtils NyARToolkit for C# 支援ライブラリ
 * --------------------------------------------------------------------------------
 * The MIT License
 * Copyright (c) 2008 nyatla
 * airmail(at)ebony.plala.or.jp
 * http://nyatla.jp/nyartoolkit/
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 * 
 */
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using Microsoft.WindowsMobile.DirectX;
using Microsoft.WindowsMobile.DirectX.Direct3D;
using System.Runtime.InteropServices;
using jp.nyatla.cs.NyWMCapture;


namespace NyARToolkitCSUtils.Direct3d
{
    /* DsRGB556Rasterのラスタデータを取り込むことが出来るSurfaceです。
     * このSurfaceはそのままARToolKitの背景描画に使います。
     */
    public class NyARSurface_RGB565 : IDisposable
    {
        private int _width;
        private int _height;
        private Microsoft.WindowsMobile.DirectX.Direct3D.Device m_ref_dev;
        private Surface m_surface;
        private Rectangle m_src_rect;
        public Rectangle d3d_surface_rect
        {
            get { return this.m_src_rect; }
        }
        public Surface d3d_surface
        {
            get { return this.m_surface; }
        }

        /* i_width x i_heightのテクスチャを格納するインスタンスを生成します。
         * 確保されるテクスチャのサイズは指定したサイズと異なり、i_width x i_heightのサイズを超える
         * 2のべき乗サイズになります。
         * 
         */
        public NyARSurface_RGB565(Microsoft.WindowsMobile.DirectX.Direct3D.Device i_dev, int i_width, int i_height)
        {
            this.m_ref_dev = i_dev;

            this._height = i_height;
            this._width = i_width;

            this.m_surface = i_dev.CreateImageSurface(i_width, i_height, Format.R5G6B5);
            this.m_src_rect = new Rectangle(0, 0, i_width,i_height);

            //OK、完成だ。
            return;
        }
        /* ISampleの内容を保持しているサーフェイスにコピーします。
         * i_is_top_to_botommは、i_sampleが上下反転画像(Y軸が下から上に向かって増加する)かを指定します。
         */
        public void CopyFromIntPtr(INySample i_sample,bool i_is_top_to_botomm)
        {
            int pitch;
            GraphicsStream gs = this.m_surface.LockRectangle(this.m_src_rect, LockFlags.None, out pitch);
            if (!i_is_top_to_botomm)
            {
                int st = this._width * 2;
                int s_idx = 0;
                int d_idx = (this._height - 1) * pitch;
                for (int i = this._height - 1; i >= 0; i--)
                {
                    i_sample.CopyToBuffer((IntPtr)((int)gs.InternalData + d_idx), s_idx, st);
                    s_idx += st;
                    d_idx -= pitch;
                }
            }else{
                i_sample.CopyToBuffer(gs.InternalData, 0, this._width * this._height * 2);
            }
            this.m_surface.UnlockRectangle();

            return;
        }
        public void Dispose()
        {
            if (this.m_surface != null)
            {
                this.m_surface.Dispose();
            }
            return;
        }

    }
}
