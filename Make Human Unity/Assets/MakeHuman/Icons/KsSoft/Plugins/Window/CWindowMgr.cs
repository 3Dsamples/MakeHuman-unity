//==============================================================================================
/*!?ウィンドウマネージャ.
	@file  CWindowMgr

	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
#if !ONLY_APPLICATION
using System.IO;
#endif
namespace KS {
    public class CWindowMgr : MonoBehaviour, IManager {
        public const float FlexibleBaseSize = -4096f;
        public class CCollision {
            public CWindowBase window = null;
            public CWinCtrlBase ctrl = null;
            public CRichTextOne richtext = null;
            public bool isTouchListbox = false;
            public CInput.e_State state = CInput.e_State.Off;

            public CCollision(CWinCtrlBase cCtrl) {
                ctrl = cCtrl;
                window = cCtrl.window;
            }
            public CCollision(CRichTextOne cRichTextOne) {
                richtext = cRichTextOne;
                ctrl = cRichTextOne.ctrl;
                window = ctrl.window;
            }
        };
        protected uint m_fontKind;
        protected Camera m_camera;
        protected CInput m_cInput;
        protected CStick m_cStick = null;
        // コリジョン情報.
        protected CCollision m_cCollision = null;
        protected CWindowBase m_winLastHit = null;

        // ドラッグ情報.
        protected enum e_DragState {
            Free,
            NotDrag,
            StandByDrag,
            Dragging,
        }
        protected e_DragState m_eDragState;
        protected CWindowBase m_cDragWindow;
        protected CWinCtrlBase m_cDragCtrl;
        protected Vector2 m_cDragOffset;
        protected CWindowRenderer m_cDragRenderer;
        protected CWindowMesh[] m_aDragMesh = new CWindowMesh[CWinCtrlBase.nTexture];
        protected CWinTextMesh m_cDragTextMesh;
        protected Vector2 m_vDragVelocity;
        protected Vector2 m_vOldTouchPos;
        // 標準SE_ID
        protected uint m_mClickSE;      //クリックしたときのSE
        protected uint m_mScrollSE; //スクロールしたときのSE
                                    // キーボード入力情報.
        protected System.Text.StringBuilder m_sb = new System.Text.StringBuilder();
#if UNITY_IOS || UNITY_ANDROID
	protected TouchScreenKeyboard	m_iKeyboard = null;
#endif
        protected IWinKeyFocusable m_keyFocusObj;                       // キーボードフォーカスを持つコントロール.
        protected string m_controlText;                     // キーボードフォーカスを持つコントロールのテキスト.
        protected int m_insert;                         // カーソル位置.
        protected int m_selection;                      // 範囲選択開始位置.
        t_KeyboardInfo m_kbInfo;                            // キーボード情報.
                                                            // 効果音.
        protected IWinSoundEffect m_iSoundEffect = null;                // Sound Effect Interface
        protected IWinCaptionData m_iCaptionData = null;                // Caption Data Interface
                                                                        // ウィンドウ情報.
        protected Dictionary<uint, CWindowBase> m_dicWindow = new Dictionary<uint, CWindowBase>();
        protected CWindowBase[] m_aWindow = new CWindowBase[0];
        // ウィンドウリソースデータ.
        protected Dictionary<uint, t_WindowData> m_dicWindowResource = new Dictionary<uint, t_WindowData>();
        protected const float m_fHoldTime = 0.5f;
        // 画面サイズ制御(実画面とウィンドウ解像度が異なっていても動くようにする)
        protected Vector4 m_area;
        protected Vector4 m_safeArea;
        protected Vector2 m_uiResolution;
        protected static float m_magScreen = 1f;
        protected static float m_invMagScreen = 1f;
        protected static Vector2 m_szScreen;
        protected static Vector2 m_size;
        //==========================================================================
        /*!IManager.getAssetBundleId
            @brief	getAssetBundleIds
        */
        public uint[] getAssetBundleIds() {
            return new uint[] { KsSoftConfig.WindowAssetbundleId };
        }
        //==========================================================================
        /*!IManager.Initialize
            @brief	initialize
        */
        public bool initialize(CAssetBundle[] aAB) {
            AssetBundle ab = aAB[0];
            UnityEngine.Object[] aObject = ab.LoadAllAssets();

            foreach (UnityEngine.Object o in aObject) {
                if (!(o is CSerializableScript)) {
                    continue;
                }
                CSerializableScript cSS = o as CSerializableScript;
                load(cSS.m_buffer);
            }
            return true;
        }
        //==========================================================================
        /*!読み込んだバイナリデータからウィンドウデータとして登録する.
            @brief	loading
        */
        public bool load(byte[] buffer) {
            CReadVariable cVariable = new CReadVariable(buffer);
            t_WindowDataHeader tWDH = new t_WindowDataHeader();
            if (!tWDH.read(cVariable)) {
                return false;
            }
            foreach (t_WindowData tWindow in tWDH.m_aWindowData) {
                m_dicWindowResource[tWindow.m_mId] = tWindow;
            }
            return true;
        }
        //==========================================================================
        /*!リソースからウィンドウデータを読み込む.
            @brief	load
        */
        public bool load(string assetname) {
            // リソースから読み込む.
            CSerializableScript cSS = Resources.Load(assetname) as CSerializableScript;
            if (cSS == null) {
                Debug.LogError("can't find resource:" + assetname + ".asset");
                return false;
            }
            return load(cSS.m_buffer);
        }
        //==========================================================================
        /*!Awake
            @brief	Unity Callback
        */
        protected void Awake() {
            SceneManager.sceneLoaded += onLevelWasLoaded;
            if (m_instance != null) {
                Debug.LogError("already instance exist");
            }
            m_instance = this;

            m_fontKind = KsSoftConfig.DefaultFontKind;

            m_camera = GetComponent<Camera>();
            GameObject goDrag = new GameObject("Drag Object");
            goDrag.layer = (int)e_LayerId.Window;
            m_cDragRenderer = new CWindowRenderer(goDrag.transform);
            GameObject.DontDestroyOnLoad(goDrag);
            clearDragInfo();

            m_szScreen.x = (float)Screen.width;
            m_szScreen.y = (float)Screen.height;
            m_size = m_szScreen;
        }
        //==========================================================================
        /*!Start
            @brief	Unity Callback
        */
        void Start() {
            m_cInput = CInput.Instance;
        }
        //==========================================================================
        /*!onLevelWasLoaded
            @brief Unity call back
        */
        void onLevelWasLoaded(Scene scenename, LoadSceneMode mode) {
            release();
        }
        //==========================================================================
        /*!OnDestroy
         * @brief	Unity Callback
        */
        void OnDestroy() {
            if (m_cDragRenderer.transform != null) {
                GameObject.Destroy(m_cDragRenderer.transform.gameObject);
            }
            m_instance = null;
        }
        //==========================================================================
        /*!update
            @brief	CMainSystemBaseから呼ばれる.
        */
        public void update(Camera uicamera) {
            // Poll the keyboard:
            if (focusObject != null) {
                pollKeyboard();
            }
            if (m_cInput == null) {
                return;
            }
            m_szScreen.x = Screen.width;
            m_szScreen.y = Screen.height;
            if (uicamera != null) {
                float aspect = m_szScreen.x / m_szScreen.y;
                uicamera.aspect = aspect;
                if (m_uiResolution.x != 0f) {
                    uicamera.orthographicSize = 0.5f * m_uiResolution.x / aspect;
                } else if (m_uiResolution.y != 0f) {
                    uicamera.orthographicSize = 0.5f * m_uiResolution.y;
                } else {
                    uicamera.orthographicSize = 0.5f * m_szScreen.y;
                }
                m_size.y = uicamera.orthographicSize * 2f;
                m_size.x = m_size.y * aspect;

                Rect orgSA = Screen.safeArea;
#if UNITY_IOS
			float kx = m_size.x/KsSoftUtility.defaultResolution.x;
			float ky = m_size.y/KsSoftUtility.defaultResolution.y;
#else
                float kx = m_size.x / m_szScreen.x;
                float ky = m_size.y / m_szScreen.y;
#endif
                Rect resizeSA = new Rect(orgSA.x * kx, orgSA.y * ky, orgSA.width * kx, orgSA.height * ky);
                KsSoftConfig.safeArea = resizeSA;
            }
            Rect rect;
#if UNITY_EDITOR
            rect = new Rect(16, 16, CWindowMgr.width - 32, CWindowMgr.height - 32);
#else
		rect = KsSoftConfig.safeArea;
#endif
            m_safeArea = new Vector4((float)rect.x, (float)rect.y, (float)rect.width, (float)rect.height);
            m_area = new Vector4(0f, 0f, CWindowMgr.width, CWindowMgr.height);
            //---------------------------------
            // update
            int priority = 16;
            List<CWindowBase> lstRemove = null;
            for (int i = m_aWindow.Length - 1; i >= 0; --i) {
                CWindowBase win = m_aWindow[i];

                win.update(this, priority);
                if (win.isClose && !win.isFade) {
                    if (lstRemove == null) {
                        lstRemove = new List<CWindowBase>();
                    }
                    lstRemove.Add(win);
                }
                priority += 128;
            }
            //---------------------------------
            // remove closed window.
            if (lstRemove != null) {
                foreach (CWindowBase win in lstRemove) {
                    removeWindow(win.uid);
                }
            }
            //---------------------------------
            // render
            for (int i = m_aWindow.Length - 1; i >= 0; --i) {
                CWindowBase win = m_aWindow[i];
                win.render();
            }
            if (isDragging) {
                renderDragObject();
            }
            //---------------------------------
            // proceed hit.
            m_cCollision = procCollision();

            //---------------------------------
            // proceed mouse.
            procMouse(m_cCollision);
        }
        //==========================================================================
        /*!全てのウィンドウを閉じる.
            @brief	release
        */
        public void release() {
            CWindowBase[] aWindow = new CWindowBase[m_dicWindow.Count];
            int i = 0;
            foreach (CWindowBase win in m_dicWindow.Values) {
                aWindow[i++] = win;
            }
            foreach (CWindowBase win in aWindow) {
                if (win != null) {
                    win.close();
                }
            }
            m_dicWindow.Clear();
            m_aWindow = new CWindowBase[0];
            m_winLastHit = null;
            m_cCollision = null;
        }

        //==========================================================================
        /*!ドラッグ中のオブジェクトをレンダリングする.
            @brief	renderDragObject
        */
        void renderDragObject() {
            CWinCtrlBase cCtrl = m_cDragCtrl;
            if (cCtrl == null) {
                if (m_cStick == null) {
                    clearDragInfo();
                }
                m_cDragWindow.position = m_cStick.windowPos + m_cDragOffset;
                return;
            }
            Vector3 pos = m_cStick.windowPos - cCtrl.absPosition + m_cDragOffset;
            pos.y = -pos.y;
            m_cDragRenderer.transform.position = pos;
            // メッシュのバックアップを取って、差し替える.
            if (!cCtrl.window.onDragRender(cCtrl, transform)) {
                return;
            }
            CWindowMesh[] aBackup = new CWindowMesh[CWinCtrlBase.nTexture];
            for (int i = 0; i < aBackup.Length; ++i) {
                CWindowMesh cMesh = cCtrl.getWindowMesh(i);
                aBackup[i] = cMesh;
                if (cMesh != null && cMesh.isLoaded) {
                    CWindowMesh cDragMesh = m_aDragMesh[i];
                    if (cDragMesh == null) {
                        cDragMesh = CWindowMesh.create((uint)i, null, m_cDragRenderer.transform, 0f);
                        cDragMesh.initialize(new Material(cMesh.material));
                        m_aDragMesh[i] = cDragMesh;
                    } else {
                        cDragMesh.material.CopyPropertiesFromMaterial(cMesh.material);
                    }
                    pos = cMesh.transform.localPosition;
                    pos.z -= 50f;
                    cDragMesh.transform.localPosition = pos;
                    cDragMesh.isLoaded = cMesh.isLoaded;
                    cCtrl.setWindowMesh(i, cDragMesh);
                } else {
                    cCtrl.setWindowMesh(i, null);
                }
            }
            // テキストメッシュのバックアップ
            CWinTextMesh cBackupTextMesh = cCtrl.textMesh;
            if (cBackupTextMesh != null) {
                if (m_cDragTextMesh == null) {
                    m_cDragTextMesh = CWinTextMesh.create(m_cDragRenderer.transform, -50.1f);
                    m_cDragTextMesh.initialize(new Material(cBackupTextMesh.material));
                } else {
                    m_cDragTextMesh.material.CopyPropertiesFromMaterial(cBackupTextMesh.material);
                }
                m_cDragTextMesh.setFont(cBackupTextMesh.spriteFont);
                cCtrl.textMesh = m_cDragTextMesh;
            }
            cCtrl.render(WinColor.white, null);
            // メッシュを差し戻す.
            for (int i = 0; i < aBackup.Length; ++i) {
                cCtrl.setWindowMesh(i, aBackup[i]);
            }
            cCtrl.textMesh = cBackupTextMesh;
        }
        //==========================================================================
        /*!コリジョンチェックする.
            @brief	procCollision
        */
        CCollision procCollision() {
            if (m_cStick != null && m_cStick.state == CInput.e_State.Off) {
                m_winLastHit = null;
                m_cStick.isUseWindow = false;
                m_cStick = null;
                return null;
            }
            CWindowBase topwin = topMostWindow;
            if (topwin != null && topwin.isFade) {
                return null;
            }
            // 前回の入力があったら優先する.
            int iPressNum = 0;
            int iReleaseNum = 0;
            if (m_cStick != null) {
                // ドラッグ速度を取得.
                Vector2 touchPos = m_cStick.windowPos;
                switch (m_cStick.state) {
                case CInput.e_State.OnMove:
                    m_vDragVelocity.x = touchPos.x - m_vOldTouchPos.x;
                    m_vDragVelocity.y = m_vOldTouchPos.y - touchPos.y;
                    break;
                case CInput.e_State.Press:
                    iPressNum++;
                    break;
                case CInput.e_State.Release:
                    goto case CInput.e_State.ReleaseMove;
                case CInput.e_State.ReleaseMove:
                    iReleaseNum = 0;
                    if (getDragCtrl() != null) {
                        // 当たりチェック.
                        for (int i = 0; i < m_aWindow.Length; ++i) {
                            CCollision cCollision = m_aWindow[i].checkHit(m_cStick.windowPos, m_cStick);
                            if (cCollision != null) {
                                m_vOldTouchPos = m_cStick.windowPos;
                                m_winLastHit = cCollision.window;
                                procHit(cCollision);
                                return cCollision;
                            }
                        }
                    }
                    break;
                }
                m_vOldTouchPos = touchPos;
                // 当たりチェック.
                if (m_cCollision != null) {
                    procHit(m_cCollision);
                    return m_cCollision;
                }
            } else {
                // 新しくヒットしている入力を探す(Pressのみ)
                CStick[] aStick = m_cInput.sticks;
                for (int j = 0; j < aStick.Length; ++j) {
                    CStick stk = aStick[j];
                    if (stk.state == CInput.e_State.Off) {
                        continue;
                    }
                    iPressNum++;
                    // 当たりチェック.
                    for (int i = 0; i < m_aWindow.Length; ++i) {
                        CCollision cCollision = m_aWindow[i].checkHit(stk.windowPos, stk);
                        if (cCollision != null) {
                            m_vOldTouchPos = stk.windowPos;
                            m_cStick = stk;
                            if (m_cStick.state == CInput.e_State.Press) {
                                m_cStick.isUseWindow = true;
                            }
                            m_winLastHit = cCollision.window;
                            if (!closePopup()) {
                                procHit(cCollision);
                                return cCollision;
                            }
                            m_winLastHit = null;
                            return null;
                        }
                    }
                }
            }
            if (iPressNum == 1 && closePopup()) {
                m_winLastHit = null;
            }
            if (iReleaseNum == 1 && focusObject != null) {
                focusObject = null;
            }
            return null;
        }
        //==========================================================================
        /*!ポップアップウィンドウを必要があれば閉じる.
            @brief	closePopup
        */
        bool closePopup() {
            // フォーカスオブジェクトが設定されているときは入力中.
            if (focusObject != null) {
                return false;
            }
            // トップモーストウィンドウがPOPUPかどうか調べ、自動で閉じる.
            for (int i = 0; i < m_aWindow.Length; ++i) {
                CWindowBase win = m_aWindow[i];
                if (win == null) {
                    return false;
                }
                switch (win.priorityStyle) {
                case e_WinStyle.TOP:
                    if ((win.style & e_WinStyle.NOECLIPSE) == 0) {
                        return false;
                    }
                    break;
                case e_WinStyle.POPUP:
                    if (m_winLastHit != win) {
                        if (!win.isFade) {
                            win.close();
                            return true;
                        }
                    }
                    return false;
                case e_WinStyle.TOPMOST:
                    if (m_winLastHit != win) {
                        return true;
                    }
                    return false;
                case e_WinStyle.NORMAL:
                    return false;
                default:
                    break;
                }
            }
            return false;
        }
        //==========================================================================
        /*!ヒット処理を行う.
            @brief	procHit
        */
        void procHit(CCollision cCollision) {
            if (cCollision == null || m_cStick == null) {
                return;
            }
            cCollision.state = m_cStick.state;
            CWindowBase cWindow = cCollision.window;
            if (!cWindow.procHitCommon(this, cCollision.ctrl)) {
                return;
            }
            CWinCtrlBase cCtrl = cCollision.ctrl;
            if (m_cStick.state == CInput.e_State.Press) {
                if (cCtrl != null && cCtrl.isDragable) {
                    setDragInfo(true, cWindow, cCtrl, m_cStick.windowPos);
                } else if (cCtrl != null && cCtrl is CWinCtrlScrollable) {
                    clearDragInfo();
                } else if (cWindow.isDragable) {
                    setDragInfo(true, cWindow, null, m_cStick.windowPos);
                } else {
                    clearDragInfo();
                }
                bringToTop(cWindow);
                focusObject = null;
            } else if (m_cStick.state == CInput.e_State.Release) {
                IWinKeyFocusable fo = cCtrl as IWinKeyFocusable;
                if (fo != null) {
                    focusObject = fo;
                } else {
                    focusObject = null;
                }
            }
            if (cCollision.richtext != null) {
                cCollision.richtext.procHit(this, m_cStick.state);
            } else if (cCollision.ctrl != null) {
                cCollision.ctrl.procHit(this, m_cStick.state);
            }
        }
        //==========================================================================
        /*!マウス処理を行う.
            @brief	procMouse
        */
        void procMouse(CCollision cCollision) {
            if (isDragging) {
                if (m_cStick == null || m_cStick.state == CInput.e_State.ReleaseMove || m_cStick.state == CInput.e_State.Release) {
                    procDrop(cCollision);
                    clearDragInfo();
                } else if (m_cDragCtrl != null) {
                    m_cDragCtrl.window.onDrag(m_cDragCtrl, cCollision.window.getLocalPos(stick.windowPos), dragVelocity);
                }
            } else {
                if (m_cStick != null && (m_eDragState == e_DragState.StandByDrag && m_cStick.state == CInput.e_State.OnMove)) {
                    if (cCollision == null || cCollision.window == null) {
                        clearDragInfo();
                    } else if (cCollision.ctrl == null) {
                        // window dragging
                        m_eDragState = e_DragState.Dragging;
                    } else if (cCollision.ctrl.queryDrag(this)) {
                        m_eDragState = e_DragState.Dragging;
                        cCollision.window.onBeginDrag(cCollision.ctrl, cCollision.window.getLocalPos(stick.windowPos));
                    }
                }
            }
        }
        //==========================================================================
        /*!ドラッグ中に離した時ドロップイベントを発生させる.
            @brief	procDrop
        */
        void procDrop(CCollision cCollision) {
            if (m_cDragWindow == null) {
                return;
            }
            if (m_cDragCtrl == null) {
                return;
            }
            if (cCollision != null && cCollision.ctrl != null) {
                cCollision.window.onDrop(cCollision.ctrl, m_cDragWindow, m_cDragCtrl);
            } else {
                m_cDragWindow.onDropGround(m_cDragCtrl);
            }
        }
        //==========================================================================
        /*!
            @brief	isDragCtrl
        */
        public bool isDragCtrl(CWinCtrlBase cCtrl) {
            if (cCtrl == null) return false;
            if (cCtrl.window != m_cDragWindow) {
                return false;
            }
            if (cCtrl != m_cDragCtrl) {
                return false;
            }
            return true;
        }
        //==========================================================================
        /*!
            @brief	getDragCtrl
        */
        public CWinCtrlBase getDragCtrl() {
            if (m_cDragWindow == null) {
                return null;
            }
            if (m_cDragCtrl == null) {
                return null;
            }
            if (!m_cDragCtrl.queryDrag(this)) {
                return null;
            }
            return m_cDragCtrl;
        }
        //==========================================================================
        /*!create
            @brief	create
            @note   ウィンドウID = uidとしてウィンドウを開く(一枚だけ開くウィンドウではこれでOK)
        */
        public virtual WindowScript create<WindowScript>(uint id, CWindowBase cParent = null) where WindowScript : CWindowBase {
            t_WindowData tData;
            if (!m_dicWindowResource.TryGetValue(id, out tData)) {
                Debug.LogError("can't open window resource:" + new MulId(id));
                return null;
            }
            return create<WindowScript>(tData.m_mId, tData, cParent);
        }
        //==========================================================================
        /*!create
            @brief	create
            @note   一種類のウィンドウを複数開くにはこちらを使う(uidは重複しないように気を付ける)
        */
        public virtual WindowScript create<WindowScript>(uint uid, uint winid, CWindowBase cParent = null) where WindowScript : CWindowBase {
            t_WindowData tData;
            if (!m_dicWindowResource.TryGetValue(winid, out tData)) {
                Debug.LogError("can't open window resource:" + new MulId(winid));
                return null;
            }
            return create<WindowScript>(uid, tData, cParent);
        }
        public WindowScript create<WindowScript>(t_WindowData tData, CWindowBase cParent = null) where WindowScript : CWindowBase {
            return create<WindowScript>(tData.m_mId, tData, cParent);
        }
        public WindowScript create<WindowScript>(uint uid, t_WindowData tData, CWindowBase cParent = null) where WindowScript : CWindowBase {
            WindowScript cWin = null;
            if (m_dicWindow.ContainsKey(uid)) {
                cWin = m_dicWindow[uid] as WindowScript;
            }
            // 既に存在しているかチェック.
            if (cWin != null) {
                if (cWin.isClose) {
                    // 閉じているときは強制終了.
                    removeWindow(uid);
                } else {
                    Debug.Log("already opened window:" + new MulId(uid));
                    return cWin;
                }
            }
            GameObject go = new GameObject("window:" + new MulId(uid));
            go.layer = (int)e_LayerId.Window;
            WindowScript win = go.AddComponent<WindowScript>();
            //ウィンドウを登録する.
            m_dicWindow[uid] = win;
            // ウィンドウ初期化.
            win.initialize(uid, tData, cParent);

            // 描画順番を制御する.
            Array.Resize(ref m_aWindow, m_aWindow.Length + 1);
            if ((win.style & e_WinStyle.OPENBOTTOM) != 0) {
                m_aWindow[m_aWindow.Length - 1] = win;
            } else {
                for (int i = m_aWindow.Length - 1; i >= 1; --i) {
                    m_aWindow[i] = m_aWindow[i - 1];
                }
                m_aWindow[0] = win;
            }
            //プライオリティを考慮に入れて前面に出す.
            bringToTop(win);
            return win;
        }
        //==========================================================================
        /*!find
            @brief	find
        */
        public WindowScript find<WindowScript>(uint id) where WindowScript : CWindowBase {
            CWindowBase cWindow;
            if (!m_dicWindow.TryGetValue(id, out cWindow)) {
                //			Debug.LogError("can't find Window:" + new MulId(id));
                return null;
            }
            if (cWindow.isClose) {
                return null;
            }
            return cWindow as WindowScript;
        }
        public CWindowBase find(uint id) {
            CWindowBase cWindow;
            if (!m_dicWindow.TryGetValue(id, out cWindow)) {
                //			Debug.LogError("can't find Window:" + new MulId(id));
                return null;
            }
            if (cWindow.isClose) {
                return null;
            }
            return cWindow;
        }
        //==========================================================================
        /*!addWindow
            @brief	addWindow
        */
        protected bool addWindow(CWindowBase cWindow) {
            return true;
        }
        //==========================================================================
        /*!特定のレイヤに配置されているウィンドウを強制的に閉じる.
            @brief	closeWindow
        */
        public void closeWindow(e_Layer eLayer) {
            List<CWindowBase> lstClose = new List<CWindowBase>();
            for (int i = 0; i < m_aWindow.Length; ++i) {
                CWindowBase cWin = m_aWindow[i];
                if (((1 << (int)cWin.layer) & (int)eLayer) != 0) {
                    cWin.close();
                    lstClose.Add(cWin);
                }
            }
            foreach (CWindowBase cWin in lstClose) {
                removeWindow(cWin.uid);
            }
        }

        //==========================================================================
        /*!removeWindow
            @brief	removeWindow
        */
        protected bool removeWindow(uint id) {
            CWindowBase cWindow;
            if (!m_dicWindow.TryGetValue(id, out cWindow)) {
                Debug.LogError("can't remove Window:" + new MulId(id));
                return false;
            }
            if (!cWindow.isClose) {
                cWindow.close();
            }
            if (m_winLastHit == cWindow) {
                m_winLastHit = null;
            }
            m_dicWindow.Remove(id);
            List<CWindowBase> lstWindow = new List<CWindowBase>(m_aWindow);
            lstWindow.Remove(cWindow);
            m_aWindow = lstWindow.ToArray();
            cWindow.release();
            GameObject.DestroyImmediate(cWindow.gameObject, true);
            Resources.UnloadUnusedAssets();
            CMainSystemBase.Instance.collectGarbage();
            return true;
        }
        //==========================================================================
        /*!前面に出す(プライオリティタイプに影響を受けるので必ずしも前面に来ない).
            @brief	bringToTop()
        */
        public void bringToTop(CWindowBase cWindow) {
            if ((cWindow.style & e_WinStyle.NOBRINGTOTOP) != 0) {
                return;
            }
            if (m_aWindow.Length <= 1) {
                return;
            }
            int iIndex = -1;
            for (int i = 0; i < m_aWindow.Length; ++i) {
                if (m_aWindow[i] == cWindow) {
                    iIndex = i;
                    break;
                }
            }
            if (iIndex < 0) {
                return;
            }
            // 挿入する場所を探しながら詰める.
            int iInsert = iIndex + 1;
            e_WinStyle ePriority = cWindow.priorityStyle;
            for (; iInsert < m_aWindow.Length; ++iInsert) {
                if (ePriority != e_WinStyle.NORMAL) {
                    if (m_aWindow[iInsert].priorityStyle == e_WinStyle.NORMAL) {
                        break;
                    }
                    if (m_aWindow[iInsert].priority <= cWindow.priority) {
                        break;
                    }
                } else if (m_aWindow[iInsert].priorityStyle == e_WinStyle.NORMAL) {
                    break;
                }
                m_aWindow[iInsert - 1] = m_aWindow[iInsert];
            }
            // 挿入.
            m_aWindow[iInsert - 1] = cWindow;
        }
        //==========================================================================
        /*!ドラッグ状態を初期化する.
            @brief	clearDragInfo
        */
        public void clearDragInfo() {
            m_eDragState = e_DragState.Free;
            m_cDragWindow = null;
            m_cDragCtrl = null;
            m_cDragRenderer.clear();
        }
        //==========================================================================
        /*!ドラッグ状態を設定する.
            @brief setDragInfo
        */
        public void setDragInfo(bool bDrag, CWindowBase cWindow, CWinCtrlBase cCtrl, Vector2 touchPos) {
            m_cDragWindow = cWindow;
            m_cDragCtrl = cCtrl;
            if (cWindow != null) {
                if (cCtrl != null) {
                    //				m_cDragOffset = cCtrl.screenPosition - touchPos;
                    m_cDragOffset.x = -cCtrl.width * 0.5f;
                    m_cDragOffset.y = -cCtrl.height * 0.5f;
                } else {
                    m_cDragOffset = cWindow.position - touchPos;
                }
            } else {
                m_cDragOffset = Vector2.zero;
            }
            if (bDrag) {
                m_eDragState = e_DragState.StandByDrag;
                m_cStick.isUseListbox = false;
            } else {
                m_eDragState = e_DragState.NotDrag;
            }
        }
        //==========================================================================
        /*!キーボード処理を行う.
            @brief pollKeyboard
        */
        protected void pollKeyboard() {
#if UNITY_IOS || UNITY_ANDROID
		if(!Application.isEditor) {
			if(m_iKeyboard == null)
				return;

			if(!m_iKeyboard.active) {
				if (m_iKeyboard.done && !m_iKeyboard.wasCanceled) {
					m_controlText = m_iKeyboard.text;
					m_selection = -1;
					m_controlText = focusObject.setInputText(m_controlText, ref m_insert,ref m_selection);
					focusObject.commit();
				}
				focusObject = null;
				return;
			} else if(m_controlText == m_iKeyboard.text)
				return; // Nothing to do

			string oldText = m_controlText;

			m_controlText = m_iKeyboard.text;

			// Find the insertion point:
			m_insert = findInsertionPoint(oldText, m_controlText);
			m_selection = -1;
			focusObject.setInputText(m_controlText, ref m_insert,ref m_selection);
		} else
			processKeyboard();
#else
            processKeyboard();

            // Look for arrow keys:
            bool isShift = Input.GetKey(KeyCode.LeftShift) | Input.GetKey(KeyCode.LeftShift);
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
		bool	isCtrl = Input.GetKey(KeyCode.LeftCommand) | Input.GetKey(KeyCode.RightCommand);
#else
            bool isCtrl = Input.GetKey(KeyCode.LeftControl) | Input.GetKey(KeyCode.RightControl);
#endif

#if UNITY_EDITOR
            if (isShift && isCtrl)
#else
		if (isCtrl)
#endif
        {
                if (Input.GetKeyDown(KeyCode.V)) {
                    string sInsert = ClipboardHelper.clipBoard;
                    if (!string.IsNullOrEmpty(sInsert)) {
                        insertString(sInsert);
                    }
                } else if (
#if UNITY_EDITOR
                Input.GetKeyDown(KeyCode.Z)
#else
				Input.GetKeyDown (KeyCode.C)
#endif
            ) {
                    copySelection();
                } else if (Input.GetKeyDown(KeyCode.X)) {
                    copySelection();
                    insertString("");
                }
            } else {
                if (Input.GetKeyDown(KeyCode.RightArrow)) {
                    m_controlText = focusObject.Content;
                    updateSelection(isShift);
                    m_insert = Mathf.Min(m_controlText.Length, m_insert + 1);
                    focusObject.setInputText(m_controlText, ref m_insert, ref m_selection);
                } else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                    m_controlText = focusObject.Content;
                    updateSelection(isShift);
                    m_insert = Mathf.Max(0, m_insert - 1);
                    focusObject.setInputText(m_controlText, ref m_insert, ref m_selection);
                } else if (Input.GetKeyDown(KeyCode.Home)) {
                    m_controlText = focusObject.Content;
                    updateSelection(isShift);
                    m_insert = 0;
                    focusObject.setInputText(m_controlText, ref m_insert, ref m_selection);
                } else if (Input.GetKeyDown(KeyCode.End)) {
                    m_controlText = focusObject.Content;
                    updateSelection(isShift);
                    m_insert = m_controlText.Length;
                    focusObject.setInputText(m_controlText, ref m_insert, ref m_selection);
                }
            }

            if (Input.GetKeyDown(KeyCode.UpArrow)) {
                updateSelection(isShift);
                focusObject.goUp();
                focusObject.setInputText(m_controlText, ref m_insert, ref m_selection);
            } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
                updateSelection(isShift);
                focusObject.goDown();
                focusObject.setInputText(m_controlText, ref m_insert, ref m_selection);
            }
#endif
        }
        protected void updateSelection(bool isShift) {
            if (isShift) {
                if (m_selection == -1) {
                    m_selection = m_insert;
                }
            } else {
                m_selection = -1;
            }
        }
        protected void processKeyboard() {
            if (Input.inputString.Length == 0 && !Input.GetKeyDown(KeyCode.Delete))
                return;

            // Start with the actual content of the focus object:
            m_controlText = focusObject.Content;
            m_insert = Mathf.Clamp(m_insert, 0, m_controlText.Length);

            insertString(Input.inputString);
        }
        protected void insertString(string str) {
            m_sb.Length = 0;
            m_sb.Append(m_controlText);

            bool isSelection = false;
            if (m_selection >= 0 && m_selection != m_insert) {
                if (m_insert > m_selection) {
                    int tmp = m_insert;
                    m_insert = m_selection;
                    m_selection = tmp;
                }
                m_sb.Remove(m_insert, m_selection - m_insert);
                isSelection = true;
            }

            foreach (char c in str) {
                // Backspace
                switch (c) {
                case '\b':
                    if (!isSelection) {
                        m_insert = Mathf.Max(0, m_insert - 1);
                        if (m_insert < m_sb.Length) {
                            m_sb.Remove(m_insert, 1);
                        }
                    }
                    continue;
                case '\r':
                    break;
                case '\n':
                    break;
                default:
                    if (!focusObject.validCharCode(c)) {
                        // 無効な文字コードは無視する.
                        continue;
                    }
                    break;
                }
                m_sb.Insert(m_insert, c);
                ++m_insert;
            }
            if (!isSelection) {
                if (Input.GetKeyDown(KeyCode.Delete)) {
                    if (m_insert < m_sb.Length)
                        m_sb.Remove(m_insert, 1);
                }
            }

            m_controlText = m_sb.ToString();
            m_selection = -1;
            m_controlText = focusObject.setInputText(m_controlText, ref m_insert, ref m_selection);
        }
        protected void copySelection() {
            if (m_selection < 0 || m_selection == m_insert) {
                return;
            }
            int iStart = m_selection;
            int iEnd = m_insert;
            if (iStart > iEnd) {
                iStart = m_insert;
                iEnd = m_selection;
            }
            ClipboardHelper.clipBoard = m_controlText.Substring(iStart, iEnd - iStart);
        }
        public IWinKeyFocusable focusObject {
            get {
                if (m_keyFocusObj != null && !m_keyFocusObj.editable) {
                    focusObject = null;
                }
                return m_keyFocusObj;
            }
            set {
                // See if another object is losing focus:
                if (value != null && !value.editable) {
                    value = null;
                }
                IWinKeyFocusable oldFocus = m_keyFocusObj;
                m_keyFocusObj = value;
                if (oldFocus != null)
                    oldFocus.lostFocus();

                if (m_keyFocusObj != null) {
                    m_controlText = m_keyFocusObj.getInputText(ref m_kbInfo);
                    if (m_controlText == null)
                        m_controlText = ""; // To be safe

#if UNITY_IOS || UNITY_ANDROID
				if(!Application.isEditor) {
					TouchScreenKeyboard.hideInput = m_kbInfo.m_hideInput;
					m_iKeyboard = TouchScreenKeyboard.Open(m_controlText, m_kbInfo.m_type, m_kbInfo.m_autoCorrect, m_kbInfo.m_multiline, m_kbInfo.m_secure, m_kbInfo.m_alert, m_controlText);
				}
#endif

                    m_insert = m_kbInfo.m_insert;

                    if (m_sb.Length > 0)
                        m_sb.Replace(m_sb.ToString(), m_controlText);
                    else
                        m_sb.Append(m_controlText);
#if !UNITY_IOS && !UNITY_ANDROID
                    Input.imeCompositionMode = IMECompositionMode.On;
#endif
                } else {
#if UNITY_IOS || UNITY_ANDROID
				if(m_iKeyboard != null) {
					m_iKeyboard.active = false;
					m_iKeyboard = null;
				}
#else
                    Input.imeCompositionMode = IMECompositionMode.Off;
#endif
                }
            }
        }
        //==========================================================================
        /*!テキストの挿入位置を返す.
            @brief insertionPoint
        */
        public int insertionPoint {
            get { return m_insert; }
            set { m_insert = value; }
        }
        //==========================================================================
        /*!テキスト選択範囲を返す.
            @brief selectionPoint
        */
        public int selectionPoint {
            get { return m_selection; }
            set { m_selection = value; }
        }
        //==========================================================================
        /*!テキストの挿入位置を探す.
            @brief findInsertionPoint
        */
        protected static int findInsertionPoint(string before, string after) {
            if (before == null || after == null)
                return 0;

            for (int i = 0; i < before.Length && i < after.Length; ++i)
                if (before[i] != after[i])
                    return i + 1;

            // If we got this far, either the two are identical,
            // or one is longer/shorter than the other.
            // In which case, the insertion point should be
            // at the end of "after":
            return after.Length;
        }
        //==========================================================================
        /*!キャプションデータを取得する.
            @brief	getCaption
        */
        virtual public string getCaption(uint mCaptionId) {
            if (mCaptionId == 0) {
                return "";
            }
            if (m_iCaptionData == null) {
                return "null";
            }
            string val = m_iCaptionData.find(mCaptionId);
            if (val == null) {
                return "null";
            }
            return val;

        }
        //==========================================================================
        /*!SEを鳴らす.
            @brief	play
        */
        virtual public void play(uint mSE) {
            if (mSE == 0) {
                return;
            }
            if (m_iSoundEffect == null) {
                return;
            }
            m_iSoundEffect.play(mSE);

        }
        //==========================================================================
        /*!特定のウィンドウが外的要因によりDisableかどうか判定する.
            @brief	isDisable
        */
        public bool isDisable(CWindowBase cWindow) {
            for (int i = 0; i < m_aWindow.Length; ++i) {
                CWindowBase win = m_aWindow[i];
                if (cWindow == win) {
                    return false;
                }
                if (win.priorityStyle == e_WinStyle.NORMAL) {
                    return false;
                }
                if ((win.style & e_WinStyle.NOECLIPSE) == 0) {
                    return true;
                }
            }
            return false;
        }
        //==========================================================================
        /*!UIの解像度を設定する.
            @brief setUIResolution
                width,heightが0の時は、
        */
        public void setUIResolution(float width, float height) {
            m_uiResolution.x = width;
            m_uiResolution.y = height;
        }

        public CInput input {
            get {
                return m_cInput;
            }
        }
        public CStick stick {
            get {
                return m_cStick;
            }
        }
        public Camera Camera {
            get {
                return m_camera;
            }
        }
        public bool isDragging {
            get {
                return (m_eDragState == e_DragState.Dragging);
            }
        }
        public Vector2 dragVelocity {
            get {
                return m_vDragVelocity / Time.deltaTime;
            }
        }
        public CWindowBase topWindow {
            get {
                if (m_aWindow.Length == 0) {
                    return null;
                }
                return m_aWindow[0];
            }
        }
        public CWindowBase topMostWindow {
            get {
                if (m_aWindow.Length == 0) {
                    return null;
                }
                CWindowBase win = m_aWindow[0];
                if (win.priorityStyle != e_WinStyle.NORMAL) {
                    return win;
                }
                return null;
            }
        }
        public Vector4 area {
            get {
                return m_area;
            }
        }
        public Vector4 safeArea {
            get {
                return m_safeArea;
            }
        }
        public static float screenWidth {
            get {
                return m_szScreen.x;
            }
        }
        public static float screenHeight {
            get {
                return m_szScreen.y;
            }
        }
        public static float width {
            get {
                return m_size.x;
            }
        }
        public static float height {
            get {
                return m_size.y;
            }
        }
        public uint fontKind {
            get {
                return m_fontKind;
            }
            set {
                m_fontKind = value;
            }
        }
        public CWindowBase dragWindow {
            get {
                return m_cDragWindow;
            }
        }
        public CWinCtrlBase dragCtrl {
            get {
                return m_cDragCtrl;
            }
        }
        public CCollision collision {
            get {
                return m_cCollision;
            }
        }
        public float holdTime {
            get {
                return m_fHoldTime;
            }
        }
        // 効果音関連.
        public IWinCaptionData captiondata {
            get {
                return m_iCaptionData;
            }
            set {
                m_iCaptionData = value;
            }
        }
        // 効果音関連.
        public IWinSoundEffect soundeffect {
            get {
                return m_iSoundEffect;
            }
            set {
                m_iSoundEffect = value;
            }
        }
        public uint clickSE {
            get {
                return m_mClickSE;      //クリックしたときのSE.
            }
            set {
                m_mClickSE = value;
            }
        }
        public uint scrollSE {
            get {
                return m_mScrollSE; //スクロールしたときのSE.
            }
            set {
                m_mScrollSE = value;
            }
        }
        // インスタンス取得.
        static protected CWindowMgr m_instance;
        public static CWindowMgr Instance {
            get {
                return m_instance;
            }
        }
    }
}
