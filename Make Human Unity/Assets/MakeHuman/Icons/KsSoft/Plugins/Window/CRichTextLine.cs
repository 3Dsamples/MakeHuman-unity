//==============================================================================================
/*!?リッチテキスト一行分.
	@file  CRichTextLine
		
	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace KS {
    public class CRichTextLine {
        CRichTextOne[] m_aRTO;

        e_Anchor m_eAnchor = e_Anchor.LeftTop;
        Vector2 m_size = Vector2.zero;
        //==========================================================================
        /*!Constructor
            @brief	Constructor
        */
        public CRichTextLine(CRichTextOne[] aRTO) {
            m_aRTO = aRTO;
            if (m_aRTO == null || m_aRTO.Length == 0) {
                return;
            }
            m_eAnchor = m_aRTO[0].anchor;
            float w = 0f;
            float h = 0f;
            for (int i = 0; i < m_aRTO.Length; ++i) {
                CRichTextOne cRTO = m_aRTO[i];
                h = Mathf.Max(h, cRTO.size.y);
                w = Mathf.Max(w, cRTO.position.x + cRTO.size.x);
            }
            m_size.x = w;
            m_size.y = h;
            for (int i = 0; i < m_aRTO.Length; ++i) {
                CRichTextOne cRTO = m_aRTO[i];
                cRTO.end(height);
            }
        }
        //==========================================================================
        /*!解放.
            @brief	release
        */
        public void release() {
            for (int i = 0; i < m_aRTO.Length; ++i) {
                m_aRTO[i].release();
            }
        }
        //==========================================================================
        /*!リッチテキストのレンダリングリクエスト.
            @brief	request
        */
        public void request(CWindowBase cWindow, Vector3 basepos, float parentWidth, WinColor color, ClipRect cr) {
            if (m_aRTO == null || m_aRTO.Length == 0) {
                return;
            }
            basepos = calcAnchorPos(basepos, parentWidth);

            for (int i = 0; i < m_aRTO.Length; ++i) {
                CRichTextOne cRTO = m_aRTO[i];

                cRTO.request(basepos, color, cr);
            }
        }
        Vector3 calcAnchorPos(Vector3 pos, float parentWidth) {
            if (m_eAnchor == e_Anchor.Center || m_eAnchor == e_Anchor.Top || m_eAnchor == e_Anchor.Bottom) {
                pos.x += parentWidth * 0.5f - width * 0.5f;
            } else if (m_eAnchor == e_Anchor.RightCenter || m_eAnchor == e_Anchor.RightTop || m_eAnchor == e_Anchor.RightBottom) {
                pos.x += parentWidth - width;
            }
            return KsSoftUtility.pixelPerfect(pos);
        }
        //==========================================================================
        /*!ヒットチェックする.
            @brief	checkHit
        */
        public CWindowMgr.CCollision checkHit(Vector2 touchPos, Vector2 basepos, float parentWidth) {
            if (m_aRTO == null || m_aRTO.Length == 0) {
                return null;
            }
            basepos = calcAnchorPos(basepos, parentWidth);

            for (int i = 0; i < m_aRTO.Length; ++i) {
                CRichTextOne cRTO = m_aRTO[i];

                CWindowMgr.CCollision cCollision = cRTO.checkHit(touchPos, basepos);
                if (cCollision != null) {
                    return cCollision;
                }
            }
            return null;
        }
        public float width {
            get {
                return m_size.x;
            }
        }
        public float height {
            get {
                return m_size.y;
            }
        }

    }
}
