//==============================================================================================
/*!?クローズボタン.
	@file  CWinCtrlCloseButton
	
	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KS {
    public class CWinCtrlCloseButton : CWinCtrlButton {
        //==========================================================================
        /*!Constructor
            @brief	Constructor
        */
        public CWinCtrlCloseButton(CWindowBase cWindow) : base(cWindow, e_WinCtrlKind.WINDOWCLOSEBUTTON) {
            m_aTexture[0].partId = FiveCC.Id("CLOS?");
        }
        //==========================================================================
        /*!Constructor
            @brief	Constructor
        */
        public CWinCtrlCloseButton(CWinCtrlCloseButton src, CWinContents parent) : base(src, parent) {
        }
        //==========================================================================
        /*!ヒットを実行する.
            @brief	procHit
        */
        override public void procHit(CWindowMgr cWindowMgr, CInput.e_State eState) {
            // 押されているのでコールバックを発行.
            if (eState == CInput.e_State.Release) {
                // 離す瞬間をクリックとする.
                m_cWindowBase.close();
            } else if (eState == CInput.e_State.On) {
                m_tProcHit.tap = true;
            }
        }
    }
}
