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
using jp.nyatla.nyartoolkit.cs.utils;

namespace jp.nyatla.nyartoolkit.cs.core
{


    /**
     * NyARModifyMatrixの最適化バージョン3
     * O3版の演算テーブル版
     * 計算速度のみを追求する
     *
     */
    class NyARTransRot_Mobile : NyARTransRot_OptimizeCommon
    {
        private NySinTable sin_tbl = new NySinTable();
        public NyARTransRot_Mobile(NyARParam i_param,int i_number_of_vertex):base(i_param,i_number_of_vertex)
        {
	        if(i_number_of_vertex!=4){
	            //4以外の頂点数は処理しない
	            throw new NyARException();
	        }
        }  
        
        //private double CACA,SASA,SACA,CA,SA;    
        private double[][] wk_initRot_wdir=new double[3][]{
            new double[3],
            new double[3],
            new double[3]};
        /**
         * int arGetInitRot( ARMarkerInfo *marker_info, double cpara[3][4], double rot[3][3] )
         * Optimize:2008.04.20:STEP[716→698]
         * @param marker_info
         * @param i_direction
         * @param i_param
         * @throws NyARException
         */
        public override void initRot(NyARSquare marker_info,int i_direction)
        {
	        double[] cpara=	cparam.get34Array();
	        double[][]  wdir=wk_initRot_wdir;//この関数で初期化される
	        double  w, w1, w2, w3;
	        int     dir;
	        int     j;

	        dir = i_direction;

	        for( j = 0; j < 2; j++ ) {
                w1 = marker_info.line[(4 - dir + j) % 4][0] * marker_info.line[(6 - dir + j) % 4][1] - marker_info.line[(6 - dir + j) % 4][0] * marker_info.line[(4 - dir + j) % 4][1];
                w2 = marker_info.line[(4 - dir + j) % 4][1] * marker_info.line[(6 - dir + j) % 4][2] - marker_info.line[(6 - dir + j) % 4][1] * marker_info.line[(4 - dir + j) % 4][2];
                w3 = marker_info.line[(4 - dir + j) % 4][2] * marker_info.line[(6 - dir + j) % 4][0] - marker_info.line[(6 - dir + j) % 4][2] * marker_info.line[(4 - dir + j) % 4][0];

	            wdir[j][0] =  w1*(cpara[0*4+1]*cpara[1*4+2]-cpara[0*4+2]*cpara[1*4+1])+  w2*cpara[1*4+1]-  w3*cpara[0*4+1];
	            wdir[j][1] = -w1*cpara[0*4+0]*cpara[1*4+2]+  w3*cpara[0*4+0];
	            wdir[j][2] =  w1*cpara[0*4+0]*cpara[1*4+1];
	            w = Math.Sqrt( wdir[j][0]*wdir[j][0]+ wdir[j][1]*wdir[j][1]+ wdir[j][2]*wdir[j][2] );
	            wdir[j][0] /= w;
                wdir[j][1] /= w;
                wdir[j][2] /= w;
	        }

	        //以下3ケースは、計算エラーのときは例外が発生する。
	        check_dir(wdir[0], marker_info.sqvertex[(4-dir)%4],marker_info.sqvertex[(5-dir)%4], cpara);

	        check_dir(wdir[1], marker_info.sqvertex[(7-dir)%4],marker_info.sqvertex[(4-dir)%4], cpara);

	        check_rotation(wdir);


	        wdir[2][0] = wdir[0][1]*wdir[1][2] - wdir[0][2]*wdir[1][1];
	        wdir[2][1] = wdir[0][2]*wdir[1][0] - wdir[0][0]*wdir[1][2];
	        wdir[2][2] = wdir[0][0]*wdir[1][1] - wdir[0][1]*wdir[1][0];
	        w = Math.Sqrt( wdir[2][0]*wdir[2][0]+ wdir[2][1]*wdir[2][1]+ wdir[2][2]*wdir[2][2] );
	        wdir[2][0] /= w;
	        wdir[2][1] /= w;
	        wdir[2][2] /= w;
	        double[] rot=this.array;
	        rot[0] = wdir[0][0];
	        rot[3] = wdir[0][1];
	        rot[6] = wdir[0][2];
	        rot[1] = wdir[1][0];
	        rot[4] = wdir[1][1];
	        rot[7] = wdir[1][2];
	        rot[2] = wdir[2][0];
	        rot[5] = wdir[2][1];
	        rot[8] = wdir[2][2];
	        //</Optimize>    
        }

        protected override void arGetRot(double a, double b, double c, double[] o_rot)
        {
            /*
                    |cos(a) -sin(a) 0| |cos(b)  0 sin(b)| |cos(a-c)  sin(a-c) 0|
              rot = |sin(a)  cos(a) 0| |0       1     0 | |-sin(a-c) cos(a-c) 0|
                    |0       0      1| |-sin(b) 0 cos(b)| |0         0        1|
             */

            double Sa, Sb, Ca, Cb, Sac, Cac, CaCb, SaCb;
            Sa = sin_tbl.Sin(a);
            Ca = sin_tbl.Cos(a);
            Sb = sin_tbl.Sin(b);
            Cb = sin_tbl.Cos(b);
            Sac = sin_tbl.Sin(a - c);
            Cac = sin_tbl.Cos(a - c);
            CaCb = Ca * Cb;
            SaCb = Sa * Cb;
            /*
             o_rot[0]
              = Ca*Ca*Cb*Cc+Sa*Sa*Cc+SaCa*Cb*Sc-Sa*Ca*Sc
              = Ca*Cb*cos(a-c) + Sa*sin(a-c)
             o_rot[1]
              = -Ca*Ca*Cb*Sc-Sa*Sa*Sc+Sa*Ca*Cb*Cc-Sa*Ca*Cc
              = Ca*Cb*sin(a-c) - Sa*cos(a-c)
             o_rot[3]
              = Sa*Ca*Cb*Cc-Sa*Ca*Cc+Sa*Sa*Cb*Sc+Ca*Ca*Sc
              = Sa*Cb*cos(a-c) - Ca*sin(a-c)
             o_rot[4]
              = -Sa*Ca*Cb*Sc+Sa*Ca*Sc+Sa*Sa*Cb*Cc+Ca*Ca*Cc
              = Sa*Cb*sin(a-c) + Ca*cos(a-c)
             o_rot[6]
              = -Ca*Sb*Cc-Sa*Sb*Sc;
              = -Sb*cos(a-c)
             o_rot[7]
              = Ca*Sb*Sc-Sa*Sb*Cc;
              = -Sb*sin(a-c)
             */

            o_rot[0] = CaCb * Cac + Sa * Sac;
            o_rot[1] = CaCb * Sac - Sa * Cac;
            o_rot[2] = Ca * Sb;
            o_rot[3] = SaCb * Cac - Ca * Sac;
            o_rot[4] = SaCb * Sac + Ca * Cac;
            o_rot[5] = Sa * Sb;
            o_rot[6] = -Sb * Cac;
            o_rot[7] = -Sb * Sac;
            o_rot[8] = Cb;
            return;

        }


        private double[][] wk_arModifyMatrix_double1D=new double[8][]{
            new double[3],
            new double[3],
            new double[3],
            new double[3],
            new double[3],
            new double[3],
            new double[3],
            new double[3]
        };
        /**
         * arGetRot計算を階層化したModifyMatrix
         * 896
         * @param nyrot
         * @param trans
         * @param vertex
         * [m][3]
         * @param pos2d
         * [n][2]
         * @return
         * @throws NyARException
         */
        public override double modifyMatrix(double[] trans,double[,] vertex, double[,] pos2d)
        {
	        double    factor;
	        double    a2, b2, c2;
	        double    ma = 0.0, mb = 0.0, mc = 0.0;
	        double    h, x, y;
	        double    err, minerr=0;
	        int       t1, t2, t3;
	        int       s1 = 0, s2 = 0, s3 = 0;

	        factor = 10.0*Math.PI/180.0;
	        double rot0,rot1,rot3,rot4,rot6,rot7;
	        double combo00,combo01,combo02,combo03,combo10,combo11,combo12,combo13,combo20,combo21,combo22,combo23;
	        double combo02_2,combo02_5,combo02_8,combo02_11;
	        double combo22_2,combo22_5,combo22_8,combo22_11;
	        double combo12_2,combo12_5,combo12_8,combo12_11;
	        //vertex展開
	        double VX00,VX01,VX02,VX10,VX11,VX12,VX20,VX21,VX22,VX30,VX31,VX32;
            VX00 = vertex[0, 0]; VX01 = vertex[0, 1]; VX02 = vertex[0, 2];
            VX10 = vertex[1, 0]; VX11 = vertex[1, 1]; VX12 = vertex[1, 2];
            VX20 = vertex[2, 0]; VX21 = vertex[2, 1]; VX22 = vertex[2, 2];
            VX30 = vertex[3, 0]; VX31 = vertex[3, 1]; VX32 = vertex[3, 2];
	        double P2D00,P2D01,P2D10,P2D11,P2D20,P2D21,P2D30,P2D31;
            P2D00 = pos2d[0, 0]; P2D01 = pos2d[0, 1];
            P2D10 = pos2d[1, 0]; P2D11 = pos2d[1, 1];
            P2D20 = pos2d[2, 0]; P2D21 = pos2d[2, 1];
            P2D30 = pos2d[3, 0]; P2D31 = pos2d[3, 1];
	        double[] cpara=cparam.get34Array();
	        double CP0,CP1,CP2,CP3,CP4,CP5,CP6,CP7,CP8,CP9,CP10;
	        CP0=cpara[0];CP1=cpara[1];CP2=cpara[2];CP3=cpara[3];
	        CP4=cpara[4];CP5=cpara[5];CP6=cpara[6];CP7=cpara[7];
	        CP8=cpara[8];CP9=cpara[9];CP10=cpara[10];
	        combo03 = CP0 * trans[0]+ CP1 * trans[1]+ CP2 * trans[2]+ CP3;
	        combo13 = CP4 * trans[0]+ CP5 * trans[1]+ CP6 * trans[2]+ CP7;
	        combo23 = CP8 * trans[0]+ CP9 * trans[1]+ CP10 * trans[2]+ cpara[11];
	        double CACA,SASA,SACA,CA,SA;
	        double CACACB,SACACB,SASACB,CASB,SASB;
	        double SACASC,SACACBSC,SACACBCC,SACACC;        
	        double[][] double1D=this.wk_arModifyMatrix_double1D;

	        double[] abc     =double1D[0];
	        double[] a_factor=double1D[1];
	        double[] sinb    =double1D[2];
	        double[] cosb    =double1D[3];
	        double[] b_factor=double1D[4];
	        double[] sinc    =double1D[5];
	        double[] cosc    =double1D[6];
	        double[] c_factor=double1D[7];
	        double w,w2;
	        double wsin,wcos;

	        arGetAngle(abc);//arGetAngle( rot, &a, &b, &c );
	        a2 = abc[0];
	        b2 = abc[1];
	        c2 = abc[2];
        	
	        //comboの3行目を先に計算
	        for(int i = 0; i < 10; i++ ) {
	            minerr = 1000000000.0;
	            //sin-cosテーブルを計算(これが外に出せるとは…。)
	            for(int j=0;j<3;j++){
		            w2=factor*(j-1);
		            w= a2 + w2;
		            a_factor[j]=w;
		            w= b2 + w2;
		            b_factor[j]=w;
                    sinb[j] = sin_tbl.Sin(w);
                    cosb[j] = sin_tbl.Cos(w);
		            w= c2 + w2;
		            c_factor[j]=w;
                    sinc[j] = sin_tbl.Sin(w);
                    cosc[j] = sin_tbl.Cos(w);
	            }
	            //
	            for(t1=0;t1<3;t1++) {
                    SA = sin_tbl.Sin(a_factor[t1]);
                    CA = sin_tbl.Cos(a_factor[t1]);
		            //Optimize
		            CACA=CA*CA;
		            SASA=SA*SA;
		            SACA=SA*CA;
		            for(t2=0;t2<3;t2++) {
		                wsin=sinb[t2];
		                wcos=cosb[t2];
		                CACACB=CACA*wcos;
		                SACACB=SACA*wcos;
		                SASACB=SASA*wcos;
		                CASB=CA*wsin;
		                SASB=SA*wsin;
		                //comboの計算1
		                combo02 = CP0 * CASB+ CP1 * SASB+ CP2 * wcos;
		                combo12 = CP4 * CASB+ CP5 * SASB+ CP6 * wcos;
		                combo22 = CP8 * CASB+ CP9 * SASB+ CP10 * wcos;

		                combo02_2 =combo02 * VX02 + combo03;
		                combo02_5 =combo02 * VX12 + combo03;
		                combo02_8 =combo02 * VX22 + combo03;
		                combo02_11=combo02 * VX32 + combo03;
		                combo12_2 =combo12 * VX02 + combo13;
		                combo12_5 =combo12 * VX12 + combo13;
		                combo12_8 =combo12 * VX22 + combo13;
		                combo12_11=combo12 * VX32 + combo13;
		                combo22_2 =combo22 * VX02 + combo23;
		                combo22_5 =combo22 * VX12 + combo23;
		                combo22_8 =combo22 * VX22 + combo23;
		                combo22_11=combo22 * VX32 + combo23;	    
		                for(t3=0;t3<3;t3++){
			                wsin=sinc[t3];
			                wcos=cosc[t3];			
			                SACASC=SACA*wsin;
			                SACACC=SACA*wcos;
			                SACACBSC=SACACB*wsin;
			                SACACBCC=SACACB*wcos;

			                rot0 = CACACB*wcos+SASA*wcos+SACACBSC-SACASC;
			                rot3 = SACACBCC-SACACC+SASACB*wsin+CACA*wsin;
			                rot6 = -CASB*wcos-SASB*wsin;

			                combo00 = CP0 * rot0+ CP1 * rot3+ CP2 * rot6;
			                combo10 = CP4 * rot0+ CP5 * rot3+ CP6 * rot6;
			                combo20 = CP8 * rot0+ CP9 * rot3+ CP10 * rot6;

			                rot1 = -CACACB*wsin-SASA*wsin+SACACBCC-SACACC;
			                rot4 = -SACACBSC+SACASC+SASACB*wcos+CACA*wcos;
			                rot7 = CASB*wsin-SASB*wcos;
			                combo01 = CP0 * rot1+ CP1 * rot4+ CP2 * rot7;
			                combo11 = CP4 * rot1+ CP5 * rot4+ CP6 * rot7;
			                combo21 = CP8 * rot1+ CP9 * rot4+ CP10 * rot7;
			                //
			                err = 0.0;
			                h  = combo20 * VX00+ combo21 * VX01+ combo22_2;
			                x = P2D00 - (combo00 * VX00+ combo01 * VX01+ combo02_2) / h;
			                y = P2D01 - (combo10 * VX00+ combo11 * VX01+ combo12_2) / h;
			                err += x*x+y*y;
			                h  = combo20 * VX10+ combo21 * VX11+ combo22_5;
			                x = P2D10 - (combo00 * VX10+ combo01 * VX11+ combo02_5) / h;
			                y = P2D11 - (combo10 * VX10+ combo11 * VX11+ combo12_5) / h;
			                err += x*x+y*y;
			                h  = combo20 * VX20+ combo21 * VX21+ combo22_8;
			                x = P2D20 - (combo00 * VX20+ combo01 * VX21+ combo02_8) / h;
			                y = P2D21 - (combo10 * VX20+ combo11 * VX21+ combo12_8) / h;
			                err += x*x+y*y;
			                h  = combo20 * VX30+ combo21 * VX31+ combo22_11;
			                x = P2D30 - (combo00 * VX30+ combo01 * VX31+ combo02_11) / h;
			                y = P2D31 - (combo10 * VX30+ combo11 * VX31+ combo12_11) / h;
			                err += x*x+y*y;
			                if( err < minerr ) {
			                    minerr = err;
			                    ma = a_factor[t1];
			                    mb = b_factor[t2];
			                    mc = c_factor[t3];
			                    s1 = t1-1;
			                    s2 = t2-1;
			                    s3 = t3-1;
			                }
		                }
		            }
	            }
                if( s1 == 0 && s2 == 0 && s3 == 0 ){
		            factor *= 0.5;
	            }
	            a2 = ma;
	            b2 = mb;
	            c2 = mc;
	        }
	        arGetRot(ma,mb,mc,this.array);
	        /*  printf("factor = %10.5f\n", factor*180.0/MD_PI); */
	        return minerr/4;
        }                       
    }
}