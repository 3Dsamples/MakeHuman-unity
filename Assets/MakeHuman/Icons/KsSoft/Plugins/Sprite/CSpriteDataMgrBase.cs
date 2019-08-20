//==============================================================================================
/*!CSpriteDataMgr系を管理するマネージャを作るための基底クラス.
	@file  Sprite Data Manager Base
	
	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace KS {
    public enum e_Anchor {
        Default,
        LeftTop,
        LeftCenter,
        LeftBottom,
        TopCenter,
        Center,
        BottomCenter,
        RightTop,
        RightCenter,
        RightBottom,
        Left = LeftCenter,
        Right = RightCenter,
        Top = TopCenter,
        Bottom = BottomCenter,
    };

    abstract public class CSpriteDataMgrBase : MonoBehaviour, ILoader {
        protected bool m_bLoaded = false;
        protected uint m_mResourceId = 0;
        private Dictionary<uint, CSpriteBase> m_dicSprites = new Dictionary<uint, CSpriteBase>();

        //==========================================================================
        /*!Start
            @brief	Unity Callback
        */
        public void Start() {
            m_bLoaded = false;
            if (m_mResourceId == 0) {
                Debug.LogError("m_mResourceId == null!!");
                return;
            }
            StartCoroutine(loading(m_mResourceId));
        }
        //==========================================================================
        /*!ビルボードデータを参照する.
            @brief	reference
        */
        public CSpriteBase reference(uint id) {
            CSpriteBase res;
            if (m_dicSprites.TryGetValue(id, out res)) {
                return res;
            }
            return null;
        }
        //==========================================================================
        /*!保持しているSpriteのIDを列挙する.
            @brief	enumrate
        */
        public uint[] enumrate() {
            List<uint> lstKeys = new List<uint>();
            foreach (uint id in m_dicSprites.Keys) {
                lstKeys.Add(id);
            }
            return lstKeys.ToArray();
        }
        //==========================================================================
        /*!メッシュオブジェクトを生成する.
            @brief	createMeshObject
        */
        protected GameObject createMeshObject(uint id) {
            if (id == 0) {
                id = m_mResourceId;
            }
            GameObject go = new GameObject("mesh:" + new MulId(id));
            go.AddComponent<MeshFilter>();
            MeshRenderer mr = go.AddComponent<MeshRenderer>();
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            mr.receiveShadows = false;
            return go;
        }
        //==========================================================================
        /*!読み込み中かチェック.
            @brief	isLoading
        */
        virtual public bool isLoading() {
            return !m_bLoaded;
        }
        //==========================================================================
        /*!スプライトデータを追加する.
            @brief	entry
        */
        protected bool entry(uint id, CSpriteBase o) {
            if (!m_dicSprites.ContainsKey(id)) {
                m_dicSprites[id] = o;
                return true;
            }
            return false;
        }
        //==========================================================================
        /*!スプライトデータ読み込み.
            @brief	loading
        */
        IEnumerator loading(uint mId) {
            CSerializableScript cSS = null;
            CAssetBundleMgr cAssetBundleMgr = CAssetBundleMgr.Instance;
            if (cAssetBundleMgr != null) {
                CAssetBundle cAB = null;
                int iResult = 1;
                while (iResult > 0) {
                    iResult = cAssetBundleMgr.checkToLoadAssetbundle(ref cAB, mId);
                    if (iResult < 0) {
                        yield break;
                    } else if (iResult > 0) {
                        yield return 0;
                    }
                }
                AssetBundle ab = cAB;

                UnityEngine.Object[] aObject = ab.LoadAllAssets();

                foreach (UnityEngine.Object o in aObject) {
                    if (o is CSerializableScript) {
                        cSS = o as CSerializableScript;
                        break;
                    }
                }
            } else {
                string path = "bin/" + MulId.ToString(mId);
                cSS = Resources.Load<CSerializableScript>(path);
            }
            if (cSS == null) {
                Debug.LogError("can't load CSerializableScript:" + MulId.ToString(mId));
                yield break;
            }
            yield return StartCoroutine(loading(cSS));
        }
        virtual protected IEnumerator loading(CSerializableScript cSS) {
            m_bLoaded = true;
            yield break;
        }
    };
}
