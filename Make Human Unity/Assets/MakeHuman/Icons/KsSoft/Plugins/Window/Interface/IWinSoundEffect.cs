//==============================================================================================
/*!効果音発生インターフェース.
	@file  IWinSoundEffect
	
	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KS {
    public interface IWinSoundEffect {
        void play(uint mSE);
    }
}
