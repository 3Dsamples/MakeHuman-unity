//==============================================================================================
/*!メッセージダイアログシステムウィンドウ.
	@file	CMessageBox

	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System;
using KS;

public delegate bool DlMessageBox(CMessageBox.e_Kind eKind, int iMsgBoxId);
public class CMessageBox : CMessageBoxBase {
    public enum e_Type {
        None,
        Ok,
        OkCancel,
        YesNo,
        YesNoCancel,
    };
    public enum e_Kind {
        Ok,
        Yes,
        No,
        Cancel,
    };
    const int CANCEL = 0;
    const int NO = 1;
    const int OK = 2;
    const int YES = 3;
    const int NUM = 4;
    protected int m_msgBoxId = 0;
    protected e_Type m_eType = e_Type.None;
    CWinCtrlBase m_cCaption = null;
    CWinCtrlBase[] m_aButton = null;
    bool m_bClick = false;
    IMessageBox m_iMessageBox = null;
    DlMessageBox m_dlMessageBox = null;
    //==========================================================================
    /*!create
        @brief	create MessageBox
    */
    static public CMessageBox create(string msg, e_Type eType, int iMsgBoxId, IMessageBox iMB = null) {
        CMessageBox mb;
        if (iMB is CWindowBase) {
            mb = create(iMB as CWindowBase);
        } else {
            mb = create();
        }
        mb.m_iMessageBox = iMB;
        mb.m_msgBoxId = iMsgBoxId;
        mb.type = eType;
        mb.caption = msg;
        return mb;
    }
    static public CMessageBox create(string msg, e_Type eType, int iMsgBoxId, DlMessageBox dlMessageBox) {
        CMessageBox mb = create();
        mb.m_msgBoxId = iMsgBoxId;
        mb.type = eType;
        mb.caption = msg;
        mb.m_dlMessageBox = dlMessageBox;
        return mb;
    }
    //==========================================================================
    /*!create
        @brief	create MessageBox
    */
    static public CMessageBox create(uint mMsgId, e_Type eType, int iMsgBoxId, IMessageBox cParent = null) {
        return create(CWindowMgr.Instance.getCaption(mMsgId), eType, iMsgBoxId, cParent);
    }
    static public CMessageBox create(uint mMsgId, e_Type eType, int iMsgBoxId, DlMessageBox dlMessageBox) {
        return create(CWindowMgr.Instance.getCaption(mMsgId), eType, iMsgBoxId, dlMessageBox);
    }
    //==========================================================================
    /*!onCreate
        @brief	Window Callback
    */
    public override void onCreate() {
        m_cCaption = find(RICHTEXT_MESSAGE);
        if (base.caption != null) {
            m_cCaption.caption = base.caption;
        }
        m_bClick = false;
    }
    //==========================================================================
    /*!onUpdate
        @brief	Window Callback
    */
    public override void onUpdate() {
        height = m_cCaption.height + 24f + 300f;
    }
    //==========================================================================
    /*!onClick
        @brief	Window Callback
    */
    public override void onClick(CWinCtrlBase cCtrl) {
        m_bClick = true;
        e_Kind eKind = e_Kind.Yes;
        switch (cCtrl.id) {
        case BUTTON_YES:
            eKind = e_Kind.Yes;
            break;
        case BUTTON_NO:
            eKind = e_Kind.No;
            break;
        case BUTTON_CANCEL:
            eKind = e_Kind.Cancel;
            break;
        case BUTTON_OK:
            eKind = e_Kind.Ok;
            break;
        default:
            return;
        }
        if (m_dlMessageBox != null) {
            m_dlMessageBox(eKind, m_msgBoxId);
        }
        if (m_iMessageBox != null) {
            m_iMessageBox.iMessageBox(eKind, m_msgBoxId);
        }
        close();
    }
    //==========================================================================
    /*!onClick
        @brief	Window Callback
    */
    public override bool onClose(int iCloseInfo) {
        if (!m_bClick) {
            if (m_dlMessageBox != null) {
                return m_dlMessageBox(e_Kind.Cancel, m_msgBoxId);
            }
            if (m_iMessageBox != null) {
                m_iMessageBox.iMessageBox(e_Kind.Cancel, m_msgBoxId);
            }
            if (parent != null) {
                return parent.onCancel(m_msgBoxId);
            }
        }
        return true;
    }
    //==========================================================================
    /*!キャプション.
        @brief	caption
    */
    override public string caption {
        get {
            if (m_cCaption == null) {
                return base.caption;
            }
            return m_cCaption.caption;
        }
        set {
            if (m_cCaption != null) {
                m_cCaption.caption = value;
            }
            base.caption = value;
        }
    }
    public void setCaption(e_Kind eKind, string str) {
        int idx = OK;
        switch (eKind) {
        case e_Kind.Ok:
            idx = OK;
            break;
        case e_Kind.No:
            idx = NO;
            break;
        case e_Kind.Cancel:
            idx = CANCEL;
            break;
        case e_Kind.Yes:
            idx = YES;
            break;
        }
        m_aButton[idx].caption = str;
    }
    //==========================================================================
    /*!ボタンのタイプを設定する.
        @brief	type
    */
    public e_Type type {
        get {
            return m_eType;
        }
        set {
            if (m_aButton == null) {
                m_aButton = new CWinCtrlBase[NUM];
                m_aButton[OK] = find(BUTTON_OK);
                m_aButton[CANCEL] = find(BUTTON_CANCEL);
                m_aButton[YES] = find(BUTTON_YES);
                m_aButton[NO] = find(BUTTON_NO);
            }
            m_eType = value;
            int mask = 0;
            switch (m_eType) {
            case e_Type.None:
                break;
            case e_Type.Ok:
                mask = (1 << OK);
                break;
            case e_Type.OkCancel:
                mask = (1 << OK) | (1 << CANCEL);
                break;
            case e_Type.YesNo:
                mask = (1 << YES) | (1 << NO);
                break;
            case e_Type.YesNoCancel:
                mask = (1 << YES) | (1 << NO) | (1 << CANCEL);
                break;
            }
            int n = 0;
            for (int i = 0; i < NUM; i++) {
                if ((mask & (1 << i)) == 0) {
                    m_aButton[i].hide = true;
                } else {
                    m_aButton[i].hide = false;
                    n++;
                }
            }
            float bx = 0f;
            float w = 0f;
            switch (n) {
            case 0:
                return;
            case 2:
                bx = -160f;
                w = 320f;
                break;
            case 3:
                bx = -240f;
                w = 240f;
                break;
            }
            for (int i = 0; i < NUM; i++) {
                if ((mask & (1 << i)) != 0) {
                    Vector3 pos = m_aButton[i].position;
                    pos.x = bx;
                    m_aButton[i].position = pos;
                    bx += w;
                }
            }
        }
    }
}
