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
    public interface NyARTransRot
    {
        double[] getArray();
        /**
         * 
         * @param trans
         * @param vertex
         * @param pos2d
         * [n*2]配列
         * @return
         * @throws NyARException
         */
        double modifyMatrix(double[] trans,double[,] vertex, double[,] pos2d);
        void initRot(NyARSquare marker_info,int i_direction);
        void initRotByPrevResult(NyARTransMatResult i_prev_result);
    }

    /**
     * NyARTransRot派生クラスで共通に使いそうな関数類をまとめたもの。
     *
     */
    public abstract class NyARTransRot_OptimizeCommon : NyARTransRot
    {
        protected int number_of_vertex;
        protected double[] array=new double[9];
        protected NyARParam cparam;
        public void initRotByPrevResult(NyARTransMatResult i_prev_result)
        {
	        double[][] prev_array=i_prev_result.getArray();
	        double[] pt;
	        double[] L_rot=this.array;
	        pt=prev_array[0];
            L_rot[0 * 3 + 0] = pt[0];
            L_rot[0 * 3 + 1] = pt[1];
            L_rot[0 * 3 + 2] = pt[2];
	        pt=prev_array[1];
            L_rot[1 * 3 + 0] = pt[0];
            L_rot[1 * 3 + 1] = pt[1];
            L_rot[1 * 3 + 2] = pt[2];
	        pt=prev_array[2];
            L_rot[2 * 3 + 0] = pt[0];
            L_rot[2 * 3 + 1] = pt[1];
            L_rot[2 * 3 + 2] = pt[2];
        }
        public double[] getArray()
        {
    	    return this.array;
        }
        /**
         * インスタンスを準備します。
         * @param i_param
         * nullを指定した場合、一部の関数が使用不能になります。
         */
        public NyARTransRot_OptimizeCommon(NyARParam i_param,int i_number_of_vertex)
        {
	        number_of_vertex=i_number_of_vertex;
	        cparam=i_param;
        }

        private double[] wk_check_dir_world=new double[6];
        private double[] wk_check_dir_camera=new double[4];
        private NyARMat wk_check_dir_NyARMat=new NyARMat( 3, 3 );
        /**
         * static int check_dir( double dir[3], double st[2], double ed[2],double cpara[3][4] )
         * Optimize:STEP[526->468]
         * @param dir
         * @param st
         * @param ed
         * @param cpara
         * 
         * @throws NyARException
         */
        protected void check_dir(double[] dir, double[] st, double[] ed,double[] cpara)
        {
	        double    h;
	        int       i, j;

	        NyARMat mat_a = this.wk_check_dir_NyARMat;//ここ、事前に初期化できそう
	        double[][] a_array=mat_a.getArray();
	        for(j=0;j<3;j++){
	            for(i=0;i<3;i++){
                    a_array[j][i]=cpara[j*4+i];//m[j*3+i] = cpara[j][i];
	            }
        	    
	        }
	        //	JartkException.trap("未チェックのパス");
	        mat_a.matrixSelfInv();
	        double[] world=wk_check_dir_world;//[2][3];
	        //<Optimize>
	        //world[0][0] = a_array[0][0]*st[0]*10.0+ a_array[0][1]*st[1]*10.0+ a_array[0][2]*10.0;//mat_a->m[0]*st[0]*10.0+ mat_a->m[1]*st[1]*10.0+ mat_a->m[2]*10.0;
	        //world[0][1] = a_array[1][0]*st[0]*10.0+ a_array[1][1]*st[1]*10.0+ a_array[1][2]*10.0;//mat_a->m[3]*st[0]*10.0+ mat_a->m[4]*st[1]*10.0+ mat_a->m[5]*10.0;
	        //world[0][2] = a_array[2][0]*st[0]*10.0+ a_array[2][1]*st[1]*10.0+ a_array[2][2]*10.0;//mat_a->m[6]*st[0]*10.0+ mat_a->m[7]*st[1]*10.0+ mat_a->m[8]*10.0;
	        //world[1][0] = world[0][0] + dir[0];
	        //world[1][1] = world[0][1] + dir[1];
	        //world[1][2] = world[0][2] + dir[2];
            world[0] = a_array[0][0] * st[0] * 10.0 + a_array[0][1] * st[1] * 10.0 + a_array[0][2] * 10.0;//mat_a->m[0]*st[0]*10.0+ mat_a->m[1]*st[1]*10.0+ mat_a->m[2]*10.0;
            world[1] = a_array[1][0] * st[0] * 10.0 + a_array[1][1] * st[1] * 10.0 + a_array[1][2] * 10.0;//mat_a->m[3]*st[0]*10.0+ mat_a->m[4]*st[1]*10.0+ mat_a->m[5]*10.0;
            world[2] = a_array[2][0] * st[0] * 10.0 + a_array[2][1] * st[1] * 10.0 + a_array[2][2] * 10.0;//mat_a->m[6]*st[0]*10.0+ mat_a->m[7]*st[1]*10.0+ mat_a->m[8]*10.0;
	        world[3] = world[0] + dir[0];
	        world[4] = world[1] + dir[1];
	        world[5] = world[2] + dir[2];
	        //</Optimize>

	        double[] camera=wk_check_dir_camera;//[2][2];
	        for( i = 0; i < 2; i++ ) {
	            h = cpara[2*4+0] * world[i*3+0]+ cpara[2*4+1] * world[i*3+1]+ cpara[2*4+2] * world[i*3+2];
	            if( h == 0.0 ){
		        throw new NyARException();
	            }
	            camera[i*2+0] = (cpara[0*4+0] * world[i*3+0]+ cpara[0*4+1] * world[i*3+1]+ cpara[0*4+2] * world[i*3+2]) / h;
	            camera[i*2+1] = (cpara[1*4+0] * world[i*3+0]+ cpara[1*4+1] * world[i*3+1]+ cpara[1*4+2] * world[i*3+2]) / h;
	        }
	        //<Optimize>
	        //v[0][0] = ed[0] - st[0];
	        //v[0][1] = ed[1] - st[1];
	        //v[1][0] = camera[1][0] - camera[0][0];
	        //v[1][1] = camera[1][1] - camera[0][1];
	        double v=(ed[0]-st[0])*(camera[2]-camera[0])+(ed[1]-st[1])*(camera[3]-camera[1]);
	        //</Optimize>
	        if(v<0) {//if( v[0][0]*v[1][0] + v[0][1]*v[1][1] < 0 ) {
	            dir[0] = -dir[0];
	            dir[1] = -dir[1];
	            dir[2] = -dir[2];
	        }
        }
        /*int check_rotation( double rot[2][3] )*/
        protected static void check_rotation(double[][] rot )
        {
	        double[]  v1=new double[3], v2=new double[3], v3=new double[3];
	        double  ca, cb, k1, k2, k3, k4;
	        double  a, b, c, d;
	        double  p1, q1, r1;
	        double  p2, q2, r2;
	        double  p3, q3, r3;
	        double  p4, q4, r4;
	        double  w;
	        double  e1, e2, e3, e4;
	        int     f;

	        v1[0] = rot[0][0];
            v1[1] = rot[0][1];
            v1[2] = rot[0][2];
            v2[0] = rot[1][0];
            v2[1] = rot[1][1];
            v2[2] = rot[1][2];


            v3[0] = v1[1] * v2[2] - v1[2] * v2[1];
            v3[1] = v1[2] * v2[0] - v1[0] * v2[2];
            v3[2] = v1[0] * v2[1] - v1[1] * v2[0];
	        w = Math.Sqrt( v3[0]*v3[0]+v3[1]*v3[1]+v3[2]*v3[2] );
	        if( w == 0.0 ){
	            throw new NyARException();
	        }
	        v3[0] /= w;
	        v3[1] /= w;
	        v3[2] /= w;

	        cb = v1[0]*v2[0] + v1[1]*v2[1] + v1[2]*v2[2];
	        if( cb < 0 ) cb *= -1.0;
	        ca = (Math.Sqrt(cb+1.0) + Math.Sqrt(1.0-cb)) * 0.5;

	        if( v3[1]*v1[0] - v1[1]*v3[0] != 0.0 ) {
	            f = 0;
	        }
	        else {
	            if( v3[2]*v1[0] - v1[2]*v3[0] != 0.0 ) {
		        w = v1[1]; v1[1] = v1[2]; v1[2] = w;
		        w = v3[1]; v3[1] = v3[2]; v3[2] = w;
		        f = 1;
	            }
	            else {
		        w = v1[0]; v1[0] = v1[2]; v1[2] = w;
		        w = v3[0]; v3[0] = v3[2]; v3[2] = w;
		        f = 2;
	            }
	        }
	        if( v3[1]*v1[0] - v1[1]*v3[0] == 0.0 ){
	            throw new NyARException();
	        }
	        k1 = (v1[1]*v3[2] - v3[1]*v1[2]) / (v3[1]*v1[0] - v1[1]*v3[0]);
	        k2 = (v3[1] * ca) / (v3[1]*v1[0] - v1[1]*v3[0]);
	        k3 = (v1[0]*v3[2] - v3[0]*v1[2]) / (v3[0]*v1[1] - v1[0]*v3[1]);
	        k4 = (v3[0] * ca) / (v3[0]*v1[1] - v1[0]*v3[1]);

	        a = k1*k1 + k3*k3 + 1;
	        b = k1*k2 + k3*k4;
	        c = k2*k2 + k4*k4 - 1;

	        d = b*b - a*c;
	        if( d < 0 ){
	            throw new NyARException();
	        }
	        r1 = (-b + Math.Sqrt(d))/a;
	        p1 = k1*r1 + k2;
	        q1 = k3*r1 + k4;
	        r2 = (-b - Math.Sqrt(d))/a;
	        p2 = k1*r2 + k2;
	        q2 = k3*r2 + k4;
	        if( f == 1 ) {
	            w = q1; q1 = r1; r1 = w;
	            w = q2; q2 = r2; r2 = w;
	            w = v1[1]; v1[1] = v1[2]; v1[2] = w;
	            w = v3[1]; v3[1] = v3[2]; v3[2] = w;
	            f = 0;
	        }
	        if( f == 2 ) {
	            w = p1; p1 = r1; r1 = w;
	            w = p2; p2 = r2; r2 = w;
	            w = v1[0]; v1[0] = v1[2]; v1[2] = w;
	            w = v3[0]; v3[0] = v3[2]; v3[2] = w;
	            f = 0;
	        }

	        if( v3[1]*v2[0] - v2[1]*v3[0] != 0.0 ) {
	            f = 0;
	        }else {
	            if( v3[2]*v2[0] - v2[2]*v3[0] != 0.0 ) {
		        w = v2[1]; v2[1] = v2[2]; v2[2] = w;
		        w = v3[1]; v3[1] = v3[2]; v3[2] = w;
		        f = 1;
	            }
	            else {
		        w = v2[0]; v2[0] = v2[2]; v2[2] = w;
		        w = v3[0]; v3[0] = v3[2]; v3[2] = w;
		        f = 2;
	            }
	        }
	        if( v3[1]*v2[0] - v2[1]*v3[0] == 0.0 ){
	            throw new NyARException();
	        }
	        k1 = (v2[1]*v3[2] - v3[1]*v2[2]) / (v3[1]*v2[0] - v2[1]*v3[0]);
	        k2 = (v3[1] * ca) / (v3[1]*v2[0] - v2[1]*v3[0]);
	        k3 = (v2[0]*v3[2] - v3[0]*v2[2]) / (v3[0]*v2[1] - v2[0]*v3[1]);
	        k4 = (v3[0] * ca) / (v3[0]*v2[1] - v2[0]*v3[1]);

	        a = k1*k1 + k3*k3 + 1;
	        b = k1*k2 + k3*k4;
	        c = k2*k2 + k4*k4 - 1;

	        d = b*b - a*c;
	        if( d < 0 ){
	            throw new NyARException();
	        }
	        r3 = (-b + Math.Sqrt(d))/a;
	        p3 = k1*r3 + k2;
	        q3 = k3*r3 + k4;
	        r4 = (-b - Math.Sqrt(d))/a;
	        p4 = k1*r4 + k2;
	        q4 = k3*r4 + k4;
	        if( f == 1 ) {
	            w = q3; q3 = r3; r3 = w;
	            w = q4; q4 = r4; r4 = w;
	            w = v2[1]; v2[1] = v2[2]; v2[2] = w;
	            w = v3[1]; v3[1] = v3[2]; v3[2] = w;
	            f = 0;
	        }
	        if( f == 2 ) {
	            w = p3; p3 = r3; r3 = w;
	            w = p4; p4 = r4; r4 = w;
	            w = v2[0]; v2[0] = v2[2]; v2[2] = w;
	            w = v3[0]; v3[0] = v3[2]; v3[2] = w;
	            f = 0;
	        }

	        e1 = p1*p3+q1*q3+r1*r3;
	        if( e1 < 0 ){
	            e1 = -e1;
	        }
	        e2 = p1*p4+q1*q4+r1*r4;
	        if( e2 < 0 ){
	            e2 = -e2;
	        }
	        e3 = p2*p3+q2*q3+r2*r3;
	        if( e3 < 0 ){
	            e3 = -e3;
	        }
	        e4 = p2*p4+q2*q4+r2*r4;
	        if( e4 < 0 ){
	            e4 = -e4;
	        }
	        if( e1 < e2 ) {
	            if( e1 < e3 ) {
		        if( e1 < e4 ) {
                    rot[0][0] = p1;
                    rot[0][1] = q1;
                    rot[0][2] = r1;
                    rot[1][0] = p3;
                    rot[1][1] = q3;
                    rot[1][2] = r3;
		        }
		        else {
                    rot[0][0] = p2;
                    rot[0][1] = q2;
                    rot[0][2] = r2;
                    rot[1][0] = p4;
                    rot[1][1] = q4;
                    rot[1][2] = r4;
		        }
	            }
	            else {
		        if( e3 < e4 ) {
                    rot[0][0] = p2;
                    rot[0][1] = q2;
                    rot[0][2] = r2;
                    rot[1][0] = p3;
                    rot[1][1] = q3;
                    rot[1][2] = r3;
		        }
		        else {
                    rot[0][0] = p2;
                    rot[0][1] = q2;
                    rot[0][2] = r2;
                    rot[1][0] = p4;
                    rot[1][1] = q4;
		            rot[1][2] = r4;
		        }
	            }
	        }
	        else {
	            if( e2 < e3 ) {
		        if( e2 < e4 ) {
                    rot[0][0] = p1;
                    rot[0][1] = q1;
                    rot[0][2] = r1;
                    rot[1][0] = p4;
                    rot[1][1] = q4;
                    rot[1][2] = r4;
		        }
		        else {
                    rot[0][0] = p2;
                    rot[0][1] = q2;
                    rot[0][2] = r2;
                    rot[1][0] = p4;
                    rot[1][1] = q4;
                    rot[1][2] = r4;
		        }
	            }
	            else {
		        if( e3 < e4 ) {
                    rot[0][0] = p2;
                    rot[0][1] = q2;
                    rot[0][2] = r2;
                    rot[1][0] = p3;
                    rot[1][1] = q3;
                    rot[1][2] = r3;
		        }
		        else {
                    rot[0][0] = p2;
                    rot[0][1] = q2;
                    rot[0][2] = r2;
                    rot[1][0] = p4;
                    rot[1][1] = q4;
                    rot[1][2] = r4;
		        }
	            }
	        }
        }  
        /**
         * パラメタa,b,cからrotを計算してインスタンスに保存する。
         * rotを1次元配列に変更
         * Optimize:2008.04.20:STEP[253→186]
         * @param a
         * @param b
         * @param c
         * @param o_rot
         */
        protected static void arGetRot( double a, double b, double c,double[] o_rot)
        {
	        double   sina, sinb, sinc;
	        double   cosa, cosb, cosc;

	        sina = Math.Sin(a);
	        cosa = Math.Cos(a);
	        sinb = Math.Sin(b);
	        cosb = Math.Cos(b);
	        sinc = Math.Sin(c);
	        cosc = Math.Cos(c);
	        //Optimize
	        double CACA,SASA,SACA,SASB,CASB,SACACB;
	        CACA  =cosa*cosa;
	        SASA  =sina*sina;
	        SACA  =sina*cosa;
	        SASB  =sina*sinb;
	        CASB  =cosa*sinb;
	        SACACB=SACA*cosb;
        	

	        o_rot[0] = CACA*cosb*cosc+SASA*cosc+SACACB*sinc-SACA*sinc;
	        o_rot[1] = -CACA*cosb*sinc-SASA*sinc+SACACB*cosc-SACA*cosc;
	        o_rot[2] = CASB;
	        o_rot[3] = SACACB*cosc-SACA*cosc+SASA*cosb*sinc+CACA*sinc;
	        o_rot[4] = -SACACB*sinc+SACA*sinc+SASA*cosb*cosc+CACA*cosc;
	        o_rot[5] = SASB;
	        o_rot[6] = -CASB*cosc-SASB*sinc;
	        o_rot[7] = CASB*sinc-SASB*cosc;
	        o_rot[8] = cosb;
        }
        /**
         * int arGetAngle( double rot[3][3], double *wa, double *wb, double *wc )
         * Optimize:2008.04.20:STEP[481→433]
         * @param rot
         * 2次元配列を1次元化してあります。
         * @param o_abc
         * @return
         */
        protected int arGetAngle(double[] o_abc)
        {
	        double      a, b, c,tmp;
	        double      sina, cosa, sinb, cosb, sinc, cosc;
	        double[] rot=array;
	        if( rot[8] > 1.0 ) {//<Optimize/>if( rot[2][2] > 1.0 ) {
	            rot[8] = 1.0;//<Optimize/>rot[2][2] = 1.0;
	        }else if( rot[8] < -1.0 ) {//<Optimize/>}else if( rot[2][2] < -1.0 ) {
	            rot[8] = -1.0;//<Optimize/>rot[2][2] = -1.0;
	        }
	        cosb = rot[8];//<Optimize/>cosb = rot[2][2];
	        b = Math.Acos( cosb );
	        sinb = Math.Sin( b );
	        if( b >= 0.000001 || b <= -0.000001) {
	            cosa = rot[2] / sinb;//<Optimize/>cosa = rot[0][2] / sinb;
	            sina = rot[5] / sinb;//<Optimize/>sina = rot[1][2] / sinb;
	            if( cosa > 1.0 ) {
		        /* printf("cos(alph) = %f\n", cosa); */
		        cosa = 1.0;
		        sina = 0.0;
	            }
	            if( cosa < -1.0 ) {
		        /* printf("cos(alph) = %f\n", cosa); */
		        cosa = -1.0;
		        sina =  0.0;
	            }
	            if( sina > 1.0 ) {
		        /* printf("sin(alph) = %f\n", sina); */
		        sina = 1.0;
		        cosa = 0.0;
	            }
	            if( sina < -1.0 ) {
		        /* printf("sin(alph) = %f\n", sina); */
		        sina = -1.0;
		        cosa =  0.0;
	            }
	            a = Math.Acos( cosa );
	            if( sina < 0 ){
		        a = -a;
	            }
	            //<Optimize>
	            //sinc =  (rot[2][1]*rot[0][2]-rot[2][0]*rot[1][2])/ (rot[0][2]*rot[0][2]+rot[1][2]*rot[1][2]);
	            //cosc =  -(rot[0][2]*rot[2][0]+rot[1][2]*rot[2][1])/ (rot[0][2]*rot[0][2]+rot[1][2]*rot[1][2]);
	            tmp = (rot[2]*rot[2]+rot[5]*rot[5]);
	            sinc =  (rot[7]*rot[2]-rot[6]*rot[5])/ tmp;
	            cosc =  -(rot[2]*rot[6]+rot[5]*rot[7])/ tmp;
	            //</Optimize>

	            if( cosc > 1.0 ) {
		        /* printf("cos(r) = %f\n", cosc); */
		        cosc = 1.0;
		        sinc = 0.0;
	            }
	            if( cosc < -1.0 ) {
		        /* printf("cos(r) = %f\n", cosc); */
		        cosc = -1.0;
		        sinc =  0.0;
	            }
	            if( sinc > 1.0 ) {
		        /* printf("sin(r) = %f\n", sinc); */
		        sinc = 1.0;
		        cosc = 0.0;
	            }
	            if( sinc < -1.0 ) {
		        /* printf("sin(r) = %f\n", sinc); */
		        sinc = -1.0;
		        cosc =  0.0;
	            }
	            c = Math.Acos( cosc );
	            if( sinc < 0 ){
		        c = -c;
	            }
	        }else {
	            a = b = 0.0;
	            cosa = cosb = 1.0;
	            sina = sinb = 0.0;
	            cosc = rot[0];//<Optimize/>cosc = rot[0][0];
	            sinc = rot[1];//<Optimize/>sinc = rot[1][0];
	            if( cosc > 1.0 ) {
		        /* printf("cos(r) = %f\n", cosc); */
		        cosc = 1.0;
		        sinc = 0.0;
	            }
	            if( cosc < -1.0 ) {
		        /* printf("cos(r) = %f\n", cosc); */
		        cosc = -1.0;
		        sinc =  0.0;
	            }
	            if( sinc > 1.0 ) {
		        /* printf("sin(r) = %f\n", sinc); */
		        sinc = 1.0;
		        cosc = 0.0;
	            }
	            if( sinc < -1.0 ) {
		        /* printf("sin(r) = %f\n", sinc); */
		        sinc = -1.0;
		        cosc =  0.0;
	            }
	            c = Math.Acos( cosc );
	            if( sinc < 0 ){
		        c = -c;
	            }
	        }
	        o_abc[0]=a;//wa.value=a;//*wa = a;
	        o_abc[1]=b;//wb.value=b;//*wb = b;
	        o_abc[2]=c;//wc.value=c;//*wc = c;
	        return 0;
        }
        public abstract double modifyMatrix(double[] trans, double[,] vertex, double[,] pos2d);
        public abstract void initRot(NyARSquare marker_info, int i_direction);   
    }
    /**
     * NyARModifyMatrixの最適化バージョン3
     * O3版の演算テーブル版
     * 計算速度のみを追求する
     *
     */
    class NyARTransRot_O3 : NyARTransRot_OptimizeCommon
    {
        public NyARTransRot_O3(NyARParam i_param,int i_number_of_vertex):base(i_param,i_number_of_vertex)
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
		            sinb[j]=Math.Sin(w);
		            cosb[j]=Math.Cos(w);
		            w= c2 + w2;
		            c_factor[j]=w;
		            sinc[j]=Math.Sin(w);
		            cosc[j]=Math.Cos(w);
	            }
	            //
	            for(t1=0;t1<3;t1++) {
		            SA = Math.Sin(a_factor[t1]);
		            CA = Math.Cos(a_factor[t1]);
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
