//==============================================================================================
/*!?リストボックスコントロール.
	@file  CWinCtrlListbox
	
	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KS {
    public class CWinCtrlListbox : CWinCtrlScrollable {
        protected int m_idxLastTopContent;
        //==========================================================================
        /*!Constructor
            @brief	Constructor
        */
        public CWinCtrlListbox(CWindowBase cWindow, e_WinCtrlKind eKind = e_WinCtrlKind.LISTBOX) : base(cWindow, eKind) {
            m_aContents = new CWinContents[0];
            m_idxLastTopContent = 0;
            SEId = CWindowMgr.Instance.scrollSE;
        }
        //==========================================================================
        /*!Copy Constructor
            @brief	Copy Constructor
        */
        public CWinCtrlListbox(CWinCtrlListbox src, CWinContents parent) : base(src, parent) {
        }
        //==========================================================================
        /*!Update Contents
            @brief	Update Contents
        */
        override protected void updateContents(Vector3 pos) {
            if (count <= 0) {
                return;
            }
            float k = 0f;
            float basepos;
            if (stackOrientation == e_StackOrientation.Vertical) {
                basepos = pos.x;
            } else {
                basepos = pos.y;
            }
            if (window.isFade) {
                k = window.fadeInterpolate;
                if (!window.isClose) {
                    k = 1f - k;
                }
            }
            int idxOldLastTop = m_idxLastTopContent;
            m_idxLastTopContent = -1;
            pos = updateContents(basepos, pos, false, idxOldLastTop, k);
            if (isLoop) {
                if (stackOrientation == e_StackOrientation.Vertical && pos.y > m_clipRect.yMin) {
                    pos = updateContents(basepos, pos, true, idxOldLastTop, k);
                } else if (stackOrientation == e_StackOrientation.Horizontal && pos.x < m_clipRect.xMax) {
                    pos = updateContents(basepos, pos, true, idxOldLastTop, k);
                }
            }
        }
        Vector3 updateContents(float basepos, Vector3 pos, bool isLoop, int idxOldLastTop, float k) {
            float fAnim = 0f;
            Vector2 conSize = contentsSize;
            for (int i = 0; i < count; i++) {
                CWinContents con = m_aContents[i];
                if (con != null) {
                    if (isLoop) {
                        if (con.clipState != ClipRect.State.Outside) {
                            continue;
                        }
                    }
                    con.clipState = m_clipRect.check(pos, conSize);
                    if (con.clipState != ClipRect.State.Outside) {
                        if (k != 0f) {
                            fAnim += 0.3f;
                            if (stackOrientation == e_StackOrientation.Vertical) {
                                pos.x = basepos + conSize.x * fAnim * k;
                            } else {
                                pos.y = basepos + conSize.y * fAnim * k;
                            }
                        }
                        con.update(pos, m_absPriority);
                        if (m_idxLastTopContent < 0) {
                            m_idxLastTopContent = i;
                            if (m_idxLastTopContent != idxOldLastTop) {
                                play(SEId);
                            }
                        }
                    }
                }
                if (stackOrientation == e_StackOrientation.Vertical) {
                    pos.y += conSize.y + lineSpace;
                } else {
                    pos.x += conSize.x + lineSpace;
                }
            }
            return pos;
        }
        //==========================================================================
        /*!ヒットチェックする.
            @brief	checkHitContents
        */
        override protected CWindowMgr.CCollision checkHitContents(Vector3 touchPos, ClipRect cr, CStick stk) {
            for (int i = 0; i < count; i++) {
                CWinContents con = m_aContents[i];
                if (con == null) {
                    continue;
                }
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
                if (con == null) {
                    continue;
                }
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
        /*!指定のコンテンツを先頭に設定するためのオフセットを取得する.
            @brief	getContentsOffset
        */
        virtual public Vector2 getContentsOffset(int index, e_Anchor eAnchor) {
            index = Mathf.Clamp(index, 0, count);
            if (stackOrientation == e_StackOrientation.Vertical) {
                if (eAnchor == e_Anchor.Bottom || eAnchor == e_Anchor.LeftBottom || eAnchor == e_Anchor.RightBottom) {
                    return new Vector2(0, -(contentsSize.y + lineSpace) * (index + 1) + size.y);
                }
                if (eAnchor == e_Anchor.Top || eAnchor == e_Anchor.LeftTop || eAnchor == e_Anchor.RightTop) {
                    return new Vector2(0, -(contentsSize.y + lineSpace) * index);
                }
                return (getContentsOffset(index, e_Anchor.Bottom) + getContentsOffset(index, e_Anchor.Top)) * 0.5f;
            } else {
                if (eAnchor == e_Anchor.Left || eAnchor == e_Anchor.LeftBottom || eAnchor == e_Anchor.LeftTop) {
                    return new Vector2(-(contentsSize.x + lineSpace) * index, 0);
                }
                if (eAnchor == e_Anchor.Right || eAnchor == e_Anchor.RightBottom || eAnchor == e_Anchor.RightTop) {
                    return new Vector2(-(contentsSize.x + lineSpace) * (index + 1) + size.x, 0);
                }
                return (getContentsOffset(index, e_Anchor.Left) + getContentsOffset(index, e_Anchor.Right)) * 0.5f;
            }
        }
        override public Vector2 screensize {
            get {
                if (stackOrientation == e_StackOrientation.Vertical) {
                    return new Vector2(contentsSize.x, (contentsSize.y + lineSpace) * count);
                } else {
                    return new Vector2((contentsSize.x + lineSpace) * count, contentsSize.y);
                }
            }
        }
    }
}
