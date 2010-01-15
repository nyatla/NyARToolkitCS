/* 
 * PROJECT: NyARToolkitCS
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
using System.Diagnostics;
namespace jp.nyatla.nyartoolkit.cs.utils
{
    /**
     * スタック型の可変長配列。
     * 配列には実体を格納します。
     */
    public abstract class NyObjectStack<T>
    {
        protected T[] _items;
        protected int _length;

        /**
         * 最大ARRAY_MAX個の動的割り当てバッファを準備する。
         * 
         * @param i_array
         * @param i_element_type
         * JavaのGenedicsの制限突破
         */
        protected NyObjectStack(int i_length)
        {
            //領域確保
            this._items = new T[i_length];
            for (int i = 0; i < i_length; i++)
            {
                this._items[i] = createElement();
            }
            //使用中個数をリセット
            this._length = 0;
            return;
        }
        protected abstract T createElement();

        /**
         * 新しい領域を予約します。
         * @return
         * 失敗するとnull
         * @throws NyARException
         */
        public T prePush()
        {
            // 必要に応じてアロケート
            if (this._length >= this._items.Length)
            {
                return default(T);
            }
            // 使用領域を+1して、予約した領域を返す。
            T ret = this._items[this._length];
            this._length++;
            return ret;
        }
        /**
         * スタックを初期化します。
         * @param i_reserv_length
         * 使用済みにするサイズ
         * @return
         */
        public void init(int i_reserv_length)
        {
            // 必要に応じてアロケート
            if (i_reserv_length >= this._items.Length)
            {
                throw new NyARException();
            }
            this._length = i_reserv_length;
        }

        /** 
         * 見かけ上の要素数を1減らして、そのオブジェクトを返します。
         * 返却したオブジェクトの内容は、次回のpushまで有効です。
         * @return
         */
        public T pop()
        {
            Debug.Assert(this._length >= 1);
            this._length--;
            return this._items[this._length];
        }
        /**
         * 見かけ上の要素数をi_count個減らします。
         * @param i_count
         * @return
         */
        public void pops(int i_count)
        {
            Debug.Assert(this._length >= i_count);
            this._length -= i_count;
            return;
        }
        /**
         * 配列を返します。
         * 
         * @return
         */
        public T[] getArray()
        {
            return this._items;
        }
        public T getItem(int i_index)
        {
            return this._items[i_index];
        }
        /**
         * 配列の見かけ上の要素数を返却します。
         * @return
         */
        public int getLength()
        {
            return this._length;
        }
        /**
         * 見かけ上の要素数をリセットします。
         */
        public void clear()
        {
            this._length = 0;
        }
    }
}