//==============================================================================================
/*!?スクロールバー.
	@file  CWinCtrlScrollbar
	
	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KS {
    public class CWinCtrlScrollbar : CWinCtrlBase {
        protected CWinCtrlScrollable m_ctrlScrollable = null;
        protected int m_index;  //伸縮するテクスチャインデックス番号.
        protected Vector2 m_viewsize;
        protected Vector2 m_offset;
        protected Vector2[] m_aOldSize = new Vector2[nTexture];
        protected int m_angFade = 0;
        protected float m_fadeSpeed = 0.5f;
        //==========================================================================
        /*!Constructor
            @brief	Constructor
        */
        public CWinCtrlScrollbar(CWindowBase cWindow) : base(cWindow, e_WinCtrlKind.SCROLLBAR) {
            m_aTexture[0].partId = FiveCC.Id("SCBRH");
            m_index = 0;
        }
        //==========================================================================
        /*!Copy Constructor
            @brief	Copy Constructor
        */
        public CWinCtrlScrollbar(CWinCtrlScrollbar src, CWinContents parent) : base(src, parent) {
            m_ctrlScrollable = src.m_ctrlScrollable;
            m_index = src.m_index;
        }
        //==========================================================================
        /*!Update
            @brief	update
        */
        override public void update(Vector3 pos, int priority) {
            if (m_ctrlScrollable != null) {
                pos = m_ctrlScrollable.absPosition;
            }
            base.update(pos, priority);
        }
        //==========================================================================
        /*!コントロールの親のサイズを取得する.
            @brief	getParentSize
        */
        override protected Vector2 getParentSize() {
            if (m_ctrlScrollable != null) {
                return m_ctrlScrollable.size;
            }
            return base.getParentSize();
        }
        //==========================================================================
        /*!Render
            @brief	render
        */
        override public void render(Vector3 pos, CWindowMesh cWinMesh, CWinTextMesh cTextMesh, WinColor rtcolor, ClipRect cr) {
            WinColor col;
            int iRes = check(pos);
            if (iRes < 0) {
                return;
            }
            e_WinCtrlStyle eDisplay = style & e_WinCtrlStyle.SCROLLBAR_DISPLAY_MASK;
            float k = 0f;
            if (eDisplay == e_WinCtrlStyle.SCROLLBAR_DISPLAY_SCROLLABLE) {
                if (iRes == 0) {
                    if (m_angFade == 0) {
                        return;
                    }
                    k = Angle.LerpFactor(ref m_angFade, fadeSpeed);
                } else {
                    m_angFade = Angle.InitLerpFactor();
                }
            } else if (eDisplay == e_WinCtrlStyle.SCROLLBAR_DISPLAY_NORMAL) {
                if (iRes != 2) {
                    if (m_angFade == 0) {
                        return;
                    }
                    k = Angle.LerpFactor(ref m_angFade, fadeSpeed);
                } else {
                    m_angFade = Angle.InitLerpFactor();
                }
            } else {
                m_angFade = Angle.InitLerpFactor();
            }
            rtcolor.a = (byte)((float)rtcolor.a * (1f - k));
            for (int i = 0; i < m_aTexture.Length; i++) {
                if (m_aTexture[i].partId != 0) {
                    CWindowMesh cMesh = getWindowMesh(i);
                    if (cMesh == null) {
                        continue;
                    }
                    if (m_index != i && m_aOldSize[i] != getTextureSize(i)) {
                        onResetParts(i);
                    }
                    col = rtcolor * m_aTexture[i].color;
                    Vector3 texpos = pos;
                    Vector2 offset = getRatioValue(m_aTexture[i].offset);
                    texpos.x += offset.x;
                    texpos.y += offset.y;
                    if (m_index == i) {
                        texpos.x += m_offset.x;
                        texpos.y += m_offset.y;
                    }
                    cMesh.request(m_aTexture[i].parts, texpos, col, 0, cr);
                }
            }
        }
        //==========================================================================
        /*!ヒットを処理する.
            @brief	procHit
        */
        public override void procHit(CWindowMgr cWindowMgr, CInput.e_State eState) {
        }
        //==========================================================================
        /*!スクロールバーの更新を行う.
            @brief	render
        */
        protected int check(Vector3 pos) {
            CWinCtrlScrollable cScrollable = m_ctrlScrollable;
            if (cScrollable == null) {
                return -1;
            }
            Parts parts = m_aTexture[m_index].parts;
            if (parts == null) {
                return -1;
            }
            Vector2 szView = cScrollable.viewsize;
            Vector2 szScreen = cScrollable.screensize;
            Vector2 size = getTextureSize(m_index);

            Vector2 oldSize = m_viewsize;
            bool bSmallArea = false;
            if (stackOrientation == e_StackOrientation.Vertical) {
                m_viewsize.x = size.x;
                if (szView.y >= szScreen.y) {
                    m_viewsize.y = size.y;
                    m_offset = Vector2.zero;
                    bSmallArea = true;
                } else {
                    m_viewsize.y = Mathf.Max(parts.minheight, size.y * szView.y / szScreen.y);
                    m_offset = new Vector2(0f, (m_viewsize.y - size.y) * cScrollable.offset.y / (szScreen.y - szView.y));
                }
            } else {
                m_viewsize.y = size.y;
                if (szView.x >= szScreen.x) {
                    m_viewsize.x = size.x;
                    m_offset = Vector2.zero;
                    bSmallArea = true;
                } else {
                    m_viewsize.x = Mathf.Max(parts.minwidth, size.x * szView.x / szScreen.x);
                    m_offset = new Vector2((m_viewsize.x - size.x) * cScrollable.offset.x / (szScreen.x - szView.x), 0f);
                }
            }
            if (oldSize != m_viewsize) {
                onResetParts(m_index);
            }
            if (bSmallArea) {
                return 0;
            }
            if (!cScrollable.isMove) {
                return 1;
            }
            return 2;
        }
        //==========================================================================
        /*!パーツを再更新する必要があるとき呼ばれる.
            @brief	onResetParts
        */
        override public void onResetParts(int iTexIndex) {
            if (m_index != iTexIndex) {
                base.onResetParts(iTexIndex);
                m_aOldSize[iTexIndex] = getTextureSize(iTexIndex);
                return;
            }
            CWindowMesh cWM = getWindowMesh(iTexIndex);
            if (cWM == null) {
                return;
            }
            m_aTexture[iTexIndex].parts = cWM.create(new FiveCC(m_aTexture[iTexIndex].partId), m_viewsize);

            m_aOldSize[iTexIndex] = m_viewsize;
        }
        public CWinCtrlScrollable scrollable {
            set {
                m_ctrlScrollable = value;
            }
        }
        protected e_StackOrientation stackOrientation {
            get {
                if ((style & e_WinCtrlStyle.ITEM_STACK_H) != 0) {
                    return e_StackOrientation.Horizontal;
                }
                return e_StackOrientation.Vertical;
            }
        }
        public override bool isDragable {
            get {
                return false;
            }
        }
        public override bool nohit {
            get {
                return true;
            }
            set {
            }
        }
        override public Vector2 getTextureSize(int index) {
            if (m_ctrlScrollable == null) {
                return Vector2.zero;
            }
            if (index < 0 || index >= m_aTexture.Length) {
                Debug.LogError("texture index is illegal:" + index);
                return Vector2.zero;
            }
            Vector2 sz = getRatioValue(m_aTexture[index].size);
            switch (stackOrientation) {
            case e_StackOrientation.Vertical:
                sz.y += m_ctrlScrollable.height;
                break;
            case e_StackOrientation.Horizontal:
                sz.x += m_ctrlScrollable.width;
                break;
            }
            return sz;
        }
        public float fadeSpeed {
            get {
                return m_fadeSpeed;
            }
            set {
                m_fadeSpeed = value;
            }
        }
        public int index {
            get {
                return m_index;
            }
            set {
                m_index = value;
            }
        }
    }
}
