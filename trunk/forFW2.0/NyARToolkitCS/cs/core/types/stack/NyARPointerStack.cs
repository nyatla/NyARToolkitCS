using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace jp.nyatla.nyartoolkit.cs.core
{
    /**
     * 実体をもたない可変長配列です。
     * このクラスは実体化できません。継承して使います。
     *
     * @param <T>
     */
    public class NyARPointerStack<T>
    {
	    protected T[] _items;
	    protected int _length;
    	
	    /**
	     * このクラスは実体化できません。
	     * @throws NyARException
	     */
	    protected NyARPointerStack()
	    {
	    }

	    /**
	     * スタックのメンバ変数を初期化します。この関数は、このクラスを継承したクラスを公開するときに、コンストラクタから呼び出します。
	     * @param i_length
	     * @param i_element_type
	     * @throws NyARException
	     */
	    
	    protected virtual void initInstance(int i_length)
	    {
		    //領域確保
		    this._items = new T[i_length];
		    //使用中個数をリセット
		    this._length = 0;
		    return;		
	    }

	    /**
	     * スタックに参照を積みます。
	     * @return
	     * 失敗するとnull
	     */
	    public virtual T push(T i_object)
	    {
		    // 必要に応じてアロケート
		    if (this._length >= this._items.Length){
			    return default(T);
		    }
		    // 使用領域を+1して、予約した領域を返す。
		    this._items[this._length]=i_object;
		    this._length++;
		    return i_object;
	    }
	    /**
	     * スタックに参照を積みます。pushとの違いは、失敗した場合にassertすることです。
	     * @param i_object
	     * @return
	     */
	    public T pushAssert(T i_object)
	    {
		    // 必要に応じてアロケート
            Debug.Assert(this._length < this._items.Length);
		    // 使用領域を+1して、予約した領域を返す。
		    this._items[this._length]=i_object;
		    this._length++;
		    return i_object;
	    }
    	
	    /** 
	     * 見かけ上の要素数を1減らして、そのオブジェクトを返します。
	     * @return
	     */
	    public virtual T pop()
	    {
		    Debug.Assert(this._length>=1);
		    this._length--;
		    return this._items[this._length];
	    }
	    /**
	     * 見かけ上の要素数をi_count個減らします。
	     * @param i_count
	     * @return
	     */
        public virtual void pops(int i_count)
	    {
		    Debug.Assert(this._length>=i_count);
		    this._length-=i_count;
		    return;
	    }	
	    /**
	     * 配列を返します。
	     * 
	     * @return
	     */
	    public T[] getArray()
	    {
		    return this._items;
	    }
	    public T getItem(int i_index)
	    {
		    return this._items[i_index];
	    }
	    /**
	     * 配列の見かけ上の要素数を返却します。
	     * @return
	     */
        public int getLength()
	    {
		    return this._length;
	    }
	    /**
	     * 指定した要素を削除します。
	     * 削除した要素は前方詰めで詰められます。
	     */
	    public virtual void remove(int i_index)
	    {
		    Debug.Assert(this._length>i_index && i_index>=0);
    		
		    if(i_index!=this._length-1){
			    int i;
			    int len=this._length-1;
			    T[] items=this._items;
			    for(i=i_index;i<len;i++)
			    {
				    items[i]=items[i+1];
			    }
		    }
		    this._length--;
	    }
	    /**
	     * 指定した要素を順序を無視して削除します。
	     * @param i_index
	     */
	    public virtual void removeIgnoreOrder(int i_index)
	    {
            Debug.Assert(this._length > i_index && i_index >= 0);
		    //値の交換
		    if(i_index!=this._length-1){
			    this._items[i_index]=this._items[this._length-1];
		    }
		    this._length--;
	    }
	    /**
	     * 見かけ上の要素数をリセットします。
	     */
	    public void clear()
	    {
		    this._length = 0;
	    }
    }
}
