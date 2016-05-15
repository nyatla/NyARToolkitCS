using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using NyARToolkitCSUtils.Capture;
namespace SimpleLiteForm
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
                using (Form1 frm = new Form1()){
                    // メインフォームを表示
                    frm.Show();
                    using (SimpleLiteMain sample = new SimpleLiteMain(frm, capture_device))
                    {
                        capture_device.StartCapture();
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
                        capture_device.StopCapture();
                    }
                }
            }
        }
    }
}
