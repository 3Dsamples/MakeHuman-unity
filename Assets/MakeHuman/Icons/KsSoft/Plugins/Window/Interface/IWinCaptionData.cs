//==============================================================================================
/*!キャプションデータインターフェース.
	@file  IWinCaptionData
	
	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KS {
    public interface IWinCaptionData {
        string find(uint mCaptionId);
    }
}
