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
using NyARToolkitCSUtils;
using System.Drawing;
namespace ConsoleApplication1
{
    /**
     * 320x240のBGRA32で記録されたRAWイメージから、１種類のパターンを認識し、
     * その変換行列を1000回求め、それにかかったミリ秒時間を表示します。
     *
     */
    public class RawFileTest
    {
        private const String code_file = "../../../../../data/patt.hiro";
        private const String data_file = "../../../../../data/320x240ABGR.raw";
        private const String camera_file = "../../../../../data/camera_para4.dat";
        public RawFileTest()
        {
//            NyMath.initialize();
        }
        public void Test()
        {
            //AR用カメラパラメタファイルをロード
            NyARParam ap = NyARParam.loadFromARParamFile(File.OpenRead(camera_file),320,240);

            //AR用のパターンコードを読み出し	
            NyARCode code = NyARCode.loadFromARPattFile(File.OpenRead(code_file), 16, 16);

            //試験イメージの読み出し(320x240 BGRAのRAWデータ)
            StreamReader sr = new StreamReader(data_file);
            BinaryReader bs = new BinaryReader(sr.BaseStream);
            byte[] raw = bs.ReadBytes(320 * 240 * 4);
            
//            NyARBitmapRaster ra = new NyARBitmapRaster(320, 240);
//            Graphics g = Graphics.FromImage(ra.getBitmap());
//            g.DrawImage(new Bitmap("../../../../../data/320x240ABGR.png"), 0, 0);
            

            INyARRgbRaster ra = NyARRgbRaster.createInstance(320, 240,NyARBufferType.BYTE1D_B8G8R8X8_32,false);
            ra.wrapBuffer(raw);

            //１パターンのみを追跡するクラスを作成
            NyARSingleDetectMarker ar = NyARSingleDetectMarker.createInstance(ap, code, 80.0,NyARSingleDetectMarker.PF_NYARTOOLKIT);
            NyARDoubleMatrix44 result_mat = new NyARDoubleMatrix44();
            ar.setContinueMode(false);
            ar.detectMarkerLite(ra, 100);
            ar.getTransmationMatrix(result_mat);

            //マーカーを検出
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < 1000; i++)
            {
                //変換行列を取得
                ar.detectMarkerLite(ra, 100);
                ar.getTransmationMatrix(result_mat);
            }
            Console.WriteLine(result_mat.m00 + "," + result_mat.m01 + ","+result_mat.m02+","+result_mat.m03);
            Console.WriteLine(result_mat.m10 + "," + result_mat.m11 + ","+result_mat.m12+","+result_mat.m13);
            Console.WriteLine(result_mat.m20 + "," + result_mat.m21 + ","+result_mat.m22+","+result_mat.m23);
            Console.WriteLine(result_mat.m30 + "," + result_mat.m31 + ","+result_mat.m32+","+result_mat.m33);
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds + "[ms]");
            return;
        }
    }
}
