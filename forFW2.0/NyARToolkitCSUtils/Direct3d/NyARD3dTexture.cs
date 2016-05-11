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
using jp.nyatla.nyartoolkit.cs.cs4;

namespace NyARToolkitCSUtils.Direct3d
{
    /* DirectShowのテクスチャをカプセル化して、NyARRgbRasterの入力インタフェイスを定義します。
     * 入力できるNyARRgbRasterの種類には制限があります。注意してください。
     */
    public class NyARD3dTexture:IDisposable
    {
        private bool _is_dispose = false;
        private int m_width;
        private int m_height;
        private int m_texture_width;
        private int m_texture_height;
        private Microsoft.DirectX.Direct3D.Device m_ref_dev;
        private Texture m_texture;

        /* i_valueを超える最も小さい2のべき乗の値を返します。
         * 
         */
        private static int getSquareSize(int i_value)
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
        public static explicit operator Texture(NyARD3dTexture t)
        {
            return t.m_texture;
        }

        public bool isEqualSize(NyARIntSize i_s)
        {
            return (this.m_height==i_s.h && this.m_width==i_s.w);
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
        ~NyARD3dTexture()
        {
            this.Dispose();
        }
        public void Dispose()
        {
            if (this.m_texture != null)
            {
                this.m_texture.Dispose();
                this.m_texture = null;
            }
            this._is_dispose = true;
            //GCからデストラクタを除外
            GC.SuppressFinalize(this);

        }
        /* DsXRGB32Rasterの内容を保持しているテクスチャにコピーします。
         * i_rasterのサイズは、このインスタンスに指定したテクスチャサイズ（コンストラクタ等に指定したサイズ）と同じである必要です。
         * ラスタデータはテクスチャの左上を基点にwidth x heightだけコピーされ、残りの部分は更新されません。
         */
        public void setRaster(INyARRgbRaster i_raster)
        {
            Debug.Assert(!this._is_dispose);
            int pitch;
            using(GraphicsStream texture_rect = this.m_texture.LockRectangle(0, LockFlags.None, out pitch))
            {
                try{
                    int dst = (int)texture_rect.InternalData;
                    switch (i_raster.getBufferType())
                    {
                        case NyARBufferType.BYTE1D_B8G8R8X8_32:
                            {
                                byte[] buf = (byte[])i_raster.getBuffer();
                                //テクスチャのピッチって何？
                                int src_w = this.m_width * 4;
                                int src = 0;
                                for (int r = this.m_height - 1; r >= 0; r--)
                                {
                                    Marshal.Copy(buf, src, (IntPtr)dst, pitch);
                                    dst += pitch;
                                    src += src_w;
                                }
                            }
                            break;
                        case NyARBufferType.OBJECT_CS_Bitmap:
                            NyARBitmapRaster ra = (NyARBitmapRaster)(i_raster);
                            BitmapData bm = ra.lockBitmap(); 
                            try
                            {
                                int src = (int)bm.Scan0;
                                for (int r = this.m_height - 1; r >= 0; r--)
                                {
                                    NyARD3dUtil.RtlCopyMemory((IntPtr)dst, (IntPtr)src, pitch);
                                    dst += pitch;
                                    src += bm.Stride;
                                }
                            }
                            finally
                            {
                                ra.unlockBitmap();
                            }
                            break;
                        default:
                            throw new NyARRuntimeException();
                    }
                }
                finally
                {
                    //テクスチャをアンロックする
                    this.m_texture.UnlockRectangle(0);
                }
            }
            return;
        }
    }
}
