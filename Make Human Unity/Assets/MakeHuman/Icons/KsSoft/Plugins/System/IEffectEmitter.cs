 //==============================================================================================
/*!エフェクトエミッターインターフェースクラス.
	@file  IEffectEmitter
	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;

namespace KS {
    //==========================================================================
    /*!IEffectEmitter
        @brief	IEffectEmitter
    */
    public interface IEffectEmitter {
        void addRefEmitter();
        void releaseEmitter();
        uint animationId {
            get;
        }
        Transform getTransform(uint fcBone);
    };
}
