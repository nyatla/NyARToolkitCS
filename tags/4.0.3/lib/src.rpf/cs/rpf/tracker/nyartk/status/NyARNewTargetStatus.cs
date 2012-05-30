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
    public sealed class NyARNewTargetStatus : NyARTargetStatus
    {

	    public LowResolutionLabelingSamplerOut.Item current_sampleout;
        public NyARNewTargetStatus(INyARManagedObjectPoolOperater i_ref_pool_operator)
            : base(i_ref_pool_operator)
	    {
		    this.current_sampleout=null;
	    }
	    /**
	     * @Override
	     */
	    public override int releaseObject()
	    {
		    int ret=base.releaseObject();
		    if(ret==0 && this.current_sampleout!=null)
		    {
			    this.current_sampleout.releaseObject();
			    this.current_sampleout=null;
		    }
		    return ret;
	    }
	    /**
	     * 値をセットします。この関数は、処理の成功失敗に関わらず、内容変更を行います。
	     * @param i_src
	     * セットするLowResolutionLabelingSamplerOut.Itemを指定します。関数は、このアイテムの参照カウンタをインクリメントします。
	     * @throws NyARException
	     */
	    public void setValue(LowResolutionLabelingSamplerOut.Item i_src)
	    {
		    if(this.current_sampleout!=null){
			    this.current_sampleout.releaseObject();
		    }
		    if(i_src!=null){
                this.current_sampleout = (LowResolutionLabelingSamplerOut.Item)i_src.referenceObject();
		    }else{
			    this.current_sampleout=null;
		    }
	    }
    	
    }
}
