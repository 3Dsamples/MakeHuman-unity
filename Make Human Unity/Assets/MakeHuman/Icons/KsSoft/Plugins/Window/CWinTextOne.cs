//==============================================================================================
/*!?ウィンドウ用テキスト一行分を保持する.
	@file  CWinTextOne
	
	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KS {
    public class CWinTextOne {
        public enum e_Style {
            Normal,
            Bold,
            Dent,
            Shadow,
        };
        protected string m_text;
        protected CSpriteFont m_sprFont;
        protected t_SpriteChar[] m_aSprChar = new t_SpriteChar[0];
        protected Vector3[] m_aCharOffset = new Vector3[0];
        protected WinColor[] m_aColor = new WinColor[0];
        protected float m_width = 0;
        protected float m_maxwidth = 0;
        protected float m_height = 0;
        protected float m_linefeed = 0;
        protected int m_line = 0;
        protected int m_length = 0;
        protected e_Style m_eStyle = e_Style.Normal;
        protected e_Anchor m_eAnchor = e_Anchor.Left;
        protected float m_space = 0;

        //==========================================================================
        /*!Constructor
            @brief	Constructor
        */
        public CWinTextOne() {
        }
        //==========================================================================
        /*!Copy Constructor
            @brief	Copy Constructor
        */
        public CWinTextOne(CWinTextOne src) {
            initialize(src.sprFont);
        }
        //==========================================================================
        /*!Initilaize
            @brief	initilaize
        */
        public bool initialize(CSpriteFont sprFont) {
            if (sprFont == null) {
                return false;
            }
            m_sprFont = sprFont;
            return true;
        }
        //==========================================================================
        /*!テキストを登録する.
            @brief	setText
        */
        virtual public void setText(string text, float space, float linefeedHeight, bool bForce, float width) {
            if (text == null) {
                text = "";
            }
            if (!bForce) {
                if (m_text == text && m_space == space && m_maxwidth == width) {
                    return;
                }
            }
            m_line = 0;
            m_text = text;
            // 文字列の長さを取得する.		
            int len = 0;
            for (int i = 0; i < text.Length; i++) {
                if (text[i] == 0) {
                    break;
                }
                len++;
            }
            if (width == 0f) {
                width = 4096f;
            }
            linefeedHeight += m_sprFont.lineHeight;

            m_aSprChar = new t_SpriteChar[len];
            m_aCharOffset = new Vector3[len];
            m_aColor = new WinColor[len];
            m_maxwidth = width;
            m_width = 0f;
            m_space = space;
            m_linefeed = linefeedHeight;
            m_length = 0;
            float xoffset = 0f;
            float xmax = 0f;
            float yoffset = 0f;
            float[] aWidth = new float[16];
            int[] aLine = new int[16];
            for (int i = 0; i < len; i++) {
                char ch = text[i];
                if (ch == '\n' || ch == '\r') {
                    if (m_line >= aWidth.Length) {
                        System.Array.Resize(ref aWidth, aWidth.Length + 16);
                        System.Array.Resize(ref aLine, aLine.Length + 16);
                    }
                    aWidth[m_line] = xoffset - space;
                    aLine[m_line] = m_length;
                    xmax = Mathf.Max(xoffset - space, xmax);
                    m_aSprChar[m_length] = null;
                    m_aCharOffset[m_length].x = xoffset;
                    m_aCharOffset[m_length].y = yoffset;
                    m_aCharOffset[m_length].z = yoffset;
                    m_length++;
                    xoffset = 0f;
                    yoffset += m_linefeed;
                    m_line++;
                    continue;
                }
                t_SpriteChar sc = m_sprFont.getSpriteChar(ch);
                if (sc == null) {
                    Debug.LogError("this character code is illegal:" + ch + "(" + (int)ch + ")");
                    continue;
                }
                if (sc.xAdvance + xoffset > width) {
                    if (m_line >= aWidth.Length) {
                        System.Array.Resize(ref aWidth, aWidth.Length + 16);
                        System.Array.Resize(ref aLine, aLine.Length + 16);
                    }
                    aWidth[m_line] = xoffset - space;
                    aLine[m_line] = m_length;
                    xmax = Mathf.Max(xoffset - space, xmax);
                    xoffset = 0f;
                    yoffset += m_linefeed;
                    m_line++;
                }
                m_aSprChar[m_length] = sc;
                m_aCharOffset[m_length].x = xoffset + sc.xOffset;
                m_aCharOffset[m_length].y = yoffset - sc.yOffset;
                m_aCharOffset[m_length].z = yoffset;
                xoffset += sc.xAdvance + space;
                m_length++;
            }
            if (m_line >= aWidth.Length) {
                System.Array.Resize(ref aWidth, aWidth.Length + 16);
                System.Array.Resize(ref aLine, aLine.Length + 16);
            }
            aWidth[m_line] = xoffset - space;
            aLine[m_line] = m_length;
            m_line++;

            m_width = Mathf.Max(xoffset - space, xmax);
            m_height = yoffset + m_sprFont.lineHeight;
            if (m_line > 1) {
                int idx = 0;
                if (anchor == e_Anchor.Center) {
                    for (int i = 0; i < aLine.Length; ++i) {
                        int ln = aLine[i];
                        float offset = Mathf.Round((m_width - aWidth[i]) * 0.5f);
                        for (; idx < ln; ++idx) {
                            m_aCharOffset[idx].x += offset;
                        }
                    }
                } else if (anchor == e_Anchor.Right) {
                    for (int i = 0; i < aLine.Length; ++i) {
                        int ln = aLine[i];
                        float offset = m_width - aWidth[i];
                        for (; idx < ln; ++idx) {
                            m_aCharOffset[idx].x += offset;
                        }
                    }
                }
            }
        }
        //==========================================================================
        /*!文字一つの情報を取得する.
            @brief	getSpriteChar
        */
        virtual public t_SpriteChar getSpriteChar(out Vector3 rOffset, int index, ref WinColor color) {
            if (index > Length) {
                rOffset = Vector3.zero;
                return null;
            } else if (index == Length) {
                --index;
                if (index >= 0) {
                    rOffset = m_aCharOffset[index];
                    t_SpriteChar sc = m_aSprChar[index];
                    if (sc != null) {
                        rOffset.x += sc.xOffset + sc.xAdvance;
                    }
                } else {
                    rOffset = Vector3.zero;
                }
                return null;
            }
            rOffset = m_aCharOffset[index];
            return m_aSprChar[index];
        }
        //==========================================================================
        /*!文字列のオフセットの位置を取得.
            @brief	getOffset
        */
        public Vector3 getOffset(int index) {
            if (index > Length) {
                return Vector3.zero;
            }
            if (index == Length) {
                --index;
                if (index >= 0) {
                    Vector3 offset = m_aCharOffset[index];
                    t_SpriteChar sc = m_aSprChar[index];
                    if (sc != null) {
                        offset.x += sc.xOffset + sc.xAdvance;
                    }
                    return offset;
                }
                return Vector3.zero;
            }
            return m_aCharOffset[index];
        }
        //==========================================================================
        /*!指定された座標のうち一番近い場所を探し、挿入ポイントにする.
            @brief	findNearestInsertionPoint
        */
        public int findNearestInsertionPoint(Vector3 offset) {
            int iFind = 0;
            WinColor color = WinColor.black;
            float minDistance = 65536f * 65536f;
            for (int i = 0; i <= m_text.Length; i++) {
                Vector3 ofs;
                getSpriteChar(out ofs, i, ref color);
                float dx = ofs.x - offset.x;
                float dy = (ofs.z - offset.z) * 1024f;
                float d = dx * dx + dy * dy;
                if (d < minDistance) {
                    iFind = i;
                    minDistance = d;
                }
            }
            return iFind;
        }
        //==========================================================================
        /*!指定された座標のうち一番近い場所を探し、挿入ポイントにする.
            @brief	findNearestInsertionPoint
            @note	クリックした中心の位置から計測する.
        */
        public int findNearestInsertionPoint(Vector2 offset) {
            int iFind = 0;
            WinColor color = WinColor.black;
            float minDistance = 65536f * 65536f;
            for (int i = 0; i <= m_text.Length; i++) {
                Vector3 ofs;
                getSpriteChar(out ofs, i, ref color);
                float dx = ofs.x - offset.x;
                float dy = (ofs.z + linefeed * 0.5f - offset.y) * 1024f;
                float d = dx * dx + dy * dy;
                if (d < minDistance) {
                    iFind = i;
                    minDistance = d;
                }
            }
            return iFind;
        }
        public int Length {
            get {
                return m_length;
            }
        }
        public float space {
            get {
                switch (m_eStyle) {
                case e_Style.Bold:
                    return m_space + 1f;
                case e_Style.Shadow:
                    return m_space + 1f;
                }
                return m_space;
            }
        }
        public float width {
            get {
                return m_width;
            }
        }
        public float height {
            get {
                return m_height;
            }
        }
        public string text {
            get {
                return m_text;
            }
            set {
                if (m_sprFont == null) {
                    m_text = value;
                } else {
                    setText(value, 1f, 0f, true, 4096f);
                }
            }
        }
        public e_Style style {
            get {
                return m_eStyle;
            }
            set {
                m_eStyle = value;
            }
        }
        public e_Anchor anchor {
            get {
                return m_eAnchor;
            }
            set {
                m_eAnchor = value;
            }
        }
        public float linefeed {
            get {
                return m_linefeed;
            }
        }
        public float lineheight {
            get {
                if (m_sprFont != null) {
                    return m_sprFont.lineHeight;
                }
                return 0f;
            }
        }
        public int line {
            get {
                return m_line;
            }
        }
        public CSpriteFont sprFont {
            get {
                return m_sprFont;
            }
        }
    }
}
