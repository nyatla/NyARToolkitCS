/* 
 * PROJECT: Capture Test NyARToolkitCSサンプルプログラム
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
using System.IO;
using System.Diagnostics;
using jp.nyatla.nyartoolkit.cs;
using jp.nyatla.nyartoolkit.cs.core;
using jp.nyatla.nyartoolkit.cs.nyidmarker;
using jp.nyatla.nyartoolkit.cs.processor;

namespace ConsoleApplication1
{

    /**
     * 320x240のBGRA32で記録されたIdmarkerを撮影したRAWイメージから、
     * Idマーカを認識します。
     *
     */
    public class NyIdTest
    {
        public class MarkerProcessor : SingleNyIdMarkerProcesser
        {
            public NyARDoubleMatrix44 transmat = null;
            public int current_id = -1;

            public MarkerProcessor(NyARParam i_cparam, int i_raster_format)
            {
                initInstance(i_cparam, new NyIdMarkerDataEncoder_RawBit(), 100);

                //アプリケーションフレームワークの初期化
                return;
            }
            /**
             * アプリケーションフレームワークのハンドラ（マーカ出現）
             */
            protected override void onEnterHandler(INyIdMarkerData i_code)
            {
                NyIdMarkerData_RawBit code = (NyIdMarkerData_RawBit)i_code;
                if (code.length > 4)
                {
                    //4バイト以上の時はint変換しない。
                    this.current_id = -1;//undefined_id
                }
                else
                {
                    this.current_id = 0;
                    //最大4バイト繋げて１個のint値に変換
                    for (int i = 0; i < code.length; i++)
                    {
                        this.current_id = (this.current_id << 8) | code.packet[i];
                    }
                }
                this.transmat = null;
            }
            /**
             * アプリケーションフレームワークのハンドラ（マーカ消滅）
             */
            protected override void onLeaveHandler()
            {
                this.current_id = -1;
                this.transmat = null;
                return;
            }
            /**
             * アプリケーションフレームワークのハンドラ（マーカ更新）
             */
            protected override void onUpdateHandler(NyARSquare i_square, NyARDoubleMatrix44 result)
            {
                this.transmat = result;
            }
        }
        private const String data_file = "../../../../../data/320x240NyId.raw";
        private const String camera_file = "../../../../../data/camera_para.dat";
        public NyIdTest()
        {
        }
        public void Test()
        {
            //AR用カメラパラメタファイルをロード
            NyARParam ap = NyARParam.loadFromARParamFile(File.OpenRead(camera_file),320,240);

            //試験イメージの読み出し(320x240 RGBのRAWデータ)
            StreamReader sr = new StreamReader(data_file);
            BinaryReader bs = new BinaryReader(sr.BaseStream);
            byte[] raw = bs.ReadBytes(320 * 240 * 3);
            INyARRgbRaster ra = NyARRgbRaster.createInstance(320, 240,NyARBufferType.BYTE1D_R8G8B8_24,false);
            ra.wrapBuffer(raw);

            MarkerProcessor pr = new MarkerProcessor(ap, ra.getBufferType());
            pr.detectMarker(ra);
            Console.WriteLine(pr.transmat.m00 + "," + pr.transmat.m01 + "," + pr.transmat.m02 + "," + pr.transmat.m03);
            Console.WriteLine(pr.transmat.m10 + "," + pr.transmat.m11 + "," + pr.transmat.m12 + "," + pr.transmat.m13);
            Console.WriteLine(pr.transmat.m20 + "," + pr.transmat.m21 + "," + pr.transmat.m22 + "," + pr.transmat.m23);
            Console.WriteLine(pr.transmat.m30 + "," + pr.transmat.m31 + "," + pr.transmat.m32 + "," + pr.transmat.m33);
            Console.WriteLine(pr.current_id);
            return;
        }
    }
}
