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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using jp.nyatla.nyartoolkit.cs;
using jp.nyatla.nyartoolkit.cs.core;
using jp.nyatla.nyartoolkit.cs.sandbox.x2;
using jp.nyatla.nyartoolkit.cs.sandbox.quadx2;
using jp.nyatla.nyartoolkit.cs.detector;

namespace ConsoleApplication1
{
    /**
     * 320x240のBGRA32で記録されたRAWイメージから、１種類のパターンを認識し、
     * その変換行列を1000回求め、それにかかったミリ秒時間を表示します。
     *
     */
    public class RawFileTest
    {
        private const String RES_PATT = "RawTest.data.patt.hiro";
        private const String RES_CAMERA = "RawTest.data.camera_para.dat";
        private const String RES_DATA = "RawTest.data.320x240ABGR.raw";
        public RawFileTest()
        {
            NyMath.initialize();
        }
        public void Test_arDetectMarkerLite()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            
            //AR用カメラパラメタファイルをロード
            NyARParam ap = new NyARParam();
            ap.loadARParam(assembly.GetManifestResourceStream(RES_CAMERA));
            ap.changeScreenSize(320, 240);

            //AR用のパターンコードを読み出し	
            NyARCode code = new NyARCode(16, 16);
            Stream sr1=assembly.GetManifestResourceStream(RES_PATT);
            code.loadARPatt(new StreamReader(sr1));

            //試験イメージの読み出し(320x240 BGRAのRAWデータ)
            StreamReader sr = new StreamReader(assembly.GetManifestResourceStream(RES_DATA));
            BinaryReader bs = new BinaryReader(sr.BaseStream);
            byte[] raw = bs.ReadBytes(320 * 240 * 4);
            NyARRgbRaster_BGRA ra = new NyARRgbRaster_BGRA(320, 240,false);
            ra.wrapBuffer(raw);
            //		Blank_Raster ra=new Blank_Raster(320, 240);

            //１パターンのみを追跡するクラスを作成
//            NyARSingleDetectMarker_Quad ar = new NyARSingleDetectMarker_Quad(ap, code, 80.0);
            NyARSingleDetectMarker ar = new NyARSingleDetectMarker(ap, code, 80.0,ra.getBufferType());
            NyARTransMatResult result_mat = new NyARTransMatResult();
            ar.setContinueMode(false);
            ar.detectMarkerLite(ra, 100);
            ar.getTransmationMatrix(result_mat);

            //マーカーを検出
            for (int i3 = 0; i3 < 10; i3++)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                for (int i = 0; i < 10; i++)
                {
                    //変換行列を取得
                    ar.detectMarkerLite(ra, 100);
                    ar.getTransmationMatrix(result_mat);
                }
                sw.Stop();
                Debug.WriteLine(sw.ElapsedMilliseconds + "[ms]");
            }
            return;
        }
        public static void main(String[] args)
        {
            try
            {
                RawFileTest t = new RawFileTest();
                //t.Test_arGetVersion();
                t.Test_arDetectMarkerLite();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        static void Main(string[] args)
        {
            RawFileTest rf;
            rf = new RawFileTest();
            rf.Test_arDetectMarkerLite();
        }
    }
}