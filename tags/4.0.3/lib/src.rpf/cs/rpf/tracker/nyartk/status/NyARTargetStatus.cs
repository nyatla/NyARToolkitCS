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
     * TargetStatusの基礎クラスです。TargetStatusは、ステータス毎に異なるターゲットのパラメータを格納します。
     * @note
     * ST_から始まるID値は、NyARTrackerのコンストラクタと密接に絡んでいるので、変更するときは気をつけて！
     *
     */
    public class NyARTargetStatus : NyARManagedObject
    {
	    public const int ST_IGNORE=0;
	    public const int ST_NEW=1;
	    public const int ST_RECT=2;
	    public const int ST_CONTURE=3;
	    public const int MAX_OF_ST_KIND=3;
	    protected NyARTargetStatus(INyARManagedObjectPoolOperater iRefPoolOperator)
            : base(iRefPoolOperator)
	    {
	    }
    }
}