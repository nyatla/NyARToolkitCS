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
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Reflection;

namespace SimpleLiteDirect3d.WindowsMobile5
{
    class AppFilePathInfo
    {
        private String _root_path;
        private String _cpara_file="data\\camera_para.dat";
        private String _code_file="data\\patt.hiro";
        public String cpara_file_path
        {
            get { return this._cpara_file; }
        }
        public String code_file_path
        {
            get { return this._code_file; }
        }
        public String getCameraParamFilePath()
        {
            return this._root_path + "\\" + this._cpara_file;
        }
        public String getCodeFileNamePath()
        {
            return this._root_path + "\\" + this._code_file;
        }
        public AppFilePathInfo()
        {
            //ルートパスの取得
            this._root_path=Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
            return;
        }
    }
}
