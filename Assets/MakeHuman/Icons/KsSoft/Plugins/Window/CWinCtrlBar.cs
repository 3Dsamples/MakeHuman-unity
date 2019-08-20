//==============================================================================================
/*!?ウィンドウバーをレンダリングする.
	@file  CWinCtrlBar
		
	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KS {
    public class CWinCtrlBar : CWinCtrlLabel {
        //==========================================================================
        /*!Constructor
            @brief	Constructor
        */
        public CWinCtrlBar(CWindowBase cWindowBase) : base(cWindowBase, e_WinCtrlKind.BAR) {
            m_aTexture[0].partId = FiveCC.Id("BAR");
            textAnchor = e_Anchor.LeftCenter;
            m_bNohit = false;
        }
        //==========================================================================
        /*!Copy Constructor
            @brief	Copy Constructor
        */
        public CWinCtrlBar(CWinCtrlBar src, CWinContents parent) : base(src, parent) {
        }
    }
}
