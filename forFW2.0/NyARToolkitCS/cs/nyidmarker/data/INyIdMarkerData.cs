using System;
using System.Collections.Generic;
using System.Text;

namespace jp.nyatla.nyartoolkit.cs.nyidmarker
{
    public interface INyIdMarkerData
    {
        /**
         * i_targetのマーカデータと自身のデータが等しいかを返します。
         * @param i_target
         * 比較するマーカオブジェクト
         * @return
         * 等しいかの真偽値
         */
        bool isEqual(INyIdMarkerData i_target);
        /**
         * i_sourceからマーカデータをコピーします。
         * @param i_source
         */
        void copyFrom(INyIdMarkerData i_source);
    }
}
