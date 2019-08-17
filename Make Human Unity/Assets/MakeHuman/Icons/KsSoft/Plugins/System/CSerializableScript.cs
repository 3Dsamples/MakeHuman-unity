//==============================================================================================
/*!シリアライズされたバイナリデータを保持するScriptableObject.
	@file  CSerializable Script
	
	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KS {
    public class CSerializableScript : ScriptableObject {
        public byte[] m_buffer;
    }

}
