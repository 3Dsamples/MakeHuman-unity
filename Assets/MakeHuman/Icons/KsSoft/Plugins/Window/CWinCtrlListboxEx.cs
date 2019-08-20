//==============================================================================================
/*!?可変サイズのコンテンツを制御可能なリストボックスコントロール.
	@file  CWinCtrlListboxEx
	
	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KS {
    public class CWinCtrlListboxEx : CWinCtrlListbox {
        //==========================================================================
        /*!Constructor
            @brief	Constructor
        */
        public CWinCtrlListboxEx(CWindowBase cWindow) : base(cWindow, e_WinCtrlKind.LISTBOXEX) {
        }
        //==========================================================================
        /*!Copy Constructor
            @brief	Copy Constructor
        */
        public CWinCtrlListboxEx(CWinCtrlListboxEx src, CWinContents parent) : base(src, parent) {
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
            m_idxLastTopContent = -1;
            int idxOldLastTop = m_idxLastTopContent;
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
            for (int i = 0; i < count; i++) {
                CWinContents con = m_aContents[i];
                if (con != null) {
                    if (isLoop) {
                        if (con.clipState != ClipRect.State.Outside) {
                            continue;
                        }
                    }
                    Vector2 conSize = con.size;
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
                    if (stackOrientation == e_StackOrientation.Vertical) {
                        pos.y += conSize.y + lineSpace;
                    } else {
                        pos.x += conSize.x + lineSpace;
                    }
                }
            }
            return pos;
        }

        //==========================================================================
        /*!指定のコンテンツを先頭に設定するためのオフセットを取得する.
            @brief	getContentsOffset
        */
        override public Vector2 getContentsOffset(int index, e_Anchor eAnchor) {
            if (stackOrientation == e_StackOrientation.Vertical) {
                if (eAnchor == e_Anchor.Bottom || eAnchor == e_Anchor.LeftBottom || eAnchor == e_Anchor.RightBottom) {
                    Vector2 vOffset = getContentsOffset(index + 1);
                    vOffset.y += size.y;
                    return vOffset;
                }
                if (eAnchor == e_Anchor.Top || eAnchor == e_Anchor.LeftTop || eAnchor == e_Anchor.RightTop) {
                    return getContentsOffset(index);
                }
                return (getContentsOffset(index, e_Anchor.Bottom) + getContentsOffset(index, e_Anchor.Top)) * 0.5f;
            } else {
                if (eAnchor == e_Anchor.Left || eAnchor == e_Anchor.LeftBottom || eAnchor == e_Anchor.LeftTop) {
                    return getContentsOffset(index);
                }
                if (eAnchor == e_Anchor.Right || eAnchor == e_Anchor.RightBottom || eAnchor == e_Anchor.RightTop) {
                    Vector2 vOffset = getContentsOffset(index + 1);
                    vOffset.x += size.x;
                    return vOffset;
                }
                return (getContentsOffset(index, e_Anchor.Left) + getContentsOffset(index, e_Anchor.Right)) * 0.5f;
            }
        }
        protected Vector2 getContentsOffset(int index) {
            index = Mathf.Clamp(index, 0, count);
            Vector2 offset = Vector2.zero;
            if (stackOrientation == e_StackOrientation.Vertical) {
                for (int i = 0; i < index; i++) {
                    CWinContents con = m_aContents[i];
                    if (con != null) {
                        offset.y -= con.size.y + lineSpace;
                    }
                }
            } else {
                for (int i = 0; i < index; i++) {
                    CWinContents con = m_aContents[i];
                    if (con != null) {
                        offset.x -= con.size.x + lineSpace;
                    }
                }
            }
            return offset;
        }
        override public Vector2 screensize {
            get {
                if (count == 0) {
                    return Vector2.zero;
                }
                Vector2 sz;
                sz.x = 0f;
                sz.y = 0f;
                if (stackOrientation == e_StackOrientation.Vertical) {
                    for (int i = 0; i < m_aContents.Length; ++i) {
                        CWinContents cContents = m_aContents[i];
                        if (cContents == null) {
                            break;
                        }
                        Vector2 szCon = cContents.size;
                        sz.x = Mathf.Max(sz.x, szCon.x);
                        sz.y += szCon.y + lineSpace;
                    }
                } else {
                    for (int i = 0; i < m_aContents.Length; ++i) {
                        CWinContents cContents = m_aContents[i];
                        if (cContents == null) {
                            break;
                        }
                        Vector2 szCon = cContents.size;
                        sz.x += szCon.x + lineSpace;
                        sz.y = Mathf.Max(sz.y, szCon.y);
                    }
                }
                return sz;
            }
        }
    }
}
