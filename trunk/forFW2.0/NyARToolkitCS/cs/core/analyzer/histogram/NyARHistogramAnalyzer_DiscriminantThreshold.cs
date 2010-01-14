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
    /**
     * 判別法で閾値を求めます。
     * 画素数が2048^2に満たない場合は、fixedint(24-8)で計算できます。
     * @param i_histogram
     * @param o_value
     * @return
     */
    public class NyARHistogramAnalyzer_DiscriminantThreshold : INyARHistogramAnalyzer_Threshold
    {
	    private double _score;


	    public int getThreshold(NyARHistogram i_histogram)
	    {
		    int[] hist=i_histogram.data;
		    int n=i_histogram.length;
		    int da,sa,db,sb,dt,pt,st;
		    int i;		
		    int th=0;
		    //後で使う
		    dt=pt=0;
		    for(i=0;i<n;i++){
			    int h=hist[i];
			    dt+=h*i;
			    pt+=h*i*i;//正規化の時に使う定数
		    }
		    st=i_histogram.total_of_data;
		    //Low側(0<=i<=n-2)
		    da=dt;
		    sa=st;
		    //High側(i=n-1)
		    db=sb=0;		
    		
		    double max=-1;
		    double max_mt=0;
		    //各ヒストグラムの分離度を計算する(1<=i<=n-1の範囲で評価)
		    for(i=n-1;i>0;i--){
			    //次のヒストグラムを計算
			    int hist_count=hist[i];
			    int hist_val=hist_count*i;
			    da-=hist_val;
			    sa-=hist_count;
			    db+=hist_val;
			    sb+=hist_count;
    			
			    //クラス間分散を計算
			    double dv=(sa+sb);
			    double mt=(double)(da+db)/dv;
			    double ma=(sa!=0?((double)da/(double)sa):0)-mt;
			    double mb=(sb!=0?((double)db/(double)sb):0)-mt;
			    double kai=((double)(sa*(ma*ma)+sb*(mb*mb)))/dv;
			    if(max<kai){
				    max_mt=mt;
				    max=kai;
				    th=i;
			    }
			    //System.out.println(kai);
		    }
		    //max_mtを元に正規化
		    this._score=max/((double)(pt+max_mt*max_mt*st-2*max_mt*dt)/st);//129,0.8888888888888887
		    return th;
	    }
	    /**
	     * 最後に実行したgetThresholdのスコアを返します。
	     * スコアは正規化された分離度。1.0-0.0の範囲を取る。0.7以上なら概ね双峰的です。
	     * @return
	     */
	    public double getLastScore()
	    {
		    return this._score;
	    }
    }
}
