using System;
using System.Collections.Generic;
using System.Text;

namespace jp.nyatla.nyartoolkit.cs.nyidmarker
{
    /**
     * [[Strage class]]
     * マーカを抽出した時のパラメータを格納するクラスです。
     *
     */
    public class NyIdMarkerParam
    {
        /**
         * マーカの方位値です。
         */
        public int direction;
        /**
         * マーカ周辺のパターン閾値です。
         */
        public int threshold;

    }
}
