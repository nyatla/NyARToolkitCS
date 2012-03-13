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
//CFWでコンパイルするときはNyartoolkitCS_FRAMEWORK_CFWをアクティブにしてください。
//#define NyartoolkitCS_FRAMEWORK_CFW
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using Microsoft.WindowsMobile.DirectX;
using Microsoft.WindowsMobile.DirectX.Direct3D;
using System.Runtime.InteropServices;
using jp.nyatla.cs.NyWMCapture;
using jp.nyatla.nyartoolkit.cs.core;
using NyARToolkitCSUtils.WMCapture;
using System.Diagnostics;

namespace NyARToolkitCSUtils.Direct3d
{
    /* DsRGB556Rasterのラスタデータを取り込むことが出来るTextureです。
     * このテクスチャはそのままARToolKitの背景描画に使います。
     * 動くか判りません。
     */
    public class NyARTexture_RGB565
    {
        private int m_width;
        private int m_height;
        private int m_texture_width;
        private int m_texture_height;
        private Microsoft.WindowsMobile.DirectX.Direct3D.Device m_ref_dev;
        private Texture _texture;

        /* i_valueを超える最も小さい2のべき乗の値を返します。
         */
        private int GetSquareSize(int i_value)
        {
            int u = 2;
            //2^nでサイズを超える一番小さな値を得る。
            for (; ; )
            {
                if (u >= i_value)
                {
                    break;
                }
                u = u << 1;
                if (u <= 0)
                {
                    throw new Exception();
                }
            }
            return u;
        }
        public Texture d3d_texture
        {
            get { return this._texture; }
        }

        /* i_width x i_heightのテクスチャを格納するインスタンスを生成します。
         * 確保されるテクスチャのサイズは指定したサイズと異なり、i_width x i_heightのサイズを超える
         * 2のべき乗サイズになります。
         * 
         */
        public NyARTexture_RGB565(Microsoft.WindowsMobile.DirectX.Direct3D.Device i_dev, int i_width, int i_height)
        {
            this.m_ref_dev = i_dev;

            this.m_height = i_height;
            this.m_width = i_width;

            //テクスチャサイズの確定(2^n)
            this.m_texture_height = GetSquareSize(i_height);
            this.m_texture_width = GetSquareSize(i_width);

            //テクスチャを作るよ！
            this._texture = new Texture(i_dev, this.m_texture_width ,this.m_texture_height, 0, Usage.None | Usage.Lockable, Format.R5G6B5, Pool.Managed);

            //OK、完成だ。
            return;
        }
        public void CopyFromRaster(DsRGB565Raster i_raster)
        {
            //BUFFERFORMAT_WORD1D_R5G6B5_16LEしか受けられません。
            Debug.Assert(i_raster.isEqualBufferType(NyARBufferType.WORD1D_R5G6B5_16LE));
            int pi;
            int w = this.m_width;
            GraphicsStream gs = this._texture.LockRectangle(0, LockFlags.None, out pi);
            short[] buf = (short[])i_raster.getBuffer();
            int st = this.m_width;
            int s_idx = 0;
            int d_idx = 0;
            for (int i = this.m_height - 1; i >= 0; i--)
            {
                Marshal.Copy(buf, s_idx, (IntPtr)((int)gs.InternalData + d_idx), w);
                s_idx += st;
                d_idx += pi;
            }
            this._texture.UnlockRectangle(0);
            return;
        }
        public void Dispose()
        {
            if (this._texture != null)
            {
                this._texture.Dispose();
            }
            return;
        }
    }
}
