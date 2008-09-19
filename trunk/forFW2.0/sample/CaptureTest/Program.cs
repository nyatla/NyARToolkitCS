/**
 * Capture Test NyARToolkitCSサンプルプログラム
 * 
 * このサンプルは、カメラキャプチャデバイスから得た画像内からARコード（マーカー）を
 * 探し出し、もし存在すればその変換行列を数値で表示します。 
 * 画像はビットマップで、GDIによって出力します。
 * 
 * (c)2008 A虎＠nyatla.jp
 * airmail(at)ebony.plala.or.jp
 * http://nyatla.jp/
 */
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CaptureTest
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
