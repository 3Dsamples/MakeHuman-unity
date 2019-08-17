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
    public class CRichText {
        CRichTextLine[] m_aRTL = null;
        CWinTextOne.e_Style m_eStyle = CWinTextOne.e_Style.Normal;
        CWinCtrlBase m_cCtrl = null;
        float m_lineSpace = 1f;
        Vector2 m_size;
        //==========================================================================
        /*!Constructor.
            @brief	Constructor
        */
        public CRichText(CWinCtrlBase cCtrl) {
            m_cCtrl = cCtrl;
        }
        //==========================================================================
        /*!解放.
            @brief	release
        */
        public void release() {
            if (m_aRTL != null) {
                for (int i = 0; i < m_aRTL.Length; ++i) {
                    m_aRTL[i].release();
                }
                m_aRTL = null;
            }
        }
        //==========================================================================
        /*!描画の発行を行う.
            @brief	request
        */
        public void request(Vector3 pos, WinColor color, ClipRect cr) {
            if (m_aRTL == null) {
                return;
            }
            for (int i = 0; i < m_aRTL.Length; ++i) {
                CRichTextLine cRTL = m_aRTL[i];
                cRTL.request(m_cCtrl.window, pos, width, color, cr);
                pos.y += cRTL.height + m_lineSpace;
            }
        }
        //==========================================================================
        /*!ヒットチェックする.
            @brief	checkHit
        */
        public CWindowMgr.CCollision checkHit(Vector2 touchPos, Vector2 basepos, CStick stk) {
            if (m_aRTL == null) {
                return null;
            }
            for (int i = 0; i < m_aRTL.Length; ++i) {
                CRichTextLine cRTL = m_aRTL[i];
                CWindowMgr.CCollision cCollision = cRTL.checkHit(touchPos, basepos, width);
                if (cCollision != null) {
                    return cCollision;
                }
            }
            return null;
        }
        //==========================================================================
        /*!文字列を設定する(コマンドもパースする).
            @brief	setText
        */
        public bool setText(string sString, WinColor color, float width, float lineSpace) {
            if (sString == null) {
                sString = "";
            }
            CWinTextMesh cTextMesh = m_cCtrl.textMesh;
            if (cTextMesh == null) {
                return false;
            }
            m_lineSpace = lineSpace;
            if (width == 0f) {
                width = 4096f;
            }

            CRichTextOne cOne = null;
            float space = 1f;
            Vector2 pos = Vector2.zero;
            CWinTextOne.e_Style eStyle = m_eStyle;
            List<CRichTextOne> lstRichText = new List<CRichTextOne>();
            WinColor colDefault = color;
            e_Anchor eAnchor = m_cCtrl.textAnchor;
            bool res = true;
            for (int i = 0; i < sString.Length;) {
                char c = sString[i];
                if (c == '\n' || c == '\r') {
                    // 改行コードとして偽装する.
                    c = 'n';
                    ++i;
                } else if (c != '\\') {
                    if (cOne == null) {
                        cOne = new CRichTextOne(cTextMesh, CRichTextOne.e_Cmd.String, m_cCtrl, color, eStyle, eAnchor, space, width);
                    } else {
                        if (cOne.cmd != CRichTextOne.e_Cmd.String) {
                            lstRichText.Add(cOne);
                            cOne = new CRichTextOne(cTextMesh, CRichTextOne.e_Cmd.String, m_cCtrl, color, eStyle, eAnchor, space, width);
                        }
                    }
                    cOne = addChar(ref pos, ref i, c, cOne, lstRichText);
                    if (cOne == null) {
                        // インデントしたとき.
                        cOne = new CRichTextOne(cTextMesh, CRichTextOne.e_Cmd.String, m_cCtrl, color, eStyle, eAnchor, space, width);
                        //                    cOne = addChar(ref pos,ref i,c,cOne,lstRichText);
                    }
                    ++i;
                    continue;
                } else {
                    ++i;
                    if (i >= sString.Length) {
                        break;
                    }
                    c = sString[i++];
                }
                switch (c) {
                case '\\':  // \コード.
                    if (cOne == null) {
                        cOne = new CRichTextOne(cTextMesh, CRichTextOne.e_Cmd.String, m_cCtrl, color, eStyle, eAnchor, space, width);
                    }
                    --i;
                    cOne = addChar(ref pos, ref i, '\\', cOne, lstRichText);
                    ++i;
                    break;
                case 'n':       //改行.
                    if (cOne == null) {
                        cOne = new CRichTextOne(cTextMesh, CRichTextOne.e_Cmd.String, m_cCtrl, color, eStyle, eAnchor, space, width);
                        if (cOne.size.y == 0.0f) {
                            Vector2 sz = cOne.size;
                            sz.y = cTextMesh.spriteFont.lineHeight;
                            cOne.size = sz;
                        }
                    }
                    pos.x = 0f;
                    cOne.isIndent = true;
                    lstRichText.Add(cOne);
                    cOne = null;
                    break;
                case 'a':       //テキストアンカー変更.
                    if (cOne != null) {
                        lstRichText.Add(cOne);
                        int iOldAnchor = (int)eAnchor;
                        if (iOldAnchor > 0) {
                            iOldAnchor -= 1;
                        }
                        eAnchor = (e_Anchor)getNumber(sString, ref i);
                        int iAnchor = (int)eAnchor;
                        if (iAnchor > 0) {
                            iAnchor -= 1;
                        }
                        if (iAnchor / 3 != iOldAnchor / 3) {
                            // 横方向への揃え方が変更されたのでインデントが必要.
                            cOne.isIndent = true;
                            pos.x = 0f;
                        }
                        cOne = null;
                    } else {
                        eAnchor = (e_Anchor)getNumber(sString, ref i);
                    }
                    break;
                case 'c':     //カラーパレット.
                  {
                    if (cOne != null) {
                        lstRichText.Add(cOne);
                        cOne = null;
                    }
                    if (sString.Length > i && sString[i] == '[') {
                        int iColor = getNumber(sString, ref i);
                        if (iColor < 0) {
                            color = colDefault;
                        } else {
                            color = WinColor.getColor(iColor);
                        }
                    } else {
                        color = colDefault;
                    }
                }
                break;
                case 'C':       //カラー(16進数カラー).
                    if (cOne != null) {
                        lstRichText.Add(cOne);
                        cOne = null;
                    }
                    color = new WinColor(getHex(sString, ref i));
                    break;
                case 'f':   //フォント変更.
                    if (cOne != null) {
                        lstRichText.Add(cOne);
                        cOne = null;
                    }
                    cTextMesh = getTextMesh(sString, ref i);
                    if (cTextMesh == null) {
                        return false;
                    }
                    break;
                case 'w':
                    if (cOne != null) {
                        lstRichText.Add(cOne);
                        cOne = null;
                    }
                    cOne = getOpenWindow(cTextMesh, ref pos, sString, color, eStyle, eAnchor, width, ref i);
                    if (cOne == null) {
                        Debug.LogError("\\w :window cmd is illegal(" + i + ")");
                        return false;
                    }
                    break;
                case 't':
                    goto case 'T';
                case 'T':
                    if (cOne != null) {
                        lstRichText.Add(cOne);
                        cOne = null;
                    }
                    cOne = getTexture(cTextMesh, ref pos, eAnchor, sString, width, ref i, lstRichText);
                    if (cOne == null) {
                        Debug.LogError("\\t :texture cmd is illegal(" + i + ")");
                        return false;
                    }
                    break;
                case 'i':
                    if (cOne != null) {
                        lstRichText.Add(cOne);
                        cOne = null;
                    }
                    cOne = getImage(cTextMesh, ref pos, eAnchor, sString, width, ref i, lstRichText);
                    if (cOne == null) {
                        Debug.LogError("\\i :image cmd is illegal(" + i + ")");
                        return false;
                    }
                    break;
                case 'b':
                    goto case 'B';
                case 'B':
                    if (cOne != null) {
                        lstRichText.Add(cOne);
                        cOne = null;
                    }
                    eStyle = CWinTextOne.e_Style.Bold;
                    break;
                case 's':
                    goto case 'S';
                case 'S':
                    if (cOne != null) {
                        lstRichText.Add(cOne);
                        cOne = null;
                    }
                    eStyle = CWinTextOne.e_Style.Shadow;
                    break;
                case 'N':
                    if (cOne != null) {
                        lstRichText.Add(cOne);
                        cOne = null;
                    }
                    eStyle = CWinTextOne.e_Style.Normal;
                    break;
                default:
                    if (c == 'm') {
                        break;
                    }
                    res = false;
                    Debug.LogError("\\" + c + ":this control code is not support(" + i + ")");
                    break;
                }
            }
            if (cOne != null) {
                lstRichText.Add(cOne);
            }
            // 行形式に整形する.
            List<CRichTextOne> lstOneLine = new List<CRichTextOne>();
            List<CRichTextLine> lstRTL = new List<CRichTextLine>();
            foreach (CRichTextOne cRTO in lstRichText) {
                lstOneLine.Add(cRTO);
                if (cRTO.isIndent) {
                    lstRTL.Add(new CRichTextLine(lstOneLine.ToArray()));
                    lstOneLine.Clear();
                }
            }
            // 残りを解決.
            if (lstOneLine.Count != 0) {
                lstRTL.Add(new CRichTextLine(lstOneLine.ToArray()));
            }
            m_aRTL = lstRTL.ToArray();
            calcSize();
            return res;
        }
        //==========================================================================
        /*!一文字追加.
            @brief	addChar
        */
        CRichTextOne addChar(ref Vector2 rPos, ref int idx, char c, CRichTextOne cOne, List<CRichTextOne> lstRichTextOne) {
            int res = cOne.append(ref rPos, c, idx);
            if (res < 0) {
                return cOne;
            }
            idx = res;
            if (cOne.isIndent) {
                lstRichTextOne.Add(cOne);
                return null;
            } else if (cOne.Count == 0 && lstRichTextOne.Count > 0) {
                lstRichTextOne[lstRichTextOne.Count - 1].isIndent = true;
            }
            return cOne;
        }
        //==========================================================================
        /*!{～}で囲まれた文字列を取得する.
            @brief	getBlock
        */
        protected string getBlock(string sString, ref int i) {
            if (sString[i] != '[') {
                Debug.LogError("block token error");
                return "";
            }
            i++;
            int nest = 1;
            string sResult = "";
            while (nest != 0) {
                if (i >= sString.Length) {
                    Debug.LogError("block token error(" + i + ")");
                    return "";
                }
                char c = sString[i++];
                if (c == '\\') {
                    if (i >= sString.Length) {
                        Debug.LogError("block token error(" + i + ")");
                        return "";
                    }
                    c = sString[i++];
                } else {
                    if (c == '[') {
                        nest++;
                    } else if (c == ']') {
                        nest--;
                        if (nest == 0) {
                            break;
                        }
                    }
                }
                sResult += c;
            }
            return sResult;
        }
        //==========================================================================
        /*!{数字}を取得する.
            @brief	getNumber
        */
        protected int getNumber(string sString, ref int i) {
            string sBlock = getBlock(sString, ref i);
            int num;
            if (!int.TryParse(sBlock, out num)) {
                Debug.LogWarning("hex number error:" + sBlock);
                return 0;
            }
            return num;
        }
        //==========================================================================
        /*!{16進数}を取得する.
            @brief	getHex
        */
        protected uint getHex(string sString, ref int i) {
            string sBlock = getBlock(sString, ref i);
            uint hex;
            if (!uint.TryParse(sBlock, System.Globalization.NumberStyles.HexNumber, null, out hex)) {
                Debug.LogWarning("hex number error:" + sBlock);
                return 0;
            }
            return hex;
        }
        //==========================================================================
        /*!\?UserIdを取得する.
            @brief	getUserId
        */
        protected uint getUserId(string sString, ref int i) {
            uint userId = 0;
            for (; i < sString.Length; i++) {
                char c = sString[i];
                if (c < '0' || c > '9') break;
                userId = userId * 10 + (uint)(c - '0');
            }
            return userId;
        }
        //==========================================================================
        /*!\t{テクスチャID,パーツID,横,縦}を取得する.
            @brief	getTexture
        */
        protected CRichTextOne getTexture(CWinTextMesh cTextMesh, ref Vector2 basepos, e_Anchor eAnchor, string sString, float width, ref int i, List<CRichTextOne> lstRichTextOne) {
            CRichTextOne cRTO = new CRichTextOne(cTextMesh, CRichTextOne.e_Cmd.Texture, m_cCtrl, WinColor.white, CWinTextOne.e_Style.Normal, eAnchor, 0f, width);
            cRTO.userId = getUserId(sString, ref i);
            string sBlock = getBlock(sString, ref i);

            string[] aSplit = sBlock.Split(new char[] { ',' });
            if (aSplit.Length < 4) {
                Debug.LogError("texture format error" + sBlock);
                return null;
            }
            MulId mTexId = new MulId(aSplit[0]);
            FiveCC fcPart = new FiveCC(aSplit[1]);
            float w, h, yoffset;
            if (!float.TryParse(aSplit[2], out w)) {
                Debug.LogError("texture width is illegal:" + sBlock);
                return null;
            }
            if (!float.TryParse(aSplit[3], out h)) {
                Debug.LogError("texture height is illegal:" + sBlock);
                return null;
            }
            if (aSplit.Length >= 5) {
                if (!float.TryParse(aSplit[4], out yoffset)) {
                    Debug.LogError("texture height is illegal:" + sBlock);
                    return null;
                }
            } else {
                yoffset = 0;
            }

            if (cRTO.setTexture(ref basepos, mTexId, fcPart, w, h, yoffset)) {
                // インデントした.
                if (lstRichTextOne.Count >= 1) {
                    lstRichTextOne[lstRichTextOne.Count - 1].isIndent = true;
                }
            }
            return cRTO;
        }
        //==========================================================================
        /*!\t{テクスチャID,パーツID,横,縦}を取得する.
            @brief	getTexture
        */
        protected CRichTextOne getImage(CWinTextMesh cTextMesh, ref Vector2 basepos, e_Anchor eAnchor, string sString, float width, ref int i, List<CRichTextOne> lstRichTextOne) {
            CRichTextOne cRTO = new CRichTextOne(cTextMesh, CRichTextOne.e_Cmd.Image, m_cCtrl, WinColor.white, CWinTextOne.e_Style.Normal, eAnchor, 0f, width);
            cRTO.userId = getUserId(sString, ref i);
            string sBlock = getBlock(sString, ref i);

            string[] aSplit = sBlock.Split(new char[] { ',' });
            if (aSplit.Length < 4) {
                Debug.LogError("image format error" + sBlock);
                return null;
            }
            string path = aSplit[0];
            float w, h, yoffset;
            if (aSplit.Length >= 2) {
                if (!float.TryParse(aSplit[1], out yoffset)) {
                    Debug.LogError("image height is illegal:" + sBlock);
                    return null;
                }
            } else {
                yoffset = 0;
            }
            if (aSplit.Length >= 3) {
                if (!float.TryParse(aSplit[2], out w)) {
                    Debug.LogError("image width is illegal:" + sBlock);
                    return null;
                }
            } else {
                w = 0f;
            }
            if (aSplit.Length >= 4) {
                if (!float.TryParse(aSplit[3], out h)) {
                    Debug.LogError("image height is illegal:" + sBlock);
                    return null;
                }
            } else {
                h = 0f;
            }
            if (cRTO.setImage(ref basepos, path, w, h, yoffset)) {
                // インデントした.
                if (lstRichTextOne.Count >= 1) {
                    lstRichTextOne[lstRichTextOne.Count - 1].isIndent = true;
                }
            }
            return cRTO;
        }
        //==========================================================================
        /*!Windowをオープンする.
            @brief	getOpenWindow
        */
        protected CRichTextOne getOpenWindow(CWinTextMesh cTextMesh, ref Vector2 basepos, string sString, WinColor color, CWinTextOne.e_Style eStyle, e_Anchor eAnchor, float width, ref int i) {
            CRichTextOne cRTO = new CRichTextOne(cTextMesh, CRichTextOne.e_Cmd.Window, m_cCtrl, color, eStyle, eAnchor, 0f, width);
            cRTO.userId = getUserId(sString, ref i);
            string sBlock = getBlock(sString, ref i);

            string[] aSplit = sBlock.Split(new char[] { ',' });
            if (aSplit.Length != 2) {
                Debug.LogError("window format error" + sBlock);
                return null;
            }

            cRTO.windowId = MulId.Id(aSplit[0]);
            foreach (char c in aSplit[1]) {
                cRTO.append(ref basepos, c, 0);
            }
            return cRTO;
        }
        //==========================================================================
        /*!フォントに対応するテキストメッシュを取得する.
            @brief	getTextMesh
        */
        protected CWinTextMesh getTextMesh(string sString, ref int i) {
            string sBlock = getBlock(sString, ref i);

            if (!FiveCC.isFiveCC(sBlock)) {
                Debug.LogError("\\f " + sBlock + ": font cmd is illegal(" + i + ")");
                return null;
            }
            CWinTextMesh cTextMesh = m_cCtrl.window.getTextMesh(new FiveCC(sBlock));
            if (cTextMesh == null) {
                Debug.LogError("\\f " + sBlock + ": font cmd is illegal(" + i + ")");
            }
            return cTextMesh;
        }
        //==========================================================================
        /*!リッチテキスト全体のサイズを計算する.
            @brief	calcSize
        */
        public void calcSize() {
            float w = 0f;
            float h = 0f;
            for (int i = 0; i < m_aRTL.Length; ++i) {
                CRichTextLine cRTL = m_aRTL[i];

                w = Mathf.Max(cRTL.width, w);
                h += cRTL.height + m_lineSpace;
            }
            if (h > 0f) {
                h -= m_lineSpace;
            }
            m_size = new Vector2(w, h);
        }
        public CWinTextOne.e_Style style {
            get {
                return m_eStyle;
            }
            set {
                m_eStyle = value;
            }
        }
        public Vector2 size {
            get {
                return m_size;
            }
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
