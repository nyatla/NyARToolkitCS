/* 
 * PROJECT: NyARToolkitCSUtils NyARToolkit for C# 支援ライブラリ
 * --------------------------------------------------------------------------------
 * The MIT License
 * Copyright (c) 2008 nyatla
 * airmail(at)ebony.plala.or.jp
 * http://nyatla.jp/nyartoolkit/
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 * 
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
