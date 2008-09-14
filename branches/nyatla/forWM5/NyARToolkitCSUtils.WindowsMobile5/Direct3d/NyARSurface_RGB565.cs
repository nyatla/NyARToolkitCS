/*
 * NyARToolkitCSUtils NyARToolkit for C# 支援ライブラリ
 * 
 * (c)2008 A虎＠nyatla.jp
 * airmail(at)ebony.plala.or.jp
 * http://nyatla.jp/
 */
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using Microsoft.WindowsMobile.DirectX;
using Microsoft.WindowsMobile.DirectX.Direct3D;
using NyARToolkitCSUtils.Raster;
using System.Runtime.InteropServices;
using jp.nyatla.cs.NyWMCapture;

namespace NyARToolkitCSUtils.Direct3d
{
    /* DsRGB556Rasterのラスタデータを取り込むことが出来るSurfaceです。
     * このSurfaceはそのままARToolKitの背景描画に使います。
     */
    public class NyARSurface_RGB565
    {
        private int m_width;
        private int m_height;
        private Microsoft.WindowsMobile.DirectX.Direct3D.Device m_ref_dev;
        private Surface m_surface;
        private Rectangle m_src_rect;
        public Rectangle d3d_surface_rect
        {
            get { return this.m_src_rect; }
        }
        public Surface d3d_surface
        {
            get { return this.m_surface; }
        }

        /* i_width x i_heightのテクスチャを格納するインスタンスを生成します。
         * 確保されるテクスチャのサイズは指定したサイズと異なり、i_width x i_heightのサイズを超える
         * 2のべき乗サイズになります。
         * 
         */
        public NyARSurface_RGB565(Microsoft.WindowsMobile.DirectX.Direct3D.Device i_dev, int i_width, int i_height)
        {
            this.m_ref_dev = i_dev;

            this.m_height = i_height;
            this.m_width = i_width;

            this.m_surface = i_dev.CreateImageSurface(i_width, i_height, Format.R5G6B5);
            this.m_src_rect = new Rectangle(0, 0, i_width,i_height);

            //OK、完成だ。
            return;
        }
        /* DsXRGB32Rasterの内容を保持しているサーフェイスにコピーします。
         */
        public void CopyFromIntPtr(INySample i_sample,bool i_is_turn)
        {
            int pitch;
            GraphicsStream gs = this.m_surface.LockRectangle(this.m_src_rect, LockFlags.None, out pitch);
            if (i_is_turn)
            {
                i_sample.CopyToBuffer(gs.InternalData, 0, this.m_width * this.m_height * 2);
            }else{
                int st = this.m_width * 2;
                int s_idx=0;
                int d_idx = (this.m_height-1) * st;
                for(int i=this.m_height-1;i>=0;i--){
                    i_sample.CopyToBuffer((IntPtr)((int)gs.InternalData+d_idx), s_idx, st);
                    s_idx += st;
                    d_idx -= st;
                }
            }
            this.m_surface.UnlockRectangle();

            return;
        }

    }
}
