======================================================================
NyARToolkitCS
 version 4.1.1
======================================================================

Copyright (C)2008-2012 Ryo Iizuka

http://nyatla.jp/nyartoolkit/
airmail(at)ebony.plala.or.jp
wm(at)nyatla.jp


----------------------------------------------------------------------
 About NyARToolkit
----------------------------------------------------------------------
 * NyARToolkitCSは、NyARToolKit 4.1.1のAPIを基盤としたARアプリケーション向けの
   クラスライブラリです。
 * .Net Framework 2.0以上に対応しています。
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
 * 複数のAR/NyIdマーカを容易に利用する為のMarkerSystemクラスがあります。
 * ManagedDirect3d向けの簡易なスケッチシステムがあります。
 * Bitmapと互換性のあるAPIがあります。PNG画像をそのままマーカイメージ
   にしたり、撮影画像の一部を切り出す機能があります。


----------------------------------------------------------------------
 NyARToolkitCS License
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
    forWM5 現在サポートしていません

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
   
 * NyARToolkitCS.markersystem
   NyARToolkit/4.0で追加したNyARToolkitライブラリのmarkerSystemモジュールです。
   依存する外部ライブラリはありません。 
   
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
 
 * Sample
  サンプルプログラムです。
  
  ** test
    ライブラリの動作チェックプログラムがあります。サンプルとしては役に立ちません。

   1.Sample/CaptureTest
     カメラキャプチャの実験プログラムです。DirectShowNetに依存します。
 
   2.Sample/RawTest
     NyARToolkitのベンチマークプログラムです。静止画に1000回マーカ検出
     処理をして、処理時間を計測します。


  ** old
    NyARToolkit3.0.0以前のサンプルがあります。

   1. SimpleLite_ImageSource
     静止画からマーカ検出をするサンプルプログラムです。
     NyARSingleDetectMarkerのサンプルプログラムでもあります。
 
   2. SimpleLiteDirect3d
     ManagedDirect3Dを出力先とする、カメラ画像の上に立方体を表示する
     プログラムです。
     NyARSingleDetectMarkerのサンプルプログラムでもあります。
 
   3. SingleARMarkerDirect3d
     ManagedDirect3Dを出力先とする、カメラ画像のの認識したマーカの
     に、マーカ番号を表示するプログラムです。
     SingleARMarkerProcesserのサンプルプログラムでもあります。
 
   4. SingleNyIdMarkerDirect3d
     ManagedDirect3Dを出力先とする、カメラ画像のの認識したマーカの
     に、Idマーカ番号を表示するプログラムです。
     SingleNyIdMarkerProcesserのサンプルプログラムでもあります。



  ** rpf
   NyARToolkit3.0.0の、RPFモジュールを使ったサンプルがあります。特にこだわりがなければ、
   MarkerSystemのほうが使いやすいので、そちらを使ってください。

   1. Test_NyARRealityD3d_ARMarker
     RPFを使ったARマーカ認識プログラムです。最大２個のマーカを同時に認識します。
     NyARRealityD3dと、ARTKMarkerTableのサンプルプログラムでもあります。
 
   2. Test_NyARRealityD3d_IdMarker
     RPFを使ったARマーカ認識プログラムです。最大２個のマーカを同時に認識します。
     NyARRealityD3dと、RawbitSerialIdTableのサンプルプログラムでもあります。



   
  ** sketch
   NyARToolkit4.0.0から追加した、簡易スケッチシステムを使ったサンプルです。
   他のサンプルよりもコードが短縮されています。

   1. SimpleLite
     SimpleLiteDirect3dを、MarkerSystemモジュールを使って書き直したものです。
   2. ImagePickup
     撮影画像の取得や、座標変換のサンプルです。
   3. MarkerPlane
     マーカ表面の座標を得るサンプルです。
   4. SimpleLiteM
     ２つのマーカを同時に認識するサンプルです。

  **SimpleLiteForm
   スケッチシステムを使わないMarkerSystemのサンプルです。 SimpleLiteと同じです。 
 



forWM5

現在サポートしていません。

  
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
