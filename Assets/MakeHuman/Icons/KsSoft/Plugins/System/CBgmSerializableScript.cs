//==============================================================================================
/*!BGM用のシリアライズされたバイナリデータを保持するScriptableObject.
	@file  CBgmS Serializable Script
	
	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KS {
    public class CBgmSerializableScript : CSerializableScript {
        public AudioClip m_intro;
        public AudioClip m_loop;
    }
}
