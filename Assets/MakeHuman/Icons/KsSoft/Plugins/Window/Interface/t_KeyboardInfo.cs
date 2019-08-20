//==============================================================================================
/*!キーボード情報.
	@file  t_KeyboardInfo
	
	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KS {
    public struct t_KeyboardInfo {
#if UNITY_IOS || UNITY_ANDROID
	public TouchScreenKeyboardType m_type;
	public bool m_autoCorrect;
	public bool m_multiline;
	public bool m_secure;
	public bool m_alert;
	public bool m_hideInput;
	public uint	m_style;
#endif
        // The insertion point for the text
        public int m_insert;
    }
}
