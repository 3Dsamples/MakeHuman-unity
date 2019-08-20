//==============================================================================================
/*!?レンダーテクスチャを表示する.
	@file  CWinCtrlRenderIcon

	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KS {
    public class CWinCtrlRenderIcon : CWinCtrlBase {
        Parts m_parts;
        Camera m_camera;
        ulong m_regionId = 0;
        bool m_NeedToRender = true;
        bool m_bHideRenderMesh = false;
        protected t_ProcHit m_tProcHit;
        protected CWindowMesh m_meshRender = null;
        protected int m_idxRenderTex = -1;
        protected WinColor m_clearcolor = new WinColor(0, 0, 0, 0);
        //==========================================================================
        /*!Constructor
            @brief	Constructor
        */
        public CWinCtrlRenderIcon(CWindowBase cWindowBase) : base(cWindowBase, e_WinCtrlKind.RENDERICON) {
            m_cTextOne = new CWinTextOne();
            m_tProcHit.clear();
            m_bNohit = false;
            SEId = CWindowMgr.Instance.clickSE;
        }
        //==========================================================================
        /*!Copy Constructor
            @brief	Copy Constructor
        */
        public CWinCtrlRenderIcon(CWinCtrlRenderIcon src, CWinContents parent) : base(src, parent) {
            initialize();
            if (src.m_parts == null) {
                m_parts = null;
            } else {
                m_parts = new Parts(src.m_parts);
            }
            m_NeedToRender = true;
            m_tProcHit.clear();
        }
        //==========================================================================
        /*!Rende
            @brief	initialize render textre
        */
        override protected void initialize() {
            base.initialize();
            float szRenderTexture = contentsSize.x;
            if (szRenderTexture == 0f) {
                Debug.LogError(this + "render size is illegal!!:" + szRenderTexture);
                return;
            }

            for (int i = 0; i < m_aTexture.Length; ++i) {
                if (m_aTexture[i].partId == 0) {
                    CWindowMesh cWM = window.getRenderMesh(getTextureId(i), (int)szRenderTexture, RenderTextureFormat.Default);
                    if (cWM == null) {
                        return;
                    }
                    m_meshRender = cWM;
                    m_idxRenderTex = i;
                    m_aTexture[i].cWindowMesh = cWM;
                    break;
                }
            }
            m_regionId = 0;
            if (m_camera == null) { // カメラオブジェクトの設定.
                GameObject go = new GameObject("camera:" + this);
                m_camera = go.AddComponent<Camera>();
            }
            if (m_meshRender != null) {
                m_camera.targetTexture = m_meshRender.material.mainTexture as RenderTexture;
            }
            m_camera.clearFlags = CameraClearFlags.Color;
            m_camera.cullingMask = (int)e_Layer.RenderIcon;
            m_camera.gameObject.SetActive(false);
            m_camera.backgroundColor = m_clearcolor;
            onResetParts(0);
        }
        //==========================================================================
        /*!Release
            @brief	release
        */
        public override void release() {
            if (m_camera != null) {
                GameObject.DestroyImmediate(m_camera.gameObject, true);
                m_camera = null;
            }
            if (m_regionId != 0) {
                if (m_meshRender != null) {
                    m_meshRender.releaseRegion(m_regionId);
                }
            }
            base.release();
        }
        //==========================================================================
        /*!Update
            @brief	update
        */
        override public void update(Vector3 pos, int priority) {
            base.update(pos, priority);
            if (m_camera == null) {
                return;
            }
        }
        //==========================================================================
        /*!Render
            @brief	render
        */
        override public void render(Vector3 pos, CWindowMesh cWinMesh, CWinTextMesh cTextMesh, WinColor rtcolor, ClipRect cr) {
            if (m_camera == null || m_camera.targetTexture == null || !m_camera.targetTexture.IsCreated()) {
                m_tProcHit.update();
                return;
            }
            if (m_meshRender == null) {
                m_tProcHit.update();
                return;
            }
            if (m_regionId == 0) {
#if !ONLY_APPLICATION
                regionId = m_meshRender.createRegionId();
                m_camera.Render();
#endif
                m_tProcHit.update();
                return;
            }
            if (m_NeedToRender) {
                if (!m_bHideRenderMesh) {
                    if (!render()) {
                        m_tProcHit.update();
                        return;
                    }
                }
            }
            if (m_parts == null) {
                m_tProcHit.update();
                return;
            }
            if (m_tProcHit.tap) {
                rtcolor = rtcolor * 0.5f;
            }
            WinColor col;
            for (int i = 0; i < m_aTexture.Length; i++) {
                CWindowMesh cMesh = getWindowMesh(i);
                if (cMesh == null) {
                    continue;
                }
                Parts parts = null;
                if (m_idxRenderTex == i) {
                    if (m_bHideRenderMesh) {
                        continue;
                    }
                    parts = m_parts;
                } else if (m_aTexture[i].partId != 0) {
                    parts = m_aTexture[i].parts;
                } else {
                    continue;
                }
                col = rtcolor * m_aTexture[i].color;
                Vector3 texpos = pos;
                Vector2 offset = getRatioValue(m_aTexture[i].offset);
                texpos.x += offset.x;
                texpos.y += offset.y;
                cMesh.request(parts, texpos, col, 0, cr);
            }
            col = rtcolor * captionColor;
            cTextMesh.request(m_cTextOne, pos, captionOffset, textAnchor, col, size, cr);

            m_tProcHit.update();
        }
        //==========================================================================
        /*!RenderTextureが再作成されたことが通知される.
            @brief	onRecreatedRenderTexture
        */
        override public void onRecreatedRenderTexture(uint mRenderTextureId) {
            if (m_meshRender == null) {
                return;
            }
            if (m_meshRender.id != mRenderTextureId) {
                return;
            }
            m_NeedToRender = true;
        }
        //==========================================================================
        /*!レンダリングする.
            @brief	render
        */
        bool render() {
            m_camera.gameObject.SetActive(true);
            if (window.onBeginRenderIcon(this)) {
                m_camera.Render();
                m_NeedToRender = window.onEndRenderIcon(this);
            }
            m_camera.gameObject.SetActive(false);
            return true;
        }
        //==========================================================================
        /*!ヒットを処理する.
            @brief	procHit
        */
        override public void procHit(CWindowMgr cWindowMgr, CInput.e_State eState) {
            procHit(ref m_tProcHit, cWindowMgr, eState);
        }
        //==========================================================================
        /*!パーツを再更新する必要があるとき呼ばれる.
            @brief	updateRegionIndex
        */
        protected void updateRegionIndex() {
            if (m_meshRender == null) {
                return;
            }
            if (m_regionId == 0) {
                return;
            }
            float w = Mathf.Round(getTextureWidth(m_idxRenderTex));
            float h = Mathf.Round(getTextureHeight(m_idxRenderTex));
            // サイズの補正.
            if (w == 0f && h == 0f) {
                w = contentsSize.x;
            }
            if (w == 0f) {
                w = h;
            } else if (h == 0f) {
                h = w;
            }
            m_parts = m_meshRender.allocRegion(m_camera, regionId, w, h);
            m_NeedToRender = true;
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
        public ulong regionId {
            set {
                if (m_regionId == value) {
                    return;
                }
                if (m_meshRender == null) {
                    return;
                }
                if (m_regionId != 0) {
                    m_meshRender.releaseRegion(m_regionId);
                }
                m_regionId = value;
                updateRegionIndex();
            }
            get {
                return m_regionId;
            }
        }
        public Camera camera {
            get {
                return m_camera;
            }
        }
        public WinColor clearcolor {
            get {
                return m_clearcolor;
            }
            set {
                m_clearcolor = value;
                m_NeedToRender = true;
                if (m_camera != null) {
                    m_camera.backgroundColor = m_clearcolor;
                }
            }
        }
        public bool needToRender {
            set {
                m_NeedToRender = value;
            }
        }
        public bool hideRenderMesh {
            get {
                return m_bHideRenderMesh;
            }
            set {
                m_bHideRenderMesh = value;
            }
        }
        public CInput.e_State state {
            get {
                return m_tProcHit.state;
            }
        }
    }
}
