//==============================================================================================
/*!?リッチテキストボックスコントロール.
	@file  CWinCtrlRichTextbox
	
	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KS {
    public class CWinCtrlRichTextbox : CWinCtrlScrollable {
        protected CRichText m_cRichText = null;
        protected Parts[] m_aParts = new Parts[2];
        protected bool m_bHasFocus = false;
        protected ClipRect m_crText = new ClipRect();
        protected Vector2 m_textOffset = Vector2.zero;

        //==========================================================================
        /*!Constructor
            @brief	Constructor
        */
        public CWinCtrlRichTextbox(CWindowBase cWindowBase, e_WinCtrlKind eKind = e_WinCtrlKind.RICHTEXTBOX) : base(cWindowBase, eKind) {
            m_aTexture[0].partId = new FiveCC("TXFD");
            m_cRichText = new CRichText(this);
            m_cTextOne = new CWinTextOne();
        }
        //==========================================================================
        /*!Copy Constructor
            @brief	Copy Constructor
        */
        public CWinCtrlRichTextbox(CWinCtrlRichTextbox src, CWinContents parent) : base(src, parent) {
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
        override protected void updateContents(Vector3 pos) {
        }
        //==========================================================================
        /*!Render
            @brief	render
        */
        override public void render(Vector3 pos, CWindowMesh cWinMesh, CWinTextMesh cTextMesh, WinColor rtcolor, ClipRect cr) {
            mergeClipRect(cr);
            WinColor col = rtcolor * color;
            Parts parts;
            if (!m_bHasFocus) {
                parts = m_aParts[0];
            } else {
                parts = m_aParts[1];
            }
            float xB = m_absPosition.x + captionOffset.x;
            float yB = m_absPosition.y + captionOffset.y;
            float crWidth = m_crText.width;
            if (cr != null) {
                m_crText.x = Mathf.Max(xB, cr.x);
                m_crText.y = Mathf.Min(yB, cr.y);
                m_crText.ex = Mathf.Min(xB + contentsSize.x, cr.ex);
                m_crText.ey = Mathf.Max(yB + contentsSize.y, cr.ey);
            } else {
                m_crText.x = xB;
                m_crText.y = yB;
                m_crText.width = contentsSize.x;
                m_crText.height = contentsSize.y;
            }
            if (crWidth != m_crText.width) {
                resetCaption();
            }
            if (partId != 0) {
                cWinMesh.request(parts, pos, col, 0, cr);
            }
            // テキスト.
            col = rtcolor * captionColor;
            pos.x += m_offset.x + captionOffset.x;
            pos.y += m_offset.y + captionOffset.y;

            m_cRichText.request(pos, col, m_crText);
        }
        //==========================================================================
        /*!ヒットを実行する.
            @brief	procHit
        */
        override public void procHit(CWindowMgr cWindowMgr, CInput.e_State eState) {
            // 押されているのでコールバックを発行.
            CInput cInput = cWindowMgr.input;
            if (cInput == null) {
                return;
            }
            switch (eState) {
            case CInput.e_State.Release:
                // 離す瞬間をクリックとする.
                m_cWindowBase.onClick(this);
                break;
            }
        }
        override protected CWindowMgr.CCollision checkHitContents(Vector3 touchPos, ClipRect cr, CStick stk) {
            CWindowMgr.CCollision col = m_cRichText.checkHit(touchPos, m_absPosition, stk);
            if (col == null) {
                return new CWindowMgr.CCollision(this);
            }
            return null;
        }
        //==========================================================================
        /*!パーツを再更新する必要があるとき呼ばれる.
            @brief	onResetParts
        */
        override public void onResetParts(int iTexIndex) {
            if (iTexIndex != 0) {
                return;
            }
            if (partId != 0) {
                m_aParts[0] = windowMesh.create(new FiveCC(partId), size);
                m_aParts[1] = m_aParts[0];
            } else {
                m_aParts[0] = null;
                m_aParts[1] = null;
            }
            setContentsSize(m_aParts[0]);
        }
        protected void setContentsSize(Parts parts) {
            if (parts == null) {
                m_contentsSize.set(size);
                m_textOffset = Vector2.zero;
                return;
            }
            switch (parts.patch) {
            case e_Patch.None:
                m_contentsSize.set(size);
                m_textOffset = Vector2.zero;
                break;
            case e_Patch.H3:
                m_contentsSize.set(parts.part[1].vert.z - parts.part[0].vert.z, parts.part[1].vert.w);
                m_textOffset.x = parts.part[0].vert.z;
                m_textOffset.y = Mathf.Ceil(m_contentsSize.y.x - m_cTextOne.lineheight) * 0.5f;
                break;
            case e_Patch.HV9:
                m_contentsSize.set(parts.part[4].vert.z - parts.part[3].vert.z, parts.part[4].vert.w - parts.part[1].vert.w);
                m_textOffset.x = parts.part[0].vert.z;
                m_textOffset.y = parts.part[0].vert.w;
                break;
            }
            resetCaption();
        }
        //==========================================================================
        /*!キャプションパーツを再更新する必要があるとき呼ばれる.
            @brief	onResetCaption
        */
        override public void onResetCaption() {
            if (contentsSize.x == 0) {
                return;
            }
            if (m_cRichText.setText(caption, captionColor, lineFeedWidth, lineSpace)) {
                m_eUpdateFlag &= ~e_UpdateFlag.CAPTION;
            }
        }
        override public void setTextureSize(int index, Vector3 x, Vector3 y) {
            base.setTextureSize(index, x, y);
            resetCaption();
        }
        override public void setContentsSize(Vector3 x, Vector3 y) {
            base.setContentsSize(x, y);
            resetCaption();
        }
        override public Vector2 viewsize {
            get {
                return new Vector2(m_crText.width, m_crText.height);
            }
        }
        override public Vector2 screensize {
            get {
                return new Vector2(m_cRichText.width, m_cRichText.height);
            }
        }
        override public Vector2 captionOffset {
            get {
                return getRatioValue(m_captionOffset) + m_textOffset;
            }
            set {
                base.captionOffset = value;
            }
        }

    }
}
