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
using System.Runtime.InteropServices;
using jp.nyatla.nyartoolkit.cs.core;
using jp.nyatla.nyartoolkit.cs.cs4;
using System.IO;
using DirectShowLib;
using System.Drawing.Imaging;
using System.Drawing;



namespace NyARToolkitCSUtils.Capture
{
    public class DsRgbRaster : NyARBitmapRaster
    {
        #region APIs
        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory")]
        private static extern void CopyMemory(IntPtr Destination, IntPtr Source, [MarshalAs(UnmanagedType.U4)] int Length);
        #endregion
        public DsRgbRaster(int i_width, int i_height)
            : base(i_width, i_height)
        {
        }
        public void setBuffer(IntPtr i_buf,int i_buf_size, bool i_flip_vertical)
        {

            BitmapData bm=this.lockBitmap();
            try
            {
                if (i_flip_vertical)
                {
                    //上下反転させる
                    int w = this._size.w * 4;
                    long st = (long)i_buf + w * (this._size.h - 1);
                    long et = (long)bm.Scan0;
                    for (int i = this._size.h - 1; i >= 0; i--)
                    {
                        CopyMemory((IntPtr)et, (IntPtr)st, w);
                        st -= w;
                        et += bm.Stride;
                    }
                }
                else
                {
                    if (bm.Width * 4 == bm.Stride)
                    {
                        //そのままコピー
                        CopyMemory(bm.Scan0, i_buf, bm.Height * bm.Width * 4);
                    }
                    else
                    {
                        for (int i = this._size.h - 1; i >= 0; i--)
                        {
                            CopyMemory((IntPtr)((int)bm.Scan0 + bm.Stride * i), (IntPtr)((int)bm.Scan0 + bm.Width * i), bm.Width * 4);
                        }
                    }
                }
            }
            finally
            {
                this.unlockBitmap();
            }
            return;
        }
    }
}
