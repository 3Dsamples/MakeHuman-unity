//==============================================================================================
/*!CItemBoxBase
	@file  CItemBoxBase

	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System.Collections.Generic;
namespace KS {
    public class CMessageDataSheetMgr : IManager {
        // key:type(FiveCC).
        protected Dictionary<uint, CMessageDataSheet> m_dicMessageDataSheet = new Dictionary<uint, CMessageDataSheet>();
        SystemLanguage m_language;
        //==========================================================================
        /*!コンストラクタ.
            @brief	Constructor
        */
        public CMessageDataSheetMgr() {
            if (m_instance != null) {
                Debug.LogError("already exist instance");
            }
            m_instance = this;
            m_dicMessageDataSheet.Clear();
        }
        //==========================================================================
        /*!アセットバンドルIDを取得.
            @brief	getAssetBundleId
        */
        public uint[] getAssetBundleIds() {
            SystemLanguage lang = KsSoftConfig.language;
            uint mTarget = AssetBundleId.MessageData + (uint)lang;
            if (CAssetBundleMgr.Instance.isExist(mTarget)) {
                m_language = lang;
                return new uint[] { mTarget };
            }
            Debug.LogError("this locale is not implemented:" + lang);
            m_language = SystemLanguage.English;
            return new uint[] { AssetBundleId.MessageData + (uint)SystemLanguage.English };
        }

        //==========================================================================
        /*!
            @brief	initialize
        */
        public bool initialize(CAssetBundle[] aAB) {
            AssetBundle ab = aAB[0];

            UnityEngine.Object[] aObject = ab.LoadAllAssets();

            foreach (UnityEngine.Object o in aObject) {
                CSerializableScript cSS = o as CSerializableScript;
                if (cSS == null) {
                    Debug.LogError("this object is not CSrializableScript.");
                    continue;
                }
                load(cSS.m_buffer);
            }
            return true;
        }
        //==========================================================================
        /*!
            @brief	initialize
        */
        virtual public bool initialize() {
            return true;
        }
        //==========================================================================
        /*!
            @brief	load
        */
        public bool load(byte[] buffer) {
            CReadVariable cVariable = new CReadVariable(buffer);
            t_MessageData tMD = new t_MessageData();
            tMD.read(cVariable);

            foreach (t_MessageDataSheet sheet in tMD.m_aSheet) {
                m_dicMessageDataSheet[sheet.m_type] = new CMessageDataSheet(sheet);
            }
            initialize();
            return true;
        }
        //==========================================================================
        /*!IManager.release
            @brief	release
        */
        public void release() {
            m_instance = null;
        }
        //==========================================================================
        /*!
            @brief	findSheet
        */
        public CMessageDataSheet find(uint type) {
            CMessageDataSheet cTarget;
            if (!m_dicMessageDataSheet.TryGetValue(type, out cTarget)) {
                //			Debug.LogWarning("can't find message sheet:" + new FiveCC(type));
                return null;
            }
            return cTarget;
        }
        //==========================================================================
        /*!
            @brief	validate
        */
        public void validate(CWinCtrlRichText rt) {
#if UNITY_EDITOR
            foreach (CMessageDataSheet sheet in m_dicMessageDataSheet.Values) {
                sheet.validate(rt);
            }
#endif
        }
        public SystemLanguage language {
            get {
                return m_language;
            }
        }
        //==========================================================================
        /*!
            @brief	Instance
        */
        static protected CMessageDataSheetMgr m_instance;
        public static CMessageDataSheetMgr Instance {
            get {
                return m_instance;
            }
        }
    }
}
