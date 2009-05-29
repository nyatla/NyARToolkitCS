ARToolkit C# class library NyARToolkitCS.
Copyright (C)2008-2009 R.Iizuka

version 2.3.0

http://nyatla.jp/
airmail(at)ebony.plala.or.jp
--------------------------------------------------


・NyARToolkitCS

NyARToolkitCSは、純粋なC#のみで実装したNyARToolkit互換の
クラスライブラリです。

ARToolkit 2.72.1及び、NyARToolkit version 2.3.0
をベースにしています。

※NyARToolkitのcore2パッケージの一部は実装されていません。
　今後徐々に実装する予定です。


ARToolkitは加藤博一先生とHuman Interface Technology Labにより
開発されたAugmented Reality (AR) ライブラリです。
詳しくはこちらをご覧下さい。
http://www.hitl.washington.edu/artoolkit/

NyARToolkitはARToolkit 2.72.1をベースに開発された、ARToolKit互換の
演算結果を持つ、Javaクラスライブラリです。
詳しくはこちらをご覧下さい。
http://nyatla.jp/nyartoolkit/


・動作/開発環境

NyARToolkitCS
 .Net Framework 2.0以上が必要です。
 コンパイルには、VisualC# 2008 Express Edition以上が必要です。

NyARToolkitCSforWM5
 .Net Compact Framework 3.5 以上が必要です。
 コンパイルには、VisualC# 2008 Professional Edition以上が必要です。
 ※Compact Frameworkで開発すれば、Express Editionでもコンパイル自体は出来ます。
 
・ディレクトリ構成

./data
  マーカーファイル/カメラ設定ファイルがあります。
./extlib
  外部ライブラリがあります。
  DirectShowLibV2….NET FW用のDirectshowライブラリ(LGPL)です。
  NyWMCapture    …Windows Mobile用のキャプチャライブラリです。

./forFW2.0
  .Net2.0以降用のプロジェクトです。

./forWM5
  WindowsMobile5用のプロジェクトです。




・forFW2.0

.Net Framework 2.0以降で動作するように設定したプロジェクト群です。

・・モジュール構成

+------------------------------------------------------+
|                      Application                     |
+---------------------------+--------+-----------------+
|     NyARToolkitCSUtil     |        |                 |
+-----------------+---------+        |                 |
|DirectShowLibNET |     Direct3D     |  NyARToolKitCS  |
+-----------------+------------------+                 |
|     Camera      |        3D        |                 |
-------------------------------------------------------+

NyARToolkitがNyARToolkitCSの本体です。

NyARToolkitCSUtilは、DirectShowLibNET及びDirect3DをNyARToolkitCS
から使いやすくするためのユーティリティクラス群です。

DirectShowLibNET（LGPLライセンス）は、DirectShowのマネージドクラス群です。
こちらのURLで配布されているものを使っています。
http://sourceforge.net/projects/directshownet/

3D出力部分はManaged Direct3Dをそのまま使用しています。



・・ソースコード構成

.NyARToolkitCS.slnを開くと、5つのプロジェクトがあります。

NyARToolkitCS,NyARToolkitCSUtilsはクラスライブラリ、NyARToolkitCS.sandbox
は試験中のクラスライブラリ群、他の３つはサンプルプログラムです。

NyARToolkitCS.sandbox
　固定小数点計算クラスや、結果値の互換性を無視した高速化を行った
　クラス、実験中のクラス群があります。

RawTest (./sample/RawTest)
  ライブラリのベンチマークプログラムです。固定画像にあるマーカーを
  1000回認識させ、その計算時間を表示します。

CaptureTest (./sample/CaptureTest)
  Direct3Dを使用しない、キャプチャとNyARToolKitの試験用のプログラムです。
  カメラ映像からマーカーを検出し、その計算結果を表示します。

SimpleLiteDirect3d (./sample/SimpleLiteDirect3d)
  SimpleLiteを移植したものです。
  キャプチャデバイスの設定からマーカーの検出後のDirect3Dへの出力までの、
  一連の流れを追うことが出来ます。

SingleNyIdMarkerDirect3d (./sample/SingleNyIdMarkerDirect3d)
  １種類のNyIdマーカを認識するサンプルです。



・forWM5

.Net Compact Framework 3.5以降で動作するように設定した
プロジェクト群です。

※.Net Compact Frameworkを使うように設定したプロジェクトファイル
　を用意すれば、Express Editionでもコンパイルできます。


・・モジュール構成

+------------------------------------------------------+
|                      Application                     |
+---------------------------+--------+-----------------+
|     NyARToolkitCSUtil.WM5 |        |                 |
+-----------------+---------+        |                 |
|    NyWMCapture  | Mobile  Direct3D |  NyARToolKitCS  |
+-----------------+------------------+                 |
|     Camera      |        3D        |                 |
-------------------------------------------------------+

NyARToolkitがNyARToolkitCSの本体です。.Net版との差異は、参照
するモジュールの違いと、サンプルのコメントアウト部分のみです。


NyARToolkitCSUtil.WMは、NyWMCapture及びMobile  Direct3Dを
NyARToolkitCSから使いやすくするためのユーティリティクラス群です。


NyWMCapture（MITライセンス）は、WindowsMobileのカメラ制御用の
COMオブジェクトです。

3D出力部分はManaged Direct3Dをそのまま使用しています。



・・ソースコード構成

NyARToolkitCS.WindowsMobile5.slnを開くと、5つのプロジェクトがあります。

NyARToolkitCS.WindowsMobile5,NyARToolkitCSUtils.WindowsMobile5,
NyWMCaptureはクラスライブラリ、NyARToolkitCS.WindowsMobile5.sandboxは
実験中のクラスライブラリ群、SimpleLiteDirect3d.WindowsMobile5がサンプル
プログラムです。

NyARToolkitCS.WindowsMobile5.sandboxには、固定小数点対応のクラス群があります。
モバイルデバイスでの開発では、このクラス郡を使用することにより、浮動小数点版
の約5倍の高速化が可能です。

SimpleLiteDirect3d.WindowsMobile5
  SimpleLiteをWindowsMobile用に移植したものです。
　
  ※注意※
  サンプルを実行するには、NyWMCapture.dllをシステムに登録する必要があります。

  NyWMCapture.dllをモバイルデバイスの\Windowsへコピーして、regsvrce
  等でレジストリに登録して下さい。

  手書きで登録をする場合は、以下の情報を登録してください。
  
  [HKEY_CLASSES_ROOT\CLSID\{32F37E70-B633-4253-B8E0-A99A1BBEEA84}]
  @="NyWMCapture"

  [HKEY_CLASSES_ROOT\CLSID\{32F37E70-B633-4253-B8E0-A99A1BBEEA84}\InprocServer32]
  @="NyWMCapture.dll"
  "ThreadingModel"="Both"






・NyARToolkitとの差分

NyARToolkitのクラス構造・演算結果に互換性があります。
システムクラスについては、.NETとjavaでそれぞれ異なったものを使用しています。



・ライセンス
extlib以下のものは、各モジュールのライセンスに従ってください。
NyARToolkitは、特にソースに表記の無い限り、GPLライセンスです。
詳しくはLICENCE.txtをみてください。

