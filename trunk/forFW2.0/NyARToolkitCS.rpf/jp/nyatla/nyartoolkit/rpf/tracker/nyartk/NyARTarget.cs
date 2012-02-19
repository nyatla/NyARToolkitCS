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
using jp.nyatla.nyartoolkit.cs;
using jp.nyatla.nyartoolkit.cs.core;

namespace jp.nyatla.nyartoolkit.cs.rpf
{
    /**
     * トラッキングターゲットのクラスです。
     * {@link #tag}以外の要素については、ユーザからの直接アクセスを推奨しません。
     *
     */
    public class NyARTarget : NyARManagedObject
    {
	    /**
	     * シリアルID生成時に使うロックオブジェクト。
	     */
        private static object _serial_lock = new object();
	    /**
	     * システム動作中に一意なシリアル番号
	     */
	    private static long _serial_counter=0;
	    /**
	     * 新しいシリアルIDを返します。この値は、NyARTargetを新規に作成したときに、Poolクラスがserialプロパティに設定します。
	     * @return
	     */
	    public static long createSerialId()
	    {
		    lock(NyARTarget._serial_lock){
			    return NyARTarget._serial_counter++;
		    }
	    }
	    ////////////////////////
	    //targetの基本情報
	    /**
	     * ステータスのタイプを表します。この値はref_statusの型と同期しています。
	     */
	    public int _st_type;
	    /**
	     * Targetを識別するID値
	     */
	    public long _serial;
	    /**
	     * 認識サイクルの遅延値。更新ミスの回数と同じ。
	     */
	    public int _delay_tick;

	    /**
	     * 現在のステータスの最大寿命。
	     */
	    public int _status_life;

	    ////////////////////////
	    //targetの情報
	    public NyARTargetStatus _ref_status;
    	
	    /**
	     * ユーザオブジェクトを配置するポインタータグです。リリース時にNULL初期化されます。
	     */
        public object tag;
    //	//Samplerからの基本情報
    	
	    /**
	     * サンプリングエリアを格納する変数です。
	     */
	    public NyARIntRect _sample_area=new NyARIntRect();
	    //アクセス用関数
    	
	    /**
	     * Constructor
	     */
	    public NyARTarget(INyARManagedObjectPoolOperater iRefPoolOperator)
            : base(iRefPoolOperator)
	    {
		    this.tag=null;
	    }
	    /**
	     * この関数は、ref_statusの内容を安全に削除します。
	     */
	    public override int releaseObject()
	    {
		    int ret=base.releaseObject();
		    if(ret==0 && this._ref_status!=null)
		    {
			    this._ref_status.releaseObject();
		    }
		    return ret;
	    }
    	
	    /**
	     * 頂点情報を元に、sampleAreaにRECTを設定します。
	     * @param i_vertex
	     */
	    public void setSampleArea(NyARDoublePoint2d[] i_vertex)
	    {
		    this._sample_area.setAreaRect(i_vertex,4);
	    }	

	    /**
	     * LowResolutionLabelingSamplerOut.Itemの値をを元に、sample_areaにRECTを設定します。
	     * @param i_item
	     * 設定する値です。
	     */
	    public void setSampleArea(LowResolutionLabelingSamplerOut.Item i_item)
	    {
		    this._sample_area.setValue(i_item.base_area);
	    }
    }
}