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

namespace jp.nyatla.nyartoolkit.cs.core
{
    public class NyARSquareContourDetector_Rle : NyARSquareContourDetector
    {
        private const int AR_AREA_MAX = 100000;// #define AR_AREA_MAX 100000
        private const int AR_AREA_MIN = 70;// #define AR_AREA_MIN 70
        private int _width;
        private int _height;

        private NyARLabeling_Rle _labeling;

        private LabelOverlapChecker<RleLabelFragmentInfoStack.RleLabelFragmentInfo> _overlap_checker = new LabelOverlapChecker<RleLabelFragmentInfoStack.RleLabelFragmentInfo>(32);
        private ContourPickup _cpickup = new ContourPickup();
        private RleLabelFragmentInfoStack _stack;
        private Coord2SquareVertexIndexes _coord2vertex = new Coord2SquareVertexIndexes();

        private int _max_coord;
        private int[] _xcoord;
        private int[] _ycoord;
        /**
         * 最大i_squre_max個のマーカーを検出するクラスを作成する。
         * 
         * @param i_param
         */
        public NyARSquareContourDetector_Rle(NyARIntSize i_size)
        {
            this._width = i_size.w;
            this._height = i_size.h;
            //ラベリングのサイズを指定したいときはsetAreaRangeを使ってね。
            this._labeling = new NyARLabeling_Rle(this._width, this._height);
            this._labeling.setAreaRange(AR_AREA_MAX, AR_AREA_MIN);
            this._stack = new RleLabelFragmentInfoStack(i_size.w * i_size.h * 2048 / (320 * 240) + 32);//検出可能な最大ラベル数


            // 輪郭の最大長は画面に映りうる最大の長方形サイズ。
            int number_of_coord = (this._width + this._height) * 2;

            // 輪郭バッファ
            this._max_coord = number_of_coord;
            this._xcoord = new int[number_of_coord];
            this._ycoord = new int[number_of_coord];
            return;
        }

        private int[] __detectMarker_mkvertex = new int[4];

        public override void detectMarkerCB(NyARBinRaster i_raster, IDetectMarkerCallback i_callback)
        {
            RleLabelFragmentInfoStack flagment = this._stack;
            LabelOverlapChecker<RleLabelFragmentInfoStack.RleLabelFragmentInfo> overlap = this._overlap_checker;

            // ラベル数が0ならここまで
            int label_num = this._labeling.labeling(i_raster, 0, i_raster.getHeight(), flagment);
            if (label_num < 1)
            {
                return;
            }
            //ラベルをソートしておく
            flagment.sortByArea();
            //ラベルリストを取得
            RleLabelFragmentInfoStack.RleLabelFragmentInfo[] labels = flagment.getArray();

            int xsize = this._width;
            int ysize = this._height;
            int[] xcoord = this._xcoord;
            int[] ycoord = this._ycoord;
            int coord_max = this._max_coord;
            int[] mkvertex = this.__detectMarker_mkvertex;


            //重なりチェッカの最大数を設定
            overlap.setMaxLabels(label_num);

            for (int i = 0; i < label_num; i++)
            {
                RleLabelFragmentInfoStack.RleLabelFragmentInfo label_pt = labels[i];
                int label_area = label_pt.area;

                // クリップ領域が画面の枠に接していれば除外
                if (label_pt.clip_l == 0 || label_pt.clip_r == xsize - 1)
                {
                    continue;
                }
                if (label_pt.clip_t == 0 || label_pt.clip_b == ysize - 1)
                {
                    continue;
                }
                // 既に検出された矩形との重なりを確認
                if (!overlap.check(label_pt))
                {
                    // 重なっているようだ。
                    continue;
                }

                //輪郭を取得
                int coord_num = _cpickup.getContour(i_raster, label_pt.entry_x, label_pt.clip_t, coord_max, xcoord, ycoord);
                if (coord_num == coord_max)
                {
                    // 輪郭が大きすぎる。
                    continue;
                }
                //輪郭線をチェックして、矩形かどうかを判定。矩形ならばmkvertexに取得
                if (!this._coord2vertex.getVertexIndexes(xcoord, ycoord, coord_num, label_area, mkvertex))
                {
                    // 頂点の取得が出来なかった
                    continue;
                }
                //矩形を発見したことをコールバック関数で通知
                i_callback.onSquareDetect(this, xcoord, ycoord, coord_num, mkvertex);

                // 検出済の矩形の属したラベルを重なりチェックに追加する。
                overlap.push(label_pt);

            }
            return;
        }
        /**
         * デバック用API
         * @return
         */
        public RleLabelFragmentInfoStack _getFragmentStack()
        {
            return this._stack;
        }

    }
}
