/* 
 * PROJECT: NyARToolkit
 * --------------------------------------------------------------------------------
 * This work is based on the original ARToolKit developed by
 *   Hirokazu Kato
 *   Mark Billinghurst
 *   HITLab, University of Washington, Seattle
 * http://www.hitl.washington.edu/artoolkit/
 *
 * The NyARToolkit is Java version ARToolkit class library.
 * Copyright (C)2008 R.Iizuka
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this framework; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 * 
 * For further information please contact.
 *	http://nyatla.jp/nyatoolkit/
 *	<airmail(at)ebony.plala.or.jp>
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
    public class NyARSquareDetector_X2 : INyARSquareDetector
    {
        private const int PCA_LENGTH = 20;
        private const int AR_AREA_MAX = 100000;// #define AR_AREA_MAX 100000

        private const int AR_AREA_MIN = 70;// #define AR_AREA_MIN 70
        private int _width;
        private int _height;

        private INyARLabeling _labeling;

        private NyARLabelingImage _limage;

        private OverlapChecker _overlap_checker = new OverlapChecker();
        private NyARFixedFloatObserv2IdealMap _dist_factor;
        private NyARFixedFloatPca2d _pca;

        /**
         * 最大i_squre_max個のマーカーを検出するクラスを作成する。
         * 
         * @param i_param
         */
        public NyARSquareDetector_X2(NyARFixedFloatObserv2IdealMap i_dist_factor_ref, NyARIntSize i_size)
        {
            this._width = i_size.w;
            this._height = i_size.h;
            this._dist_factor = i_dist_factor_ref;
            this._labeling = new NyARLabeling_ARToolKit_X2();
            this._limage = new NyARLabelingImage(this._width, this._height);
            this._labeling.attachDestination(this._limage);

            // 輪郭の最大長は画面に映りうる最大の長方形サイズ。
            int number_of_coord = (this._width + this._height) * 2;

            // 輪郭バッファは頂点変換をするので、輪郭バッファの２倍取る。
            this._max_coord = number_of_coord;
            this._xcoord = new int[number_of_coord * 2];
            this._ycoord = new int[number_of_coord * 2];
            //PCA
            this._pca = new NyARFixedFloatPca2d();
            this._xpos = new int[PCA_LENGTH];//最大辺長はthis._width+this._height
            this._ypos = new int[PCA_LENGTH];//最大辺長はthis._width+this._height

        }

        private int _max_coord;
        private int[] _xcoord;
        private int[] _ycoord;

        private void normalizeCoord(int[] i_coord_x, int[] i_coord_y, int i_index, int i_coord_num)
        {
            // vertex1を境界にして、後方に配列を連結
            Array.Copy(i_coord_x, 1, i_coord_x, i_coord_num, i_index);
            Array.Copy(i_coord_y, 1, i_coord_y, i_coord_num, i_index);
        }

        private int[] __detectMarker_mkvertex = new int[5];

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
            INyARLabeling labeling_proc = this._labeling;
            NyARLabelingImage limage = this._limage;

            // 初期化

            // マーカーホルダをリセット
            o_square_stack.clear();

            // ラベリング
            labeling_proc.labeling(i_raster);

            // ラベル数が0ならここまで
            int label_num = limage.getLabelStack().getLength();
            if (label_num < 1)
            {
                return;
            }

            NyARLabelingLabelStack stack = limage.getLabelStack();
            NyARLabelingLabel[] labels = (NyARLabelingLabel[])stack.getArray();


            // ラベルを大きい順に整列
            stack.sortByArea();

            // デカいラベルを読み飛ばし
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
            int[] mkvertex = this.__detectMarker_mkvertex;
            OverlapChecker overlap = this._overlap_checker;
            int coord_num;
            int label_area;
            NyARLabelingLabel label_pt;

            //重なりチェッカの最大数を設定
            overlap.reset(label_num);

            for (; i < label_num; i++)
            {
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
                if (!overlap.check(label_pt))
                {
                    // 重なっているようだ。
                    continue;
                }

                // 輪郭を取得
                coord_num = limage.getContour(i, coord_max, xcoord, ycoord);
                if (coord_num == coord_max)
                {
                    // 輪郭が大きすぎる。
                    continue;
                }
                //頂点候補のインデクスを取得
                int vertex1 = scanVertex(xcoord, ycoord, coord_num);

                // 頂点候補(vertex1)を先頭に並べなおした配列を作成する。
                normalizeCoord(xcoord, ycoord, vertex1, coord_num);

                // 領域を準備する。
                NyARSquare square_ptr = (NyARSquare)o_square_stack.prePush();
                // 頂点情報を取得
                if (!getSquareVertex(xcoord, ycoord, vertex1, coord_num, label_area, mkvertex))
                {
                    o_square_stack.pop();// 頂点の取得が出来なかったので破棄
                    continue;
                }
                // マーカーを検出
                if (!getSquareLine(mkvertex, xcoord, ycoord, square_ptr))
                {
                    // 矩形が成立しなかった。
                    o_square_stack.pop();
                    continue;
                }
                // 検出済の矩形の属したラベルを重なりチェックに追加する。
                overlap.push(label_pt);
            }
            return;
        }

        /**
         * 辺からの対角線が最長になる点を対角線候補として返す。
         * 
         * @param i_xcoord
         * @param i_ycoord
         * @param i_coord_num
         * @return
         */
        private int scanVertex(int[] i_xcoord, int[] i_ycoord, int i_coord_num)
        {
            int sx = i_xcoord[0];
            int sy = i_ycoord[0];
            int d = 0;
            int w, x, y;
            int ret = 0;
            for (int i = 1; i < i_coord_num; i++)
            {
                x = i_xcoord[i] - sx;
                y = i_ycoord[i] - sy;
                w = x * x + y * y;
                if (w > d)
                {
                    d = w;
                    ret = i;
                }
                // ここでうまく終了条件入れられないかな。
            }
            return ret;
        }

        private NyARFixedFloatVertexCounter __getSquareVertex_wv1 = new NyARFixedFloatVertexCounter();

        private NyARFixedFloatVertexCounter __getSquareVertex_wv2 = new NyARFixedFloatVertexCounter();

        /**
         * static int arDetectMarker2_check_square( int area, ARMarkerInfo2 *marker_info2, double factor ) 関数の代替関数 OPTIMIZED STEP [450->415] o_squareに頂点情報をセットします。
         * 
         * @param i_x_coord
         * @param i_y_coord
         * @param i_vertex1_index
         * @param i_coord_num
         * @param i_area
         * @param o_vertex
         * 要素数はint[4]である事
         * @return
         */
        private bool getSquareVertex(int[] i_x_coord, int[] i_y_coord, int i_vertex1_index, int i_coord_num, int i_area, int[] o_vertex)
        {
            NyARFixedFloatVertexCounter wv1 = this.__getSquareVertex_wv1;
            NyARFixedFloatVertexCounter wv2 = this.__getSquareVertex_wv2;
            int end_of_coord = i_vertex1_index + i_coord_num - 1;
            int sx = i_x_coord[i_vertex1_index];// sx = marker_info2->x_coord[0];
            int sy = i_y_coord[i_vertex1_index];// sy = marker_info2->y_coord[0];
            int dmax = 0;
            int v1 = i_vertex1_index;
            for (int i = 1 + i_vertex1_index; i < end_of_coord; i++)
            {// for(i=1;i<marker_info2->coord_num-1;i++)
                // {
                int d = (i_x_coord[i] - sx) * (i_x_coord[i] - sx) + (i_y_coord[i] - sy) * (i_y_coord[i] - sy);
                if (d > dmax)
                {
                    dmax = d;
                    v1 = i;
                }
            }
            //final double thresh = (i_area / 0.75) * 0.01;
            long thresh_f16 = (i_area << 16) / 75;

            o_vertex[0] = i_vertex1_index;

            if (!wv1.getVertex(i_x_coord, i_y_coord, i_vertex1_index, v1, thresh_f16))
            { // if(get_vertex(marker_info2->x_coord,marker_info2->y_coord,0,v1,thresh,wv1,&wvnum1)<
                // 0 ) {
                return false;
            }
            if (!wv2.getVertex(i_x_coord, i_y_coord, v1, end_of_coord, thresh_f16))
            {// if(get_vertex(marker_info2->x_coord,marker_info2->y_coord,v1,marker_info2->coord_num-1,thresh,wv2,&wvnum2)
                // < 0) {
                return false;
            }

            int v2;
            if (wv1.number_of_vertex == 1 && wv2.number_of_vertex == 1)
            {// if(wvnum1 == 1 && wvnum2== 1) {
                o_vertex[1] = wv1.vertex[0];
                o_vertex[2] = v1;
                o_vertex[3] = wv2.vertex[0];
            }
            else if (wv1.number_of_vertex > 1 && wv2.number_of_vertex == 0)
            {// }else if( wvnum1 > 1 && wvnum2== 0) {
                //頂点位置を、起点から対角点の間の1/2にあると予想して、検索する。
                v2 = (v1 - i_vertex1_index) / 2 + i_vertex1_index;
                if (!wv1.getVertex(i_x_coord, i_y_coord, i_vertex1_index, v2, thresh_f16))
                {
                    return false;
                }
                if (!wv2.getVertex(i_x_coord, i_y_coord, v2, v1, thresh_f16))
                {
                    return false;
                }
                if (wv1.number_of_vertex == 1 && wv2.number_of_vertex == 1)
                {
                    o_vertex[1] = wv1.vertex[0];
                    o_vertex[2] = wv2.vertex[0];
                    o_vertex[3] = v1;
                }
                else
                {
                    return false;
                }
            }
            else if (wv1.number_of_vertex == 0 && wv2.number_of_vertex > 1)
            {
                //v2 = (v1-i_vertex1_index+ end_of_coord-i_vertex1_index) / 2+i_vertex1_index;
                v2 = (v1 + end_of_coord) / 2;

                if (!wv1.getVertex(i_x_coord, i_y_coord, v1, v2, thresh_f16))
                {
                    return false;
                }
                if (!wv2.getVertex(i_x_coord, i_y_coord, v2, end_of_coord, thresh_f16))
                {
                    return false;
                }
                if (wv1.number_of_vertex == 1 && wv2.number_of_vertex == 1)
                {
                    o_vertex[1] = v1;
                    o_vertex[2] = wv1.vertex[0];
                    o_vertex[3] = wv2.vertex[0];
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            o_vertex[4] = end_of_coord;
            return true;
        }
        private int[] _xpos;
        private int[] _ypos;

        private NyARI64Matrix22 __getSquareLine_evec = new NyARI64Matrix22();
        private NyARI64Point2d __getSquareLine_mean = new NyARI64Point2d();
        private NyARI64Point2d __getSquareLine_ev = new NyARI64Point2d();
        private NyARI64Linear[] __getSquareLine_i64liner = NyARI64Linear.createArray(4);
        /**
         * arGetLine(int x_coord[], int y_coord[], int coord_num,int vertex[], double line[4][3], double v[4][2]) arGetLine2(int x_coord[], int y_coord[], int
         * coord_num,int vertex[], double line[4][3], double v[4][2], double *dist_factor) の２関数の合成品です。 マーカーのvertex,lineを計算して、結果をo_squareに保管します。
         * Optimize:STEP[424->391]
         * 
         * @param i_cparam
         * @return
         * @throws NyARException
         */
        private bool getSquareLine(int[] i_mkvertex, int[] i_xcoord, int[] i_ycoord, NyARSquare o_square)
        {
            NyARLinear[] l_line = o_square.line;
            NyARI64Matrix22 evec = this.__getSquareLine_evec;
            NyARI64Point2d mean = this.__getSquareLine_mean;
            NyARI64Point2d ev = this.__getSquareLine_ev;
            NyARI64Linear[] i64liner = this.__getSquareLine_i64liner;

            for (int i = 0; i < 4; i++)
            {
                //			final double w1 = (double) (i_mkvertex[i + 1] - i_mkvertex[i] + 1) * 0.05 + 0.5;
                int w1 = ((((i_mkvertex[i + 1] - i_mkvertex[i] + 1) << 8) * 13) >> 8) + (1 << 7);
                int st = i_mkvertex[i] + (w1 >> 8);
                int ed = i_mkvertex[i + 1] - (w1 >> 8);
                int n = ed - st + 1;
                if (n < 2)
                {
                    // nが2以下でmatrix.PCAを計算することはできないので、エラー
                    return false;
                }
                //配列作成
                n=this._dist_factor.observ2IdealSampling(i_xcoord, i_ycoord, st, n, this._xpos, this._ypos,PCA_LENGTH);

                //主成分分析する。
                this._pca.pcaF16(this._xpos, this._ypos, n, evec, ev, mean);
                NyARI64Linear l_line_i = i64liner[i];
                l_line_i.run = evec.m01;// line[i][0] = evec->m[1];
                l_line_i.rise = -evec.m00;// line[i][1] = -evec->m[0];
                l_line_i.intercept = -((l_line_i.run * mean.x + l_line_i.rise * mean.y) >> 16);// line[i][2] = -(line[i][0]*mean->v[0] + line[i][1]*mean->v[1]);
            }

            NyARDoublePoint2d[] l_sqvertex = o_square.sqvertex;
            NyARIntPoint2d[] l_imvertex = o_square.imvertex;
            for (int i = 0; i < 4; i++)
            {
                NyARI64Linear l_line_i = i64liner[i];
                NyARI64Linear l_line_2 = i64liner[(i + 3) % 4];
                long w1 = (l_line_2.run * l_line_i.rise - l_line_i.run * l_line_2.rise) >> 16;
                if (w1 == 0)
                {
                    return false;
                }
                l_sqvertex[i].x = (double)((l_line_2.rise * l_line_i.intercept - l_line_i.rise * l_line_2.intercept) / w1) / 65536.0;
                l_sqvertex[i].y = (double)((l_line_i.run * l_line_2.intercept - l_line_2.run * l_line_i.intercept) / w1) / 65536.0;
                // 頂点インデクスから頂点座標を得て保存
                l_imvertex[i].x = i_xcoord[i_mkvertex[i]];
                l_imvertex[i].y = i_ycoord[i_mkvertex[i]];
                l_line[i].run = (double)l_line_i.run / 65536.0;
                l_line[i].rise = (double)l_line_i.rise / 65536.0;
                l_line[i].intercept = (double)l_line_i.intercept / 65536.0;
            }
            return true;
        }
    }



    /**
     * ラベル同士の重なり（内包関係）を調べるクラスです。 
     * ラベルリストに内包するラベルを蓄積し、それにターゲットのラベルが内包されているか を確認します。
     */
    class OverlapChecker
    {
        private NyARLabelingLabel[] _labels = new NyARLabelingLabel[32];

        private int _length;

        /**
         * 最大i_max_label個のラベルを蓄積できるようにオブジェクトをリセットする
         * 
         * @param i_max_label
         */
        public void reset(int i_max_label)
        {
            if (i_max_label > this._labels.Length)
            {
                this._labels = new NyARLabelingLabel[i_max_label];
            }
            this._length = 0;
        }

        /**
         * チェック対象のラベルを追加する。
         * 
         * @param i_label_ref
         */
        public void push(NyARLabelingLabel i_label_ref)
        {
            this._labels[this._length] = i_label_ref;
            this._length++;
        }

        /**
         * 現在リストにあるラベルと重なっているかを返す。
         * 
         * @param i_label
         * @return 何れかのラベルの内側にあるならばfalse,独立したラベルである可能性が高ければtrueです．
         */
        public bool check(NyARLabelingLabel i_label)
        {
            // 重なり処理かな？
            NyARLabelingLabel[] label_pt = this._labels;
            int px1 = (int)i_label.pos_x;
            int py1 = (int)i_label.pos_y;
            for (int i = this._length - 1; i >= 0; i--)
            {
                int px2 = (int)label_pt[i].pos_x;
                int py2 = (int)label_pt[i].pos_y;
                int d = (px1 - px2) * (px1 - px2) + (py1 - py2) * (py1 - py2);
                if (d < label_pt[i].area / 4)
                {
                    // 対象外
                    return false;
                }
            }
            // 対象
            return true;
        }
    }
    class NyARFixedFloatVertexCounter
    {
        public int[] vertex = new int[6];// 5まで削れる

        public int number_of_vertex;

        private long thresh_16f;

        private int[] x_coord;

        private int[] y_coord;

        public bool getVertex(int[] i_x_coord, int[] i_y_coord, int st, int ed, long i_thresh)
        {
            this.number_of_vertex = 0;
            this.thresh_16f = i_thresh;
            this.x_coord = i_x_coord;
            this.y_coord = i_y_coord;
            return get_vertex(st, ed);
        }

        /**
         * static int get_vertex( int x_coord[], int y_coord[], int st, int ed,double thresh, int vertex[], int *vnum) 関数の代替関数
         * 
         * @param x_coord
         * @param y_coord
         * @param st
         * @param ed
         * @param thresh
         * @return
         */
        private bool get_vertex(int st, int ed)
        {
            int v1 = 0;
            int[] lx_coord = this.x_coord;
            int[] ly_coord = this.y_coord;
            int a = ly_coord[ed] - ly_coord[st];
            int b = lx_coord[st] - lx_coord[ed];
            int c = lx_coord[ed] * ly_coord[st] - ly_coord[ed] * lx_coord[st];
            long dmax = 0;
            for (int i = st + 1; i < ed; i++)
            {
                long d = a * lx_coord[i] + b * ly_coord[i] + c;
                if (d * d > dmax)
                {
                    dmax = d * d;
                    v1 = i;
                }
            }
            if ((dmax << 16) / (long)(a * a + b * b) > this.thresh_16f)
            {
                if (!get_vertex(st, v1))
                {
                    return false;
                }
                if (number_of_vertex > 5)
                {
                    return false;
                }
                vertex[number_of_vertex] = v1;// vertex[(*vnum)] = v1;
                number_of_vertex++;// (*vnum)++;

                if (!get_vertex(v1, ed))
                {
                    return false;
                }
            }
            return true;
        }
    }

}