/* 
 * The MIT License
 * 
 * Copyright (c) 2008 nyatla
 * airmail(at)ebony.plala.or.jp
 * http://nyatla.jp/
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
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
using System.Drawing;
using Microsoft.WindowsMobile.DirectX;
using Microsoft.WindowsMobile.DirectX.Direct3D;
using jp.nyatla.cs.NyWMCapture;


namespace SimpleLiteDirect3d.WindowsMobile5
{
    /* デバイス依存のパラメタとかを吸収するクラス
     * デバイスごとにこのクラスから派生させる。
     * 
     */
    public abstract class DeviceAdapter
    {
        //派生クラスのInitでここのメンバ変数を初期化する。
        protected Matrix       m_raster_mat;
        protected Viewport     m_viewport;
        protected Size         m_capture_size;
        protected INyWMCapture m_capture;
        protected bool m_is_turn_vertical;

        //D3Dのビューポートを返す
        public Viewport D3dViewport
        {
            get{return this.m_viewport;}
        }
        //キャプチャサイズを返す
        public Size CaptureSize
        {
            get{return this.m_capture_size;}
        }
        //キャプチャインタフェイスを返す
        public INyWMCapture CaptureIf
        {
            get{return (INyWMCapture)this.m_capture;}
        }
        //ラスタ表示用マトリクスを返す
        public Matrix RasterMat
        {
            get{return this.m_raster_mat;}
        }
        //キャプチャデータが反転しているかのフラグ値
        public bool IsTurnCapVertical
        {
            get{return this.m_is_turn_vertical;}
        }
        /*------------------------------------------------*/
        //ビューポートを初期化する。
        protected static Viewport InitViewPort(Size i_client_size,Size i_cap_size,out float o_scale)
        {
            //クライアントサイズを得る
            float scale;
            int new_w,new_h;
            //縦にあわせてみる。
            scale = (float)i_client_size.Height / (float)i_cap_size.Height;
            new_h = i_client_size.Height;
            new_w = (int)(i_cap_size.Width * scale);
            //幅が収まってないなら、幅に合わせる。
            if (new_w > i_client_size.Width)
            {
                scale = (float)i_client_size.Width / (float)i_cap_size.Width;
                new_w = i_client_size.Width;
                new_h = (int)(i_cap_size.Height * scale);

            }
            o_scale=scale;

            //ビューポート作成
            Viewport vp=new Viewport();
            vp.Height = new_h;
            vp.Width = new_w;
            vp.X = (i_client_size.Width - new_w) / 2;
            vp.Y = (i_client_size.Height - new_h) / 2;
            o_scale=scale;
            return vp;
        }
        public void Finish()
        {
            this.m_capture.Finalize();
        }
        public abstract void Init(Size i_client_size, INySampleCB i_sample_cb);
    }

    /*EM-ONE
     */
    public class DeviceAdapter_S01SH:DeviceAdapter
    {
        public override void Init(Size i_client_size,INySampleCB i_sample_cb)
        {
            float scale;
            this.m_is_turn_vertical=false;
            //キャプチャサイズの決定
            this.m_capture_size = new Size(240, 320);
            //スクリーンサイズと倍率を決定
            this.m_viewport=InitViewPort(i_client_size, this.m_capture_size, out scale);

            //ラスタの変換行列の決定
            Vector2 scale_vec = new Vector2(-scale, scale);
            this.m_raster_mat = Matrix.Transformation2D(new Vector2(160, 0), 0.0f, scale_vec, new Vector2(240, 320), (float)Math.PI, new Vector2(this.m_viewport.X, this.m_viewport.Y));

            //キャプチャ作る。
            NyWMCapture cap = new NyWMCapture();
            INyWMCapture cap_if = (INyWMCapture)cap;
            int hr;
            hr = cap_if.SetCallBack(i_sample_cb);//これInitializeの前にやらないといけないのよね。
            hr = cap_if.SetSize(this.m_capture_size.Width, this.m_capture_size.Height);
            hr = cap_if.Initialize(NyWMCapture.DeviceId_WM5, NyWMCapture.MediaSubType_RGB565, NyWMCapture.PinCategory_PREVIEW);
            this.m_capture = cap_if;
            return;
        }
    }
    /*ES
     */
    public class DeviceAdapter_WS007SH: DeviceAdapter
    {
        public override void Init(Size i_client_size, INySampleCB i_sample_cb)
        {
            float scale;
            this.m_is_turn_vertical = true;
            //キャプチャサイズの決定
            this.m_capture_size = new Size(240, 320);
            //スクリーンサイズと倍率を決定
            this.m_viewport = InitViewPort(i_client_size, this.m_capture_size, out scale);

            //ラスタの変換行列の決定
            Vector2 scale_vec = new Vector2(scale, scale);
            this.m_raster_mat = Matrix.Transformation2D(Vector2.Empty, 0.0f, scale_vec, Vector2.Empty, (float)0, new Vector2(this.m_viewport.X, this.m_viewport.Y));

            //キャプチャ作る。
            NyWMCapture cap = new NyWMCapture();
            INyWMCapture cap_if = (INyWMCapture)cap;
            int hr;
            hr = cap_if.SetCallBack(i_sample_cb);//これInitializeの前にやらないといけないのよね。
            hr = cap_if.SetSize(this.m_capture_size.Width, this.m_capture_size.Height);
            hr = cap_if.Initialize(NyWMCapture.DeviceId_WM5, NyWMCapture.MediaSubType_RGB565, NyWMCapture.PinCategory_PREVIEW);
            this.m_capture = cap_if;
            return;
        }
    }
}
