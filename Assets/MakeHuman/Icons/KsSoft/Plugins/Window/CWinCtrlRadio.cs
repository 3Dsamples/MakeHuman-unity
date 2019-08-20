//==============================================================================================
/*!?チェックボックスコントロール.
	@file  CWinCtrlCheckbox
	
	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KS {
    public class CWinCtrlRadio : CWinCtrlCheckbox {
        //==========================================================================
        /*!Constructor
            @brief	Constructor
        */
        public CWinCtrlRadio(CWindowBase cWindow) : base(cWindow, e_WinCtrlKind.RADIO) {
            m_aTexture[0].partId = FiveCC.Id("BTN0?");
        }
        //==========================================================================
        /*!Constructor
            @brief	Constructor
        */
        public CWinCtrlRadio(CWinCtrlRadio src, CWinContents parent) : base(src, parent) {
        }
        //==========================================================================
        /*!ヒットを実行する.
            @brief	procHit
        */
        override public void procHit(CWindowMgr cWindowMgr, CInput.e_State eState) {
            procHit(ref m_tProcHit, cWindowMgr, eState, true);
        }
        //==========================================================================
        /*!チェック状態を切り替える.
            @brief	check
        */
        override protected void setCheckState(bool bState) {
            // グループ化されているラジオボタンを解除.	
            if (m_aGroup != null) {
                foreach (CWinCtrlBase cCtrl in m_aGroup) {
                    if (cCtrl is CWinCtrlRadio) {
                        CWinCtrlRadio cRadio = cCtrl as CWinCtrlRadio;
                        cRadio.m_bCheckState = !bState;
                    }
                }
            }
            m_bCheckState = bState;
        }
    }
}

