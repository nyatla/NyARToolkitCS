/* 
 * PROJECT: NyARToolkitCS(Extension)
 * --------------------------------------------------------------------------------
 * The NyARToolkitCS is C# edition ARToolKit class library.
 * Copyright (C)2008-2009 Ryo Iizuka
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * For further information please contact.
 *	http://nyatla.jp/nyatoolkit/
 *	<airmail(at)ebony.plala.or.jp> or <nyatla(at)nyatla.jp>
 * 
 */
using System;
using System.Diagnostics;
using jp.nyatla.nyartoolkit.cs.core;

namespace jp.nyatla.nyartoolkit.cs.rpf

{
    /**
     * NyARRealityクラスの入力コンテナです。
     * NyARRealityへ入力する情報セットを定義します。
     * 
     * このクラスは、元画像、元画像に対するPerspectiveReader,元画像からのSampleOutを提供します。
     * </ul>
     */
    public abstract class NyARRealitySource
    {
	    /**
	     * RealitySourceの主ラスタ。継承先のコンストラクタで実体を割り当ててください。
	     */
	    protected INyARRgbRaster _rgb_source;
	    /**
	     * RealitySourceの主ラスタにリンクしたPerspectiveReader。継承先のコンストラクタで実体を割り当ててください。
	     */
	    protected INyARPerspectiveCopy _source_perspective_reader;


	    /**
	     * TrackerSorceのホルダ。継承先のコンストラクタで実体を割り当ててください。
	     */
	    protected NyARTrackerSource _tracksource;

    	
	    protected NyARRealitySource(){}
    	

	    /**
	     * このRealitySourceに対する読出し準備ができているかを返します。
	     * @return
	     * trueならば、{@link #makeTrackSource}が実行可能。
	     */
        public abstract bool isReady();	
	    /**
	     * 現在のRGBラスタを{@link NyARTrackerSource}の基本ラスタに書込み、その参照値を返します。
	     * この関数は、{@link NyARReality#progress}が呼び出します。
	     * この関数は、{@link NyARTrackerSource}内の基本ラスタに書き込みを行うだけで、その内容を同期しません。
	     * 継承クラスでは、{@link #_tracksource}の基本GS画像を、{@link #_rgb_source}の内容で更新する実装をしてください。
	     * @throws NyARException 
	     */
	    public abstract NyARTrackerSource makeTrackSource();
	    /**
	     * 現在のRGBラスタを{@link NyARTrackerSource}の基本ラスタに書込み、{@link NyARTrackerSource}も含めて同期します。
	     * 通常、この関数は使用することはありません。デバックなどで、{@link NyARReality#progress}以外の方法でインスタンスの同期を行いたいときに使用してください。
	     * 継承クラスでは、{@link #_tracksource}の基本GS画像を、{@link #_rgb_source}の内容で更新してから、この関数を呼び出して同期する処理を実装をしてください。
	     * @throws NyARException 
	　    */
	    public virtual void syncResource()
	    {
		    //下位の同期
		    this._tracksource.syncResource();
	    }
	    /**
	     * {@link #_rgb_source}を参照するPerspectiveRasterReaderを返します。
	     * @return
	     */
        public INyARPerspectiveCopy refPerspectiveRasterReader()
	    {
		    return this._source_perspective_reader;
	    }
    	
	    /**
	     * 元画像への参照値を返します。
	     * @return
	     */
	    public INyARRgbRaster refRgbSource()
	    {
		    return this._rgb_source;
	    }
	    /**
	     * 最後に作成したTrackSourceへのポインタを返します。
	     * この関数は、{@link NyARReality#progress}、または{@link #syncResource}の後に呼び出すことを想定しています。
	     * それ以外のタイミングでは、返却値の内容が同期していないことがあるので注意してください。
	     * @return
	     */
        public NyARTrackerSource refLastTrackSource()
	    {
		    return this._tracksource;
	    }
    }
}

