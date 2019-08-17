//==============================================================================================
/*!SEリソースマネージャ.
	@file  CSeResourceMgr
*/
//==============================================================================================
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
namespace KS {
    //==========================================================================
    /*!
        @brief	class CSeResourceMgr
        @note	this class is sigleton
    */
    public class CSeResourceMgr : MonoBehaviour, ILoader {
        protected Dictionary<uint, CSeResource> m_dicSeResource = new Dictionary<uint, CSeResource>();
        protected int m_iNumLoading = 0;
        //==========================================================================
        /*!Awake
            @brief	Unity callback
        */
        void Awake() {
            SceneManager.sceneLoaded += onLevelWasLoaded;
            if (m_instance != null) {
                Debug.LogError("already instance exist");
            }
            m_instance = this;
        }
        //==========================================================================
        /*!OnDestroy
         * @brief	Unity Callback
        */
        void OnDestroy() {
            foreach (CSeResource cSR in m_dicSeResource.Values) {
                cSR.release();
            }
            m_dicSeResource.Clear();
            m_instance = null;
        }
        //==========================================================================
        /*!isLoading
            @brief	読み込んでいるかどうか?.
        */
        public bool isLoading() {
            return (m_iNumLoading != 0);
        }
        public int loadNum {
            get {
                return m_iNumLoading;
            }
        }
        //==========================================================================
        /*!reference
            @brief	リソースを参照する.
        */
        public CSeResource reference(uint id, bool bRemain = false) {
            if (CMainSystemBase.Instance.isChangingScene) {
#if UNITY_EDITOR
                Debug.LogWarning("chaging scene now....");
#endif
                return null;
            }
            CSeResource cSR;
            if (!m_dicSeResource.TryGetValue(id, out cSR)) {
                // if unregist,create
                cSR = new CSeResource(id);
                m_dicSeResource[id] = cSR;
                m_iNumLoading++;
                StartCoroutine(loadingCR(cSR, bRemain));
            } else {
                if (cSR.state == CSeResource.e_State.NONE) {
                    m_iNumLoading++;
                    StartCoroutine(loadingCR(cSR, bRemain));
                }
            }
            return cSR;
        }
        //==========================================================================
        /*!テクスチャをResourcesからロードする.
            @brief	loadFromResources.
        */
        public CSeResource loadFromResources(uint id, bool bRemain) {
            CSeResource cSR;
            // 既に読み込まれているなら読み込まない.
            if (m_dicSeResource.TryGetValue(id, out cSR)) {
                return cSR;
            }
            cSR = new CSeResource(id);
            m_dicSeResource[id] = cSR;
            cSR.state = CSeResource.e_State.NOEXIST;
            cSR.flag = (bRemain) ? e_ResourceFlag.REMAIN : e_ResourceFlag.NONE;

            string path = new MulId(id).ToString();
            CSeSerializableScript cSS = Resources.Load(path + ".se") as CSeSerializableScript;
            //-----------------------------
            // スプライトデータ登録.
            if (cSS == null) {
                return null;
            }
            t_SoundParamHeader tSPH = new t_SoundParamHeader();
            CReadVariable cVariable = new CReadVariable(cSS.m_buffer);
            tSPH.read(cVariable);

            cSR.set(tSPH.m_aSoundParam, cSS.m_aAC);
            cSR.flag |= e_ResourceFlag.RESOURCE;
            // リソース登録.
            m_dicSeResource[id] = cSR;
            return cSR;
        }
        //==========================================================================
        /*!find
            @brief	CSoundEffectを取得する.
        */
        public CSoundEffect find(uint mAssetBundle, uint mId) {
            CSeResource cSR = reference(mAssetBundle);
            if (cSR == null) {
                if (CMainSystemBase.Instance.isChangingScene) {
                    return null;
                }
                Debug.LogWarning("can't find se assetbundle:" + new MulId(mAssetBundle) + ",se:" + new MulId(mId));
                return null;
            }
            if (!cSR.isLoaded) {
                return null;
            }
            return cSR.find(mId);
        }
        //==========================================================================
        /*!release
         * @brief	非常駐型リソースを解放する.
         * @note	オブジェクトの解放はシーン遷移に任せる.
        */
        void release() {
            List<uint> lstRemove = new List<uint>();
            foreach (CSeResource cSR in m_dicSeResource.Values) {
                if (!cSR.isRemain) {
                    lstRemove.Add(cSR.id);
                }
            }
            foreach (uint id in lstRemove) {
                m_dicSeResource[id].release();
                m_dicSeResource.Remove(id);
            }
        }
        //==========================================================================
        /*!onLevelWasLoaded
            @brief
        */
        void onLevelWasLoaded(Scene scenename, LoadSceneMode mode) {
            release();
        }
        //==========================================================================
        /*!OnApplicationQuit
            @brief	Unity callback
        */
        void OnApplicationQuit() {
            foreach (CSeResource cSR in m_dicSeResource.Values) {
                if (cSR.isRemain) {
                    cSR.release();
                }
            }
        }
        //==========================================================================
        /*!loading
            @brief	SEを非同期でAssetbundleとして読み込むためのコルーチン.
        */
        IEnumerator loadingCR(CSeResource cSR, bool bRemain) {
            cSR.state = CSeResource.e_State.LOADING;

            CAssetBundleMgr cAssetBundleMgr = CAssetBundleMgr.Instance;
            CAssetBundle cAB = null;
            int iResult = 1;
            while (iResult > 0) {
                iResult = cAssetBundleMgr.checkToLoadAssetbundle(ref cAB, cSR);
                if (iResult < 0) {
                    m_iNumLoading--;
                    yield break;
                } else if (iResult > 0) {
                    yield return 0;
                }
            }
            AssetBundle ab = cAB;
            UnityEngine.Object[] aObj = ab.LoadAllAssets();
            List<AudioClip> lstAC = new List<AudioClip>();
            t_SoundParamHeader tSPH = null;
            foreach (UnityEngine.Object obj in aObj) {
                if (obj is CSerializableScript) {
                    CSerializableScript cSS = obj as CSerializableScript;
                    CReadVariable cVariable = new CReadVariable(cSS.m_buffer);
                    tSPH = new t_SoundParamHeader();
                    tSPH.read(cVariable);
                } else if (obj is AudioClip) {
                    lstAC.Add(obj as AudioClip);
                }
            }

            cSR.flag = cAB.flag;
            if (bRemain) {
                cSR.flag |= e_ResourceFlag.REMAIN;
                cAB.flag |= e_ResourceFlag.REMAIN;
            }
            cSR.set(tSPH.m_aSoundParam, lstAC.ToArray());
            cSR.state = CSeResource.e_State.LOADED;
            m_iNumLoading--;
        }
        static protected CSeResourceMgr m_instance;
        public static CSeResourceMgr Instance {
            get {
                return m_instance;
            }
        }
    };
}
