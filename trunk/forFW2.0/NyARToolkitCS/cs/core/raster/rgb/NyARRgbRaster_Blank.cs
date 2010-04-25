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
namespace jp.nyatla.nyartoolkit.cs.core
{
    /*
     * 真っ黒の矩形を定義する。
     * 
     */
    public class NyARRgbRaster_Blank : NyARRgbRaster_BasicClass
    {
	    private class PixelReader : INyARRgbPixelReader
	    {
		    public void getPixel(int i_x, int i_y, int[] o_rgb)
		    {
			    o_rgb[0] = 0;// R
			    o_rgb[1] = 0;// G
			    o_rgb[2] = 0;// B
			    return;
		    }

		    public void getPixelSet(int[] i_x, int[] i_y, int i_num, int[] o_rgb)
		    {
			    for (int i = i_num - 1; i >= 0; i--) {
				    o_rgb[i * 3 + 0] = 0;// R
				    o_rgb[i * 3 + 1] = 0;// G
				    o_rgb[i * 3 + 2] = 0;// B
			    }
		    }
		    public void setPixel(int i_x, int i_y, int[] i_rgb)
		    {
			    NyARException.notImplement();		
		    }
		    public void setPixels(int[] i_x, int[] i_y, int i_num, int[] i_intrgb)
		    {
			    NyARException.notImplement();		
		    }
		    public void switchBuffer(object i_ref_buffer)
		    {
			    NyARException.notImplement();		
		    }		
    		
	    }

	    private INyARRgbPixelReader _reader;

        public NyARRgbRaster_Blank(int i_width, int i_height)
            : base(i_width, i_height, NyARBufferType.NULL_ALLZERO)
	    {
		    this._reader = new PixelReader();
		    return;
	    }
        public override INyARRgbPixelReader getRgbPixelReader()
	    {
		    return this._reader;
	    }
        public override object getBuffer()
	    {
		    return null;
	    }
        public override bool hasBuffer()
	    {
		    return false;
	    }
        public override void wrapBuffer(object i_ref_buf)
	    {
		    NyARException.notImplement();
	    }
    }
}
