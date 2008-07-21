/*
 * NyARToolkitCSUtils NyARToolkit for C#
 * SimpleLiteDirect3d for WindowsMobile
 * 
 * (c)2008 nyatla
 * airmail(at)ebony.plala.or.jp
 * http://nyatla.jp/
 */
using System;
using System.Threading;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SimpleLiteDirect3d.WindowsMobile5
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
            using (NyARToolkitCS frm = new NyARToolkitCS())
            {
                DeviceAdapter dev_adapter;
                //デバイスを選択して作る。
                using (Form2 frm2 = new Form2())
                {
                    frm2.ShowDialog();
                    dev_adapter = frm2.GetSelectedDeviceAdapter();
                }
                //デバイス作れなかったらおしまい。
                if (dev_adapter == null)
                {
                    return;
                }
                using (SimpleLiteD3d sample = new SimpleLiteD3d())
                {

                    dev_adapter.Init(frm.ClientSize, sample);

                    // アプリケーションの初期化
                    if (sample.InitializeApplication(frm, dev_adapter))
                    {
                        // メインフォームを表示
                        frm.Show();
                        //キャプチャ開始
                        sample.StartCap();
                        // フォームにフォーカスがある間はループし続ける
                        while (frm.Focused)
                        {
                            // メインループ処理を行う
                            sample.MainLoop();

                            //スレッドスイッチ
                            Thread.Sleep(0);



                            // イベントがある場合はその処理する
                            Application.DoEvents();
                        }
                        //キャプチャの停止
                        sample.StopCap();
                    }
                    else
                    {
                        // 初期化に失敗
                    }
                    dev_adapter.Finish();
                }
            }
        }
    }
}