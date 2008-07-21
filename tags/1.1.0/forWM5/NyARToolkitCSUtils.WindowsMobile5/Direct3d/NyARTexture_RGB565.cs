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
    /* DsRGB556Rasterのラスタデータを取り込むことが出来るTextureです。
     * このテクスチャはそのままARToolKitの背景描画に使います。
     */
    public class NyARTexture_RGB565
    {
        private int m_width;
        private int m_height;
        private int m_texture_width;
        private int m_texture_height;
        private Microsoft.WindowsMobile.DirectX.Direct3D.Device m_ref_dev;
        private Texture m_texture;
        private Bitmap m_bitmap;
        private Surface m_bmp_surface;

        /* i_valueを超える最も小さい2のべき乗の値を返します。
         * 
         */
        private int GetSquareSize(int i_value)
        {
            int u = 2;
            //2^nでサイズを超える一番小さな値を得る。
            for (; ; )
            {
                if (u >= i_value)
                {
                    break;
                }
                u = u << 1;
                if (u <= 0)
                {
                    throw new Exception();
                }
            }
            return u;
        }
        public Texture d3d_texture
        {
            get { return this.m_texture; }
        }

        /* i_width x i_heightのテクスチャを格納するインスタンスを生成します。
         * 確保されるテクスチャのサイズは指定したサイズと異なり、i_width x i_heightのサイズを超える
         * 2のべき乗サイズになります。
         * 
         */
        public NyARTexture_RGB565(Microsoft.WindowsMobile.DirectX.Direct3D.Device i_dev, int i_width, int i_height)
        {
            this.m_ref_dev = i_dev;

            this.m_height = i_height;
            this.m_width = i_width;

            //テクスチャサイズの確定(2^n)
            this.m_texture_height = GetSquareSize(i_height);
            this.m_texture_width = GetSquareSize(i_width);

            this.m_bitmap     = new Bitmap(i_width, i_height);
            this.m_bmp_surface = new Surface(this.m_ref_dev, this.m_bitmap, Pool.SystemMemory);

            //テクスチャを作るよ！
            this.m_texture = new Texture(this.m_ref_dev, this.m_texture_width, this.m_texture_height, 1, Usage.None, Format.R5G6B5, Pool.SystemMemory);
            //OK、完成だ。
            return;
        }
        /* DsXRGB32Rasterの内容を保持しているテクスチャにコピーします。
         * i_rasterのサイズは、このインスタンスに指定したテクスチャサイズ（コンストラクタ等に指定したサイズ）と同じである必要です。
         * ラスタデータはテクスチャの左上を基点にwidth x heightだけコピーされ、残りの部分は更新されません。
         */
        public void CopyFromIntPtr(INySample i_sample)
        {
            //いまいち納得いかないビットマップ転送()GC経由
            Rectangle rect = new Rectangle(0, 0, this.m_width, this.m_height);
            
            //転送元の準備
            BitmapData bd = this.m_bitmap.LockBits(
                rect,
                ImageLockMode.WriteOnly,
                PixelFormat.Format16bppRgb565);
            //
            i_sample.CopyToBuffer(bd.Scan0, 0, this.m_width * this.m_height * 2);
            this.m_bitmap.UnlockBits(bd);
            //
            this.m_bmp_surface.GetGraphics().DrawImage(this.m_bitmap, 0, 0);
            this.m_bmp_surface.ReleaseGraphics();

            //転送先の準備
            Surface dest_su = this.m_texture.GetSurfaceLevel(0);

            //コピー
            this.m_ref_dev.CopyRects(this.m_bmp_surface,rect, dest_su, new Point(0, 0));
            dest_su = null;
            bd = null;
            return;
        }
    }
}
