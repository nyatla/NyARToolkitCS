using System;
using System.Collections.Generic;
using System.Collections;
using DirectShowLib;
using System.Diagnostics;
using System.Runtime.InteropServices;
using jp.nyatla.nyartoolkit.cs.rpf.realitysource.nyartk;
using jp.nyatla.nyartoolkit.cs.core;
using jp.nyatla.nyartoolkit.cs.rpf.tracker.nyartk;
using NyARToolkitCSUtils.NyAR;

namespace NyARToolkitCSUtils.Capture
{
    /**
     * このクラスは、JMFと互換性のあるNyARRealitySourceです。
     * @author nyatla
     *
     */
    public class NyARRealitySource_DShow : NyARRealitySource
    {
	    protected NyARRasterFilter_Rgb2Gs_RgbAve192 _filter;
        /**
         * コンストラクタです。NyARBufferType.BYTE1D_B8G8R8X8_32形式のRGBラスタを所有するRealitySourceを生成します。
         * @param i_fmt_width
         * @param i_fmt_height
         * 入力フォーマットを指定します。
         * @param i_ref_raster_distortion
         * 歪み矯正の為のオブジェクトを指定します。歪み矯正が必要ない時は、NULLを指定します。
         * @param i_depth
         * エッジ画像のサイズを1/(2^n)で指定します。(例:QVGA画像で1を指定すると、エッジ検出画像は160x120になります。)
         * 数値が大きいほど高速になり、検出精度は低下します。実用的なのは、1<=n<=3の範囲です。標準値は2です。
         * @param i_number_of_sample
         * サンプリングするターゲット数を指定します。大体100以上をしておけばOKです。具体的な計算式は、{@link NyARTrackerSource_Reference#NyARTrackerSource_Reference}を参考にして下さい。
         * @throws NyARException
         */
        public NyARRealitySource_DShow(int i_fmt_width,int i_fmt_height,NyARCameraDistortionFactor i_ref_raster_distortion,int i_depth,int i_number_of_sample)
	    {
            this._rgb_source = new DsBGRX32Raster(i_fmt_width, i_fmt_height);
		    this._filter=new NyARRasterFilter_Rgb2Gs_RgbAve192(this._rgb_source.getBufferType());
		    this._source_perspective_reader=new NyARPerspectiveRasterReader(_rgb_source.getBufferType());
            this._tracksource = new NyARTrackerSource_Reference(i_number_of_sample, i_ref_raster_distortion, i_fmt_width, i_fmt_height, i_depth, true);
		    return;
	    }
    	
	    /**
	     * DirectShow.NETのキャプチャデータをセットします。データ形式は、でなければいけません。
	     * @param i_buffer
	     * @throws NyARException
	     */
	    public void setDShowImage(IntPtr i_buffer,bool i_flip_virtical)
	    {
            ((DsBGRX32Raster)this._rgb_source).setBuffer(i_buffer, i_flip_virtical);
		    return;
	    }
	    public sealed override bool isReady()
	    {
            return ((DsBGRX32Raster)this._rgb_source).hasBuffer();
	    }
	    public sealed override void syncResource()
	    {
		    this._filter.doFilter(this._rgb_source,this._tracksource.refBaseRaster());
		    base.syncResource();
	    }
	    public sealed override NyARTrackerSource makeTrackSource()
	    {
		    this._filter.doFilter(this._rgb_source,this._tracksource.refBaseRaster());		
		    return this._tracksource;
	    }
    }
}