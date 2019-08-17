//==============================================================================================
/*!アセットバンドルマネージャ.
	@file  CAssetBundleMgr
*/
//==============================================================================================
//#define		OUTPUT_LOG
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KS {
    //==========================================================================
    /*!
        @brief	class CAssetBundle
    */
    public class CAssetBundle : CResource<AssetBundle> {
        protected UnityWebRequest m_www;
        protected Texture2D m_texture;
        protected bool m_bIsCached = false;
        //==========================================================================
        /*!CAssetBundle
            @brief	Constructor
        */
        public CAssetBundle(uint id) : base(id) {
        }
        public CAssetBundle(t_AssetVersionOne version) : base(version.m_mId) {
            m_version = version;
            flag = (e_ResourceFlag)m_version.m_uFlag;
        }
        public void set(t_AssetVersionOne version) {
            m_version = version;
            flag = (e_ResourceFlag)m_version.m_uFlag;
        }
        override public void Destroy() {
            if (m_www != null) {
                m_www.Dispose();
            }
            m_www = null;
            if (m_Object != null) {
                m_Object.Unload(true);
            }
            base.Destroy();
        }
        //==========================================================================
        /*!version
            @brief	version
        */
        public uint version {
            get {
                return (uint)m_version.m_iVersion;
            }
        }
        public UnityWebRequest www {
            set {
                m_www = value;
            }
            get {
                return m_www;
            }
        }
        public float progress {
            get {
                if (isLoaded) {
                    return 1f;
                }
                if (m_www == null) {
                    return -1f;
                }
                return m_www.downloadProgress;
            }
        }
        public bool isImage {
            get {
                if (width == 0 || height == 0) {
                    return false;
                }
                return true;
            }
        }
        public string path {
            get {
                return m_version.m_path;
            }
        }
        public int width {
            get {
                return m_version.m_width;
            }
        }
        public int height {
            get {
                return m_version.m_height;
            }
        }
        protected t_AssetVersionOne m_version;
        public bool isCached {
            get {
                return m_bIsCached;
            }
            set {
                m_bIsCached = value;
            }
        }
        public Texture2D texture {
            get {
                return m_texture;
            }
            set {
                m_texture = value;
            }
        }
    };
    //==========================================================================
    /*!
        @brief	class CAssetBundleMgr
        @note	this class is singleton
    */
    public class CAssetBundleMgr : MonoBehaviour, ILoader {
        public enum e_InitializeState {
            None,
            CheckVersion,
            CheckCacheFile,
            Finish,
            Error,
        }
        static protected string m_sHttpServerPath = KsSoftConfig.httpserver;
        protected CAssetVersion m_cVersion = null;
        protected int m_iPreload = 0;
        protected bool m_isStreaming = KsSoftConfig.UseStreaming;
        protected Dictionary<string, CAssetBundle> m_dicPath = new Dictionary<string, CAssetBundle>();
        protected int m_iNowLoadNum = 0;                            //ダウンロード要求がきている数.
        protected int m_iLastError = 0;
        protected string m_sError;
        protected e_InitializeState m_eInitializeState = e_InitializeState.None;
        protected List<CAssetBundle> m_lstNowLoading = new List<CAssetBundle>();    //同時ダウンロードしている数.
        protected List<CResourceBase> m_lstReleaseResource = new List<CResourceBase>();
        // 定数[必要なら変更してください].
        protected const int MaxConcurrentLoadNum = 8;                   //サーバダウンロード同時最大数.
        protected const long MaximumAvailableDiskSpace = 512 * 1024 * 1024;
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

            outputCachingData();
        }
        //==========================================================================
        /*!初期化を行う.
            @brief	initialize
            @note	assetversionを読みこみキャッシュのリフレッシュを行う.
        */
        void Start() {
            StartCoroutine(initilaizeCo());
        }
        IEnumerator initilaizeCo() {
            //--------------------------------------------------------
            // バージョンファイルを読み込む.
            if (m_cVersion == null) {
                m_cVersion = new CAssetVersion();
                m_eInitializeState = e_InitializeState.CheckVersion;

                StartCoroutine(loading(m_cVersion));
                while (!m_cVersion.isLoaded) {
                    if (m_iLastError < 0) {
                        m_eInitializeState = e_InitializeState.Error;
                        yield break;
                    }
                    yield return 0;
                }
            }
            if (setupAssetVersion(m_cVersion) < 0) {
                error(-4, "initialize error");
                m_eInitializeState = e_InitializeState.Error;
                yield break;
            }
            // キャッシュされているファイルが追い出されないようにする.
            m_eInitializeState = e_InitializeState.CheckCacheFile;
            m_iNowLoadNum = m_dicPath.Count;
            foreach (CAssetBundle cAB in m_dicPath.Values) {
                if (cAB.id == 0) {
                    m_iNowLoadNum--;
                    continue;
                }
                cAB.isCached = Caching.MarkAsUsed(cAB.path, new Hash128((uint)cAB.version, 0, 0, 0));
                yield return 0;
                m_iNowLoadNum--;
            }
            // ここまできたら外部からのアクセスを許可してよい.
            m_eInitializeState = e_InitializeState.Finish;
        }
        //==========================================================================
        /*!AssetVersionを読み込む.
            @brief	loadAssetVersion
            @note	version.unity3dを読み込む.
        */
        public CAssetVersion loadAssetVersion() {
            CAssetVersion cAssetVersion = new CAssetVersion();
            StartCoroutine(loading(cAssetVersion));
            return cAssetVersion;
        }
        //==========================================================================
        /*!AssetVersionを読み込み初期化する.
            @brief	loading
            @note	version.unity3dを読み込む.
        */
        IEnumerator loading(CAssetVersion cAssetVersion) {
            if (cAssetVersion == null) {
                error(-5, "cAssetVersion == null");
                yield break;
            }
            string name = "version" + KsSoftConfig.AssetbundleExt;
            string sPath = getServerPath(m_isStreaming) + name;
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
            string sDebugPath = KsSoftConfig.debugPath + name;
            Debug.Log(sDebugPath + ":" + sPath);
            if (File.Exists(sDebugPath)) {
                sPath = "file://" + sDebugPath;
            }
#else
        if (!m_isStreaming) {
            sPath += "?time=" + KsSoftUtility.getUnixTime();
        }
#endif
            UnityWebRequest www = LoadAssetBundle(sPath);
            yield return www.SendWebRequest();
            // 読み込み失敗.
            if (www.isNetworkError || www.isNetworkError) {
                // 読みこみリトライ.
                www = LoadAssetBundle(sPath);
                yield return www.SendWebRequest();
                if (www.isNetworkError || www.isNetworkError) {
                    error(-1, sPath + ":" + www.error);
                    yield break;
                }
            }
            AssetBundle ab = DownloadHandlerAssetBundle.GetContent(www);
            CSerializableScript cSS = ab.LoadAsset<CSerializableScript>("version");
            if (cSS == null) {
                Debug.Log("can't load version data from " + sPath);
                error(-2, "can't load version data");
                yield break;
            }
            // バージョンデータ構築.
            if (cAssetVersion.read(cSS.m_buffer) != 0) {
                Debug.Log("broken version data:" + sPath);
                error(-3, "broken version data");
                cAssetVersion = null;
                yield break;
            }
            ab.Unload(true);
            www.Dispose();
        }

        //==========================================================================
        /*!AssetVersionのデータに沿って初期化.
            @brief	setupAssetVersion
        */
        int setupAssetVersion(CAssetVersion cVersion) {
            m_cVersion = cVersion;
            if (m_cVersion == null) {
                return -1;
            }
            foreach (t_AssetVersionOne avo in m_cVersion.Versions) {
                CAssetBundle cAB;
                if (!m_dicPath.TryGetValue(avo.m_path, out cAB)) {
                    cAB = new CAssetBundle(avo);
                    m_dicPath[avo.m_path] = cAB;
                } else {
                    cAB.set(avo);
                }
            }
            return 0;
        }
        void error(int iLastError, string sError) {
            m_iLastError = iLastError;
            m_sError = sError;
            Debug.LogError(sError + "(" + iLastError + ")");
        }
        //==========================================================================
        /*!isLoading
            @brief	読み込んでいるかどうか?.
        */
        public bool isLoading() {
            return (m_iNowLoadNum != 0);
        }
        public int loadNum {
            get {
                return m_iNowLoadNum;
            }
        }
        public CAssetBundle[] loadings {
            get {
                return m_lstNowLoading.ToArray();
            }
        }
        //==========================================================================
        /*!AssetBundle読み込み中のエラーを取得.
            @brief	lastError
        */
        public int lastError {
            get {
                return m_iLastError;
            }
        }
        public string errormessage {
            get {
                return m_sError;
            }
        }
        //==========================================================================
        /*!isInitialize
            @brief	初期化が済んでいるか?.
        */
        public bool isInitialized() {
            return (m_eInitializeState == e_InitializeState.Finish) ? true : false;
        }
        //==========================================================================
        /*!initializeState
            @brief	初期化の進み具合をチェックする.
        */
        public e_InitializeState initializeState {
            get {
                return m_eInitializeState;
            }
        }
        //==========================================================================
        /*!reference
            @brief	AssetBundleを参照する
            @param	int	iId				:AssetBundle
        */
        public CAssetBundle reference(uint id) {
            return reference(MulId.ToString(id) + KsSoftConfig.AssetbundleExt);
        }
        public CAssetBundle reference(string file) {
            CAssetBundle cAB;
            if (CMainSystemBase.Instance.isChangingScene) {
                Debug.LogWarning("chaging scene now....");
                return null;
            }
            if (m_dicPath.TryGetValue(file, out cAB)) {
                // ハッシュテーブルに存在する.
                if (cAB.state == CAssetBundle.e_State.NONE) {
                    // 読み開始.
                    cAB.state = CAssetBundle.e_State.LOADING;
                    m_iNowLoadNum++;
                    StartCoroutine(loading(cAB));
                }
            } else {
                Debug.LogError("this asset bundle is not exist!!:" + file);
            }
            return cAB;
        }
        //==========================================================================
        /*!アセットが存在するかどうかチェックする.
            @brief	isExist
            @retval	true:	存在する
                    false:	存在しない.
        */
        public bool isExist(uint id) {
            return isExist(MulId.ToString(id) + KsSoftConfig.AssetbundleExt);
        }
        public bool isExist(string file) {
            if (CMainSystemBase.Instance.isChangingScene) {
                Debug.LogWarning("chaging scene now....");
                return false;
            }
            return m_dicPath.ContainsKey(file);
        }
        //==========================================================================
        /*!startPreload
            @brief	プリロードを開始する.
            @retval	true:プリロード開始.
                    false:プリロード済みなのでキャンセル.
        */
        public bool startPreload() {
            if (m_iPreload != 0) {
                return false;
            }
            m_iPreload = 1;
            StartCoroutine(preload());
            return true;
        }
        IEnumerator preload() {
            // プリロードが必要なら全て読み込みをかける.
            List<CAssetBundle> lstAB = new List<CAssetBundle>();
            foreach (CAssetBundle cAB in m_dicPath.Values) {
                if (cAB.id == 0) {
                    continue;
                }
                if (!cAB.isCached && cAB.state == CAssetBundle.e_State.NONE) {
                    if (cAB.isDownload) {
                        reference(cAB.id);
                    }
                    lstAB.Add(cAB);
                }
            }
            if (lstAB.Count > 0) {
                int nRelease = 0;
                while (nRelease != lstAB.Count) {
                    bool bRelease = false;
                    for (int i = 0; i < lstAB.Count; ++i) {
                        CAssetBundle cAB = lstAB[i];
                        if (cAB == null) {
                            continue;
                        }
                        if (cAB.state == CAssetBundle.e_State.LOADED) {
                            cAB.release();
                            lstAB[i] = null;
                            nRelease++;
                            bRelease = true;
                        }
                    }
                    if (bRelease) {
                        System.GC.Collect(2);
                        Resources.UnloadUnusedAssets();
                    }
                    yield return 0;
                }
            }
            m_iPreload = 2;

        }
        //==========================================================================
        /*!Update
         * @brief	Unity Callback
        */
        void Update() {
            releaseReservedResources();
        }
        void releaseReservedResources() {
            if (m_lstReleaseResource.Count == 0) {
                return;
            }
            for (int i = 0; i < m_lstReleaseResource.Count; ++i) {
                CResourceBase r = m_lstReleaseResource[i];
                if (r.state == CResourceBase.e_State.NONE) {
                    continue;
                }
                if (r.refcnt == 0) {
                    r.Destroy();
                    r.state = CResourceBase.e_State.NONE;
                }
            }
            m_lstReleaseResource.Clear();
        }
        //==========================================================================
        /*!OnDestroy
         * @brief	Unity Callback
        */
        void OnDestroy() {
            releaseReservedResources();
            m_instance = null;
        }
        //==========================================================================
        /*!OnApplicationQuit
            @brief	Unity callback
        */
        void OnApplicationQuit() {
            outputCachingData();
#if OUTPUT_LOG
		Debug.Log("AssetBundle Manager Quit");
#endif
#if DESTORY_OBJECT_ON_QUIT
		// ここを有効にすると.
		foreach (CAssetBundle cAB in m_dicAssetBundle.Values) {
			AssetBundle ab = cAB.get();
			if (ab != null) {
				UnityEngine.Object.DestroyImmediate(ab,true);
			}
		}
#endif
        }
        //==========================================================================
        /*!WWW
            @brief	outputCachingData
        */
        public void outputCachingData() {
#if !UNITY_WEBPLAYER
            Debug.Log("====================================\n");
#endif
        }
        //==========================================================================
        /*!loading
            @brief	AssetBundleを非同期読み込みするためのコルーチン.
            @param	CAssetBundle	cAB	:読み込む対象のAssetBundle
        */
        IEnumerator loading(CAssetBundle cAB) {
            yield return 0;
            string sPath = getServerPath(cAB.isStreaming);
#if OUTPUT_LOG
		Debug.Log("Load start:" + sPath);
#endif
            while (MaxConcurrentLoadNum <= m_lstNowLoading.Count) {
                yield return 0;
            }
            m_lstNowLoading.Add(cAB);
            UnityWebRequest www;
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
            if (cAB.isStreaming) {
                if (cAB.isImage) {
                    sPath += "image/" + cAB.path;
                    www = LoadTexture(sPath);
                } else {
                    sPath += cAB.path;
                    www = LoadAssetBundle(sPath);
                }
                Debug.Log(sPath + ":" + cAB.version);
            } else {
                string sDebugPath;
                if (cAB.isImage) {
                    string[] aDebugPath = KsSoftConfig.debugPath.Split(new char[] { '/', '\\' }, System.StringSplitOptions.RemoveEmptyEntries);
                    aDebugPath[aDebugPath.Length - 1] = "image";
                    sDebugPath = getImagePath(KsSoftConfig.debugPath) + cAB.path;
                } else {
                    sDebugPath = KsSoftConfig.debugPath + "/" + cAB.path;
                }
                if (File.Exists(sDebugPath)) {
                    Debug.Log(cAB.path + ":assetbundle load from local strage[Debug]:" + sDebugPath);
                    if (cAB.isImage) {
                        www = LoadTexture("file://" + sDebugPath);
                    } else {
                        www = LoadAssetBundle("file://" + sDebugPath);
                    }
                } else {
                    if (cAB.isImage) {
                        sPath = getImagePath(sPath) + cAB.path;
                        www = LoadTexture(sPath + "?time=" + KsSoftUtility.getUnixTime());
                    } else {
                        sPath += cAB.path;
                        www = LoadAssetBundle(sPath + "?time=" + KsSoftUtility.getUnixTime(), cAB.version);
                    }
                    Debug.Log(sPath + ":" + cAB.version);
                }
            }
#else
		if (cAB.isStreaming) {
            if (cAB.isImage) {
                sPath += "image/" + cAB.path;
	            www = LoadTexture(sPath);
            } else {
                sPath += cAB.path;
	            www = LoadAssetBundle(sPath);
            }
        } else {
		    while (!Caching.ready)
			    yield return 0;
            if (cAB.isImage) {
                sPath = getImagePath(sPath) + cAB.path;
	    		www = LoadTexture(sPath + "?time=" + KsSoftUtility.getUnixTime());
            } else {
                sPath += cAB.path;
	    		www = LoadAssetBundle(sPath + "?time=" + KsSoftUtility.getUnixTime(),cAB.version);
            }
        }
#endif
            cAB.www = www;
            yield return www.SendWebRequest();
            // 読み込み失敗.
            if (www.isNetworkError || www.isNetworkError) {
                faileLoadAssetBundle(cAB, www);
                yield return new WaitForSeconds(5);
                // 読みこみリトライ.
                if (cAB.isImage) {
                    www = LoadTexture(sPath);
                } else {
                    www = LoadAssetBundle(sPath, cAB.version);
                }
                cAB.www = www;
                yield return www.SendWebRequest();
                if (www.isNetworkError || www.isHttpError) {
                    error(-10, www.error);
                    yield break;
                }
            }
            // 読み込み成功.
            if (cAB.isImage) {
                cAB.texture = DownloadHandlerTexture.GetContent(www);
            } else {
                cAB.set(DownloadHandlerAssetBundle.GetContent(www));
            }
            cAB.state = CAssetBundle.e_State.LOADED;
            m_lstNowLoading.Remove(cAB);
            m_iNowLoadNum--;
#if OUTPUT_LOG
		Debug.Log("Load finished:" + sPath);
#endif
        }
        //==========================================================================
        /*!アセットバンドルの読み込み&完了チェック.
            @brief	checkToLoadAssetbundle.
            @retval	1	継続.
                    負	深刻なエラー.
                    0	終了.
        */
        public int checkToLoadAssetbundle(ref CAssetBundle cAB, uint mId) {
            if (cAB == null) {
                if (!isInitialized()) {
                    return 1;
                }
                cAB = reference(mId);
                if (cAB == null) {
                    Debug.LogError("can't find assetbundle:" + new MulId(mId));
                    return -2;
                }
            }
            if (!cAB.isLoaded) {
                // wait for load asset bundle
                if (cAB.state == CAssetBundle.e_State.NOEXIST) {
                    // occur error
                    Debug.LogError("not exist assetbundle:" + new MulId(mId));
                    return -3;
                }
                return 1;
            }
            return 0;
        }
        public int checkToLoadAssetbundle<Type>(ref CAssetBundle cAB, CResourceBase cRes) {
            if (cAB == null) {
                if (!isInitialized()) {
                    return 1;
                }
                cAB = reference(cRes.id);
                if (cAB == null) {
                    cRes.state = CResourceBase.e_State.NOEXIST;
                    return -2;
                }
            }
            if (!cAB.isLoaded) {
                // wait for load asset bundle
                if (cAB.state == CAssetBundle.e_State.NOEXIST) {
                    // occur error
                    cRes.state = CResourceBase.e_State.NOEXIST;
                    return -3;
                }
                return 1;
            }
            return 0;
        }
        //==========================================================================
        /*!faileLoadAssetBundle
            @brief	読み込み失敗.
        */
        void faileLoadAssetBundle(CAssetBundle cAB, UnityWebRequest www) {
            MulId mId = new MulId(cAB.id);
            if (string.IsNullOrEmpty(www.error)) {
                Debug.LogError(mId + "(assset bundle) is not exist");
            } else {
                Debug.LogError(mId + "(assset bundle) is not exist:" + www.error);
            }
            cAB.state = CAssetBundle.e_State.NOEXIST;
            m_lstNowLoading.Remove(cAB);
        }
        //==========================================================================
        /*!対象のリソースを解放する候補として登録する.
         * @brief	release
         * @note	後でまとめて解放される.
        */
        public void release(CResourceBase cResource) {
            m_lstReleaseResource.Add(cResource);
        }
        //==========================================================================
        /*!release
         * @brief	非常駐型アセットバンドルを解放する.
         * @note	オブジェクトの解放はシーン遷移に任せる.
        */
        public void release() {
            foreach (CAssetBundle cAB in m_dicPath.Values) {
                if (!cAB.isRemain) {
                    cAB.refcnt = 0;
                    cAB.release();
                }
            }
            releaseReservedResources();
        }
        //==========================================================================
        /*!releaseAll
         * @brief	アセットバンドルを全て解放する.
        */
        public void releaseAll() {
            foreach (CAssetBundle cAB in m_dicPath.Values) {
                cAB.refcnt = 0;
                cAB.release();
            }
            m_dicPath.Clear();
            releaseReservedResources();

        }
        //==========================================================================
        /*!onLevelWasLoaded
            @brief Unity call back
        */
        void onLevelWasLoaded(Scene scenename, LoadSceneMode mode) {
            release();
        }
        //==========================================================================
        /*!getServerPath
            @brief	サーバパスを取得する.
        */
        public static string getServerPath(bool UseStreaming) {
            if (UseStreaming) {
                return KsSoftConfig.PlatformPath(UseStreaming);
            }
            string sPath = m_sHttpServerPath;
            return sPath + KsSoftConfig.PlatformPath(UseStreaming);
        }
        //==========================================================================
        /*!どのHTTPサーバパスからアセットを落としてくるか.
            @brief	httpServerPath
        */
        static public string httpServerPath {
            get {
                return m_sHttpServerPath;
            }
            set {
                m_sHttpServerPath = value;
            }
        }
        //==========================================================================
        /*!アセットバンドルの読み込み
            @brief	GetAssetBundle
        */
        static UnityWebRequest LoadAssetBundle(string path) {
#if UNITY_2018_1_OR_NEWER
		return UnityWebRequestAssetBundle.GetAssetBundle(path);
#else
            return UnityWebRequest.GetAssetBundle(path);
#endif
        }
        static UnityWebRequest LoadAssetBundle(string path, uint version) {
#if UNITY_2018_1_OR_NEWER
		return UnityWebRequestAssetBundle.GetAssetBundle(path,version,0);
#else
            return UnityWebRequest.GetAssetBundle(path, version, 0);
#endif
        }
        //==========================================================================
        /*!テクスチャの読み込み
            @brief	GetTexture
        */
        UnityWebRequest LoadTexture(string path) {
#if UNITY_2017_1_OR_NEWER
            return UnityWebRequestTexture.GetTexture(path);
#else
		return UnityWebRequest.GetTexture(path);
#endif
        }
        //==========================================================================
        /*!image用のパスを生成する.
            @brief	一つ上のimage/というフォルダを指す.
        */
        static string getImagePath(string path) {
            string[] aDebugPath = path.Split(new char[] { '/', '\\' }, System.StringSplitOptions.RemoveEmptyEntries);
            aDebugPath[aDebugPath.Length - 1] = "image";
            return string.Join("/", aDebugPath) + "/";
        }
        //==========================================================================
        /*!WWWで読み込んだデータが正常化どうかチェックする.
            @brief	checkWWW
        */
        static public int checkWWW(byte[] bytes, string sHeader) {
            string sValue;
            try {
                sValue = CVariable.utf8Encode.GetString(bytes, 0, sHeader.Length);
            } catch (System.Text.DecoderFallbackException e) {
                Debug.Log(e);
                return -1;
            }
            if (sValue == sHeader) {
                return 0;
            }
            sValue = CVariable.utf8Encode.GetString(bytes);
            int index = sValue.IndexOf("<title>");
            int error;
            string sNum = sValue.Substring(index + 7, 3);
            Int32.TryParse(sNum, out error);
            Debug.LogError(error + ":" + sValue);
            return -error;
        }

        public CAssetVersion AssetVersion {
            get {
                return m_cVersion;
            }
            set {
                m_cVersion = value;
            }
        }
        public bool isPreloaded {
            get {
                return (m_iPreload == 2) ? true : false;
            }
        }
        public bool isStreaming {
            get {
                return m_isStreaming;
            }
            set {
                m_isStreaming = value;
            }
        }
        static protected CAssetBundleMgr m_instance;
        public static CAssetBundleMgr Instance {
            get {
                return m_instance;
            }
        }
    }
}