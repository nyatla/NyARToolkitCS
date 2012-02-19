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
    public class NyARRealityTargetPool : NyARManagedObjectPool<NyARRealityTarget>
    {
	    //targetでの共有オブジェクト
	    public NyARPerspectiveProjectionMatrix _ref_prj_mat;
	    /** Target間での共有ワーク変数。*/
	    public NyARDoublePoint3d[] _wk_da3_4=NyARDoublePoint3d.createArray(4);
	    public NyARDoublePoint2d[] _wk_da2_4=NyARDoublePoint2d.createArray(4);
    	
	    public NyARRealityTargetPool(int i_size,NyARPerspectiveProjectionMatrix i_ref_prj_mat)
	    {
		    this.initInstance(i_size);
		    this._ref_prj_mat=i_ref_prj_mat;
		    return;
	    }
	    protected override NyARRealityTarget createElement()
	    {
		    return new NyARRealityTarget(this);
	    }
	    /**
	     * 新しいRealityTargetを作って返します。
	     * @param tt
	     * @return
	     * @throws NyARException 
	     */
	    public NyARRealityTarget newNewTarget(NyARTarget tt)
	    {
		    NyARRealityTarget ret=base.newObject();
		    if(ret==null){
			    return null;
		    }
		    ret.grab_rate=50;//開始時の捕捉レートは10%
		    ret._ref_tracktarget=(NyARTarget) tt.referenceObject();
		    ret._serial=NyARRealityTarget.createSerialId();
		    ret.tag=null;
		    tt.tag=ret;//トラックターゲットのタグに自分の値設定しておく。
		    return ret;
	    }	
    }
}