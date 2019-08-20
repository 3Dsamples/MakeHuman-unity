//==============================================================================================
/*!?リストボックスコントロール.
	@file  CWinCtrlListbox

	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KS {
    public enum e_StackOrientation {
        Vertical,
        Horizontal,
    };

    abstract public class CWinCtrlScrollable : CWinCtrlBase {
        protected Vector2 m_offset;         //表示位置基準.
        protected Vector2 m_velScroll;      //自動スクロール速度.
        protected Vector2 m_vBounces;       //領域外マージン.
        protected Vector2 m_posOldTouch;
        protected ClipRect m_clipRect = new ClipRect();
        protected Vector2 m_posSwipeStart;
        protected CWinCtrlScrollbar m_ctrlScrollbar = null;
        protected CStick m_cStick = null;
        protected WinColor m_sbColor;
        // スムーズスクロール用ワーク.
        protected Vector2 m_offsetSmoothBegin = Vector2.zero;
        protected Vector2 m_offsetSmoothEnd = Vector2.zero;
        protected float m_spdSmooth = 0.25f;
        protected int m_angSmooth = 0;      //アニメーション用.

        protected const float m_swipeMergin = 1f;
        protected const float m_dumping = 0.09f;
        protected const float m_bounceMergin = 32f;
        //==========================================================================
        /*!Constructor
            @brief	Constructor
        */
        public CWinCtrlScrollable(CWindowBase cWindow, e_WinCtrlKind eKind) : base(cWindow, eKind) {
        }
        //==========================================================================
        /*!Copy Constructor
            @brief	Copy Constructor
        */
        public CWinCtrlScrollable(CWinCtrlScrollable src, CWinContents parent) : base(src, parent) {
        }
        //==========================================================================
        /*!Initialize
            @brief	Initialize
        */
        protected override void initialize() {
            base.initialize();
            // スクロールバーを探す.
            if (m_aGroup != null) {
                for (int i = 0; i < m_aGroup.Length; i++) {
                    if (m_aGroup[i] is CWinCtrlScrollbar) {
                        CWinCtrlScrollbar cSB = m_aGroup[i] as CWinCtrlScrollbar;
                        cSB.scrollable = this;
                    }
                }
            }
            m_cStick = null;
        }
        //==========================================================================
        /*!Update
            @brief	update
        */
        override public void update(Vector3 pos, int priority) {
            base.update(pos, priority);

            updateSwipe();
            //クリップ領域の更新.
            m_clipRect.x = m_absPosition.x;
            m_clipRect.y = m_absPosition.y;
            m_clipRect.width = size.x;
            m_clipRect.height = size.y;

            CWinContents cContents = getContents();

            if (cContents != null) {
                Vector3 contentsPos = m_absPosition;
                contentsPos.x += offset.x;
                contentsPos.y += offset.y;
                // コンテンツの更新.
                updateContents(contentsPos);
            }
            updateOffset();
        }
        //==========================================================================
        /*!RenderTextureが再作成されたことが通知される.
            @brief	onRecreatedRenderTexture
        */
        override public void onRecreatedRenderTexture(uint mRenderTextureId) {
            if (m_aContents == null) {
                return;
            }
            for (int i = 0; i < m_aContents.Length; ++i) {
                CWinContents cContents = m_aContents[i];
                if (cContents != null) {
                    cContents.onRecreatedRenderTexture(mRenderTextureId);
                }
            }
        }
        //==========================================================================
        /*!mergeClipRect
            @brief	mergeClipRect
        */
        protected void mergeClipRect(ClipRect cr) {
            if (cr != null) {
                m_clipRect.x = Mathf.Max(m_absPosition.x, cr.x);
                m_clipRect.y = Mathf.Max(m_absPosition.y, cr.y);
                m_clipRect.ex = Mathf.Min(m_absPosition.x + size.x, cr.ex);
                m_clipRect.ey = Mathf.Min(m_absPosition.y + size.y, cr.ey);
            }
        }
        //==========================================================================
        /*!スワイプ処理をチェックする.
            @brief	updateSwipe
        */
        protected void updateSwipe() {
            bool bLock = false;
            if ((style & e_WinCtrlStyle.SCROLL_LOCK) == e_WinCtrlStyle.SCROLL_LOCK) {
                bLock = true;
                if (m_angSmooth != 0) {
                    m_cStick = null;
                }
            }
            if (m_cStick != null && !m_cStick.isUseListbox) {
                m_cStick = null;
            }
            if (m_cStick != null) {
                // スワイプチェック.
                switch (m_cStick.state) {
                default:
                    m_angSmooth = 0;    //アニメーションを無効にする.
                                        //離したとき、スワイプしていたかチェック.
                  {
                        bool bMove = false;
                        Vector2 mv = m_cStick.windowPos - m_posOldTouch;
                        Vector2 vOffset = offset;
                        if (Mathf.Abs(mv.x) > m_swipeMergin) {
                            vOffset.x += mv.x;
                            bMove = true;
                        }
                        if (Mathf.Abs(mv.y) > m_swipeMergin) {
                            vOffset.y += mv.y;
                            bMove = true;
                        }
                        // 慣性を保存.
                        m_velScroll = mv / Time.deltaTime;
                        m_posOldTouch = m_cStick.windowPos;
                        if (Mathf.Abs(m_velScroll.x) < m_swipeMergin) {
                            m_velScroll.x = 0f;
                        }
                        if (Mathf.Abs(m_velScroll.y) < m_swipeMergin) {
                            m_velScroll.y = 0f;
                        }
                        if (bMove) {
                            m_vBounces.x = m_bounceMergin;
                            m_vBounces.y = m_bounceMergin;
                            if (!bLock) {
                                setOffset(vOffset, m_vBounces);
                            } else {
                                if (m_angSmooth == 0) {
                                    window.onDrag(this, window.getLocalPos(m_cStick.windowPos), m_velScroll);
                                }
                            }
                        }
                    }
                    break;
                case CInput.e_State.Off:
                    goto case CInput.e_State.Release;
                case CInput.e_State.ReleaseMove:
                    goto case CInput.e_State.Release;
                case CInput.e_State.Release:
                    // スワイプしているときは入力を受け付けない.
                    m_cStick = null;
                    break;
                }
            }
            if (m_cStick == null) {
                //自動スクロール.
                if (m_angSmooth != 0) {
                    m_vBounces = Vector2.zero;
                    m_velScroll = Vector2.zero;
                    float k = Angle.LinearLerpFactor(ref m_angSmooth, m_spdSmooth);
                    Vector2 offset;
                    Vector2 offsetSave = m_offsetSmoothEnd;
                    if (m_angSmooth == 0) {
                        offset = m_offsetSmoothEnd;
                    } else {
                        offset = Vector2.Lerp(m_offsetSmoothBegin, m_offsetSmoothEnd, k);
                    }
                    setOffset(offset, m_vBounces);
                    m_offsetSmoothEnd = offsetSave;
                }
                // 慣性スクロール.
                if (m_velScroll != Vector2.zero) {
                    int btHit = 0;
                    if (!bLock) {
                        btHit = setOffset(m_offset + m_velScroll * Time.deltaTime, m_vBounces);
                    }
                    if ((btHit & 1) != 0) {
                        m_velScroll.x = 0f;
                    }
                    if ((btHit & 2) != 0) {
                        m_velScroll.y = 0f;
                    }
                    if (m_velScroll.x > 0f) {
                        m_velScroll.x -= m_velScroll.x * m_dumping;
                        if (m_velScroll.x < 0f) {
                            m_velScroll.x = 0f;
                        }
                    } else if (m_velScroll.x < 0f) {
                        m_velScroll.x -= m_velScroll.x * m_dumping;
                        if (m_velScroll.x > 0f) {
                            m_velScroll.x = 0f;
                        }
                    }
                    if (m_velScroll.y > 0f) {
                        m_velScroll.y -= m_velScroll.y * m_dumping;
                        if (m_velScroll.y < 0f) {
                            m_velScroll.y = 0f;
                        }
                    } else if (m_velScroll.y < 0f) {
                        m_velScroll.y -= m_velScroll.y * m_dumping;
                        if (m_velScroll.y > 0f) {
                            m_velScroll.y = 0f;
                        }
                    }
                    if (m_velScroll.sqrMagnitude < 1f) {
                        m_velScroll = Vector2.zero;
                    }
                }
                if (m_vBounces != Vector2.zero) {
                    // 跳ね返り処理.
                    if (m_vBounces.x > 0f && m_velScroll.x == 0) {
                        m_vBounces.x -= m_vBounces.x * m_dumping;
                        if (m_vBounces.x < 1f) {
                            m_vBounces.x = 0f;
                        }
                    }
                    if (m_vBounces.y > 0f && m_velScroll.y == 0) {
                        m_vBounces.y -= m_vBounces.y * m_dumping;
                        if (m_vBounces.y < 1f) {
                            m_vBounces.y = 0f;
                        }
                    }
                    if (!bLock) {
                        setOffset(offset, m_vBounces);
                    }
                }
            }
        }
        //==========================================================================
        /*!オフセットの更新を行う.
            @brief	updateOffset
        */
        protected void updateOffset() {
            Vector2 szView = viewsize;
            Vector2 szScreen = screensize;

            // スクロール範囲にオフセットがはいっているかチェック.
            Vector2 rg = szScreen - szView;
            if (rg.x > 0f) {
                if (m_offset.x > m_vBounces.x) {
                    m_vBounces.x = m_offset.x;
                } else if (m_offset.x < -(rg.x + m_vBounces.x)) {
                    m_vBounces.x = -(m_offset.x + rg.x);
                }
            } else {
                m_offset.x = 0f;
            }
            if (rg.y > 0f) {
                if (m_offset.y > m_vBounces.y) {
                    m_vBounces.y = m_offset.y;
                } else if (m_offset.y < -(rg.y + m_vBounces.y)) {
                    m_vBounces.y = -(m_offset.y + rg.y);
                }
            } else {
                m_offset.y = 0f;
            }
        }
        //==========================================================================
        /*!コンテンツの更新を行う.
            @brief	updateContents
        */
        abstract protected void updateContents(Vector3 pos);

		//==========================================================================
		/*!ヒットチェックする.
            @brief	checkHit
        */
		override public CWindowMgr.CCollision checkHit(Vector2 touchPos, ClipRect cr, CStick stk) {
            // リストボックス内を触ったかチェック.
            if (!isHit(touchPos)) {
                return null;
            }
            CWindowMgr.CCollision cCollision = checkHitContents(touchPos, cr, stk);
            if (cCollision == null) {
                cCollision = new CWindowMgr.CCollision(this);
            }
            Vector2 rg = screensize - viewsize;
            if (rg.x > 0f || rg.y > 0f) {
                if (!cCollision.isTouchListbox && m_cStick == null && !stk.isUseListbox) {
                    if (stk.state == CInput.e_State.Press || stk.state == CInput.e_State.On || stk.state == CInput.e_State.OnMove) {
                        m_cStick = stk;
                        m_cStick.isUseListbox = true;
                        m_posSwipeStart = stk.windowPos;
                        m_posOldTouch = stk.windowPos;
                        m_velScroll = Vector2.zero;
                        cCollision.isTouchListbox = true;
                    }
                }
            }
            return cCollision;
        }
        abstract protected CWindowMgr.CCollision checkHitContents(Vector3 touchPos, ClipRect cr, CStick stk);

        virtual public Vector2 viewsize {
            get {
                return size;
            }
        }
        abstract public Vector2 screensize {
            get;
        }
        override public void setTextureSize(int index, Vector3 x, Vector3 y) {
            base.setTextureSize(index, x, y);
            offset = m_offset;
        }
        public Vector2 offset {
            get {
                return m_offset;
            }
            set {
                setOffset(value, Vector2.zero);
                m_angSmooth = 0;
            }
        }
        protected int setOffset(Vector2 value, Vector2 vBounces) {
            Vector2 rg = screensize - viewsize;
            int btHit = 0;
            if (rg.x > 0f) {
                if (isLoop && stackOrientation == e_StackOrientation.Horizontal) {
                    m_offset.x = value.x;
                    if (m_offset.x < -screensize.x) {
                        m_offset.x += screensize.x;
                    } else if (m_offset.x > 0f) {
                        m_offset.x -= screensize.x;
                    }
                } else if (value.x > vBounces.x) {
                    m_offset.x = vBounces.x;
                    btHit |= 1;
                } else if (value.x < -(rg.x + vBounces.x)) {
                    m_offset.x = -(rg.x + vBounces.x);
                    btHit |= 1;
                } else {
                    m_offset.x = value.x;
                }
            } else {
                m_offset.x = 0f;
            }
            if (rg.y > 0f) {
                if (isLoop && stackOrientation == e_StackOrientation.Vertical) {
                    m_offset.y = value.y;
                    if (m_offset.y < -screensize.y) {
                        m_offset.y += screensize.y;
                    } else if (m_offset.y > 0f) {
                        m_offset.y -= screensize.y;
                    }
                } else if (value.y > vBounces.y) {
                    m_offset.y = vBounces.y;
                    btHit |= 2;
                } else if (value.y < -(rg.y + vBounces.y)) {
                    m_offset.y = -(rg.y + vBounces.y);
                    btHit |= 2;
                } else {
                    m_offset.y = value.y;
                }
            } else {
                m_offset.y = 0f;
            }
            m_offsetSmoothEnd = m_offset;
            return btHit;
        }
        public bool isSwipe {
            get {
                if (m_cStick == null) {
                    return (m_velScroll != Vector2.zero) ? true : false;
                }
                return true;
            }
        }
        public bool isMove {
            get {
                if (m_velScroll != Vector2.zero || m_vBounces != Vector2.zero || isSmoothScrolled) {
                    return true;
                }
                return false;
            }
        }
        public e_StackOrientation stackOrientation {
            get {
                if ((style & e_WinCtrlStyle.ITEM_STACK_H) != 0) {
                    return e_StackOrientation.Horizontal;
                }
                return e_StackOrientation.Vertical;
            }
        }
        protected bool isLoop {
            get {
                if ((style & e_WinCtrlStyle.SCROLL_LOOP) != 0) {
                    return true;
                }
                return false;
            }
        }
        public void setSmoothOffset(Vector2 offset, float spd) {
			//動くかどうか判定する
			//動けるだけ自動スクロール値を設定する
			m_offsetSmoothBegin = m_offset;
            m_offsetSmoothEnd = offset;
            m_spdSmooth = spd;
            m_angSmooth = Angle.InitLerpFactor();
        }
        public Vector2 smoothOffset {
            get {
                return m_offsetSmoothEnd;
            }
            set {
				if (value != m_offsetSmoothEnd) {
					setSmoothOffset(value, 0.25f);
				}
			}
        }
        public bool isSmoothScrolled {
            get {
                if (m_angSmooth != 0) {
                    return true;
                }
                return false;
            }
        }
        public override bool isDragable {
            get {
                return false;
            }
        }
		public ClipRect	clipRect {
			get {
				return m_clipRect;
			}
		}
    }
}
