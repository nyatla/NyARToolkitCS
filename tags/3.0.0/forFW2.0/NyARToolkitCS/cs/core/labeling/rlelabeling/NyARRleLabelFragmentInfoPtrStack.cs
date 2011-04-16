using System;
using System.Collections.Generic;
using System.Text;

namespace jp.nyatla.nyartoolkit.cs.core
{
    public class NyARRleLabelFragmentInfoPtrStack : NyARPointerStack<NyARRleLabelFragmentInfo>
    {
	    public NyARRleLabelFragmentInfoPtrStack(int i_length)
	    {
		    this.initInstance(i_length);
		    return;
	    }

	    /**
	     * エリアの大きい順にラベルをソートします。
	     */
	    public void sortByArea()
	    {
		    int len=this._length;
		    if(len<1){
			    return;
		    }
		    int h = len *13/10;
		    NyARRleLabelFragmentInfo[] item=this._items;
		    for(;;){
		        int swaps = 0;
		        for (int i = 0; i + h < len; i++) {
		            if (item[i + h].area > item[i].area) {
		                NyARRleLabelFragmentInfo temp = item[i + h];
		                item[i + h] = item[i];
		                item[i] = temp;
		                swaps++;
		            }
		        }
		        if (h == 1) {
		            if (swaps == 0){
		        	    break;
		            }
		        }else{
		            h=h*10/13;
		        }
		    }		
	    }	
    }
}
