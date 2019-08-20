//==============================================================================================
/*!汎用的に使うテクスチャをキャッシュする.
	@file  CTextureResourceMgr
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
        @brief	class CTextureResourceMgr
        @note	this class is sigleton
    */
    public class CTextureResourceMgr : MonoBehaviour, ILoader {
        protected Dictionary<uint, CTextureResource> m_dicTextureResource = new Dictionary<uint, CTextureResource>();
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
            foreach (CTextureResource cTR in m_dicTextureResource.Values) {
                cTR.release();
            }
            m_dicTextureResource.Clear();
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
            @brief	テクスチャリソースを参照する.
        */
        public CTextureResource reference(uint id) {
            if (CMainSystemBase.Instance.isChangingScene) {
#if UNITY_EDITOR
                Debug.Log("chaging scene now....");
#endif
                return null;
            }
            CAssetBundleMgr cAssetBundleMgr = CAssetBundleMgr.Instance;
            CTextureResource cTR;
            if (cAssetBundleMgr.isExist(id)) {
                // アセットバンドルとして存在する.
                if (!m_dicTextureResource.TryGetValue(id, out cTR)) {
                    // if unregist,create
                    cTR = new CTextureResource(id);
                    m_dicTextureResource[id] = cTR;
                    m_iNumLoading++;
                    StartCoroutine(loadingCR(cTR));
                } else {
                    if (cTR.state == CTextureResource.e_State.NONE) {
                        m_iNumLoading++;
                        StartCoroutine(loadingCR(cTR));
                    }
                }
            } else {
                // Resource側に存在する可能性がある.
                cTR = loadFromResources(id, false);
            }
            return cTR;
        }
        //==========================================================================
        /*!テクスチャをResourcesからロードする.
            @brief	loadFromResources.
        */
        public CTextureResource loadFromResources(uint id, bool bRemain) {
            CTextureResource cTR;
            // 既に読み込まれているなら読み込まない.
            if (m_dicTextureResource.TryGetValue(id, out cTR)) {
                return cTR;
            }
            // if unregist,create
            cTR = new CTextureResource(id);
            cTR.state = CTextureResource.e_State.NOEXIST;
            cTR.flag = (bRemain) ? e_ResourceFlag.REMAIN : e_ResourceFlag.NONE;

            string path = new MulId(id).ToString();
            CSpriteData cSD = null;
            Texture2D texture = null;
            CSerializableScript cSS = Resources.Load(path + ".spr") as CSerializableScript;
            //-----------------------------
            // スプライトデータ登録.
            if (cSS != null) {
                cSD = new CSpriteData(cSS);
            }
            //-----------------------------
            // テクスチャロード.
            cSS = Resources.Load(path + ".img") as CSerializableScript;
            if (cSS != null) {
                texture = new Texture2D(4, 4);
                texture.LoadImage(cSS.m_buffer);
            } else {
                texture = Resources.Load(path + ".tex") as Texture2D;
                if (texture == null) {
                    Debug.LogError("can't find resource:" + path + ".tex");
                    return null;
                }
                texture = GameObject.Instantiate(texture);
            }
            if (!setTextureResource(cTR, texture, cSD)) {
                return null;
            }
            cTR.flag |= e_ResourceFlag.RESOURCE;
            // リソース登録.
            m_dicTextureResource[id] = cTR;
            return cTR;
        }
        //==========================================================================
        /*!release
         * @brief	非常駐型マテリアルを解放する.
         * @note	オブジェクトの解放はシーン遷移に任せる.
        */
        void release() {
            List<uint> lstRemove = new List<uint>();
            foreach (CTextureResource cTR in m_dicTextureResource.Values) {
                if (!cTR.isRemain) {
                    lstRemove.Add(cTR.id);
                    cTR.release();
                }
            }
            foreach (uint id in lstRemove) {
                m_dicTextureResource.Remove(id);
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
            foreach (CTextureResource cTR in m_dicTextureResource.Values) {
                if (cTR.isRemain) {
                    if (cTR.get() != null) {
                        UnityEngine.Object.DestroyImmediate(cTR.get(), true);
                    }
                }
            }
        }
        //==========================================================================
        /*!loading
            @brief	テクスチャリソースを非同期読み込みするためのコルーチン.
        */
        IEnumerator loadingCR(CTextureResource cTR) {
            cTR.state = CTextureResource.e_State.LOADING;

            CAssetBundleMgr cAssetBundleMgr = CAssetBundleMgr.Instance;
            CAssetBundle cAB = null;
            int iResult = 1;
            while (iResult > 0) {
                iResult = cAssetBundleMgr.checkToLoadAssetbundle(ref cAB, cTR);
                if (iResult < 0) {
                    m_iNumLoading--;
                    yield break;
                } else if (iResult > 0) {
                    yield return 0;
                }
            }
            AssetBundle ab = cAB;
            cTR.flag = cAB.flag;
            cAB.flag &= ~e_ResourceFlag.REMAIN;
            // テクスチャをアタッチ.
            CSpriteData cSD = null;
            // Texture Resource.
            if (ab.Contains("sprite")) {
                CSerializableScript cSS = ab.LoadAsset<CSerializableScript>("sprite");
                cSD = new CSpriteData(cSS);
            }
            Texture2D texture = null;
            if (ab.Contains("image")) {
                CSerializableScript cSS = ab.LoadAsset<CSerializableScript>("image");
                texture = new Texture2D(4, 4);
                texture.LoadImage(cSS.m_buffer);
                texture.wrapMode = TextureWrapMode.Clamp;
            } else {
                texture = ab.LoadAsset<Texture2D>("texture");
            }
            if (texture == null) {
                Debug.LogError("can't find texture.asset!!" + MulId.ToString(cTR.id));
            }
            if (!setTextureResource(cTR, texture, cSD)) {
                m_iNumLoading--;
                yield break;
            }
            m_iNumLoading--;
        }
        bool setTextureResource(CTextureResource cTR, Texture2D texture, CSpriteData cSD) {
            string shader = CSpriteData.DefaultShader;
            if (cSD != null) {
                cTR.spriteData = cSD.m_aData;
                shader = cSD.shader;
            } else {
                CSpriteDataOne[] aSDO = new CSpriteDataOne[1];
                aSDO[0] = new CSpriteDataOne(FiveCC.Id("image"));
                cTR.spriteData = aSDO;
            }
            // マテリアルを設定.
            Shader sh = Shader.Find(shader);
            if (sh == null) {
                cTR.state = CTextureResource.e_State.NOEXIST;
                return false;
            }
            Material m = new Material(sh);
            m.SetTexture("_MainTex", texture);
            cTR.set(m);

            cTR.state = CTextureResource.e_State.LOADED;
            return true;
        }
        static protected CTextureResourceMgr m_instance;
        public static CTextureResourceMgr Instance {
            get {
                return m_instance;
            }
        }
    };
}
