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
         * ImputStreamからARToolKit形式のマーカデータを読み込んでo_raster[4]に格納します。
         * @param i_stream
         * 読出し元のストリームです。
         * @param o_raster
         * 出力先のラスタ配列です。バッファ形式は形式はINT1D_X8R8G8B8_32であり、全て同一なサイズである必要があります。
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
         * ImputStreamからARToolKit形式のマーカデータを読み込んでo_codeに格納します。
         * @param i_stream
         * 読出し元のストリームです。
         * @param o_code
         * 出力先のNyARCodeオブジェクトです。
         * @throws NyARException
         */
        public static void loadFromARToolKitFormFile(StreamReader i_stream, NyARCode o_code)
        {
            int width = o_code.getWidth();
            int height = o_code.getHeight();
            NyARRgbRaster tmp_raster = new NyARRgbRaster(width, height, NyARBufferType.INT1D_X8R8G8B8_32);
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
         * 入力元のStreamTokenizerを指定します。関数実行後、読み取り位置は更新されます。
         * @param i_width
         * パターンの横幅です。
         * @param i_height
         * パターンの縦幅です。
         * @param o_buf
         * 読み取った値を格納する配列です。
         * @throws NyARException
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
     * ARToolKitのマーカーパターン1個のデータに相当するクラスです。
     * マーカーパターンに対する、ARToolKit相当のプロパティ値を提供します。
     */
    public class NyARCode
    {
	    private NyARMatchPattDeviationColorData[] _color_pat=new NyARMatchPattDeviationColorData[4];
	    private NyARMatchPattDeviationBlackWhiteData[] _bw_pat=new NyARMatchPattDeviationBlackWhiteData[4];
	    private int _width;
	    private int _height;
    	
	    /**
	     * directionを指定して、NyARMatchPattDeviationColorDataオブジェクトを取得します。
	     * @param i_index
	     * 0<=n<4の数値
	     * @return
	     * NyARMatchPattDeviationColorDataオブジェクト
	     */
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

	    /**
	     * パターンデータの高さを取得します。
	     * @return
	     */
	    public int getHeight()
	    {
		    return _height;
	    }
	    /**
	     * コンストラクタです。空のNyARCodeオブジェクトを作成します。
	     * @param i_width
	     * 作成するマーカパターンの幅
	     * @param i_height
	     * 作成するマーカパターンの高さ
	     * @throws NyARException
	     */
	    public NyARCode(int i_width, int i_height)
	    {
		    this._width = i_width;
		    this._height = i_height;
		    //空のラスタを4個作成
		    for(int i=0;i<4;i++){
			    this._color_pat[i]=new NyARMatchPattDeviationColorData(i_width,i_height);
			    this._bw_pat[i]=new NyARMatchPattDeviationBlackWhiteData(i_width,i_height);
		    }
		    return;
	    }
	    /**
	     * ファイル名を指定して、パターンデータをロードします。
	     * ロードするパターンデータのサイズは、現在の値と同じである必要があります。
	     * @param filename
	     * ARToolKit形式のパターンデータファイルの名前
	     * @throws NyARException
	     */
	    public void loadARPattFromFile(String filename)
	    {
		    try {
			    loadARPatt(new StreamReader(filename));
		    } catch (Exception e) {
			    throw new NyARException(e);
		    }
		    return;
	    }
	    /**
	     * 4枚のラスタから、マーカーパターンを生成して格納します。
	     * @param i_raster
	     * direction毎のパターンを格納したラスタ配列を指定します。
	     * ラスタは同一なサイズ、かつマーカーパターンと同じサイズである必要があります。
	     * 格納順は、パターンの右上が、1,2,3,4象限になる順番です。
	     * @throws NyARException
	     */
	    public void setRaster(INyARRgbRaster[] i_raster)
	    {
            Debug.Assert(i_raster.Length != 4);
		    //ラスタにパターンをロードする。
		    for(int i=0;i<4;i++){
			    this._color_pat[i].setRaster(i_raster[i]);
		    }
		    return;
	    }
	    /**
	     * 1枚のラスタから、マーカーパターンを生成して格納します。
	     * @param i_raster
	     * 基準となるラスタを指定します。ラスタの解像度は、ARマーカコードと同じである必要があります。
	     * @throws NyARException
	     */	
	    public void setRaster(INyARRgbRaster i_raster)
	    {
		    //ラスタにパターンをロードする。
		    for(int i=0;i<4;i++){
			    this._color_pat[i].setRaster(i_raster,i);
		    }
		    return;
	    }
    	
	    /**
	     * inputStreamから、パターンデータをロードします。
	     * ロードするパターンのサイズは、現在の値と同じである必要があります。
	     * @param i_stream
	     * @throws NyARException
	     */
        public void loadARPatt(StreamReader i_stream)
	    {
		    //ラスタにパターンをロードする。
		    NyARCodeFileReader.loadFromARToolKitFormFile(i_stream,this);
		    return;
	    }
    }


}