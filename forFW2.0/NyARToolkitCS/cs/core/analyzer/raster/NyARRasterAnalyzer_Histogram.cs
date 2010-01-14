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
     * 画像のヒストグラムを計算します。
     * RGBの場合、(R+G+B)/3のヒストグラムを計算します。
     * 
     * 
     */
    public class NyARRasterAnalyzer_Histogram
    {
	    protected ICreateHistogramImpl _histImpl;
	    /**
	     * ヒストグラム解析の縦方向スキップ数。継承クラスはこのライン数づつ
	     * スキップしながらヒストグラム計算を行うこと。
	     */
	    protected int _vertical_skip;
    	
    	
	    public NyARRasterAnalyzer_Histogram(int i_raster_format,int i_vertical_interval)
	    {
		    if(!initInstance(i_raster_format,i_vertical_interval)){
			    throw new NyARException();
		    }
	    }
	    protected bool initInstance(int i_raster_format,int i_vertical_interval)
	    {
		    switch (i_raster_format) {
		    case NyARBufferType.BYTE1D_B8G8R8_24:
		    case NyARBufferType.BYTE1D_R8G8B8_24:
			    this._histImpl = new NyARRasterThresholdAnalyzer_Histogram_BYTE1D_RGB_24();
			    break;
		    case NyARBufferType.INT1D_GRAY_8:
			    this._histImpl = new NyARRasterThresholdAnalyzer_Histogram_INT1D_GRAY_8();
			    break;
		    case NyARBufferType.BYTE1D_B8G8R8X8_32:
			    this._histImpl = new NyARRasterThresholdAnalyzer_Histogram_BYTE1D_B8G8R8X8_32();
			    break;
		    case NyARBufferType.BYTE1D_X8R8G8B8_32:
			    this._histImpl = new NyARRasterThresholdAnalyzer_Histogram_BYTE1D_X8R8G8B8_32();
			    break;
		    case NyARBufferType.WORD1D_R5G6B5_16LE:
			    this._histImpl = new NyARRasterThresholdAnalyzer_Histogram_WORD1D_R5G6B5_16LE();
			    break;
		    case NyARBufferType.INT1D_X8R8G8B8_32:
			    this._histImpl = new NyARRasterThresholdAnalyzer_Histogram_INT1D_X8R8G8B8_32();
			    break;
		    default:
			    return false;
		    }
		    //初期化
		    this._vertical_skip=i_vertical_interval;
		    return true;
	    }	
    	
    	
	    public void setVerticalInterval(int i_step)
	    {
		    this._vertical_skip=i_step;
		    return;
	    }

	    /**
	     * o_histogramにヒストグラムを出力します。
	     * @param i_input
	     * @param o_histogram
	     * @return
	     * @throws NyARException
	     */
	    public int analyzeRaster(INyARRaster i_input,NyARHistogram o_histogram)
	    {
    		
		    NyARIntSize size=i_input.getSize();
		    //最大画像サイズの制限
		    Debug.Assert(size.w*size.h<0x40000000);
		    Debug.Assert(o_histogram.length==256);//現在は固定

		    int[] h=o_histogram.data;
		    //ヒストグラム初期化
		    for (int i = o_histogram.length-1; i >=0; i--){
			    h[i] = 0;
		    }
		    o_histogram.total_of_data=size.w*size.h/this._vertical_skip;
		    return this._histImpl.createHistogram(i_input, size,h,this._vertical_skip);		
	    }
    	
	    protected interface ICreateHistogramImpl
	    {
		    int createHistogram(INyARRaster i_reader,NyARIntSize i_size, int[] o_histogram,int i_skip);
	    }

	    class NyARRasterThresholdAnalyzer_Histogram_INT1D_GRAY_8 : ICreateHistogramImpl
	    {
		    public int createHistogram(INyARRaster i_reader,NyARIntSize i_size, int[] o_histogram,int i_skip)
		    {
			    Debug.Assert(i_reader.isEqualBufferType(NyARBufferType.INT1D_GRAY_8));
			    int[] input=(int[]) i_reader.getBuffer();
			    for (int y = i_size.h-1; y >=0 ; y-=i_skip){
				    int pt=y*i_size.w;
				    for (int x = i_size.w-1; x >=0; x--) {
					    o_histogram[input[pt]]++;
					    pt++;
				    }
			    }
			    return i_size.w*i_size.h;
		    }	
	    }
	    class NyARRasterThresholdAnalyzer_Histogram_INT1D_X8R8G8B8_32 : ICreateHistogramImpl
	    {
		    public int createHistogram(INyARRaster i_reader,NyARIntSize i_size, int[] o_histogram,int i_skip)
		    {
			    Debug.Assert (i_reader.isEqualBufferType(NyARBufferType.INT1D_X8R8G8B8_32));
			    int[] input=(int[]) i_reader.getBuffer();
			    for (int y = i_size.h-1; y >=0 ; y-=i_skip){
				    int pt=y*i_size.w;
				    for (int x = i_size.w-1; x >=0; x--) {
					    int p=input[pt];
					    o_histogram[((p& 0xff)+(p& 0xff)+(p& 0xff))/3]++;
					    pt++;
				    }
			    }
			    return i_size.w*i_size.h;
		    }	
	    }


        class NyARRasterThresholdAnalyzer_Histogram_BYTE1D_RGB_24 : ICreateHistogramImpl
	    {
		    public int createHistogram(INyARRaster i_reader,NyARIntSize i_size, int[] o_histogram,int i_skip)
		    {
			    Debug.Assert (
					    i_reader.isEqualBufferType(NyARBufferType.BYTE1D_B8G8R8_24)||
					    i_reader.isEqualBufferType(NyARBufferType.BYTE1D_R8G8B8_24));
			    byte[] input=(byte[]) i_reader.getBuffer();
			    int pix_count=i_size.w;
			    int pix_mod_part=pix_count-(pix_count%8);
			    for (int y = i_size.h-1; y >=0 ; y-=i_skip) {
				    int pt=y*i_size.w*3;
				    int x,v;
				    for (x = pix_count-1; x >=pix_mod_part; x--) {
					    v=((input[pt+0]& 0xff)+(input[pt+1]& 0xff)+(input[pt+2]& 0xff))/3;
					    o_histogram[v]++;
					    pt+=3;
				    }
				    //タイリング
				    for (;x>=0;x-=8){
					    v=((input[pt+ 0]& 0xff)+(input[pt+ 1]& 0xff)+(input[pt+ 2]& 0xff))/3;
					    o_histogram[v]++;
					    v=((input[pt+ 3]& 0xff)+(input[pt+ 4]& 0xff)+(input[pt+ 5]& 0xff))/3;
					    o_histogram[v]++;
					    v=((input[pt+ 6]& 0xff)+(input[pt+ 7]& 0xff)+(input[pt+ 8]& 0xff))/3;
					    o_histogram[v]++;
					    v=((input[pt+ 9]& 0xff)+(input[pt+10]& 0xff)+(input[pt+11]& 0xff))/3;
					    o_histogram[v]++;
					    v=((input[pt+12]& 0xff)+(input[pt+13]& 0xff)+(input[pt+14]& 0xff))/3;
					    o_histogram[v]++;
					    v=((input[pt+15]& 0xff)+(input[pt+16]& 0xff)+(input[pt+17]& 0xff))/3;
					    o_histogram[v]++;
					    v=((input[pt+18]& 0xff)+(input[pt+19]& 0xff)+(input[pt+20]& 0xff))/3;
					    o_histogram[v]++;
					    v=((input[pt+21]& 0xff)+(input[pt+22]& 0xff)+(input[pt+23]& 0xff))/3;
					    o_histogram[v]++;
					    pt+=3*8;
				    }
			    }
			    return i_size.w*i_size.h;
		    }
	    }

	    class NyARRasterThresholdAnalyzer_Histogram_BYTE1D_B8G8R8X8_32 : ICreateHistogramImpl
	    {
		    public int createHistogram(INyARRaster i_reader,NyARIntSize i_size, int[] o_histogram,int i_skip)
		    {
	            Debug.Assert(i_reader.isEqualBufferType(NyARBufferType.BYTE1D_B8G8R8X8_32));
	            byte[] input = (byte[])i_reader.getBuffer();
	            int pix_count = i_size.w;
	            int pix_mod_part = pix_count - (pix_count % 8);
	            for (int y = i_size.h - 1; y >= 0; y -= i_skip)
	            {
	                int pt = y * i_size.w * 4;
	                int x, v;
	                for (x = pix_count - 1; x >= pix_mod_part; x--)
	                {
	                    v = ((input[pt + 0] & 0xff) + (input[pt + 1] & 0xff) + (input[pt + 2] & 0xff)) / 3;
	                    o_histogram[v]++;
	                    pt += 4;
	                }
	                //タイリング
	                for (; x >= 0; x -= 8)
	                {
	                    v = ((input[pt + 0] & 0xff) + (input[pt + 1] & 0xff) + (input[pt + 2] & 0xff)) / 3;
	                    o_histogram[v]++;
	                    v = ((input[pt + 4] & 0xff) + (input[pt + 5] & 0xff) + (input[pt + 6] & 0xff)) / 3;
	                    o_histogram[v]++;
	                    v = ((input[pt + 8] & 0xff) + (input[pt + 9] & 0xff) + (input[pt + 10] & 0xff)) / 3;
	                    o_histogram[v]++;
	                    v = ((input[pt + 12] & 0xff) + (input[pt + 13] & 0xff) + (input[pt + 14] & 0xff)) / 3;
	                    o_histogram[v]++;
	                    v = ((input[pt + 16] & 0xff) + (input[pt + 17] & 0xff) + (input[pt + 18] & 0xff)) / 3;
	                    o_histogram[v]++;
	                    v = ((input[pt + 20] & 0xff) + (input[pt + 21] & 0xff) + (input[pt + 22] & 0xff)) / 3;
	                    o_histogram[v]++;
	                    v = ((input[pt + 24] & 0xff) + (input[pt + 25] & 0xff) + (input[pt + 26] & 0xff)) / 3;
	                    o_histogram[v]++;
	                    v = ((input[pt + 28] & 0xff) + (input[pt + 29] & 0xff) + (input[pt + 30] & 0xff)) / 3;
	                    o_histogram[v]++;
	                    pt += 4 * 8;
	                }
	            }
	            return i_size.w*i_size.h;
	        }
	    }

	    class NyARRasterThresholdAnalyzer_Histogram_BYTE1D_X8R8G8B8_32 : ICreateHistogramImpl
	    {
		    public int createHistogram(INyARRaster i_reader,NyARIntSize i_size, int[] o_histogram,int i_skip)
		    {
	            Debug.Assert(i_reader.isEqualBufferType(NyARBufferType.BYTE1D_X8R8G8B8_32));
	            byte[] input = (byte[])i_reader.getBuffer();
	            int pix_count = i_size.w;
	            int pix_mod_part = pix_count - (pix_count % 8);
	       	    for (int y = i_size.h - 1; y >= 0; y -=i_skip)
	            {
	                int pt = y * i_size.w * 4;
	                int x, v;
	                for (x = pix_count - 1; x >= pix_mod_part; x--)
	                {
	                    v = ((input[pt + 1] & 0xff) + (input[pt + 2] & 0xff) + (input[pt + 3] & 0xff)) / 3;
	                    o_histogram[v]++;
	                    pt += 4;
	                }
	                //タイリング
	                for (; x >= 0; x -= 8)
	                {
	                    v = ((input[pt + 1] & 0xff) + (input[pt + 2] & 0xff) + (input[pt + 3] & 0xff)) / 3;
	                    o_histogram[v]++;
	                    v = ((input[pt + 5] & 0xff) + (input[pt + 6] & 0xff) + (input[pt + 7] & 0xff)) / 3;
	                    o_histogram[v]++;
	                    v = ((input[pt + 9] & 0xff) + (input[pt + 10] & 0xff) + (input[pt + 11] & 0xff)) / 3;
	                    o_histogram[v]++;
	                    v = ((input[pt + 13] & 0xff) + (input[pt + 14] & 0xff) + (input[pt + 15] & 0xff)) / 3;
	                    o_histogram[v]++;
	                    v = ((input[pt + 17] & 0xff) + (input[pt + 18] & 0xff) + (input[pt + 19] & 0xff)) / 3;
	                    o_histogram[v]++;
	                    v = ((input[pt + 21] & 0xff) + (input[pt + 22] & 0xff) + (input[pt + 23] & 0xff)) / 3;
	                    o_histogram[v]++;
	                    v = ((input[pt + 25] & 0xff) + (input[pt + 26] & 0xff) + (input[pt + 27] & 0xff)) / 3;
	                    o_histogram[v]++;
	                    v = ((input[pt + 29] & 0xff) + (input[pt + 30] & 0xff) + (input[pt + 31] & 0xff)) / 3;
	                    o_histogram[v]++;
	                    pt += 4 * 8;
	                }
	            }
	       	    return i_size.w*i_size.h;
	        }
	    }

	    protected class NyARRasterThresholdAnalyzer_Histogram_WORD1D_R5G6B5_16LE : ICreateHistogramImpl
	    {
		    public int createHistogram(INyARRaster i_reader,NyARIntSize i_size, int[] o_histogram,int i_skip)
		    {
	            Debug.Assert(i_reader.isEqualBufferType(NyARBufferType.WORD1D_R5G6B5_16LE));
	            short[] input = (short[])i_reader.getBuffer();
	            int pix_count = i_size.w;
	            int pix_mod_part = pix_count - (pix_count % 8);
	            for (int y = i_size.h - 1; y >= 0; y -= i_skip)
	            {
	                int pt = y * i_size.w;
	                int x, v;
	                for (x = pix_count - 1; x >= pix_mod_part; x--)
	                {
	                    v =(int)input[pt];
	                    v = (((v & 0xf800) >> 8) + ((v & 0x07e0) >> 3) + ((v & 0x001f) << 3))/3;
	                    o_histogram[v]++;
	                    pt++;
	                }
	                //タイリング
	                for (; x >= 0; x -= 8)
	                {
	                    v =(int)input[pt];pt++;
	                    v = (((v & 0xf800) >> 8) + ((v & 0x07e0) >> 3) + ((v & 0x001f) << 3))/3;
	                    o_histogram[v]++;
	                    v =(int)input[pt];pt++;
	                    v = (((v & 0xf800) >> 8) + ((v & 0x07e0) >> 3) + ((v & 0x001f) << 3))/3;
	                    o_histogram[v]++;
	                    v =(int)input[pt];pt++;
	                    v = (((v & 0xf800) >> 8) + ((v & 0x07e0) >> 3) + ((v & 0x001f) << 3))/3;
	                    o_histogram[v]++;
	                    v =(int)input[pt];pt++;
	                    v = (((v & 0xf800) >> 8) + ((v & 0x07e0) >> 3) + ((v & 0x001f) << 3))/3;
	                    o_histogram[v]++;
	                    v =(int)input[pt];pt++;
	                    v = (((v & 0xf800) >> 8) + ((v & 0x07e0) >> 3) + ((v & 0x001f) << 3))/3;
	                    o_histogram[v]++;
	                    v =(int)input[pt];pt++;
	                    v = (((v & 0xf800) >> 8) + ((v & 0x07e0) >> 3) + ((v & 0x001f) << 3))/3;
	                    o_histogram[v]++;
	                    v =(int)input[pt];pt++;
	                    v = (((v & 0xf800) >> 8) + ((v & 0x07e0) >> 3) + ((v & 0x001f) << 3))/3;
	                    o_histogram[v]++;
	                    v =(int)input[pt];pt++;
	                    v = (((v & 0xf800) >> 8) + ((v & 0x07e0) >> 3) + ((v & 0x001f) << 3))/3;
	                    o_histogram[v]++;
	                }
	            }
	            return i_size.w*i_size.h;
	        }
	    }


    }
}
