/* 
 * PROJECT: NyARToolkitCS
 * --------------------------------------------------------------------------------
 * This work is based on the original ARToolKit developed by
 *   Hirokazu Kato
 *   Mark Billinghurst
 *   HITLab, University of Washington, Seattle
 * http://www.hitl.washington.edu/artoolkit/
 *
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
using System.Diagnostics;

namespace jp.nyatla.nyartoolkit.cs.core
{
    /**
     * 明点と暗点をPタイル法で検出して、その中央値を閾値とする。
     * 
     * 
     */
    public class NyARRasterThresholdAnalyzer_SlidePTile : INyARRasterThresholdAnalyzer
    {
	    protected NyARRasterAnalyzer_Histogram _raster_analyzer;
	    private NyARHistogramAnalyzer_SlidePTile _sptile;
	    private NyARHistogram _histogram;
	    public void setVerticalInterval(int i_step)
	    {
		    this._raster_analyzer.setVerticalInterval(i_step);
		    return;
	    }
	    public NyARRasterThresholdAnalyzer_SlidePTile(int i_persentage,int i_raster_format,int i_vertical_interval)
	    {
		    Debug.Assert (0 <= i_persentage && i_persentage <= 50);
		    //初期化
		    if(!initInstance(i_raster_format,i_vertical_interval)){
			    throw new NyARException();
		    }
		    this._sptile=new NyARHistogramAnalyzer_SlidePTile(i_persentage);
		    this._histogram=new NyARHistogram(256);
	    }
	    protected bool initInstance(int i_raster_format,int i_vertical_interval)
	    {
		    this._raster_analyzer=new NyARRasterAnalyzer_Histogram(i_raster_format,i_vertical_interval);
		    return true;
	    }
	    public int analyzeRaster(INyARRaster i_input)
	    {
		    this._raster_analyzer.analyzeRaster(i_input, this._histogram);
		    return this._sptile.getThreshold(this._histogram);
	    }
    }
}
