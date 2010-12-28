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
#endif

namespace NyARToolkitCSUtils.NyAR
{
    public class NyARD3dDrawUtil
    {
	    //private static TextRenderer _tr=new TextRenderer(new Font("SansSerif", Font.PLAIN, 10));
        private static Int16[] _vertexIndices = new Int16[] { 2, 0, 1, 1, 3, 2, 4, 0, 2, 2, 6, 4, 5, 1, 0, 0, 4, 5, 7, 3, 1, 1, 5, 7, 6, 2, 3, 3, 7, 6, 4, 6, 7, 7, 5, 4 };

        /**
         * カラーキューブをカプセル化したクラス
         */
        public class ColorCube :IDisposable
        {
            private VertexBuffer _vertexBuffer;
            private IndexBuffer _indexBuffer;
            public ColorCube(Microsoft.DirectX.Direct3D.Device i_dev, int i_size)
            {
                //立方体（頂点数8）の準備
                this._vertexBuffer = new VertexBuffer(typeof(CustomVertex.PositionColored),
                    8,i_dev, Usage.None, CustomVertex.PositionColored.Format, Pool.Managed);
                //8点の情報を格納するためのメモリを確保
                CustomVertex.PositionColored[] vertices = new CustomVertex.PositionColored[8];
                const float CUBE_SIZE = 20.0f;//1辺40[mm]の
                //頂点を設定
                vertices[0] = new CustomVertex.PositionColored(-CUBE_SIZE, CUBE_SIZE, CUBE_SIZE, Color.Yellow.ToArgb());
                vertices[1] = new CustomVertex.PositionColored(CUBE_SIZE, CUBE_SIZE, CUBE_SIZE, Color.Gray.ToArgb());
                vertices[2] = new CustomVertex.PositionColored(-CUBE_SIZE, CUBE_SIZE, -CUBE_SIZE, Color.Purple.ToArgb());
                vertices[3] = new CustomVertex.PositionColored(CUBE_SIZE, CUBE_SIZE, -CUBE_SIZE, Color.Red.ToArgb());
                vertices[4] = new CustomVertex.PositionColored(-CUBE_SIZE, -CUBE_SIZE, CUBE_SIZE, Color.SkyBlue.ToArgb());
                vertices[5] = new CustomVertex.PositionColored(CUBE_SIZE, -CUBE_SIZE, CUBE_SIZE, Color.Orange.ToArgb());
                vertices[6] = new CustomVertex.PositionColored(-CUBE_SIZE, -CUBE_SIZE, -CUBE_SIZE, Color.Green.ToArgb());
                vertices[7] = new CustomVertex.PositionColored(CUBE_SIZE, -CUBE_SIZE, -CUBE_SIZE, Color.Blue.ToArgb());

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
                    Pool.Managed, true);

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
            public void draw(Microsoft.DirectX.Direct3D.Device i_dev)
            {
                VertexFormats old_VertexFormat=i_dev.VertexFormat;
                IndexBuffer old_Indices;
                Cull old_CullMode;
                // 頂点バッファをデバイスのデータストリームにバインド
                i_dev.SetStreamSource(0, this._vertexBuffer, 0);

                // 描画する頂点のフォーマットをセット
                i_dev.VertexFormat = CustomVertex.PositionColored.Format;

                // インデックスバッファをセット
                old_Indices=i_dev.Indices;
                i_dev.Indices = this._indexBuffer;

                // レンダリング（描画）
                old_CullMode=i_dev.RenderState.CullMode;
                i_dev.RenderState.CullMode = Cull.Clockwise;
                i_dev.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 8, 0, 12);
                i_dev.VertexFormat=old_VertexFormat;
                i_dev.Indices=old_Indices;
                i_dev.RenderState.CullMode=old_CullMode;
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

        ///**
        // * フォントカラーをセットします。
        // * @param i_c
        // */
        //public static void setFontColor(Color i_c)
        //{
        //    NyARGLDrawUtil._tr.setColor(i_c);
        //}
        ///**
        // * フォントスタイルをセットします。
        // * @param i_font_name
        // * @param i_font_style
        // * @param i_size
        // */
        //public static void setFontStyle(String i_font_name,int i_font_style,int i_size)
        //{
        //    NyARGLDrawUtil._tr=new TextRenderer(new Font(i_font_name,i_font_style, i_size));
        //}
        ///**
        // * 現在のフォントで、文字列を描画します。
        // * @param i_str
        // * @param i_scale
        // */
        //public static void drawText(String i_str,float i_scale)
        //{
        //    NyARGLDrawUtil._tr.begin3DRendering();
        //    NyARGLDrawUtil._tr.draw3D(i_str, 0f,0f,0f,i_scale);
        //    NyARGLDrawUtil._tr.end3DRendering();
        //    return;
        //}
        ///**
        // * INyARRasterの内容を現在のビューポートへ描画します。
        // * @param i_gl
        // * @param i_raster
        // * @param i_zoom
        // * @throws NyARException
        // */
        //public static void drawBackGround(javax.media.opengl.GL i_gl,INyARRaster i_raster, double i_zoom) throws NyARException
        //{
        //    IntBuffer texEnvModeSave = IntBuffer.allocate(1);
        //    boolean lightingSave;
        //    boolean depthTestSave;
        //    final NyARIntSize rsize=i_raster.getSize();
        //    // Prepare an orthographic projection, set camera position for 2D drawing, and save GL state.
        //    i_gl.glGetTexEnviv(GL.GL_TEXTURE_ENV, GL.GL_TEXTURE_ENV_MODE, texEnvModeSave); // Save GL texture environment mode.
        //    if (texEnvModeSave.array()[0] != GL.GL_REPLACE) {
        //        i_gl.glTexEnvi(GL.GL_TEXTURE_ENV, GL.GL_TEXTURE_ENV_MODE, GL.GL_REPLACE);
        //    }
        //    lightingSave = i_gl.glIsEnabled(GL.GL_LIGHTING); // Save enabled state of lighting.
        //    if (lightingSave == true) {
        //        i_gl.glDisable(GL.GL_LIGHTING);
        //    }
        //    depthTestSave = i_gl.glIsEnabled(GL.GL_DEPTH_TEST); // Save enabled state of depth test.
        //    if (depthTestSave == true) {
        //        i_gl.glDisable(GL.GL_DEPTH_TEST);
        //    }
        //    //ProjectionMatrixとModelViewMatrixを初期化
        //    i_gl.glMatrixMode(GL.GL_PROJECTION);
        //    i_gl.glPushMatrix();
        //    i_gl.glLoadIdentity();
        //    i_gl.glOrtho(0.0,rsize.w, 0.0,rsize.h,0,1);
        //    i_gl.glMatrixMode(GL.GL_MODELVIEW);
        //    i_gl.glPushMatrix();
        //    i_gl.glLoadIdentity();
        //    arglDispImageStateful(i_gl,rsize,i_raster.getBuffer(),i_raster.getBufferType(),i_zoom);
        //    //ProjectionMatrixとModelViewMatrixを回復
        //    i_gl.glMatrixMode(GL.GL_PROJECTION);
        //    i_gl.glPopMatrix();
        //    i_gl.glMatrixMode(GL.GL_MODELVIEW);
        //    i_gl.glPopMatrix();
        //    if (depthTestSave) {
        //        i_gl.glEnable(GL.GL_DEPTH_TEST); // Restore enabled state of depth test.
        //    }
        //    if (lightingSave) {
        //        i_gl.glEnable(GL.GL_LIGHTING); // Restore enabled state of lighting.
        //    }
        //    if (texEnvModeSave.get(0) != GL.GL_REPLACE) {
        //        i_gl.glTexEnvi(GL.GL_TEXTURE_ENV, GL.GL_TEXTURE_ENV_MODE, texEnvModeSave.get(0)); // Restore GL texture environment mode.
        //    }
        //    i_gl.glEnd();
        //}

        ///**
        // * arglDispImageStateful関数モドキ
        // * @param image
        // * @param zoom
        // */
        //private static void arglDispImageStateful(GL gl,NyARIntSize i_size,Object i_buffer,int i_buffer_type, double zoom) throws NyARException
        //{
        //    float zoomf;
        //    IntBuffer params = IntBuffer.allocate(4);
        //    zoomf = (float) zoom;
        //    gl.glDisable(GL.GL_TEXTURE_2D);
        //    gl.glGetIntegerv(GL.GL_VIEWPORT, params);
        //    gl.glPixelZoom(zoomf * ((float) (params.get(2)) / (float) i_size.w), -zoomf * ((float) (params.get(3)) / (float) i_size.h));
        //    gl.glWindowPos2f(0.0f, (float) i_size.h);
        //    gl.glPixelStorei(GL.GL_UNPACK_ALIGNMENT, 1);
        //    //BufferTypeの変換
        //    switch(i_buffer_type)
        //    {
        //    case NyARBufferType.BYTE1D_B8G8R8_24:
        //        gl.glDrawPixels(i_size.w,i_size.h,GL.GL_BGR, GL.GL_UNSIGNED_BYTE, ByteBuffer.wrap((byte[])i_buffer));
        //        break;
        //    case NyARBufferType.BYTE1D_R8G8B8_24:
        //        gl.glDrawPixels(i_size.w,i_size.h,GL.GL_RGB, GL.GL_UNSIGNED_BYTE, ByteBuffer.wrap((byte[])i_buffer));
        //        break;
        //    case NyARBufferType.BYTE1D_B8G8R8X8_32:
        //        gl.glDrawPixels(i_size.w,i_size.h,GL.GL_BGRA, GL.GL_UNSIGNED_BYTE, ByteBuffer.wrap((byte[])i_buffer));
        //        break;
        //    case NyARBufferType.INT1D_GRAY_8:
        //        /** @bug don't work*/
        //        gl.glDrawPixels(i_size.w,i_size.h,GL.GL_LUMINANCE, GL.GL_UNSIGNED_INT, IntBuffer.wrap((int[])i_buffer));
        //        break;
        //    default:
        //        throw new NyARException();
        //    }
        //}
        ///**
        // * スクリーン座標系をOpenGLにロードします。この関数は、PROJECTIONとMODELVIEWスタックをそれぞれ1づつpushします。
        // * スクリーン座標系を使用し終わったら、endScreenCoordinateSystemを呼び出してください。
        // * @param i_gl
        // * @param i_width
        // * @param i_height
        // * @param i_revers_y_direction
        // * Y軸の反転フラグです。trueならばtop->bottom、falseならばbottom->top方向になります。
        // */
        //public static void beginScreenCoordinateSystem(GL i_gl,int i_width,int i_height,boolean i_revers_y_direction)
        //{
        //    i_gl.glMatrixMode(GL.GL_PROJECTION);
        //    i_gl.glPushMatrix(); // Save world coordinate system.
        //    i_gl.glLoadIdentity();
        //    if(i_revers_y_direction){
        //        i_gl.glOrtho(0.0,i_width,i_height,0,-1,1);
        //    }else{
        //        i_gl.glOrtho(0.0,i_width,0,i_height,-1,1);
        //    }
        //    i_gl.glMatrixMode(GL.GL_MODELVIEW);
        //    i_gl.glPushMatrix(); // Save world coordinate system.
        //    i_gl.glLoadIdentity();
        //    return;
        //}
        ///**
        // * ロードしたスクリーン座標系を元に戻します。{@link #beginScreenCoordinateSystem}の後に呼び出してください。
        // * @param i_gl
        // */
        //public static void endScreenCoordinateSystem(GL i_gl)
        //{
        //    i_gl.glMatrixMode(GL.GL_PROJECTION);
        //    i_gl.glPopMatrix();
        //    i_gl.glMatrixMode(GL.GL_MODELVIEW);		
        //    i_gl.glPopMatrix();
        //    return;
        //}	
    }
}