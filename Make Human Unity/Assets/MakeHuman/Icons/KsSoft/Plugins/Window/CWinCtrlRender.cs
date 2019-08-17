//==============================================================================================
/*!?レンダーテクスチャを表示する.
	@file  CWinCtrlRender
	
	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace KS {
    public class CWinCtrlRender : CWinCtrlBase {
        Parts m_parts;
        Camera m_camera;
        protected t_ProcHit m_tProcHit;
        //==========================================================================
        /*!Constructor
            @brief	Constructor
        */
        public CWinCtrlRender(CWindowBase cWindowBase) : base(cWindowBase, e_WinCtrlKind.RENDER) {
            m_tProcHit.clear();
            color1 = new WinColor(0, 0, 0, 0);
        }
        //==========================================================================
        /*!Copy Constructor
            @brief	Copy Constructor
        */
        public CWinCtrlRender(CWinCtrlRender src, CWinContents parent) : base(src, parent) {
            initialize();
            if (src.m_parts == null) {
                m_parts = null;
            } else {
                m_parts = new Parts(src.m_parts);
            }
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
            CWindowMesh cWM = window.getRenderMesh(texId, (int)szRenderTexture, RenderTextureFormat.Default);
            if (cWM == null) {
                return;
            }
            m_aTexture[0].cWindowMesh = cWM;
            if (m_camera == null) {
                GameObject go = new GameObject("camera:" + this);
                m_camera = go.AddComponent<Camera>();
            }
            m_camera.targetTexture = cWM.material.mainTexture as RenderTexture;
            camera.clearFlags = CameraClearFlags.Color;
            camera.backgroundColor = color1;
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
            base.release();
        }
        //==========================================================================
        /*!Update
            @brief	update
        */
        override public void update(Vector3 pos, int priority) {
            base.update(pos, priority);

            m_camera.gameObject.SetActive(!window.disable);
        }
        //==========================================================================
        /*!Render
            @brief	render
        */
        override public void render(Vector3 pos, CWindowMesh cWinMesh, CWinTextMesh cTextMesh, WinColor rtcolor, ClipRect cr) {
            if (m_camera == null || m_camera.targetTexture == null || !m_camera.targetTexture.IsCreated()) {
                return;
            }
            WinColor col;
            //		if (m_aTexture[0].partId == 0) {
            //			return;
            //		}
            CWindowMesh cMesh = getWindowMesh(0);
            if (cMesh == null) {
                return;
            }
            col = rtcolor * m_aTexture[0].color;
            Vector3 texpos = pos;
            Vector2 offset = getRatioValue(m_aTexture[0].offset);
            texpos.x += offset.x;
            texpos.y += offset.y;
            cMesh.request(m_parts, texpos, col, 0, cr);
            m_tProcHit.update();
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
            @brief	onResetParts
        */
        override public void onResetParts(int iTexIndex) {
            float szRenderTexture = contentsSize.x;
            if (szRenderTexture == 0f) {
                Debug.LogError(this + "render size is illegal!!:" + szRenderTexture);
                return;
            }
            float w = width;
            float h = height;
            Rect tRect;
            Vector4 uv;
            // サイズの補正.
            if (w == 0f && h == 0f) {
                w = szRenderTexture;
            }
            if (w == 0f) {
                w = h;
            } else if (h == 0f) {
                h = w;
            }
            // UVの生成.
            if (w > h) {
                if (szRenderTexture < w) {
                    tRect = new Rect(0f, 0f, 1f, h / w);
                    uv = new Vector4(0f, 0f, 1f, h / w);
                } else {
                    tRect = new Rect(0f, 0f, w / szRenderTexture, h / szRenderTexture);
                    uv = new Vector4(0f, 0f, w / szRenderTexture, h / szRenderTexture);
                }
            } else {
                if (szRenderTexture < h) {
                    tRect = new Rect(0f, 0f, w / h, 1f);
                    uv = new Vector4(0f, 0f, w / h, 1f);
                } else {
                    tRect = new Rect(0f, 0f, w / szRenderTexture, h / szRenderTexture);
                    uv = new Vector4(0f, 0f, w / szRenderTexture, h / szRenderTexture);
                }
            }
            //		tRect.x = (1f - tRect.width) * 0.5f;
            //		tRect.y = (1f - tRect.height) * 0.5f;

            // 頂点の生成.
            m_parts = new Parts(new Vector4(0f, 0f, w, h), uv);
            // ビューポートの生成.
            m_camera.rect = tRect;
            m_camera.aspect = w / h;
        }
        public Camera camera {
            get {
                return m_camera;
            }
        }
        override public WinColor color1 {
            get {
                return base.color1;
            }
            set {
                base.color1 = value;
                if (m_camera != null) {
                    m_camera.backgroundColor = color1;
                }
            }
        }
    }
}
