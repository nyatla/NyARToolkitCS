using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
namespace jp.nyatla.nyartoolkit.cs.core
{
    /**
     * このクラスは、型Tのオブジェクトプールを提供します。
     *
     * @param <T>
     */
    public class NyARObjectPool<T>
    {
	    protected T[] _buffer;
	    protected T[] _pool;
	    protected int _pool_stock;

	    /**
	     * オブジェクトプールからオブジェクトを取り出します。
	     * @return
	     */
	    public T newObject()
	    {
		    if(this._pool_stock<1){
			    return default(T);
		    }
		    this._pool_stock--;
		    return this._pool[this._pool_stock];
    		
	    }
	    /**
	     * オブジェクトプールへオブジェクトを返却します。
	     * @return
	     */
	    public void deleteObject(T i_object)
	    {
		    Debug.Assert(i_object!=null);
            Debug.Assert(this._pool_stock < this._pool.Length);
		    //自身の提供したオブジェクトかを確認するのは省略。
		    this._pool[this._pool_stock]=i_object;
		    this._pool_stock++;
	    }

	    /**
	     * このクラスは実体化できません。
	     * @throws NyARException
	     */
	    public NyARObjectPool()
	    {
	    }
	    /**
	     * オブジェクトを初期化します。この関数は、このクラスを継承したクラスを公開するときに、コンストラクタから呼び出します。
	     * @param i_length
	     * @param i_element_type
	     * @throws NyARException
	     */
	    protected void initInstance(int i_length)
	    {
		    //領域確保
		    this._buffer = new T[i_length];
		    this._pool = new T[i_length];
		    //使用中個数をリセット
		    this._pool_stock=i_length;
		    //オブジェクトを作成
            for (int i = this._pool.Length - 1; i >= 0; i--)
		    {
			    this._buffer[i]=this._pool[i]=createElement();
		    }
		    return;		
	    }
	    /**
	     * オブジェクトを作成します。
	     * @return
	     * @throws NyARException
	     */
	    protected T createElement()
	    {
		    throw new NyARException();
	    }
    	
    }
}
