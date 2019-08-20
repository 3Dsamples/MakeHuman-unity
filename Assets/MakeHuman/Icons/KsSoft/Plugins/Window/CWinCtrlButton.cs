//==============================================================================================
/*!?ボタンコントロール.
	@file  CWinCtrlButton
	
	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KS {
    public class CWinCtrlButton : CWinCtrlBase {
        protected Parts[] m_aParts = new Parts[2];
        protected t_ProcHit m_tProcHit;
        //==========================================================================
        /*!Constructor
            @brief	Constructor
        */
        public CWinCtrlButton(CWindowBase cWindow, e_WinCtrlKind eKind = e_WinCtrlKind.BUTTON) : base(cWindow, eKind) {
            m_aTexture[0].partId = FiveCC.Id("BTN0?");
            m_cTextOne = new CWinTextOne();
            textAnchor = e_Anchor.Center;
            m_tProcHit.clear();
            SEId = CWindowMgr.Instance.clickSE;
        }
        //==========================================================================
        /*!Copy Constructor
            @brief	Copy Constructor
        */
        public CWinCtrlButton(CWinCtrlButton src, CWinContents parent) : base(src, parent) {
        }
        //==========================================================================
        /*!Update
            @brief	update
        */
        override public void update(Vector3 pos, int priority) {
            base.update(pos, priority);
        }
        //==========================================================================
        /*!Render
            @brief	render
        */
        override public void render(Vector3 pos, CWindowMesh cWinMesh, CWinTextMesh cTextMesh, WinColor rtcolor, ClipRect cr) {
            Parts parts;
            if (!m_tProcHit.tap) {
                parts = m_aParts[0];
            } else {
                parts = m_aParts[1];
            }
            WinColor col = rtcolor * color;
            cWinMesh.request(parts, pos, col, 0, cr);

            for (int i = 1; i < m_aTexture.Length; i++) {
                if (m_aTexture[i].partId != 0) {
                    CWindowMesh cMesh = getWindowMesh(i);
                    if (cMesh == null) {
                        continue;
                    }
                    col = rtcolor * m_aTexture[i].color;
                    Vector3 texpos = pos;
                    Vector2 offset = getRatioValue(m_aTexture[i].offset);
                    texpos.x += offset.x;
                    texpos.y += offset.y;
                    cMesh.request(m_aTexture[i].parts, texpos, col, 0, cr);
                }
            }
            col = rtcolor * captionColor;
            cTextMesh.request(m_cTextOne, pos, captionOffset, textAnchor, col, size, cr);

            m_tProcHit.update();
        }
        //==========================================================================
        /*!パーツを再更新する必要があるとき呼ばれる.
            @brief	onResetParts
        */
        override public void onResetParts(int iTexIndex) {
            if (iTexIndex == 0) {
                getParts2State(m_aParts, 0);
            } else {
                base.onResetParts(iTexIndex);
            }
        }
        //==========================================================================
        /*!ヒットを処理する.
            @brief	procHit
        */
        override public void procHit(CWindowMgr cWindowMgr, CInput.e_State eState) {
            procHit(ref m_tProcHit, cWindowMgr, eState);
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
        //==========================================================================
        /*!タップしているかキャプションパーツを再更新する必要があるとき呼ばれる.
            @brief	isTap
        */
        public bool isTap {
            get {
                return m_tProcHit.tap;
            }
        }
        public CInput.e_State state {
            get {
                return m_tProcHit.state;
            }
        }
    }
}
