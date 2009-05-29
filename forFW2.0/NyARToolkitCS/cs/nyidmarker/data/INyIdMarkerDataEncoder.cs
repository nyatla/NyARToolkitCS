using System;
using System.Collections.Generic;
using System.Text;

namespace jp.nyatla.nyartoolkit.cs.nyidmarker
{
    public interface INyIdMarkerDataEncoder
    {
        bool encode(NyIdMarkerPattern i_data, INyIdMarkerData o_dest);
        INyIdMarkerData createDataInstance();
    }
}
