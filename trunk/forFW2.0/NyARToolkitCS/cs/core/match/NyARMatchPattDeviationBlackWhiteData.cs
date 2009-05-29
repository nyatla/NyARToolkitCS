using System;
using System.Collections.Generic;
using System.Text;

namespace jp.nyatla.nyartoolkit.cs.core
{
    /**
     * INyARMatchPattのColor差分ラスタを格納するクラスです。
     *
     */
    public class NyARMatchPattDeviationBlackWhiteData
    {
        private int[] _data;
        private double _pow;
        //
        private int _number_of_pixels;
        public int[] refData()
        {
            return this._data;
        }
        public double getPow()
        {
            return this._pow;
        }

        public NyARMatchPattDeviationBlackWhiteData(int i_width, int i_height)
        {
            this._number_of_pixels = i_height * i_width;
            this._data = new int[this._number_of_pixels];
            return;
        }
        /**
         * XRGB[width*height]の配列から、パターンデータを構築。
         * @param i_buffer
         */
        public void setRaster(INyARRaster i_raster)
        {
            //i_buffer[XRGB]→差分[BW]変換			
            int i;
            int ave;//<PV/>
            int rgb;//<PV/>
            int[] linput = this._data;//<PV/>
            int[] buf = (int[])i_raster.getBufferReader().getBuffer();

            // input配列のサイズとwhも更新// input=new int[height][width][3];
            int number_of_pixels = this._number_of_pixels;

            //<平均値計算(FORの1/8展開)/>
            ave = 0;
            for (i = number_of_pixels - 1; i >= 0; i--)
            {
                rgb = buf[i];
                ave += ((rgb >> 16) & 0xff) + ((rgb >> 8) & 0xff) + (rgb & 0xff);
            }
            ave = (number_of_pixels * 255 * 3 - ave) / (3 * number_of_pixels);
            //
            int sum = 0, w_sum;

            //<差分値計算/>
            for (i = number_of_pixels - 1; i >= 0; i--)
            {
                rgb = buf[i];
                w_sum = ((255 * 3 - (rgb & 0xff) - ((rgb >> 8) & 0xff) - ((rgb >> 16) & 0xff)) / 3) - ave;
                linput[i] = w_sum;
                sum += w_sum * w_sum;
            }
            double p = Math.Sqrt((double)sum);
            this._pow = p != 0.0 ? p : 0.0000001;
            return;
        }
    }

}
