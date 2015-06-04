# NyARToolkitCS


Copyright (C)2008-2012 Ryo Iizuka

http://nyatla.jp/nyartoolkit/  
airmail(at)ebony.plala.or.jp  
wm(at)nyatla.jp


## NyARToolkit

* NyARToolkitCSは、NyARToolKit for JavaのAPIを基盤としたARアプリケーション向けのクラスライブラリです。
* .Net Framework 2.0以上に対応しています。
* ライブラリの構成は、ARToolKitの基本機能と、NyARToolKitオリジナルの拡張機能、アプリケーション向けのフレームワークです。
* ライブラリは、NyARTookitを純粋に移植したNyARToolkitCS、NyARToolkitのRPF(Reality Platform)クラスのあるNyARToolkitCS.rpf,C#向けの拡張クラスのある、NyARToolkitCSUtils,サンプルで構成されています。
* このSDKが提供する3Dレンダラアダプタは、Managed Direct3Dのみです。
* sampleモジュールは、いくつかの動作チェックプログラムと、RPFを使ったサンプルアプリケーションがあります。

 ARToolKitについては、下記のURLをご覧ください。
 http://www.hitl.washington.edu/artoolkit/

##特徴

NyARToolkitCSの特徴を紹介します。

* System.Drawing.Bitmapから、NyARToolkit内部形式への変換をサポートしています。
* ロジックレベルでは、ARToolKitよりも高速です。
* NyId規格のIDマーカが使用できます。
* 複数のAR/NyIdマーカを容易に利用する為のMarkerSystemクラスがあります。
* ManagedDirect3d向けの簡易なスケッチシステムがあります。
* Bitmapクラスと互換性のあるAPIがあります。PNG画像をそのままマーカイメージにしたり、撮影画像の一部を切り出す機能があります。

## インストール
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
64bit環境でコンパイルした場合、例外が発生し、「xxxは有効な Win32 アプリケーションではありません。」とメッセージと共に、プログラムが停止
する事があります。
この現象は、プロジェクトのプラットフォームターゲットを、x86にすることで解決します。

## 外部ライブラリ

#### DirectShowLibNET

NyARToolkitCSの.Net Framework版では、カメラ映像の取得にDirectShowLibNET（ライセンスはLGPL）を使用します。このライブラリはextlib/DirectShowLibV2フォルダにあります。

DirectShowLibNETは、こちらのURLからダウンロードできます。  
http://sourceforge.net/projects/directshownet/

#### NyWMCapture

NyARToolkitCSの.Net Compact Framework版では、カメラ映像の取得にNyWMCapture（ライセンスはMIT）を使用します。
このライブラリはextlib/NyWMCaptureフォルダにあります。

このモジュールをモバイルデバイスにインストールする方法は、NyWMCaptureのreadme.ja.txtを参照してください。


## プロジェクトの概要


### forFW2.0

##### NyARToolkitCS
NyARToolkitライブラリの本体です。基本的は.Net Framework 2.0規格のコードです。依存する外部ライブラリはありません。 

##### NyARToolkitCS.markersystem
NyARToolkit/4.0で追加したNyARToolkitライブラリのmarkerSystemモジュールです。
依存する外部ライブラリはありません。 
 
##### NyARToolkitCS.rpf
NyARToolkitライブラリのRPFモジュールです。NyARToolkit/3.0で追加したRPFモジュールのコードです。
依存する外部ライブラリはありません。 
 
##### NyARToolkitCSUtils
.NetFrameworkに依存するコードをまとめたプロジェクトです。
ManagedDirectX向けのコードと、DirectShow向けのクラスがあります。
ManagedDirectXと、DirectShowNetに依存します。

##### NyARToolkitCS.sandbox
実験的なコードをまとめたプロジェクトです。
このプロジェクトはコンパイルできないかもしれません。
 
##### Sample
サンプルプログラムです。
  
#####  test
ライブラリの動作チェックプログラムがあります。サンプルとしては役に立ちません。
* Sample/CaptureTest  
カメラキャプチャの実験プログラムです。DirectShowNetに依存します。
* Sample/RawTest  
NyARToolkitのベンチマークプログラムです。静止画に1000回マーカ検出処理をして、処理時間を計測します。

##### old
NyARToolkit3.0.0以前のサンプルがあります。
* SimpleLite_ImageSource  
静止画からマーカ検出します。NyARSingleDetectMarkerのサンプルです。
* SimpleLiteDirect3d  
ManagedDirect3Dを出力先とする、カメラ画像の上に立方体を表示するプログラムです。
NyARSingleDetectMarkerクラスのサンプルです。
* SingleARMarkerDirect3d  
ManagedDirect3Dを出力先とする、カメラ画像のの認識したマーカにマーカ番号を表示するプログラムです。
SingleARMarkerProcesserクラスのサンプルです。
* SingleNyIdMarkerDirect3d
ManagedDirect3Dを出力先とする、カメラ画像のの認識したマーカに、Idマーカ番号を表示するプログラムです。
SingleNyIdMarkerProcesserのサンプルプログラムです。



##### rpf
NyARToolkit3.0.0の、RPFモジュールを使ったサンプルがあります。
特にこだわりがなければMarkerSystemを使ってください。

* Test_NyARRealityD3d_ARMarker
RPFを使ったARマーカ認識プログラムです。最大２個のマーカを同時に認識します。
NyARRealityD3dと、ARTKMarkerTableのサンプルプログラムでもあります。
 
* Test_NyARRealityD3d_IdMarker
RPFを使ったARマーカ認識プログラムです。最大２個のマーカを同時に認識します。
NyARRealityD3dと、RawbitSerialIdTableのサンプルプログラムでもあります。


##### sketch
NyARToolkit4.0.0から追加した、簡易スケッチシステムを使ったサンプルです。
他のサンプルよりもコードが短縮されています。

* SimpleLite  
SimpleLiteDirect3dを、MarkerSystemモジュールを使って書き直したものです。
* ImagePickup  
撮影画像の取得や、座標変換のサンプルです。
* MarkerPlane  
マーカ表面の座標を得るサンプルです。
* SimpleLiteM  
２つのマーカを同時に認識するサンプルです。

##### SimpleLiteForm
スケッチシステムを使わないMarkerSystemのサンプルです。 SimpleLiteと同じです。 
 



### forWM5

現在サポートしていません。

  
## NyARToolkitCS License

NyARToolkitCSは、商用ライセンスとLGPLv3のデュアルライセンスを採用しています。
LGPLv3を承諾された場合には、商用、非商用にかかわらず、無償でご利用になれます。LGPLv3を承諾できない場合には、商用ライセンスの購入をご検討ください。


* LGPLv3
LGPLv3については、COPYING.txtをお読みください。

* 商用ライセンス
商用ライセンスについては、ARToolWorks社に管理を委託しております。  
http://www.artoolworks.com/Home.html
