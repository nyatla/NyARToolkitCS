using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
#if NyartoolkitCS_FRAMEWORK_CFW
using Microsoft.WindowsMobile.DirectX.Direct3D;
using Microsoft.WindowsMobile.DirectX;
#else
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
#endif
using NyARToolkitCSUtils.Direct3d;
using jp.nyatla.nyartoolkit.cs.core;
using jp.nyatla.nyartoolkit.cs.markersystem;

namespace NyARToolkitCSUtils.Direct3d
{
    public class NyARD3dRender:IDisposable, INyARSingleCameraSystemObserver
    {
        private NyARSingleCameraSystem _scs;
        private NyARIntSize _screen_size;

	    /**
	     * コンストラクタです。マーカシステムに対応したレンダラを構築します。
	     * @param i_ms
	     */
        public NyARD3dRender(Device i_dev,NyARSingleCameraSystem i_scs)
	    {
            this._scs = i_scs;
            this._screen_size = _scs.getARParam().getScreenSize();
            i_scs.getSingleView().addObserver(this);
        }

        private Matrix _projection_mat = new Matrix();
        /**
         * Observerのイベントハンドラ
         */
        public void onUpdateCameraParametor(NyARParam i_param, double i_near, double i_far)
        {
            NyARD3dUtil.toCameraFrustumRH(i_param, i_near, i_far,ref this._projection_mat);
        }

        /// <summary>
        /// Device.ViewportをAR向けに設定します。
        /// </summary>
        /// <param name="i_img_height">バックグラウンド画像の解像度を指定します。</param>
        /// <param name="i_img_width">バックグラウンド画像の解像度を指定します。</param>
        public void loadARViewPort(Device i_dev)
        {
            //ビューポート設定
            i_dev.Viewport = NyARD3dUtil.getARViewPort(this._screen_size.w, this._screen_size.h);
        }
        /// <summary>
        /// Device.Transform.ViewにAR表示向けのDirectXビューを設定します。
        /// </summary>
        /// <param name="i_d3d"></param>
        public void loadARViewMatrix(Device i_dev)
        {
            // ビュー変換の設定(左手座標系ビュー行列で設定する)
            // 0,0,0から、Z+方向を向いて、上方向がY軸
            i_dev.Transform.View = NyARD3dUtil.getARView();
        }
        public void loadARProjectionMatrix(Device i_dev)
        {
            Matrix old = i_dev.Transform.Projection;
            i_dev.Transform.Projection = this._projection_mat;
        }

        private NyARD3dSurface _surface = null;
        /// <summary>
        /// バックグラウンドにラスタを描画します。
        /// </summary>
        /// <param name="i_gl"></param>
        /// <param name="i_bg_image"></param>
        public void drawBackground(Device i_dev, INyARRgbRaster i_bg_image)
	    {
            NyARIntSize s = i_bg_image.getSize();
            if(this._surface==null){
                this._surface = new NyARD3dSurface(i_dev,s.w,s.h);
            }else if(!this._surface.isEqualSize(i_bg_image.getSize())){
                //サーフェイスの再構築
                this._surface.Dispose();
                this._surface = new NyARD3dSurface(i_dev, this._screen_size.w, this._screen_size.h);
            }
            this._surface.setRaster(i_bg_image);
            Surface dest_surface = i_dev.GetBackBuffer(0, 0, BackBufferType.Mono);
            Rectangle rect = new Rectangle(0, 0, this._screen_size.w, this._screen_size.h);
            i_dev.StretchRectangle((Surface)this._surface, rect, dest_surface, rect, TextureFilter.None);
	    }

        private NyARD3dTexture _texture=null;
        public void drawImage2d(Device i_dev,int i_x,int i_y,INyARRgbRaster i_raster)
        {
            NyARIntSize s = i_raster.getSize();
            if (this._texture == null)
            {
                this._texture=new NyARD3dTexture(i_dev,s.w,s.h);
            }
            else if (!this._texture.isEqualSize(s))
            {
                this._texture.Dispose();
                this._texture = new NyARD3dTexture(i_dev, s.w, s.h);
            }
            this._texture.setRaster(i_raster);
            using (Sprite sp = new Sprite(i_dev))
            {
                sp.Begin(SpriteFlags.None);
                sp.Draw((Texture)this._texture, new Rectangle(i_x,i_y, s.w,s.h), Vector3.Empty,new Vector3(i_dev.Viewport.X, i_dev.Viewport.Y, 0),Color.White);
                sp.End();
            }
        }

        public void colorCube(Device i_dev, float i_size_per_mm)
        {
            using(ColorCube cc=new ColorCube(i_dev,i_size_per_mm)){
                cc.draw(i_dev);
            }
        }
        public void Dispose()
        {
            if(this._surface!=null){
                this._surface.Dispose();
            }
            if (this._texture != null)
            {
                this._texture.Dispose();
            }
        }
    }
}
