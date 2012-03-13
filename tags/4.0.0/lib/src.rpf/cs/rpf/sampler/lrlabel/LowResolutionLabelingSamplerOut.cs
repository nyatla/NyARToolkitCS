/* 
 * PROJECT: NyARToolkitCS(Extension)
 * --------------------------------------------------------------------------------
 * The NyARToolkitCS is C# edition ARToolKit class library.
 * Copyright (C)2008-2009 Ryo Iizuka
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * For further information please contact.
 *	http://nyatla.jp/nyatoolkit/
 *	<airmail(at)ebony.plala.or.jp> or <nyatla(at)nyatla.jp>
 * 
 */
using System;
using System.Diagnostics;
using jp.nyatla.nyartoolkit.cs.core;

namespace jp.nyatla.nyartoolkit.cs.rpf

{
/**
 * LowResolutionLabelingSampler用の出力コンテナです。サンプリング結果を受け取ります。
 * 内容には、AreaDataItemの集合を持ちます。
 * AreaDataItemは元画像に対する、Labeling結果と元画像の情報のセットです。
 */
public class LowResolutionLabelingSamplerOut
{
	/**
	 * クラス内定義ができない処理系では、LowResolutionLabelingSamplerOutItemで定義してください。
	 *
	 */
	public class Item : NyARManagedObject
	{
		/**
		 * ラべリング対象のエントリポイントです。
		 */
		public NyARIntPoint2d entry_pos=new NyARIntPoint2d();
		/**
		 * ラべリング対象の範囲を、トップレベル換算した値です。クリップ情報から計算されます。
		 */
		public NyARIntRect    base_area  =new NyARIntRect();
		/**
		 * ラべリング対象の範囲中心を、トップレベルに換算した値です。クリップ情報から計算されます。
		 */
		public NyARIntPoint2d base_area_center=new NyARIntPoint2d();
		/**
		 * エリア矩形の対角距離の2乗値
		 */
		public int base_area_sq_diagonal;
		
		public int lebeling_th;
		
		public Item(INyARManagedObjectPoolOperater i_pool):
            base(i_pool)
		{
		}
	}	
	/**
	 * AreaのPoolクラス
	 *
	 */
	private class AreaPool : NyARManagedObjectPool<Item>
	{
		public AreaPool(int i_length)
		{
			base.initInstance(i_length);
			return;
		}
		protected override Item createElement()
		{
			return new Item(this._op_interface);
		}
	}
	/**
	 * AreaのStackクラス
	 *
	 */
	private class AreaStack : NyARPointerStack<Item>
	{
		public AreaStack(int i_length)
		{
			base.initInstance(i_length);
		}
	}
	/**
	 * 元
	 */
	private AreaPool _pool;
	private AreaStack _stack;

	public LowResolutionLabelingSamplerOut(int i_length)
	{
		this._pool=new AreaPool(i_length);
		this._stack=new AreaStack(i_length);
		return;
	}
	/**
	 * Samplerが使う関数です。ユーザは通常使用しません。
	 * SamplerOutの内容を初期状態にします。
	 * @param i_source
	 */
	public void initializeParams()
	{
		//基準ラスタの設定
		
		Item[] items=this._stack.getArray();
		//スタック内容の初期化
		for(int i=this._stack.getLength()-1;i>=0;i--){
			items[i].releaseObject();
			items[i]=null;
		}
		//スタックをクリア
		this._stack.clear();
	}
	public Item prePush()
	{
		Item result=this._pool.newObject();
		if(result==null){
			return null;
		}
		if(this._stack.push(result)==null){
			result.releaseObject();
			return null;
		}
		return result;
		
	}
	/**
	 * 検出したエリアデータの配列を返します。
	 * @return
	 */
	public Item[] getArray()
	{
		return this._stack.getArray();
	}
	/**
	 * 検出したエリアデータの総数を返します。
	 * @return
	 */
	public int getLength()
	{
		return this._stack.getLength();
	}
}
}