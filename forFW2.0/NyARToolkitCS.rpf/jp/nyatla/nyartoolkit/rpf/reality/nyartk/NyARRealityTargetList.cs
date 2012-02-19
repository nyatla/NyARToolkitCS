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
    public class NyARRealityTargetList : NyARPointerStack<NyARRealityTarget>
    {
	    public NyARRealityTargetList(int i_max_target)
	    {
		    base.initInstance(i_max_target);
	    }
	    /**
	     * RealityTargetのシリアル番号をキーに、ターゲットを探索します。
	     * @param i_serial
	     * @return
	     */
	    public NyARRealityTarget getItemBySerial(long i_serial)
	    {
		    NyARRealityTarget[] items=this._items;
		    for(int i=this._length-1;i>=0;i--)
		    {
			    if(items[i]._serial==i_serial){
				    return items[i];
			    }
		    }
		    return null;
	    }
	    /**
	     * シリアルIDがi_serialに一致するターゲットのインデクス番号を返します。
	     * @param i_serial
	     * @return
	     * @throws NyARException
	     */
	    public int getIndexBySerial(int i_serial)
	    {
		    NyARRealityTarget[] items=this._items;
		    for(int i=this._length-1;i>=0;i--)
		    {
			    if(items[i]._serial==i_serial){
				    return i;
			    }
		    }
		    return -1;
	    }
	    /**
	     * リストから特定のタイプのターゲットだけを選択して、一括でo_resultへ返します。
	     * @param i_type
	     * ターゲットタイプです。NyARRealityTarget.RT_*を指定してください。
	     * @param o_list
	     * 選択したターゲットを格納する配列です。
	     * @return
	     * 選択できたターゲットの個数です。o_resultのlengthと同じ場合、取りこぼしが発生した可能性があります。
	     */	
	    public int selectTargetsByType(int i_type,NyARRealityTarget[] o_result)
	    {
		    int num=0;
            for (int i = this._length - 1; i >= 0 && num < o_result.Length; i--)
		    {
			    if(this._items[i]._target_type!=i_type){
				    continue;
			    }
			    o_result[num]=this._items[i];
			    num++;
		    }
		    return num;
	    }
	    /**
	     * リストから特定のタイプのターゲットを1個選択して、返します。
	     * @param i_type
	     * ターゲットタイプです。NyARRealityTarget.RT_*を指定してください。
	     * @return
	     * 見つかるとターゲットへの参照を返します。見つからなければNULLです。
	     */
	    public NyARRealityTarget selectSingleTargetByType(int i_type)
	    {
		    for(int i=this._length-1;i>=0;i--)
		    {
			    if(this._items[i]._target_type!=i_type){
				    continue;
			    }
			    return this._items[i];
		    }
		    return null;
	    }	
    }
}