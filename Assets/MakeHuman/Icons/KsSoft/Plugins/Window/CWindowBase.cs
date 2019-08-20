//==============================================================================================
/*!?ウィンドウベース.
	@file  CWindowBase

	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace KS {
    public class CWindowBase : MonoBehaviour, IMessageBox {
        [Flags]
        protected enum e_UpdateFlag {
            STYLE = 1 << 0,
            SIZE = 1 << 2,
            TEXID = 1 << 4,
            FRAME = 1 << 5,
            RESETPARTS = 1 << 6,
            ALL = STYLE | SIZE | TEXID | FRAME | RESETPARTS,
        };
        protected enum e_FadeState {
            None,
            Open,
            Close,
        };
        protected Dictionary<uint, CSpriteBase> m_dicSprite = new Dictionary<uint, CSpriteBase>();
        protected uint m_id;   //ウィンドウID
        protected uint m_uid;  //ユニークなウィンドウID(通常は、m_idと同じ)
        protected uint m_uniqueId = 1;
        protected int m_priority = 0;
        protected e_WinStyle m_eStyle;
        protected Transform m_transform;
        protected CWindowBase m_cChild = null;
        protected CWindowBase m_cParent = null;
        protected CWinContents m_cContents = new CWinContents((CWinCtrlBase)null);
        protected CWindowRenderer m_cWindowRenderer = null;
        protected e_Anchor m_eAnchor = e_Anchor.Default;
        protected e_UpdateFlag m_eUpdateFlag = e_UpdateFlag.ALL;
        protected RatioPair m_position;
        protected RatioPair m_screenOrg;
        protected RatioPair m_screenSize;
        protected Vector2 m_absPosition;
        protected bool m_isSafeArea;
        protected Vector4 m_area;           //レンダリングエリア(セーフエリアを考慮にいれたもの:x,y,w,h).
        protected RatioPair m_size;
        protected uint m_fontKind;
        protected string m_caption;
        protected RatioPair m_captionOffset;
        protected WinColor m_captionColor = WinColor.white;
        protected WinColor[] m_aColor = new WinColor[4] {
        WinColor.white,
        WinColor.white,
        WinColor.white,
        WinColor.white,
    };
        // テクスチャ.
        protected struct t_Texture {
            public uint texId;
            public uint partId;
            public RatioPair offset;
        }
        protected t_Texture[] m_aTexture = new t_Texture[4];

        // システムコントロール.
        protected CWinCtrlBar m_cCtrlBar = null;
        protected CWinCtrlCloseButton m_cCtrlCloseButton = null;
        protected CWinCtrlFrame m_cCtrlFrame = null;

        // 状態変更.
        protected bool m_bClose = false;        //ウィンドウを閉じた.
                                                // フェードパラメータ:アルファアニメーション.
        protected int m_angFade = 0;
        protected e_FadeState m_eFadeState = e_FadeState.None;
        protected bool m_bFadeLoaded = false;
        protected WinColor m_colorFadeS;
        protected WinColor m_colorFadeE;
        //フェードパラメータ:移動アニメーション.
        protected RatioPair m_posClose;
        protected Vector2 m_posFadeS;
        protected Vector2 m_posFadeE;
        //フェードパラメータ:スケールアニメーション.
        protected Vector2 m_scaleClose = new Vector2(1f, 1f);
        protected Vector2 m_scaleFadeS;
        protected Vector2 m_scaleFadeE;
        protected Vector2 m_offsetClose = new Vector2(0f, 0f);  //消失点オフセット.

        protected bool m_bCloseAnimation = false;
        protected float m_fadeSpeed = 0.3f;

        // Z値情報.
        // テクスチャオフセット.
        protected Dictionary<uint, float> m_dicTextureZOffset = new Dictionary<uint, float>();
        protected const byte m_disableColor = 48;

        class CPropertyTable {
            public CPropertyTable(uint chunk, e_WinProperty eProperty, DlSetter setter) {
                m_chunk = chunk;
                m_eWinProperty = eProperty;
                m_dlSetter = setter;
            }
            public delegate void DlSetter(CWindowBase cWindow, CReadVariable cVariable);
            public uint m_chunk;
            public e_WinProperty m_eWinProperty;
            public DlSetter m_dlSetter;
        };
        static CPropertyTable[] m_aPropertyTable = new CPropertyTable[] {
        new CPropertyTable(WinPropertyChunk.ID,             e_WinProperty.ID,               setId),
        new CPropertyTable(WinPropertyChunk.CAPTION,        e_WinProperty.CAPTION,          setCaption),
        new CPropertyTable(WinPropertyChunk.CAPTION_STR,    e_WinProperty.CAPTION_STR,      setCaptionStr),
        new CPropertyTable(WinPropertyChunk.CAPTION_OFFSET, e_WinProperty.CAPTION_OFFSET,   setCaptionOffset),
        new CPropertyTable(WinPropertyChunk.CAPTION_COLOR,  e_WinProperty.CAPTION_COLOR,    setCaptionColor),
        new CPropertyTable(WinPropertyChunk.POSITION,       e_WinProperty.POSITION,         setPosition),
        new CPropertyTable(WinPropertyChunk.SCREEN,         e_WinProperty.SCREEN,           setScreen),
        new CPropertyTable(WinPropertyChunk.SAFEAREA,       e_WinProperty.SAFEAREA,         setSafeArea),
        new CPropertyTable(WinPropertyChunk.POSITION,       e_WinProperty.POSITION,         setPosition),
        new CPropertyTable(WinPropertyChunk.CLOSE_POSITION, e_WinProperty.CLOSE_POSITION,   setClosePosition),
        new CPropertyTable(WinPropertyChunk.CLOSE_SCALE,    e_WinProperty.CLOSE_SCALE,      setCloseScale),
        new CPropertyTable(WinPropertyChunk.SIZE,           e_WinProperty.SIZE,             setSize),
        new CPropertyTable(WinPropertyChunk.STYLE,          e_WinProperty.STYLE,            setStyle),
        new CPropertyTable(WinPropertyChunk.COLOR0,         e_WinProperty.COLOR0,           setColor0),
        new CPropertyTable(WinPropertyChunk.COLOR1,         e_WinProperty.COLOR1,           setColor1),
        new CPropertyTable(WinPropertyChunk.COLOR2,         e_WinProperty.COLOR2,           setColor2),
        new CPropertyTable(WinPropertyChunk.COLOR3,         e_WinProperty.COLOR3,           setColor3),
        new CPropertyTable(WinPropertyChunk.TEX_ID0,        e_WinProperty.TEX_ID0,          setTexId0),
        new CPropertyTable(WinPropertyChunk.TEX_ID1,        e_WinProperty.TEX_ID1,          setTexId1),
        new CPropertyTable(WinPropertyChunk.TEX_ID2,        e_WinProperty.TEX_ID2,          setTexId2),
        new CPropertyTable(WinPropertyChunk.TEX_ID3,        e_WinProperty.TEX_ID3,          setTexId3),
        new CPropertyTable(WinPropertyChunk.TEXTURE_OFFSET0,e_WinProperty.TEXTURE_OFFSET0,  setTextureOffset0),
        new CPropertyTable(WinPropertyChunk.TEXTURE_OFFSET1,e_WinProperty.TEXTURE_OFFSET1,  setTextureOffset1),
        new CPropertyTable(WinPropertyChunk.TEXTURE_OFFSET2,e_WinProperty.TEXTURE_OFFSET2,  setTextureOffset2),
        new CPropertyTable(WinPropertyChunk.TEXTURE_OFFSET3,e_WinProperty.TEXTURE_OFFSET3,  setTextureOffset3),
        new CPropertyTable(WinPropertyChunk.RELATION_ID,    e_WinProperty.RELATION_ID,      setRelationId),
        new CPropertyTable(WinPropertyChunk.HELP_ID,        e_WinProperty.HELP_ID,          setHelpId),
        new CPropertyTable(WinPropertyChunk.TOOLTIP,        e_WinProperty.TOOLTIP,          setTooltip),
        new CPropertyTable(WinPropertyChunk.FONT_KIND,      e_WinProperty.FONT_KIND,        setFontKind),
        new CPropertyTable(WinPropertyChunk.GROUP,          e_WinProperty.GROUP,            setGroup),
        new CPropertyTable(WinPropertyChunk.CONTENTS,       e_WinProperty.CONTENTS,         setContents),
        new CPropertyTable(WinPropertyChunk.SLIDEMAX,       e_WinProperty.SLIDEMAX,         setSlideMax),
        new CPropertyTable(WinPropertyChunk.PRIORITY,       e_WinProperty.PRIORITY,         setPriority),
        new CPropertyTable(WinPropertyChunk.CONTENTS_SIZE,  e_WinProperty.CONTENTS_SIZE,    setContentsSize),
        new CPropertyTable(WinPropertyChunk.TEXTURE_ZOFFSET,e_WinProperty.TEXTURE_ZOFFSET,  setTextureZOffset),
    };
        static Dictionary<uint, CPropertyTable> m_dicChunk = new Dictionary<uint, CPropertyTable>();
        //==========================================================================
        /*!class Constructor
            @brief	class Constructor
        */
        static CWindowBase() {
            foreach (CPropertyTable cPT in m_aPropertyTable) {
                m_dicChunk[cPT.m_chunk] = cPT;
            }
        }

        //==========================================================================
        /*!Awake
            @brief	Unity Callback
        */
        protected void Awake() {
            m_transform = base.transform;
            m_cWindowRenderer = new CWindowRenderer(this);
            CWindowMgr cMgr = CWindowMgr.Instance;
            m_fontKind = cMgr.fontKind;
            m_screenSize.x.y = 1f;
            m_screenSize.y.z = 1f;
        }
        //==========================================================================
        /*!Update
            @brief	Unity Callback
        */
        protected void Update() {
            if (m_bClose) {
                render();
                if (m_eFadeState == e_FadeState.None) {
                    DestroyImmediate(gameObject, true);
                }
            }
        }
        //==========================================================================
        /*!Initialize
            @brief	initialize
        */
        public void initialize(uint uid, t_WindowData tData, CWindowBase cParent) {
            m_uid = uid;
            m_cParent = cParent;
            if (cParent != null) {
                m_cParent.m_cChild = this;
            }
            m_cChild = null;
            // デフォルト初期化.
            for (int i = 0; i < m_aColor.Length; i++) {
                m_aColor[i] = WinColor.white;
            }
            CReadVariable cVariable = new CReadVariable(tData.m_aData);
            // ウィンドウのプロパティを設定する.
            for (uint i = 0; i < tData.m_propertyNum; i++) {
                uint chunk = cVariable.getU32();
                CPropertyTable cPT;
                if (!m_dicChunk.TryGetValue(chunk, out cPT)) {
                    Debug.LogWarning("window chunk error:" + new FiveCC(chunk) + ":" + chunk);
                    continue;
                }
                cPT.m_dlSetter(this, cVariable);
            }
            // エリアの初期化.
            if (m_isSafeArea) {
                m_area = CWindowMgr.Instance.safeArea;
            } else {
                m_area = CWindowMgr.Instance.area;
            }
            // コントロールを展開する.
            t_WinCtrlData cCtrlData = new t_WinCtrlData();
            List<CWinCtrlBase> lstCtrl = new List<CWinCtrlBase>();
            for (int i = 0; i < tData.m_ctrlNum; i++) {
                cCtrlData.read(cVariable);
                CWinCtrlBase cCtrl = CWinFactory.create(this, (e_WinCtrlKind)cCtrlData.m_kind);
                if (cCtrl == null) {
                    continue;
                }
                cCtrl.intialize(cCtrlData.m_propertyNum, cVariable);
                m_cContents.append(cCtrl);
                lstCtrl.Add(cCtrl);
            }
            // デフォルトテクスチャのZOFFSETを確定させておく.
            getMeshZOffset(texId);
            // group & contentsの解決.
            foreach (CWinCtrlBase ctrl in lstCtrl) {
                ctrl.start();
            }
            // 全てをアップデート対象に.
            m_eUpdateFlag = e_UpdateFlag.ALL;
            // コールバック呼び出し.
            onCreate();
            // フェードアニメーションパラメータ.
            startFade(e_FadeState.Open);
        }
        Vector2 getAbsPosition() {
            float ox = screenOrgX;
            float oy = screenOrgY;
            float scw = screenWidth;
            float sch = screenHeight;

            Vector2 pos;
            pos.x = -CWindowMgr.width * 0.5f + ox + x;
            pos.y = -CWindowMgr.height * 0.5f + oy + y;

            switch (m_eAnchor) {
            default:
                goto case e_Anchor.LeftTop;
            case e_Anchor.Center:
                pos.x += (scw - width) * 0.5f;
                pos.y += (sch - height) * 0.5f;
                break;
            case e_Anchor.Top:
                pos.x += (scw - width) * 0.5f;
                break;
            case e_Anchor.Bottom:
                pos.x += (scw - width) * 0.5f;
                pos.y += sch - height;
                break;
            case e_Anchor.LeftCenter:
                pos.y += (sch - height) * 0.5f;
                break;
            case e_Anchor.LeftTop:
                break;
            case e_Anchor.LeftBottom:
                pos.y += sch - height;
                break;
            case e_Anchor.RightCenter:
                pos.x += scw - width;
                pos.y += (sch - height) * 0.5f;
                break;
            case e_Anchor.RightTop:
                pos.x += scw - width;
                break;
            case e_Anchor.RightBottom:
                pos.x += scw - width;
                pos.y += sch - height;
                break;
            }
            return pos;
        }
        //==========================================================================
        /*!ウィンドウを閉じることを要求する.
            @brief	close
        */
        public void close(int iCloseInfo = 0) {
            if (m_bClose) {
                Debug.LogWarning("this window is already closed:" + this);
                return;
            }
            if (!onClose(iCloseInfo)) {
                return;
            }
            CWindowMgr cWindowMgr = CWindowMgr.Instance;
            if (cWindowMgr != null) {
                if (cWindowMgr.dragCtrl != null && cWindowMgr.dragCtrl.window == this) {
                    onDropGround(cWindowMgr.dragCtrl);
                    cWindowMgr.clearDragInfo();
                }
            }
            startFade(e_FadeState.Close);
            m_bClose = true;
        }
        //==========================================================================
        /*!Update
            @brief	update : call from CWindowMgr
        */
        public void update(CWindowMgr cWindowMgr, int priority) {
            if (m_isSafeArea) {
                m_area = cWindowMgr.safeArea;
            } else {
                m_area = cWindowMgr.area;
            }
            // 位置の更新チェック.
            m_absPosition = getAbsPosition();
            // プライオリティ反映.
            Vector3 pos;
            pos.x = m_absPosition.x;
            pos.y = -m_absPosition.y;
            pos.z = (float)-priority * (1f / 512f);
            m_transform.position = pos;
            // フレームの更新チェック.
            if ((m_eUpdateFlag & e_UpdateFlag.FRAME) != 0) {
                updateFrame();
            }
            // パーツリセット.
            if ((m_eUpdateFlag & e_UpdateFlag.RESETPARTS) != 0) {
                resetParts();
            }
            // コントロールバー更新チェック.
            if (m_cCtrlBar != null) {
                Vector3 basepos = Vector3.zero;
                basepos.y -= m_cCtrlBar.height;
                m_cCtrlBar.position = basepos;
                m_cCtrlBar.captionOffset = captionOffset;
                m_cCtrlBar.captionColor = captionColor;
                if (m_cCtrlCloseButton != null) {
                    basepos.x = width - m_cCtrlCloseButton.width;
                    m_cCtrlCloseButton.position = basepos;
                }
            }
            // アップデート呼び出し.
            onUpdate();
            // コントロールのアップデート呼び出し.
            m_cContents.update(Vector3.zero, 0);
            onAfterUpdate();
            // フェード処理.
            if (m_eFadeState != e_FadeState.None) {
                if (!m_bFadeLoaded) {
                    if (m_cWindowRenderer.isLoaded) {
                        m_bFadeLoaded = true;
                    }
                } else {
                    if (!doFade(m_eFadeState)) {
                        m_eFadeState = e_FadeState.None;
                    }
                }
            }
        }
        //==========================================================================
        /*!startFade
            @brief	フェード処理を司る.
        */
        virtual protected void startFade(e_FadeState eState) {
            m_eFadeState = eState;
            m_angFade = Angle.InitLerpFactor();
            switch (m_eFadeState) {
            case e_FadeState.Open:
                m_colorFadeS = m_aColor[0];
                m_colorFadeS.a = 0;
                m_colorFadeE = m_aColor[0];
                m_posFadeS = getRatioValue(m_posClose);
                m_posFadeE.x = x;
                m_posFadeE.y = y;
                m_scaleFadeS = m_scaleClose;
                m_scaleFadeE = new Vector2(1f, 1f);
                // テクスチャのロードを始める.
                for (int i = 0; i < m_aTexture.Length; ++i) {
                    if (m_aTexture[i].texId == 0) {
                        continue;
                    }
                    getWindowMesh(m_aTexture[i].texId);
                }
                m_bFadeLoaded = false;
                m_aColor[0] = m_colorFadeS;
                break;
            case e_FadeState.Close:
                m_colorFadeS = m_aColor[0];
                m_colorFadeE = m_aColor[0];
                m_colorFadeE.a = 0;
                m_posFadeS.x = x;
                m_posFadeS.y = y;
                m_posFadeE = getRatioValue(m_posClose);
                m_scaleFadeS = new Vector2(1f, 1f);
                m_scaleFadeE = m_scaleClose;
                m_bFadeLoaded = true;
                break;
            }
        }
        //==========================================================================
        /*!doFade
            @brief	フェード処理を司る.
        */
        virtual protected bool doFade(e_FadeState eState) {
            if (eState == e_FadeState.None) {
                return false;
            }
            m_cWindowRenderer.clearTransform();
            float kOld = Angle.Cos(m_angFade);
            float k = Angle.LerpFactor(ref m_angFade, m_fadeSpeed);
            m_aColor[0] = WinColor.Lerp(m_colorFadeS, m_colorFadeE, k);
            if (m_bCloseAnimation) {
                Vector2 scale = Vector2.Lerp(m_scaleFadeS, m_scaleFadeE, kOld);
                transform.localScale = new Vector3(scale.x, scale.y, 1f);
                scale = Vector2.Lerp(m_scaleFadeS, m_scaleFadeE, k);
                Vector2 pos = Vector2.Lerp(m_posFadeS, m_posFadeE, k);
                x = pos.x + (1f - scale.x) * (width * 0.5f + m_offsetClose.x);
                y = pos.y + (1f - scale.y) * (height * 0.5f + m_offsetClose.y);
            }
            if (k == 1f) {
                transform.localScale = new Vector3(m_scaleFadeE.x, m_scaleFadeE.y, 1f);
                return false;
            }
            return true;
        }
        //==========================================================================
        /*!Render
            @brief	render : call from CWindowMgr
        */
        public void render() {
            if (!isNeedToRender) {
                return;
            }
            WinColor color = m_aColor[0];
            if (disableColor) {
                color.r = m_disableColor;
                color.g = m_disableColor;
                color.b = m_disableColor;
            } else {
                color.r = 127;
                color.g = 127;
                color.b = 127;
            }
            m_cWindowRenderer.sort();
            onPreRender(m_cWindowRenderer);
            m_cContents.render(color, null);
            onRender(m_cWindowRenderer);
        }
        //==========================================================================
        /*!find
            @brief	find
        */
        public WinCtrl find<WinCtrl>(uint id, uint contentsId = 0) where WinCtrl : CWinCtrlBase {
            CWinCtrlBase cCtrl = find(id, contentsId);
            return cCtrl as WinCtrl;
        }
        public CWinCtrlBase find(uint id, uint contentsId = 0) {
            CWinCtrlBase cCtrl = m_cContents.find(id, contentsId);
            if (cCtrl == null) {
                Debug.LogWarning("can't find ctrl:" + new MulId(id));
            }
            return cCtrl;
        }
        //==========================================================================
        /*!append
            @brief	append
        */
        public bool append(CWinCtrlBase cCtrl) {
            return m_cContents.append(cCtrl);
        }
        //==========================================================================
        /*!remove
            @brief	remove
        */
        public void remove(CWinCtrlBase cCtrl, bool bRelease) {
            m_cContents.remove(cCtrl, bRelease);
        }
        //==========================================================================
        /*!release
            @brief	release
        */
        virtual public void release() {
            if (m_cParent != null) {
                m_cParent.onDestroyChild(this);
            }
            m_cContents.release();
        }
        //==========================================================================
        /*!ウィンドウ絶対座標系から特定のウィンドウ相対座標系に変換する.
            @brief	getLocalPos
        */
        public Vector2 getLocalPos(Vector2 pos) {
            return pos - m_absPosition;
        }
        //==========================================================================
        /*!ワールド座標系から、ウィンドウ座標系に変換する.
         * @brief getPositionFromWorld
        */
        public Vector2 getPositionFromWorld(Camera camera, Vector3 pos) {
            Vector3 scpos = camera.WorldToScreenPoint(pos);
            Vector3 winpos = m_transform.position;
            scpos.x += -winpos.x - CWindowMgr.width * 0.5f;
            scpos.y += -winpos.y - CWindowMgr.height * 0.5f;
            Vector2 res;
            res.x = scpos.x;
            res.y = -scpos.y;
            return res;
        }
        //==========================================================================
        /*!ヒットチェックする.
            @brief	checkHit
        */
        public CWindowMgr.CCollision checkHit(Vector2 touchPos, CStick stk) {
            CWindowMgr.CCollision cCollision = null;
            if (isNeedToCheckHit) {
                touchPos = getLocalPos(touchPos);
                cCollision = m_cContents.checkHit(touchPos, null, stk);
                /*
                            if (cCollision == null) {
                                if (0 <= touchPos.x && touchPos.x <= width && 0 <= touchPos.y && touchPos.y <= height) {
                                    cCollision = new CWindowMgr.CCollision(this);
                                }
                            }
                 */
            }
            return cCollision;
        }
        //==========================================================================
        /*!ヒット共通処理.
            @brief	procHitCommon
        */
        public bool procHitCommon(CWindowMgr cWindowMgr, CWinCtrlBase cCtrl) {
            CStick cStick = cWindowMgr.stick;
            if (cStick == null) {
                return false;
            }
            if ((m_eStyle & e_WinStyle.DISABLE_FLAG) != 0) {
                if (cStick.state == CInput.e_State.Press) {
                    cWindowMgr.setDragInfo(false, this, cCtrl, cStick.windowPos);
                }
                return false;
            }
            CWindowBase cChildWin = child;
            if (cChildWin != null && cChildWin.priorityStyle != e_WinStyle.POPUP) {
                if (cStick.state == CInput.e_State.Press) {
                    cWindowMgr.setDragInfo(false, this, cCtrl, cStick.windowPos);
                }
                return false;
            }
            if (cCtrl != null && cCtrl.disable) {
                return false;
            }
            return true;
        }
        //==========================================================================
        /*!描画用オブジェクトを要求する.
            @brief	getWindowMesh()
        */
        protected float getMeshZOffset(uint id) {
            float zoffset;
            if (!m_dicTextureZOffset.TryGetValue(id, out zoffset)) {
                return 0.0f;
            }
            return zoffset;
        }
        public CWindowMesh getWindowMesh(uint id) {
            return m_cWindowRenderer.getWindowMesh(id, getMeshZOffset(id));
        }
        //==========================================================================
        /*!レンダリング用オブジェクトを要求する.
            @brief	getRenderMesh()
        */
        public CWindowMesh getRenderMesh(uint id, int szRenderTexture, RenderTextureFormat format) {
            return m_cWindowRenderer.getRenderMesh(id, szRenderTexture, format, getMeshZOffset(id));
        }
        //==========================================================================
        /*!テキスト描画用オブジェクトを要求する.
            @brief	getTextMesh()
        */
        public CWinTextMesh getTextMesh(uint id) {
            return m_cWindowRenderer.getTextMesh(id);
        }
        //==========================================================================
        /*!キャプションを取得する.
            @brief	getCaption
        */
        virtual public string getCaption(uint mCaptionId) {
            CWindowMgr cMgr = CWindowMgr.Instance;
            if (cMgr == null) {
                return "null";
            }
            return cMgr.getCaption(mCaptionId);
        }
        //==========================================================================
        /*!フレームを更新する.
            @brief	updateFrame()
        */
        void updateFrame() {
            // タイトルバーを設定する.
            if (m_cCtrlBar != null) {
                m_cContents.remove(m_cCtrlBar, true);
                m_cCtrlBar = null;
            }
            if (m_cCtrlCloseButton != null) {
                m_cContents.remove(m_cCtrlCloseButton, true);
                m_cCtrlCloseButton = null;
            }
            m_cCtrlBar = null;
            if ((m_eStyle & e_WinStyle.NOTITLEBAR) == 0 && priorityStyle != e_WinStyle.POPUP) {
                m_cCtrlBar = new CWinCtrlBar(this);
                m_cContents.append(m_cCtrlBar);
                m_cCtrlBar.id = WinSystemCtrl.BAR;
                m_cCtrlBar.width = width;
                m_cCtrlBar.fontKind = fontKind;
                m_cCtrlBar.caption = caption;
                m_cCtrlBar.priority = -65536;
                if (m_aTexture[1].partId != 0) {
                    m_cCtrlBar.partId = m_aTexture[2].partId;
                }
                m_cCtrlBar.color = m_aColor[2];
                if ((m_eStyle & (e_WinStyle.NOCLOSE)) == 0) {
                    m_cCtrlCloseButton = new CWinCtrlCloseButton(this);
                    m_cContents.append(m_cCtrlCloseButton);
                    m_cCtrlCloseButton.id = WinSystemCtrl.CLOSEBUTTON;
                    if (m_aTexture[2].partId != 0) {
                        m_cCtrlCloseButton.partId = m_aTexture[3].partId;
                    }
                    m_cCtrlCloseButton.color = m_aColor[3];
                    m_cCtrlCloseButton.priority = -65535;
                }
            }
            // フレームを生成する.
            if (m_cCtrlFrame != null) {
                m_cContents.remove(m_cCtrlFrame, true);
                m_cCtrlFrame = null;
            }
            switch (m_eStyle & e_WinStyle.FRAME_MASK) {
            case e_WinStyle.FRAME:
                m_cCtrlFrame = new CWinCtrlFrame(this);
                m_cContents.append(m_cCtrlFrame);
                m_cCtrlFrame.id = WinSystemCtrl.FRAME;
                if (m_aTexture[1].partId != 0) {
                    m_cCtrlFrame.partId = m_aTexture[1].partId;
                }
                m_cCtrlFrame.color = m_aColor[1];
                m_cCtrlFrame.width = width;
                m_cCtrlFrame.height = height;
                m_cCtrlFrame.position = Vector3.zero;
                m_cCtrlFrame.priority = -65536;
                break;
            }
            // コンテンツサイズの更新.
            m_eUpdateFlag &= ~e_UpdateFlag.FRAME;
        }
        //==========================================================================
        /*!パーツを再更新する必要があるとき呼ばれる.
            @brief	resetParts
        */
        protected void resetParts() {
            if (m_cContents == null) {
                return;
            }
            m_cContents.resetParts();
            m_eUpdateFlag &= ~e_UpdateFlag.RESETPARTS;
        }
        //==========================================================================
        /*!効果音を発声する.
            @brief	play
        */
        virtual public void play(uint mSE) {
            CWindowMgr cMgr = CWindowMgr.Instance;
            if (cMgr == null) {
                return;
            }
            cMgr.play(mSE);
        }
        //==========================================================================
        /*!onUpdate
            @brief	Window Callback
            @note	コンテンツのアップデート前に呼ばれる.
        */
        public virtual void onUpdate() {
        }
        //==========================================================================
        /*!onAfterUpdate
            @brief	Window Callback
            @note	コンテンツのアップデート後に呼ばれる.
        */
        public virtual void onAfterUpdate() {
        }
        //==========================================================================
        /*!onPreRender
            @brief	Window Callback
            @note	ウィンドウコントロールのレンダリング前に呼ばれる.
        */
        public virtual void onPreRender(CWindowRenderer cRenderer) {
        }
        //==========================================================================
        /*!onRender
            @brief	Window Callback
            @note	ウィンドウコントロールのレンダリングが終わった後に呼ばれる.
        */
        public virtual void onRender(CWindowRenderer cRenderer) {
        }
        //==========================================================================
        /*!onRender
            @brief	Window Callback
            @note	メッシュに対して自由にレンダリングできる.
        */
        public virtual bool onRender(CWinCtrlCanvas cCtrl, Vector3 pos, CWindowMesh cWinMesh, CWinTextMesh textMesh, WinColor rtcolor, ClipRect cr) {
#if UNITY_EDITOR
            return false;
#else
        return true;
#endif
        }
        //==========================================================================
        /*!onCreate
            @brief	Window Callback
            @note	ウィンドウの初期化が終わった直後.
        */
        public virtual void onCreate() {
        }
        //==========================================================================
        /*!onClose
            @brief	Window Callback
            @retval	true	:ウィンドウを閉じる.
                    false	:ウィンドウを閉じるのを抑制する.
        */
        public virtual bool onClose(int iCloseInfo) {
            return true;
        }
        //==========================================================================
        /*!onClick
            @brief	Window Callback
        */
        public virtual void onClick(CWinCtrlBase cCtrl) {
        }
        //==========================================================================
        /*!onHold(特定時間押されっぱなしになったとき呼ばれる).
            @brief	Window Callback
        */
        public virtual void onHold(CWinCtrlBase cCtrl) {
        }
        //==========================================================================
        /*!onClickEnter(エディットボックス編集中にエンターキーが押された).
            @brief	Window Callback
        */
        public virtual void onClickEnter(CWinCtrlBase cCtrl) {
        }
        //==========================================================================
        /*!onClick for RichText
            @brief	Window Callback
        */
        public virtual void onClick(CWinCtrlBase cCtrl, CRichTextOne cText) {
        }
        //==========================================================================
        /*!onBeginDrag
            @brief	Window Callback
        */
        public virtual void onBeginDrag(CWinCtrlBase cCtrl, Vector2 pos) {
        }
        //==========================================================================
        /*!onDrag
            @brief	Window Callback
        */
        public virtual void onDrag(CWinCtrlBase cCtrl, Vector2 pos, Vector2 dragVelocity) {
        }
        //==========================================================================
        /*!onDragRender
            @brief	Window Callback
            @retval	true:	ドラッグされているコントロールをレンダリングする.
                    false:	ドラッグされているコントロールをレンダリングしない.
        */
        public virtual bool onDragRender(CWinCtrlBase cCtrl, Transform transform) {
            return true;
        }
        //==========================================================================
        /*!onDrop
            @brief	Window Callback
        */
        public virtual void onDrop(CWinCtrlBase cCtrl, CWindowBase cDragWindow, CWinCtrlBase cDragCtrl) {
        }
        //==========================================================================
        /*!onDropGround
            @brief	Window Callback
        */
        public virtual void onDropGround(CWinCtrlBase cCtrl) {
        }
        //==========================================================================
        /*!onDestroyChild
            @brief	Window Callback
        */
        public virtual void onDestroyChild(CWindowBase cChildWindow) {
        }
        public void iMessageBox(CMessageBox.e_Kind eKind, int msgBoxWinID) {
            switch (eKind) {
            case CMessageBox.e_Kind.Ok:
                onOK(msgBoxWinID);
                break;
            case CMessageBox.e_Kind.Yes:
                onYes(msgBoxWinID);
                break;
            case CMessageBox.e_Kind.No:
                onNo(msgBoxWinID);
                break;
            case CMessageBox.e_Kind.Cancel:
                onCancel(msgBoxWinID);
                break;
            }
        }
        //==========================================================================
        /*!onOK
            @brief	MessageBox Callback
        */
        public virtual void onOK(int msgBoxWinID) {
        }
        //==========================================================================
        /*!onYes
            @brief	MessageBox Callback
        */
        public virtual void onYes(int msgBoxWinID) {
        }
        //==========================================================================
        /*!onNo
            @brief	MessageBox Callback
        */
        public virtual void onNo(int msgBoxWinID) {
        }
        //==========================================================================
        /*!onCancel
            @brief	MessageBox Callback
            @retval bool    true    :close window
                            false   :not close window
        */
        public virtual bool onCancel(int msgBoxWinID) {
            return true;
        }
        //==========================================================================
        /*!onBeginRenderIcon
            @brief	レンダーアイコンにレンダリングを要求された.
            @retval	true	:レンダリングに成功/onEndRederIconが後で呼ばれる.
                    false	:レンダリングに失敗.
        */
        public virtual bool onBeginRenderIcon(CWinCtrlRenderIcon cCtrl) {
            return true;
        }
        //==========================================================================
        /*!onEndRenderIcon
            @brief	アイコンへのレンダリング後に呼ばれる.
            @retval	true	:次フレームも続けてレンダリングするときは、trueを返す.
                    false	:次フレームはレンダリングをせず、今フレームでレンダリングした結果を使う.
        */
        public virtual bool onEndRenderIcon(CWinCtrlRenderIcon cCtrl) {
            return true;
        }
        //==========================================================================
        /*!onRecreatedRenderTexture
            @brief	RenderTextureが再作成されたことが通知される.
        */
        public virtual void onRecreatedRenderTexture(uint mRenderTextureId) {
            m_cContents.onRecreatedRenderTexture(mRenderTextureId);
        }
        //==========================================================================
        /*!onResizeScreen
            @brief	スクリーンサイズがリサイズされた.
        */
        public virtual void onResizeScreen() {
            resetParts();
        }
        //==========================================================================
        /*!onUpdateCaption
            @brief	キャプションを更新された.
        */
        public virtual void onUpdateCaption(CWinCtrlBase cCtrl) {
        }
        //==========================================================================
        /*!コンテンツを取得する.
            @brief	getContents
        */
        public CWinContents getContents() {
            return m_cContents;
        }
        public Vector2 getRatioValue(RatioPair pair) {
            Vector2 v;
            float w = width;
            float h = height;
            v.x = pair.x.x + pair.x.y * w + pair.x.z * h;
            v.y = pair.y.x + pair.y.y * w + pair.y.z * h;
            return v;
        }

        public uint id {
            get {
                return m_id;
            }
            set {
                m_id = id;
            }
        }
        public uint uid {
            get {
                return m_uid;
            }
        }
        public CWindowBase child {
            get {
                return m_cChild;
            }
        }
        public CWindowBase parent {
            get {
                return m_cParent;
            }
        }
        public e_LayerId layer {
            get {
                return (e_LayerId)gameObject.layer;
            }
            set {
                gameObject.layer = (int)value;
                Transform[] aTrans = GetComponentsInChildren<Transform>();
                for (int i = 0; i < aTrans.Length; ++i) {
                    aTrans[i].gameObject.layer = (int)value;
                }
            }
        }
        public int priority {
            get {
                return m_priority;
            }
            set {
                m_priority = value;
            }
        }
        public CWindowRenderer windowrenderer {
            get {
                return m_cWindowRenderer;
            }
        }
        public uint texId {
            get {
                uint id = m_aTexture[0].texId;
                if (id == 0) {
                    return KsSoftConfig.DefaultTextureId;
                }
                return id;
            }
        }
        public float x {
            get {
                return m_position.x.x + m_position.x.y * screenWidth + m_position.x.z * screenHeight;
            }
            set {
                m_position.x.x = value;
                m_position.x.y = 0f;
                m_position.x.z = 0f;
            }
        }
        public float y {
            get {
                return m_position.y.x + m_position.y.y * screenWidth + m_position.y.z * screenHeight;
            }
            set {
                m_position.y.x = value;
                m_position.y.y = 0f;
                m_position.y.z = 0f;
            }
        }
        public Vector2 position {
            get {
                Vector2 pos;
                pos.x = x;
                pos.y = y;
                return pos;
            }
            set {
                m_position.set(value);
            }
        }
        public Vector2 absPosition {
            get {
                return m_absPosition;
            }
        }
        new public Transform transform {
            get {
                return m_transform;
            }
        }
        //==========================================================================
        /*!スケールを設定する(コリジョンは、反映されないので注意).
            @brief	scale
            @note	not effect to collision
        */
        public Vector2 scale {
            get {
                Vector3 scale = m_transform.localScale;
                return new Vector2(scale.x, scale.y);
            }
            set {
                m_transform.localScale = new Vector3(value.x, value.y, 1f);
            }
        }
        public e_Anchor anchor {
            get {
                return m_eAnchor;
            }
            set {
                if (m_eAnchor == value) {
                    return;
                }
                m_eAnchor = value;
                m_eUpdateFlag |= e_UpdateFlag.RESETPARTS;
            }
        }
        public float screenOrgX {
            get {
                return m_area.x + m_screenOrg.x.x + m_screenOrg.x.y * m_area.z + m_screenOrg.x.z * m_area.w;
            }
        }
        public float screenOrgY {
            get {
                return m_area.y + m_screenOrg.y.x + m_screenOrg.y.y * m_area.z + m_screenOrg.y.z * m_area.w;
            }
        }
        protected float screenWidth {
            get {
                return m_screenSize.x.x + m_screenSize.x.y * m_area.z + m_screenSize.x.z * m_area.w;
            }
        }
        protected float screenHeight {
            get {
                return m_screenSize.y.x + m_screenSize.y.y * m_area.z + m_screenSize.y.z * m_area.w;
            }
        }
        public float width {
            get {
                return m_size.x.x + m_size.x.y * screenWidth + m_size.x.z * screenHeight;
            }
            set {
                float w = Mathf.Floor(value);
                if (width == w) {
                    return;
                }
                m_size.x.x = w;
                m_size.x.y = 0f;
                m_size.x.z = 0f;
                m_eUpdateFlag |= e_UpdateFlag.FRAME | e_UpdateFlag.RESETPARTS;
            }
        }
        public float height {
            get {
                return m_size.y.x + m_size.y.y * screenWidth + m_size.y.z * screenHeight;
            }
            set {
                float h = Mathf.Floor(value);
                if (height == h) {
                    return;
                }
                m_size.y.x = h;
                m_size.y.y = 0f;
                m_size.y.z = 0f;
                m_eUpdateFlag |= e_UpdateFlag.FRAME | e_UpdateFlag.RESETPARTS;
            }
        }

        public Vector2 size {
            get {
                return new Vector2(width, height);
            }
            set {
                width = value.x;
                height = value.y;
            }
        }
        public uint fontKind {
            get {
                return m_fontKind;
            }
        }
        virtual public string caption {
            get {
                return m_caption;
            }
            set {
                if (m_caption == value) {
                    return;
                }
                m_caption = value;
                if (m_cCtrlBar != null) {
                    m_cCtrlBar.caption = value;
                }
            }
        }
        virtual public WinColor captionColor {
            get {
                return m_captionColor;
            }
            set {
                m_captionColor = value;
                if (m_cCtrlBar != null) {
                    m_cCtrlBar.captionColor = value;
                }
            }
        }
        public Vector2 captionOffset {
            get {
                return getRatioValue(m_captionOffset);
            }
            set {
                m_captionOffset.set(value);
            }
        }
        virtual public WinColor color {
            get {
                return m_aColor[0];
            }
            set {
                m_aColor[0] = value;
            }
        }
        public e_WinStyle style {
            get {
                return m_eStyle;
            }
        }
        public e_WinStyle priorityStyle {
            get {
                return (m_eStyle & e_WinStyle.PRIORITY_MASK);
            }
            set {
                if ((value & (~e_WinStyle.PRIORITY_MASK)) != e_WinStyle.NONE) {
                    Debug.LogError("illegal priority style:" + value);
                    return;
                }
                m_eStyle = (m_eStyle & (~e_WinStyle.PRIORITY_MASK)) | value;
            }
        }
        public bool isDragable {
            get {
                return ((m_eStyle & e_WinStyle.DRAG) != 0) ? true : false;
            }
        }
        public bool hide {
            get {
                return m_cContents.hide;
            }
            set {
                m_cContents.hide = value;
            }
        }
        public bool disable {
            get {
                if (m_cContents.disable) {
                    return m_cContents.disable;
                }
                return CWindowMgr.Instance.isDisable(this);
            }
            set {
                m_cContents.disable = value;
            }
        }
        public bool disableRecursive {
            set {
                m_cContents.disableRecursive = value;
            }
        }
        public bool disableColor {
            get {
                if (disable) {
                    return disable;
                }
                CWindowBase cWin = CWindowMgr.Instance.topMostWindow;
                if (cWin == null) {
                    return disable;
                }
                return CWindowMgr.Instance.isDisable(this);
            }
            set {
                disable = value;
            }
        }
        public bool nohit {
            get {
                return m_cContents.nohit;
            }
            set {
                m_cContents.nohit = value;
            }
        }
        public bool isClose {
            get {
                return m_bClose;
            }
        }
        public bool isLoaded {
            get {
                return m_cWindowRenderer.isLoaded;
            }
        }
        public bool isFade {
            get {
                return (m_eFadeState != e_FadeState.None);
            }
        }
        public float fadeInterpolate {
            get {
                int ang = m_angFade;
                return Angle.LerpFactor(ref ang, 0.5f);
            }
        }
        protected bool isNeedToRender {
            get {
                return !hide;
            }
        }
        protected bool isNeedToCheckHit {
            get {
                return !(nohit || disable || hide || isFade || isClose);
            }
        }
        public uint uniqueId {
            get {
                return m_uniqueId++;
            }
        }
        public float fadespeed {
            get {
                return m_fadeSpeed;
            }
            set {
                m_fadeSpeed = value;
            }
        }
        //==============================================================================================
        /*!ウィンドウプロパティに設定する.
            @brief	set*
        */
        //==============================================================================================
        static void setId(CWindowBase cWindow, CReadVariable cVariable) {
            t_WinProperty tProperty = new t_WinProperty();
            tProperty.read(cVariable);
            cWindow.m_id = (uint)tProperty.m_value;
        }
        static void setCaption(CWindowBase cWindow, CReadVariable cVariable) {
            t_WinProperty tProperty = new t_WinProperty();
            tProperty.read(cVariable);
            if (tProperty.m_value != 0) {
                cWindow.caption = cWindow.getCaption((uint)tProperty.m_value);
            }
        }
        static void setCaptionStr(CWindowBase cWindow, CReadVariable cVariable) {
            t_WinPropertyString tProperty = new t_WinPropertyString();
            tProperty.read(cVariable);
            cWindow.caption = tProperty.m_string;
        }
        static void setCaptionOffset(CWindowBase cWindow, CReadVariable cVariable) {
            cWindow.m_captionOffset.x = cVariable.getVector3();
            cWindow.m_captionOffset.y = cVariable.getVector3();
        }
        static void setPosition(CWindowBase cWindow, CReadVariable cVariable) {
            cWindow.m_position.x = cVariable.getVector3();
            cWindow.m_position.y = cVariable.getVector3();
        }
        static void setScreen(CWindowBase cWindow, CReadVariable cVariable) {
            cWindow.m_screenOrg.x = cVariable.getVector3();
            cWindow.m_screenOrg.y = cVariable.getVector3();
            cWindow.m_screenSize.x = cVariable.getVector3();
            cWindow.m_screenSize.y = cVariable.getVector3();
            cWindow.m_isSafeArea = false;
        }
        static void setSafeArea(CWindowBase cWindow, CReadVariable cVariable) {
            cWindow.m_screenOrg.x = cVariable.getVector3();
            cWindow.m_screenOrg.y = cVariable.getVector3();
            cWindow.m_screenSize.x = cVariable.getVector3();
            cWindow.m_screenSize.y = cVariable.getVector3();
            cWindow.m_isSafeArea = true;
        }
        static void setClosePosition(CWindowBase cWindow, CReadVariable cVariable) {
            cWindow.m_posClose.x = cVariable.getVector3();
            cWindow.m_posClose.y = cVariable.getVector3();
            cWindow.m_bCloseAnimation = true;
        }
        static void setCloseScale(CWindowBase cWindow, CReadVariable cVariable) {
            Vector4 prop = cVariable.getVector4();
            cWindow.m_scaleClose.x = prop.x / 100f;
            cWindow.m_scaleClose.y = prop.y / 100f;
            if (prop.z != 0) {
                cWindow.m_offsetClose.x = prop.z;
            }
            if (prop.w != 0) {
                cWindow.m_offsetClose.y = prop.w;
            }
            cWindow.m_bCloseAnimation = true;
        }
        static void setSize(CWindowBase cWindow, CReadVariable cVariable) {
            cWindow.m_size.x = cVariable.getVector3();
            cWindow.m_size.y = cVariable.getVector3();
        }
        static void setStyle(CWindowBase cWindow, CReadVariable cVariable) {
            t_WinProperty tProperty = new t_WinProperty();
            tProperty.read(cVariable);
            cWindow.m_eStyle = (e_WinStyle)tProperty.m_value;

            // ウィンドウの基準位置を取得.
            cWindow.anchor = (e_Anchor)(cWindow.m_eStyle & e_WinStyle.ANCHOR_MASK);
        }
        static void setColor(CWindowBase cWindow, int index, CReadVariable cVariable) {
            t_WinProperty tProperty = new t_WinProperty();
            tProperty.read(cVariable);
            cWindow.m_aColor[index] = new WinColor((uint)tProperty.m_value);
        }
        static void setColor0(CWindowBase cWindow, CReadVariable cVariable) {
            setColor(cWindow, 0, cVariable);
        }
        static void setColor1(CWindowBase cWindow, CReadVariable cVariable) {
            setColor(cWindow, 1, cVariable);
        }
        static void setColor2(CWindowBase cWindow, CReadVariable cVariable) {
            setColor(cWindow, 2, cVariable);
        }
        static void setColor3(CWindowBase cWindow, CReadVariable cVariable) {
            setColor(cWindow, 2, cVariable);
        }
        static void setCaptionColor(CWindowBase cWindow, CReadVariable cVariable) {
            t_WinProperty tProperty = new t_WinProperty();
            tProperty.read(cVariable);
            cWindow.m_captionColor = new WinColor((uint)tProperty.m_value);
        }
        static void setTexId(CWindowBase cWindow, int index, CReadVariable cVariable) {
            t_WinPropertyTexId tProperty = new t_WinPropertyTexId();
            tProperty.read(cVariable);
            if (tProperty.m_texId != 0) {
                cWindow.m_aTexture[index].texId = tProperty.m_texId;
            }
            if (tProperty.m_partId != 0) {
                cWindow.m_aTexture[index].partId = tProperty.m_partId;
            }
        }
        static void setTexId0(CWindowBase cWindow, CReadVariable cVariable) {
            setTexId(cWindow, 0, cVariable);
        }
        static void setTexId1(CWindowBase cWindow, CReadVariable cVariable) {
            setTexId(cWindow, 1, cVariable);
        }
        static void setTexId2(CWindowBase cWindow, CReadVariable cVariable) {
            setTexId(cWindow, 1, cVariable);
        }
        static void setTexId3(CWindowBase cWindow, CReadVariable cVariable) {
            setTexId(cWindow, 3, cVariable);
        }
        static void setTextureOffset(CWindowBase cWindow, int index, CReadVariable cVariable) {
            cWindow.m_aTexture[index].offset.x = cVariable.getVector3();
            cWindow.m_aTexture[index].offset.y = cVariable.getVector3();
        }
        static void setTextureOffset0(CWindowBase cWindow, CReadVariable cVariable) {
            setTextureOffset(cWindow, 0, cVariable);
        }
        static void setTextureOffset1(CWindowBase cWindow, CReadVariable cVariable) {
            setTextureOffset(cWindow, 1, cVariable);
        }
        static void setTextureOffset2(CWindowBase cWindow, CReadVariable cVariable) {
            setTextureOffset(cWindow, 2, cVariable);
        }
        static void setTextureOffset3(CWindowBase cWindow, CReadVariable cVariable) {
            setTextureOffset(cWindow, 3, cVariable);
        }
        static void setRelationId(CWindowBase cWindow, CReadVariable cVariable) {
            t_WinProperty tProperty = new t_WinProperty();
            tProperty.read(cVariable);
        }
        static void setHelpId(CWindowBase cWindow, CReadVariable cVariable) {
            t_WinProperty tProperty = new t_WinProperty();
            tProperty.read(cVariable);
        }
        static void setTooltip(CWindowBase cWindow, CReadVariable cVariable) {
            t_WinProperty tProperty = new t_WinProperty();
            tProperty.read(cVariable);
        }
        static void setFontKind(CWindowBase cWindow, CReadVariable cVariable) {
            t_WinProperty tProperty = new t_WinProperty();
            tProperty.read(cVariable);
            cWindow.m_fontKind = (uint)tProperty.m_value;
        }
        static void setGroup(CWindowBase cWindow, CReadVariable cVariable) {
            t_WinPropertyArray tProperty = new t_WinPropertyArray();
            tProperty.read(cVariable);
        }
        static void setContents(CWindowBase cWindow, CReadVariable cVariable) {
            t_WinPropertyArray tProperty = new t_WinPropertyArray();
            tProperty.read(cVariable);
        }
        static void setSlideMax(CWindowBase cWindow, CReadVariable cVariable) {
            t_WinProperty tProperty = new t_WinProperty();
            tProperty.read(cVariable);
        }
        static void setPriority(CWindowBase cWindow, CReadVariable cVariable) {
            t_WinProperty tProperty = new t_WinProperty();
            tProperty.read(cVariable);
            cWindow.priority = tProperty.m_value;
        }
        static void setContentsSize(CWindowBase cWindow, CReadVariable cVariable) {
            cVariable.getVector2();
        }
        static void setTextureZOffset(CWindowBase cWindow, CReadVariable cVariable) {
            t_WinPropertyTextureZOffset tProperty = new t_WinPropertyTextureZOffset();
            tProperty.read(cVariable);
            cWindow.m_dicTextureZOffset[tProperty.m_texId] = tProperty.m_zoffset;
        }
    }
}
