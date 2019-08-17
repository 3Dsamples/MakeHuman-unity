//==============================================================================================
/*!?ウィンドウ用テキストを一括でレンダリングする.
	@file  CWinTextMesh
	(京 counter SJIS string)
*/
//==============================================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KS {
    public class CWinTextMesh : CWindowMesh {
        public int m_fontSize = 32;
        protected Camera m_camera = null;

        CSpriteFont m_spriteFont;
        Texture m_texture;

        Vector2 m_pixelsPerUV;
        Vector2 m_InvPixelsPerUV;
        Vector2 m_screenSize;
        const float m_slideSize = 1f;

        //==========================================================================
        /*!create
         * @brief	テキストレンダリングオブジェクトを生成する.
        */
        static public CWinTextMesh create(uint id, Transform parent, float zoffset = -126f / 65536f) {
            // フォント取得.
            CSpriteFontMgr cMgr = CSpriteFontMgr.Instance;
            if (cMgr == null) {
                return null;
            }
            CSpriteFont font = cMgr.reference(id);
            if (font == null) {
                return null;
            }
            // ゲームオブジェクト作成.
            GameObject go = new GameObject("text:" + new FiveCC(id));
            go.transform.localPosition = new Vector3(0f, 0f, zoffset);
            if (parent != null) {
                go.transform.parent = parent;
                go.transform.localPosition = new Vector3(0f, 0f, zoffset);
            }
            // メッシュをアタッチ.
            CWinTextMesh cTRW = go.AddComponent<CWinTextMesh>();
            cTRW.m_id = id;

            // 初期化.
            cTRW.setFont(font);
            cTRW.initialize(font.material);
            return cTRW;
        }
        //==========================================================================
        /*!create
         * @brief	テキストレンダリングオブジェクトを生成する.
        */
        static public CWinTextMesh create(Transform parent, float zoffset = -126f / 65536f) {
            // ゲームオブジェクト作成.
            GameObject go = new GameObject("text:drag object");
            go.transform.localPosition = new Vector3(0f, 0f, zoffset);
            if (parent != null) {
                go.transform.parent = parent;
                go.transform.localPosition = new Vector3(0f, 0f, zoffset);
            }
            // メッシュをアタッチ.
            CWinTextMesh cTRW = go.AddComponent<CWinTextMesh>();
            cTRW.m_id = MulId.Id(255, 255, 65535);
            return cTRW;
        }

        //==========================================================================
        /*!Awake
            @brief	Unity Callback
        */
        new void Awake() {
            format = e_Format.UV | e_Format.Tangents | e_Format.Colors;

            e_LayerId eLayerId = e_LayerId.Window;
            Transform parent = transform.parent;
            if (parent != null) {
                eLayerId = (e_LayerId)parent.gameObject.layer;
            }
            addRenderer(eLayerId);

            if (m_camera == null) {
                setCamera(CWindowMgr.Instance.Camera);
            }
            m_invertY = true;
        }
        //==========================================================================
        /*!Start
            @brief	Unity Callback
        */
        void Start() {
        }
        //==========================================================================
        /*!テキストのレンダリングを要求する.
            @brief	request
        */
        public void request(CWinTextOne cText, Vector3 position, Vector3 offset, e_Anchor eAnchor, WinColor color, Vector2 parentSize, ClipRect clip = null) {
            if (m_mesh == null) {
                return;
            }
            if (eAnchor == e_Anchor.Center || eAnchor == e_Anchor.LeftCenter || eAnchor == e_Anchor.RightCenter) {
                position.y += -cText.height * 0.5f + parentSize.y * 0.5f;
            } else if (eAnchor == e_Anchor.Bottom || eAnchor == e_Anchor.LeftBottom || eAnchor == e_Anchor.RightBottom) {
                position.y += -cText.height + parentSize.y;
            }
            if (eAnchor == e_Anchor.Center || eAnchor == e_Anchor.Top || eAnchor == e_Anchor.Bottom) {
                position.x += -cText.width * 0.5f + parentSize.x * 0.5f;
            } else if (eAnchor == e_Anchor.RightCenter || eAnchor == e_Anchor.RightTop || eAnchor == e_Anchor.RightBottom) {
                position.x += -cText.width + parentSize.x;
            }
            position.x += offset.x;
            position.y += offset.y;
            // 座標を補正する.
            position = KsSoftUtility.pixelPerfect(position);

            request(cText, position, color, clip);
        }
        //==========================================================================
        /*!テキストのレンダリングを要求する.
            @brief	request
        */
        public void request(CWinTextOne cText, Vector3 position, WinColor color, ClipRect clip = null) {
            Vector4 UVs;
            Vector2 chSize;
            WinColor shadow = color;
            shadow.r = 0;
            shadow.g = 0;
            shadow.b = 0;
            for (int i = 0; i < cText.Length; i++) {
                Vector3 offset;
                t_SpriteChar tSC = cText.getSpriteChar(out offset, i, ref color);
                if (tSC == null) {
                    continue;
                }
                UVs = tSC.uv;
                chSize.x = m_pixelsPerUV.x * (UVs.z - UVs.x);
                chSize.y = m_pixelsPerUV.y * (UVs.w - UVs.y);
                Vector3 pos = position;
                pos.x += offset.x;
                pos.y += offset.y;
                switch (cText.style) {
                case CWinTextOne.e_Style.Normal:
                    request(pos, color, UVs, chSize, tSC.page, clip);
                    break;
                case CWinTextOne.e_Style.Bold:
                    request(pos, color, UVs, chSize, tSC.page, clip);
                    pos.x += m_slideSize;
                    request(pos, color, UVs, chSize, tSC.page, clip);
                    break;
                case CWinTextOne.e_Style.Dent:
                    pos.y -= m_slideSize;
                    request(pos, shadow, UVs, chSize, tSC.page, clip);
                    pos.y += m_slideSize;
                    request(pos, color, UVs, chSize, tSC.page, clip);
                    break;
                case CWinTextOne.e_Style.Shadow:
                    pos.x += m_slideSize;
                    pos.y += m_slideSize;
                    request(pos, shadow, UVs, chSize, tSC.page, clip);
                    pos.x += m_slideSize;
                    pos.y += m_slideSize;
                    request(pos, color, UVs, chSize, tSC.page, clip);
                    break;
                }
            }
        }
        //==========================================================================
        /*!テキストのレンダリングを要求するスケール可能).
            @brief	request
        */
        public void request(CWinTextOne cText, Vector3 position, Vector3 offset, Vector2 scale, e_Anchor eAnchor, WinColor color, Vector2 parentSize, ClipRect clip = null) {
            if (m_mesh == null) {
                return;
            }
            if (eAnchor == e_Anchor.Center || eAnchor == e_Anchor.LeftCenter || eAnchor == e_Anchor.RightCenter) {
                position.y += -cText.height * 0.5f * scale.y + parentSize.y * 0.5f;
            } else if (eAnchor == e_Anchor.Bottom || eAnchor == e_Anchor.LeftBottom || eAnchor == e_Anchor.RightBottom) {
                position.y += -cText.height * scale.y + parentSize.y;
            }
            if (eAnchor == e_Anchor.Center || eAnchor == e_Anchor.Top || eAnchor == e_Anchor.Bottom) {
                position.x += -cText.width * 0.5f * scale.x + parentSize.x * 0.5f;
            } else if (eAnchor == e_Anchor.RightCenter || eAnchor == e_Anchor.RightTop || eAnchor == e_Anchor.RightBottom) {
                position.x += -cText.width * scale.x + parentSize.x;
            }
            position.x += offset.x;
            position.y += offset.y;
            // 座標を補正する.
            position = KsSoftUtility.pixelPerfect(position);

            request(cText, position, scale, color, clip);
        }
        //==========================================================================
        /*!テキストのレンダリングを要求する(スケール可能).
            @brief	request
        */
        public void request(CWinTextOne cText, Vector3 position, Vector2 scale, WinColor color, ClipRect clip = null) {
            Vector4 UVs;
            Vector2 chSize;
            WinColor shadow = color;
            shadow.r = 0;
            shadow.g = 0;
            shadow.b = 0;
            for (int i = 0; i < cText.Length; i++) {
                Vector3 offset;
                t_SpriteChar tSC = cText.getSpriteChar(out offset, i, ref color);
                if (tSC == null) {
                    continue;
                }
                UVs = tSC.uv;
                chSize.x = m_pixelsPerUV.x * (UVs.z - UVs.x) * scale.x;
                chSize.y = m_pixelsPerUV.y * (UVs.w - UVs.y) * scale.y;
                Vector3 pos = position;
                pos.x += offset.x * scale.x;
                pos.y += offset.y * scale.y;
                switch (cText.style) {
                case CWinTextOne.e_Style.Normal:
                    request(pos, color, UVs, chSize, tSC.page, clip);
                    break;
                case CWinTextOne.e_Style.Bold:
                    request(pos, color, UVs, chSize, tSC.page, clip);
                    pos.x += m_slideSize;
                    request(pos, color, UVs, chSize, tSC.page, clip);
                    break;
                case CWinTextOne.e_Style.Dent:
                    pos.y -= m_slideSize;
                    request(pos, shadow, UVs, chSize, tSC.page, clip);
                    pos.y += m_slideSize;
                    request(pos, color, UVs, chSize, tSC.page, clip);
                    break;
                case CWinTextOne.e_Style.Shadow:
                    pos.x += m_slideSize;
                    pos.y += m_slideSize;
                    request(pos, shadow, UVs, chSize, tSC.page, clip);
                    pos.x += m_slideSize;
                    pos.y += m_slideSize;
                    request(pos, color, UVs, chSize, tSC.page, clip);
                    break;
                }
            }
        }
        protected void request(Vector3 pos, WinColor color, Vector4 uv, Vector2 size, sbyte page, ClipRect clip = null) {
            int index = base.request(pos, color, uv, size, 0, clip);
            if (index < 0) {
                return;
            }
            Vector4 vPage = Vector4.zero;
            vPage[page] = 1f;
            m_tangents[index + 0] = vPage;
            m_tangents[index + 1] = vPage;
            m_tangents[index + 2] = vPage;
            m_tangents[index + 3] = vPage;
        }
        //==========================================================================
        /*!フォントを設定する.
            @brief	setFont
        */
        public void setFont(CSpriteFont newFont) {
            m_spriteFont = newFont;
            m_texture = newFont.material.mainTexture;
            setPixelToUV(m_texture);
        }
        //==========================================================================
        /*!キャラクターコードが有効かどうか判定する.
            @brief	validCharCode
        */
        public bool validCharCode(char c) {
            if (m_spriteFont.getSpriteChar(c) == null) {
                return false;
            }
            return true;
        }
        //==========================================================================
        /*!カメラを設定する.
            @brief	setCamera
        */
        public void setCamera(Camera c) {
            m_camera = c;
            m_screenSize.x = c.pixelWidth;
            m_screenSize.y = c.pixelHeight;
        }
        public void setPixelToUV(int texWidth, int texHeight) {
            if (spriteFont == null)
                return;
            m_pixelsPerUV.x = texWidth;
            m_pixelsPerUV.y = texHeight;
            m_InvPixelsPerUV.x = 1f / m_pixelsPerUV.x;
            m_InvPixelsPerUV.y = 1f / m_pixelsPerUV.y;
        }
        public void setPixelToUV(Texture tex) {
            if (tex == null)
                return;
            setPixelToUV(tex.width, tex.height);
        }
        public CSpriteFont spriteFont {
            get {
                return m_spriteFont;
            }
        }
    }
}
