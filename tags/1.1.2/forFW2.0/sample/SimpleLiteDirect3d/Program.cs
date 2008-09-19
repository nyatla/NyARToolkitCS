/**
 * Capture Test NyARToolkitCSサンプルプログラム
 * 
 * このサンプルプログラムは、NyARToolkitのSimpleLite相当のサンプルプログラムです。
 * Hiroマーカーを識別し、最も一致するマーカーの上に、立方体（フルカラーナタデココ）
 * を表示します。
 * 
 * Direct3Dの単位系は、1.0を1mmとしています。
 * 視点は0,0,0から、Z+方向を向いて、上方向がY+です。
 * 
 * 実装には、下記URLの情報を参考にしています。
 * http://sorceryforce.com/manageddirectx/index.html
 * http://codezine.jp/a/article/aid/226.aspx?p=2
 * 
 * 
 * (c)2008 A虎＠nyatla.jp
 * airmail(at)ebony.plala.or.jp
 * http://nyatla.jp/
 */
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using NyARToolkitCSUtils.Capture;

namespace SimpleLiteDirect3d
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


            //キャプチャデバイスリストを取得
            CaptureDeviceList capture_device_list = new CaptureDeviceList();
            if (capture_device_list.count < 1)
            {
                MessageBox.Show("キャプチャデバイスが見つかりませんでした。");
                return;
            }
            //キャプチャデバイスを選択してもらう。
            int cdevice_number=0;
            using (Form2 frm2 = new Form2())
            {
                frm2.ShowDialog(capture_device_list, out cdevice_number);
            }
            CaptureDevice capture_device=capture_device_list[cdevice_number];


            // フォームとメインサンプルクラスを作成
            using (Form1 frm = new Form1())
            using (SimpleLiteD3d sample = new SimpleLiteD3d())
            {
                // アプリケーションの初期化
                if (sample.InitializeApplication(frm,capture_device))
                {
                    // メインフォームを表示
                    frm.Show();
                    //キャプチャ開始
                    sample.StartCap();
                    // フォームが作成されている間はループし続ける
                    while (frm.Created)
                    {
                        // メインループ処理を行う
                        sample.MainLoop();

                        //スレッドスイッチ
                        Thread.Sleep(1);

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
            }
        }
    }
}
