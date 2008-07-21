using System;
using System.Collections.Generic;
using jp.nyatla.nyartoolkit.cs.core;
using jp.nyatla.nyartoolkit.cs;
using jp.nyatla.nyartoolkit.cs.match;
using jp.nyatla.nyartoolkit.cs.raster;

namespace jp.nyatla.nyartoolkit.cs.detector
{
    /**
     * 画像からARCodeに最も一致するマーカーを1個検出し、その変換行列を計算するクラスです。
     *
     */
    public class NyARSingleDetectMarker
    {
        private const int AR_SQUARE_MAX=100;
        private bool is_continue=false;
        private NyARMatchPatt_Color_WITHOUT_PCA match_patt;
        private NyARDetectSquare square;
        private NyARSquareList square_list=new NyARSquareList(AR_SQUARE_MAX);
        private NyARCode code;
        protected NyARTransMat transmat;
        private double marker_width;
        //検出結果の保存用
        private int detected_direction;
        private double detected_confidence;
        private NyARSquare detected_square;
        private NyARColorPatt patt;

        /**
         * 検出するARCodeとカメラパラメータから、1個のARCodeを検出するNyARSingleDetectMarkerインスタンスを作ります。
         * @param i_param
         * カメラパラメータを指定します。
         * @param i_code
         * 検出するARCodeを指定します。
         * @param i_marker_width
         * ARコードの物理サイズを、ミリメートルで指定します。
         * @throws NyARException
         */
        public NyARSingleDetectMarker(NyARParam i_param,NyARCode i_code,double i_marker_width)
        {
	        //解析オブジェクトを作る
	        this.square=new NyARDetectSquare(i_param);
	        this.transmat=new NyARTransMat_O2(i_param);
	        //比較コードを保存
	        this.code=i_code;
	        this.marker_width=i_marker_width;
	        //評価パターンのホルダを作る
	        this.patt=new NyARColorPatt_O3(code.getWidth(),code.getHeight());
	        //評価器を作る。
	        this.match_patt=new NyARMatchPatt_Color_WITHOUT_PCA();	
        }
        /**
         * i_imageにマーカー検出処理を実行し、結果を記録します。
         * @param i_image
         * マーカーを検出するイメージを指定します。
         * @param i_thresh
         * 検出閾値を指定します。0～255の範囲で指定してください。
         * 通常は100～130くらいを指定します。
         * @return
         * マーカーが検出できたかを真偽値で返します。
         * @throws NyARException
         */
        public bool detectMarkerLite(NyARRaster i_image,int i_thresh)
        {
	        detected_square=null;
	        NyARSquareList l_square_list=this.square_list;
	        //スクエアコードを探す
	        square.detectSquare(i_image, i_thresh,l_square_list);
        	
	        int number_of_square=l_square_list.getSquareNum();
	        //コードは見つかった？
	        if(number_of_square<1){
	            return false;
	        }

	        //評価基準になるパターンをイメージから切り出す
	        if(!patt.pickFromRaster(i_image,l_square_list.getSquare(0))){
	            //パターンの切り出しに失敗
	            return false;
	        }
	        //パターンを評価器にセット
	        if(!this.match_patt.setPatt(patt)){
	            //計算に失敗した。
	            throw new NyARException();
	        }
	        //コードと比較する
	        match_patt.evaluate(code);
	        int square_index=0;
	        int direction=match_patt.getDirection();
	        double confidence=match_patt.getConfidence();
	        for(int i=1;i<number_of_square;i++){
	            //次のパターンを取得
	            patt.pickFromRaster(i_image,l_square_list.getSquare(i));
	            //評価器にセットする。
	            match_patt.setPatt(patt);
                    //コードと比較する
	            match_patt.evaluate(code);
	            double c2=match_patt.getConfidence();
	            if(confidence>c2){
		        continue;
	            }
	            //もっと一致するマーカーがあったぽい
	            square_index=i;
	            direction=match_patt.getDirection();
	            confidence=c2;
	        }
	        //マーカー情報を保存
	        detected_square=l_square_list.getSquare(square_index);
	        detected_direction=direction;
	        detected_confidence=confidence;
	        return true;
        }
        /**
         * 検出したマーカーの変換行列を計算して、o_resultへ値を返します。
         * 直前に実行したdetectMarkerLiteが成功していないと使えません。
         * @param o_result
         * 変換行列を受け取るオブジェクトを指定します。
         * @throws NyARException
         */    
        public void getTransmationMatrix(NyARTransMatResult o_result)
        {
            //一番一致したマーカーの位置とかその辺を計算
	        if(is_continue){
	            transmat.transMatContinue(detected_square,detected_direction,marker_width,o_result);
	        }else{
	            transmat.transMat(detected_square,detected_direction,marker_width,o_result);
	        }
	        return;
        }    
        /**
         * 検出したマーカーの一致度を返します。
         * @return
         * マーカーの一致度を返します。0～1までの値をとります。
         * 一致度が低い場合には、誤認識の可能性が高くなります。
         * @throws NyARException
         */
        public double getConfidence()
        {
	        return detected_confidence;
        }
        /**
         * 検出したマーカーの方位を返します。
         * @return
         * 0,1,2,3の何れかを返します。
         */
        public int getDirection()
        {
	        return detected_direction;
        }
        /**
         * getTransmationMatrixの計算モードを設定します。
         * 初期値はTRUEです。
         * @param i_is_continue
         * TRUEなら、transMatCont互換の計算をします。
         * FALSEなら、transMat互換の計算をします。
         */
        public void setContinueMode(bool i_is_continue)
        {
    	    this.is_continue=i_is_continue;
        }
    }

}
