//==============================================================================================
/*!メインシステム共通(常駐).
	@file  CMainSystemBase
*/
//==============================================================================================
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;

namespace KS {
    public class CMainSystemBase : MonoBehaviour {
        bool m_bInitialize;
        Camera m_UICamera;
        ILoader[] m_aLoader;                    //ローダを持つオブジェクト.
        List<IManager> m_lstOnceInitializeManager = new List<IManager>();   //アセットを用いて一度だけ初期化する必要のあるオブジェクト.

        protected bool m_bChangingScene = false;
        protected bool m_bBlockChangeScene = false;
        protected string m_sNowScene = "";
        protected float m_gcTime;
        protected bool m_bAutoGarbage = true;
        const float m_gcSpan = 0.25f;
        // フェード関連.
        GameObject m_goFadeObject = null;
        CFadeScreen m_cFadeScreen = null;
        // シーンロード時に削除しないオブジェクト.
        List<UnityEngine.Object> m_lstDontDestroyObject = new List<UnityEngine.Object>();
        static KsRandom m_random = new KsRandom();
        static CConfig m_cConfig;
        static CMainSystemBase() {
            KsSoftConfig.initialize();
            m_random.initialize((uint)System.Environment.TickCount);
#if !(UNITY_IOS || UNITY_ANDROID || UNITY_WEBPLAYER)
            m_cConfig = new CConfig();
            m_cConfig.load("config.cf");
            m_cConfig.load("config.local.cf");
#endif
        }
        //==========================================================================
        /*!Awake
         * @brief	Unity Callback
        */
        protected void Awake() {
            SceneManager.sceneLoaded += onLevelWasLoaded;
            m_bInitialize = false;
            if (m_instance != null) {
                Debug.LogError("already instance exist");
            }
            m_instance = this;

            // create UI Camera
            m_UICamera = KsSoftUtility.addUICamera(gameObject, e_Layer.Window, 1f);

            //------------------------------------------------------------
            // 消されないように設定.
            DontDestroyOnLoad(gameObject);

            m_gcTime = m_gcSpan;
            transform.position = new Vector3(0f, 0f, -100f);
        }
        //==========================================================================
        /*!IManagerをインターフェースとしてもつマネージャを登録する.
         * @brief	addManager
        */
        protected void addManager(IManager mgr) {
            m_lstOnceInitializeManager.Add(mgr);
        }
        //==========================================================================
        /*!Start
         * @brief	Unity Callback
        */
        IEnumerator Start() {
            MonoBehaviour[] aLoader = GetComponents<MonoBehaviour>();
            List<ILoader> lstLoader = new List<ILoader>();
            foreach (MonoBehaviour mb in aLoader) {
                if (mb is ILoader) {
                    lstLoader.Add(mb as ILoader);
                }
                if (mb is IManager) {
                    m_lstOnceInitializeManager.Add(mb as IManager);
                }
            }
            m_aLoader = lstLoader.ToArray();

            yield return 0;
            CAssetBundleMgr cAssetBundleMgr = CAssetBundleMgr.Instance;
            if (cAssetBundleMgr == null) {
                yield break;
            }
            m_bChangingScene = false;
            //-------------------------------------------------------------------------
            // アセットバンドルマネージャ初期化.
            while (!cAssetBundleMgr.isInitialized()) {
                yield return 0;
            }
            onAddedLoader();
            foreach (ILoader loader in lstLoader) {
                while (loader.isLoading()) {
                    yield return 0;
                }
            }
            // 登録されているマネージャの数だけ初期化を回す.
            foreach (IManager mgr in m_lstOnceInitializeManager) {
                Debug.Log(mgr);
                uint[] aId = mgr.getAssetBundleIds();
                if (aId.Length == 0) {
                    Debug.LogError("id list is null");
                    continue;
                }
                CAssetBundle[] aAB = new CAssetBundle[aId.Length];
                for (uint i = 0; i < aAB.Length; ++i) {
                    CAssetBundle cAB = cAssetBundleMgr.reference(aId[i]);
                    if (cAB == null) {
                        yield break;
                    }
                    if (!cAB.isLoaded) {
                        // wait for load asset bundle
                        while (cAB.state != CAssetBundle.e_State.LOADED) {
                            if (cAB.state == CAssetBundle.e_State.NOEXIST) {
                                // occur error
                                yield break;
                            }
                            yield return 0;
                        }
                    }
                    aAB[i] = cAB;
                }
                // 初期化.
                mgr.initialize(aAB);
            }
            m_bInitialize = true;
            initialize();
        }
        //==========================================================================
        /*!Initialize
         * @brief	Initialize
        */
        virtual protected void initialize() {
            KsSoftUtility.initalize();
        }
        //==========================================================================
        /*!onAddedLoader
         * @brief	ILoader系のAddが終わった直後に呼ばれる.
        */
        virtual protected void onAddedLoader() {
        }
        //==========================================================================
        /*!Update
         * @brief	Unity Callback
        */
        protected void Update() {
            CWindowMgr cWinMgr = CWindowMgr.Instance;
            if (cWinMgr != null) {
                cWinMgr.update(m_UICamera);
            }
            // ガーベージコレクション.
            m_gcTime -= Time.deltaTime;
            /*
                    if (m_gcTime <= 0f) {
                        System.GC.Collect(2);
                        m_gcTime =+ m_gcSpan;
                    }
             */
            if (m_bAutoGarbage) {
                System.GC.Collect();
            }
        }
        //==========================================================================
        /*!onLevelWasLoaded
            @brief Unity call back
        */
        void onLevelWasLoaded(Scene scenename, LoadSceneMode mode) {
            m_bChangingScene = false;
        }
        //==========================================================================
        /*!OnDestroy
         * @brief	Unity Callback
        */
        protected void OnDestroy() {
            foreach (IManager mgr in m_lstOnceInitializeManager) {
                mgr.release();
            }
            m_instance = null;
        }
        //==========================================================================
        /*!ガーベージコレクションを要求する.
            @brief GarbageCollection.
        */
        public void collectGarbage(bool force = false) {
            if (m_bAutoGarbage || force) {
                System.GC.Collect(2);
            }
        }
        //==========================================================================
        /*!シーンを遷移する直前に呼ばれる.
            @brief OnChangeScene.
        */
        virtual protected void OnChangeScene() {
        }
        //==========================================================================
        /*!シーンを遷移する直後に呼ばれる.
            @brief OnChangedScene.
        */
        virtual protected void OnChangedScene(string sScene) {
        }
        //==========================================================================
        /*!シーンを遷移させる.
            @brief changeScene.
        */
        public bool changeScene(string sScene, bool bForceChange = false) {
            if (m_sNowScene == sScene && !bForceChange) {
                return false;
            }
            if (m_bChangingScene) {
                Debug.LogWarning("already execute scene change process");
            }
            m_sNowScene = sScene;
            m_bChangingScene = true;
            StartCoroutine("changeSceneCR", sScene);
            return true;
        }
        IEnumerator changeSceneCR(string sScene) {
            if (m_cFadeScreen != null) {
                if (!m_cFadeScreen.isFadeOut) {
                    m_cFadeScreen.startFadeOut();
                    while (m_cFadeScreen.isFade) {
                        yield return 0;
                    }
                }
            }
            while (isLoading) {
                yield return 0;
            }
            while (m_bBlockChangeScene) {
                yield return 0;
            }
            OnChangeScene();
            // アセットバンドル情報を解放.
            if (CAssetBundleMgr.Instance != null) {
                CAssetBundleMgr.Instance.release();
            }
            // シーンが切り替わる前にすべてのウィンドウを閉じる(必須).
            if (CWindowMgr.Instance != null) {
                CWindowMgr.Instance.release();
            }
            // ロードする直前にGCを発動する.
            System.GC.Collect(2);

            // ロード開始.
            SceneManager.LoadScene(sScene);
            OnChangedScene(sScene);
        }
        //==========================================================================
        /*!常駐させたオブジェクトを削除する.
            @brief destroyRegisted.
        */
        public void destroyRegisted() {
            foreach (UnityEngine.Object obj in m_lstDontDestroyObject) {
                Destroy(obj);
            }
            m_lstDontDestroyObject.Clear();
        }
        //==========================================================================
        /*!常駐させる.
            @brief regist.
        */
        public void regist(UnityEngine.Object obj) {
            UnityEngine.Object.DontDestroyOnLoad(obj);
            m_lstDontDestroyObject.Add(obj);
        }
        //==========================================================================
        /*!フェードスクリーンを取得する.
            @brief	fadeScreen.
        */
        public GameObject fadeObject {
            set {
                m_goFadeObject = value;
                if (value != null) {
                    // フェードオブジェクト取得.
                    m_cFadeScreen = m_goFadeObject.GetComponent<CFadeScreen>();
                } else {
                    m_cFadeScreen = null;
                }
            }
        }
        //==========================================================================
        /*!fadeScreenを取得する.
            @brief	fadeScreen
        */
        public CFadeScreen fadeScreen {
            get {
                return m_cFadeScreen;
            }
        }
        //==========================================================================
        /*!UI用カメラを取得する.
            @brief	uicamera
        */
        public Camera uicamera {
            get {
                return m_UICamera;
            }
        }
        //==========================================================================
        /*!シーン名を取得.
            @brief	scenename
        */
        public string scenename {
            get {
                return m_sNowScene;
            }
        }
        //==========================================================================
        /*!シーン変更中かどうか?.
            @brief	isChangingScene
        */
        public bool isChangingScene {
            get {
                return m_bChangingScene;
            }
        }
        //==========================================================================
        /*!シーン変更をブロックしているかどうか?.
            @brief	blockChangeScene
        */
        public bool blockChangeScene {
            get {
                return m_bBlockChangeScene;
            }
            set {
                m_bBlockChangeScene = value;
            }
        }
        //==========================================================================
        /*!初期化がおわっているか?.
            @brief	isInitialized
        */
        public bool isInitialized {
            get {
                return m_bInitialize;
            }
        }
        //==========================================================================
        /*!読み込んでいるかどうか?.
            @brief	isLoading
        */
        virtual public bool isLoading {
            get {
                foreach (ILoader loader in m_aLoader) {
                    if (loader.isLoading()) {
                        return true;
                    }
                }
                return false;
            }
        }
        //==========================================================================
        /*!乱数取得.
            @brief	randi.
        */
        static public int randi() {
            return m_random.ivalue;
        }
        static public uint randu() {
            return m_random.uvalue;
        }
        static public float randf() {
            return m_random.fvalue;
        }
        static public int randi(int min, int max) {
            return m_random.Range(min, max);
        }
        static public uint randu(uint min, uint max) {
            return m_random.Range(min, max);
        }
        static public float randf(float min, float max) {
            return m_random.Range(min, max);
        }
        //==========================================================================
        /*!Instance.
            @brief	Instance.
        */
        static protected CMainSystemBase m_instance;
        public static CMainSystemBase Instance {
            get {
                return m_instance;
            }
        }
    }
}
