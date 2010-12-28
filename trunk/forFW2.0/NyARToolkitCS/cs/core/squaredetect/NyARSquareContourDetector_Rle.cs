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
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace jp.nyatla.nyartoolkit.cs.core
{
    public abstract class NyARSquareContourDetector_Rle : NyARSquareContourDetector
    {
	    /**
	     * label_stackにソート後の結果を蓄積するクラス
	     */
	    class Labeling : NyARLabeling_Rle
	    {
		    public NyARRleLabelFragmentInfoPtrStack label_stack;
		    int _right;
		    int _bottom;


            public Labeling(int i_width, int i_height)
                : base(i_width, i_height)
		    {
			    this.label_stack=new NyARRleLabelFragmentInfoPtrStack(i_width*i_height*2048/(320*240)+32);//検出可能な最大ラベル数
			    this._bottom=i_height-1;
			    this._right=i_width-1;
			    return;
		    }
		    public override void labeling(NyARGrayscaleRaster i_raster,NyARIntRect i_area,int i_th)
		    {
			    //配列初期化
			    this.label_stack.clear();
			    //ラベルの検出
			    base.labeling(i_raster, i_area, i_th);
			    //ソート
			    this.label_stack.sortByArea();
		    }
            public override void labeling(NyARBinRaster i_bin_raster)
		    {
			    //配列初期化
			    this.label_stack.clear();
			    //ラベルの検出
			    base.labeling(i_bin_raster);
			    //ソート
			    this.label_stack.sortByArea();			
		    }
    		
		    protected override void onLabelFound(NyARRleLabelFragmentInfo i_label)
		    {
			    // クリップ領域が画面の枠に接していれば除外
			    if (i_label.clip_l == 0 || i_label.clip_r == this._right){
				    return;
			    }
			    if (i_label.clip_t == 0 || i_label.clip_b == this._bottom){
				    return;
			    }
			    this.label_stack.push(i_label);
		    }
    		
	    }
    	
	    private int _width;
	    private int _height;

	    private Labeling _labeling;

	    private NyARLabelOverlapChecker<NyARRleLabelFragmentInfo> _overlap_checker = new NyARLabelOverlapChecker<NyARRleLabelFragmentInfo>(32);
	    private NyARContourPickup _cpickup=new NyARContourPickup();

	    private NyARCoord2SquareVertexIndexes _coord2vertex=new NyARCoord2SquareVertexIndexes();
    	
	    private NyARIntCoordinates _coord;
	    /**
	     * コンストラクタ
	     * @param i_size
	     * 入力画像のサイズ
	     */
	    public NyARSquareContourDetector_Rle(NyARIntSize i_size)
	    {
		    //特性確認
		    Debug.Assert(NyARLabeling_Rle._sf_label_array_safe_reference);
		    this._width = i_size.w;
		    this._height = i_size.h;
		    //ラベリングのサイズを指定したいときはsetAreaRangeを使ってね。
		    this._labeling = new Labeling(this._width,this._height);		

		    // 輪郭の最大長は画面に映りうる最大の長方形サイズ。
		    int number_of_coord = (this._width + this._height) * 2;

		    // 輪郭バッファ
		    this._coord = new NyARIntCoordinates(number_of_coord);
		    return;
	    }

	    private int[] __detectMarker_mkvertex = new int[4];
	    public void detectMarker(NyARGrayscaleRaster i_raster,NyARIntRect i_area,int i_th)
	    {
		    Debug.Assert(i_area.w*i_area.h>0);
    		
		    NyARRleLabelFragmentInfoPtrStack flagment=this._labeling.label_stack;
		    NyARLabelOverlapChecker<NyARRleLabelFragmentInfo> overlap = this._overlap_checker;

		    // ラベル数が0ならここまで
		    this._labeling.labeling(i_raster, i_area, i_th);
		    int label_num=flagment.getLength();
		    if (label_num < 1) {
			    return;
		    }

		    //ラベルリストを取得
		    NyARRleLabelFragmentInfo[] labels=flagment.getArray();

		    NyARIntCoordinates coord = this._coord;
		    int[] mkvertex =this.__detectMarker_mkvertex;


		    //重なりチェッカの最大数を設定
		    overlap.setMaxLabels(label_num);

		    for (int i=0; i < label_num; i++) {
			    NyARRleLabelFragmentInfo label_pt=labels[i];
			    // 既に検出された矩形との重なりを確認
			    if (!overlap.check(label_pt)) {
				    // 重なっているようだ。
				    continue;
			    }
    			
			    //輪郭を取得
			    if(!this._cpickup.getContour(i_raster,i_area, i_th,label_pt.entry_x,label_pt.clip_t,coord))
			    {
				    continue;
			    }
			    int label_area = label_pt.area;
			    //輪郭線をチェックして、矩形かどうかを判定。矩形ならばmkvertexに取得
			    if (!this._coord2vertex.getVertexIndexes(coord,label_area,mkvertex)){
				    // 頂点の取得が出来なかった
				    continue;
			    }
			    //矩形を発見したことをコールバック関数で通知
			    this.onSquareDetect(coord,mkvertex);

			    // 検出済の矩形の属したラベルを重なりチェックに追加する。
			    overlap.push(label_pt);
    		
		    }
		    return;
	    }
	    /**
	     * @override
	     */
	    public override void detectMarker(NyARBinRaster i_raster)
	    {
		    NyARRleLabelFragmentInfoPtrStack flagment=this._labeling.label_stack;
		    NyARLabelOverlapChecker<NyARRleLabelFragmentInfo> overlap = this._overlap_checker;

		    // ラベル数が0ならここまで
		    flagment.clear();
		    this._labeling.labeling(i_raster);
		    int label_num=flagment.getLength();
		    if (label_num < 1) {
			    return;
		    }
		    //ラベルをソートしておく
		    flagment.sortByArea();
		    //ラベルリストを取得
		    NyARRleLabelFragmentInfo[] labels=flagment.getArray();

		    NyARIntCoordinates coord = this._coord;
		    int[] mkvertex =this.__detectMarker_mkvertex;


		    //重なりチェッカの最大数を設定
		    overlap.setMaxLabels(label_num);

		    for (int i=0; i < label_num; i++) {
			    NyARRleLabelFragmentInfo label_pt=labels[i];
			    int label_area = label_pt.area;
    		
			    // 既に検出された矩形との重なりを確認
			    if (!overlap.check(label_pt)) {
				    // 重なっているようだ。
				    continue;
			    }
    			
			    //輪郭を取得
			    if(!this._cpickup.getContour(i_raster,label_pt.entry_x,label_pt.clip_t,coord)){
				    continue;
			    }
			    //輪郭線をチェックして、矩形かどうかを判定。矩形ならばmkvertexに取得
			    if (!this._coord2vertex.getVertexIndexes(coord,label_area, mkvertex)) {
				    // 頂点の取得が出来なかった
				    continue;
			    }
			    //矩形を発見したことをコールバック関数で通知
			    this.onSquareDetect(coord,mkvertex);

			    // 検出済の矩形の属したラベルを重なりチェックに追加する。
			    overlap.push(label_pt);
    		
		    }
		    return;
	    }
	    /**
	     * デバック用API
	     * @return
	     */
	    public Object[] _probe()
	    {
		    Object[] ret=new Object[10];
		    return ret;
	    }

    }

}
