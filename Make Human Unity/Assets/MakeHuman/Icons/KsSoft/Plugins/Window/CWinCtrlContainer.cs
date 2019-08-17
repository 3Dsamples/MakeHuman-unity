//==============================================================================================
/*!?コンテナコントロール.
	@file  CWinCtrlContainer
	
	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace KS {
    public class CWinCtrlContainer : CWinCtrlScrollable {
        //==========================================================================
        /*!Constructor
            @brief	Constructor
        */
        public CWinCtrlContainer(CWindowBase cWindow, e_WinCtrlKind eKind = e_WinCtrlKind.CONTAINER) : base(cWindow, eKind) {
        }
        //==========================================================================
        /*!Copy Constructor
            @brief	Copy Constructor
        */
        public CWinCtrlContainer(CWinCtrlContainer src, CWinContents parent) : base(src, parent) {
        }
        //==========================================================================
        /*!コンテンツの更新を行う.
            @brief	updateContents
        */
        override protected void updateContents(Vector3 pos) {
            CWinContents contents = getContents();
            if (contents != null) {
                contents.update(pos, m_absPriority);
            }
        }
        //==========================================================================
        /*!Render
            @brief	render
        */
        override public void render(Vector3 pos, CWindowMesh cWinMesh, CWinTextMesh cTextMesh, WinColor rtcolor, ClipRect cr) {
            mergeClipRect(cr);
            rtcolor = rtcolor * color;
            CWinContents contents = getContents();
            if (contents != null) {
                contents.render(rtcolor, m_clipRect);
            }
        }
        //==========================================================================
        /*!ヒットチェックする.
            @brief	checkHitContents
        */
        override protected CWindowMgr.CCollision checkHitContents(Vector3 touchPos, ClipRect cr, CStick stk) {
            CWinContents contents = getContents();
            if (contents == null) {
                return null;
            }
            CWindowMgr.CCollision cCollision = contents.checkHit(touchPos, m_clipRect, stk);
            if (cCollision != null) {
                return cCollision;
            }
            return null;
        }
        override public Vector2 screensize {
            get {
                return new Vector2(contentsSize.x, contentsSize.y);
            }
        }
    }
}
