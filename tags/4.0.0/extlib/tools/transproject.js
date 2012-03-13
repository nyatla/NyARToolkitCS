/*	This program pick up source file list of WindowsMobile from standard project file.
	Copy elements of under root element to The WM project.
*/
var fileXml_1 = "../../ForFW2.0/NyARToolkitCS/NyARToolkitCS.csproj";
var fileHtm_1 = "NyARToolkitCS.output.xml";
var fileXml_2 = "../../ForFW2.0/NyARToolkitCS.rpf/NyARToolkitCS.rpf.csproj";
var fileHtm_2 = "NyARToolkitCS.rpf.output.xml";

var fileXsl = "pprj2mprj.xsl";

var objDoc = new ActiveXObject("Msxml2.DOMDocument");
var objStl = new ActiveXObject("Msxml2.DOMDocument");
var fs = WScript.CreateObject("Scripting.FileSystemObject");


objStl.load(fileXsl);

function convertProjectFile(i_base_prj_name,i_out_xml_name)
{
	var flTxt = fs.CreateTextFile(i_out_xml_name);
	objDoc.load(i_base_prj_name);
	var txtXml = objDoc.transformNode(objStl);
	with(flTxt){
		Write(txtXml);
		Close();
	}
}

convertProjectFile(fileXml_1,fileHtm_1);
convertProjectFile(fileXml_2,fileHtm_2);


