/* 
 * PROJECT: NyARToolkitCS
 * --------------------------------------------------------------------------------
 * This work is based on the original ARToolKit developed by
 *   Hirokazu Kato
 *   Mark Billinghurst
 *   HITLab, University of Washington, Seattle
 * http://www.hitl.washington.edu/artoolkit/
 *
 * The NyARToolkitCS is C# version NyARToolkit class library.
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
using System.Collections.Generic;

namespace jp.nyatla.nyartoolkit.cs.core
{
    /**
     * このクラスは、detectMarkerがマーカーオブジェクトの蓄積に使うクラスです。
     * 実体を伴うマーカーホルダと、これを参照するマーカーアレイを持ちます。
     * 
     * マーカーアレイはマーカーホルダに存在するマーカーリストを特定の条件でフィルタした
     * 結果を格納します。
     * 
     * 一度作られたマーカーホルダは繰り返し使用されます。
     * 
     *
     */
    public class NyARMarkerList
    {
        private int marker_holder_num;//marker_holderの使用中の数
        protected int marker_array_num;//marker_arrayの有効な数
        protected NyARMarker[] marker_holder;//マーカーデータの保持配列
        protected NyARMarker[] marker_array; //マーカーデータのインデックス配列
        /**
         * 派生データ型をラップするときに使う
         * @param i_holder
         * 値の保持配列。全要素に実体を割り当てる必要がある。
         */
        protected  NyARMarkerList(NyARMarker[] i_holder)
        {
	        this.marker_holder=i_holder;
	        this.marker_array =new NyARMarker[i_holder.Length];
	        this.marker_array_num   =0;
	        this.marker_holder_num =0;
        }
        public NyARMarkerList(int i_number_of_holder)
        {
	    this.marker_holder=new NyARMarker[i_number_of_holder];
	    //先にマーカーホルダにオブジェクトを作っておく
	    for(int i=0;i<i_number_of_holder;i++){
	        this.marker_holder[i]=new NyARMarker();
	    }
	    this.marker_array =new NyARMarker[this.marker_holder.Length];
	    this.marker_array_num   =0;
	    this.marker_holder_num =0;
        }
        /**
         * 現在位置のマーカーホルダを返す。
         * 現在位置が終端の場合関数は失敗する。
         * @return
         */
        public NyARMarker getCurrentHolder()
        {
	        if(this.marker_holder_num>=this.marker_holder.Length){
	            throw new NyARException();
    	    }	
	        return this.marker_holder[this.marker_holder_num];
        }
        /**
         * マーカーホルダの現在位置を1つ進めて、そのホルダを返す。
         * この関数を実行すると、使用中のマーカーホルダの数が1個増える。
         * @return
         * 空いているマーカーホルダが無ければnullを返します。
         *
         */
        public NyARMarker getNextHolder()
        {
	        //現在位置が終端位置ならnullを返す。
	        if(this.marker_holder_num+1>=this.marker_holder.Length){
                    this.marker_holder_num=this.marker_holder.Length;
	            return null;
	        }
	        this.marker_holder_num++;
	        return this.marker_holder[this.marker_holder_num];
        }
        /**
         * マーカーアレイのi_indexの要素を返す。
         * @param i_index
         * @return
         * @throws NyARException
         */
        public NyARMarker getMarker(int i_index)
        {
	        if(i_index>=marker_array_num){
	            throw new NyARException();
	        }
	        return this.marker_array[i_index];
        }
        /**
         * マーカーアレイの要素数を返す。
         * @return
         */
        public int getMarkerNum()
        {
	        return marker_array_num;
        }
        /**
         * マーカーアレイの要素数と、マーカーホルダの現在位置をリセットする。
         * @return
         */
        public void reset()
        {
	        this.marker_array_num=0;
	        this.marker_holder_num=0;
        }
        /**
         * マーカーホルダに格納済みのマーカーから重なっているのものを除外して、
         * マーカーアレイにフィルタ結果を格納します。
         * [[この関数はマーカー検出処理と密接に関係する関数です。
         * NyARDetectMarkerクラス以外から呼び出さないで下さい。]]
         * メモ:この関数はmarker_holderの内容を変化させまするので注意。
         */
        public void updateMarkerArray()
        {
	        //重なり処理かな？
	        int i;
	        double d;
	        double[] pos_j,pos_i;
        //	NyARMarker[] marker_holder;
	        for(i=0; i < this.marker_holder_num; i++ ){
	            pos_i=marker_holder[i].pos;
	            for(int j=i+1; j < this.marker_holder_num; j++ ) {
		            pos_j=marker_holder[j].pos;
		            d = (pos_i[0] - pos_j[0])*(pos_i[0] - pos_j[0])+
		            (pos_i[1] - pos_j[1])*(pos_i[1] - pos_j[1]);
		            if(marker_holder[i].area >marker_holder[j].area ) {
		                if( d <marker_holder[i].area / 4 ) {
			            marker_holder[j].area = 0;
		                }
		            }else{
		                if( d < marker_holder[j].area / 4 ) {
			                marker_holder[i].area = 0;
		                }
		            }
	            }
	        }
	        //みつかったマーカーを整理する。
	        int l_array_num=0;
	        //エリアが0のマーカーを外した配列を作って、その数もついでに計算
	        for(i=0;i<marker_holder_num;i++){
	            if(marker_holder[i].area==0.0){
		        continue;
	            }
	            marker_array[l_array_num]=marker_holder[i];
	            l_array_num++;
	        }
	        //マーカー個数を更新
	        this.marker_array_num=l_array_num;
        }    
    }
}
