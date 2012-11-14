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
using System.IO;
using System.Diagnostics;
namespace jp.nyatla.nyartoolkit.cs.core
{
    /**
     * {@link InputStream}から読み出すための
     * @author nyatla
     *
     */
    public class ByteBufferedInputStream
    {
	    public const int ENDIAN_LITTLE=1;
        public const int ENDIAN_BIG = 2;
        private byte[] _buf;
        private BinaryReader _stream;
        private bool _is_byte_swap;
        private int _read_len;
        public ByteBufferedInputStream(StreamReader i_stream, int i_buf_size)
        {
            this._buf = new byte[i_buf_size];
            this._read_len = 0;
            this._stream = new BinaryReader(i_stream.BaseStream);
        }
        /**
         * マルチバイト読み込み時のエンディアン.{@link #ENDIAN_BIG}か{@link #ENDIAN_LITTLE}を設定してください。
         * @param i_order
         */
        public void order(int i_order)
        {
            switch (i_order)
            {
                case ENDIAN_LITTLE:
                    this._is_byte_swap = BitConverter.IsLittleEndian?false:true;
                    break;
                case ENDIAN_BIG:
                    this._is_byte_swap = BitConverter.IsLittleEndian ? true : false;
                    break;
                default:
                    throw new NyARException();
            }
        }
        /**
         * Streamからバッファへi_sizeだけ読み出す。
         * @param i_size
         * @throws NyARException 
         */
        public int readToBuffer(int i_size)
        {
            Debug.Assert(this._read_len < this._buf.Length);
            int len;
            try
            {
                len=this._stream.Read(this._buf, 0, i_size);
            }
            catch (IOException e)
            {
                throw new NyARException(e);
            }
            //バッファの読み出し位置をリセット
            this._read_len = 0;
            return len;
        }
        public int readBytes(byte[] i_buf, int i_size)
        {
            try
            {
                return this._stream.Read(i_buf, 0, i_size);
            }
            catch (IOException e)
            {
                throw new NyARException(e);
            }
        }
        public int getInt()
        {
            Debug.Assert(this._read_len < this._buf.Length);
            int ret = BitConverter.ToInt32(this._buf, this._read_len);
            this._read_len += 4;
            if (!this._is_byte_swap)
            {
                return ret;
            }
            //big endian
            byte[] ba = BitConverter.GetBytes(ret);
            Array.Reverse(ba);
            return BitConverter.ToInt32(ba, 0);

        }
        public byte getByte()
        {
            Debug.Assert(this._read_len < this._buf.Length);
            byte ret = this._buf[this._read_len];
            this._read_len += 1;
            return ret;
        }
        public float getFloat()
        {
            Debug.Assert(this._read_len < this._buf.Length);
            float ret = BitConverter.ToSingle(this._buf,this._read_len);
            this._read_len += 4;
            if (!this._is_byte_swap)
            {
                return ret;
            }
            //big endian
            byte[] ba = BitConverter.GetBytes(ret);
            Array.Reverse(ba);
            return BitConverter.ToSingle(ba, 0);
        }
        public double getDouble()
        {
            Debug.Assert(this._read_len < this._buf.Length);
            double ret = BitConverter.ToDouble(this._buf, this._read_len);
            this._read_len += 8;
            if (!this._is_byte_swap)
            {
                return ret;
            }
            //big endian
            byte[] ba = BitConverter.GetBytes(ret);
            Array.Reverse(ba);
            return BitConverter.ToDouble(ba, 0);

        }
    }
}