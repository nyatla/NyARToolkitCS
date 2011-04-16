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
    public interface INyARRaster
    {
    	
	    int getWidth();
	    int getHeight();
	    NyARIntSize getSize();
	    /**
	     * バッファオブジェクトを返します。
	     * @return
	     */
	    object getBuffer();
	    /**
	     * バッファオブジェクトのタイプを返します。
	     * @return
	     */
	    int getBufferType();
	    /**
	     * バッファのタイプがi_type_valueであるか、チェックします。
	     * @param i_type_value
	     * @return
	     */
	    bool isEqualBufferType(int i_type_value);
	    /**
	     * getBufferがオブジェクトを返せるかの真偽値です。
	     * @return
	     */
	    bool hasBuffer();
	    /**
	     * i_ref_bufをラップします。できる限り整合性チェックを行います。
	     * バッファの再ラッピングが可能な関数のみ、この関数を実装してください。
	     * @param i_ref_buf
	     */
	    void wrapBuffer(object i_ref_buf);
    }
}