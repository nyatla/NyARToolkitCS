using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
namespace jp.nyatla.nyartoolkit.cs.core
{
    /**
     * 参照カウンタ付きのobjectPoolです。NyARManagedObjectから派生したオブジェクトを管理します。
     * このクラスは、参照カウンタ付きのオブジェクト型Tのオブジェクトプールを実現します。
     * 
     * このクラスは、NyARManagedObjectと密接に関連して動作することに注意してください。
     * 要素の作成関数はこのクラスで公開しますが、要素の解放関数は要素側に公開します。
     * @param <T>
     */
    public class NyARManagedObjectPool<T> where T:NyARManagedObject
    {
	    /**
	     * Javaの都合でバッファを所有させていますが、別にこの形で実装しなくてもかまいません。
	     */
	    public class Operator : NyARManagedObject.INyARManagedObjectPoolOperater
	    {
		    public NyARManagedObject[] _buffer;
		    public NyARManagedObject[] _pool;
		    public int _pool_stock;
		    public void deleteObject(NyARManagedObject i_object)
		    {
			    Debug.Assert(i_object!=null);
                Debug.Assert(this._pool_stock < this._pool.Length);
			    this._pool[this._pool_stock]=i_object;
			    this._pool_stock++;
		    }
	    }
	    /**
	     * 公開するオペレータオブジェクトです。
	     * このプールに所属する要素以外からは参照しないでください。
	     */
        public Operator _op_interface = new Operator();

	    /**
	     * プールから型Tのオブジェクトを割り当てて返します。
	     * @return
	     * 新しいオブジェクト
	     */
	    public T newObject()
	    {
            Operator pool = this._op_interface;
		    if(pool._pool_stock<1){
			    return null;
		    }
		    pool._pool_stock--;
		    //参照オブジェクトを作成して返す。
		    return (T)(pool._pool[pool._pool_stock].initObject());
	    }
	    /**
	     * 実体化の拒否の為に、コンストラクタを隠蔽します。
	     * 継承クラスを作成して、初期化処理を実装してください。
	     */
	    protected NyARManagedObjectPool()
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
		    Operator pool=this._op_interface;
		    //領域確保
		    pool._buffer = new T[i_length];
            pool._pool = new T[i_length];
		    //使用中個数をリセット
		    pool._pool_stock=i_length;
		    //オブジェクトを作成
            for (int i = pool._pool.Length - 1; i >= 0; i--)
		    {
			    pool._buffer[i]=pool._pool[i]=createElement();
		    }
		    return;		
	    }

	    protected void initInstance(int i_length,Object i_param)
	    {
		    Operator pool=this._op_interface;
		    //領域確保
		    pool._buffer = new T[i_length];
            pool._pool = new T[i_length];
		    //使用中個数をリセット
		    pool._pool_stock=i_length;
		    //オブジェクトを作成
            for (int i = pool._pool.Length - 1; i >= 0; i--)
		    {
			    pool._buffer[i]=pool._pool[i]=createElement(i_param);
		    }
		    return;		
	    }
	    /**
	     * オブジェクトを作成します。継承クラス内で、型Tのオブジェクトを作成して下さい。
	     * @return
	     * @throws NyARException
	     */
	    protected virtual T createElement()
	    {
		    throw new NyARException();
	    }
        protected virtual T createElement(Object i_param)
	    {
		    throw new NyARException();
	    }
    }

}
