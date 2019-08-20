//==============================================================================================
/*!?ラベルコントロール.
	@file  CWinCtrlLabel
	
	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KS {
    public class CWinCtrlLabel : CWinCtrlBase {
        //==========================================================================
        /*!Constructor
            @brief	Constructor
        */
        public CWinCtrlLabel(CWindowBase cWindowBase, e_WinCtrlKind eKind = e_WinCtrlKind.LABEL) : base(cWindowBase, eKind) {
            m_aTexture[0].partId = FiveCC.Id("LABEL");
            m_cTextOne = new CWinTextOne();
            m_bNohit = true;
            textAnchor = e_Anchor.Center;
        }
        //==========================================================================
        /*!Copy Constructor
            @brief	Copy Constructor
        */
        public CWinCtrlLabel(CWinCtrlLabel src, CWinContents parent) : base(src, parent) {
        }
        //==========================================================================
        /*!Render
            @brief	render
        */
        override public void render(Vector3 basepos, CWindowMesh cWinMesh, CWinTextMesh cTextMesh, WinColor rtcolor, ClipRect cr) {
            WinColor col = rtcolor * color;

            cWinMesh.request(parts, basepos, col, 0, cr);

            col = rtcolor * captionColor;
            cTextMesh.request(m_cTextOne, basepos, captionOffset, textAnchor, col, size, cr);
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
    }
}
