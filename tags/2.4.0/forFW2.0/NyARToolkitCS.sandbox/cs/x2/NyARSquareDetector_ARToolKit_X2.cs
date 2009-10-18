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
using System.Diagnostics;
using System.Text;
using jp.nyatla.nyartoolkit.cs.core;
using jp.nyatla.nyartoolkit.cs.core2;
using jp.nyatla.nyartoolkit.cs.utils;

namespace jp.nyatla.nyartoolkit.cs.sandbox.x2
{


    /**
     * イメージから正方形候補を検出するクラス。
     * このクラスは、arDetectMarker2.cとの置き換えになります。
     * 
     */
    public class NyARSquareDetector_ARToolKit_X2 : INyARSquareDetector
    {
        private const int AR_AREA_MAX = 100000;// #define AR_AREA_MAX 100000

        private const int AR_AREA_MIN = 70;// #define AR_AREA_MIN 70
        private int _width;
        private int _height;

        private NyARLabeling_ARToolKit _labeling;

        private NyARLabelingImage _limage;

        private LabelOverlapChecker<NyARLabelingLabel> _overlap_checker = new LabelOverlapChecker<NyARLabelingLabel>(32);
        private SquareContourDetector_X2 _sqconvertor;
	    private ContourPickup _cpickup=new ContourPickup();

        /**
         * 最大i_squre_max個のマーカーを検出するクラスを作成する。
         * 
         * @param i_param
         */
        public NyARSquareDetector_ARToolKit_X2(NyARCameraDistortionFactor i_dist_factor_ref, NyARIntSize i_size)
        {
            this._width = i_size.w;
            this._height = i_size.h;
            this._labeling = new NyARLabeling_ARToolKit();
            this._sqconvertor = new SquareContourDetector_X2(i_size, i_dist_factor_ref);
            this._limage = new NyARLabelingImage(this._width, this._height);


            // 輪郭の最大長は画面に映りうる最大の長方形サイズ。
            int number_of_coord = (this._width + this._height) * 2;

            // 輪郭バッファは頂点変換をするので、輪郭バッファの２倍取る。
            this._max_coord = number_of_coord;
            this._xcoord = new int[number_of_coord * 2];
            this._ycoord = new int[number_of_coord * 2];
        }

        private int _max_coord;
        private int[] _xcoord;
        private int[] _ycoord;

        /**
         * arDetectMarker2を基にした関数
         * この関数はNyARSquare要素のうち、directionを除くパラメータを取得して返します。
         * directionの確定は行いません。
         * @param i_raster
         * 解析する２値ラスタイメージを指定します。
         * @param o_square_stack
         * 抽出した正方形候補を格納するリスト
         * @throws NyARException
         */
        public void detectMarker(NyARBinRaster i_raster, NyARSquareStack o_square_stack)
        {
            NyARLabelingImage limage = this._limage;

            // 初期化

            // マーカーホルダをリセット
            o_square_stack.clear();
            // ラベリング
            this._labeling.labeling(i_raster, this._limage);
            
            // ラベル数が0ならここまで(Labeling内部でソートするようにした。)
            int label_num = this._labeling.labeling(i_raster, limage);
		    if (label_num < 1) {
			    return;
		    }
            NyARLabelingLabelStack stack = limage.getLabelStack();
            //ラベルをソートしておく
            stack.sortByArea();
            NyARLabelingLabel[] labels = stack.getArray();

            int i;
            for (i = 0; i < label_num; i++)
            {
                // 検査対象内のラベルサイズになるまで無視
                if (labels[i].area <= AR_AREA_MAX)
                {
                    break;
                }
            }

            int xsize = this._width;
            int ysize = this._height;
 		    int[] xcoord = this._xcoord;
		    int[] ycoord = this._ycoord;
            int coord_max = this._max_coord;
            LabelOverlapChecker<NyARLabelingLabel> overlap = this._overlap_checker;
            int label_area;
            NyARLabelingLabel label_pt;

            //重なりチェッカの最大数を設定
            overlap.setMaxLabels(label_num);

		    for (; i < label_num; i++) {
                label_pt = labels[i];
                label_area = label_pt.area;
                // 検査対象サイズよりも小さくなったら終了
                if (label_area < AR_AREA_MIN)
                {
                    break;
                }
                // クリップ領域が画面の枠に接していれば除外
                if (label_pt.clip_l == 1 || label_pt.clip_r == xsize - 2)
                {// if(wclip[i*4+0] == 1 || wclip[i*4+1] ==xsize-2){
                    continue;
                }
                if (label_pt.clip_t == 1 || label_pt.clip_b == ysize - 2)
                {// if( wclip[i*4+2] == 1 || wclip[i*4+3] ==ysize-2){
                    continue;
                }

			    // 既に検出された矩形との重なりを確認
			    if (!overlap.check(label_pt)) {
				    // 重なっているようだ。
				    continue;
			    }
    			
			    // 輪郭を取得
                int coord_num = _cpickup.getContour(limage, limage.getTopClipTangentX(label_pt), label_pt.clip_t, coord_max, xcoord, ycoord);
                if (coord_num == coord_max)
                {
				    // 輪郭が大きすぎる。
				    continue;
			    }
			    //輪郭分析用に正規化する。
                int vertex1 = SquareContourDetector.normalizeCoord(xcoord, ycoord, coord_num);

			    //ここから先が輪郭分析
			    NyARSquare square_ptr = o_square_stack.prePush();
			    if(!this._sqconvertor.coordToSquare(xcoord,ycoord,vertex1,coord_num,label_area,square_ptr)){
				    o_square_stack.pop();// 頂点の取得が出来なかったので破棄
				    continue;				
			    }
			    // 検出済の矩形の属したラベルを重なりチェックに追加する。
			    overlap.push(label_pt);
		    }
            return;	
        }
    }
}