//==============================================================================================
/*!?エディットボックスコントロール.
	@file  CWinCtrlEditbox

	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KS {
    public class CWinCtrlEditbox : CWinCtrlTextbox, IWinKeyFocusable {
        protected bool m_bCommitOnLostFocus = false;
        // for iPhone Keyboard Type
#if UNITY_IOS || UNITY_ANDROID
	protected TouchScreenKeyboardType m_type = TouchScreenKeyboardType.Default;
#endif
        protected Parts m_partsCursor = null;
        protected int m_insert;
        protected int m_selection = -1;
        protected int m_crBlink = 0;
        protected bool m_bAdjustCursor = false; //カーソルがウィンドウ内に入るように調整する.
        protected bool m_autoCorrect = false;
        protected bool m_secure = false;
        protected bool m_alert = false;
        protected bool m_hideInput = false;
        protected bool m_bEditable = true;
        //==========================================================================
        /*!Constructor
            @brief	Constructor
        */
        public CWinCtrlEditbox(CWindowBase cWindowBase) : base(cWindowBase, e_WinCtrlKind.EDITBOX) {
            m_aTexture[1].partId = new FiveCC("CURSR");
        }
        //==========================================================================
        /*!Copy Constructor
            @brief	Copy Constructor
        */
        public CWinCtrlEditbox(CWinCtrlEditbox src, CWinContents parent) : base(src, parent) {
        }
        //==========================================================================
        /*!解放時に呼び出される(関連づけられているリソースがあるときはオーバライドする).
            @brief	release
        */
        public override void release() {
            CWindowMgr cMgr = CWindowMgr.Instance;

            if (cMgr.focusObject == this) {
                cMgr.focusObject = null;
            }
            base.release();
        }
        override protected void updateContents(Vector3 pos) {
        }

        //==========================================================================
        /*!Render
            @brief	render
        */
        override public void render(Vector3 pos, CWindowMesh cWinMesh, CWinTextMesh cTextMesh, WinColor rtcolor, ClipRect cr) {
            base.render(pos, cWinMesh, cTextMesh, rtcolor, cr);
            renderCursor(pos, rtcolor);
        }
        protected void renderCursor(Vector3 pos, WinColor color) {
            if (!m_bHasFocus) {
                return;
            }
            if (m_partsCursor == null) {
                return;
            }
            CWindowMesh cWM = getWindowMesh(1);
            if (cWM == null) {
                return;
            }
            CSpriteFont cSF = m_cTextOne.sprFont;
            if (cSF == null) {
                return;
            }

            pos.x += captionOffset.x;
            pos.y += captionOffset.y;
            // カーソル描画.
            Vector3 ofsCur = m_cTextOne.getOffset(m_insert);
            Vector3 posCursor = calcCursorPos(pos, ofsCur);
            if (m_bAdjustCursor) {
                Vector2 ofsScroll = m_offset;
                //カーソルがコントロールの描画範囲に入るようにスクロールを調整する.
                if (m_crText.xMin > posCursor.x) {
                    ofsScroll.x += m_crText.xMin - posCursor.x;
                } else if (m_crText.xMax < posCursor.x + 8f) {
                    ofsScroll.x += m_crText.xMax - posCursor.x - 8f;
                }
                if (m_crText.yMin > posCursor.y) {
                    ofsScroll.y += m_crText.yMin - posCursor.y;
                } else if (m_crText.yMax < posCursor.y + m_cTextOne.linefeed) {
                    ofsScroll.y += m_crText.yMax - posCursor.y - m_cTextOne.linefeed;
                }
                setOffset(ofsScroll, Vector2.zero);
                posCursor = calcCursorPos(pos, ofsCur);
                m_bAdjustCursor = false;
            }
            m_crBlink += Angle.Deg2Ang(360f * Time.deltaTime);
            color *= color1;
            color.a = (byte)((Angle.Sin(m_crBlink) + 1f) * 0.5f * (float)color.a);
            if (m_selection < 0 || m_selection == m_insert) {
                // 通常カーソル.
                cWM.resize(m_partsCursor, new Vector2(5f, cSF.baseHeight));
                cWM.request(m_partsCursor, posCursor, color, 0, m_crText);
            } else {
                // 範囲選択カーソル.
                int iStart = m_selection;
                int iEnd = m_insert;
                if (m_selection > m_insert) {
                    iStart = m_insert;
                    iEnd = m_selection;
                }
                WinColor col = color;
                Vector3 ofsStart, ofsPrev;
                float hPrev, hCur;
                m_cTextOne.getSpriteChar(out ofsStart, iStart, ref col);
                hPrev = ofsStart.z;
                ofsStart = calcCursorPos(pos, ofsStart);
                ofsPrev = ofsStart;
                t_SpriteChar tPrevSC = null;
                for (int i = iStart + 1; i <= iEnd; ++i) {
                    t_SpriteChar tSC = m_cTextOne.getSpriteChar(out ofsCur, i, ref col);
                    hCur = ofsCur.z;
                    ofsCur = calcCursorPos(pos, ofsCur);
                    if (hPrev != hCur) {
                        if (tPrevSC != null) {
                            ofsPrev.x += tPrevSC.xAdvance;
                        }
                        renderSelectionCursor(ofsStart, ofsPrev, cSF, cWM, color);
                        ofsStart = ofsCur;
                    }
                    ofsPrev = ofsCur;
                    hPrev = hCur;
                    tPrevSC = tSC;
                }
                if (ofsCur != ofsStart) {
                    renderSelectionCursor(ofsStart, ofsCur, cSF, cWM, color);
                }
            }
        }
        Vector3 calcCursorPos(Vector3 pos, Vector3 ofs) {
            pos.x += m_offset.x + ofs.x - 2f;
            pos.y += m_offset.y + ofs.z;
            return pos;
        }
        void renderSelectionCursor(Vector3 ofsStart, Vector3 ofsCur, CSpriteFont cSF, CWindowMesh cWM, WinColor color) {
            Vector2 size;
            size.x = ofsCur.x - ofsStart.x;
            size.y = cSF.baseHeight;
            if (size.x == 0f) {
                size.x = 5f;
            }
            cWM.resize(m_partsCursor, size);
            cWM.request(m_partsCursor, ofsStart, color, 0, m_crText);
        }

        //==========================================================================
        /*!ヒットを実行する.
            @brief	procHit
        */
        override public void procHit(CWindowMgr cWindowMgr, CInput.e_State eState) {
            // 押されているのでコールバックを発行.
            base.procHit(cWindowMgr, eState);

            if (cWindowMgr.focusObject == this) {
                m_bHasFocus = true;
                Vector2 pos = window.getLocalPos(cWindowMgr.stick.windowPos);

                pos.x -= captionOffset.x + m_offset.x + m_absPosition.x;
                pos.y -= captionOffset.y + m_offset.y + m_absPosition.y;
                m_insert = m_cTextOne.findNearestInsertionPoint(pos);
                cWindowMgr.insertionPoint = m_insert;
                cWindowMgr.selectionPoint = m_selection;
                m_bAdjustCursor = true;
            } else {
                m_bHasFocus = false;
            }
        }
        //==========================================================================
        /*!カーソルの上移動がおこなわれた.
            @brief	goUp
        */
        public void goUp() {
            CWindowMgr cMgr = CWindowMgr.Instance;
            Vector3 offset;
            WinColor color = WinColor.black;
            m_cTextOne.getSpriteChar(out offset, m_insert, ref color);
            offset.z -= linefeedheight;
            m_insert = m_cTextOne.findNearestInsertionPoint(offset);
            cMgr.insertionPoint = m_insert;
            m_bAdjustCursor = true;
        }
        //==========================================================================
        /*!カーソルの下移動がおこなわれた.
            @brief	goDown
        */
        public void goDown() {
            CWindowMgr cMgr = CWindowMgr.Instance;
            Vector3 offset;
            WinColor color = WinColor.black;
            m_cTextOne.getSpriteChar(out offset, m_insert, ref color);
            offset.z += linefeedheight;
            m_insert = m_cTextOne.findNearestInsertionPoint(offset);
            cMgr.insertionPoint = m_insert;
            m_bAdjustCursor = true;
        }
        //==========================================================================
        /*!入力済みの文字列を取得する(キーボードの情報も取得).
            @brief	getInputText
        */
        public string getInputText(ref t_KeyboardInfo info) {
            info.m_insert = m_insert;
#if UNITY_IOS || UNITY_ANDROID
		info.m_style = (uint) style;
		info.m_type = editype;
		info.m_autoCorrect = m_autoCorrect;
		info.m_multiline = isMultiline;
		info.m_secure = m_secure;
		info.m_alert = m_alert;
		info.m_hideInput = m_hideInput;
#endif
            return m_caption;
        }
        //==========================================================================
        /*!入力文字列を設定する.
            @brief	setInputText
            @param	inputText:	入力文字.
            @param	insertPt:	カーソル位置.
            @param	selectionPt:選択範囲開始位置.
        */
        public string setInputText(string inputText, ref int insertPt, ref int selectionPt) {
            // Validate our input:
            CWindowMgr cMgr = CWindowMgr.Instance;
            if (!isMultiline) {
                int idx;
                // Check for Enter:
                if ((idx = inputText.IndexOf('\n')) != -1) {
                    inputText = inputText.Remove(idx, 1);
                    cMgr.focusObject = null;
                }
                if ((idx = inputText.IndexOf('\r')) != -1) {
                    inputText = inputText.Remove(idx, 1);
                    cMgr.focusObject = null;
                }
            }
            int nClamp = -1;
            if (m_maxChar > 0) {
                nClamp = KsSoftUtility.clampZenHan(inputText, m_maxChar);
                if (nClamp >= 0) {
                    m_caption = inputText.Substring(0, nClamp);
                    m_insert = Mathf.Clamp(insertPt, 0, nClamp);
                    if (selectionPt >= 0) {
                        m_selection = Mathf.Clamp(selectionPt, 0, selectionPt);
                    } else {
                        m_selection = selectionPt;
                    }
                }
            }
            if (nClamp < 0) {
                caption = inputText;
                m_insert = insertPt;
                m_selection = selectionPt;
            }
            resetCaption();
            if (cMgr.focusObject == null && !m_bCommitOnLostFocus)
                commit();
            m_bAdjustCursor = true;
            return m_caption;
        }
        //==========================================================================
        /*!フォーカスを失った時の処理.
            @brief	lostFocus
        */
        public void lostFocus() {
            m_bHasFocus = false;

            if (m_bCommitOnLostFocus)
                commit();
        }
        //==========================================================================
        /*!入力文字列の確定が行われると呼ばれる.
            @brief	commit
        */
        public void commit() {
            window.onClickEnter(this);

            // フォーカスのチェック.
            CWindowMgr cWindowMgr = CWindowMgr.Instance;
            if (cWindowMgr.focusObject == this) {
                m_bHasFocus = true;
            } else {
                m_bHasFocus = false;
            }
        }
        //==========================================================================
        /*!キャラクタコードが有効かどうか判定する.
            @brief	validCharCode
        */
        public bool validCharCode(char c) {
            return textMesh.validCharCode(c);
        }
        //==========================================================================
        /*!文字列を返す.
            @brief	Content
        */
        public string Content {
            get { return m_caption; }
        }
        //==========================================================================
        /*!エディット可能かどうか?
            @brief editable
        */
        public bool editable {
            get {
                return m_bEditable;
            }
            set {
                m_bEditable = value;
            }
        }
        //==========================================================================
        /*!パーツを再更新する必要があるとき呼ばれる.
            @brief	onResetParts
        */
        override public void onResetParts(int iTexIndex) {
            if (iTexIndex > 1) {
                return;
            }
            Vector2 size = getTextureSize(iTexIndex);
            FiveCC partsid = new FiveCC(getPartId(iTexIndex));
            CWindowMesh cWM = getWindowMesh(iTexIndex);
            switch (iTexIndex) {
            case 0: //下地.
                getParts2State(m_aParts, 0);
                setContentsSize(m_aParts[0]);
                break;
            case 1: //カーソル.
                m_partsCursor = cWM.create(partsid, size);
                break;
            }
        }
        //==========================================================================
        /*!キャプションパーツを再更新する必要があるとき呼ばれる.
            @brief	onResetCaption
        */
        override public void onResetCaption() {
            if (m_crText.width == 0) {
                return;
            }
            string sText = caption;
            m_cTextOne.style = textStyle;
            // ブラインド文字列の場合は*で埋める.
            if ((m_eStyle & e_WinCtrlStyle.EDIT_BLIND) != 0) {
                sText = "";
                for (int i = 0; i < caption.Length; i++) {
                    sText += "*";
                }
            }
            if (isMultiline) {
                m_cTextOne.setText(sText, 1f, lineSpace, false, lineFeedWidth);
            } else {
                m_cTextOne.text = sText;
            }
            m_eUpdateFlag &= ~e_UpdateFlag.CAPTION;
        }
        //==========================================================================
        /*!キャプションパーツを再更新する必要があるとき呼ばれる.
            @brief	onResetCaption
        */
        public bool checkValidCaption() {
            CSpriteFont cSF = m_cTextMesh.spriteFont;
            if (cSF == null) {
                return false;
            }
            for (int i = 0; i < m_caption.Length; ++i) {
                if (cSF.getSpriteChar(m_caption[i]) == null) {
                    return false;
                }
            }
            return true;
        }
        public TouchScreenKeyboardType editype {
            get {
                switch (style & e_WinCtrlStyle.EDIT_TYPE_MASK) {
                case e_WinCtrlStyle.EDIT_TYPE_ALL:
                    return TouchScreenKeyboardType.Default;
                case e_WinCtrlStyle.EDIT_TYPE_ASCIICAPABLE:
                    return TouchScreenKeyboardType.ASCIICapable;
                case e_WinCtrlStyle.EDIT_TYPE_NUMBERANDPUNCTUATION:
                    return TouchScreenKeyboardType.NumbersAndPunctuation;
                case e_WinCtrlStyle.EDIT_TYPE_URL:
                    return TouchScreenKeyboardType.URL;
                case e_WinCtrlStyle.EDIT_TYPE_NUMBERPAD:
                    return TouchScreenKeyboardType.NumberPad;
                case e_WinCtrlStyle.EDIT_TYPE_PHONEPAD:
                    return TouchScreenKeyboardType.PhonePad;
                case e_WinCtrlStyle.EDIT_TYPE_NAMEPHONEPAD:
                    return TouchScreenKeyboardType.NamePhonePad;
                case e_WinCtrlStyle.EDIT_TYPE_EMAILADDRESS:
                    return TouchScreenKeyboardType.EmailAddress;
                }
                return TouchScreenKeyboardType.Default;
            }
            set {
                m_eStyle &= ~e_WinCtrlStyle.EDIT_TYPE_MASK;
                switch (value) {
                case TouchScreenKeyboardType.Default:
                    m_eStyle |= e_WinCtrlStyle.EDIT_TYPE_ALL;
                    break;
                case TouchScreenKeyboardType.ASCIICapable:
                    m_eStyle |= e_WinCtrlStyle.EDIT_TYPE_ASCIICAPABLE;
                    break;
                case TouchScreenKeyboardType.NumbersAndPunctuation:
                    m_eStyle |= e_WinCtrlStyle.EDIT_TYPE_NUMBERANDPUNCTUATION;
                    break;
                case TouchScreenKeyboardType.URL:
                    m_eStyle |= e_WinCtrlStyle.EDIT_TYPE_URL;
                    break;
                case TouchScreenKeyboardType.NumberPad:
                    m_eStyle |= e_WinCtrlStyle.EDIT_TYPE_NUMBERPAD;
                    break;
                case TouchScreenKeyboardType.PhonePad:
                    m_eStyle |= e_WinCtrlStyle.EDIT_TYPE_PHONEPAD;
                    break;
                case TouchScreenKeyboardType.NamePhonePad:
                    m_eStyle |= e_WinCtrlStyle.EDIT_TYPE_NAMEPHONEPAD;
                    break;
                case TouchScreenKeyboardType.EmailAddress:
                    m_eStyle |= e_WinCtrlStyle.EDIT_TYPE_EMAILADDRESS;
                    break;
                }
            }
        }
    }
}
