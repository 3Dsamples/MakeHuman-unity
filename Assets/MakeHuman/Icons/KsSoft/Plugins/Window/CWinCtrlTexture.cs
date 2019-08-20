//==============================================================================================
/*!?テクスチャをレンダリングする.
	@file  CWinCtrlTexture

	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KS {
    public class CWinCtrlTexture : CWinCtrlBase {
        protected t_ProcHit m_tProcHit;
        //==========================================================================
        /*!Constructor
            @brief	Constructor
        */
        public CWinCtrlTexture(CWindowBase cWindowBase, e_WinCtrlKind eKind = e_WinCtrlKind.TEXTURE) : base(cWindowBase, eKind) {
            m_tProcHit.clear();
            m_bNohit = true;
            SEId = CWindowMgr.Instance.clickSE;
        }
        //==========================================================================
        /*!Copy Constructor
            @brief	Copy Constructor
        */
        public CWinCtrlTexture(CWinCtrlTexture src, CWinContents parent) : base(src, parent) {
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
            WinColor col;
            for (int i = 0; i < m_aTexture.Length; i++) {
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
            m_tProcHit.update();
        }
        //==========================================================================
        /*!ヒットを処理する.
            @brief	procHit
        */
        override public void procHit(CWindowMgr cWindowMgr, CInput.e_State eState) {
            procHit(ref m_tProcHit, cWindowMgr, eState);
        }
    }
}
