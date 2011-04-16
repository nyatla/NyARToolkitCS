using System;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;
using System.Windows.Forms;

namespace NyARToolkitCS.WM5.RPF
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [MTAThread]
        static void Main()
        {
            // フォームとメインサンプルクラスを作成
            using (Form1 frm = new Form1())
            {
                ResourceBuilder nyar_res;
                try
                {
                    nyar_res = new ResourceBuilder();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "SimpleLiteD3d::デバイスの初期化に失敗しました。");
                    return;
                }

                using (Test_NyARRealityD3d_ARMarker sample = new Test_NyARRealityD3d_ARMarker(frm, nyar_res))
                {


                    // メインフォームを表示
                    frm.Show();
                    //キャプチャ開始
                    sample.start();
                    Stopwatch sw = new Stopwatch();
                    // フォームにフォーカスがある間はループし続ける
                    while (frm.Focused)
                    {
                        sw.Start();
                        // メインループ処理を行う
                        sample.MainLoop();
                        //スレッドスイッチ
                        Thread.Sleep(0);



                        // イベントがある場合はその処理する
                        Application.DoEvents();
                        sw.Stop();
                        //sample.fps_x_100 = (int)(1000 * 100 / (sw.ElapsedMilliseconds+1));
                        sw.Reset();

                    }
                    //キャプチャの停止
                    sample.stop();
                }
            }
            return;
        }
    }
}