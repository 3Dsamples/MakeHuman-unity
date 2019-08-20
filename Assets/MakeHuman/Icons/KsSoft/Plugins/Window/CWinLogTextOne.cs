//==============================================================================================
/*!?ログ用テキスト一行分を保持する.
	@file  CWinLogTextOne
	
	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KS {
    public class CWinLogTextOne : CWinTextOne {
        protected float m_newlineoffset = 8f;
        protected WinColor m_color;
        //==========================================================================
        /*!Constructor
            @brief Constructor
        */
        public CWinLogTextOne() {
        }
        //==========================================================================
        /*!Copy Constructor
            @brief	Copy Constructor
        */
        public CWinLogTextOne(CWinLogTextOne src) : base(src) {
        }
        //==========================================================================
        /*!テキストを登録する.
            @brief	setText
        */
        public void clear() {
            if (m_text == null || m_text.Length == 0) {
                return;
            }
            m_text = "";
            m_aSprChar = new t_SpriteChar[0];
            m_aCharOffset = new Vector3[0];
            m_width = 0;
            m_height = 0;
            m_line = 0;
        }

        //==========================================================================
        /*!テキストを登録する.
            @brief	setText
        */
        public void setText(string sentence, WinColor color, float space, float linefeedHeight, float width) {
            if (m_text == sentence && m_space == space) {
                return;
            }
            m_text = sentence;
            // 文字列の長さを取得する.		
            int len = text.Length;
            for (int i = 0; i < text.Length; i++) {
                if (text[i] == 0) {
                    len = i;
                }
            }
            if (width == 0f) {
                width = 4096f;
            }
            linefeedHeight += m_sprFont.lineHeight;
            m_color = color;
            m_aSprChar = new t_SpriteChar[len];
            m_aCharOffset = new Vector3[len];
            m_width = 0f;
            m_space = space;
            float xoffset = 0f;
            float xmax = 0f;
            float yoffset = 0f;
            m_line = 0;
            m_length = len;
            // 区切り文字の最後.
            int lastSepPos = -1;
            for (int i = 0; i < len; i++) {
                char ch = text[i];
                t_SpriteChar sc = m_sprFont.getSpriteChar(ch);
                if (sc == null) {
                    Debug.LogError("this character code is illegal:" + (int)ch);
                    continue;
                }
                // 自動改行チェック.
                m_aSprChar[i] = sc;
                if (xoffset + sc.xAdvance > width) {
                    xmax = Mathf.Max(xoffset - space, xmax);
                    // 改行が必要.
                    xoffset = m_newlineoffset;
                    yoffset += linefeedHeight;
                    if (lastSepPos >= 0) {
                        // 最後の区切り文字から改行させる.
                        for (int j = lastSepPos + 1; j < i; j++) {
                            m_aCharOffset[j].x = xoffset + m_aSprChar[j].xOffset;
                            m_aCharOffset[j].y = yoffset - m_aSprChar[j].yOffset;
                            m_aCharOffset[j].z = yoffset;
                            xoffset += m_aSprChar[j].xAdvance + space;
                        }
                        m_line++;
                    }
                }
                if (System.Char.IsSeparator(ch)) {
                    lastSepPos = i;
                }
                m_aSprChar[i] = sc;
                m_aCharOffset[i].x = xoffset + sc.xOffset;
                m_aCharOffset[i].y = yoffset - sc.yOffset;
                m_aCharOffset[i].z = yoffset;
                xoffset += sc.xAdvance + space;
            }
            m_line++;
            m_width = Mathf.Max(xoffset - space, xmax);
            m_height = yoffset + linefeedHeight;
        }
        //==========================================================================
        /*!文字一つの情報を取得する.
            @brief	getSpriteChar
        */
        override public t_SpriteChar getSpriteChar(out Vector3 rOffset, int index, ref WinColor color) {
            byte alpha = color.a;
            color = m_color;
            color.a = alpha;
            if (index > m_aSprChar.Length) {
                rOffset = Vector3.zero;
                return null;
            } else if (index == m_aSprChar.Length) {
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
    }
}
