//==============================================================================================
/*!テクスチャをまとめたアセットを管理する.
	@file  CTexturePacksMgr	
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
        @brief	class CTexturePacksMgr
        @note	this class is sigleton
    */
    public class CTexturePacksMgr : MonoBehaviour, ILoader {
        protected Dictionary<uint, CTexturePacks> m_dicTexturePacks = new Dictionary<uint, CTexturePacks>();
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
            foreach (CTexturePacks cTP in m_dicTexturePacks.Values) {
                cTP.release();
            }
            m_dicTexturePacks.Clear();
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
            @brief	テクスチャパックを参照する.		
        */
        public CTexturePacks reference(uint id) {
            if (CMainSystemBase.Instance.isChangingScene) {
#if UNITY_EDITOR
                Debug.Log("chaging scene now....");
#endif
                return null;
            }
            CAssetBundleMgr cAssetBundleMgr = CAssetBundleMgr.Instance;
            if (!cAssetBundleMgr.isExist(id)) {
                return null;
            }
            CTexturePacks cTP;
            // アセットバンドルとして存在する.
            if (!m_dicTexturePacks.TryGetValue(id, out cTP)) {
                // if unregist,create
                cTP = new CTexturePacks(id);
                m_dicTexturePacks[id] = cTP;
                m_iNumLoading++;
                StartCoroutine(loadingCR(cTP));
            } else {
                if (cTP.state == CTexturePacks.e_State.NONE) {
                    m_iNumLoading++;
                    StartCoroutine(loadingCR(cTP));
                }
            }
            return cTP;
        }
        //==========================================================================
        /*!release
         * @brief	非常駐型マテリアルを解放する.
         * @note	オブジェクトの解放はシーン遷移に任せる.
        */
        void release() {
            List<uint> lstRemove = new List<uint>();
            foreach (CTexturePacks cTP in m_dicTexturePacks.Values) {
                if (!cTP.isRemain) {
                    lstRemove.Add(cTP.id);
                    cTP.release();
                }
            }
            foreach (uint id in lstRemove) {
                m_dicTexturePacks.Remove(id);
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
            foreach (CTexturePacks cTP in m_dicTexturePacks.Values) {
                if (cTP.isRemain) {
                    cTP.Destroy();
                }
            }
        }
        //==========================================================================
        /*!loading
            @brief	テクスチャリソースを非同期読み込みするためのコルーチン.
        */
        IEnumerator loadingCR(CTexturePacks cTP) {
            cTP.state = CTextureResource.e_State.LOADING;

            CAssetBundleMgr cAssetBundleMgr = CAssetBundleMgr.Instance;
            CAssetBundle cAB = null;
            int iResult = 1;
            while (iResult > 0) {
                iResult = cAssetBundleMgr.checkToLoadAssetbundle(ref cAB, cTP);
                if (iResult < 0) {
                    m_iNumLoading--;
                    yield break;
                } else if (iResult > 0) {
                    yield return 0;
                }
            }
            AssetBundle ab = cAB;
            cTP.flag = cAB.flag;
            cAB.flag &= ~e_ResourceFlag.REMAIN;
            // テクスチャをアタッチ.
            // Texture Resource.
            Texture2D[] textures = ab.LoadAllAssets<Texture2D>();
            if (textures == null) {
                Debug.LogError("can't find texture2d(s)!!" + MulId.ToString(cTP.id));
            }
            cTP.textures = textures;
            cTP.state = CTextureResource.e_State.LOADED;
            m_iNumLoading--;
        }
        static protected CTexturePacksMgr m_instance;
        public static CTexturePacksMgr Instance {
            get {
                return m_instance;
            }
        }
    };
}
