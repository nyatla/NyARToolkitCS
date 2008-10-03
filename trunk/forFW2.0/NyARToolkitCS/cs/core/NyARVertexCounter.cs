using System;
using System.Collections.Generic;
using System.Text;

namespace jp.nyatla.nyartoolkit.cs.core
{
    /**
     * get_vertex関数を切り離すためのクラス
     * 
     */
    public class NyARVertexCounter
    {
        public int[] vertex = new int[10];// 5まで削れる

        public int number_of_vertex;

        private double thresh;

        private int[] x_coord;

        private int[] y_coord;

        public bool getVertex(int[] i_x_coord, int[] i_y_coord, int st, int ed, double i_thresh)
        {
            this.number_of_vertex = 0;
            this.thresh = i_thresh;
            this.x_coord = i_x_coord;
            this.y_coord = i_y_coord;
            return get_vertex(st, ed);
        }

        /**
         * static int get_vertex( int x_coord[], int y_coord[], int st, int ed,double thresh, int vertex[], int *vnum) 関数の代替関数
         * 
         * @param x_coord
         * @param y_coord
         * @param st
         * @param ed
         * @param thresh
         * @return
         */
        private bool get_vertex(int st, int ed)
        {
            int v1 = 0;
            int[] lx_coord = this.x_coord;
            int[] ly_coord = this.y_coord;
            double a = ly_coord[ed] - ly_coord[st];
            double b = lx_coord[st] - lx_coord[ed];
            double c = lx_coord[ed] * ly_coord[st] - ly_coord[ed] * lx_coord[st];
            double dmax = 0;
            double d;
            for (int i = st + 1; i < ed; i++)
            {
                d = a * lx_coord[i] + b * ly_coord[i] + c;
                if (d * d > dmax)
                {
                    dmax = d * d;
                    v1 = i;
                }
            }
            if (dmax / (a * a + b * b) > thresh)
            {
                if (!get_vertex(st, v1))
                {
                    return false;
                }
                if (number_of_vertex > 5)
                {
                    return false;
                }
                vertex[number_of_vertex] = v1;// vertex[(*vnum)] = v1;
                number_of_vertex++;// (*vnum)++;

                if (!get_vertex(v1, ed))
                {
                    return false;
                }
            }
            return true;
        }
    }

}
