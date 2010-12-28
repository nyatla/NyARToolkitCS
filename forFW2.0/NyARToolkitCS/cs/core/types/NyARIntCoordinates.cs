using System;
using System.Collections.Generic;
using System.Text;

namespace jp.nyatla.nyartoolkit.cs.core
{
    /**
     * 座標点配列を定義します。
     * 座標点配列は、輪郭線やパスの定義に使用します。
     */
    public class NyARIntCoordinates
    {
        /**
         * 点を格納する配列です。
         */
        public NyARIntPoint2d[] items;
        /**
         * 配列の有効な長さです。0から、items.length-1までの数を取ります。
         */
        public int length;
        public NyARIntCoordinates(int i_length)
        {
            this.items = NyARIntPoint2d.createArray(i_length);
            this.length = 0;
        }
        /**
         * 指定した点を結ぶ直線を計算して、輪郭に保存します。
         * 動作チェックはしたけど、多分動くレベル。
         * @param i_x0
         * @param i_y0
         * @param i_x1
         * @param i_y1
         * @param o_coord
         * @return
         * 成功するとtrueを返します。
         */
        public bool setLineCoordinates(int i_x0, int i_y0, int i_x1, int i_y1)
        {
            NyARIntPoint2d[] ptr = this.items;
            // 線分を定義
            int dx = (i_x1 > i_x0) ? i_x1 - i_x0 : i_x0 - i_x1;
            int dy = (i_y1 > i_y0) ? i_y1 - i_y0 : i_y0 - i_y1;
            int sx = (i_x1 > i_x0) ? 1 : -1;
            int sy = (i_y1 > i_y0) ? 1 : -1;

            // Bresenham
            int idx = 0;
            if (dx >= dy)
            {
                // 傾きが1以下の場合
                if (dx >= ptr.Length)
                {
                    return false;
                }
                int E = -dx;
                for (int i = 0; i <= dx; i++)
                {
                    ptr[idx].x = i_x0;
                    ptr[idx].y = i_y0;
                    idx++;
                    i_x0 += sx;
                    E += 2 * dy;
                    if (E >= 0)
                    {
                        i_y0 += sy;
                        E -= 2 * dx;
                    }
                }
            }
            else
            {
                // 傾きが1より大きい場合
                if (dy >= this.items.Length)
                {
                    return false;
                }
                int E = -dy;
                for (int i = 0; i <= dy; i++)
                {
                    ptr[idx].x = i_x0;
                    ptr[idx].y = i_y0;
                    idx++;
                    i_y0 += sy;
                    E += 2 * dx;
                    if (E >= 0)
                    {
                        i_x0 += sx;
                        E -= 2 * dy;
                    }
                }
            }
            this.length = idx;
            return true;
        }
    }

}
