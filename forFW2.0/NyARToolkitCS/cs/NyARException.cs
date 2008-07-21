using System;
using System.Collections.Generic;

namespace jp.nyatla.nyartoolkit.cs
{
    public class NyARException : Exception {
	    public NyARException():base()
        {
	    }
        public NyARException(Exception e):base("",e)
        {
	    }
	    public NyARException(String m):base(m)
        {
	    }
	    public static void trap(String m)
	    {
	        throw new NyARException("トラップ:"+m);
	    }
    }
}
