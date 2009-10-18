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
/**
 * 並進ベクトル[T]を３次元座標[b]と基点の回転済行列[M]から計算します。
 * ARToolKit互換の数値を計算します。
 *
 */
public class NyARTransportVectorSolver_ARToolKit : INyARTransportVectorSolver
{
	private NyARMat _mat_at = new NyARMat(3,8);//3,NUMBER_OF_VERTEX*2
	private NyARMat _mat_a =  new NyARMat(8,3);//NUMBER_OF_VERTEX,3
	private NyARMat _mat_t =  new NyARMat(3,3);//NUMBER_OF_VERTEX,3
	private NyARMat _mat_c =  new NyARMat(8,1);//NUMBER_OF_VERTEX * 2, 1
	private NyARMat _mat_e =  new NyARMat(3,1);
	private NyARMat _mat_f =  new NyARMat(3,1);
	private double[] _cx=new double[4];
	private double[] _cy=new double[4];
	
	private NyARPerspectiveProjectionMatrix _projection_mat;
	public NyARTransportVectorSolver_ARToolKit(NyARPerspectiveProjectionMatrix i_projection_mat_ref)
	{
		this._projection_mat=i_projection_mat_ref;
		//aとb(aの転置行列)の固定部分を設定。
		double[][] mata = this._mat_a.getArray();
		double[][] matat = this._mat_at.getArray();

		//変換用行列のcpara部分を先に作成
		for (int i = 0; i < 4; i++) {
			int x2 = i * 2;
			mata[x2][0] = matat[0][x2] = i_projection_mat_ref.m00;// mat_a->m[j*6+0]=mat_b->m[num*0+j*2] =cpara[0][0];
			mata[x2][1] = matat[1][x2] = i_projection_mat_ref.m01;// mat_a->m[j*6+1]=mat_b->m[num*2+j*2]=cpara[0][1];
			mata[x2 + 1][0] = matat[0][x2 + 1] = 0.0;// mat_a->m[j*6+3] =mat_b->m[num*0+j*2+1]= 0.0;
			mata[x2 + 1][1] = matat[1][x2 + 1] = i_projection_mat_ref.m11;// mat_a->m[j*6+4] =mat_b->m[num*2+j*2+1]= cpara[1][1];
		}
		return;
	}
	public void set2dVertex(NyARDoublePoint2d[] i_ref_vertex_2d,int i_number_of_vertex)
	{		
		Debug.Assert(i_number_of_vertex==4);
		double[] cx=this._cx;
		double[] cy=this._cy;
		double cpara02=this._projection_mat.m02;
		double cpara12=this._projection_mat.m12;		
		NyARMat mat_t=this._mat_t;
		double[][] mata = this._mat_a.getArray();
		double[][] matat= this._mat_at.getArray();
		for (int i = 0; i < 4; i++){
			cx[i]=i_ref_vertex_2d[i].x;
			cy[i]=i_ref_vertex_2d[i].y;
			int x2 = i * 2;	
			mata[x2][2] = matat[2][x2] = cpara02 - i_ref_vertex_2d[i].x;// mat_a->m[j*6+2]=mat_b->m[num*4+j*2]=cpara[0][2]-pos2d[j][0];
			mata[x2 + 1][2] = matat[2][x2 + 1] = cpara12 - i_ref_vertex_2d[i].y;// mat_a->m[j*6+5]=mat_b->m[num*4+j*2+1]=cpara[1][2]-pos2d[j][1];
		}
		//T(3x3行列)の作成
		mat_t.matrixMul(this._mat_at, this._mat_a);
		mat_t.matrixSelfInv();		
		return;		
	}
	/**
	 * 画面座標群と3次元座標群から、平行移動量を計算します。
	 * 2d座標系は、直前に実行したset2dVertexのものを使用します。
	 * @param i_vertex_2d
	 * 直前のset2dVertexコールで指定したものと同じものを指定してください。
	 * @param i_vertex3d
	 * 3次元空間の座標群を設定します。頂点の順番は、画面座標群と同じ順序で格納してください。
	 * @param o_transfer
	 * @throws NyARException
	 */
	public void solveTransportVector(NyARDoublePoint3d[] i_vertex3d,NyARDoublePoint3d o_transfer)
	{
		double[][] matc = this._mat_c.getArray();
		double cpara00=this._projection_mat.m00;
		double cpara01=this._projection_mat.m01;
		double cpara02=this._projection_mat.m02;
		double cpara11=this._projection_mat.m11;
		double cpara12=this._projection_mat.m12;
		double[] cx=this._cx;
		double[] cy=this._cy;
		
		//（3D座標？）を一括請求
		for (int i = 0; i < 4; i++) {
			int x2 = i+i;
			NyARDoublePoint3d point3d_ptr=i_vertex3d[i];
			//透視変換？
			matc[x2][0] = point3d_ptr.z * cx[i] - cpara00 * point3d_ptr.x - cpara01 * point3d_ptr.y - cpara02 * point3d_ptr.z;// mat_c->m[j*2+0] = wz*pos2d[j][0]-cpara[0][0]*wx-cpara[0][1]*wy-cpara[0][2]*wz;
			matc[x2 + 1][0] = point3d_ptr.z * cy[i] - cpara11 * point3d_ptr.y - cpara12 * point3d_ptr.z;// mat_c->m[j*2+1]= wz*pos2d[j][1]-cpara[1][1]*wy-cpara[1][2]*wz;
		}
		this._mat_e.matrixMul(this._mat_at,this._mat_c);
		this._mat_f.matrixMul(this._mat_t, this._mat_e);
		
		double[][] matf = this._mat_f.getArray();
		o_transfer.x= matf[0][0];// trans[0] = mat_f->m[0];
		o_transfer.y= matf[1][0];
		o_transfer.z= matf[2][0];// trans[2] = mat_f->m[2];
		return;		
	}
}
}
