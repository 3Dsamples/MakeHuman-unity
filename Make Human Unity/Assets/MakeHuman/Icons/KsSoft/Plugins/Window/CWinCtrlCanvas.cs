//==============================================================================================
/*!?キャンバス(自由に割り当てられたメッシュにレンダリング可能).
	@file  CWinCtrlCanvas

	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KS {
    public class CWinCtrlCanvas : CWinCtrlBase {
        protected t_ProcHit m_tProcHit;
        //==========================================================================
        /*!Constructor
            @brief	Constructor
        */
        public CWinCtrlCanvas(CWindowBase cWindowBase) : base(cWindowBase, e_WinCtrlKind.CANVAS) {
            m_bNohit = true;
        }
        //==========================================================================
        /*!initialize
            @brief	Copy initialize
        */
        override protected void initialize() {
            m_cTextOne = new CWinTextOne();
        }
        //==========================================================================
        /*!Copy Constructor
            @brief	Copy Constructor
        */
        public CWinCtrlCanvas(CWinCtrlCanvas src, CWinContents parent) : base(src, parent) {
        }
        //==========================================================================
        /*!ヒットを処理する.
            @brief	procHit
        */
        override public void procHit(CWindowMgr cWindowMgr, CInput.e_State eState) {
            procHit(ref m_tProcHit, cWindowMgr, eState);
        }
        //==========================================================================
        /*!Render
            @brief	render
        */
        override public void render(Vector3 pos, CWindowMesh cWinMesh, CWinTextMesh cTextMesh, WinColor rtcolor, ClipRect cr) {
            if ((m_eUpdateFlag & e_UpdateFlag.TEXID) != 0) {
                return;
            }

            if (cWinMesh != null && !cWinMesh.isLoaded) {
                return;
            }
#if UNITY_EDITOR
            bool res =
#endif
        window.onRender(this, pos, cWinMesh, cTextMesh, rtcolor, cr);
#if UNITY_EDITOR
            if (!res) {
                if (m_aTexture[0].partId != 0) {
                    CWindowMesh cMesh = getWindowMesh(0);
                    if (cMesh != null) {
                        WinColor col = rtcolor * m_aTexture[0].color;
                        Vector3 texpos = pos;
                        Vector2 offset = getRatioValue(m_aTexture[0].offset);
                        texpos.x += offset.x;
                        texpos.y += offset.y;
                        cMesh.request(m_aTexture[0].parts, texpos, col, 0, cr);
                    }
                }
            }
#endif
            m_tProcHit.update();
        }
        //==========================================================================
        /*!キャプションパーツを再更新する必要があるとき呼ばれる.
            @brief	onResetCaption
        */
        override public void onResetCaption() {
            if (string.IsNullOrEmpty(caption)) {
                return;
            }
            if (m_cTextOne == null) {
                m_cTextOne = new CWinTextOne();
            }
            m_cTextOne.style = textStyle;
            m_cTextOne.anchor = textAnchor;
            m_cTextOne.setText(caption, 1f, lineSpace, true, lineFeedWidth);
            m_eUpdateFlag &= ~e_UpdateFlag.CAPTION;
        }
        public CWinTextOne winTextOne {
            get {
                if (m_cTextOne == null || m_cTextOne.Length == 0) {
                    return null;
                }
                return m_cTextOne;
            }
        }
        public bool isTap {
            get {
                return m_tProcHit.tap;
            }
        }
    }
}
