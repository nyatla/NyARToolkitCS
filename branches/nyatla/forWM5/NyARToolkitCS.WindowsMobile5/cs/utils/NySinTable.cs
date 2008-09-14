using System;
using System.Text;

namespace jp.nyatla.nyartoolkit.cs.utils
{
    class NySinTable
    {
        const int DIVISION = 1024;

        private static Object m_single_lock=new Object();
        private static double[] m_table =null;
        public NySinTable()
        {

            lock (NySinTable.m_single_lock)
            {
                if (NySinTable.m_table == null)
                {
                    NySinTable.m_table = new double[DIVISION];
                    int d4 = DIVISION / 4;

                    //テーブル初期化(0-PI/2を作成)
                    for (int i = 1; i < DIVISION; i++)
                    {
                        NySinTable.m_table[i] = (Math.Sin(2*Math.PI * (double)i / (double)DIVISION));
                    }
                    NySinTable.m_table[0] = 0;
                    NySinTable.m_table[d4 - 1] = 1;
                    NySinTable.m_table[d4 * 2 - 1] = 0;
                    NySinTable.m_table[d4 * 3 - 1] = -1;
                }
            }

        }
        public double Sin(double i_rad)
        {
            //0～Math.PI/2を　0～256の値空間に変換
            int rad_index = (int)(i_rad * DIVISION / (2*Math.PI));
            //-1024<rad_index<=1024に整形しる
            rad_index = rad_index % DIVISION;
            //負の領域に居たら、+1024しておく
            if (rad_index < 0)
            {
                rad_index += DIVISION;
            }
            //ここで0-1024にいるから…、
            return NySinTable.m_table[rad_index];
        }
        public double Cos(double i_rad)
        {
            //0～Math.PI/2を　0～256の値空間に変換
            int rad_index = (int)(i_rad * DIVISION / (2*Math.PI));
            //90度ずらす
            rad_index = (rad_index + DIVISION/4) % DIVISION;
            //負の領域に居たら、+1024しておく
            if (rad_index < 0)
            {
                rad_index += DIVISION;
            }
            //ここで0-1024にいるから…、
            return NySinTable.m_table[rad_index];
        }
    }
}
