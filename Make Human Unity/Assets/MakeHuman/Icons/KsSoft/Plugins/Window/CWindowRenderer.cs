//==============================================================================================
/*!?ウィンドウレンダラー.
	@file  CWindowRenderer

	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


namespace KS {
    public class CWindowRenderer {
        protected Dictionary<uint, CWindowMesh> m_dicWindowMesh = new Dictionary<uint, CWindowMesh>();
        protected Dictionary<uint, CWindowMesh> m_dicRenderMesh = null;
        protected Dictionary<uint, CWinTextMesh> m_dicTextMesh = new Dictionary<uint, CWinTextMesh>();
        protected Transform m_transform;
        protected CWindowBase m_cWindow;
        protected CTextureResourceMgr m_cTextureResourceMgr;
        protected float m_zoffset = 0f;
        protected bool m_needToSort = false;

        protected const float zbase = -1f / 2048f;
        protected const float zspace = (1f / 2048f) / 8f;
        protected const float textzbase = zbase * 3f;
        //==========================================================================
        /*!コンストラクタ.
            @brief	Constructor
        */
        public CWindowRenderer(Transform tf) {
            m_cWindow = null;
            m_transform = tf;
            m_cTextureResourceMgr = CTextureResourceMgr.Instance;
        }
        //==========================================================================
        /*!コンストラクタ.
            @brief	Constructor
        */
        public CWindowRenderer(CWindowBase cWindow) {
            m_cWindow = cWindow;
            m_transform = cWindow.transform;
            m_cTextureResourceMgr = CTextureResourceMgr.Instance;
        }
        //==========================================================================
        /*!ウィンドウメッシュ取得.
            @brief	getWindowMesh
        */
        public void clear() {
            if (m_dicWindowMesh.Count != 0) {
                foreach (CWindowMesh cMesh in m_dicWindowMesh.Values) {
                    UnityEngine.Object.DestroyImmediate(cMesh.gameObject, true);
                }
                m_dicWindowMesh.Clear();
            }
            if (m_dicRenderMesh != null) {
                foreach (CWindowMesh cMesh in m_dicRenderMesh.Values) {
                    UnityEngine.Object.DestroyImmediate(cMesh.gameObject, true);
                }
                m_dicRenderMesh.Clear();
                m_dicRenderMesh = null;
            }
            if (m_dicTextMesh.Count != 0) {
                foreach (CWinTextMesh cMesh in m_dicTextMesh.Values) {
                    UnityEngine.Object.DestroyImmediate(cMesh.gameObject, true);
                }
                m_dicTextMesh.Clear();
            }
        }

        //==========================================================================
        /*!ウィンドウメッシュ取得.
            @brief	getWindowMesh
        */
        public CWindowMesh getWindowMesh(uint id, float zoffset) {
            CWindowMesh cWM;
            if (!m_dicWindowMesh.TryGetValue(id, out cWM)) {
                CTextureResource cTR = m_cTextureResourceMgr.reference(id);
                cWM = CWindowMesh.create(id, cTR, m_transform, zoffset);
                m_dicWindowMesh[id] = cWM;
                m_needToSort = true;
            }
            return cWM;
        }
        //==========================================================================
        /*!レンダーメッシュ取得.
            @brief	getRenderMesh
        */
        public CWindowMesh getRenderMesh(uint id, int size, RenderTextureFormat rtf, float zoffset) {
            CWindowMesh cWM;
            if (m_cWindow == null) {
                return null;
            }
            if (m_dicRenderMesh == null) {
                m_dicRenderMesh = new Dictionary<uint, CWindowMesh>();
            }
            if (!m_dicRenderMesh.TryGetValue(id, out cWM)) {
                cWM = CWindowMesh.create(id, size, rtf, m_cWindow, zoffset);
                m_dicRenderMesh[id] = cWM;
                m_needToSort = true;
            }
            return cWM;
        }
        //==========================================================================
        /*!テキストメッシュ取得.
            @brief	getTextMesh
        */
        public CWinTextMesh getTextMesh(uint id) {
            CWinTextMesh cTWM;
            if (id == 0) {
                if (m_cWindow != null) {
                    id = m_cWindow.fontKind;
                } else {
                    id = new FiveCC("fn40");
                }
            }
            if (!m_dicTextMesh.TryGetValue(id, out cTWM)) {
                cTWM = CWinTextMesh.create(id, m_transform, textzbase);
                m_dicTextMesh[id] = cTWM;
            }
            return cTWM;
        }
        public void clearTransform() {
            foreach (CWinTextMesh cWTM in m_dicTextMesh.Values) {
                if (cWTM == null) {
                    continue;
                }
                cWTM.transform.localScale = new Vector3(1f, 1f, 1f);
                cWTM.transform.localRotation = Quaternion.identity;
            }
            foreach (CWindowMesh cWM in m_dicWindowMesh.Values) {
                if (cWM == null) {
                    continue;
                }
                cWM.transform.localScale = new Vector3(1f, 1f, 1f);
                cWM.transform.localRotation = Quaternion.identity;
            }
        }
        //==========================================================================
        /*!ウィンドウメッシュ取得.
            @brief	isLoaded
        */
        public void sort() {
            if (!m_needToSort) {
                return;
            }
            int n = m_dicWindowMesh.Count;
            if (m_dicRenderMesh != null) {
                n += m_dicRenderMesh.Count;
            }
            CWindowMesh[] aSort = new CWindowMesh[n];
            int idx = 0;
            foreach (CWindowMesh wm in m_dicWindowMesh.Values) {
                aSort[idx++] = wm;
            }
            if (m_dicRenderMesh != null) {
                foreach (CWindowMesh wm in m_dicRenderMesh.Values) {
                    aSort[idx++] = wm;
                }
            }
            Array.Sort(aSort, compare);
            Vector3 pos = new Vector3(0f, 0f, zbase);
            for (int i = 0; i < aSort.Length; ++i) {
                aSort[i].transform.localPosition = pos;
                pos.z += zspace;
            }
            m_needToSort = false;
        }
        static int compare(CWindowMesh a, CWindowMesh b) {
            float d = a.zoffset - b.zoffset;
            if (d == 0.0f) {
                if (a.id < b.id) {
                    return 1;
                } else if (a.id > b.id) {
                    return -1;
                }
                return 0;
            }
            if (d < 0) {
                return 1;
            }
            return -1;
        }
        //==========================================================================
        /*!ウィンドウメッシュ取得.
            @brief	isLoaded
        */
        public bool isLoaded {
            get {
                foreach (CWindowMesh cMesh in m_dicWindowMesh.Values) {
                    if (!cMesh.isLoaded) {
                        return false;
                    }
                }
                return true;
            }
        }
        public Transform transform {
            get {
                return m_transform;
            }
        }
    }

}
