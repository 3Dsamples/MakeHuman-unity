//==============================================================================================
/*!アセットバンドルデータを受け取り初期化するインターフェースを持つ.
	@file  IManager	
*/
//==============================================================================================
using UnityEngine;


namespace KS {
    //==========================================================================
    /*!
        @brief	IManager
    */
    public interface IManager {
        bool initialize(CAssetBundle[] aAssetBundles);
        void release();
        uint[] getAssetBundleIds();
    };
}
