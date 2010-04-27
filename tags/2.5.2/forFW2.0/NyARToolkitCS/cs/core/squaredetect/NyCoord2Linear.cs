using System;
using System.Collections.Generic;
using System.Text;

namespace jp.nyatla.nyartoolkit.cs.core
{
    /**
     * 頂点集合を一次方程式のパラメータに変換します。
     * 
     *
     */
    public class Coord2Linear
    {
	    private double[] _xpos;
	    private double[] _ypos;	
	    private INyARPca2d _pca;
	    private NyARDoubleMatrix22 __getSquareLine_evec=new NyARDoubleMatrix22();
	    private double[] __getSquareLine_mean=new double[2];
	    private double[] __getSquareLine_ev=new double[2];
	    private NyARObserv2IdealMap _dist_factor;
	    public Coord2Linear(NyARIntSize i_size,NyARCameraDistortionFactor i_distfactor_ref)
	    {
		    //歪み計算テーブルを作ると、8*width/height*2の領域を消費します。
		    //領域を取りたくない場合は、i_dist_factor_refの値をそのまま使ってください。
		    this._dist_factor = new NyARObserv2IdealMap(i_distfactor_ref,i_size);


		    // 輪郭バッファ
		    this._pca=new NyARPca2d_MatrixPCA_O2();
		    this._xpos=new double[i_size.w+i_size.h];//最大辺長はthis._width+this._height
		    this._ypos=new double[i_size.w+i_size.h];//最大辺長はthis._width+this._height
		    return;
	    }


	    /**
	     * 輪郭点集合からay+bx+c=0の直線式を計算します。
	     * @param i_st
	     * @param i_ed
	     * @param i_xcoord
	     * @param i_ycoord
	     * @param i_cood_num
	     * @param o_line
	     * @return
	     * @throws NyARException
	     */
	    public bool coord2Line(int i_st,int i_ed,int[] i_xcoord, int[] i_ycoord,int i_cood_num, NyARLinear o_line)
	    {
		    //頂点を取得
		    int n,st,ed;
		    double w1;
    	
		    //探索区間の決定
		    if(i_ed>=i_st){
			    //頂点[i]から頂点[i+1]までの輪郭が、1区間にあるとき
			    w1 = (double) (i_ed - i_st + 1) * 0.05 + 0.5;
			    //探索区間の決定
			    st = (int) (i_st+w1);
			    ed = (int) (i_ed - w1);
		    }else{
			    //頂点[i]から頂点[i+1]までの輪郭が、2区間に分かれているとき
			    w1 = (double) ((i_ed+i_cood_num-i_st+1)%i_cood_num) * 0.05 + 0.5;
			    //探索区間の決定
			    st = ((int) (i_st+w1))%i_cood_num;
			    ed = ((int) (i_ed+i_cood_num-w1))%i_cood_num;
		    }
		    //探索区間数を確認
		    if(st<=ed){
			    //探索区間は1区間
			    n = ed - st + 1;
			    this._dist_factor.observ2IdealBatch(i_xcoord, i_ycoord, st, n,this._xpos,this._ypos,0);
		    }else{
			    //探索区間は2区間
			    n=ed+1+i_cood_num-st;
			    this._dist_factor.observ2IdealBatch(i_xcoord, i_ycoord, st,i_cood_num-st,this._xpos,this._ypos,0);
			    this._dist_factor.observ2IdealBatch(i_xcoord, i_ycoord, 0,ed+1,this._xpos,this._ypos,i_cood_num-st);
		    }
		    //要素数の確認
		    if (n < 2) {
			    // nが2以下でmatrix.PCAを計算することはできないので、エラー
			    return false;
		    }
		    //主成分分析する。
		    NyARDoubleMatrix22 evec=this.__getSquareLine_evec;
		    double[] mean=this.__getSquareLine_mean;

    		
		    this._pca.pca(this._xpos,this._ypos,n,evec, this.__getSquareLine_ev,mean);
		    o_line.dy = evec.m01;// line[i][0] = evec->m[1];
		    o_line.dx = -evec.m00;// line[i][1] = -evec->m[0];
		    o_line.c = -(o_line.dy * mean[0] + o_line.dx * mean[1]);// line[i][2] = -(line[i][0]*mean->v[0] + line[i][1]*mean->v[1]);

		    return true;
	    }
    }
}
