/* 
 * Capture Test NyARToolkitCSサンプルプログラム
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
using System;
using System.Xml;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Reflection;
using System.Drawing;
using jp.nyatla.nyartoolkit.cs.core;
using NyARToolkitCSUtils.WMCapture;
using NyARToolkitCSUtils.Direct3d;
using System.Windows.Forms;
using Microsoft.WindowsMobile.DirectX;

namespace NyARToolkitCS.WM5.RPF
{
    public class ResourceBuilder
    {
        private const int BGMODE_SURFACE = 0;
        private const int BGMODE_TEXTURE = 1;

        private String _root_path;
        private String _cpara_file;//data\\camera_para.dat";
        private String _code_file;//data\\patt.hiro";
        private bool _cvertical;//false
        private int _background_type;
        private int _code_size;//16
        private Size _cap_size = new Size();

        private NyARParam _ar_param;
        public NyARParam ar_param { get { return this._ar_param; } }

        public NyARCode createNyARCode()
        {
            NyARCode result = new NyARCode(this._code_size, this._code_size);
            result.loadARPattFromFile(this._root_path + "\\" + this._code_file);
            return result;
        }
        public WmCapture createWmCapture()
        {
            WmCapture result = new WmCapture(this._cap_size,this._cvertical);
            return result;
        }
        public DsRGB565Raster createARRaster()
        {
            return new DsRGB565Raster(this._cap_size.Width, this._cap_size.Height);
        }
        public D3dManager createD3dManager(Form i_form)
        {
            D3dManager result;
            result=new D3dManager(i_form, this._ar_param, -1);
            Matrix tmp = new Matrix();
            NyARD3dUtil.toCameraFrustumRH(this._ar_param,10,10000,ref tmp);
            result.d3d_device.Transform.Projection = tmp;
            return result;

        }
        public ID3dBackground createBackGround(D3dManager i_d3dmgr)
        {
            switch(this._background_type){
                case BGMODE_SURFACE:
                    return new D3dSurfaceBackground(i_d3dmgr.d3d_device, i_d3dmgr.background_size.Width,i_d3dmgr.background_size.Height);
                case BGMODE_TEXTURE:
                    return new D3dTextureBackground(i_d3dmgr.d3d_device, i_d3dmgr.background_size.Width, i_d3dmgr.background_size.Height,i_d3dmgr.scale);
                default:
                    throw new Exception("unknown this._background_type");
            }
        }

        public String getCameraParamFilePath()
        {
            return this._root_path + "\\" + this._cpara_file;
        }
        public String getCodeFileNamePath()
        {
            return this._root_path + "\\" + this._code_file;
        }
        /*
         * <nyar>
         *  <version>MobileSimpleLite/0.1</version>
         *  <config>
         *   <camera file="camera_para.dat" width="240" height="320" vertical="false"></camera>
         *   <patt file="patt.hiro" size="16"></patt>
         *   <direct3d background="texture"/>
         *  </config>
         * </nyar>
         */
        public ResourceBuilder()
        {
            //ルートパスの取得
            this._root_path=Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
            //設定の読み出し

            XmlDocument dom = new XmlDocument();
            dom.Load(this._root_path + "\\setting.xml");
            if (dom.SelectSingleNode("/root/version").InnerText != "MobileSimpleLite/0.1")
            {
                throw new Exception("設定ファイルのバージョンが違います？");
            }
            XmlNode config_node = dom.SelectSingleNode("/root/config");


            this._cap_size.Width = int.Parse(config_node.SelectSingleNode("camera/@width").Value);
            this._cap_size.Height =int.Parse(config_node.SelectSingleNode("camera/@height").Value);
            this._cpara_file = config_node.SelectSingleNode("camera/@file").Value;
            this._cvertical = bool.Parse(config_node.SelectSingleNode("camera/@vertical").Value);

            this._code_file = config_node.SelectSingleNode("patt/@file").Value;
            this._code_size = int.Parse(config_node.SelectSingleNode("patt/@size").Value);

            String bgmode=config_node.SelectSingleNode("direct3d/@background").Value;
            this._background_type = bgmode.CompareTo("texture")==0 ? BGMODE_TEXTURE : BGMODE_SURFACE;
            dom = null;

            //初期化セクション
            this._ar_param=new NyARParam();
            this._ar_param.loadARParamFromFile(this._root_path + "\\" +this._cpara_file);
            this._ar_param.changeScreenSize(this._cap_size.Width, this._cap_size.Height);


            return;
        }
    }
}
