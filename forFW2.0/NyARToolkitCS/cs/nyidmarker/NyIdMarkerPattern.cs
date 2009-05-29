using System;
using System.Collections.Generic;
using System.Text;

namespace jp.nyatla.nyartoolkit.cs.nyidmarker
{
    /**
     * [[Strage class]]
     * IDマーカパターン値を格納するクラスです。
     * クラスは、未整形のマーカデータを格納しています。
     *
     */
    public class NyIdMarkerPattern
    {
        public int model;
        public int ctrl_domain;
        public int ctrl_mask;
        public int check;
        public int[] data = new int[32];
    }
}
