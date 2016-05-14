/* 
 * PROJECT: NyARToolkitCSUtils NyARToolkit for C# 支援ライブラリ
 * --------------------------------------------------------------------------------
 * The MIT License
 * Copyright (c) 2008 nyatla
 * airmail(at)ebony.plala.or.jp
 * http://nyatla.jp/nyartoolkit/
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 * 
 */
//CFWでコンパイルするときはNyartoolkitCS_FRAMEWORK_CFWをアクティブにしてください。
//#define NyartoolkitCS_FRAMEWORK_CFW
using System;
using System.Collections.Generic;
using System.Drawing;
using jp.nyatla.nyartoolkit.cs.core;
#if NyartoolkitCS_FRAMEWORK_CFW
using Microsoft.WindowsMobile.DirectX.Direct3D;
using Microsoft.WindowsMobile.DirectX;
#else
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.VisualC;
#endif

namespace NyARToolkitCSUtils.Direct3d
{
    /**
     * カラーキューブをカプセル化したクラス
     */
    public class ColorCube : IDisposable
    {
#if NyartoolkitCS_FRAMEWORK_CFW
        private const Pool _pool_mode=Pool.SystemMemory;
#else
        private const Pool _pool_mode=Pool.Managed;
#endif

        //private static TextRenderer _tr=new TextRenderer(new Font("SansSerif", Font.PLAIN, 10));
        private static Int16[] _vertexIndices = new Int16[] { 2, 0, 1, 1, 3, 2, 4, 0, 2, 2, 6, 4, 5, 1, 0, 0, 4, 5, 7, 3, 1, 1, 5, 7, 6, 2, 3, 3, 7, 6, 4, 6, 7, 7, 5, 4 };

        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;
        public ColorCube(Device i_dev, float i_size)
        {
            //立方体（頂点数8）の準備
            this._vertexBuffer = new VertexBuffer(typeof(CustomVertex.PositionColored),
                8, i_dev, Usage.None, CustomVertex.PositionColored.Format, _pool_mode);
            //8点の情報を格納するためのメモリを確保
            CustomVertex.PositionColored[] vertices = new CustomVertex.PositionColored[8];
            float size = i_size / 2;
            //頂点を設定
            vertices[0] = new CustomVertex.PositionColored(-size, size, size, Color.Yellow.ToArgb());
            vertices[1] = new CustomVertex.PositionColored(size, size, size, Color.Gray.ToArgb());
            vertices[2] = new CustomVertex.PositionColored(-size, size, -size, Color.Purple.ToArgb());
            vertices[3] = new CustomVertex.PositionColored(size, size, -size, Color.Red.ToArgb());
            vertices[4] = new CustomVertex.PositionColored(-size, -size, size, Color.SkyBlue.ToArgb());
            vertices[5] = new CustomVertex.PositionColored(size, -size, size, Color.Orange.ToArgb());
            vertices[6] = new CustomVertex.PositionColored(-size, -size, -size, Color.Green.ToArgb());
            vertices[7] = new CustomVertex.PositionColored(size, -size, -size, Color.Blue.ToArgb());

            //頂点バッファをロックする
            using (GraphicsStream data = this._vertexBuffer.Lock(0, 0, LockFlags.None))
            {
                // 頂点データを頂点バッファにコピーします
                data.Write(vertices);
                // 頂点バッファのロックを解除します
                this._vertexBuffer.Unlock();
            }

            // インデックスバッファの作成
            // 第２引数の数値は(三角ポリゴンの数)*(ひとつの三角ポリゴンの頂点数)*
            // (16 ビットのインデックスサイズ(2byte))
            this._indexBuffer = new IndexBuffer(i_dev, 12 * 3 * 2, Usage.WriteOnly,
                _pool_mode, true);

            // インデックスバッファをロックする
            using (GraphicsStream data = this._indexBuffer.Lock(0, 0, LockFlags.None))
            {
                // インデックスデータをインデックスバッファにコピーします
                data.Write(_vertexIndices);

                // インデックスバッファのロックを解除します
                this._indexBuffer.Unlock();
            }
            return;
        }
        public void draw(Device i_dev)
        {
            IndexBuffer old_Indices;
            Cull old_CullMode;
            old_CullMode = i_dev.RenderState.CullMode;
            old_Indices = i_dev.Indices;
            i_dev.RenderState.CullMode = Cull.Clockwise;
            i_dev.Indices = this._indexBuffer;

#if NyartoolkitCS_FRAMEWORK_CFW
            // 頂点バッファをデバイスのデータストリームにバインド
            i_dev.SetStreamSource(0, this._vertexBuffer, 0);
            // レンダリング（描画）
            i_dev.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 8, 0, 12);
            i_dev.Indices = old_Indices;
            i_dev.RenderState.CullMode = old_CullMode;
#else
            VertexFormats old_VertexFormat = i_dev.VertexFormat;
            // 頂点バッファをデバイスのデータストリームにバインド
            i_dev.SetStreamSource(0, this._vertexBuffer, 0);
            // 描画する頂点のフォーマットをセット
            i_dev.VertexFormat = CustomVertex.PositionColored.Format;
            i_dev.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 8, 0, 12);
            i_dev.VertexFormat = old_VertexFormat;
#endif
            i_dev.Indices = old_Indices;
            i_dev.RenderState.CullMode = old_CullMode;
            return;
        }
        public void Dispose()
        {
            // 頂点バッファを解放
            if (this._vertexBuffer != null)
            {
                this._vertexBuffer.Dispose();
            }

            // インデックスバッファを解放
            if (this._indexBuffer != null)
            {
                this._indexBuffer.Dispose();
            }
        }
    }
//#endif
}
