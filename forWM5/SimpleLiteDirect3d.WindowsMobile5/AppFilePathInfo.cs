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
