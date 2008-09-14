/**
 * NyARToolkitCSUtils NyARToolkit for C# 支援ライブラリ
 * 
 * (c)2008 A虎＠nyatla.jp
 * airmail(at)ebony.plala.or.jp
 * http://nyatla.jp/
 */
using System;
using System.Collections.Generic;
using DirectShowLib;
namespace NyARToolkitCSUtils.Capture
{
    /* キャプチャデバイスリストを保持するオブジェクト。
     * キャプチャデバイス集合を保持するクラスです。
     * 動作中にキャプチャデバイスの数に変動があった場合には追従できない。
     */
    public class CaptureDeviceList
    {
        public List<CaptureDevice> m_dev_list = new List<CaptureDevice>();
        public CaptureDeviceList()
        {
            DsDevice[] capDevices;

            // Get the collection of video devices
            capDevices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
            for (int i = 0; i < capDevices.Length; i++)
            {
                this.m_dev_list.Add(new CaptureDevice(capDevices[i]));
            }
        }
        public CaptureDevice this[int index]
        {
            get { return this.m_dev_list[index]; }
        }
        public int count
        {
            get { return this.m_dev_list.Count; }
        }
        public void Dump()
        {
            for (int i = 0; i < this.m_dev_list.Count; i++)
            {
                Console.Out.Write(i + ":");
                this.m_dev_list[i].Dump();
            }
        }
    }
}
