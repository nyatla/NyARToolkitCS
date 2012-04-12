/* 
 * PROJECT: NyARToolkitCS(Extension)
 * --------------------------------------------------------------------------------
 * The NyARToolkitCS is Java edition ARToolKit class library.
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
namespace jp.nyatla.nyartoolkit.cs.core
{

    /**
     * このクラスは、0/ 255 の二値GrayscaleRasterです。
     */
    public class NyARBinRaster : NyARGrayscaleRaster
    {
        /**
         * コンストラクタです。
         * 画像のサイズパラメータを指定して、{@link NyARBufferType#INT2D_BIN_8}形式のバッファを持つインスタンスを生成します。
         * このラスタは、内部参照バッファを持ちます。
         * @param i_width
         * ラスタのサイズ
         * @param i_height
         * ラスタのサイズ
         * @
         */
        public NyARBinRaster(int i_width, int i_height)
            : base(i_width, i_height, NyARBufferType.INT1D_BIN_8, true)
        {
        }
        /*
         * この関数は、インスタンスの初期化シーケンスを実装します。
         * コンストラクタから呼び出します。
         * @param i_size
         * ラスタのサイズ
         * @param i_buf_type
         * バッファ形式定数
         * @param i_is_alloc
         * 内部バッファ/外部バッファのフラグ
         * @return
         * 初期化に成功するとtrue
         * @ 
         */
        protected override void initInstance(NyARIntSize i_size, int i_buf_type, bool i_is_alloc)
        {
            switch (i_buf_type)
            {
                case NyARBufferType.INT1D_BIN_8:
                    this._buf = i_is_alloc ? new int[i_size.w * i_size.h] : null;
                    break;
                default:
                    base.initInstance(i_size, i_buf_type, i_is_alloc);
                    return;
            }
            this._pixdrv = NyARGsPixelDriverFactory.createDriver(this);
            this._is_attached_buffer = i_is_alloc;
            return;
        }
        public override object createInterface(Type i_iid)
        {
            if (i_iid == typeof(NyARLabeling_Rle.IRasterDriver))
            {
                return NyARLabeling_Rle.RasterDriverFactory.createDriver(this);
            }
            if (i_iid == typeof(NyARContourPickup.IRasterDriver))
            {
                return NyARContourPickup.ImageDriverFactory.createDriver(this);
            }
            throw new NyARException();
        }
    }
}