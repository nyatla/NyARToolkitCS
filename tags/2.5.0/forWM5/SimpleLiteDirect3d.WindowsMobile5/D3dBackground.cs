/* 
 * Capture Test NyARToolkitCSサンプルプログラム
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
using System.Drawing;
using System.Diagnostics;
using Microsoft.WindowsMobile.DirectX.Direct3D;
using Microsoft.WindowsMobile.DirectX;
using NyARToolkitCSUtils.Direct3d;
using jp.nyatla.cs.NyWMCapture;
using jp.nyatla.nyartoolkit.cs.core;
using NyARToolkitCSUtils.NyAR;
using System.Runtime.InteropServices;

namespace SimpleLiteDirect3d.WindowsMobile5
{
    public interface ID3dBackground : IDisposable
    {
        void drawBackGround();
        void CopyFromRaster(DsRGB565Raster i_raster);
    }
    public class D3dSurfaceBackground : ID3dBackground
    {
        private int _width;
        private int _height;
        private D3dManager _d3dm;
        private Surface _surface;
        private Rectangle _src_rect;
        public D3dSurfaceBackground(D3dManager i_mgr)
        {
            this._d3dm = i_mgr;
            this._height = i_mgr.background_size.Height;
            this._width = i_mgr.background_size.Width;
            this._surface = i_mgr.d3d_device.CreateImageSurface(this._width, this._height, Format.R5G6B5);
            this._src_rect = new Rectangle(0, 0, this._width, this._height);
            return;
        }
        public void drawBackGround()
        {
            //背景描画
            Surface dest_surface = this._d3dm.d3d_device.GetBackBuffer(0, BackBufferType.Mono);
            this._d3dm.d3d_device.StretchRectangle(this._surface, this._src_rect, dest_surface, this._d3dm.view_rect, TextureFilter.None);
            return;
        }
        public void CopyFromRaster(DsRGB565Raster i_raster)
        {
            Debug.Assert(i_raster.isEqualBufferType(NyARBufferType.WORD1D_R5G6B5_16LE));
            int pitch;
            GraphicsStream gs = this._surface.LockRectangle(this._src_rect, LockFlags.None, out pitch);
            /*
            int cp_size = this.m_width * 4;
            int s_idx=0;
            int d_idx = (this.m_height - 1) * cp_size;
            for(int i=this.m_height-1;i>=0;i--){
                //どう考えてもポインタです。
                Marshal.Copy((byte[])i_sample.getBufferReader().getBuffer(),s_idx,(IntPtr)((int)gs.InternalData+d_idx),cp_size);
                s_idx += cp_size;
                d_idx -= cp_size;
            }
            */
            Marshal.Copy((short[])i_raster.getBuffer(), 0, (IntPtr)((int)gs.InternalData), this._width  * this._height);

            this._surface.UnlockRectangle();

            return;
        }
        public void Dispose()
        {
            if (this._surface != null)
            {
                this._surface.Dispose();
            }
            return;
        }
    }

    public class D3dTextureBackground : ID3dBackground
    {
        private int _width;
        private int _height;
        private int _texture_width;
        private int _texture_height;
        private D3dManager _d3dm;
        private Rectangle _src_rect;
        private Texture _texture;
        private Sprite _sprite;
        private Vector3 _pos_vec;
        private Matrix _scaling;
        public D3dTextureBackground(D3dManager i_mgr)
        {
            this._d3dm = i_mgr;
            float scale = this._d3dm.scale;
            this._height = this._d3dm.background_size.Height;
            this._width  = this._d3dm.background_size.Width;
            this._scaling=Matrix.Scaling(this._d3dm.scale, this._d3dm.scale, 0f);
            this._pos_vec = new Vector3(this._d3dm.d3d_device.Viewport.X / scale, this._d3dm.d3d_device.Viewport.Y / scale, 0);
            this._src_rect = new Rectangle(0, 0, this._width, this._height);

            //テクスチャサイズの確定(2^n)
            this._texture_height = getSquareSize(this._height);
            this._texture_width = getSquareSize(this._width);
            this._sprite = new Sprite(this._d3dm.d3d_device);
            //テクスチャを作るよ！
            this._texture = new Texture(i_mgr.d3d_device, this._texture_width, this._texture_height, 0, Usage.None|Usage.Lockable , Format.R5G6B5, Pool.Managed);
            return;
        }
        public void drawBackGround()
        {
            this._sprite.Begin(SpriteFlags.None);
            this._sprite.Transform = this._scaling;
            this._sprite.Draw(this._texture, this._src_rect, Vector3.Empty, this._pos_vec, Color.White);
            this._sprite.End();
            return;
        }
        public void CopyFromRaster(DsRGB565Raster i_raster)
        {
            //BUFFERFORMAT_WORD1D_R5G6B5_16LEしか受けられません。
            Debug.Assert(i_raster.isEqualBufferType(NyARBufferType.WORD1D_R5G6B5_16LE));
            int pi;
            int w = this._width;
            GraphicsStream gs = this._texture.LockRectangle(0, LockFlags.None, out pi);
            short[] buf = (short[])i_raster.getBuffer();
            int st = this._width;
            int s_idx = 0;
            int d_idx = 0;
            for (int i = this._height - 1; i >= 0; i--)
            {
                Marshal.Copy(buf, s_idx, (IntPtr)((int)gs.InternalData + d_idx),w);
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
            if(this._sprite!=null){
                this._sprite.Dispose();
            }
            return;
        }
        /* i_valueを超える最も小さい2のべき乗の値を返します。
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
    }
}
