//==============================================================================================
/*!?ラベルコントロール.
	@file  CWinCtrlIcon

	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KS {
    public class CWinCtrlIcon : CWinCtrlTexture {
        //==========================================================================
        /*!Constructor
            @brief	Constructor
        */
        public CWinCtrlIcon(CWindowBase cWindowBase, e_WinCtrlKind eKind = e_WinCtrlKind.ICON) : base(cWindowBase, eKind) {
            m_aTexture[0].partId = FiveCC.Id("ICON");
            m_cTextOne = new CWinTextOne();
            m_bNohit = false;
            SEId = CWindowMgr.Instance.clickSE;
        }
        //==========================================================================
        /*!Copy Constructor
            @brief	Copy Constructor
        */
        public CWinCtrlIcon(CWinCtrlIcon src, CWinContents parent) : base(src, parent) {
        }
        //==========================================================================
        /*!Render
            @brief	render
        */
        override public void render(Vector3 pos, CWindowMesh cWinMesh, CWinTextMesh cTextMesh, WinColor rtcolor, ClipRect cr) {
            WinColor col = rtcolor;
            if (m_tProcHit.tap) {
                col = rtcolor * 0.5f;
            }
            base.render(pos, cWinMesh, cTextMesh, col, cr);

            col = rtcolor * captionColor;
            cTextMesh.request(m_cTextOne, pos, captionOffset, textAnchor, col, size, cr);
        }
        //==========================================================================
        /*!キャプションパーツを再更新する必要があるとき呼ばれる.
            @brief	onResetCaption
        */
        override public void onResetCaption() {
            m_cTextOne.style = textStyle;
            m_cTextOne.anchor = textAnchor;
            m_cTextOne.text = caption;
            m_eUpdateFlag &= ~e_UpdateFlag.CAPTION;
        }
        public CInput.e_State state {
            get {
                return m_tProcHit.state;
            }
        }
    }
}
