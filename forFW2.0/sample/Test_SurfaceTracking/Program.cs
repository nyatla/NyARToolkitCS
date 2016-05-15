using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jp.nyatla.nyartoolkit.cs.core;


namespace Test_SurfaceTracking
{
    class Program
    {
        static void Main(string[] args)
        {
           NyARDoubleMatrix44 DEST_MAT=new NyARDoubleMatrix44(
                    new double[]{
                            0.9832165682361184,0.004789697223621061,-0.18237945710280384,-190.59060790299358,
                            0.012860184615056927,-0.9989882709616935,0.04309419210331572,64.04490277502563,
                            -0.18198852802987958,-0.044716355753573425,-0.9822833548209547,616.6427596804766,
                    0,0,0,1});
            NyARDoubleMatrix44 SRC_MAT=new NyARDoubleMatrix44(new double[]{
                0.984363556,	0.00667689135,	-0.176022261,	-191.179672,
                0.0115975942,	-0.999569774,	0.0269410834,	63.0028076,
                -0.175766647,	-0.0285612550,	-0.984017432,	611.758728,
                0,0,0,1});

            String img_file = "../../../../../data/testcase/test.raw";
            String cparam = "../../../../../data/testcase/camera_para5.dat";
            String fsetfile = "../../../../../data/testcase/pinball.fset";
            String isetfile = "../../../../../data/testcase/pinball.iset5";
            //カメラパラメータ
            NyARParam param=NyARParam.loadFromARParamFile(File.OpenRead(cparam),640,480,NyARParam.DISTFACTOR_LT_ARTK5);


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

            NyARNftFsetFile fset=NyARNftFsetFile.loadFromFsetFile(File.OpenRead(fsetfile));
            NyARNftIsetFile iset=NyARNftIsetFile.loadFromIsetFile(File.OpenRead(isetfile));
            NyARSurfaceTracker st=new NyARSurfaceTracker(param,16,0.5);
            NyARSurfaceDataSet sd=new NyARSurfaceDataSet(iset,fset);
            NyARDoubleMatrix44 sret=new NyARDoubleMatrix44();
            NyARDoublePoint2d[] o_pos2d=NyARDoublePoint2d.createArray(16);
            NyARDoublePoint3d[] o_pos3d=NyARDoublePoint3d.createArray(16);
            NyARSurfaceTrackingTransmatUtils tmat=new NyARSurfaceTrackingTransmatUtils(param,5.0);
            NyARDoubleMatrix44 tret=new NyARDoubleMatrix44();
            for(int j=0;j<10;j++){
                Stopwatch s=new Stopwatch();
                s.Reset();
                s.Start();
                for(int i=0;i<3000;i++){
                    sret.setValue(SRC_MAT);
                    int nop=st.tracking(gs, sd,sret, o_pos2d, o_pos3d,16);
                    //Transmatの試験
                    NyARDoublePoint3d off=NyARSurfaceTrackingTransmatUtils.centerOffset(o_pos3d,nop,new NyARDoublePoint3d());
                    NyARSurfaceTrackingTransmatUtils.modifyInputOffset(sret, o_pos3d,nop,off);
                    tmat.surfaceTrackingTransmat(sret, o_pos2d, o_pos3d, nop,tret,new NyARTransMatResultParam());
                    NyARSurfaceTrackingTransmatUtils.restoreOutputOffset(tret,off);
                    System.Console.WriteLine(tret.Equals(DEST_MAT));
                }
                s.Stop();
                System.Console.WriteLine(s.ElapsedMilliseconds);
            }
            return;
        }
    }
}
