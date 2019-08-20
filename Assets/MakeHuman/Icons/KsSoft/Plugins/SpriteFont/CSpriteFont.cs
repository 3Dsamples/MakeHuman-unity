//==============================================================================================
/*!?スプライトフォントデータ.
	@file  CSpriteFont
	
	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace KS {
    public class CSpriteFont {
        protected uint m_id;
        protected string m_face;
        protected Material m_material;
        protected float m_pxSize;
        protected float m_charSpacing;
        protected float m_lineHeight;
        protected float m_baseHeight;
        protected float m_texWidth;
        protected float m_texHeight;
        protected t_SpriteFont m_tFont;
        protected Dictionary<char, t_SpriteChar> m_dicChar = new Dictionary<char, t_SpriteChar>();

        //==========================================================================
        /*!Constructor
            @brief	CSpriteFont
        */
        public CSpriteFont(t_SpriteFont tFont, Texture2D texture, string sShader = "Custom UI/Font") {
            m_tFont = tFont;
            m_id = tFont.m_id;
            m_face = tFont.m_face;
            m_pxSize = (float)tFont.m_pxSize;
            m_charSpacing = (float)tFont.m_charSpacing;
            m_lineHeight = (float)tFont.m_lineHeight;
            m_baseHeight = (float)tFont.m_baseHeight;
            m_texWidth = (float)tFont.m_texWidth;
            m_texHeight = (float)tFont.m_texHeight;
            foreach (t_SpriteChar ch in tFont.m_aChar) {
                m_dicChar[(char)ch.id] = ch;
            }
            Shader shader = Shader.Find(sShader);
            m_material = new Material(shader);
            m_material.mainTexture = texture;
        }
        //==========================================================================
        /*!文字スプライト情報を取得する.
            @brief getSpriteChar
        */
        public t_SpriteChar getSpriteChar(char c) {
            t_SpriteChar tChar = null;
            if (m_dicChar.TryGetValue(c, out tChar)) {
                return tChar;
            }
            return null;
        }
        //==========================================================================
        /*!文字列スプライトの長さを計算する.
            改行は無視.
            @brief　getLength
        */
        public float getLength(string text, float space = 1f) {
            float ln = 0f;
            for (int i = 0; i < text.Length; i++) {
                char ch = text[i];
                if (ch == '\n' || ch == '\r') {
                    return ln;
                }
                t_SpriteChar sc = getSpriteChar(ch);
                if (sc == null) {
                    Debug.LogError("this character code is illegal:" + ch + "(" + (int)ch + ")");
                    continue;
                }
                ln += sc.xAdvance + space;
            }
            return ln;
        }
        //==========================================================================
        /*!Property
            @brief
        */
        public uint id {
            get {
                return m_id;
            }
        }
        public string face {
            get {
                return m_face;
            }
        }
        public Material material {
            get {
                return m_material;
            }
        }
        public float pxSize {
            get {
                return m_pxSize;
            }
        }
        public float charSpacing {
            get {
                return m_charSpacing;
            }
        }
        public float lineHeight {
            get {
                return m_lineHeight;
            }
        }
        public float baseHeight {
            get {
                return m_baseHeight;
            }
        }
        public float texWidth {
            get {
                return m_texWidth;
            }
        }
        public float texHeight {
            get {
                return m_texHeight;
            }
        }
        public Texture texture {
            get {
                return m_material.mainTexture;
            }
        }
        public t_SpriteFont font {
            get {
                return m_tFont;
            }
        }
    }
}
