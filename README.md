# NyARToolkitCS

version 5.0.8
Copyright (C)2008-2016 Ryo Iizuka

http://nyatla.jp/nyartoolkit/  
airmail(at)ebony.plala.or.jp  
wm(at)nyatla.jp


## NyARToolkit

* NyARToolkitCSは、NyARToolKit 5.0.8をC#で書き直したARアプリケーション向けのクラスライブラリです。* ARマーカ、IDマーカ(NyIDマーカ)、NFTターゲットを扱うことができます。
* .Net Framework 2.0以上に対応しています。
* キャプチャライブラリにはDirectShowLibNET,レンダリングシステムにはManaged Direct3Dを使うことができます。
* System.Drawing.Bitmapと互換性のあるAPIがあり、.NETフレームワークと相互に運用ができます。
* ManagedDirect3d向けの簡易なスケッチシステムがあります。

 ARToolKitについては、下記のURLをご覧ください。
 http://www.hitl.washington.edu/artoolkit/


## ディレクトリ

* data  
プログラム以外のマーカファイル、サンプルカメラパラメータファイル、画像データがあります。サンプルプログラムが使うデータファイルはすべてこのディレクトリの中にあります。
* extlib  
外部ライブラリやツールプログラムがあります。NFTのマーカファイルジェネレータはここにあります。
* forFW2.0  
.NetFramework用のソリューションファイル・プロジェクトファイルがあります。
* forWM5  
.NetCompactFramework用のソリューションファイルの残骸です。
* lib  
NyARToolKitCSのソースファイルがここにあります。


## セットアップ

Visual Stadioのソリューションファイルがありますので開いてください。

開発環境は以下のものが必要です。
* .Net Framefork  
Visual Stadio 2013 Express以上

各環境向けのソリューションファイルは、以下の場所にあります。
* .Net Framefork  
forFW2.0
* .Net Framefork  
forWM5 現在サポートしていません

(注意)  
64bit環境でコンパイルした場合、例外が発生し、「xxxは有効な Win32 アプリケーションではありません。」とメッセージと共に、プログラムが停止する事があります。この現象は、プロジェクトのプラットフォームターゲットを、x86にすることで解決します。


## 外部ライブラリとツール

#### DirectShowLibNET

NyARToolkitCSの.Net Framework版では、カメラ映像の取得にDirectShowLibNET（ライセンスはLGPL）を使用します。このライブラリはextlib/DirectShowLibV2フォルダにあります。

DirectShowLibNETは、こちらのURLからダウンロードできます。  
http://sourceforge.net/projects/directshownet/

#### NyWMCapture

NyARToolkitCSの.Net Compact Framework版では、カメラ映像の取得にNyWMCapture（ライセンスはMIT）を使用します。
このライブラリはextlib/NyWMCaptureフォルダにあります。

このモジュールをモバイルデバイスにインストールする方法は、NyWMCaptureのreadme.ja.txtを参照してください。

#### tools
NftFileGenerator - 画像ファイルからNFTターゲットデータを作るツールです。実行にはJavaが必要です。


## プロジェクトの概要

プロジェクトの概要を説明します。


### forFW2.0

#### NyARToolkitCS
NyARToolkitライブラリの本体です。基本的は.Net Framework 2.0規格のコードです。依存する外部ライブラリはありません。 
#### NyARToolkitCS.markersystem
NyARToolkitライブラリのMarkerSystemに相当するモジュールです。依存する外部ライブラリはありません。 

#### NyARToolkitCSUtils
.NetFrameworkに依存するコードをまとめたプロジェクトです。ManagedDirectX向けのコードと、DirectShow向けのクラスがあります。ManagedDirectXと、DirectShowNetに依存します。

#### NyARToolkitCS.sandbox
実験的なコードをまとめたプロジェクトです。このプロジェクトはコンパイルできないかもしれません。
 
#### Sample
サンプルプログラムです。
##### test
ライブラリの動作チェックプログラムがあります。サンプルとしては役に立ちません。

###### Sample/CaptureTest
DirectShowNetの動作チェックプログラムです。
 
###### Sample/RawTest
 NyARToolkitのベンチマークプログラムです。静止画に1000回マーカ検出処理をして、処理時間を計測します。


##### old
スケッチシステムを使わない、NyARToolkit3.0.0以前のサンプルがあります。

###### SimpleLite_ImageSource
静止画からマーカ検出をするサンプルプログラムです。

###### SimpleLiteDirect3d
ManagedDirect3Dを出力先とする、カメラ画像の上に立方体を表示するプログラムです。
 
###### SingleARMarkerDirect3d
ManagedDirect3Dを出力先とする、カメラ画像のの認識したマーカのに、マーカ番号を表示するプログラムです。
 
###### SingleNyIdMarkerDirect3d
ManagedDirect3Dを出力先とする、カメラ画像のの認識したマーカのに、Idマーカ番号を表示するプログラムです。


##### sketch
簡易スケッチシステムを使ったサンプルです。
###### SimpleLite
SimpleLiteDirect3dを、MarkerSystemモジュールを使って書き直したものです。
###### ImagePickup
撮影画像の取得や、座標変換のサンプルです。
###### MarkerPlane
マーカ表面の座標を得るサンプルです。
###### SimpleLiteM
２つのマーカを同時に認識するサンプルです。
###### SimpleNft
NFTのサンプルです。マーカの代わりに、自然特徴点画像を認識します。画像には、Data/infiniticat.pdfを使ってください。

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
