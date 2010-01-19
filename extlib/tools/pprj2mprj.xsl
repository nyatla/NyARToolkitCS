<?xml version="1.0" encoding="Shift_JIS"?>
<xsl:stylesheet version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns="http://www.w3.org/1999/xhtml"
	xmlns:vsprj="http://schemas.microsoft.com/developer/msbuild/2003">
	
	<xsl:output method="xml" encoding="Shift_JIS"/>

<xsl:template match="/">
<root>
	<xsl:apply-templates select="vsprj:Project/vsprj:ItemGroup/vsprj:Compile"/>
</root>
</xsl:template>
<xsl:template match="vsprj:Project/vsprj:ItemGroup/vsprj:Compile">
	<xsl:element name="Compile">
		<xsl:attribute name="Include"><xsl:value-of select="concat('..\..\forFW2.0\\NyARToolkitCS\',@Include)"/></xsl:attribute>
		<xsl:element name="Link"><xsl:value-of select="@Include"/></xsl:element>
	</xsl:element> 
</xsl:template>
</xsl:stylesheet>