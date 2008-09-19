/* 
 * The MIT License
 * 
 * Copyright (c) 2008 nyatla
 * airmail(at)ebony.plala.or.jp
 * http://nyatla.jp/
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
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
using System.Text;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace jp.nyatla.cs.NyWMCapture
{
    [ComImport, Guid("32f37e70-b633-4253-b8e0-a99a1bbeea84")]
    public class NyWMCapture
    {
        public const UInt32 DeviceId_WM5     = 0x00000001;
        public const UInt32 DeviceId_Win32   = 0x00000002;
        public const UInt32 DeviceId_S01SH   = 0x00010001;
        public const UInt32 DeviceId_WS007SH = 0x00010002;
        /*	MediaSubType*/
        public const UInt32 MediaSubType_RGB565 = 0x00000001;

        /*	PIN CATEGORY ID*/
        public const UInt32 PinCategory_CAPTURE = 0x00000001;
        public const UInt32 PinCategory_PREVIEW = 0x00000002;
        public const UInt32 PinCategory_ANALOGVIDEOIN = 0x00000003;
        public const UInt32 PinCategory_VBI = 0x00000004;
        public const UInt32 PinCategory_VIDEOPORT = 0x00000005;
        public const UInt32 PinCategory_NABTS = 0x00000006;
        public const UInt32 PinCategory_EDS = 0x00000007;
        public const UInt32 PinCategory_TELETEXT = 0x00000008;
        public const UInt32 PinCategory_CC = 0x00000009;
        public const UInt32 PinCategory_STILL = 0x0000000a;
    }

    [ComImport,
    Guid("f633a3d8-d61f-4059-bc45-aba52b975c13"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface INyWMCapture
    {
        [PreserveSig]
        int Initialize(UInt32 i_device_id,UInt32 i_subtype_id,UInt32 i_capture_category_id);
        [PreserveSig]
        int Finalize();
        [PreserveSig]
        int Start();
        [PreserveSig]
        int Stop();
        [PreserveSig]
        int SetCallBack(INySampleCB i_callback);
        [PreserveSig]
        int SetSize(Int32 i_width, Int32 i_height);
        [PreserveSig]
        int SetSubType(UInt32 i_subtype_id);

    }

    [ComImport,
    Guid("9665C35C-8F46-48e0-A99D-61FFABB19607"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface INySampleCB
    {
        [PreserveSig]
        int OnSample(INySample i_sample);
    };

    [ComImport,
    Guid("88468260-109A-4bb1-AED7-5BD39CFEBB7C"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface INySample
    {
        [PreserveSig]
        IntPtr GetData();
        [PreserveSig]
        Int32 GetDataSize();
        [PreserveSig]
        int CopyToBuffer(IntPtr o_data, Int32 i_start, Int32 i_length);
    };
}
