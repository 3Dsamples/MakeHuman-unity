//==============================================================================================
/*!BGMリソースマネージャ.
	@file  CBgmResourceMgr	
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
        @brief	class CBgmResourceMgr
        @note	this class is sigleton
    */
    public class CBgmResourceMgr : MonoBehaviour, ILoader {
        protected Dictionary<uint, CBgmResource> m_dicBgmResource = new Dictionary<uint, CBgmResource>();
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
        /*!loadFromResource
            @brief	リソースからBGMを読み込む.
        */
        public CBgmResource loadFromResources(uint id) {
            CBgmResource cBR;
            // 既に読み込まれているなら読み込まない.
            if (m_dicBgmResource.TryGetValue(id, out cBR)) {
                return cBR;
            }
            // if unregist,create
            cBR = new CBgmResource(id);
            cBR.state = CBgmResource.e_State.NOEXIST;

            string path = new MulId(id).ToString() + ".bgm";
            CBgmSerializableScript cSS = Resources.Load<CBgmSerializableScript>(path);
            if (cSS == null) {
                return null;
            }
            cBR.set(cSS.m_loop, cSS.m_intro);
            cBR.state = CBgmResource.e_State.LOADED;
            return cBR;
        }
        //==========================================================================
        /*!reference
            @brief	リソースを参照する.		
        */
        public CBgmResource reference(uint id) {
            if (CMainSystemBase.Instance.isChangingScene) {
#if UNITY_EDITOR
                Debug.Log("chaging scene now....");
#endif
                return null;
            }
            CBgmResource cBR;
            if (!m_dicBgmResource.TryGetValue(id, out cBR)) {
                // if unregist,create
                cBR = new CBgmResource(id);
                m_dicBgmResource[id] = cBR;
                m_iNumLoading++;
                StartCoroutine(loadingCR(cBR));
            } else {
                if (cBR.state == CBgmResource.e_State.NONE) {
                    m_iNumLoading++;
                    StartCoroutine(loadingCR(cBR));
                }
            }
            return cBR;
        }
        //==========================================================================
        /*!release
         * @brief	非常駐型リソースを解放する.
         * @note	オブジェクトの解放はシーン遷移に任せる.
        */
        void release() {
            List<uint> lstRemove = new List<uint>();
            foreach (CBgmResource cBR in m_dicBgmResource.Values) {
                if (!cBR.isRemain) {
                    lstRemove.Add(cBR.id);
                }
            }
            foreach (uint id in lstRemove) {
                m_dicBgmResource[id].release();
                m_dicBgmResource.Remove(id);
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
            foreach (CBgmResource cBR in m_dicBgmResource.Values) {
                if (cBR.isRemain) {
                    if (cBR.get() != null) {
                        UnityEngine.Object.DestroyImmediate(cBR.get(), true);
                    }
                }
            }
        }
        //==========================================================================
        /*!loading
            @brief	テクスチャリソースを非同期読み込みするためのコルーチン.
        */
        IEnumerator loadingCR(CBgmResource cBR) {
            cBR.state = CBgmResource.e_State.LOADING;

            CAssetBundleMgr cAssetBundleMgr = CAssetBundleMgr.Instance;
            CAssetBundle cAB = null;
            int iResult = 1;
            while (iResult > 0) {
                iResult = cAssetBundleMgr.checkToLoadAssetbundle(ref cAB, cBR);
                if (iResult < 0) {
                    m_iNumLoading--;
                    yield break;
                } else if (iResult > 0) {
                    yield return 0;
                }
            }
            AssetBundle ab = cAB;
            AudioClip[] aAudio = ab.LoadAllAssets<AudioClip>();
            AudioClip acLoop = null;
            AudioClip acIntro = null;
            foreach (AudioClip ac in aAudio) {
                if (ac.name.Contains(".intro")) {
                    acIntro = ac;
                } else {
                    acLoop = ac;
                }
            }
            if (acLoop == null) {
                Debug.LogError("this asset is not audio clip" + cBR);
                cBR.state = CBgmResource.e_State.NONE;
                yield break;
            }
            cBR.flag = cAB.flag;
            cBR.set(acLoop, acIntro);
            cBR.state = CBgmResource.e_State.LOADED;
            m_iNumLoading--;
        }
        static protected CBgmResourceMgr m_instance;
        public static CBgmResourceMgr Instance {
            get {
                return m_instance;
            }
        }
    };
}
