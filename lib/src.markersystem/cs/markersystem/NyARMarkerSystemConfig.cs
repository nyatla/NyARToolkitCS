/* 
 * PROJECT: NyARToolkitCS
 * --------------------------------------------------------------------------------
 *
 * The NyARToolkitCS is C# edition NyARToolKit class library.
 * Copyright (C)2008-2012 Ryo Iizuka
 *
 * This work is based on the ARToolKit developed by
 *   Hirokazu Kato
 *   Mark Billinghurst
 *   HITLab, University of Washington, Seattle
 * http://www.hitl.washington.edu/artoolkit/
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as publishe
 * by the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * For further information please contact.
 *	http://nyatla.jp/nyatoolkit/
 *	<airmail(at)ebony.plala.or.jp> or <nyatla(at)nyatla.jp>
 * 
 */
using jp.nyatla.nyartoolkit.cs.core;
using System.Diagnostics;
using System.IO;
namespace jp.nyatla.nyartoolkit.cs.markersystem
{
    public class NyARMarkerSystemConfig : INyARMarkerSystemConfig
    {
	    /** ARToolkit v2互換のニュートン法を使った変換行列計算アルゴリズムを選択します。*/
	    public const int TM_ARTKV2=1;
	    /** NyARToolKitの偏微分を使った変換行列アルゴリズムです。*/
	    public const int TM_NYARTK=2;
	    /** ARToolkit v4に搭載されているICPを使った変換行列計算アルゴリズムを選択します。*/
    	public const int TM_ARTKICP=3;
        //
        protected NyARParam _param;
    	private int _transmat_algo_type;
        /**
         * コンストラクタです。カメラパラメータにサンプル値(../Data/camera_para.dat)をロードして、コンフィギュレーションを生成します。
         * @param i_width
         * @param i_height
         * @
         */
        public NyARMarkerSystemConfig(NyARParam i_param, int i_transmat_algo_type)
        {
            Debug.Assert(1 <= i_transmat_algo_type && i_transmat_algo_type <= 3);
            this._param = i_param;
            this._transmat_algo_type = i_transmat_algo_type;
            return;
        }
        public NyARMarkerSystemConfig(NyARParam i_param)
            : this(i_param, TM_ARTKICP)
        {
        }
        public NyARMarkerSystemConfig(StreamReader i_ar_param_stream, int i_width, int i_height)
            : this(NyARParam.createFromARParamFile(i_ar_param_stream))
        {
            this._param.changeScreenSize(i_width, i_height);
        }

        public NyARMarkerSystemConfig(int i_width, int i_height)
            : this(NyARParam.createDefaultParameter())
        {
            this._param.changeScreenSize(i_width, i_height);
        }
        public virtual INyARTransMat createTransmatAlgorism()
        {
            switch (this._transmat_algo_type)
            {
                case TM_ARTKV2:
                    return new NyARTransMat_ARToolKit(this._param);
                case TM_NYARTK:
                    return new NyARTransMat(this._param);
                case TM_ARTKICP:
                    return new NyARIcpTransMat(this._param, NyARIcpTransMat.AL_POINT_ROBUST);
            }
            throw new NyARException();
        }
        public virtual INyARHistogramAnalyzer_Threshold createAutoThresholdArgorism()
        {
            return new NyARHistogramAnalyzer_SlidePTile(15);
        }
        public virtual NyARParam getNyARParam()
        {
            return this._param;
        }
	    /**
	     * この値は、カメラパラメータのスクリーンサイズです。
	     */
	    public NyARIntSize getScreenSize()
	    {
		    return this._param.getScreenSize();
	    }

    }
}