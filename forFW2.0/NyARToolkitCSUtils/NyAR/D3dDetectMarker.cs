/*
 * NyARToolkitCSUtils NyARToolkit for C# 支援ライブラリ
 * 
 * (c)2008 A虎＠nyatla.jp
 * airmail(at)ebony.plala.or.jp
 * http://nyatla.jp/
 */
using System;
using System.Collections.Generic;
using Microsoft.DirectX;
using jp.nyatla.nyartoolkit.cs.core;
using jp.nyatla.nyartoolkit.cs.detector;


namespace NyARToolkitCSUtils.NyAR
{
    /*
     * Direct3dに使うことが出来るMatrixを出力できるようにしたNyARSingleDetectMarkerです。
     * ※テストしてないけど多分大丈夫だと思います…。
     */
    public class D3dDetectMarker : jp.nyatla.nyartoolkit.cs.detector.NyARDetectMarker
    {
        private NyARTransMatResult m_result = new NyARTransMatResult();
        public D3dDetectMarker(NyARParam i_param, NyARCode[] i_code, double[] i_marker_width, int i_number_of_code)
            : base(i_param,i_code,i_marker_width,i_number_of_code)
        {
        }

        public void getD3dMatrix(int i_index, out Matrix o_result)
        {
            this.getTransmationMatrix(i_index,m_result);
            double[][] s = m_result.getArray();

            //ARのMatrixをDirectXのMatrixに変換
            o_result.M11 = (float)s[0][0];
            o_result.M12 = (float)s[1][0];
            o_result.M13 = (float)s[2][0];
            o_result.M14 = 0;
            o_result.M21 = (float)s[0][1];
            o_result.M22 = (float)s[1][1];
            o_result.M23 = (float)s[2][1];
            o_result.M24 = 0;
            o_result.M31 = (float)s[0][2];
            o_result.M32 = (float)s[1][2];
            o_result.M33 = (float)s[2][2];
            o_result.M34 = 0;
            o_result.M41 = (float)s[0][3];
            o_result.M42 = (float)s[1][3];
            o_result.M43 = (float)s[2][3];
            o_result.M44 = 1;
        }
    }
}