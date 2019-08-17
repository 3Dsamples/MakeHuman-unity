//==============================================================================================
/*!?ウィンドウメッシュ.
	@file  CWindowMesh

	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace KS {
    public class CWindowMesh : CSprite {
        protected RenderTexture m_renderTexture = null;
        CWindowBase m_cWindow = null;
        protected float m_zoffset = 0.0f;
        struct t_RegionInfo {
            public ulong m_id;
            public Parts m_parts;
            public Rect m_rcCamera;
            public Rect m_rcPixel;
        };
        Vector2 m_vRegionSize;
        t_RegionInfo[] m_aRegionInfo = null;

        //==========================================================================
        /*!ウィンドウレンダリングオブジェクトを生成する.
         * @brief	create
        */
        static public CWindowMesh create(uint id, CTextureResource cResource, Transform parent, float zoffset) {
            GameObject go = new GameObject("win:" + new MulId(id));
            if (parent != null) {
                go.transform.parent = parent;
            }
            // メッシュをアタッチ.
            CWindowMesh cWM = go.AddComponent<CWindowMesh>();
            cWM.m_id = id;
            cWM.m_cResource = cResource;
            cWM.m_zoffset = zoffset;
            return cWM;
        }
        //==========================================================================
        /*!レンダリングテクスチャオブジェクトを生成する.
         * @brief	create
        */
        static public CWindowMesh create(uint id, int size, RenderTextureFormat rtf, CWindowBase cWindow, float zoffset) {
            if (cWindow == null) {
                return null;
            }
            // ゲームオブジェクトの作成.
            GameObject go = new GameObject("render:" + new MulId(id));
            Transform parent = cWindow.transform;
            if (parent != null) {
                go.transform.parent = parent;
            }
            // メッシュをアタッチ.
            CWindowMesh cWM = go.AddComponent<CWindowMesh>();
            cWM.m_id = id;
            cWM.m_cWindow = cWindow;
            RenderTexture rt = new RenderTexture(size, size, 16, rtf, RenderTextureReadWrite.Default);
            rt.Create();
            Material material = new Material(Shader.Find("Custom UI/Alpha"));
            material.mainTexture = rt;
            cWM.initialize(material);
            cWM.m_isLoaded = true;
            cWM.m_renderTexture = rt;
            cWM.m_zoffset = zoffset;
            return cWM;
        }
        //==========================================================================
        /*!Awake
            @brief	Unity Callback
        */
        new public void Awake() {
            format = e_Format.UV | e_Format.Colors;
            e_LayerId eLayerId = e_LayerId.Window;
            Transform parent = transform.parent;
            if (parent != null) {
                eLayerId = (e_LayerId)parent.gameObject.layer;
            }
            addRenderer(eLayerId);
            m_invertY = true;
        }
        //==========================================================================
        /*!Start
         * @brief	Unity Callback
        */
        IEnumerator Start() {
            if (m_cResource == null) {
                yield break;
            }
            while (!m_cResource.isLoaded) {
                yield return 0;
            }
            // 一枚絵の場合はリソースを複製する.
            Material material = m_cResource.get();
            if (m_cResource.spriteData == null) {
                material = new Material(material);
                material.mainTexture = GameObject.Instantiate(material.mainTexture) as Texture2D;
            }
            initialize(material, m_cResource.spriteData);
            m_isLoaded = true;
        }
        //==========================================================================
        /*!LateUpdate
            @brief	Unity Callback
        */
        new protected void LateUpdate() {
            if (m_renderTexture != null && !m_renderTexture.IsCreated()) {
                if (m_renderTexture.Create()) {
                    if (m_cWindow != null) {
                        m_cWindow.onRecreatedRenderTexture(m_id);
                    }
                } else {
                    Debug.LogError("can't recreate render texture:" + new MulId(m_id));
                    return;
                }
            }
            base.LateUpdate();
        }
        //==========================================================================
        /*!getRegionIndex
         * @brief	領域のインデックスを取得する.
        */
        protected int getRegionIndex(ulong regionId) {
            if (m_aRegionInfo == null) {
                return -1;
            }
            if (regionId == 0) {
                return -1;
            }
            for (int i = 0; i < m_aRegionInfo.Length; ++i) {
                if (m_aRegionInfo[i].m_id == regionId) {
                    return i;
                }
            }
            return -1;
        }
#if !ONLY_APPLICATION
        public ulong createRegionId() {
            if (m_aRegionInfo == null) {
                return 0xffff0000;
            }
            for (int i = 0; i < m_aRegionInfo.Length; ++i) {
                if (m_aRegionInfo[i].m_id == 0) {
                    return (ulong)(0xffff0000 + i);
                }
            }
            return 0;
        }
#endif
        //==========================================================================
        /*!allocRegion
         * @brief	領域を一つ取得する.
        */
        public Parts allocRegion(Camera camera, ulong regionId, float width, float height) {
            if (m_renderTexture == null) {
                return null;
            }
            if (width == 0 || height == 0) {
                Debug.LogError("render texture region size is illegal!:" + width + "," + height);
                return null;
            }
            if (m_vRegionSize.x != width || m_vRegionSize.y != height) {
                if (m_vRegionSize.x == 0f && m_vRegionSize.y == 0f) {
                    m_vRegionSize.x = width;
                    m_vRegionSize.y = height;
                } else {
                    Debug.LogError("render texture region size can't resize!:" + width + "," + height);
                    return null;
                }
            }
            if (m_aRegionInfo == null) {
                // 領域の初期化.
                int w = m_renderTexture.width;
                int h = m_renderTexture.height;
                int ix = (int)((float)w / width);
                int iy = (int)((float)h / height);
                m_aRegionInfo = new t_RegionInfo[ix * iy];
                for (int y = 0; y < iy; ++y) {
                    for (int x = 0; x < ix; ++x) {
                        float x0 = (x * m_vRegionSize.x) / w;
                        float y0 = (y * m_vRegionSize.y) / h;
                        float x1 = (x * m_vRegionSize.x + m_vRegionSize.x) / w;
                        float y1 = (y * m_vRegionSize.y + m_vRegionSize.y) / h;
                        int idx = ix * y + x;
                        m_aRegionInfo[idx].m_id = 0;
                        m_aRegionInfo[idx].m_parts = new Parts(new Vector4(0f, 0f, m_vRegionSize.x, m_vRegionSize.y), new Vector4(x0, y0, x1, y1));
                        m_aRegionInfo[idx].m_rcCamera = new Rect(0f, 0f, m_vRegionSize.x / w, m_vRegionSize.y / h);
                        m_aRegionInfo[idx].m_rcPixel = new Rect(x * m_vRegionSize.x, y * m_vRegionSize.y, m_vRegionSize.x, m_vRegionSize.y);
                    }
                }
            }
            if (getRegionIndex(regionId) >= 0) {
                Debug.LogError("already alloc region!:" + regionId);
                return null;
            }
            for (int i = 0; i < m_aRegionInfo.Length; ++i) {
                if (m_aRegionInfo[i].m_id == 0) {
                    m_aRegionInfo[i].m_id = regionId;
                    if (camera != null) {
                        //					camera.rect = m_aRegionInfo[i].m_rcCamera;
                        camera.pixelRect = m_aRegionInfo[i].m_rcPixel;
                    }
                    return m_aRegionInfo[i].m_parts;
                }
            }
            return null;
        }
        //==========================================================================
        /*!releaseRegion
         * @brief	領域を一つ解放する.
        */
        public void releaseRegion(ulong regionId) {
            int idx = getRegionIndex(regionId);
            if (idx < 0) {
                Debug.LogError("can't releae render texture region:" + regionId);
                return;
            }
            m_aRegionInfo[idx].m_id = 0;
        }
        public float zoffset {
            get {
                return m_zoffset;
            }
        }
    };
}
