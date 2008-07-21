using System;
using System.Collections.Generic;
using jp.nyatla.nyartoolkit.cs;
using jp.nyatla.nyartoolkit.cs.match;
using jp.nyatla.nyartoolkit.cs.core;

namespace jp.nyatla.nyartoolkit.cs.match
{
    /**
     * ARColorPattのマッチング計算をするインタフェイスです。
     * 基準Patに対して、計算済みのARCodeデータとの間で比較演算をします。
     * pattern_match関数を分解した３種類のパターン検出クラスを定義します。
     *
     */
    public interface NyARMatchPatt{
        double getConfidence();
        int getDirection();
        void evaluate(NyARCode i_code);
        bool setPatt(NyARColorPatt i_target_patt);
    }
}
