//==============================================================================================
/*!CSpriteDataを束ねて管理するマネージャ.
	@file  Sprite Data Manager
	
	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace KS {
    public class CSpriteDataMgr : CSpriteDataMgrBase {
        int m_iNumLoading = 0;
        //==========================================================================
        /*!Awake
            @brief	Unity callback
        */
        void Awake() {
            if (m_instance != null) {
                Debug.LogError("already instance exist");
            }
            m_instance = this;
        }
        //==========================================================================
        /*!Start
            @brief	Unity callback
        */
        new void Start() {
            // 自動で読み込まないようにする.
        }
        //==========================================================================
        /*!OnDestroy
         * @brief	Unity Callback
        */
        void OnDestroy() {
            m_instance = null;
        }
        //==========================================================================
        /*!読み込み中かチェック.
            @brief	isLoading
        */
        override public bool isLoading() {
            return (m_iNumLoading != 0);
        }
        //==========================================================================
        /*!ロード開始.
            @brief	load
        */
        public void load<Type>(uint mId, bool bRemain) where Type : CSpriteBase {
            if (reference(mId) != null) {
                Debug.LogWarning("already exist sprite data:" + new MulId(mId));
                return;
            }
            m_iNumLoading++;
            StartCoroutine(loadingCR<Type>(mId, bRemain));
        }
        //==========================================================================
        /*!Resourcesからロード.
            @brief	loadFromResources
        */
        public bool loadFromResources<Type>(uint mId, bool bRemain) where Type : CSpriteBase {
            if (reference(mId) != null) {
                return true;
            }
            string path = new MulId(mId).ToString();
            Texture2D texture = Resources.Load(path + ".tex") as Texture2D;
            if (texture == null) {
                Debug.LogError("can't find resource:" + path + ".tex");
                return false;
            }
            CSerializableScript cSS = Resources.Load(path + ".spr") as CSerializableScript;
            CSpriteData cSD = new CSpriteData(cSS);
            if (cSD == null) {
                Debug.LogError("can't find resource:" + path + ".spr");
                return false;
            }
            return load<Type>(cSD, texture, bRemain);
        }
        //==========================================================================
        /*!スプライトメッシュを一つ生成する.
            @brief	create
        */
        public Type create<Type>(CTextureResource cTR) where Type : CSpriteBase {
            return create<Type>(cTR.get(), cTR.spriteData, cTR.id);
        }
        public Type create<Type>(Material mat, CSpriteDataOne[] aData, uint mId) where Type : CSpriteBase {
            // レンダリング用ゲームオブジェクトを作成する.
            GameObject go = createMeshObject(mId);
            Type cBase = go.AddComponent<Type>();

            // 初期化.
            cBase.initialize(mat, aData);

            return cBase;
        }
        //==========================================================================
        /*!スプライトメッシュをインスタンス化する.
            @brief	instantiate
        */
        public Type instantiate<Type>(CSpriteBase src, uint mId) where Type : CSpriteBase {
            GameObject go = createMeshObject(mId);
            go.layer = src.gameObject.layer;
            Type cBase = go.AddComponent<Type>();

            cBase.initialize(src);

            return cBase;
        }
        //==========================================================================
        /*!スプライトデータ読み込み.
            @brief	loading
        */
        IEnumerator loadingCR<Type>(uint mId, bool bRemain) where Type : CSpriteBase {
            CAssetBundleMgr cAssetBundleMgr = CAssetBundleMgr.Instance;
            if (cAssetBundleMgr == null) {
                --m_iNumLoading;
                yield break;
            }
            CAssetBundle cAB = null;
            int iResult = 1;
            while (iResult > 0) {
                iResult = cAssetBundleMgr.checkToLoadAssetbundle(ref cAB, mId);
                if (iResult < 0) {
                    --m_iNumLoading;
                    yield break;
                } else if (iResult > 0) {
                    yield return 0;
                }
            }
            AssetBundle ab = cAB;

            CSerializableScript cSS = ab.LoadAsset<CSerializableScript>("sprite");
            CSpriteData cSD = new CSpriteData(cSS);
            Texture2D texture = ab.LoadAsset<Texture2D>("texture");

            if (load<Type>(cSD, texture, bRemain)) {
                --m_iNumLoading;
            } else {
                yield break;
            }
        }
        bool load<Type>(CSpriteData cSD, Texture2D texture, bool bRemain) where Type : CSpriteBase {
            uint mId = (uint)cSD.m_mId;
            // スプライトデータを常駐させるためにリソース系をインスタンス化する.
            Material m = new Material(Shader.Find(cSD.shader));
            m.mainTexture = Instantiate(texture) as Texture2D;

            CSpriteBase cBase = create<Type>(m, cSD.m_aData, mId);
            if (cBase == null) {
                UnityEngine.Object.DestroyImmediate(cBase.gameObject, true);
                Debug.LogError("GameObject do'nt have cBase:" + name);
                --m_iNumLoading;
                return false;
            }
            // 常駐させるときは、DontDestroyOnLoadを適用.
            if (bRemain) {
                CMainSystemBase.Instance.regist(cBase.gameObject);
            }
            // エントリ.
            entry(mId, cBase);
            return true;
        }
        static CSpriteDataMgr m_instance = null;
        public static CSpriteDataMgr Instance {
            get {
                return m_instance;
            }
        }
    };
}
