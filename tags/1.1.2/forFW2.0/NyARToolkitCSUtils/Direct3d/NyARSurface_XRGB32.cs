/*
 * NyARToolkitCSUtils NyARToolkit for C# 支援ライブラリ
 * 
 * (c)2008 A虎＠nyatla.jp
 * airmail(at)ebony.plala.or.jp
 * http://nyatla.jp/
 */
using System;
using System.Drawing;
using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using NyARToolkitCSUtils.Raster;
using System.Runtime.InteropServices;

namespace NyARToolkitCSUtils.Direct3d
{
    /* DsRGB556Rasterのラスタデータを取り込むことが出来るSurfaceです。
     * このSurfaceはそのままARToolKitの背景描画に使います。
     */
    public class NyARSurface_XRGB32
    {
        private int m_width;
        private int m_height;
        private Microsoft.DirectX.Direct3D.Device m_ref_dev;
        private Surface m_surface;
        public Surface d3d_surface
        {
            get { return this.m_surface; }
        }

        /* i_width x i_heightのテクスチャを格納するインスタンスを生成します。
         * 確保されるテクスチャのサイズは指定したサイズと異なり、i_width x i_heightのサイズを超える
         * 2のべき乗サイズになります。
         * 
         */
        public NyARSurface_XRGB32(Microsoft.DirectX.Direct3D.Device i_dev, int i_width, int i_height)
        {
            this.m_ref_dev = i_dev;

            this.m_height = i_height;
            this.m_width = i_width;

            this.m_surface = i_dev.CreateOffscreenPlainSurface(i_width, i_height, Format.X8R8G8B8, Pool.Default);

            //OK、完成だ。
            return;
        }
        /* DsXRGB32Rasterの内容を保持しているサーフェイスにコピーします。
         */
        public void CopyFromXRGB32(DsXRGB32Raster i_sample)
        {
            GraphicsStream gs = this.m_surface.LockRectangle(LockFlags.None);
            int cp_size = this.m_width * 4;
            int s_idx=0;
            int d_idx = (this.m_height - 1) * cp_size;
            for(int i=this.m_height-1;i>=0;i--){
                //どう考えてもポインタです。
                Marshal.Copy(i_sample.buffer,s_idx,(IntPtr)((int)gs.InternalData+d_idx),cp_size);
                s_idx += cp_size;
                d_idx -= cp_size;
            }
            this.m_surface.UnlockRectangle();

            return;
        }

    }
}
