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
using jp.nyatla.nyartoolkit.cs.nyidmarker;

namespace jp.nyatla.nyartoolkit.cs.rpf
{
    /**
     * 簡易な同期型NyIdマーカIDテーブルです。
     * このクラスは、RawBitフォーマットドメインのNyIdマーカのIdとメタデータセットテーブルを定義します。
     * SerialIDは、RawBitマーカのデータパケットを、[0][1]...[n]の順に並べて、64bitの整数値に変換した値です。
     * 判別できるIdマーカは、domain=0(rawbit),model&lt;5,mask=0のもののみです。
     * <p>
     * このクラスは、NyRealityTargetをRawBitフォーマットドメインのSerialNumberマーカにエンコードする
     * 機能を提供します。
     * 使い方は、ユーザは、このクラスにIDマーカのSerialNumberとそのサイズを登録します。その後に、
     * NyRealityTargetをキーに、登録したデータからそのSerialNumberをサイズを得ることができます。
     * </p>
     * 
     * NyIdRawBitSerialNumberTable
     */
    public class RawbitSerialIdTable
    {
	    /**
	     * selectTarget関数の戻り値を格納します。
	     * 入れ子クラスの作れない処理系では、RawbitSerialIdTable_IdentifyIdResultとして宣言してください。
	     */
	    public class IdentifyIdResult
	    {
		    /** ID番号です。*/
		    public long id;
		    /** 名前です。*/
		    public String name;
		    /** 登録時に設定したマーカサイズです。*/
		    public double marker_width;
		    /** ARToolKit準拠の、マーカの方位値です。*/
		    public int artk_direction;
	    }
    	

	    private class SerialTable : NyARObjectStack<SerialTable.SerialTableRow>
	    {
		    public class SerialTableRow
		    {
			    public long id_st;
			    public long id_ed;
			    public double marker_width;
			    public String name;
			    public void setValue(String i_name,long i_st,long i_ed,double i_width)
			    {
				    this.id_ed=i_ed;
				    this.id_st=i_st;
				    this.marker_width=i_width;
				    this.name=i_name;
			    }
		    }		
		    public SerialTable(int i_length)
		    {
			    base.initInstance(i_length);
		    }
		    protected override SerialTableRow createElement()
		    {
			    return new SerialTableRow();
		    }
		    public SerialTableRow getItembySerialId(long i_serial)
		    {
			    for(int i=this._length-1;i>=0;i--)
			    {
				    SerialTableRow s=this._items[i];
				    if(i_serial<s.id_st || i_serial>s.id_ed){
					    continue;
				    }
				    return s;
			    }
			    return null;
		    }
	    }

	    private SerialTable _table;
	    private readonly NyIdMarkerPickup _id_pickup;
	    private NyIdMarkerPattern _temp_nyid_info=new NyIdMarkerPattern();
	    private NyIdMarkerParam _temp_nyid_param=new NyIdMarkerParam();
    	
	    private NyIdMarkerDataEncoder_RawBitId _rb=new NyIdMarkerDataEncoder_RawBitId();
	    private NyIdMarkerData_RawBitId _rb_dest=new NyIdMarkerData_RawBitId();


	    /**
	     * コンストラクタです。
	     * @param i_max
	     * 登録するアイテムの最大数です。
	     * @throws NyARException 
	     */
	    public RawbitSerialIdTable(int i_max)
	    {
            this._id_pickup = new NyIdMarkerPickup();
		    this._table=new SerialTable(i_max);
	    }
	    /**
	     * IDの範囲に対するメタデータセットを、テーブルに追加します。
	     * この要素にヒットする範囲は,i_st&lt;=n&lt;=i_edになります。
	     * @param i_name
	     * このID範囲の名前を指定します。不要な場合はnullを指定します。
	     * @param i_st
	     * ヒット範囲の開始値です。
	     * @param i_ed
	     * ヒット範囲の終了値です。
	     * @param　i_width
	     * ヒットしたマーカのサイズ値を指定します。
	     */
        public bool addSerialIdRangeItem(String i_name, long i_st, long i_ed, double i_width)
	    {
		    SerialTable.SerialTableRow d=this._table.prePush();
		    if(d==null){
			    return false;
		    }
		    d.setValue(i_name,i_st,i_ed,i_width);
		    return true;
	    }
	    /**
	     * SerialIDに対するメタデータセットを、テーブルに追加します。
	     * @param i_serial
	     * ヒットさせるシリアルidです。
	     * @param i_width
	     * ヒットしたマーカのサイズ値です。
	     * @return
	     * 登録に成功するとtrueを返します。
	     */
        public bool addSerialIdItem(String i_name, long i_serial, double i_width)
	    {
		    SerialTable.SerialTableRow d=this._table.prePush();
		    if(d==null){
			    return false;
		    }
		    d.setValue(i_name,i_serial,i_serial,i_width);
		    return true;
	    }
	    /**
	     * 全てのSerialIDにヒットするメタデータセットを、テーブルに追加します。
	     * @param i_width
	     * ヒットしたマーカのサイズ値です。
	     * @return
	     * 登録に成功するとtrueです。
	     */
        public bool addAnyItem(String i_name, double i_width)
	    {
		    SerialTable.SerialTableRow d=this._table.prePush();
		    if(d==null){
			    return false;
		    }
		    d.setValue(i_name,0,long.MaxValue,i_width);
		    return true;
	    }
        private INyARRaster _last_laster = null;
        private INyARGsPixelDriver _gs_pix_reader;
	
	    /**
	     * i_raster上にあるi_vertexの頂点で定義される四角形のパターンから、一致するID値を特定します。
	     * @param i_vertex
	     * 4頂点の座標
	     * @param i_raster
	     * @param o_result
	     * @return
	     * @throws NyARException
	     */
	    public bool identifyId(NyARDoublePoint2d[] i_vertex,INyARRgbRaster i_raster,IdentifyIdResult o_result)
	    {
            if (this._last_laster != i_raster)
            {
                this._gs_pix_reader = NyARGsPixelDriverFactory.createDriver(i_raster);
                this._last_laster = i_raster;
            }
            if (!this._id_pickup.pickFromRaster(this._gs_pix_reader, i_vertex, this._temp_nyid_info, this._temp_nyid_param))
            {
                return false;
            }
            if (!this._rb.encode(this._temp_nyid_info, this._rb_dest))
            {
                return false;
            }
            //SerialID引きする。
            SerialTable.SerialTableRow d = this._table.getItembySerialId(this._rb_dest.marker_id);
            if (d == null)
            {
                return false;
            }
            //戻り値を設定
            o_result.marker_width = d.marker_width;
            o_result.id = this._rb_dest.marker_id;
            o_result.artk_direction = this._temp_nyid_param.direction;
            o_result.name = d.name;
            return true;	
	    }
	    /**
	     * RealityTargetに一致するID値を特定します。
	     * 複数のパターンにヒットしたときは、一番初めにヒットした項目を返します。
	     * @param i_target
	     * Realityが検出したターゲット。
	     * Unknownターゲットを指定すること。
	     * @param i_rtsorce
	     * i_targetを検出したRealitySourceインスタンス。
	     * @param o_result
	     * 返却値を格納するインスタンスを設定します。
	     * 返却値がtrueの場合のみ、内容が更新されています。
	     * @return
	     * 特定に成功すると、trueを返します。
	     * @throws NyARException 
	     */
	    public bool identifyId(NyARRealityTarget i_target,NyARRealitySource i_rtsorce,IdentifyIdResult o_result)
	    {
		    //NyARDoublePoint2d[] i_vertex,NyARRgbRaster i_raster,SelectResult o_result
		    return this.identifyId(
			    ((NyARRectTargetStatus)(i_target._ref_tracktarget._ref_status)).vertex,
			    i_rtsorce.refRgbSource(),
			    o_result);
	    }
	    //指定したIDとパターンが一致するか確認するAPIも用意するか？
    }
}