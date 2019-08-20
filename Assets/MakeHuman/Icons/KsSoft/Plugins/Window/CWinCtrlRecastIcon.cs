//==============================================================================================
/*!?リキャスト機能付きアイコン.
	@file  CWinCtrlRecastIcon
	
	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KS {
    public class CWinCtrlRecastIcon : CWinCtrlTexture {
        protected float[] m_aRecastTime = new float[nTexture];
        //==========================================================================
        /*!Constructor
            @brief	Constructor
        */
        public CWinCtrlRecastIcon(CWindowBase cWindowBase, e_WinCtrlKind eKind = e_WinCtrlKind.RECASTICON) : base(cWindowBase, eKind) {
            m_aTexture[0].partId = FiveCC.Id("ICON");
            m_cTextOne = new CWinTextOne();
            m_bNohit = false;
            initializeRecastTime();
        }
        //==========================================================================
        /*!Copy Constructor
            @brief	Copy Constructor
        */
        public CWinCtrlRecastIcon(CWinCtrlRecastIcon src, CWinContents parent) : base(src, parent) {
            initializeRecastTime();
        }
        void initializeRecastTime() {
            for (int i = 0; i < m_aRecastTime.Length; ++i) {
                m_aRecastTime[i] = 0f;
            }
        }
        //==========================================================================
        /*!Render
            @brief	render
        */
        override public void render(Vector3 pos, CWindowMesh cWinMesh, CWinTextMesh cTextMesh, WinColor rtcolor, ClipRect cr) {
            WinColor col = rtcolor;
            if (m_tProcHit.tap) {
                col = rtcolor * 0.5f;
            }
            for (int i = 0; i < m_aTexture.Length; i++) {
                if (m_aTexture[i].partId != 0) {
                    CWindowMesh cMesh = getWindowMesh(i);
                    if (cMesh == null) {
                        continue;
                    }
                    WinColor texcol = col * m_aTexture[i].color;
                    Vector3 texpos = pos;
                    Vector2 offset = getRatioValue(m_aTexture[i].offset);
                    texpos.x += offset.x;
                    texpos.y += offset.y;

                    float tmRC = m_aRecastTime[i];
                    Parts parts = m_aTexture[i].parts;
                    if (tmRC == 0f) {
                        cMesh.request(parts, texpos, texcol, 0, cr);
                    } else if (tmRC < 1f) {
                        Vector4 uv = parts.uv;
                        Vector4 vert = new Vector4(0f, -parts.height, parts.width, 0f);
                        vert.w = (vert.y - vert.w) * tmRC + vert.w;
                        uv.w = (uv.y - uv.w) * tmRC + uv.w;
                        vert.x += texpos.x;
                        vert.z += texpos.x;
                        vert.y += texpos.y;
                        vert.w += texpos.y;
                        cMesh.request(vert, pos.z, uv, texcol, 0, cr);
                    }
                }
            }

            col = rtcolor * captionColor;
            cTextMesh.request(m_cTextOne, pos, captionOffset, textAnchor, col, size, cr);
        }
        //==========================================================================
        /*!キャプションパーツを再更新する必要があるとき呼ばれる.
            @brief	onResetCaption
        */
        override public void onResetCaption() {
            m_cTextOne.style = textStyle;
            m_cTextOne.anchor = textAnchor;
            m_cTextOne.text = caption;
            m_eUpdateFlag &= ~e_UpdateFlag.CAPTION;
        }
        public CInput.e_State state {
            get {
                return m_tProcHit.state;
            }
        }
        public void setRecastTime(int index, float tmRecast) {
            if (index < 0 || index >= nTexture) {
                return;
            }
            m_aRecastTime[index] = tmRecast;
        }
        public float getRecastTime(int index) {
            if (index < 0 || index >= nTexture) {
                return 0f;
            }
            return m_aRecastTime[index];
        }
    }
}
