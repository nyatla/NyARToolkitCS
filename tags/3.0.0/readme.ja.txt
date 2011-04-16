======================================================================
NyARToolkitCS
 version 3.0.0
======================================================================

Copyright (C)2008-2010 Ryo Iizuka

http://nyatla.jp/nyartoolkit/
airmail(at)ebony.plala.or.jp
wm(at)nyatla.jp


----------------------------------------------------------------------
 About NyARToolkit
----------------------------------------------------------------------
 * NyARToolkitCSは、NyARToolKit 3.0.0のAPIを基盤としたARアプリケーション向けの
   クラスライブラリです。
 * .Net Framework 2.0以上と、.net Compact Framework 2.0以上に対応しています。
 * ライブラリの構成は、ARToolKitの基本機能と、NyARToolKitオリジナルの拡張機能、
   アプリケーション向けのフレームワークです。
 * ライブラリは、NyARTookitを純粋に移植したNyARToolkitCS、NyARToolkitの
   RPF(Reality Platform)クラスのあるNyARToolkitCS.rpf,C#向けの拡張クラスのある、
   NyARToolkitCSUtils,サンプルで構成されています。
 * このSDKが提供する3Dレンダラアダプタは、Managed Direct3Dのみです。他の3Dレンダラ
   アダプタに対応するときの参考にして下さい。
 * sampleモジュールは、いくつかの動作チェックプログラムと、RPFを使ったサンプルアプ
   リケーションがあります。

 ARToolKitについては、下記のURLをご覧ください。
 http://www.hitl.washington.edu/artoolkit/


----------------------------------------------------------------------
NyARToolkitCSの特徴
----------------------------------------------------------------------
NyARToolkitCSの特徴を紹介します。

 * System.Drawing.Bitmapから、NyARToolkit内部形式への変換をサポートしています。
 * ロジックレベルでは、ARToolKitよりも高速です。
 * 次の項目について、高速な機能が利用できます。(ラべリング、姿勢最適化、
   画像処理、行列計算、方程式計算)
 * NyId規格のIDマーカが使用できます。
 * RPF(RealityPlatform - マーカ状態管理システム)が利用できます。

(注意)
 * RPFの不具合の為、RPFを使用してアプリケーションを作成すると、小さなマーカ
   の認識率が低くなります。修正まで、しばらくお待ちください。
   

----------------------------------------------------------------------
 NyARToolkitAS3 License
----------------------------------------------------------------------


NyARToolkitCSは、商用ライセンスとGPLv3以降のデュアルライセンスを採用して
います。

GPLv3を承諾された場合には、商用、非商用にかかわらず、無償でご利用にな
れます。GPLv3を承諾できない場合には、商用ライセンスの購入をご検討くだ
さい。


 * GPLv3
   GPLv3については、LICENCE.txtをお読みください。

 * 商用ライセンス(日本国)
   株式会社 エム・ソフトにお問い合わせください。
   http://www.msoft.co.jp/pressrelease/press090928-1.html

 * 商用ライセンス(その他の国)
   商用ライセンスについては、ARToolWorks社に管理を委託しております。
   http://www.artoolworks.com/Home.html

----------------------------------------------------------------------
 インストール
----------------------------------------------------------------------
開発環境は、それぞれ、以下のものが必要です。
 * .Net Framefork
	Visual Stadio 2008 Express以上
 * .Net Compact Framefork
	Visual Stadio 2008 Professional以上
	(注)Express Editionで、Compact Frameworkで開発すればコンパイル自体は出来ます。


それぞれの環境で使用できるソリューションファイルが以下の場所にあります。
 * .Net Framefork
    forFW2.0
 * .Net Framefork
    forWM5

(注意)
 64bit環境でコンパイルした場合、例外が発生し、「は有効な Win32 アプ
 リケーションではありません。」とメッセージと共に、プログラムが停止
 する事があります。この現象は、プロジェクトのプラットフォームターゲット
 を、x86にすることで解決します。

----------------------------------------------------------------------
 外部ライブラリ
----------------------------------------------------------------------

*DirectShowLibNET

 NyARToolkitCSの.Net Framework版では、カメラ映像の取得にDirectShowLibNET
 （ライセンスはLGPL）を使用します。このライブラリはextlib/DirectShowLibV2
 フォルダにあります。

 DirectShowLibNETは、こちらのURLからダウンロードできます。
 http://sourceforge.net/projects/directshownet/

*NyWMCapture

 NyARToolkitCSの.Net Compact Framework版では、カメラ映像の取得にNyWMCapture
 （ライセンスはMIT）を使用します。このライブラリはextlib/NyWMCapture
 フォルダにあります。

 このモジュールをモバイルデバイスにインストールする方法は、NyWMCaptureの
 readme.ja.txtを参照してください。

----------------------------------------------------------------------
 プロジェクトの概要
----------------------------------------------------------------------
プロジェクトの概要を説明します。


forFW2.0

 * NyARToolkitCS
   NyARToolkitライブラリの本体です。基本的は.Net Framework 2.0規格の
   コードです。依存する外部ライブラリはありません。 
   
 * NyARToolkitCS.rpf
   NyARToolkitライブラリのRPFモジュールです。NyARToolkit/3.0で追加した
   RPFモジュールのコードです。依存する外部ライブラリはありません。 
   
 * NyARToolkitCSUtils
  .NetFrameworkに依存するコードをまとめたプロジェクトです。
  ManagedDirectX向けのコードと、DirectShow向けのクラスがあります。
  ManagedDirectXと、DirectShowNetに依存します。

 * NyARToolkitCS.sandbox
  実験的なコードをまとめたプロジェクトです。
  このプロジェクトはコンパイルできないかもしれません。
  
 * Sample/CaptureTest
  カメラキャプチャの実験プログラムです。DirectShowNetに依存します。
 
 * Sample/RawTest
  NyARToolkitのベンチマークプログラムです。静止画に1000回マーカ検出
  処理をして、処理時間を計測します。
 
 * Sample/SimpleLite_ImageSource
  静止画からマーカ検出をするサンプルプログラムです。
  NyARSingleDetectMarkerのサンプルプログラムでもあります。
 
 * Sample/SimpleLiteDirect3d
  ManagedDirect3Dを出力先とする、カメラ画像の上に立方体を表示する
  プログラムです。
  NyARSingleDetectMarkerのサンプルプログラムでもあります。
 
 * Sample/SingleARMarkerDirect3d
  ManagedDirect3Dを出力先とする、カメラ画像のの認識したマーカの
  に、マーカ番号を表示するプログラムです。
  SingleARMarkerProcesserのサンプルプログラムでもあります。
 
 * Sample/SingleNyIdMarkerDirect3d
  ManagedDirect3Dを出力先とする、カメラ画像のの認識したマーカの
  に、Idマーカ番号を表示するプログラムです。
  SingleNyIdMarkerProcesserのサンプルプログラムでもあります。

 * Sample/Test_NyARRealityD3d_ARMarker
  RPFを使ったARマーカ認識プログラムです。最大２個のマーカを同時に認識します。
  NyARRealityD3dと、ARTKMarkerTableのサンプルプログラムでもあります。
 
 * Sample/Test_NyARRealityD3d_IdMarker
  RPFを使ったARマーカ認識プログラムです。最大２個のマーカを同時に認識します。
  NyARRealityD3dと、RawbitSerialIdTableのサンプルプログラムでもあります。

forWM5

 * NyARToolkitCS.WindowsMobile5
   NyARToolkitライブラリの本体です。基本的は.Net Compact Framework 2.0規格の
   コードです。forFW2.0との差はありません。
   
 * NyARToolkitCS.rpf.WindowsMobile5
   NyARToolkitライブラリのRPFモジュールです。NyARToolkit/3.0で追加した
   RPFモジュールのコードです。forFW2.0との差はありません。
   
 * NyARToolkitCSUtils.WindowsMobile5
  .Net Compact Frameworkに依存するコードをまとめたプロジェクトです。
  MobileManagedDirectX向けのコードと、NyWMCapture向けのクラスがあります。
  ManagedDirectXと、NyWMCaptureに依存します。

 * NyARToolkitCS.sandbox.WindowsMobile5
  実験的なコードをまとめたプロジェクトです。
  このプロジェクトはコンパイルできないかもしれません。

 * NyWMCaptureCS
  WindowsMobile用のキャプチャライブラリNyWMCaptureのC#インタフェイス
  です。

 * Sample/NyARToolkitCS.WM5.RPF
  WindowsMobile用の、Test_NyARRealityD3d_ARMarker相当のサンプルプログラムです。

 * Sample/RawTest
  WindowsMobile用の、RawTest相当のサンプルプログラムです。
  ただし、計測回数は10回です。

 * Sample/SimpleLiteDirect3d.WindowsMobile5
  WindowsMobile用の、Sample/SingleNyIdMarkerDirect3d相当のサンプルプログラムです。

  
----------------------------------------------------------------------
 既知の不具合
----------------------------------------------------------------------
 1.RPFの姿勢フィードバックが未実装。
 2.RPFの輪郭線抽出系のノイズ処理が最小二乗法の為、遅延が大きい。
 3.RPFの輪郭線抽出系のアルゴリズム不備で輪郭線のドリフトが発生し、トラッキング
   性能が低下する。
 4.RPFの二次元系追跡機能の性能が低い。
 5.RPFのエッジ抽出が、ブラーに弱い。
 6.RPFの初期検出性能が、旧NyARToolkit系と比較して、低い。

----------------------------------------------------------------------
 Special thanks
----------------------------------------------------------------------
加藤博一先生 (Hirokazu Kato, Ph. D.)
 http://www.hitl.washington.edu/artoolkit/

Prof. Mark Billinghurst
 http://www.hitlabnz.org/
