//==============================================================================================
/*!?ログコントロール.
	@file  CWinCtrlLog
	
	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KS {
    public class CWinCtrlLog : CWinCtrlScrollable {
        protected int m_topIndex = 0;
        protected int m_insertPoint = 0;
        protected int m_indexLogText = -1;
        //==========================================================================
        /*!Constructor
            @brief	Constructor
        */
        public CWinCtrlLog(CWindowBase cWindow) : base(cWindow, e_WinCtrlKind.LOG) {
        }
        //==========================================================================
        /*!Copy Constructor
            @brief	Copy Constructor
        */
        public CWinCtrlLog(CWinCtrlLog src, CWinContents parent) : base(src, parent) {
        }
        //==========================================================================
        /*!Initialize
            @brief	Initialize
        */
        protected override void initialize() {
            base.initialize();
            CWinContents cContents = getContents();
            m_indexLogText = -1;
            for (int i = 0; i < cContents.count; ++i) {
                if (cContents[i] is CWinCtrlLogText) {
                    m_indexLogText = i;
                    break;
                }
            }
            if (m_indexLogText < 0) {
                Debug.LogWarning("can't find ctrl(LOGTEXT) in LOG:" + this);
            }
        }
        //==========================================================================
        /*!Update Contents
            @brief	Update Contents
        */
        override protected void updateContents(Vector3 pos) {
            int iCount = count;
            for (int i = 0; i < iCount; i++) {
                int index = (i + m_topIndex) % iCount;
                CWinContents con = m_aContents[index];
                if (con == null) break;
                //			Vector2	szLog = con[m_indexLogText].size;
                Vector2 szCon = con.size;
                con.clipState = m_clipRect.check(pos, szCon);
                if (con.clipState != ClipRect.State.Outside) {
                    con.update(pos, m_absPriority);
                }
                pos.y += szCon.y + lineSpace;
            }
        }
        //==========================================================================
        /*!ヒットチェックする.
            @brief	checkHitContents
        */
        override protected CWindowMgr.CCollision checkHitContents(Vector3 touchPos, ClipRect cr, CStick stk) {
            int iCount = count;
            for (int i = 0; i < iCount; i++) {
                int index = (i + m_topIndex) % iCount;
                CWinContents con = m_aContents[index];
                if (con == null) break;
                if (con.clipState != ClipRect.State.Outside) {
                    CWindowMgr.CCollision cCollision = con.checkHit(touchPos, m_clipRect, stk);
                    if (cCollision != null) {
                        return cCollision;
                    }
                }
            }
            return null;
        }
        //==========================================================================
        /*!Render
            @brief	render
        */
        override public void render(Vector3 basepos, CWindowMesh cWinMesh, CWinTextMesh cTextMesh, WinColor color, ClipRect cr) {
            mergeClipRect(cr);
            for (int i = 0; i < count; i++) {
                CWinContents con = m_aContents[i];
                if (con == null) break;
                switch (con.clipState) {
                case ClipRect.State.Outside:
                    break;
                case ClipRect.State.Inside:
                    con.render(color, m_clipRect);
                    break;
                case ClipRect.State.Clipped:
                    con.render(color, m_clipRect);
                    break;
                }
            }
        }
        //==========================================================================
        /*!ログのサイズを変更する.
            @brief	resize
        */
        override public void resize(int count) {
            if (m_contents == null || m_contents.count == 0) {
                return;
            }
            if (count < 0) {
                return;
            }
            m_aContents = new CWinContents[count];
        }
        //==========================================================================
        /*!コンテンツの数を取得する.
            @brief	count
        */
        override public int count {
            get {
                if (m_aContents == null) {
                    return 0;
                }
                return m_insertPoint;
            }
        }
        //==========================================================================
        /*!ログを全て消す.
            @brief	clearLog
        */
        public void clearLog() {
            m_insertPoint = 0;
            m_topIndex = 0;
        }
        //==========================================================================
        /*!ログを追加する.
            @brief	add
        */
        protected CWinContents add() {
            if (m_aContents == null) {
                return null;
            }
            if (m_contents == null || m_contents.count == 0) {
                return null;
            }
            if (m_insertPoint < m_aContents.Length) {
                CWinContents c = new CWinContents(m_contents, this);
                m_aContents[m_insertPoint++] = c;
                return c;
            }
            int index = m_topIndex;
            m_topIndex = (m_topIndex + 1) % count;
            return m_aContents[index];
        }
        //==========================================================================
        /*!ログを追加する.
            @brief	add
        */
        public CWinCtrlLogText add(string text, WinColor color) {
            if (m_indexLogText < 0) {
                return null;
            }
            CWinContents cContents = add();
            if (cContents == null) {
                return null;
            }
            CWinCtrlLogText cLogText = cContents[m_indexLogText] as CWinCtrlLogText;
            if (cLogText == null) {
                return null;
            }
            cLogText.setLog(text, color);

            Vector2 vOffset = smoothOffset;
            vOffset.y -= cContents.height + lineSpace;
            smoothOffset = vOffset;
            return cLogText;
        }
        override public Vector2 screensize {
            get {
                if (m_aContents == null || m_indexLogText < 0) {
                    return Vector2.zero;
                }
                float sx = 0f;
                float sy = 0f;
                for (int i = 0; i < m_aContents.Length; ++i) {
                    CWinContents cContents = m_aContents[i];
                    if (cContents == null) {
                        break;
                    }
                    Vector2 size = cContents.size;
                    sx = Mathf.Max(sx, size.x);
                    sy += size.y + lineSpace;
                }
                return new Vector2(sx, sy);
            }
        }
    }
}
