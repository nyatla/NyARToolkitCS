/*
 * NyARToolkitCSUtils NyARToolkit for C# 支援ライブラリ
 * 
 * (c)2008 A虎＠nyatla.jp
 * airmail(at)ebony.plala.or.jp
 * http://nyatla.jp/
 */
using System;
using Microsoft.WindowsMobile.DirectX.Direct3D;
using Microsoft.WindowsMobile.DirectX;
using jp.nyatla.nyartoolkit.cs.detector;
using jp.nyatla.nyartoolkit.cs.core;

namespace NyARToolkitCSUtils.NyAR
{
    /*
     * Direct3dに使うことが出来るMatrixを出力できるようにしたNyARSingleDetectMarkerです。
     */
    public class D3dSingleDetectMarker : NyARSingleDetectMarker
    {
        private NyARTransMatResult m_result = new NyARTransMatResult();
        public D3dSingleDetectMarker(NyARParam i_param, NyARCode i_code, double i_marker_width)
            : base(i_param, i_code, i_marker_width)
        {
        }

        public void getD3dMatrix(out Matrix o_result)
        {
            this.getTransmationMatrix(m_result);
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
