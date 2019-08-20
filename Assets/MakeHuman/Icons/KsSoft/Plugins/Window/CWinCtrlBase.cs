//==============================================================================================
/*!?ウィンドウフレームをレンダリングする.
	@file  CUICtrlBase

	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace KS {
    public struct RatioPair {
        public void set(Vector2 pair) {
            x.x = pair.x;
            x.y = 0f;
            x.z = 0f;
            y.x = pair.y;
            y.y = 0f;
            y.z = 0f;
        }
        public void set(float _x, float _y) {
            x.x = _x;
            x.y = 0f;
            x.z = 0f;
            y.x = _y;
            y.y = 0f;
            y.z = 0f;
        }
        public Vector3 x;
        public Vector3 y;
    };
    public class CWinCtrlBase : IComparable {
        [Flags]
        protected enum e_UpdateFlag {
            STYLE = 1 << 0,
            SIZE = 1 << 2,
            TEXID0 = 1 << 4,
            TEXID1 = 1 << 5,
            TEXID2 = 1 << 6,
            TEXID3 = 1 << 7,
            TEXID4 = 1 << 8,
            TEXID5 = 1 << 9,
            TEXID6 = 1 << 10,
            TEXID7 = 1 << 11,
            CAPTION = 1 << 12,
            CONTENTS = 1 << 13,
            GROUP = 1 << 14,
            TEXID = TEXID0 | TEXID1 | TEXID2 | TEXID3 | TEXID4 | TEXID5 | TEXID6 | TEXID7,
            ALL = STYLE | SIZE | TEXID | CAPTION | CONTENTS | GROUP,
        };
        [Flags]
        public enum e_Flags {
            Disable = 1 << 0,
            NoTap = 1 << 1,
        };
        protected CWindowBase m_cWindowBase;
        protected e_WinCtrlKind m_eKind = e_WinCtrlKind.NONE;
        protected uint m_id = 0;
        protected e_WinCtrlStyle m_eStyle = e_WinCtrlStyle.NONE;
        // 位置.
        protected int m_priority = 0;
        protected int m_absPriority = 0;
        protected RatioPair m_position;
        protected RatioPair m_position2;
        protected Vector3 m_absPosition = Vector3.zero;
        protected e_Anchor m_eAnchor = e_Anchor.Default;
        protected e_Anchor m_eBaseAnchor = e_Anchor.Default;

        public const int nTexture = 8;
        // テクスチャ.
        protected struct t_Texture {
            public uint texId;
            public uint partId;
            public Parts parts;
            public RatioPair size;
            public CWindowMesh cWindowMesh;
            public WinColor color;
            public RatioPair offset;
        };
        protected t_Texture[] m_aTexture = new t_Texture[nTexture];
        protected e_UpdateFlag m_eUpdateFlag;

        protected CWinTextMesh m_cTextMesh = null;
        protected CWinTextOne m_cTextOne = null;
        protected uint m_fontKind;

        // コンテンツ関連.
        protected CWinContents m_parent;            //コントロールが入っているコンテンツ.
        protected uint m_contentsId = 0;
        protected uint[] m_aContentsId = null;		//コンテンツIDのリスト.
        protected CWinContents m_contents;          //コンテンツ.
        protected CWinContents[] m_aContents = null;//コンテンツリスト.
        protected RatioPair m_contentsSize;         //コンテンツサイズ.
		protected int m_idxFocusContents = -1;		//直接動作に影響を与えないがjoyconによる操作などでフォーカスの当たっているコンテンツインデックスを保持する.

		// グループ関連.
		protected uint[] m_aGroupId = null;
        protected CWinCtrlBase[] m_aGroup = null;

        // キャプション.
        protected e_Anchor m_eTextAnchor = e_Anchor.LeftTop;
        protected CWinTextOne.e_Style m_eTextStyle = CWinTextOne.e_Style.Normal;
        protected uint m_captionId = 0;
        protected RatioPair m_captionOffset;
        protected string m_caption = "";
        protected string m_defaultCaption = "";
        protected WinColor m_captionColor = WinColor.white;
        protected float m_lineSpace = 1f;
        protected float m_lineFeedOffset = 0f;
        // エディットボックス.
        protected int m_maxChar = 0;
        protected int m_maxLine = 0;
        // SE_ID
        protected uint m_SEId;
        // 状態変更.
        protected bool m_bHide = false;        //非表示.
        protected bool m_bDisable = false;     //押せなくし、暗くする.
        protected bool m_bNohit = false;       //マウスの当たりなし.

        // クリック状態保持用.
        protected struct t_ProcHit {
            public void clear() {
                tap = false;
                hold = false;
                tmPress = 0f;
                state = CInput.e_State.Off;
            }
            public void update() {
                tap = false;
                state = CInput.e_State.Off;
            }
            public bool tap;
            public bool hold;
            public CInput.e_State state;
            public float tmPress;
        }
        class CPropertyTable {
            public CPropertyTable(uint chunk, e_WinProperty eProperty, DlSetter setter) {
                m_chunk = chunk;
                m_eWinProperty = eProperty;
                m_dlSetter = setter;
            }
            public delegate void DlSetter(CWinCtrlBase cWindow, CReadVariable cVariable);
            public uint m_chunk;
            public e_WinProperty m_eWinProperty;
            public DlSetter m_dlSetter;
        };
        static CPropertyTable[] m_aPropertyTable = new CPropertyTable[] {
        new CPropertyTable(WinPropertyChunk.ID,         e_WinProperty.ID,               setId),
        new CPropertyTable(WinPropertyChunk.CAPTION,    e_WinProperty.CAPTION,          setCaption),
        new CPropertyTable(WinPropertyChunk.CAPTION_STR,e_WinProperty.CAPTION_STR,      setCaptionStr),
        new CPropertyTable(WinPropertyChunk.CAPTION_OFFSET, e_WinProperty.CAPTION_OFFSET,   setCaptionOffset),
        new CPropertyTable(WinPropertyChunk.TEXTURE_OFFSET0,e_WinProperty.TEXTURE_OFFSET0,  setTextureOffset0),
        new CPropertyTable(WinPropertyChunk.TEXTURE_OFFSET1,e_WinProperty.TEXTURE_OFFSET1,  setTextureOffset1),
        new CPropertyTable(WinPropertyChunk.TEXTURE_OFFSET2,e_WinProperty.TEXTURE_OFFSET2,  setTextureOffset2),
        new CPropertyTable(WinPropertyChunk.TEXTURE_OFFSET3,e_WinProperty.TEXTURE_OFFSET3,  setTextureOffset3),
        new CPropertyTable(WinPropertyChunk.TEXTURE_OFFSET4,e_WinProperty.TEXTURE_OFFSET4,  setTextureOffset4),
        new CPropertyTable(WinPropertyChunk.TEXTURE_OFFSET5,e_WinProperty.TEXTURE_OFFSET5,  setTextureOffset5),
        new CPropertyTable(WinPropertyChunk.TEXTURE_OFFSET6,e_WinProperty.TEXTURE_OFFSET6,  setTextureOffset6),
        new CPropertyTable(WinPropertyChunk.TEXTURE_OFFSET7,e_WinProperty.TEXTURE_OFFSET7,  setTextureOffset7),
        new CPropertyTable(WinPropertyChunk.TEXTURE_SIZE1,e_WinProperty.TEXTURE_SIZE1,  setTextureSize1),
        new CPropertyTable(WinPropertyChunk.TEXTURE_SIZE2,e_WinProperty.TEXTURE_SIZE2,  setTextureSize2),
        new CPropertyTable(WinPropertyChunk.TEXTURE_SIZE3,e_WinProperty.TEXTURE_SIZE3,  setTextureSize3),
        new CPropertyTable(WinPropertyChunk.TEXTURE_SIZE4,e_WinProperty.TEXTURE_SIZE4,  setTextureSize4),
        new CPropertyTable(WinPropertyChunk.TEXTURE_SIZE5,e_WinProperty.TEXTURE_SIZE5,  setTextureSize5),
        new CPropertyTable(WinPropertyChunk.TEXTURE_SIZE6,e_WinProperty.TEXTURE_SIZE6,  setTextureSize6),
        new CPropertyTable(WinPropertyChunk.TEXTURE_SIZE7,e_WinProperty.TEXTURE_SIZE7,  setTextureSize7),
        new CPropertyTable(WinPropertyChunk.POSITION,   e_WinProperty.POSITION,         setPosition),
        new CPropertyTable(WinPropertyChunk.POSITION2,  e_WinProperty.POSITION2,        setPosition2),
        new CPropertyTable(WinPropertyChunk.SIZE,       e_WinProperty.SIZE,             setSize),
        new CPropertyTable(WinPropertyChunk.STYLE,      e_WinProperty.STYLE,            setStyle),
        new CPropertyTable(WinPropertyChunk.SE_ID,      e_WinProperty.SE_ID,            setSEId),
        new CPropertyTable(WinPropertyChunk.CAPTION_COLOR,e_WinProperty.CAPTION_COLOR,  setCaptionColor),
        new CPropertyTable(WinPropertyChunk.COLOR0,     e_WinProperty.COLOR0,           setColor0),
        new CPropertyTable(WinPropertyChunk.COLOR1,     e_WinProperty.COLOR1,           setColor1),
        new CPropertyTable(WinPropertyChunk.COLOR2,     e_WinProperty.COLOR2,           setColor2),
        new CPropertyTable(WinPropertyChunk.COLOR3,     e_WinProperty.COLOR3,           setColor3),
        new CPropertyTable(WinPropertyChunk.COLOR4,     e_WinProperty.COLOR4,           setColor4),
        new CPropertyTable(WinPropertyChunk.COLOR5,     e_WinProperty.COLOR5,           setColor5),
        new CPropertyTable(WinPropertyChunk.COLOR6,     e_WinProperty.COLOR6,           setColor6),
        new CPropertyTable(WinPropertyChunk.COLOR7,     e_WinProperty.COLOR7,           setColor7),
        new CPropertyTable(WinPropertyChunk.TEX_ID0,    e_WinProperty.TEX_ID0,          setTexId0),
        new CPropertyTable(WinPropertyChunk.TEX_ID1,    e_WinProperty.TEX_ID1,          setTexId1),
        new CPropertyTable(WinPropertyChunk.TEX_ID2,    e_WinProperty.TEX_ID2,          setTexId2),
        new CPropertyTable(WinPropertyChunk.TEX_ID3,    e_WinProperty.TEX_ID3,          setTexId3),
        new CPropertyTable(WinPropertyChunk.TEX_ID4,    e_WinProperty.TEX_ID4,          setTexId4),
        new CPropertyTable(WinPropertyChunk.TEX_ID5,    e_WinProperty.TEX_ID5,          setTexId5),
        new CPropertyTable(WinPropertyChunk.TEX_ID6,    e_WinProperty.TEX_ID6,          setTexId6),
        new CPropertyTable(WinPropertyChunk.TEX_ID7,    e_WinProperty.TEX_ID7,          setTexId7),
        new CPropertyTable(WinPropertyChunk.EDIT,       e_WinProperty.EDIT,             setEdit),
        new CPropertyTable(WinPropertyChunk.RELATION_ID,e_WinProperty.RELATION_ID,      setRelationId),
        new CPropertyTable(WinPropertyChunk.HELP_ID,    e_WinProperty.HELP_ID,          setHelpId),
        new CPropertyTable(WinPropertyChunk.TOOLTIP,    e_WinProperty.TOOLTIP,          setTooltip),
        new CPropertyTable(WinPropertyChunk.FONT_KIND,  e_WinProperty.FONT_KIND,        setFontKind),
        new CPropertyTable(WinPropertyChunk.GROUP,      e_WinProperty.GROUP,            setGroup),
        new CPropertyTable(WinPropertyChunk.CONTENTS,   e_WinProperty.CONTENTS,         setContents),
        new CPropertyTable(WinPropertyChunk.SLIDEMAX,   e_WinProperty.SLIDEMAX,         setSlideMax),
        new CPropertyTable(WinPropertyChunk.PRIORITY,   e_WinProperty.PRIORITY,         setPriority),
        new CPropertyTable(WinPropertyChunk.CONTENTS_SIZE,e_WinProperty.CONTENTS_SIZE,  setContentsSize),
        new CPropertyTable(WinPropertyChunk.LINE_SPACE, e_WinProperty.LINE_SPACE,       setLineSpace),
        new CPropertyTable(WinPropertyChunk.LINE_FEED_OFFSET, e_WinProperty.LINE_FEED_OFFSET, setLineFeedOffset),
    };
        static Dictionary<uint, CPropertyTable> m_dicChunk = new Dictionary<uint, CPropertyTable>();
        //==========================================================================
        /*!class Constructor
            @brief	class Constructor
        */
        static CWinCtrlBase() {
            foreach (CPropertyTable cPT in m_aPropertyTable) {
                m_dicChunk[cPT.m_chunk] = cPT;
            }
        }
        //==========================================================================
        /*!Constructor
            @brief	Constructor
        */
        public CWinCtrlBase(CWindowBase cWindow, e_WinCtrlKind eKind) {
            m_cWindowBase = cWindow;
            m_eKind = eKind;
            m_eUpdateFlag = e_UpdateFlag.ALL;
            for (int i = 0; i < m_aTexture.Length; i++) {
                m_aTexture[i].color = WinColor.white;
            }
        }
        //==========================================================================
        /*!Copy Constructor
            @brief	Copy Constructor
        */
        public CWinCtrlBase(CWinCtrlBase src, CWinContents parent) {
            m_eUpdateFlag = e_UpdateFlag.ALL;

            m_cWindowBase = src.m_cWindowBase;
            m_cTextMesh = src.m_cTextMesh;
            if (src.m_cTextOne != null) {
                m_cTextOne = new CWinTextOne(src.m_cTextOne);
            } else {
                m_cTextOne = null;
            }
            m_eKind = src.m_eKind;
            m_id = src.m_id;
            m_eStyle = src.m_eStyle;
            m_priority = src.m_priority;
            m_absPriority = src.m_absPriority;
            m_position = src.m_position;
            m_position2 = src.m_position2;
            m_absPosition = src.m_absPosition;

            m_eAnchor = src.m_eAnchor;
            m_eBaseAnchor = src.m_eBaseAnchor;

            for (int i = 0; i < m_aTexture.Length; i++) {
                m_aTexture[i] = src.m_aTexture[i];
            }

            m_fontKind = src.m_fontKind;
            if (parent == null) {
                m_parent = src.m_parent;
            } else {
                m_parent = parent;
            }
            m_contentsId = src.m_contentsId;
            m_contentsSize = src.m_contentsSize;

            m_eTextAnchor = src.m_eTextAnchor;
            m_eTextStyle = src.m_eTextStyle;
            m_captionId = src.m_captionId;
            m_caption = src.m_caption;
            m_defaultCaption = src.m_caption;
            m_captionOffset = src.m_captionOffset;
            m_captionColor = src.m_captionColor;
            m_lineSpace = src.m_lineSpace;

            m_maxChar = src.m_maxChar;
            m_maxLine = src.m_maxLine;

            m_SEId = src.m_SEId;

            m_bHide = src.m_bHide;
            m_bDisable = src.m_bDisable;
            m_bNohit = src.m_bNohit;

            m_aContentsId = src.m_aContentsId;
            m_contents = src.m_contents;
            if (src.m_aContents == null) {
                m_aContents = null;
            } else {
                m_aContents = new CWinContents[src.m_aContents.Length];
                for (int j = 0; j < m_aContents.Length; j++) {
                    CWinContents cContents = src.m_aContents[j];
                    if (cContents != null) {
                        cContents = new CWinContents(cContents, this);
                    }
                    m_aContents[j] = cContents;
                }
            }
            m_aGroupId = src.m_aGroupId;
            m_aGroup = src.m_aGroup;
        }
        //==========================================================================
        /*!Initialize
            @brief	Initialize
        */
        public void intialize(uint propertyNum, CReadVariable cVariable) {
            // コントロールのプロパティを設定する.
            for (int i = 0; i < propertyNum; i++) {
                uint chunk = cVariable.getU32();
                CPropertyTable cPT;
                if (!m_dicChunk.TryGetValue(chunk, out cPT)) {
                    Debug.LogWarning("window chunk error:" + new FiveCC(chunk));
                    continue;
                }
                cPT.m_dlSetter(this, cVariable);
            }
        }
        //==========================================================================
        /*!Initialize
            @brief	Initialize
        */
        protected virtual void initialize() {
        }
        //==========================================================================
        /*!コンテンツとグループの解決を行う.
            @brief	start
        */
        public void start() {
            m_contents = createContents(m_aContentsId);
            m_aGroup = createGroup(m_aGroupId);

            // ラジオボタンの場合先頭を選択状態にする.
            if (m_aGroup != null && m_aGroup.Length >= 1) {
                CWinCtrlRadio cRadio = m_aGroup[0] as CWinCtrlRadio;
                if (cRadio != null) {
                    cRadio.check = true;
                }
            }
            initialize();
            if (m_cTextOne != null && m_cTextMesh == null) {
                m_cTextMesh = window.getTextMesh(m_fontKind);
                if (m_cTextMesh != null) {
                    m_cTextOne.initialize(m_cTextMesh.spriteFont);
                }
            }
        }
        protected CWinContents createContents(uint[] aId) {
            if (aId == null || aId.Length <= 0) {
                return null;
            }
            CWinContents contents = new CWinContents(this);
            foreach (uint id in aId) {
                CWinCtrlBase cCtrl = m_cWindowBase.find(id);
                if (cCtrl == null) {
                    Debug.LogWarning("can't find ctlr:" + new MulId(id));
                    continue;
                }
                contents.append(cCtrl);
                m_cWindowBase.remove(cCtrl, false);
            }
            if (contents.count == 0) {
                return null;
            }
            return contents;
        }
        protected CWinCtrlBase[] createGroup(uint[] aId) {
            if (aId == null || aId.Length <= 0) {
                return null;
            }
            List<CWinCtrlBase> lstGroup = new List<CWinCtrlBase>();
            foreach (uint id in aId) {
                CWinCtrlBase cCtrl = m_cWindowBase.find(id);
                if (cCtrl == null) {
                    Debug.LogWarning("can't find ctlr:" + new MulId(id));
                    continue;
                }
                if (cCtrl == this) {
                    continue;
                }
                lstGroup.Add(cCtrl);
            }
            if (lstGroup.Count == 0) {
                return null;
            }
            return lstGroup.ToArray();
        }
        //==========================================================================
        /*!解放時に呼び出される(関連づけられているリソースがあるときはオーバライドする).
            @brief	release
        */
        public virtual void release() {
            parent = null;
            if (m_contents != null) {
                m_contents.release();
                m_contents = null;
            }
            clear();
        }
        //==========================================================================
        /*!コンテンツを全て消去する.
            @brief	clear
        */
        public void clear() {
            if (m_aContents == null) {
                return;
            }
            foreach (CWinContents cContents in m_aContents) {
                if (cContents == null) {
                    continue;
                }
                cContents.release();
            }
            m_aContents = new CWinContents[0];
        }
        //==========================================================================
        /*!コンテンツの数をリサイズする.
            @brief	resize
        */
        virtual public void resize(int count) {
            if (m_aContents == null) {
                return;
            }
            if (m_contents == null || m_contents.count == 0) {
                return;
            }
            if (count < 0) {
                return;
            }
            CWinContents[] aOld = m_aContents;
            int n = 0;
            // 配列を指定されたサイズに変更する.
            if (count < m_aContents.Length) {
                // 減る場合.
                n = count;
                for (int j = count; j < m_aContents.Length; ++j) {
                    m_aContents[j].release();
                }
                m_aContents = new CWinContents[count];
            } else if (count > m_aContents.Length) {
                // 増える場合.
                n = m_aContents.Length;
                m_aContents = new CWinContents[count];
            } else {
                return;
            }
            // コピーする.
            for (int i = 0; i < n; i++) {
                m_aContents[i] = aOld[i];
            }
            for (int i = n; i < m_aContents.Length; i++) {
                m_aContents[i] = new CWinContents(m_contents, this);
            }
        }
        //==========================================================================
        /*!コンテンツを特定の場所に追加する.
            @brief	insert
        */
        public CWinContents insert(int pos) {
            if (m_aContents == null) {
                return null;
            }
            if (m_contents == null || m_contents.count == 0) {
                return null;
            }
            if (pos < 0 || pos > m_aContents.Length) {
                return null;
            }
            CWinContents c = new CWinContents(m_contents, this);
            CWinContents[] aOld = m_aContents;
            m_aContents = new CWinContents[aOld.Length + 1];
            for (int i = 0; i < pos; i++) {
                m_aContents[i] = aOld[i];
            }
            m_aContents[pos] = c;
            for (int i = pos; i < aOld.Length; i++) {
                m_aContents[i + 1] = aOld[i];
            }
            return c;
        }
        //==========================================================================
        /*!Update
            @brief	update
        */
        public virtual void update(Vector3 pos, int priority) {
            if ((m_eUpdateFlag & e_UpdateFlag.TEXID) != 0) {
                updateTexture();
            }
            if ((m_eUpdateFlag & e_UpdateFlag.CAPTION) != 0) {
                if (m_cTextOne != null) {
                    if (m_cTextMesh == null) {
                        m_cTextMesh = window.getTextMesh(m_fontKind);
                    } else {
                        onResetCaption();
                        if ((m_eUpdateFlag & e_UpdateFlag.CAPTION) == 0) {
                            window.onUpdateCaption(this);
                        }
                    }
                }
            }
            m_absPosition = calcAbsPosition(pos, m_position);
            m_absPriority = m_priority + priority;
            // ピクセルの中心に合わせる.
            m_absPosition = KsSoftUtility.pixelPerfect(m_absPosition);
        }
        //==========================================================================
        /*!Update Texture
            @brief	update texture
        */
        protected void updateTexture() {
            e_UpdateFlag eUpdateFlag = e_UpdateFlag.TEXID0;
            for (int i = 0; i < m_aTexture.Length; i++, eUpdateFlag = (e_UpdateFlag)((int)eUpdateFlag << 1)) {
                if ((m_eUpdateFlag & eUpdateFlag) == 0) {
                    continue;
                }
                uint texid = getTextureId(i);
                if (texid != 0) {
                    if (m_aTexture[i].cWindowMesh != null) {
                        if (m_aTexture[i].cWindowMesh.isLoaded) {
                            m_eUpdateFlag &= ~eUpdateFlag;
                            onResetParts(i);
                        }
                    } else {
                        m_aTexture[i].cWindowMesh = m_cWindowBase.getWindowMesh(texid);
                    }
                } else {
                    m_eUpdateFlag &= ~eUpdateFlag;
                }
            }
        }
        virtual protected Vector3 calcAbsPosition(Vector3 basepos, RatioPair pos) {
            Vector2 sz = getParentSize();
            basepos.x += pos.x.x + pos.x.y * sz.x + pos.x.z * sz.y;
            basepos.y += pos.y.x + pos.y.y * sz.x + pos.y.z * sz.y;
            e_Anchor eAnchor = anchor;
            if (eAnchor == e_Anchor.Center || eAnchor == e_Anchor.LeftCenter || eAnchor == e_Anchor.RightCenter) {
                basepos.y += sz.y * 0.5f;
            } else if (eAnchor == e_Anchor.Bottom || eAnchor == e_Anchor.LeftBottom || eAnchor == e_Anchor.RightBottom) {
                basepos.y += sz.y;
            }
            if (eAnchor == e_Anchor.Center || eAnchor == e_Anchor.Top || eAnchor == e_Anchor.Bottom) {
                basepos.x += sz.x * 0.5f;
            } else if (eAnchor == e_Anchor.RightCenter || eAnchor == e_Anchor.RightTop || eAnchor == e_Anchor.RightBottom) {
                basepos.x += sz.x;
            }
            eAnchor = baseAnchor;
            if (eAnchor == e_Anchor.Center || eAnchor == e_Anchor.LeftCenter || eAnchor == e_Anchor.RightCenter) {
                basepos.y -= height * 0.5f;
            } else if (eAnchor == e_Anchor.Bottom || eAnchor == e_Anchor.LeftBottom || eAnchor == e_Anchor.RightBottom) {
                basepos.y -= height;
            }
            if (eAnchor == e_Anchor.Center || eAnchor == e_Anchor.Top || eAnchor == e_Anchor.Bottom) {
                basepos.x -= width * 0.5f;
            } else if (eAnchor == e_Anchor.RightCenter || eAnchor == e_Anchor.RightTop || eAnchor == e_Anchor.RightBottom) {
                basepos.x -= width;
            }
            return basepos;
        }
        //==========================================================================
        /*!Render
            @brief	render
        */
        public void render(WinColor color, ClipRect cr) {
            render(m_absPosition, windowMesh, textMesh, color, cr);
        }
        //==========================================================================
        /*!Render
            @brief	render
        */
        public virtual void render(Vector3 basepos, CWindowMesh cWinMesh, CWinTextMesh cTextMesh, WinColor color, ClipRect cr) {
        }
        //==========================================================================
        /*!ヒットチェックする.
            @brief	checkHit
        */
        public virtual CWindowMgr.CCollision checkHit(Vector2 touchPos, ClipRect cr, CStick stk) {
            if (cr != null && !cr.check(touchPos)) {
                return null;
            }
            if (isHit(touchPos)) {
                return new CWindowMgr.CCollision(this);
            }
            return null;
        }
        public virtual bool isHit(CStick stk) {
            Vector2 touchPos = window.getLocalPos(stk.windowPos);
            return isHit(touchPos);
        }
        //==========================================================================
        /*!ヒットを処理する.
            @brief	procHit
        */
        public virtual void procHit(CWindowMgr cWindowMgr, CInput.e_State eState) {
            if (eState == CInput.e_State.Release) {
                m_cWindowBase.onClick(this);
            }
        }
        //==========================================================================
        /*!ヒットを処理する(一般的ボタン用).
            @brief	procHit
        */
        protected void procHit(ref t_ProcHit rProcHit, CWindowMgr cWindowMgr, CInput.e_State eState) {
            // 押されているのでコールバックを発行.
            if (eState == CInput.e_State.Release) {
                if (!rProcHit.hold) {
                    // 離す瞬間をクリックとする.
                    m_cWindowBase.onClick(this);
                    rProcHit.hold = false;
                    play(m_SEId);
                }
            } else if (eState == CInput.e_State.On) {
                rProcHit.tmPress += Time.deltaTime;
                if (!rProcHit.hold && rProcHit.tmPress >= cWindowMgr.holdTime) {
                    // 長押しされているときはHoldコールバックを呼ぶ.
                    m_cWindowBase.onHold(this);
                    rProcHit.hold = true;
                }
                rProcHit.tap = true;
            } else if (eState == CInput.e_State.Press) {
                rProcHit.clear();
            }
            rProcHit.state = eState;
        }
        //==========================================================================
        /*!ヒットを処理する(チェックボックス/ラジオボタン用).
            @brief	procHit
        */
        protected void procHit(ref t_ProcHit rProcHit, CWindowMgr cWindowMgr, CInput.e_State eState, bool bCheck) {
            // 押されているのでコールバックを発行.
            if (eState == CInput.e_State.Release) {
                if (!rProcHit.hold) {
                    setCheckState(bCheck);
                    // 離す瞬間をクリックとする.
                    m_cWindowBase.onClick(this);
                    rProcHit.hold = false;
                    if (m_SEId != 0) {
                        play(m_SEId);
                    }
                }
            } else if (eState == CInput.e_State.On) {
                rProcHit.tmPress += Time.deltaTime;
                if (!rProcHit.hold && rProcHit.tmPress >= cWindowMgr.holdTime) {
                    // 長押しされているときはHoldコールバックを呼ぶ(チェック状態は変更しない).
                    m_cWindowBase.onHold(this);
                    rProcHit.hold = true;
                }
                rProcHit.tap = true;
            } else if (eState == CInput.e_State.Press) {
                rProcHit.hold = false;
                rProcHit.tmPress = 0f;
            }
        }
        //==========================================================================
        /*!チェック状態を変更するメソッド.
            @brief	setCheckState
        */
        virtual protected void setCheckState(bool bCheck) {
        }
        //==========================================================================
        /*!ヒットしているかチェック.
            @brief	isHit
        */
        public bool isHit(Vector2 touchPos) {
            if (m_absPosition.x <= touchPos.x && touchPos.x <= m_absPosition.x + width &&
                m_absPosition.y <= touchPos.y && touchPos.y <= m_absPosition.y + height) {
                return true;
            }
            return false;
        }
        //==========================================================================
        /*!ヒットしているかチェック.
            @brief	queryDrag
        */
        virtual public bool queryDrag(CWindowMgr cWindowMgr) {
            if (cWindowMgr.stick != null) {
                CInput.e_State eState = cWindowMgr.stick.state;
                if (eState == CInput.e_State.OnMove || eState == CInput.e_State.ReleaseMove) {
                    return true;
                }
            }
            return false;
        }
        //==========================================================================
        /*!効果音を発生する.
            @brief	play
        */
        virtual public void play(uint mSE) {
            if (mSE == 0) {
                return;
            }
            if (m_cWindowBase == null) {
                return;
            }
            m_cWindowBase.play(mSE);
        }
        //==========================================================================
        /*!コンテンツの数を取得する.
            @brief	count
        */
        virtual public int count {
            get {
                if (m_aContents == null) {
                    return 0;
                }
                return m_aContents.Length;
            }
        }
        //==========================================================================
        /*!コンテンツを取得する.
            @brief	getContents
        */
        public CWinContents getContents() {
            return m_contents;
        }
        //==========================================================================
        /*!コンテンツリストからインデックス指定で取得する.
            @brief	getContentsFromIndex
        */
        public CWinContents getContentsFromIndex(int index) {
            if (m_aContents == null) {
                return null;
            }
            if (index < 0 || index >= m_aContents.Length) {
                return null;
            }
            return m_aContents[index];
        }
        //==========================================================================
        /*!コントロールが何番目のコンテンツリストに格納されているものかチェックする.
            @brief	getContentsIndex
        */
        public int getContentsIndex(CWinCtrlBase ctrl) {
            if (ctrl == null || ctrl.parent == null) {
                return -1;
            }
            return getContentsIndex(ctrl.parent);
        }
        //==========================================================================
        /*!コンテンツが何番目のコンテンツリストに格納されているものかチェックする.
            @brief	getContentsIndex
        */
        public int getContentsIndex(CWinContents contents) {
            if (contents == null) {
                return -1;
            }
            for (int index = 0; index < m_aContents.Length; index++) {
                CWinContents cContents = m_aContents[index];
                if (cContents == contents) {
                    return index;
                }
            }
            return -1;
        }
        //==========================================================================
        /*!RenderTextureが再作成されたことが通知される.
            @brief	onRecreatedRenderTexture
        */
        virtual public void onRecreatedRenderTexture(uint mRenderTextureId) {
        }
        //==========================================================================
        /*!パーツを再更新する必要があるとき呼ばれる.
            @brief	onResetParts
        */
        virtual public void onResetParts(int iTexIndex) {
            CWindowMesh cWM = getWindowMesh(iTexIndex);
            if (cWM == null) {
                return;
            }
            m_aTexture[iTexIndex].parts = cWM.create(new FiveCC(m_aTexture[iTexIndex].partId), getTextureSize(iTexIndex));

            adjustSize(m_aTexture[iTexIndex].parts, iTexIndex);
        }
        protected void getParts2State(Parts[] aParts, int iTexIndex) {
            FiveCC partsid = new FiveCC(getPartId(iTexIndex));
            Vector2 size = getTextureSize(iTexIndex);
            CWindowMesh cWM = getWindowMesh(iTexIndex);
            if (partsid[4] == '_') {
                partsid[4] = '0';
                aParts[0] = cWM.create(partsid, size);
                partsid[4] = '1';
                aParts[1] = cWM.create(partsid, size);
            } else {
                aParts[0] = cWM.create(partsid, size);
                aParts[1] = cWM.create(partsid, size);
                if (aParts[1] != null) {
                    WinColor color = aParts[1].color;
                    color.r /= 2;
                    color.g /= 2;
                    color.b /= 2;
                    aParts[1].color = color;
                }
            }
            adjustSize(aParts[0], 0);
            adjustSize(aParts[1], 0);
        }
        protected void adjustSize(Parts parts, int iTexIndex) {
            if (parts == null) {
                return;
            }
            bool bUpdate = false;
            Vector2 size = getTextureSize(iTexIndex);
            if (size.x == 0f) {
                size.x = Mathf.Floor(parts.width);
                bUpdate = true;
            }
            if (size.y == 0f) {
                size.y = Mathf.Floor(parts.height);
                bUpdate = true;
            }
            if (bUpdate) {
                setTextureSize(iTexIndex, size);
            }
        }
        //==========================================================================
        /*!パーツを再更新する必要があるとき呼ばれる.
            @brief	resetParts
        */
        public void resetParts() {
            for (int i = 0; i < m_aTexture.Length; ++i) {
                CWindowMesh cWM = getWindowMesh(i);
                if (cWM == null) {
                    continue;
                }
                onResetParts(i);
            }
            if (m_aContents != null) {
                foreach (CWinContents cContents in m_aContents) {
                    if (cContents == null) {
                        continue;
                    }
                    cContents.resetParts();
                }
            } else if (m_contents != null) {
                m_contents.resetParts();
            }
        }
        //==========================================================================
        /*!キャプションを再更新する必要があるとき呼ばれる.
            @brief	onResetCaption
        */
        virtual public void onResetCaption() {
        }
        public void resetCaption() {
            m_eUpdateFlag |= e_UpdateFlag.CAPTION;
        }
        //==========================================================================
        /*!ウィンドウメッシュ取得.
            @brief	getWindowMesh
        */
        virtual public CWindowMesh getWindowMesh(int index) {
            if (index < 0 || index >= m_aTexture.Length) {
                return null;
            }
            CWindowMesh cWinMesh = m_aTexture[index].cWindowMesh;
            if (cWinMesh != null && cWinMesh.isLoaded) {
                return cWinMesh;
            }
            return null;
        }
        //==========================================================================
        /*!ウィンドウメッシュ設定.
            @brief	setWindowMesh
        */
        public void setWindowMesh(int index, CWindowMesh cMesh) {
            if (index < 0 || index >= m_aTexture.Length) {
                return;
            }
            m_aTexture[index].cWindowMesh = cMesh;
            return;
        }
        //==========================================================================
        /*!コントロールの親のサイズを取得する.
            @brief	getParentSize
        */
        virtual protected Vector2 getParentSize() {
            if (parent.parent == null) {
                return window.size;
            }
            if (parent.parent is CWinCtrlScrollable) {
                return parent.parent.size;
            }
            return parent.parent.getParentSize();
        }
        protected Vector2 getRatioValue(RatioPair pair) {
            Vector2 size = getParentSize();
            Vector2 res;
            res.x = pair.x.x + pair.x.y * size.x + pair.x.z * size.y;
            res.y = pair.y.x + pair.y.y * size.x + pair.y.z * size.y;
            return res;
        }
        //==========================================================================
        /*!ToString
            @brief	文字列に変換
        */
        override public string ToString() {
            return new MulId(id) + ":" + kind;
        }

        public uint id {
            get {
                return m_id;
            }
            set {
                m_id = value;
            }
        }
        public e_WinCtrlKind kind {
            get {
                return m_eKind;
            }
        }
        public uint contentsId {
            get {
                return m_contentsId;
            }
            set {
                m_contentsId = value;
            }
        }
        public int priority {
            get {
                return m_priority;
            }
            set {
                m_priority = value;
                if (m_parent != null) {
                    m_parent.sort();
                }
            }
        }
        public WinColor getColor(int index) {
            if (index >= m_aTexture.Length) {
                return WinColor.white;
            }
            return m_aTexture[index].color;
        }
        public void setColor(int index, WinColor color) {
            if (index >= m_aTexture.Length) {
            }
            m_aTexture[index].color = color;
        }
        virtual public WinColor color {
            get {
                return m_aTexture[0].color;
            }
            set {
                m_aTexture[0].color = value;
            }
        }
        virtual public WinColor color1 {
            get {
                return m_aTexture[1].color;
            }
            set {
                m_aTexture[1].color = value;
            }
        }
        virtual public WinColor color2 {
            get {
                return m_aTexture[2].color;
            }
            set {
                m_aTexture[2].color = value;
            }
        }
        virtual public WinColor color3 {
            get {
                return m_aTexture[3].color;
            }
            set {
                m_aTexture[3].color = value;
            }
        }
        virtual public WinColor color4 {
            get {
                return m_aTexture[4].color;
            }
            set {
                m_aTexture[4].color = value;
            }
        }
        virtual public WinColor color5 {
            get {
                return m_aTexture[5].color;
            }
            set {
                m_aTexture[5].color = value;
            }
        }
        virtual public WinColor color6 {
            get {
                return m_aTexture[6].color;
            }
            set {
                m_aTexture[6].color = value;
            }
        }
        virtual public WinColor color7 {
            get {
                return m_aTexture[7].color;
            }
            set {
                m_aTexture[7].color = value;
            }
        }
        virtual public WinColor captionColor {
            get {
                return m_captionColor;
            }
            set {
                m_captionColor = value;
            }
        }
        public float width {
            get {
                return getTextureWidth(0);
            }
            set {
                Vector2 sz = getTextureSize(0);
                sz.x = value;
                setTextureSize(0, sz);
            }
        }
        public float height {
            get {
                return getTextureHeight(0);
            }
            set {
                Vector2 sz = getTextureSize(0);
                sz.y = value;
                setTextureSize(0, sz);
            }
        }
        public void setSize(Vector2 sz, Vector2 ratio) {
            setTextureSize(0, sz, ratio);
        }
        public Vector2 size {
            get {
                return getTextureSize(0);
            }
            set {
                setTextureSize(0, value);
            }
        }
        public uint SEId {
            get {
                return m_SEId;
            }
            set {
                m_SEId = value;
            }
        }
        public void setPosition(RatioPair pos) {
            m_position = pos;
        }
        public void setPosition(Vector2 pos) {
            m_position.set(pos);
        }
        public Vector2 position {
            get {
                return getRatioValue(m_position);
            }
            set {
                setPosition(value);
            }
        }
        public Vector2 absPosition {
            get {
                return new Vector2(m_absPosition.x, m_absPosition.y);
            }
        }
        public Vector2 screenPosition {
            get {
                return absPosition + window.absPosition;
            }
        }
        virtual public void setContentsSize(Vector3 width, Vector3 height) {
            m_contentsSize.x = width;
            m_contentsSize.y = height;
        }
        public void setContentsSize(Vector2 sz) {
            m_contentsSize.set(sz);
        }
        public Vector2 contentsSize {
            get {
                Vector2 sz = getRatioValue(m_contentsSize);
                if (lineFeedOffset > 0f) {
                    sz.x += lineFeedOffset;
                }
                return sz;
            }
            set {
                setContentsSize(value);
            }
        }
        public void setTextureId(int index, uint texid) {
            if (index < 0 || index >= m_aTexture.Length) {
                Debug.LogError("texture index is illegal:" + index);
                return;
            }
            if (m_aTexture[index].texId == texid) {
                return;
            }
            m_aTexture[index].texId = texid;
            m_aTexture[index].cWindowMesh = null;
            m_eUpdateFlag |= (e_UpdateFlag)((int)e_UpdateFlag.TEXID0 << index);
        }
        public uint getTextureId(int index) {
            if (index < 0 || index >= m_aTexture.Length) {
                Debug.LogError("texture index is illegal:" + index);
                return 0;
            }
            uint texId = m_aTexture[index].texId;
            if (texId == 0 && m_aTexture[index].partId != 0) {
                return window.texId;
            }
            return m_aTexture[index].texId;
        }
        virtual public uint texId {
            get {
                return getTextureId(0);
            }
            set {
                setTextureId(0, value);
            }
        }
        virtual public uint texId1 {
            get {
                return getTextureId(1);
            }
            set {
                setTextureId(1, value);
            }
        }
        public void setPartId(int index, uint partid) {
            if (index < 0 || index >= m_aTexture.Length) {
                Debug.LogError("texture index is illegal:" + index);
                return;
            }
            if (m_aTexture[index].partId == partid) {
                return;
            }
            m_aTexture[index].partId = partid;
            m_eUpdateFlag |= (e_UpdateFlag)((int)e_UpdateFlag.TEXID0 << index);
        }
        public uint getPartId(int index) {
            if (index < 0 || index >= m_aTexture.Length) {
                Debug.LogError("texture index is illegal:" + index);
                return 0;
            }
            return m_aTexture[index].partId;
        }
        public Parts getParts(int index) {
            if (index < 0 || index >= m_aTexture.Length) {
                Debug.LogError("texture index is illegal:" + index);
                return null;
            }
            return m_aTexture[index].parts;
        }
        virtual public void setTextureSize(int index, Vector3 x, Vector3 y) {
            if (index < 0 || index >= m_aTexture.Length) {
                Debug.LogError("texture index is illegal:" + index);
                return;
            }
            m_aTexture[index].size.x = x;
            m_aTexture[index].size.y = y;
            m_eUpdateFlag |= (e_UpdateFlag)((int)e_UpdateFlag.TEXID0 << index);
        }
        public void setTextureSize(int index, Vector2 size) {
            if (index < 0 || index >= m_aTexture.Length) {
                Debug.LogError("texture index is illegal:" + index);
                return;
            }
            m_aTexture[index].size.set(size);
            m_eUpdateFlag |= (e_UpdateFlag)((int)e_UpdateFlag.TEXID0 << index);
        }
        virtual public Vector2 getTextureSize(int index) {
            if (index < 0 || index >= m_aTexture.Length) {
                Debug.LogError("texture index is illegal:" + index);
                return Vector2.zero;
            }
            return getRatioValue(m_aTexture[index].size);
        }
        public float getTextureWidth(int index) {
            return getTextureSize(index).x;
        }
        public float getTextureHeight(int index) {
            return getTextureSize(index).y;
        }
        virtual public uint partId {
            get {
                return m_aTexture[0].partId;
            }
            set {
                setPartId(0, value);
            }
        }
        virtual public uint partId1 {
            get {
                return m_aTexture[1].partId;
            }
            set {
                setPartId(1, value);
            }
        }
        virtual public uint partId2 {
            get {
                return m_aTexture[2].partId;
            }
            set {
                setPartId(2, value);
            }
        }
        virtual public uint partId3 {
            get {
                return m_aTexture[3].partId;
            }
            set {
                setPartId(3, value);
            }
        }
        virtual public uint partId4 {
            get {
                return m_aTexture[4].partId;
            }
            set {
                setPartId(4, value);
            }
        }
        virtual public uint partId5 {
            get {
                return m_aTexture[5].partId;
            }
            set {
                setPartId(5, value);
            }
        }
        virtual public uint partId6 {
            get {
                return m_aTexture[6].partId;
            }
            set {
                setPartId(6, value);
            }
        }
        virtual public uint partId7 {
            get {
                return m_aTexture[7].partId;
            }
            set {
                setPartId(7, value);
            }
        }
        public Parts parts {
            get {
                return m_aTexture[0].parts;
            }
        }
        public void setTextureOffset(int index, Vector2 to) {
            if (index < 0 || index >= m_aTexture.Length) {
                Debug.LogError("texture index is illegal:" + index);
                return;
            }
            m_aTexture[index].offset.set(to);
        }
        public void setTextureOffset(int index, Vector3 x, Vector3 y) {
            if (index < 0 || index >= m_aTexture.Length) {
                Debug.LogError("texture index is illegal:" + index);
                return;
            }
            m_aTexture[index].offset.x = x;
            m_aTexture[index].offset.y = y;
        }
        public Vector2 getTextureOffset(int index) {
            if (index < 0 || index >= m_aTexture.Length) {
                Debug.LogError("texture index is illegal:" + index);
                return Vector2.zero;
            }
            return getRatioValue(m_aTexture[index].offset);
        }
        virtual public CWindowMesh windowMesh {
            get {
                return m_aTexture[0].cWindowMesh;
            }
        }
        virtual public e_Anchor anchor {
            get {
                if (m_eAnchor == e_Anchor.Default) {
                    return e_Anchor.LeftTop;
                }
                return m_eAnchor;
            }
            set {
                m_eAnchor = value;
            }
        }
        virtual public e_Anchor baseAnchor {
            get {
                if (m_eBaseAnchor == e_Anchor.Default) {
                    return anchor;
                }
                return m_eBaseAnchor;
            }
            set {
                m_eBaseAnchor = value;
            }
        }
        virtual public e_Anchor textAnchor {
            get {
                return m_eTextAnchor;
            }
            set {
                m_eTextAnchor = value;
            }
        }
        public CWinTextOne.e_Style textStyle {
            get {
                return m_eTextStyle;
            }
            set {
                m_eTextStyle = value;
                resetCaption();
            }
        }
        public uint fontKind {
            get {
                return m_fontKind;
            }
            set {
                if (m_fontKind == value) {
                    return;
                }
                m_fontKind = value;
                if (m_cTextOne == null) {
                    return;
                }
                m_cTextMesh = window.getTextMesh(m_fontKind);
                if (m_cTextMesh != null) {
                    m_cTextOne.initialize(m_cTextMesh.spriteFont);
                }
                resetCaption();
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
                resetCaption();
            }
        }
        public uint captionId {
            get {
                return m_captionId;
            }
        }
        public string defaultCaption {
            get {
                return m_defaultCaption;
            }
        }
        public void setCaptionOffset(Vector3 x, Vector3 y) {
            m_captionOffset.x = x;
            m_captionOffset.y = y;
        }
        public void setCaptionOffset(Vector2 co) {
            m_captionOffset.set(co);
        }
        virtual public Vector2 captionOffset {
            get {
                return getRatioValue(m_captionOffset);
            }
            set {
                setCaptionOffset(value, Vector2.zero);
            }
        }
        public float lineSpace {
            get {
                return m_lineSpace;
            }
            set {
                m_lineSpace = value;
                resetCaption();
            }
        }
        virtual public float lineFeedWidth {
            get {
                if ((style & e_WinCtrlStyle.TEXT_AUTOSCALE) != 0) {
                    return 0f;
                }
                float w = getRatioValue(m_contentsSize).x;
                if (lineFeedOffset < 0f) {
                    w += lineFeedOffset;
                }
                return w;
            }
        }
        public float lineFeedOffset {
            get {
                return m_lineFeedOffset;
            }
            set {
                m_lineFeedOffset = value;
            }
        }
		public int focusIndex {
			get {
				if (m_aContents.Length == 0) {
					return -1;
				}
				if (m_idxFocusContents >= m_aContents.Length) {
					m_idxFocusContents = m_aContents.Length - 1;
				} else if (m_idxFocusContents < 0) {
					m_idxFocusContents = 0;
				}
				return m_idxFocusContents;
			}
			set {
				if (m_aContents.Length == 0) {
					m_idxFocusContents = -1;
					return;
				}
				m_idxFocusContents = Mathf.Clamp(value,0, m_aContents.Length - 1);
			}
		}
        public CWinContents parent {
            get {
                return m_parent;
            }
            set {
                m_parent = value;
            }
        }
        public CWinCtrlBase[] groups {
            get {
                return m_aGroup;
            }
        }
        public CWindowBase window {
            get {
                return m_cWindowBase;
            }
        }
        public CWinTextMesh textMesh {
            get {
                return m_cTextMesh;
            }
            set {
                m_cTextMesh = value;
            }
        }
        public bool isEnableToRender {
            get {
                if (m_aTexture[0].texId == 0 && m_aTexture[0].partId == 0) {
                    return true;
                }
                if (m_aTexture[0].cWindowMesh == null) {
                    return false;
                }
                return m_aTexture[0].cWindowMesh.isLoaded;
            }
        }
        public e_WinCtrlStyle style {
            get {
                return m_eStyle;
            }
            set {
                m_eStyle = value;
            }
        }
        public bool hide {
            get {
				if (parent != null && parent.hide) {
					return true;
				}
                return m_bHide;
            }
            set {
                m_bHide = value;
            }
        }
        public bool disable {
            get {
                return m_bDisable;
            }
            set {
                m_bDisable = value;
            }
        }
        virtual public bool nohit {
            get {
                return m_bNohit;
            }
            set {
                m_bNohit = value;
            }
        }
        virtual public bool isDragable {
            get {
                return ((style & e_WinCtrlStyle.DRAG) != 0) ? true : false;
            }
            set {
                if (value) {
                    style |= e_WinCtrlStyle.DRAG;
                } else {
                    style &= ~e_WinCtrlStyle.DRAG;
                }
            }
        }
        public bool bounces {
            get {
                return ((style & e_WinCtrlStyle.NOBOUNCES) == e_WinCtrlStyle.NONE) ? true : false;
            }
        }
        public bool isNeedToRender {
            get {
                return !m_bHide;
            }
        }
        public bool isNeedToCheckHit {
            get {
                return !(nohit || hide || disable);
            }
        }
        public uint uniqueId {
            get {
                if (m_cWindowBase == null) {
                    return 0;
                }
                return m_cWindowBase.uniqueId;
            }
        }
        //==========================================================================
        /*!IComparer
            @brief	IComparer
        */
        public int CompareTo(object x) {
            CWinCtrlBase obj = x as CWinCtrlBase;
            if (obj == null) {
                return 1;
            }
            if (priority == obj.priority) {
                return (int)obj.kind - (int)kind;
            }
            return priority - obj.priority;
        }

        //==============================================================================================
        /*!ウィンドウプロパティに設定する.
            @brief	set*
        */
        //==============================================================================================
        static void setId(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            t_WinProperty tProperty = new t_WinProperty();
            tProperty.read(cVariable);
            cCtrl.m_id = (uint)tProperty.m_value;
        }
        static void setCaption(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            t_WinProperty tProperty = new t_WinProperty();
            tProperty.read(cVariable);
            if (tProperty.m_value != 0) {
                cCtrl.m_captionId = (uint)tProperty.m_value;
                cCtrl.caption = cCtrl.window.getCaption(cCtrl.captionId);
                cCtrl.m_defaultCaption = cCtrl.caption;
            }
        }
        static void setCaptionStr(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            t_WinPropertyString tProperty = new t_WinPropertyString();
            tProperty.read(cVariable);
            cCtrl.caption = tProperty.m_string;
        }
        static void setTextureOffset(CWinCtrlBase cCtrl, int index, CReadVariable cVariable) {
            cCtrl.m_aTexture[index].offset.x = cVariable.getVector3();
            cCtrl.m_aTexture[index].offset.y = cVariable.getVector3();
        }
        static void setTextureOffset0(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            setTextureOffset(cCtrl, 0, cVariable);
        }
        static void setTextureOffset1(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            setTextureOffset(cCtrl, 1, cVariable);
        }
        static void setTextureOffset2(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            setTextureOffset(cCtrl, 2, cVariable);
        }
        static void setTextureOffset3(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            setTextureOffset(cCtrl, 3, cVariable);
        }
        static void setTextureOffset4(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            setTextureOffset(cCtrl, 4, cVariable);
        }
        static void setTextureOffset5(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            setTextureOffset(cCtrl, 5, cVariable);
        }
        static void setTextureOffset6(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            setTextureOffset(cCtrl, 6, cVariable);
        }
        static void setTextureOffset7(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            setTextureOffset(cCtrl, 7, cVariable);
        }
        static void setTextureSize(CWinCtrlBase cCtrl, int index, CReadVariable cVariable) {
            cCtrl.m_aTexture[index].size.x = cVariable.getVector3();
            cCtrl.m_aTexture[index].size.y = cVariable.getVector3();
        }
        static void setTextureSize1(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            setTextureSize(cCtrl, 1, cVariable);
        }
        static void setTextureSize2(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            setTextureSize(cCtrl, 2, cVariable);
        }
        static void setTextureSize3(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            setTextureSize(cCtrl, 3, cVariable);
        }
        static void setTextureSize4(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            setTextureSize(cCtrl, 4, cVariable);
        }
        static void setTextureSize5(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            setTextureSize(cCtrl, 5, cVariable);
        }
        static void setTextureSize6(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            setTextureSize(cCtrl, 6, cVariable);
        }
        static void setTextureSize7(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            setTextureSize(cCtrl, 7, cVariable);
        }
        static void setCaptionOffset(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            cCtrl.m_captionOffset.x = cVariable.getVector3();
            cCtrl.m_captionOffset.y = cVariable.getVector3();
        }
        static void setPosition(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            cCtrl.m_position.x = cVariable.getVector3();
            cCtrl.m_position.y = cVariable.getVector3();
        }
        static void setPosition2(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            cCtrl.m_position2.x = cVariable.getVector3();
            cCtrl.m_position2.y = cVariable.getVector3();
        }
        static void setSize(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            setTextureSize(cCtrl, 0, cVariable);
        }
        static void setStyle(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            t_WinProperty tProperty = new t_WinProperty();
            tProperty.read(cVariable);
            // スタイルの解消を行う.
            cCtrl.m_eStyle = (e_WinCtrlStyle)tProperty.m_value;
            // コントロールの基準位置を取得.
            cCtrl.anchor = (e_Anchor)(cCtrl.m_eStyle & e_WinCtrlStyle.ANCHOR_MASK);
            cCtrl.baseAnchor = (e_Anchor)((int)(cCtrl.m_eStyle & e_WinCtrlStyle.BASE_MASK) >> 4);
            // キャプションのアラインを設定する(ボタン、ラベル系に有効).
            switch ((cCtrl.m_eStyle & e_WinCtrlStyle.TEXT_ALIGN_MASK)) {
            case e_WinCtrlStyle.TEXT_CENTER:
                cCtrl.textAnchor = e_Anchor.Center;
                break;
            case e_WinCtrlStyle.TEXT_LEFT:
                cCtrl.textAnchor = e_Anchor.LeftCenter;
                break;
            case e_WinCtrlStyle.TEXT_RIGHT:
                cCtrl.textAnchor = e_Anchor.RightCenter;
                break;
            }
            // テキストスタイルを設定する.
            switch ((cCtrl.m_eStyle & e_WinCtrlStyle.TEXT_STYLE_MASK)) {
            case e_WinCtrlStyle.TEXT_BOLD:
                cCtrl.m_eTextStyle = CWinTextOne.e_Style.Bold;
                break;
            case e_WinCtrlStyle.TEXT_DENT:
                cCtrl.m_eTextStyle = CWinTextOne.e_Style.Dent;
                break;
            case e_WinCtrlStyle.TEXT_SHADOW:
                cCtrl.m_eTextStyle = CWinTextOne.e_Style.Shadow;
                break;
            }
            // 状態を初期化.
            if ((cCtrl.m_eStyle & e_WinCtrlStyle.HIDE) != 0) {
                cCtrl.hide = true;
            }
            if ((cCtrl.m_eStyle & e_WinCtrlStyle.DISABLE) != 0) {
                cCtrl.disable = true;
            }
            if ((cCtrl.m_eStyle & e_WinCtrlStyle.NOHIT) != 0) {
                cCtrl.nohit = true;
            }
            // HITは、初期化時のみ使用する.
            if ((cCtrl.m_eStyle & e_WinCtrlStyle.HIT) != 0) {
                cCtrl.nohit = false;
            }
        }
        static void setSEId(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            t_WinProperty tProperty = new t_WinProperty();
            tProperty.read(cVariable);
            cCtrl.m_SEId = (uint)tProperty.m_value;
        }
        static void setColor(CWinCtrlBase cCtrl, int index, CReadVariable cVariable) {
            t_WinProperty tProperty = new t_WinProperty();
            tProperty.read(cVariable);
            cCtrl.m_aTexture[index].color = new WinColor((uint)tProperty.m_value);
        }
        static void setColor0(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            setColor(cCtrl, 0, cVariable);
        }
        static void setColor1(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            setColor(cCtrl, 1, cVariable);
        }
        static void setColor2(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            setColor(cCtrl, 2, cVariable);
        }
        static void setColor3(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            setColor(cCtrl, 3, cVariable);
        }
        static void setColor4(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            setColor(cCtrl, 4, cVariable);
        }
        static void setColor5(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            setColor(cCtrl, 5, cVariable);
        }
        static void setColor6(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            setColor(cCtrl, 6, cVariable);
        }
        static void setColor7(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            setColor(cCtrl, 7, cVariable);
        }
        static void setCaptionColor(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            t_WinProperty tProperty = new t_WinProperty();
            tProperty.read(cVariable);
            cCtrl.m_captionColor = new WinColor((uint)tProperty.m_value);
        }
        static void setTexId(CWinCtrlBase cCtrl, int index, CReadVariable cVariable) {
            t_WinPropertyTexId tProperty = new t_WinPropertyTexId();
            tProperty.read(cVariable);
            if (tProperty.m_texId != 0) {
                cCtrl.m_aTexture[index].texId = tProperty.m_texId;
            }
            if (tProperty.m_partId != 0) {
                if (tProperty.m_partId == 0xffffffff) {
                    cCtrl.m_aTexture[index].partId = 0;
                } else {
                    cCtrl.m_aTexture[index].partId = tProperty.m_partId;
                }
            } else if (index > 0) {
                cCtrl.m_aTexture[index].partId = tProperty.m_partId;
            }
        }
        static void setTexId0(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            setTexId(cCtrl, 0, cVariable);
        }
        static void setTexId1(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            setTexId(cCtrl, 1, cVariable);
        }
        static void setTexId2(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            setTexId(cCtrl, 2, cVariable);
        }
        static void setTexId3(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            setTexId(cCtrl, 3, cVariable);
        }
        static void setTexId4(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            setTexId(cCtrl, 4, cVariable);
        }
        static void setTexId5(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            setTexId(cCtrl, 5, cVariable);
        }
        static void setTexId6(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            setTexId(cCtrl, 6, cVariable);
        }
        static void setTexId7(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            setTexId(cCtrl, 7, cVariable);
        }
        static void setEdit(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            t_WinPropertyTexId tProperty = new t_WinPropertyTexId();
            tProperty.read(cVariable);
            cCtrl.m_maxChar = (int)tProperty.m_texId;
            cCtrl.m_maxLine = (int)tProperty.m_partId;
        }
        static void setRelationId(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            t_WinProperty tProperty = new t_WinProperty();
            tProperty.read(cVariable);
        }
        static void setHelpId(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            t_WinProperty tProperty = new t_WinProperty();
            tProperty.read(cVariable);
        }
        static void setTooltip(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            t_WinProperty tProperty = new t_WinProperty();
            tProperty.read(cVariable);
        }
        static void setFontKind(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            t_WinProperty tProperty = new t_WinProperty();
            tProperty.read(cVariable);
            cCtrl.m_fontKind = (uint)tProperty.m_value;
        }
        static void setGroup(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            t_WinPropertyArray tProperty = new t_WinPropertyArray();
            tProperty.read(cVariable);
            cCtrl.m_aGroupId = tProperty.m_aValue;
        }
        static void setContents(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            t_WinPropertyArray tProperty = new t_WinPropertyArray();
            tProperty.read(cVariable);
            cCtrl.m_aContentsId = tProperty.m_aValue;
        }
        static void setSlideMax(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            t_WinProperty tProperty = new t_WinProperty();
            tProperty.read(cVariable);
        }
        static void setPriority(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            t_WinProperty tProperty = new t_WinProperty();
            tProperty.read(cVariable);
            cCtrl.m_priority = tProperty.m_value;
        }
        static void setLineSpace(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            t_WinProperty tProperty = new t_WinProperty();
            tProperty.read(cVariable);
            cCtrl.m_lineSpace = tProperty.m_value;
        }
        static void setLineFeedOffset(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            t_WinProperty tProperty = new t_WinProperty();
            tProperty.read(cVariable);
            cCtrl.m_lineFeedOffset = tProperty.m_value;
        }
        static void setContentsSize(CWinCtrlBase cCtrl, CReadVariable cVariable) {
            cCtrl.m_contentsSize.x = cVariable.getVector3();
            cCtrl.m_contentsSize.y = cVariable.getVector3();
        }
    }
}
