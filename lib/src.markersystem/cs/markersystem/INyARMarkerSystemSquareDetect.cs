using jp.nyatla.nyartoolkit.cs.core;
namespace jp.nyatla.nyartoolkit.cs.markersystem
{
    public interface INyARMarkerSystemSquareDetect
    {
        void detectMarkerCb(NyARSensor i_sensor, int i_th, NyARSquareContourDetector.CbHandler i_handler);

    }
}