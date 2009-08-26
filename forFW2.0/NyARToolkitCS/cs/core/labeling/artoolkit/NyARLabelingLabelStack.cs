using System;
using System.Collections.Generic;
using System.Text;

namespace jp.nyatla.nyartoolkit.cs.core
{
    public class NyARLabelingLabelStack : NyARLabelInfoStack<NyARLabelingLabel>
    {
        public NyARLabelingLabelStack(int i_max_array_size)
            : base(i_max_array_size)
        {
        }
        protected override NyARLabelingLabel createElement()
        {
            return new NyARLabelingLabel();
        }
    }
}
