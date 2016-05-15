/* 
 * Capture Test NyARToolkitCSサンプルプログラム
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
using System.Windows.Forms;
using System.Threading;
using NyARToolkitCSUtils.Capture;
/**
 * 
 * このサンプルプログラムは、NyARToolkitのSimpleLite相当のサンプルプログラムです。
 * Hiroマーカーを識別し、最も一致するマーカーの上に、立方体を表示します。
 * 
 * Direct3Dの単位系は、1.0を1mmとしています。
 * 視点は0,0,0から、Z+方向を向いて、上方向がY+です。
 * 
 * 実装には、下記URLの情報を参考にしています。
 * http://sorceryforce.com/manageddirectx/index.html
 * http://codezine.jp/a/article/aid/226.aspx?p=2
 */
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
            int cdevice_number = 0;
            using (CameraSelectDialog frm2 = new CameraSelectDialog())
            {
                frm2.ShowDialog(capture_device_list, out cdevice_number);
            }
            using (CaptureDevice capture_device = capture_device_list[cdevice_number])
            {
                // フォームとメインサンプルクラスを作成
                using (Form1 frm = new Form1())
                using (SimpleLiteD3d sample = new SimpleLiteD3d())
                {
                    // アプリケーションの初期化
                    if (sample.InitializeApplication(frm, capture_device))
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
}
