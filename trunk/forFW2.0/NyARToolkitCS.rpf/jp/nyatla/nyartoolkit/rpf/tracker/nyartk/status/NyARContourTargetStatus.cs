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
using System.Diagnostics;
using jp.nyatla.nyartoolkit.cs.core;

namespace jp.nyatla.nyartoolkit.cs.rpf

{
    /**
     * 輪郭ソース1個を格納するクラスです。
     *
     */
    public sealed class NyARContourTargetStatus : NyARTargetStatus
    {
	    /**
	     * ベクトル要素を格納する配列です。
	     */
	    public VecLinearCoordinates vecpos=new VecLinearCoordinates(100);

    	
    	
	    //
	    //制御部

	    /**
	     * @param i_ref_pool_operator
	     * @param i_shared
	     * 共有ワークオブジェクトを指定します。
	     * 
	     */
	    public NyARContourTargetStatus(INyARManagedObjectPoolOperater i_ref_pool_operator):
	        base(i_ref_pool_operator)
	    {
	    }
	    /**
	     * @param i_vecreader
	     * @param i_sample
	     * @return
	     * @throws NyARException
	     */
        public bool setValue(INyARVectorReader i_vecreader, LowResolutionLabelingSamplerOut.Item i_sample)
	    {
		    return i_vecreader.traceConture(i_sample.lebeling_th, i_sample.entry_pos, this.vecpos);
	    }	
    }
}