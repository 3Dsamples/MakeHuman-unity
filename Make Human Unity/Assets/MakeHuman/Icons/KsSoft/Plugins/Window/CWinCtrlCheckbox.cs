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
    public class CWinCtrlCheckbox : CWinCtrlButton {
        protected bool m_bCheckState = false;
        //==========================================================================
        /*!Constructor
            @brief	Constructor
        */
        public CWinCtrlCheckbox(CWindowBase cWindow, e_WinCtrlKind eKind = e_WinCtrlKind.CHECKBOX) : base(cWindow, eKind) {
            m_aTexture[0].partId = FiveCC.Id("BTN0?");
        }
        //==========================================================================
        /*!Constructor
            @brief	Constructor
        */
        public CWinCtrlCheckbox(CWinCtrlCheckbox src, CWinContents parent) : base(src, parent) {
            m_bCheckState = src.m_bCheckState;
        }
        //==========================================================================
        /*!Update
            @brief	Update
        */
        override public void update(Vector3 pos, int priority) {
            base.update(pos, priority);
			// コンテンツのアップデート.
			CWinContents contents = getContents();
			if (contents != null) {
				contents.hide = !check;
				if (check) {
					contents.update(pos, m_absPriority);
				}
			}
        }
        //==========================================================================
        /*!Render
            @brief	render
        */
        override public void render(Vector3 basepos, CWindowMesh cWinMesh, CWinTextMesh cTextMesh, WinColor rtcolor, ClipRect cr) {
            Vector3 pos = basepos;
            Parts parts;
            if (!m_tProcHit.tap) {
                if (check) {
                    parts = m_aParts[1];
                } else {
                    parts = m_aParts[0];
                }
            } else {
                if (check) {
                    parts = m_aParts[0];
                } else {
                    parts = m_aParts[1];
                }
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
            // コンテンツのレンダリング.
            if (check) {
                CWinContents contents = getContents();
                if (contents != null) {
                    contents.render(rtcolor, cr);
                }
            }
        }
        //==========================================================================
        /*!ヒットチェックする.
            @brief	checkHit
        */
        override public CWindowMgr.CCollision checkHit(Vector2 touchPos, ClipRect cr, CStick stk) {
            if (cr != null && !cr.check(touchPos)) {
                return null;
            }
            if (isHit(touchPos)) {
                return new CWindowMgr.CCollision(this);
            }
            // コンテンツのアップデート.
            if (check) {
                CWinContents contents = getContents();
                if (contents != null) {
                    CWindowMgr.CCollision cCollision = contents.checkHit(touchPos, cr, stk);
                    if (cCollision != null) {
                        return cCollision;
                    }
                }
            }
            return null;
        }
        //==========================================================================
        /*!ヒットを実行する.
            @brief	procHit
        */
        override public void procHit(CWindowMgr cWindowMgr, CInput.e_State eState) {
            procHit(ref m_tProcHit, cWindowMgr, eState, !check);
        }
        //==========================================================================
        /*!チェック状態を切り替える.
            @brief	check
        */
        virtual public bool check {
            get {
                return m_bCheckState;
            }
            set {
                setCheckState(value);
            }
        }
        override protected void setCheckState(bool bState) {
            m_bCheckState = bState;
        }
    }
}
