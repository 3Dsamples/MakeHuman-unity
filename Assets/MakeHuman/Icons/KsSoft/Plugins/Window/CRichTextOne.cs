//==============================================================================================
/*!?リッチテキスト.
	@file  CRichText

	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace KS {
    public class CRichTextOne : CWinTextOne {
        public enum e_Cmd {
            String,
            Window,
            Texture,
            Image,  //download from network.
            Num,
        };
        struct t_TempBuffer {
            public t_SpriteChar sc;
            public int idx;
            public Vector2 offset;
        };
        e_Cmd m_eCmd;
        CWinCtrlBase m_cCtrl;
        CWinTextMesh m_cTextMesh;
        CWindowMesh m_cWindowMesh;
        Vector4 m_uv;
        Vector2 m_pos;
        Vector2 m_basepos;  //レンダリングしたときの座標.
        Vector2 m_size;
        float m_maxWidth;
        float m_maxHeight;
        float m_yoffset = 0f;
        WinColor m_color;
        uint m_texId = 0;
        uint m_partId = 0;
        uint m_mWindowId;
        uint m_mUserId;
        bool m_bCollision = false;
        bool m_bIndent = false;
        List<t_TempBuffer> m_lstTempBuffer = null;
        CWindowMesh m_wmImage = null;
        CAssetBundle m_cAB = null;

        public CRichTextOne(CWinTextMesh cTextMesh, e_Cmd eCmd, CWinCtrlBase cCtrl, WinColor color, e_Style eStyle, e_Anchor eAnchor, float space, float maxWidth) {
            m_eCmd = eCmd;
            m_eAnchor = eAnchor;
            switch (m_eCmd) {
            case e_Cmd.Window:
                m_bCollision = true;
                break;
            case e_Cmd.Texture:
                m_bCollision = true;
                break;
            }
            m_cCtrl = cCtrl;
            m_space = space;
            m_color = color;
            m_eStyle = eStyle;
            m_maxWidth = maxWidth;
            m_mWindowId = 0;
            m_mUserId = 0;
            m_cTextMesh = cTextMesh;
            m_cWindowMesh = null;
            initialize(cTextMesh.spriteFont);
        }
        //==========================================================================
        /*!解放.
            @brief	release
        */
        public void release() {
            if (m_wmImage != null) {
                GameObject.DestroyImmediate(m_wmImage.gameObject);
                m_wmImage = null;
            }
            if (m_cAB != null) {
                m_cAB.release();
            }
        }
        //==========================================================================
        /*!一文字追加する.
            @brief	append
        */
        public int append(ref Vector2 rPos, char c, int idx) {
            if (m_lstTempBuffer == null) {
                m_lstTempBuffer = new List<t_TempBuffer>();
            }
            t_SpriteChar sc = m_sprFont.getSpriteChar(c);
            if (sc == null) {
                Debug.LogWarning("character code is illegal:" + c + "(" + (int)c + ")");
                return -1;
            }
            if (m_lstTempBuffer.Count == 0) {
                m_width = -space;
                rPos.x -= space;
                m_size.x = 0f;
                m_size.y = m_sprFont.baseHeight;
            }
            if (isAutoIndent) {
                if (rPos.x + space + sc.xAdvance > m_maxWidth) {
                    if (!isHyphenation) {
                        rPos.x = 0f;
                        m_bIndent = true;
                        return idx - 1;
                    }
                    if (CMessageDataSheetMgr.Instance.language == SystemLanguage.Japanese) {
                        if (!CHyphenation.isNonstart(c)) {
                            int resize = -1;
                            for (int i = m_lstTempBuffer.Count - 1; i >= 1; --i) {
                                if (!CHyphenation.isNonend((char)m_lstTempBuffer[i].sc.id)) {
                                    break;
                                }
                                resize = i;
                                idx = m_lstTempBuffer[i].idx;
                            }
                            if (resize >= 0) {
                                m_lstTempBuffer.RemoveRange(resize, m_lstTempBuffer.Count - resize);
                            }
                            rPos.x = 0f;
                            m_bIndent = true;
                            return idx - 1;
                        }
                    } else {    //スペースのみでの禁則.
                        int resize = 0;
                        for (int i = m_lstTempBuffer.Count - 1; i >= 0; --i) {
                            if ((char)m_lstTempBuffer[i].sc.id == ' ') {
                                resize = i;
                                idx = m_lstTempBuffer[i].idx;
                                break;
                            }
                        }
                        if (resize == 0) {
                            //インデントする場所が見つからないときは切らない.
                            if (m_lstTempBuffer.Count > 0) {
                                idx = m_lstTempBuffer[m_lstTempBuffer.Count - 1].idx;
                            }
                        } else {
                            m_lstTempBuffer.RemoveRange(resize + 1, m_lstTempBuffer.Count - resize - 1);
                        }
                        m_bIndent = true;
                        rPos.x = 0f;
                        return idx;
                    }
                }
            }
            rPos.x += space;
            t_TempBuffer tTemp;
            tTemp.sc = sc;
            tTemp.idx = idx;
            tTemp.offset.x = sc.xOffset + rPos.x;
            tTemp.offset.y = -sc.yOffset + rPos.y;

            m_lstTempBuffer.Add(tTemp);
            rPos.x += sc.xAdvance;
            m_size.x = Mathf.Max(m_size.x, rPos.x);
            return idx;
        }
        //==========================================================================
        /*!y方向の整形を行い出力可能な形にする(下辺に合わせる).
            @brief	end
        */
        public bool end(float h) {
            if (m_eCmd != e_Cmd.Texture) {
                // テクスチャ以外.
                if (m_lstTempBuffer == null || m_lstTempBuffer.Count == 0) {
                    return false;
                }
                int n = m_lstTempBuffer.Count;
                m_aSprChar = new t_SpriteChar[n];
                m_aCharOffset = new Vector3[n];
                int i = 0;
                foreach (t_TempBuffer tTemp in m_lstTempBuffer) {
                    m_aSprChar[i] = tTemp.sc;
                    m_aCharOffset[i] = tTemp.offset;
                    if (m_eAnchor == e_Anchor.Center || m_eAnchor == e_Anchor.LeftCenter || m_eAnchor == e_Anchor.RightCenter) {
                        m_aCharOffset[i].y += (h - m_size.y) * 0.5f;
                    } else if (m_eAnchor == e_Anchor.Bottom || m_eAnchor == e_Anchor.LeftBottom || m_eAnchor == e_Anchor.RightBottom) {
                        m_aCharOffset[i].y += h - m_size.y;
                    }
                    i++;
                }
                m_pos = m_aCharOffset[0];
                m_length = n;
                m_lstTempBuffer.Clear();
            } else {
                // テクスチャ.
                if (m_eAnchor == e_Anchor.Center || m_eAnchor == e_Anchor.LeftCenter || m_eAnchor == e_Anchor.RightCenter) {
                    m_pos.y += (h - m_size.y) * 0.5f;
                } else if (m_eAnchor == e_Anchor.Bottom || m_eAnchor == e_Anchor.LeftBottom || m_eAnchor == e_Anchor.RightBottom) {
                    m_pos.y += h - m_size.y;
                }
            }
            return true;
        }
        //==========================================================================
        /*!テクスチャとしてセットする.
            @brief	setTexture
        */
        public bool setTexture(ref Vector2 rPos, uint mTexId, uint partId, float w, float h, float yoffset) {
            bool isIndent = false;
            if (rPos.x + w > m_maxWidth) {
                rPos.x = 0f;
                isIndent = true;
            }
            m_texId = mTexId;
            m_partId = partId;
            m_pos = rPos;
            m_size.x = w;
            m_size.y = h;
            m_yoffset = yoffset;
            rPos.x += w;
            return isIndent;
        }
        //==========================================================================
        /*!イメージをネットワークからダウンロードする.
            @brief	setImage
        */
        public bool setImage(ref Vector2 rPos, string path, float w, float h, float yoffset) {
            bool isIndent = false;
            if (rPos.x + w > m_maxWidth) {
                rPos.x = 0f;
                isIndent = true;
            }
            release();
            while (true) {
                CAssetBundleMgr mgr = CAssetBundleMgr.Instance;
                if (mgr == null) {
                    Debug.LogError("need assetbundle mgr to load image.:" + path);
                    break;
                }
                CAssetBundle cAB = mgr.reference(path);
                if (cAB == null) {
                    Debug.LogError("can't find image:" + path);
                    break;
                }
                if (cAB.width == 0 || cAB.height == 0) {
                    Debug.LogError("this file is not image:" + path);
                    break;
                }
                m_cAB = cAB;
                if (w == 0f) {
                    w = (float)cAB.width;
                } else if (w != Mathf.Floor(w)) {
                    w *= (float)cAB.width;
                }
                if (h == 0f) {
                    h = (float)cAB.height;
                } else if (h != Mathf.Floor(h)) {
                    h *= (float)cAB.height;
                }
                break;
            }
            m_pos = rPos;
            m_size.x = w;
            m_size.y = h;
            m_uv = new Vector4(0f, 0f, 1f, 1f);
            m_yoffset = yoffset;
            rPos.x += w;
            return isIndent;
        }
        //==========================================================================
        /*!描画の発行を行う.
            @brief	request
        */
        public void request(Vector3 basepos, WinColor color, ClipRect cr) {
            color = color * this.color;
            switch (m_eCmd) {
            case e_Cmd.Texture:
                if (m_cWindowMesh == null) {
                    CWindowMesh cWM = m_cCtrl.window.getWindowMesh(m_texId);
                    if (cWM != null && !cWM.isLoaded) {
                        return;
                    }
                    m_cWindowMesh = cWM;
                    m_uv = cWM.reference(m_partId);
                }
                if (m_cWindowMesh != null) {
                    basepos.x += m_pos.x;
                    basepos.y += m_pos.y + m_yoffset;
                    m_cWindowMesh.request(basepos, color, m_uv, m_size, 0, cr);
                }
                break;
            case e_Cmd.Image:
                if (m_wmImage == null && m_cAB != null && m_cAB.isLoaded) {
                    m_wmImage = CWindowMesh.create(0, null, m_cCtrl.window.transform, -120f / 65536f);
                    Material m = new Material(Shader.Find("Custom UI/Alpha"));
                    m.SetTexture("_MainTex", m_cAB.texture);
                    m_wmImage.initialize(m);
                }
                if (m_wmImage != null) {
                    basepos.x += m_pos.x;
                    basepos.y += m_pos.y + m_yoffset;
                    m_wmImage.request(basepos, color, m_uv, m_size, 0, cr);
                }
                break;
            default:
                if (m_cTextMesh != null) {
                    m_cTextMesh.request(this, basepos, color, cr);
                }
                break;
            }
        }
        //==========================================================================
        /*!ヒットチェックする.
            @brief	checkHit
        */
        public CWindowMgr.CCollision checkHit(Vector2 touchPos, Vector2 basepos) {
            if (m_bCollision) {
                basepos += m_pos;
                basepos.y += m_yoffset;
                if (basepos.x <= touchPos.x && touchPos.x <= basepos.x + m_size.x &&
                    basepos.y <= touchPos.y && touchPos.y <= basepos.y + m_size.y) {
                    return new CWindowMgr.CCollision(this);
                }
            }
            return null;
        }
        //==========================================================================
        /*!ヒット処理を行う.
            @brief	procHit
        */
        public void procHit(CWindowMgr cWindowMgr, CInput.e_State eState) {
            if (eState == CInput.e_State.Release) {
                ctrl.window.onClick(ctrl, this);
            }
        }
        protected bool isAutoIndent {
            get {
                if (m_eCmd == e_Cmd.String) {
                    return true;
                }
                return false;
            }
        }
        protected bool isHyphenation {
            get {
                if ((m_cCtrl.style & e_WinCtrlStyle.TEXT_NOHYPHENATION) != 0) {
                    return false;
                }
                return true;
            }
        }

        public CWinCtrlBase ctrl {
            get {
                return m_cCtrl;
            }
        }
        public bool isIndent {
            get {
                return m_bIndent;
            }
            set {
                m_bIndent = value;
            }
        }
        public int Count {
            get {
                return m_lstTempBuffer.Count;
            }
        }
        public e_Cmd cmd {
            get {
                return m_eCmd;
            }
        }
        public WinColor color {
            get {
                return m_color;
            }
        }
        public uint windowId {
            get {
                return m_mWindowId;
            }
            set {
                m_mWindowId = value;
            }
        }
        public uint userId {
            get {
                return m_mUserId;
            }
            set {
                m_mUserId = value;
            }
        }
        public uint texId {
            get {
                return m_texId;
            }
        }
        public uint partId {
            get {
                return m_partId;
            }
        }
        public Vector2 size {
            get {
                return m_size;
            }
            set {
                m_size = value;
            }
        }
        public Vector2 position {
            get {
                return m_pos;
            }
        }
    }
}
