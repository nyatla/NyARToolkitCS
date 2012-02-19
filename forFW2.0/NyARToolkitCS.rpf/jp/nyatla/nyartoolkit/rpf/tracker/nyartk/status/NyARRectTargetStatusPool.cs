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

namespace jp.nyatla.nyartoolkit.cs.rpf
{

    public class NyARRectTargetStatusPool : NyARManagedObjectPool<NyARRectTargetStatus>
    {
	    /**
	     * 要素間で共有するオブジェクト。この変数は、NyARRectTargetStatus以外から使わないでください。
	     */
	    public VecLinearCoordinates _vecpos=new VecLinearCoordinates(100);
	    public LineBaseVertexDetector _line_detect=new LineBaseVertexDetector();
	    public VecLinearCoordinatesOperator _vecpos_op=new VecLinearCoordinatesOperator(); 
	    public VecLinearCoordinates.VecLinearCoordinatePoint[] _indexbuf=new VecLinearCoordinates.VecLinearCoordinatePoint[4];
	    public NyARLinear[] _line=NyARLinear.createArray(4);
	    /**
	     * @param i_size
	     * スタックの最大サイズ
	     * @param i_cood_max
	     * 輪郭ベクトルの最大数
	     * @throws NyARException
	     */
	    public NyARRectTargetStatusPool(int i_size)
	    {
		    base.initInstance(i_size);
	    }
	    protected override NyARRectTargetStatus createElement()
	    {
		    return new NyARRectTargetStatus(this);
	    }

	    private int[] __sq_table=new int[4];
	    /**
	     * 頂点セット同士の差分を計算して、極端に大きな誤差を持つ点が無いかを返します。
	     * チェックルールは、頂点セット同士の差のうち一つが、全体の一定割合以上の誤差を持つかです。
	     * @param i_point1
	     * @param i_point2
	     * @return
	     * @todo 展開して最適化
	     */
        public bool checkLargeDiff(NyARDoublePoint2d[] i_point1, NyARDoublePoint2d[] i_point2)
	    {
            Debug.Assert(i_point1.Length == i_point2.Length);
		    int[] sq_tbl=this.__sq_table;
		    int all=0;
		    for(int i=3;i>=0;i--){
			    sq_tbl[i]=(int)i_point1[i].sqDist(i_point2[i]);
			    all+=sq_tbl[i];
		    }
		    //移動距離の2乗の平均値
		    if(all<4){
			    return true;
		    }
		    for(int i=3;i>=0;i--){
			    //1個が全体の75%以上を持っていくのはおかしい。
			    if(sq_tbl[i]*100/all>70){
				    return false;
			    }
		    }
		    return true;
	    }
    }
}