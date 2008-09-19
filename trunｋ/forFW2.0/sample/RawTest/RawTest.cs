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
        private const String camera_file = "../../../../../data/camera_para.dat";
        public RawFileTest()
        {
        }
        public void Test_arDetectMarkerLite()
        {
            //AR用カメラパラメタファイルをロード
            NyARParam ap = new NyARParam();
            ap.loadARParamFromFile(camera_file);
            ap.changeScreenSize(320, 240);

            //AR用のパターンコードを読み出し	
            NyARCode code = new NyARCode(16, 16);
            code.loadARPattFromFile(code_file);

            //試験イメージの読み出し(320x240 BGRAのRAWデータ)
            StreamReader sr = new StreamReader(data_file);
            BinaryReader bs = new BinaryReader(sr.BaseStream);
            byte[] raw = bs.ReadBytes(320 * 240 * 4);
            NyARRgbRaster_BGRA ra = NyARRgbRaster_BGRA.wrap(raw, 320, 240);
            //		Blank_Raster ra=new Blank_Raster(320, 240);

            //１パターンのみを追跡するクラスを作成
            NyARSingleDetectMarker ar = new NyARSingleDetectMarker(ap, code, 80.0);
            NyARTransMatResult result_mat = new NyARTransMatResult();
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
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds + "[ms]");
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
