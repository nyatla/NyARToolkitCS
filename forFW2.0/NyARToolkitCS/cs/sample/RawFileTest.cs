/* 
 * PROJECT: NyARToolkitCS
 * --------------------------------------------------------------------------------
 *
 * The NyARToolkitCS is C# version NyARToolkit class library.
 * 
 * Copyright (C)2008 R.Iizuka
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this framework; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 * 
 * For further information please contact.
 *	http://nyatla.jp/nyatoolkit/
 *	<airmail(at)ebony.plala.or.jp>
 * 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using jp.nyatla.nyartoolkit.cs;
using jp.nyatla.nyartoolkit.cs.core;
using jp.nyatla.nyartoolkit.cs.match;
using jp.nyatla.nyartoolkit.cs.raster;
using jp.nyatla.nyartoolkit.cs.detector;


namespace jp.nyatla.nyartoolkit.cs.sample
{
    /**
     * 320x240のBGRA32で記録されたRAWイメージから、１種類のパターンを認識し、
     * その変換行列を1000回求め、それにかかったミリ秒時間を表示します。
     *
     */
    public class RawFileTest {
        private const String code_file  ="../../../../data/patt.hiro";
        private const String data_file = "../../../../data/320x240ABGR.raw";
        private const String camera_file = "../../../../data/camera_para.dat";
        public RawFileTest()
        {
        }
        public void Test_arDetectMarkerLite()
        {
	        //AR用カメラパラメタファイルをロード
	        NyARParam ap	=new NyARParam();
	        ap.loadFromARFile(camera_file);
	        ap.changeSize(320,240);

	        //AR用のパターンコードを読み出し	
	        NyARCode code=new NyARCode(16,16);
	        code.loadFromARFile(code_file);

	        //試験イメージの読み出し(320x240 BGRAのRAWデータ)
            StreamReader sr = new StreamReader(data_file);
            BinaryReader bs = new BinaryReader(sr.BaseStream);
            byte[] raw = bs.ReadBytes(320*240*4);
            NyARRaster_BGRA ra = NyARRaster_BGRA.wrap(raw, 320, 240);
	        //		Blank_Raster ra=new Blank_Raster(320, 240);

	        //１パターンのみを追跡するクラスを作成
	        NyARSingleDetectMarker ar=new NyARSingleDetectMarker(ap,code,80.0);
	        NyARTransMatResult result_mat=new NyARTransMatResult();
	        ar.setContinueMode(false);
	        ar.detectMarkerLite(ra,100);
	        ar.getTransmationMatrix(result_mat);

	        //マーカーを検出
            Stopwatch sw = new Stopwatch();
            sw.Start();
	        for(int i=0;i<1000;i++){
	            //変換行列を取得
	            ar.detectMarkerLite(ra,100);
	            ar.getTransmationMatrix(result_mat);
	        }
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds + "[ms]"); 
        }
        public static void main(String[] args)
        {
            try{
                RawFileTest t=new RawFileTest();
                //t.Test_arGetVersion();
                t.Test_arDetectMarkerLite();
            }catch(Exception e){
                Console.WriteLine(e.ToString());
            }
        }
    }
}
