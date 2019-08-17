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
    public class CWinCtrlTextBase : CWinCtrlBase {
        //==========================================================================
        /*!Constructor
            @brief	Constructor
        */
        public CWinCtrlTextBase(CWindowBase cWindowBase, e_WinCtrlKind eKind) : base(cWindowBase, eKind) {
            m_bNohit = true;
        }
        //==========================================================================
        /*!Copy Constructor
            @brief	Copy Constructor
        */
        public CWinCtrlTextBase(CWinCtrlTextBase src, CWinContents parent) : base(src, parent) {
        }
        //==========================================================================
        /*!Render
            @brief	render
        */
        override public void render(Vector3 pos, CWindowMesh cWinMesh, CWinTextMesh cTextMesh, WinColor color, ClipRect cr) {
            if (m_cTextOne == null || m_cTextOne.Length == 0) {
                return;
            }
            color = color * captionColor;
            if ((style & e_WinCtrlStyle.TEXT_AUTOSCALE) != 0 && m_cTextOne.width > contentsSize.x) {
                if (contentsSize.x == 0) {
                    Debug.LogError("contents size is zero:TEXT_AUTOSCALE");
                    return;
                }
                Vector2 scale;
                scale.x = contentsSize.x / m_cTextOne.width;
                scale.y = scale.x;
                cTextMesh.request(m_cTextOne, pos, captionOffset, scale, e_Anchor.LeftTop, color, Vector2.zero, cr);
            } else {
                cTextMesh.request(m_cTextOne, pos, captionOffset, e_Anchor.LeftTop, color, Vector2.zero, cr);
            }
        }
        //==========================================================================
        /*!パーツを再更新する必要があるとき呼ばれる.
            @brief	onResetParts
        */
        override public void onResetParts(int iTexIndex) {
        }
        //==========================================================================
        /*!キャプションパーツを再更新する必要があるとき呼ばれる.
            @brief	onResetCaption
        */
        override public void onResetCaption() {
            m_cTextOne.style = textStyle;
            m_cTextOne.anchor = textAnchor;
            m_cTextOne.setText(caption, 1f, lineSpace, true, lineFeedWidth);
            if ((style & e_WinCtrlStyle.TEXT_AUTOSCALE) != 0 && m_cTextOne.width > contentsSize.x) {
                float sc = contentsSize.x / m_cTextOne.width;
                size = new Vector2(m_cTextOne.width * sc, m_cTextOne.height * sc);
            } else {
                size = new Vector2(m_cTextOne.width, m_cTextOne.height);
            }
            m_eUpdateFlag &= ~e_UpdateFlag.CAPTION;
        }
    }
}
