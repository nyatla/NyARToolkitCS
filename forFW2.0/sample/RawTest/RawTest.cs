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
using System.Collections.Generic;
using jp.nyatla.nyartoolkit.cs.sample;

namespace ConsoleApplication1
{
    class RawTest
    {
        static void Main(string[] args)
        {
            RawFileTest rf;
            rf = new RawFileTest();
            rf.Test_arDetectMarkerLite();
        }
    }
}
