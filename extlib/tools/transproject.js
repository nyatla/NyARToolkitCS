/*	This program pick up source file list of WindowsMobile from standard project file.
	Copy elements of under root element to The WM project.
*/
var fileXml = "../../ForFW2.0/NyARToolkitCS/NyARToolkitCS.csproj";
var fileXsl = "pprj2mprj.xsl";
var fileHtm = "output.xml";

var objDoc = new ActiveXObject("Msxml2.DOMDocument");
var objStl = new ActiveXObject("Msxml2.DOMDocument");
var fs = WScript.CreateObject("Scripting.FileSystemObject");
var flTxt = fs.CreateTextFile(fileHtm);

objDoc.load(fileXml);
objStl.load(fileXsl);

var txtXml = objDoc.transformNode(objStl);

with(flTxt){
  Write(txtXml);
  Close()
}