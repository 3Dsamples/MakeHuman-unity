//==============================================================================================
/*!?テキストコントロール.
	@file  CWinCtrlText
	
	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KS {
    public class CWinCtrlText : CWinCtrlTextBase {
        //==========================================================================
        /*!Constructor
            @brief	Constructor
        */
        public CWinCtrlText(CWindowBase cWindowBase) : base(cWindowBase, e_WinCtrlKind.TEXT) {
            m_cTextOne = new CWinTextOne();
        }
        //==========================================================================
        /*!Copy Constructor
            @brief	Copy Constructor
        */
        public CWinCtrlText(CWinCtrlText src, CWinContents parent) : base(src, parent) {
        }
    }
}
