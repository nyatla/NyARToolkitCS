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
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Diagnostics;
using jp.nyatla.nyartoolkit.cs.core;
using NyARToolkitCSUtils.Capture;
using System.Drawing.Imaging;
using System.Drawing;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace NyARToolkitCSUtils.Direct3d
{
    /* DirectShowのテクスチャをカプセル化して、NyARRgbRasterの入力インタフェイスを定義します。
     * 入力できるNyARRgbRasterの種類には制限があります。注意してください。
     */
    public class NyARD3dTexture:IDisposable
    {
        private int m_width;
        private int m_height;
        private int m_texture_width;
        private int m_texture_height;
        private Microsoft.DirectX.Direct3D.Device m_ref_dev;
        private Texture m_texture;

        /* i_valueを超える最も小さい2のべき乗の値を返します。
         * 
         */
        private int getSquareSize(int i_value)
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
            get { return this.m_texture; }
        }

        /* i_width x i_heightのテクスチャを格納するインスタンスを生成します。
         * 確保されるテクスチャのサイズは指定したサイズと異なり、i_width x i_heightのサイズを超える
         * 2のべき乗サイズになります。
         * 
         */
        public NyARD3dTexture(Microsoft.DirectX.Direct3D.Device i_dev, int i_width, int i_height)
        {
            this.m_ref_dev = i_dev;

            this.m_height = i_height;
            this.m_width = i_width;

            //テクスチャサイズの確定
            this.m_texture_height = getSquareSize(i_height);
            this.m_texture_width = getSquareSize(i_width);

            //テクスチャを作るよ！
            this.m_texture = new Texture(this.m_ref_dev, this.m_texture_width, this.m_texture_height, 1, Usage.Dynamic, Format.X8R8G8B8, Pool.Default);
            //OK、完成だ。
            return;
        }
        public void Dispose()
        {
            this.m_texture.Dispose();
        }
        /* DsXRGB32Rasterの内容を保持しているテクスチャにコピーします。
         * i_rasterのサイズは、このインスタンスに指定したテクスチャサイズ（コンストラクタ等に指定したサイズ）と同じである必要です。
         * ラスタデータはテクスチャの左上を基点にwidth x heightだけコピーされ、残りの部分は更新されません。
         */
        public void CopyFromXRGB32(INyARRgbRaster i_raster)
        {
            GraphicsStream texture_rect;
            switch (i_raster.getBufferType())
            {
                case NyARBufferType.BYTE1D_B8G8R8X8_32:
                    try
                    {
                        byte[] buf = (byte[])i_raster.getBuffer();
                        // テクスチャをロックする
                        texture_rect = this.m_texture.LockRectangle(0, LockFlags.None);
                        //テクスチャのピッチって何？
                        int cp_size = this.m_width * 4;
                        int sk_size = (this.m_texture_width - this.m_width) * 4;
                        int s = 0;
                        for (int r = this.m_height - 1; r >= 0; r--, s++)
                        {
                            texture_rect.Write(buf, s * cp_size, cp_size);
                            texture_rect.Seek(sk_size, System.IO.SeekOrigin.Current);
                        }
                    }
                    finally
                    {
                        //テクスチャをアンロックする
                        this.m_texture.UnlockRectangle(0);
                    }
                    break;
                case NyARBufferType.OBJECT_CS_Bitmap:
                    try
                    {
                        NyARBitmapRaster ra = (NyARBitmapRaster)(i_raster.getBuffer());
                        // テクスチャをロックする
                        texture_rect = this.m_texture.LockRectangle(0, LockFlags.None);
                        //テクスチャのピッチって何？
                        int cp_size = this.m_width * 4;
                        int sk_size = (this.m_texture_width - this.m_width) * 4;
                        int s = 0;
                        BitmapData bm = ra.lockBitmap();
                        byte[] tmp = new byte[cp_size];
                        for (int r = this.m_height - 1; r >= 0; r--, s++)
                        {
                            Marshal.Copy((IntPtr)((int)bm.Stride+bm.Stride*s),tmp,0,cp_size);
                            texture_rect.Write(tmp,0,cp_size);
                            texture_rect.Seek(sk_size, System.IO.SeekOrigin.Current);//padding
                        }
                        ra.unlockBitmap();
                    }
                    finally
                    {
                        //テクスチャをアンロックする
                        this.m_texture.UnlockRectangle(0);
                    }
                    break;
                default:
                    throw new NyARException();
            }

            return;
        }
    }
}
