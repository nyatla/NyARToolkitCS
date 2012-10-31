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
using System.IO;

namespace jp.nyatla.nyartoolkit.cs.rpf
{

    /**
     * 簡易なARToolKitパターンテーブルです。
     * このクラスは、ARToolKitスタイルのパターンファイルとIdとメタデータセットテーブルを定義します。
     */
    public class ARTKMarkerTable
    {
	    /**
	     * selectTarget関数の戻り値を格納します。
	     * 入れ子クラスの作れない処理系では、ARTKMarkerTable_GetBestMatchTargetResultとして宣言してください。
	     */
	    public class GetBestMatchTargetResult
	    {
		    /** 登録時に設定したIDです。*/
		    public int idtag;
		    /** 登録時に設定した名前です。*/
		    public String name;
		    /** 登録時に設定したマーカサイズです。*/
		    public double marker_width;
		    /** 登録時に設定したマーカサイズです。*/
		    public double marker_height;
		    /** ARToolKit準拠の、マーカの方位値*/
		    public int artk_direction;
		    /** 一致率*/
		    public double confidence;
	    }
    	

	    private class MarkerTable : NyARObjectStack<MarkerTable.SerialTableRow>
	    {
		    public class SerialTableRow
		    {
			    public int idtag;
			    public String name;
			    public NyARCode code;
			    public double marker_width;
			    public double marker_height;
			    public void setValue(NyARCode i_code,int i_idtag,String i_name,double i_width,double i_height)
			    {
				    this.code=i_code;
				    this.marker_height=i_height;
				    this.marker_width=i_width;
				    this.name=i_name;
				    this.idtag=i_idtag;
			    }
		    }		
		    public MarkerTable(int i_length)
		    {
			    base.initInstance(i_length);
		    }
		    protected override SerialTableRow createElement()
		    {
			    return new SerialTableRow();
		    }
	    }
	    private int _resolution_width;
	    private int _resolution_height;
	    private int _edge_x;
	    private int _edge_y;
	    private int _sample_per_pix;
	    private NyARRgbRaster _tmp_raster;
	    private NyARMatchPatt_Color_WITHOUT_PCA _match_patt;
	    private NyARMatchPattDeviationColorData _deviation_data;
	    private MarkerTable _table;
	    /**
	     * コンストラクタです。
	     * @param i_max
	     * 登録するアイテムの最大数です。
	     * @param i_resolution_x
	     * 登録するパターンの解像度です。
	     * ARToolKit互換の標準値は16です。
	     * @param i_resolution_y
	     * 登録するパターンの解像度です。
	     * ARToolKit互換の標準値は16です。
	     * @param i_edge_x
	     * エッジ部分の割合です。ARToolKit互換の標準値は25です。
	     * @param i_edge_y
	     * エッジ部分の割合です。ARToolKit互換の標準値は25です。
	     * @param i_sample_per_pix
	     * パターン取得の1ピクセルあたりのサンプリング数です。1なら1Pixel=1,2なら1Pixel=4のサンプリングをします。
	     * ARToolKit互換の標準値は4です。
	     * 高解像度(64以上)のパターンを用いるときは、サンプリング数を低く設定してください。
	     * @throws NyARException 
	     */
	    public ARTKMarkerTable(int i_max,int i_resolution_x,int i_resolution_y,int i_edge_x,int i_edge_y,int i_sample_per_pix)
	    {
		    this._resolution_width=i_resolution_x;
		    this._resolution_height=i_resolution_y;
		    this._edge_x=i_edge_x;
		    this._edge_y=i_edge_y;
		    this._sample_per_pix=i_sample_per_pix;
		    this._tmp_raster=new NyARRgbRaster(i_resolution_x,i_resolution_y,NyARBufferType.INT1D_X8R8G8B8_32);
		    this._table=new MarkerTable(i_max);
		    this._deviation_data=new NyARMatchPattDeviationColorData(i_resolution_x,i_resolution_y);		
		    this._match_patt=new NyARMatchPatt_Color_WITHOUT_PCA(i_resolution_x,i_resolution_y);
	    }
	    /**
	     * ARTKパターンコードを、テーブルに追加します。このパターンコードのメタデータとして、IDと名前を指定できます。
	     * @param i_code
	     * ARToolKit形式のパターンコードを格納したオブジェクト。このオブジェクトは、関数成功後はインスタンスに所有されます。
	     * パターンコードの解像度は、コンストラクタに指定した高さと幅である必要があります。
	     * @param i_id
	     * このマーカを識別するユーザ定義のID値です。任意の値を指定できます。不要な場合は0を指定してください。
	     * @param i_name
	     * ユーザ定義の名前です。任意の値を指定できます。不要な場合はnullを指定して下さい。
	     * @param i_width
	     * マーカの高さ[通常mm単位]
	     * @param i_height
	     * マーカの幅[通常mm単位]
	     * @return
	     */
        public bool addMarker(NyARCode i_code, int i_id, String i_name, double i_width, double i_height)
	    {
		    Debug.Assert(i_code.getHeight()== this._resolution_height && i_code.getHeight()== this._resolution_width);
		    MarkerTable.SerialTableRow d=this._table.prePush();
		    if(d==null){
			    return false;
		    }
		    d.setValue(i_code,i_id,i_name,i_width,i_height);
		    return true;
	    }
	    /**
	     * i_rasterからパターンコードを生成して、テーブルへ追加します。
	     * @param i_raster
	     * @param i_id
	     * このマーカを識別するユーザ定義のID値です。任意の値を指定できます。不要な場合は0を指定してください。
	     * @param i_name
	     * ユーザ定義の名前です。任意の値を指定できます。不要な場合はnullを指定して下さい。
	     * @param i_width
	     * マーカの高さ[通常mm単位]
	     * @param i_height
	     * マーカの幅[通常mm単位]
	     * @return
	     * @throws NyARException
	     */
        public bool addMarker(NyARRgbRaster i_raster, int i_id, String i_name, double i_width, double i_height)
	    {
		    MarkerTable.SerialTableRow d=this._table.prePush();
		    if(d==null){
			    return false;
		    }
            NyARCode c = new NyARCode(this._resolution_width, this._resolution_height);
		    c.setRaster(i_raster);
		    d.setValue(c,i_id,i_name,i_width,i_height);
		    return true;
	    }
	    /**
	     * ARToolkit準拠のパターンファイルからパターンコードを生成して、テーブルへ追加します。
	     * @param i_filename
	     * @param i_id
	     * このマーカを識別するユーザ定義のID値です。任意の値を指定できます。不要な場合は0を指定してください。
	     * @param i_name
	     * ユーザ定義の名前です。任意の値を指定できます。不要な場合はnullを指定して下さい。
	     * @param i_width
	     * マーカの高さ[通常mm単位]
	     * @param i_height
	     * マーカの幅[通常mm単位]
	     * @return
	     * @throws NyARException
	     */
	    public bool addMarkerFromARPatt(StreamReader i_stream,int i_id,String i_name,double i_width,double i_height)
	    {
		    MarkerTable.SerialTableRow d=this._table.prePush();
		    if(d==null){
			    return false;
		    }
            NyARCode c = NyARCode.createFromARPattFile(i_stream, this._resolution_width, this._resolution_height);
		    d.setValue(c,i_id,i_name,i_width,i_height);
		    return true;
	    }	
    	
	    private NyARMatchPattResult __tmp_patt_result=new NyARMatchPattResult();
	    /**
	     * RealityTargetに最も一致するパターンをテーブルから検索して、メタデータを返します。
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
        public bool getBestMatchTarget(NyARRealityTarget i_target, NyARRealitySource i_rtsorce, GetBestMatchTargetResult o_result)
	    {
		    //パターン抽出
		    NyARMatchPattResult tmp_patt_result=this.__tmp_patt_result;
            INyARPerspectiveCopy r = i_rtsorce.refPerspectiveRasterReader();
            r.copyPatt(i_target.refTargetVertex(), this._edge_x, this._edge_y, this._sample_per_pix, this._tmp_raster);
            //比較パターン生成
		    this._deviation_data.setRaster(this._tmp_raster);
		    int ret=-1;
		    int dir=-1;
		    double cf=Double.MinValue;
		    for(int i=this._table.getLength()-1;i>=0;i--){
			    this._match_patt.setARCode(this._table.getItem(i).code);
			    this._match_patt.evaluate(this._deviation_data, tmp_patt_result);
			    if(cf<tmp_patt_result.confidence){
				    ret=i;
				    cf=tmp_patt_result.confidence;
				    dir=tmp_patt_result.direction;
			    }
		    }
		    if(ret<0){
			    return false;
		    }
		    //戻り値を設定
		    MarkerTable.SerialTableRow row=this._table.getItem(ret);
		    o_result.artk_direction=dir;
		    o_result.confidence=cf;
		    o_result.idtag=row.idtag;
		    o_result.marker_height=row.marker_height;
		    o_result.marker_width=row.marker_width;
		    o_result.name=row.name;
		    return true;
	    }
    }
}