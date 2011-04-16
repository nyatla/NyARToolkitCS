/* 
 * PROJECT: NyARToolkitCS(Extension)
 * --------------------------------------------------------------------------------
 * The NyARToolkitCS is C# edition ARToolKit class library.
 * Copyright (C)2008-2009 Ryo Iizuka
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * For further information please contact.
 *	http://nyatla.jp/nyatoolkit/
 *	<airmail(at)ebony.plala.or.jp> or <nyatla(at)nyatla.jp>
 * 
 */
using System;
using System.Collections.Generic;
using System.Text;
using jp.nyatla.nyartoolkit.cs.core;
using jp.nyatla.nyartoolkit.cs.rpf.realitysource.nyartk;
using jp.nyatla.nyartoolkit.cs.rpf.tracker.nyartk;

namespace jp.nyatla.nyartoolkit.cs.rpf.realitysource.nyartk
{

    /**
     * RGBラスタをラップしたRealitySourceです。
     * @author nyatla
     *
     */
    public class NyARRealitySource_Reference : NyARRealitySource
    {
	    protected NyARRasterFilter_Rgb2Gs_RgbAve192 _filter;
	    /**
	     * 
	     * @param i_width
	     * ラスタのサイズを指定します。
	     * @param i_height
	     * ラスタのサイズを指定します。
	     * @param i_ref_raster_distortion
	     * 歪み矯正の為のオブジェクトを指定します。歪み矯正が必要ない時は、NULLを指定します。
	     * @param i_depth
	     * エッジ画像のサイズを1/(2^n)で指定します。(例:QVGA画像で1を指定すると、エッジ検出画像は160x120になります。)
	     * 数値が大きいほど高速になり、検出精度は低下します。実用的なのは、1<=n<=3の範囲です。標準値は2です。
	     * @param i_number_of_sample
	     * サンプリングするターゲット数を指定します。大体100以上をしておけばOKです。具体的な計算式は、{@link NyARTrackerSource_Reference#NyARTrackerSource_Reference}を参考にして下さい。
	     * @param i_raster_type
	     * ラスタタイプ
	     * @throws NyARException
	     */
	    public NyARRealitySource_Reference(int i_width,int i_height,NyARCameraDistortionFactor i_ref_raster_distortion,int i_depth,int i_number_of_sample,int i_raster_type)
	    {
		    this._rgb_source=new NyARRgbRaster(i_width,i_height,i_raster_type);
		    this._filter=new NyARRasterFilter_Rgb2Gs_RgbAve192(this._rgb_source.getBufferType());
		    this._source_perspective_reader=new NyARPerspectiveRasterReader(_rgb_source.getBufferType());
		    this._tracksource=new NyARTrackerSource_Reference(i_number_of_sample,i_ref_raster_distortion,i_width,i_height,i_depth,true);
		    return;
	    }
        public override bool isReady()
	    {
		    return this._rgb_source.hasBuffer();
	    }
        public override void syncResource()
	    {
		    this._filter.doFilter(this._rgb_source,this._tracksource.refBaseRaster());
		    base.syncResource();
	    }
        public override NyARTrackerSource makeTrackSource()
	    {
		    this._filter.doFilter(this._rgb_source,this._tracksource.refBaseRaster());		
		    return this._tracksource;
	    }

    }
}