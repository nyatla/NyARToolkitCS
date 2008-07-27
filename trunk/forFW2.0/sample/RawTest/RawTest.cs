/**
 * Capture Test NyARToolkitCSサンプルプログラム
 * 
 * このサンプルプログラムは、NyARToolkitのRawFileTestサンプルを実行します。
 * マーカーを1000回検出し、変換行列を計算するためにかかった時間を表示します。
 * 
 * (c)2008 A虎＠nyatla.jp
 * airmail(at)ebony.plala.or.jp
 * http://nyatla.jp/
 */
using System;
using System.IO;
using System.Diagnostics;
using jp.nyatla.nyartoolkit.cs;
using jp.nyatla.nyartoolkit.cs.core;
using jp.nyatla.nyartoolkit.cs.match;
using jp.nyatla.nyartoolkit.cs.raster;
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
            ap.loadFromARFile(camera_file);
            ap.changeSize(320, 240);

            //AR用のパターンコードを読み出し	
            NyARCode code = new NyARCode(16, 16);
            code.loadFromARFile(code_file);

            //試験イメージの読み出し(320x240 BGRAのRAWデータ)
            StreamReader sr = new StreamReader(data_file);
            BinaryReader bs = new BinaryReader(sr.BaseStream);
            byte[] raw = bs.ReadBytes(320 * 240 * 4);
            NyARRaster_BGRA ra = NyARRaster_BGRA.wrap(raw, 320, 240);
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
