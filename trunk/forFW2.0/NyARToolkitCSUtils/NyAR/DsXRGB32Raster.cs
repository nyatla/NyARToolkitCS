/*
 * NyARToolkitCSUtils NyARToolkit for C# 支援ライブラリ
 * 
 * (c)2008 A虎＠nyatla.jp
 * airmail(at)ebony.plala.or.jp
 * http://nyatla.jp/
 */
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using jp.nyatla.nyartoolkit.cs.raster;
using System.IO;

namespace NyARToolkitCSUtils.Raster
{

    /*
     * DirectShowから受け取った、XRGB32ラスタデータを保持するNyARRasterです。
     */
    public class DsXRGB32Raster : NyARRaster
    {
        private const int INDEX_R = 2;
        private const int INDEX_G = 1;
        private const int INDEX_B = 0;
        private int m_stride;
        private int m_width;
        private int m_height;
        private byte[] m_rgb_buf;
        public byte[] buffer
        {
            get { return this.m_rgb_buf; }
        }
        public DsXRGB32Raster(int i_width, int i_height, int i_stride)
        {
            this.m_width = i_width;
            this.m_height = i_height;
            this.m_stride = i_stride;
            this.m_rgb_buf = new byte[i_height * i_stride];
        }
        //RGBの合計値を返す
        public int getPixelTotal(int i_x, int i_y)
        {
            int idx = i_y * this.m_stride + i_x * 4;
            byte[] rgb = this.m_rgb_buf;
            return (int)rgb[idx + 0] + (int)rgb[idx + 1] + (int)rgb[idx + 2];
        }
        public void getPixelTotalRowLine(int i_row, int[] o_line)
        {
            byte[] rgb = this.m_rgb_buf;
            int bp = i_row * this.m_stride;
            for (int i = this.m_width - 1; i >= 0; i--)
            {
                o_line[i] = (int)rgb[bp + 0] + (int)rgb[bp + 1] + (int)rgb[bp + 2];
                bp -= 4;
            }
        }
        public void setBuffer(IntPtr i_buf)
        {
            Marshal.Copy(i_buf, this.m_rgb_buf, 0, this.m_rgb_buf.Length);
        }
        public int getWidth()
        {
            return this.m_width;
        }
        public int getHeight()
        {
            return this.m_height;
        }
        public void getPixel(int i_x, int i_y, int[] i_rgb)
        {
            int idx = i_y * this.m_stride + i_x * 4;
            byte[] rgb = this.m_rgb_buf;
            i_rgb[0] = rgb[idx + INDEX_R];//R
            i_rgb[1] = rgb[idx + INDEX_G];//G
            i_rgb[2] = rgb[idx + INDEX_B];//B
        }
        public void getPixelSet(int[] i_x, int[] i_y, int i_num, int[] o_rgb)
        {
            int idx;
            int s = this.m_stride;
            byte[] rgb = this.m_rgb_buf;
            for (int i = i_num - 1; i >= 0; i--)
            {
                idx = i_y[i] * s + i_x[i] * 4;
                o_rgb[i * 3 + 0] = rgb[idx + INDEX_R];//R
                o_rgb[i * 3 + 1] = rgb[idx + INDEX_G];//G
                o_rgb[i * 3 + 2] = rgb[idx + INDEX_B];//B
            }
        }
        public void SaveToFile(String i_file_name)
        {
            FileStream fs = new FileStream(i_file_name, FileMode.Create);
            fs.Write(this.m_rgb_buf, 0, this.m_rgb_buf.Length);
            fs.Close();
        }

    }
}
