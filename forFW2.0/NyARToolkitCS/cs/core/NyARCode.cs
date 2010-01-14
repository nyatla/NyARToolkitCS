/* 
 * PROJECT: NyARToolkitCS
 * --------------------------------------------------------------------------------
 * This work is based on the original ARToolKit developed by
 *   Hirokazu Kato
 *   Mark Billinghurst
 *   HITLab, University of Washington, Seattle
 * http://www.hitl.washington.edu/artoolkit/
 *
 * The NyARToolkitCS is C# edition ARToolKit class library.
 * Copyright (C)2008-2009 Ryo Iizuka
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * For further information please contact.
 *	http://nyatla.jp/nyatoolkit/
 *	<airmail(at)ebony.plala.or.jp> or <nyatla(at)nyatla.jp>
 * 
 */
using System.IO;
using System;
using System.Diagnostics;
namespace jp.nyatla.nyartoolkit.cs.core
{
    class NyARCodeFileReader
    {

        /**
         * ARコードファイルからデータを読み込んでo_raster[4]に格納します。
         * @param i_stream
         * @param o_raster
         * @throws NyARException
         */
        public static void loadFromARToolKitFormFile(StreamReader i_stream, NyARRaster[] o_raster)
        {
            Debug.Assert(o_raster.Length == 4);
            //4個の要素をラスタにセットする。
            try
            {
                string[] data = i_stream.ReadToEnd().Split(new Char[] { ' ', '\r', '\n' });
                //GBRAで一度読みだす。
                int idx = 0;
                for (int h = 0; h < 4; h++)
                {
                    Debug.Assert(o_raster[h].isEqualBufferType(NyARBufferType.INT1D_X8R8G8B8_32));
                    NyARRaster ra = o_raster[h];
                    idx = readBlock(data, idx, ra.getWidth(), ra.getHeight(), (int[])ra.getBuffer());
                }
            }
            catch (Exception e)
            {
                throw new NyARException(e);
            }
            return;
        }
        /**
         * ARコードファイルからデータを読み込んでo_codeに格納します。
         * @param i_stream
         * @param o_code
         * @throws NyARException
         */
        public static void loadFromARToolKitFormFile(StreamReader i_stream, NyARCode o_code)
        {
            int width = o_code.getWidth();
            int height = o_code.getHeight();
            NyARRaster tmp_raster = new NyARRaster(width, height, NyARBufferType.INT1D_X8R8G8B8_32);
            //4個の要素をラスタにセットする。
            try
            {
                int[] buf = (int[])tmp_raster.getBuffer();
                string[] data = i_stream.ReadToEnd().Split(new Char[] { ' ', '\r', '\n' });
                //GBRAで一度読みだす。
                int idx=0;
                for (int h = 0; h < 4; h++)
                {
                    idx=readBlock(data,idx, width, height, buf);
                    //ARCodeにセット(カラー)
                    o_code.getColorData(h).setRaster(tmp_raster);
                    o_code.getBlackWhiteData(h).setRaster(tmp_raster);
                }
            }
            catch (Exception e)
            {
                throw new NyARException(e);
            }
            tmp_raster = null;//ポイ
            return;
        }
        /**
         * 1ブロック分のXRGBデータをi_stからo_bufへ読みだします。
         * @param i_st
         * @param o_buf
         */
        private static int readBlock(string[] i_data, int i_idx, int i_width, int i_height, int[] o_buf)
        {
            int idx = i_idx;
            try
            {
                int pixels = i_width * i_height;
                for (int i3 = 0; i3 < 3; i3++)
                {
                    for (int i2 = 0; i2 < pixels; i2++)
                    {
                        //数値のみ読み出す(空文字は読み飛ばし！)
                        for (; ; )
                        {
                            if (i_data[idx].Length > 0)
                            {
                                break;
                            }
                            idx++;
                        }
                        o_buf[i2] = (o_buf[i2] << 8) | ((0x000000ff & (int)int.Parse(i_data[idx])));
                        idx++;
                    }
                }
                //GBR→RGB
                for (int i3 = 0; i3 < pixels; i3++)
                {
                    o_buf[i3] = ((o_buf[i3] << 16) & 0xff0000) | (o_buf[i3] & 0x00ff00) | ((o_buf[i3] >> 16) & 0x0000ff);
                }
            }
            catch (Exception e)
            {
                throw new NyARException(e);
            }
            return idx;
        }
    }

    /**
     * ARToolKitのマーカーコードを1個保持します。
     * 
     */
    public class NyARCode
    {
        private NyARMatchPattDeviationColorData[] _color_pat = new NyARMatchPattDeviationColorData[4];
        private NyARMatchPattDeviationBlackWhiteData[] _bw_pat = new NyARMatchPattDeviationBlackWhiteData[4];
        private int _width;
        private int _height;

        public NyARMatchPattDeviationColorData getColorData(int i_index)
        {
            return this._color_pat[i_index];
        }
        public NyARMatchPattDeviationBlackWhiteData getBlackWhiteData(int i_index)
        {
            return this._bw_pat[i_index];
        }
        public int getWidth()
        {
            return _width;
        }

        public int getHeight()
        {
            return _height;
        }
        public NyARCode(int i_width, int i_height)
        {
            this._width = i_width;
            this._height = i_height;
            //空のラスタを4個作成
            for (int i = 0; i < 4; i++)
            {
                this._color_pat[i] = new NyARMatchPattDeviationColorData(i_width, i_height);
                this._bw_pat[i] = new NyARMatchPattDeviationBlackWhiteData(i_width, i_height);
            }
            return;
        }
        public void loadARPattFromFile(String filename)
        {
            try
            {
                loadARPatt(new StreamReader(filename));
            }
            catch (Exception e)
            {
                throw new NyARException(e);
            }
            return;
        }
        public void setRaster(NyARRaster[] i_raster)
        {
            Debug.Assert(i_raster.Length != 4);
            //ラスタにパターンをロードする。
            for (int i = 0; i < 4; i++)
            {
                this._color_pat[i].setRaster(i_raster[i]);
            }
            return;
        }

        public void loadARPatt(StreamReader i_reader)
        {
            //ラスタにパターンをロードする。
            NyARCodeFileReader.loadFromARToolKitFormFile(i_reader, this);
            return;
        }
        public void loadARPatt(Stream i_stream)
        {
            //ラスタにパターンをロードする。
            NyARCodeFileReader.loadFromARToolKitFormFile(new StreamReader(i_stream), this);
            return;
        }

    }
}