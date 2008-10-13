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
using System.Text;
using Microsoft.WindowsMobile.DirectX;
using Microsoft.WindowsMobile.DirectX.Direct3D;
using System.Windows.Forms;
using System.Drawing;
using jp.nyatla.nyartoolkit.cs.core;
/*
 * Direct3Dの統括クラス
 */
namespace SimpleLiteDirect3d.WindowsMobile5
{
    public class D3dManager:IDisposable
    {
        private Rectangle _view_rect;
        private Device _d3d_device;
        private Size _background_size;
        private float _scale;
        public Device d3d_device{
            get { return this._d3d_device; }
        }
        public Rectangle view_rect
        {
            get { return this._view_rect; }
        }
        public Size background_size
        {
            get { return this._background_size; }
        }
        public float scale
        {
            get { return this._scale; }
        }
        public D3dManager(Form i_main_window, NyARParam i_nyparam, int i_profile_id)
        {
            PresentParameters pp = new PresentParameters();
            // ウインドウモードなら true、フルスクリーンモードなら false を指定
            pp.Windowed = true;
            // スワップとりあえずDiscardを指定。
            pp.SwapEffect = SwapEffect.Discard;
            pp.EnableAutoDepthStencil = true;
            pp.AutoDepthStencilFormat = DepthFormat.D16;
            pp.BackBufferCount = 0;
            pp.BackBufferFormat = Format.R5G6B5;
            this._d3d_device = new Device(0, DeviceType.Default, i_main_window.Handle, CreateFlags.None, pp);

            //ビューポートを指定
            float scale = setupView(i_nyparam,i_main_window.ClientSize);
            this._scale = scale;
            // ライトを無効
            this._d3d_device.RenderState.Lighting = false;

            //カメラ画像の転写矩形を作成
            Viewport vp=this._d3d_device.Viewport;
            this._view_rect = new Rectangle(vp.X, vp.Y, vp.Width, vp.Height);

            NyARIntSize cap_size = i_nyparam.getScreenSize();
            this._background_size = new Size(cap_size.w, cap_size.h);
            return;
        }
        private float setupView(NyARParam i_nyparam, Size i_client_size)
        {
            NyARIntSize cap_size=i_nyparam.getScreenSize();
            float scale;
            int new_w, new_h;
            //縦にあわせてみる。
            scale = (float)i_client_size.Height / (float)cap_size.h;
            new_h = i_client_size.Height;
            new_w = (int)((float)cap_size.w * scale);
            //幅が収まってないなら、幅に合わせる。
            if (new_w > i_client_size.Width)
            {
                scale = (float)i_client_size.Width / (float)cap_size.w;
                new_w = i_client_size.Width;
                new_h = (int)(cap_size.h * scale);
            }

            //ビューポート作成
            Viewport vp = new Viewport();
            vp.Height = new_h;
            vp.Width = new_w;
            vp.X = (i_client_size.Width - new_w) / 2;
            vp.Y = (i_client_size.Height - new_h) / 2;

            //ビューポート設定
            this._d3d_device.Viewport = vp;

            // ビュー変換の設定(左手座標系ビュー行列で設定する)
            // 0,0,0から、Z+方向を向いて、上方向がY軸
            this._d3d_device.Transform.View = Matrix.LookAtLH(
                new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(0.0f, 1.0f, 0.0f));
            return scale;
        }
        public void Dispose()
        {
            if (this._d3d_device != null)
            {
                this._d3d_device.Dispose();
            }
            return;
        }
    }
}
