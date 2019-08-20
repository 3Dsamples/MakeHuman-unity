//==============================================================================================
/*!?メータ(アニメーション付).
	@file  CWinCtrlMeter
	
	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KS {
    public class CWinCtrlMeter : CWinCtrlBase {
        protected struct t_MeterInfo {
            public float old;
            public float val;
            public float render;
            public int angAnim;
            public float speed;
        };
        protected t_MeterInfo[] m_aMeterInfo = new t_MeterInfo[nTexture];
        //==========================================================================
        /*!Constructor
            @brief	Constructor
        */
        public CWinCtrlMeter(CWindowBase cWindow) : base(cWindow, e_WinCtrlKind.METER) {
            m_aTexture[0].partId = FiveCC.Id("MTR");
            m_aTexture[1].partId = FiveCC.Id("MTRB");

            for (int i = 0; i < m_aMeterInfo.Length; ++i) {
                m_aMeterInfo[i].val = 1;
            }

            setMeter(0, 1f, 0f);

            m_bNohit = true;
        }
        //==========================================================================
        /*!Copy Constructor
            @brief	Copy Constructor
        */
        public CWinCtrlMeter(CWinCtrlMeter src, CWinContents parent) : base(src, parent) {
            setMeter(0, 1f, 0f);
        }
        //==========================================================================
        /*!Render
            @brief	render
        */
        override public void render(Vector3 pos, CWindowMesh cWinMesh, CWinTextMesh cTextMesh, WinColor rtcolor, ClipRect cr) {
            // バーの表示.
            WinColor col;
            // 下地.
            for (int i = 0; i < m_aMeterInfo.Length; ++i) {
                uint partId = m_aTexture[i].partId;
                if (partId == 0) {
                    continue;
                }
                CWindowMesh cMesh = getWindowMesh(i);
                if (cMesh == null) {
                    continue;
                }
                // メータの長さを計算.
                Vector3 posMeter = pos;
                Vector2 offset = getRatioValue(m_aTexture[i].offset);
                posMeter.x += offset.x;
                posMeter.y += offset.y;
                col = rtcolor * m_aTexture[i].color;
                if (m_aMeterInfo[i].angAnim != 0) {
                    float k = Angle.LerpFactor(ref m_aMeterInfo[i].angAnim, m_aMeterInfo[i].speed);
                    float fMeter = Mathf.Lerp(m_aMeterInfo[i].old, m_aMeterInfo[i].val, k);
                    if (refreshMeter(i, fMeter)) {
                        cMesh.request(m_aTexture[i].parts, posMeter, col, 0, cr);
                    }
                } else if (m_aMeterInfo[i].val > 0f) {
                    if (refreshMeter(i, m_aMeterInfo[i].val)) {
                        cMesh.request(m_aTexture[i].parts, posMeter, col, 0, cr);
                    }
                }
            }
        }
        //==========================================================================
        /*!パーツを再更新する必要があるとき呼ばれる.
            @brief	onResetParts
        */
        override public void onResetParts(int iTexIndex) {
            Vector2 size = getTextureSize(iTexIndex);
            CWindowMesh cWM = getWindowMesh(iTexIndex);
            if (cWM == null) {
                return;
            }
            FiveCC partsid = new FiveCC(m_aTexture[iTexIndex].partId);
            size.x = m_aMeterInfo[iTexIndex].render;
            m_aTexture[iTexIndex].parts = cWM.create(partsid, size);
            adjustSize(m_aTexture[iTexIndex].parts, iTexIndex);
        }
        //==========================================================================
        /*!メータの値に応じてメッシュを生成しなおす.
            @brief	reseteMeter
        */
        bool refreshMeter(int index, float fMeter) {
            FiveCC partId = new FiveCC(m_aTexture[index].partId);
            if (partId == 0) {
                return false;
            }
            Vector2 sz = getTextureSize(index);
            fMeter *= sz.x;
            if (fMeter == m_aMeterInfo[index].render) {
                if (fMeter == 0) {
                    return false;
                }
                return true;
            }
            // メータ本体.
            m_aMeterInfo[index].render = fMeter;
            CWindowMesh cMesh = getWindowMesh(index);
            if (cMesh == null) {
                return false;
            }
            sz.x = fMeter;
            m_aTexture[index].parts = cMesh.create(partId, sz);
            return true;
        }
        //==========================================================================
        /*!メータの値を設定.
            @brief	meter
        */
        public float meter(int index) {
            if (index >= m_aMeterInfo.Length) {
                return 0f;
            }
            return m_aMeterInfo[index].val;
        }
        public void setMeter(int index, float value, float speed = 0.2f) {
            if (index >= m_aMeterInfo.Length) {
                return;
            }
            float fMeter = Mathf.Clamp01(value);
            m_aMeterInfo[index].speed = speed;
            if (speed == 0f) {
                m_aMeterInfo[index].old = fMeter;
                m_aMeterInfo[index].angAnim = 0;
            } else if (m_aMeterInfo[index].angAnim == 0) {
                m_aMeterInfo[index].angAnim = Angle.InitLerpFactor();
                m_aMeterInfo[index].old = m_aMeterInfo[index].val;
            }
            m_aMeterInfo[index].val = fMeter;
        }
    }
}
