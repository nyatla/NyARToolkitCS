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
    public class NyARSquareList : NyARMarkerList
    {
        private NyARSquare[] square_array;
        private int square_array_num;
        public NyARSquareList(int i_number_of_holder): base(new NyARSquare[i_number_of_holder])
        {
	        //マーカーホルダに実体を割り当てる。
	        for(int i=0;i<this.marker_holder.Length;i++){
	            this.marker_holder[i]=new NyARSquare();
	        }
	        this.square_array=new NyARSquare[i_number_of_holder];
	        this.square_array_num=0;
        }
        /**
         * マーカーアレイをフィルタして、square_arrayを更新する。
         * [[この関数はマーカー検出処理と密接に関係する関数です。
         * NyARDetectSquareクラス以外から呼び出さないで下さい。]]
         */
        public void updateSquareArray(NyARParam i_param)
        {
	        NyARSquare square;
	        int j=0;
	        for (int i = 0; i <this.marker_array_num; i++){
        //	    double[][]  line	=new double[4][3];
        //	    double[][]  vertex	=new double[4][2];
	            //NyARMarker marker=detect.getMarker(i);
	            square=(NyARSquare)this.marker_array[i];
	            //・・・線の検出？？
                    if (!square.getLine(i_param))
                    {
            	        continue;
                    }
                    this.square_array[j]=square;
        //ここで計算するのは良くないと思うんだ	
        //		marker_infoL[j].id  = id.get();
        //		marker_infoL[j].dir = dir.get();
        //		marker_infoL[j].cf  = cf.get();	
                    j++;
	        }
	        this.square_array_num=j;
        }
        /**
         * スクエア配列に格納されている要素数を返します。
         * @return
         */
        public int getSquareNum()
        {
	        return 	this.square_array_num;
        }
        /**
         * スクエア配列の要素を返します。
         * スクエア配列はマーカーアレイをさらにフィルタした結果です。
         * マーカーアレイの部分集合になっている点に注意してください。
         * @param i_index
         * @return
         * @throws NyARException
         */
        public NyARSquare getSquare(int i_index)
        {
	        if(i_index>=this.square_array_num){
	            throw new NyARException();
	        }
	        return this.square_array[i_index];
        }
    }

}
