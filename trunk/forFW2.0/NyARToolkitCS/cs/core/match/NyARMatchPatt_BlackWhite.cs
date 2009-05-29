using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace jp.nyatla.nyartoolkit.cs.core
{
    /**
     * AR_TEMPLATE_MATCHING_BWと同等のルールで マーカを評価します。
     * 
     */
    public class NyARMatchPatt_BlackWhite : INyARMatchPatt
    {
        protected NyARCode _code_patt;
        protected int _pixels;

        public NyARMatchPatt_BlackWhite(int i_width, int i_height)
        {
            //最適化定数の計算
            this._pixels = i_height * i_width;
            return;
        }
        public NyARMatchPatt_BlackWhite(NyARCode i_code_ref)
        {
            //最適化定数の計算
            this._pixels = i_code_ref.getWidth() * i_code_ref.getHeight();
            this._code_patt = i_code_ref;
            return;
        }
        /**
         * 比較対象のARCodeをセットします。
         * @throws NyARException
         */
        public void setARCode(NyARCode i_code_ref)
        {
            this._code_patt = i_code_ref;
            return;
        }
        /**
         * 現在セットされているコードとパターンを比較して、結果値o_resultを更新します。
         * 比較部分はFor文を16倍展開してあります。
         */
        public bool evaluate(NyARMatchPattDeviationBlackWhiteData i_patt, NyARMatchPattResult o_result)
        {
            Debug.Assert(this._code_patt != null);

            int[] linput = i_patt.refData();
            int sum;
            double max = 0.0;
            int res = NyARSquare.DIRECTION_UNKNOWN;


            for (int j = 0; j < 4; j++)
            {
                //合計値初期化
                sum = 0;
                NyARMatchPattDeviationBlackWhiteData code_patt = this._code_patt.getBlackWhiteData(j);
                int[] pat_j = code_patt.refData();
                //<全画素について、比較(FORの1/16展開)/>
                int i;
                for (i = this._pixels - 1; i >= 0; i--)
                {
                    sum += linput[i] * pat_j[i];
                }
                //0.7776737688877927がでればOK
                double sum2 = sum / code_patt.getPow() / i_patt.getPow();// sum2 = sum / patpow[k][j]/ datapow;
                if (sum2 > max)
                {
                    max = sum2;
                    res = j;
                }
            }
            o_result.direction = res;
            o_result.confidence = max;
            return true;
        }
    }

}
