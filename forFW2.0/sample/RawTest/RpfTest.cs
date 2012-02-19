/* 
 * PROJECT: Capture Test NyARToolkitCSサンプルプログラム
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
using System.IO;
using System.Diagnostics;
using jp.nyatla.nyartoolkit.cs;
using jp.nyatla.nyartoolkit.cs.core;
using jp.nyatla.nyartoolkit.cs.detector;
using jp.nyatla.nyartoolkit.cs.rpf;

//using jp.nyatla.nyartoolkit.cs.sandbox.x2;
//using jp.nyatla.nyartoolkit.cs.sandbox.quadx2;
namespace ConsoleApplication1
{
    /**
     * NyARRealityのテストプログラム。動作保証なし。
     * 
     * ターゲットプロパティの取得実験用のテストコードです。
     * クリックしたマーカや、その平面周辺から、画像を取得するテストができます。
     *
     */

    public class RpfTest
    {
        private const String PARAM_FILE = "../../../../../data/camera_para.dat";
        private const String DATA_FILE = "../../../../../data/320x240ABGR.raw";
	    public void Test()
	    {

		    try {
			    NyARParam param=new NyARParam();
			    param.loadARParam(new StreamReader(PARAM_FILE));
			    param.changeScreenSize(320,240);
			    NyARReality reality=new NyARReality(param.getScreenSize(),10,1000,param.getPerspectiveProjectionMatrix(),null,10,10);
			    NyARRealitySource reality_in=new NyARRealitySource_Reference(320,240,null,2,100,NyARBufferType.BYTE1D_B8G8R8X8_32);

                //試験イメージの読み出し(320x240 BGRAのRAWデータ)
                StreamReader sr = new StreamReader(DATA_FILE);
                BinaryReader bs = new BinaryReader(sr.BaseStream);
                byte[] raw = bs.ReadBytes(320 * 240 * 4);
                Array.Copy(raw, (byte[])reality_in.refRgbSource().getBuffer(), raw.Length);
                
                
                Stopwatch sw = new Stopwatch();
                sw.Start();
			    for(int i=0;i<1000;i++){
				    reality.progress(reality_in);
			    }
                sw.Stop();
                Console.WriteLine(sw.ElapsedMilliseconds + "[ms]");    			
			    Console.WriteLine(reality.getNumberOfKnown());
			    Console.WriteLine(reality.getNumberOfUnknown());
			    Console.WriteLine(reality.getNumberOfDead());
                NyARRealityTarget[] rt = new NyARRealityTarget[10];
			    reality.selectUnKnownTargets(rt);
			    reality.changeTargetToKnown(rt[0],2,80);
			    Console.WriteLine(rt[0]._transform_matrix.m00+","+rt[0]._transform_matrix.m01+","+rt[0]._transform_matrix.m02+","+rt[0]._transform_matrix.m03);
			    Console.WriteLine(rt[0]._transform_matrix.m10+","+rt[0]._transform_matrix.m11+","+rt[0]._transform_matrix.m12+","+rt[0]._transform_matrix.m13);
			    Console.WriteLine(rt[0]._transform_matrix.m20+","+rt[0]._transform_matrix.m21+","+rt[0]._transform_matrix.m22+","+rt[0]._transform_matrix.m23);
			    Console.WriteLine(rt[0]._transform_matrix.m30+","+rt[0]._transform_matrix.m31+","+rt[0]._transform_matrix.m32+","+rt[0]._transform_matrix.m33);
		    } catch (Exception e) {
			   Console.WriteLine(e.StackTrace);
		    }
            return;
	    }
    }
}