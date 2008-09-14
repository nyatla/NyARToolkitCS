/**
 * NyARToolkitCSUtils NyARToolkit for C# 支援ライブラリ
 * 
 * (c)2008 A虎＠nyatla.jp
 * airmail(at)ebony.plala.or.jp
 * http://nyatla.jp/
 */
using System;
using System.Collections.Generic;

namespace NyARToolkitCSUtils.Capture
{
    /* CaptureDeviceからフレームメモリを回収するためのリスナインタフェイス
     * CaptureDeviceのキャプチャイベントを受け取るインタフェイスです。
     */
    public interface CaptureListener
    {
        /// <summary> sample callback, NOT USED. </summary>
        void OnBuffer(CaptureDevice i_sender, double i_sample_time, IntPtr i_buffer, int i_buffer_len);
    }
}
