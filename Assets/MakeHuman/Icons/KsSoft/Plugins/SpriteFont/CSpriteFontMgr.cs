//==============================================================================================
/*!?スプライトフォントデータマネージャ.
	@file  CSpriteFontMgr
	
	( counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace KS {
    public class CSpriteFontMgr : MonoBehaviour, ILoader {
        Dictionary<uint, CSpriteFont> m_dicFont = new Dictionary<uint, CSpriteFont>();
        bool m_bIsLoading = false;

        //==========================================================================
        /*!Awake
            @brief	Unity callback
        */
        void Awake() {
            if (m_instance != null) {
                Debug.LogError("already instance exist");
            }
            m_instance = this;
            CAssetBundleMgr cAssetBundleMgr = CAssetBundleMgr.Instance;
            if (cAssetBundleMgr != null) {
                m_bIsLoading = true;
            }
        }
        //==========================================================================
        /*!Start
         * @brief	Unity Callback
        */
        void Start() {
            if (CAssetBundleMgr.Instance != null) {
                if (KsSoftConfig.FontsAssetbundleId != 0) {
                    StartCoroutine(loadingCR());
                } else {
                    m_bIsLoading = false;
                }
            }
        }
        //==========================================================================
        /*!OnDestroy
         * @brief	Unity Callback
        */
        void OnDestroy() {
            m_instance = null;
        }
        //==========================================================================
        /*!フォントデータ取得.
            @brief	reference
        */
        public CSpriteFont reference(uint id) {
            CSpriteFont cSF = null;
            if (!m_dicFont.TryGetValue(id, out cSF)) {
                cSF = loadFromResource(id);
                if (cSF == null) {
                    Debug.LogWarning("can't reference sprite font:" + new FiveCC(id).ToString());
                    return null;
                }
            }
            return cSF;
        }
        //==========================================================================
        /*!リソースからフォントデータを読み込む.
            @brief	loadFromResource
        */
        public CSpriteFont loadFromResource(uint mId) {
            // 既に読み込まれているかチェック.
            if (m_dicFont.ContainsKey(mId)) {
                return m_dicFont[mId];
            }
            // フォント情報を読み込む.
            string path = new FiveCC(mId).ToString();
            CSerializableScript cSS = Resources.Load(path + ".font") as CSerializableScript;
            if (cSS == null) {
                Debug.LogError("can't find resource:" + path + ".font");
                return null;
            }
            t_SpriteFont tSF = new t_SpriteFont();
            CReadVariable cVariable = new CReadVariable(cSS.m_buffer, 0);
            tSF.read(cVariable);

            // テクスチャ情報を読み込む.
            Texture2D texFont = Resources.Load(path + ".tex") as Texture2D;
            if (texFont == null) {
                Debug.LogError("can't find resource:" + path + ".tex");
                return null;
            }
            texFont = GameObject.Instantiate(texFont) as Texture2D;
            //		CMainSystemBase.Instance.regist(texFont);			
            // 登録.
            CSpriteFont cSF = new CSpriteFont(tSF, texFont);
            m_dicFont[tSF.m_id] = cSF;

            return cSF;
        }
        //==========================================================================
        /*!読み込み中かどうか判定.
            @brief	isLoading
        */
        public bool isLoading() {
            return m_bIsLoading;
        }
        //==========================================================================
        /*!フォントデータ読み込み.
            @brief	loading
        */
        IEnumerator loadingCR() {
            CAssetBundleMgr cAssetBundleMgr = CAssetBundleMgr.Instance;
            if (cAssetBundleMgr == null) {
                yield break;
            }
			if (!cAssetBundleMgr.isInitialized()) {
				yield return 0;
			}
			// ロケールに対応するフォントがあるかどうか調べる.
			MulId mId = new MulId(KsSoftConfig.FontsAssetbundleId);
			mId.Lower = (uint)KsSoftConfig.language;
			if (!CAssetBundleMgr.Instance.isExist(mId)) {
				mId.Lower = 0;    //存在しないときはデフォルトで読み込む.
			}

			CAssetBundle cAB = null;
            int iResult = 1;
            while (iResult > 0) {
                iResult = cAssetBundleMgr.checkToLoadAssetbundle(ref cAB, mId);
                if (iResult < 0) {
                    Debug.Log("error" + iResult);
                    yield break;
                } else if (iResult > 0) {
                    yield return 0;
                }
            }
            AssetBundle ab = cAB;

            CSerializableScript cSS = ab.LoadAsset<CSerializableScript>("fonts");
            if (cSS == null) {
                yield break;
            }
            t_SpriteFonts tSpriteFonts = new t_SpriteFonts();
            CReadVariable cVariable = new CReadVariable(cSS.m_buffer, 0);
            tSpriteFonts.read(cVariable);
            foreach (t_SpriteFont tSF in tSpriteFonts.m_aFont) {
                //			Debug.Log (new FiveCC(tSF.m_id) + ":" + tSF.m_textureName);
                Texture2D texture = ab.LoadAsset<Texture2D>(tSF.m_textureName);
                texture = GameObject.Instantiate(texture) as Texture2D;
                //			CMainSystemBase.Instance.regist(texture);			
                m_dicFont[tSF.m_id] = new CSpriteFont(tSF, texture);
            }
            m_bIsLoading = false;
        }
        static protected CSpriteFontMgr m_instance;
        public static CSpriteFontMgr Instance {
            get {
                return m_instance;
            }
        }
    }
}
