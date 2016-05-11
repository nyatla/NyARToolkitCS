using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jp.nyatla.nyartoolkit.cs.core;
using System.Diagnostics;
namespace Test_Kpmbenchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            String img_file = "../../../../../data/testcase/test.raw";
            String cparam_file = "../../../../../data/testcase/camera_para5.dat";
            String fset3file = "../../../../../data/testcase/pinball.fset3";
			//カメラパラメータ
			NyARParam param=NyARParam.loadFromARParamFile(File.OpenRead(cparam_file),640,480,NyARParam.DISTFACTOR_LT_ARTK5);
			
			INyARGrayscaleRaster gs=NyARGrayscaleRaster.createInstance(640,480);
			//試験画像の準備
			{
				INyARRgbRaster rgb=NyARRgbRaster.createInstance(640,480,NyARBufferType.BYTE1D_B8G8R8X8_32);
				Stream fs = File.OpenRead(img_file);
                byte[] b=(byte[])rgb.getBuffer();
				fs.Read(b,0,b.Length);
				INyARRgb2GsFilterRgbAve filter=(INyARRgb2GsFilterRgbAve) rgb.createInterface(typeof(INyARRgb2GsFilterRgbAve));
				filter.convert(gs);				
			}
			NyARDoubleMatrix44 tmat=new NyARDoubleMatrix44();
			NyARNftFreakFsetFile f = NyARNftFreakFsetFile.loadFromfset3File(File.OpenRead(fset3file));
//			KpmHandle kpm=new KpmHandle(new ARParamLT(param));
            Stopwatch sw=new Stopwatch();
            FreakKeypointMatching kpm=new FreakKeypointMatching(param);
			KeyframeMap keymap=new KeyframeMap(f,0);
			for(int j=0;j<4;j++){
				sw.Reset();
                sw.Start();
			    for(int i=0;i<20;i++){
				    kpm.updateInputImage(gs);
				    kpm.updateFeatureSet();
				    kpm.kpmMatching(keymap,tmat);
			    }
			    //FreakKeypointMatching#kMaxNumFeaturesを300にしてテストして。
                sw.Stop();
			    System.Console.WriteLine("Total="+(sw.ElapsedMilliseconds));
                NyARDoubleMatrix44 TEST_PATT = new NyARDoubleMatrix44(new double[]{
                    0.98436354107742652,0.0066768917838370646,-0.17602226595996517,-191.17967199668533,
					    0.011597578022657571,-0.99956974712564306,0.026940987645082352,63.00280574839347,
					    -0.17576664981496215,-0.028561157958401542,-0.98401745160789567	,611.75871553558636,
					    0,0,0,1});
                System.Console.WriteLine(TEST_PATT.Equals(tmat));
			    }
        }
    }
}
