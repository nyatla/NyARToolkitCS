using System;
using System.Collections.Generic;
using System.Text;
#if NyartoolkitCS_FRAMEWORK_CFW
using Microsoft.WindowsMobile.DirectX.Direct3D;
using Microsoft.WindowsMobile.DirectX;
#else
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
#endif
namespace NyARToolkitCSUtils.Direct3d.markersystem
{
    public class D3dRender
    {
	private NyARD3dMarkerSystem _ms;

	/**
	 * コンストラクタです。マーカシステムに対応したレンダラを構築します。
	 * @param i_ms
	 */
    public D3dRender(NyARD3dMarkerSystem i_ms)
	{
		this._ms=i_ms;		
	}
    /// <summary>
    /// DirectXのビューをAR向けに設定します。
    /// </summary>
    public void setARView(Device i_dev)
    {

    }
	//
	// Graphics toolkit
	//
/*	public void drawBackground(GL i_gl,INyARRgbRaster i_bg_image) throws NyARException
	{
		i_gl.glClear(GL.GL_COLOR_BUFFER_BIT | GL.GL_DEPTH_BUFFER_BIT); // Clear the buffers for new frame.
		NyARGLDrawUtil.drawBackGround(i_gl,i_bg_image, 1.0);
	}
	public void drawBackground(GL i_gl,INyARGrayscaleRaster i_bg_image) throws NyARException
	{
		i_gl.glClear(GL.GL_COLOR_BUFFER_BIT | GL.GL_DEPTH_BUFFER_BIT); // Clear the buffers for new frame.
		NyARGLDrawUtil.drawBackGround(i_gl,i_bg_image, 1.0);
	}
*/	
	/**
	 * 指定位置にカラーキューブを書き込みます。
	 * @param i_gl
	 * @param i_size_per_mm
	 * @param i_x
	 * @param i_y
	 * @param i_z
	 *//*
	public void colorCube(GL i_gl,float i_size_per_mm,double i_x,double i_y,double i_z)
	{
		int old_mode=this.getGlMatrixMode(i_gl);
		i_gl.glMatrixMode(GL.GL_MODELVIEW);
		i_gl.glPushMatrix();
		i_gl.glTranslated(i_x,i_y,i_z);
		NyARGLDrawUtil.drawColorCube(i_gl,i_size_per_mm);
		i_gl.glPopMatrix();
		i_gl.glMatrixMode(old_mode);
	}
	
	public void setColor(GL i_gl,float r,float g,float b)
	{
		i_gl.glColor3f(r,g,b);
	}
	public void setStrokeWeight(GL i_gl,float i_width)
	{
		i_gl.glLineWidth(i_width);
	}*/
	/**
	 * この関数は、現在のカラーで
	 * @param i_gl
	 * @param i_x
	 * @param i_y
	 * @param i_x2
	 * @param i_y2
	 *//*
	public void line(GL i_gl,float i_x,double i_y,double i_x2,double i_y2)
	{
		int old_mode=this.getGlMatrixMode(i_gl);
		i_gl.glMatrixMode(GL.GL_MODELVIEW);
		i_gl.glBegin(GL.GL_LINE);
		
		i_gl.glVertex2d(i_x,i_y);
		i_gl.glEnd();
		i_gl.glMatrixMode(old_mode);
	}
	public void polygon(GL i_gl,NyARDoublePoint2d[] i_vertex)
	{
		int old_mode=this.getGlMatrixMode(i_gl);
		i_gl.glMatrixMode(GL.GL_MODELVIEW);
		i_gl.glBegin(GL.GL_LINE_LOOP);
		for(int i=0;i<i_vertex.length;i++)
		{
			i_gl.glVertex2d(i_vertex[i].x,i_vertex[i].y);
		}
		i_gl.glEnd();
		i_gl.glMatrixMode(old_mode);
	}
	public void polygon(GL i_gl,NyARIntPoint2d[] i_vertex)
	{
		int old_mode=this.getGlMatrixMode(i_gl);
		i_gl.glMatrixMode(GL.GL_MODELVIEW);
		i_gl.glBegin(GL.GL_LINE_LOOP);
		for(int i=0;i<i_vertex.length;i++)
		{
			i_gl.glVertex2d(i_vertex[i].x,i_vertex[i].y);
		}
		i_gl.glEnd();
		i_gl.glMatrixMode(old_mode);
	}
	public void drawRaster(GL i_gl,double i_x, double i_y, NyARRgbRaster i_raster) throws NyARException
	{
		i_gl.glPushMatrix();
		try{
			i_gl.glTranslated(i_x,i_y,0);
			NyARGLDrawUtil.drawRaster(i_gl, i_raster);
		}finally{
			i_gl.glPopMatrix();
		}
	}*/
    }
}
