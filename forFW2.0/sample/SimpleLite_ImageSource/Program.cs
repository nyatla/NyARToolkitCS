using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace SimpleLite_ImageSource
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


            // フォームとメインサンプルクラスを作成
            using (Form1 frm = new Form1())
            using (SimpleLite_ImageSource sample = new SimpleLite_ImageSource())
            {
                // アプリケーションの初期化
                if (sample.InitializeApplication(frm))
                {
                    // メインフォームを表示
                    frm.Show();
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
                }
                else
                {
                    // 初期化に失敗
                }
            }
        }
    }
}
