//==============================================================================================
/*!?ラインコントロール.
	@file  CWinCtrlLine
	
	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KS {
    public class CWinCtrlLine : CWinCtrlBase {
        //==========================================================================
        /*!Constructor
            @brief	Constructor
        */
        public CWinCtrlLine(CWindowBase cWindowBase, e_WinCtrlKind eKind = e_WinCtrlKind.LINE) : base(cWindowBase, eKind) {
            m_aTexture[0].partId = FiveCC.Id("LINE");
            m_bNohit = true;
        }
        //==========================================================================
        /*!Copy Constructor
            @brief	Copy Constructor
        */
        public CWinCtrlLine(CWinCtrlLine src, CWinContents parent) : base(src, parent) {
        }
        //==========================================================================
        /*!Render
            @brief	render
        */
        override public void render(Vector3 basepos, CWindowMesh cWinMesh, CWinTextMesh cTextMesh, WinColor rtcolor, ClipRect cr) {
            WinColor col = rtcolor * color;
            Vector3 endpos = basepos;

            Vector2 d = getRatioValue(m_position2) - getRatioValue(m_position);
            endpos.x += d.x;
            endpos.y += d.y;
            cWinMesh.request(m_aTexture[0].parts, basepos, endpos, height, col, cr);
        }
        //==========================================================================
        /*!ヒットチェックする.
            @brief	checkHit
        */
        public override CWindowMgr.CCollision checkHit(Vector2 touchPos, ClipRect cr, CStick stk) {
            if (cr != null && !cr.check(touchPos)) {
                return null;
            }
            Vector3 endpos = m_absPosition;

            Vector2 d = getRatioValue(m_position2) - getRatioValue(m_position);
            endpos.x += d.x;
            endpos.y += d.y;
            if (isHit(touchPos, m_absPosition, endpos, height)) {
                return new CWindowMgr.CCollision(this);
            }
            return null;
        }
        protected bool isHit(Vector2 vP, Vector2 vA, Vector2 vB, float height) {
            Vector2 vAB = vB - vA;
            Vector2 vAP = vP - vA;
            float lnAB = vAB.magnitude;
            Vector2 nAB = vAB / lnAB;
            float lnAX = Vector2.Dot(nAB, vAP);

            if (lnAX < 0f || lnAX > lnAB) {
                return false;
            }
            Vector2 vX = vA + nAB * lnAX;
            if (height == 0f) {
                if (m_aTexture[0].parts == null) {
                    return false;
                }
                height = m_aTexture[0].parts.height;
            }
            height *= 0.5f;
            if ((vX - vP).sqrMagnitude <= height * height) {
                return true;
            }
            return false;
        }
        //==========================================================================
        /*!パーツを再更新する必要があるとき呼ばれる.
            @brief	onResetParts
        */
        override public void onResetParts(int iTexIndex) {
            if (iTexIndex != 0) {
                return;
            }
            Vector2 size = getTextureSize(0);
            Vector2 d = getRatioValue(m_position2) - getRatioValue(m_position);
            float len = d.magnitude;
            size.x = Mathf.Floor(len);
            setTextureSize(0, size, Vector2.zero);
            base.onResetParts(iTexIndex);
        }
    }
}
