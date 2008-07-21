/*
 * NyARToolkitCSUtils NyARToolkit for C# 支援ライブラリ
 * 
 * (c)2008 A虎＠nyatla.jp
 * airmail(at)ebony.plala.or.jp
 * http://nyatla.jp/
 */
using System;
using System.Runtime.InteropServices;
using jp.nyatla.nyartoolkit.cs.raster;
using System.IO;

namespace NyARToolkitCSUtils.Raster
{

    /*
     * DirectShowから受け取った、XRGB32ラスタデータを保持するNyARRasterです。
     */
    public class DsRGB565Raster : NyARRaster
    {
        private bool m_vertical_turn;
        private int m_stride;
        private int m_width;
        private int m_height;
        private byte[] m_rgb_buf;
        public byte[] buffer
        {
            get { return this.m_rgb_buf; }
        }
        public DsRGB565Raster(int i_width, int i_height,bool i_vertical_turn)
        {
            this.m_width = i_width;
            this.m_height = i_height;
            this.m_stride = i_width*2;
            this.m_vertical_turn = i_vertical_turn;
            this.m_rgb_buf = new byte[i_height * this.m_stride];
        }
        //RGBの合計値を返す
        public int getPixelTotal(int i_x, int i_y)
        {
            int y = this.m_vertical_turn ? this.m_height - i_y : i_y;
            int idx=y * this.m_stride + i_x * 2;
            uint pixcel = (uint)(this.m_rgb_buf[idx+1] << 8) | (uint)this.m_rgb_buf[idx];
            return (int)((pixcel & 0xf800)>>8) + (int)((pixcel & 0x07e0)>>3) + (int)((pixcel & 0x001f)<<3);
        }
        public void getPixelTotalRowLine(int i_row, int[] o_line)
        {
            int row_idx = (this.m_vertical_turn ? this.m_height - i_row : i_row) * this.m_stride;
            for (int i = this.m_width - 1; i >= 0; i--)
            {
                int idx = row_idx + i * 2;
                uint pixcel = (uint)(this.m_rgb_buf[idx + 1] << 8) | (uint)this.m_rgb_buf[idx];
                o_line[i] = (int)((pixcel & 0xf800) >> 8) + (int)((pixcel & 0x07e0) >> 3) + (int)((pixcel & 0x001f) << 3);
            }
        }
        public void setBuffer(IntPtr i_buf)
        {
            Marshal.Copy(i_buf,this.m_rgb_buf, 0, this.m_rgb_buf.Length);
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
            int y = this.m_vertical_turn ? this.m_height - i_y : i_y;
            int idx = y * this.m_stride + i_x * 2;
            uint pixcel = (uint)(this.m_rgb_buf[idx+1] << 8) | (uint)this.m_rgb_buf[idx];

            i_rgb[0] = (int)((pixcel & 0xf800) >> 8);//R
            i_rgb[1] = (int)((pixcel & 0x07e0) >> 3);//G
            i_rgb[2] = (int)((pixcel & 0x001f) << 3);//B
        }
        public void getPixelSet(int[] i_x, int[] i_y, int i_num, int[] o_rgb)
        {
            int s = this.m_stride;
            byte[] rgb = this.m_rgb_buf;
            if (this.m_vertical_turn)
            {
                for (int i = i_num - 1; i >= 0; i--)
                {
                    int idx = (this.m_height - i_y[i]) * this.m_stride + i_x[i] * 2;
                    uint pixcel = (uint)(this.m_rgb_buf[idx + 1] << 8) | (uint)this.m_rgb_buf[idx];
                    o_rgb[i * 3 + 0] = (int)((pixcel & 0xf800) >> 8);//R
                    o_rgb[i * 3 + 1] = (int)((pixcel & 0x07e0) >> 3);//G
                    o_rgb[i * 3 + 2] = (int)((pixcel & 0x001f) << 3);//B
                }
            }else{
                for (int i = i_num - 1; i >= 0; i--)
                {
                    int idx = i_y[i] * this.m_stride + i_x[i] * 2;

                    uint pixcel = (uint)(this.m_rgb_buf[idx + 1] << 8) | (uint)this.m_rgb_buf[idx];
                    o_rgb[i * 3 + 0] = (int)((pixcel & 0xf800) >> 8);//R
                    o_rgb[i * 3 + 1] = (int)((pixcel & 0x07e0) >> 3);//G
                    o_rgb[i * 3 + 2] = (int)((pixcel & 0x001f) << 3);//B
                }
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
