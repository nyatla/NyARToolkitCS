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
using System.Collections.Generic;
using System.Diagnostics;
using jp.nyatla.nyartoolkit.cs.core;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using NyARToolkitCSUtils.NyAR;

namespace NyARToolkitCSUtils.Direct3d
{
    /* DsXRGB32Rasterのラスタデータを取り込むことが出来るTextureです。
     * このテクスチャはそのままARToolKitの背景描画に使います。
     */
    public class NyARTexture_XRGB32
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
            get { return this.m_texture; }
        }

        /* i_width x i_heightのテクスチャを格納するインスタンスを生成します。
         * 確保されるテクスチャのサイズは指定したサイズと異なり、i_width x i_heightのサイズを超える
         * 2のべき乗サイズになります。
         * 
         */
        public NyARTexture_XRGB32(Microsoft.DirectX.Direct3D.Device i_dev, int i_width, int i_height)
        {
            this.m_ref_dev = i_dev;

            this.m_height = i_height;
            this.m_width = i_width;

            //テクスチャサイズの確定
            this.m_texture_height = GetSquareSize(i_height);
            this.m_texture_width = GetSquareSize(i_width);

            //テクスチャを作るよ！
            this.m_texture = new Texture(this.m_ref_dev, this.m_texture_width, this.m_texture_height, 1, Usage.Dynamic, Format.X8R8G8B8, Pool.Default);
            //OK、完成だ。
            return;
        }
        /* DsXRGB32Rasterの内容を保持しているテクスチャにコピーします。
         * i_rasterのサイズは、このインスタンスに指定したテクスチャサイズ（コンストラクタ等に指定したサイズ）と同じである必要です。
         * ラスタデータはテクスチャの左上を基点にwidth x heightだけコピーされ、残りの部分は更新されません。
         */
        public void CopyFromXRGB32(DsBGRX32Raster i_raster)
        {
            //BUFFERFORMAT_BYTE1D_B8G8R8X8_32しか受けられません。
            Debug.Assert(i_raster.isEqualBufferType(NyARBufferType.BYTE1D_B8G8R8X8_32));
            GraphicsStream texture_rect;
            try
            {
                byte[] buf =(byte[])i_raster.getBuffer();
                // テクスチャをロックする
                texture_rect = this.m_texture.LockRectangle(0, LockFlags.None);
                //テクスチャのピッチって何？
                int cp_size = this.m_width * 4;
                int sk_size = (this.m_texture_width - this.m_width) * 4;
                int s = 0;
                for (int r = this.m_height - 1; r >= 0; r--,s++)
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
            return;
        }
        public void Dispose()
        {
            this.d3d_texture.Dispose();
        }
    }
}
