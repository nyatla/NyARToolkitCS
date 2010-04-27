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
    public abstract class NyARRaster_BasicClass : INyARRaster
    {
	    protected NyARIntSize _size;
	    private int _buffer_type;
        protected NyARRaster_BasicClass(int i_width, int i_height, int i_buffer_type)
	    {
            this._size = new NyARIntSize(i_width, i_height);
		    this._buffer_type=i_buffer_type;
	    }

        public int getWidth()
	    {
		    return this._size.w;
	    }

        public int getHeight()
	    {
		    return this._size.h;
	    }

        public NyARIntSize getSize()
	    {
		    return this._size;
	    }
        public int getBufferType()
	    {
		    return _buffer_type;
	    }
        public bool isEqualBufferType(int i_type_value)
	    {
		    return this._buffer_type==i_type_value;
	    }
        public abstract bool hasBuffer();
        public abstract void wrapBuffer(object i_ref_buf);
        public abstract object getBuffer();

    }
}