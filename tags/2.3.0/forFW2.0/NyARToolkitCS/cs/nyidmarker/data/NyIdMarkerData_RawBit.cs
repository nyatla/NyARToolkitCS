using System;
using System.Collections.Generic;
using System.Text;

namespace jp.nyatla.nyartoolkit.cs.nyidmarker
{
    /**
     * [[Strage class]]
     *
     */
    public class NyIdMarkerData_RawBit : INyIdMarkerData
    {
        public int[] packet = new int[22];
        public int length;
        public bool isEqual(INyIdMarkerData i_target)
        {
            NyIdMarkerData_RawBit s = (NyIdMarkerData_RawBit)i_target;
            if (s.length != this.length)
            {
                return false;
            }
            for (int i = s.length - 1; i >= 0; i--)
            {
                if (s.packet[i] != s.packet[i])
                {
                    return false;
                }
            }
            return true;
        }
        public void copyFrom(INyIdMarkerData i_source)
        {
            NyIdMarkerData_RawBit s = (NyIdMarkerData_RawBit)i_source;
            System.Array.Copy(s.packet, 0, this.packet, 0, s.length);
            this.length = s.length;
            return;
        }
    }
}
