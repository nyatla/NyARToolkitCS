/* 
 * PROJECT: NyARToolkitCSUtils NyARToolkit for C# 支援ライブラリ
 * --------------------------------------------------------------------------------
 * The MIT License
 * Copyright (c) 2008 nyatla
 * airmail(at)ebony.plala.or.jp
 * http://nyatla.jp/nyartoolkit/
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 * 
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
            return;
        }

        public void getD3dMatrix(int i_index, out Matrix o_result)
        {
            this.getTransmationMatrix(i_index,m_result);

            //ARのMatrixをDirectXのMatrixに変換
            o_result.M11 = (float)m_result.m00;
            o_result.M12 = (float)m_result.m10;
            o_result.M13 = (float)m_result.m20;
            o_result.M14 = 0;
            o_result.M21 = (float)m_result.m01;
            o_result.M22 = (float)m_result.m11;
            o_result.M23 = (float)m_result.m21;
            o_result.M24 = 0;
            o_result.M31 = (float)m_result.m02;
            o_result.M32 = (float)m_result.m12;
            o_result.M33 = (float)m_result.m22;
            o_result.M34 = 0;
            o_result.M41 = (float)m_result.m03;
            o_result.M42 = (float)m_result.m13;
            o_result.M43 = (float)m_result.m23;
            o_result.M44 = 1;
            return;
        }
    }
}