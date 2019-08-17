//==============================================================================================
/*!?リッチテキストコントロール.
	@file  CWinCtrlRichText

	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KS {
    public class CWinCtrlRichText : CWinCtrlBase {
        CRichText m_cRichText = null;
        //==========================================================================
        /*!Constructor
            @brief	Constructor
        */
        public CWinCtrlRichText(CWindowBase cWindowBase) : base(cWindowBase, e_WinCtrlKind.RICHTEXT) {
            m_cRichText = new CRichText(this);
            m_cTextOne = new CWinTextOne();
        }
        //==========================================================================
        /*!Copy Constructor
            @brief	Copy Constructor
        */
        public CWinCtrlRichText(CWinCtrlRichText src, CWinContents parent) : base(src, parent) {
            m_cRichText = new CRichText(this);
            m_cTextOne = new CWinTextOne();
        }
        //==========================================================================
        /*!解放.
            @brief	release
        */
        public override void release() {
            base.release();
            m_cRichText.release();
        }
        //==========================================================================
        /*!Render
            @brief	render
        */
        override public void render(Vector3 pos, CWindowMesh cWinMesh, CWinTextMesh cTextMesh, WinColor color, ClipRect cr) {
            pos.x += captionOffset.x;
            pos.z += captionOffset.y;
            color = color * this.color;
            m_cRichText.request(pos, color, cr);
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
                CWindowMgr.CCollision col = m_cRichText.checkHit(touchPos, m_absPosition, stk);
                if (col == null) {
                    return new CWindowMgr.CCollision(this);
                }
            }
            return null;
        }
        //==========================================================================
        /*!パーツを再更新する必要があるとき呼ばれる.
            @brief	onResetParts
        */
        override public void onResetParts(int iTexIndex) {
        }
        //==========================================================================
        /*!キャプションパーツを再更新する必要があるとき呼ばれる.
            @brief	onResetCaption
        */
        override public void onResetCaption() {
            if (m_cRichText.setText(caption, captionColor, lineFeedWidth, lineSpace)) {
                m_eUpdateFlag &= ~e_UpdateFlag.CAPTION;
            }
        }
#if UNITY_EDITOR
        public bool validate(string str) {
            return m_cRichText.setText(str, captionColor, lineFeedWidth, lineSpace);
        }
#endif
        override public Vector2 getTextureSize(int index) {
            if (index == 0) {
                return m_cRichText.size;
            }
            return base.getTextureSize(index);
        }
        override public void setTextureSize(int index, Vector3 x, Vector3 y) {
        }
        public override bool isDragable {
            get {
                return false;
            }
        }
        override public WinColor captionColor {
            get {
                return m_captionColor;
            }
            set {
                if (m_captionColor != value) {
                    m_captionColor = value;
                    m_eUpdateFlag |= e_UpdateFlag.CAPTION;
                }
            }
        }

    }
}
