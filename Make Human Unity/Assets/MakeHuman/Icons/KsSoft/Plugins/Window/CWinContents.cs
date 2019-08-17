//==============================================================================================
/*!?コンテンツコントロール.
	@file  CWinContents

	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace KS {
    public class CWinContents {
        CWinCtrlBase m_parent;
        uint m_id;  //固有ID,使う場合は、外部から設定する必要がある(Uniqueの保障も設定する側がする必要がある).
        CWinCtrlBase[] m_aCtrl = new CWinCtrlBase[0];
        Vector3 m_position;
        ClipRect.State m_clipState = ClipRect.State.Inside;
        bool m_bSort = false;
        bool m_bHide = false;
        bool m_bNohit = false;
        bool m_bDisable = false;
        const float m_disableColorRate = 0.7f;
        //==========================================================================
        /*!Constructor
            @brief	Constructor
        */
        public CWinContents(CWinCtrlBase parent) {
            m_parent = parent;
        }
        //==========================================================================
        /*!Constructor
            @brief	Constructor
        */
        public CWinContents(CWinContents src, CWinCtrlBase parent) {
            if (parent == null) {
                m_parent = src.m_parent;
            } else {
                m_parent = parent;
            }
            m_position = src.m_position;
            m_clipState = src.m_clipState;

            m_bSort = src.m_bSort;
            m_bHide = src.m_bHide;
            m_bNohit = src.m_bNohit;
            m_bDisable = src.m_bDisable;

            m_aCtrl = new CWinCtrlBase[src.m_aCtrl.Length];
            for (int i = 0; i < m_aCtrl.Length; i++) {
                CWinCtrlBase ctrl = CWinFactory.copyCtrl(src.m_aCtrl[i], this);
                ctrl.contentsId = ctrl.uniqueId;
                m_aCtrl[i] = ctrl;
            }
        }
        //==========================================================================
        /*!doSort
            @brief	doSort
        */
        void doSort() {
            Array.Sort(m_aCtrl);
            m_bSort = false;
        }
        //==========================================================================
        /*!update
            @brief	update
        */
        public void update(Vector3 pos, int priority) {
            if (m_bSort) {
                doSort();
            }
            m_position = pos;
            foreach (CWinCtrlBase cCtrl in m_aCtrl) {
                cCtrl.update(pos, priority);
            }
        }
        //==========================================================================
        /*!render
            @brief	render
        */
        public void render(WinColor color, ClipRect cr) {
            if (!isNeedToRender) {
                return;
            }
            WinColor disableColor = color * m_disableColorRate;

            foreach (CWinCtrlBase cCtrl in m_aCtrl) {
                if (!cCtrl.isEnableToRender) {
                    continue;
                }
                if (cCtrl.isNeedToRender) {
                    if (disable || cCtrl.disable) {
                        cCtrl.render(disableColor, cr);
                    } else {
                        cCtrl.render(color, cr);
                    }
                }
            }
        }
        //==========================================================================
        /*!resetParts
            @brief	Reset Parts
            @note	パーツをリセットする.
        */
        public void resetParts() {
            foreach (CWinCtrlBase cCtrl in m_aCtrl) {
                cCtrl.resetParts();
            }
        }
        //==========================================================================
        /*!RenderTextureが再作成されたことが通知される.
            @brief	onRecreatedRenderTexture
        */
        public void onRecreatedRenderTexture(uint mRenderTextureId) {
            foreach (CWinCtrlBase cCtrl in m_aCtrl) {
                cCtrl.onRecreatedRenderTexture(mRenderTextureId);
            }
        }
        //==========================================================================
        /*!release.
            @brief	release
        */
        public void release() {
            for (int i = 0; i < m_aCtrl.Length; i++) {
                m_aCtrl[i].release();
            }
        }
        //==========================================================================
        /*!ヒットチェックする.
            @brief	checkHit
        */
        public CWindowMgr.CCollision checkHit(Vector2 touchPos, ClipRect cr, CStick stk) {
            if (!isNeedToCheckHit) {
                return null;
            }
            for (int i = m_aCtrl.Length - 1; i >= 0; --i) {
                if (m_aCtrl[i].isNeedToCheckHit) {
                    CWindowMgr.CCollision cCollision = m_aCtrl[i].checkHit(touchPos, cr, stk);
                    if (cCollision != null) {
                        return cCollision;
                    }
                }
            }
            return null;
        }
        //==========================================================================
        /*!append
            @brief	append
        */
        public bool append(CWinCtrlBase cCtrl) {
            Array.Resize(ref m_aCtrl, m_aCtrl.Length + 1);
            m_aCtrl[m_aCtrl.Length - 1] = cCtrl;
            cCtrl.parent = this;
            m_bSort = true;
            return true;
        }
        //==========================================================================
        /*!remove
            @brief	remove
        */
        public void remove(CWinCtrlBase cCtrl, bool bRelease) {
            List<CWinCtrlBase> lstCtrl = new List<CWinCtrlBase>(m_aCtrl);
            lstCtrl.Remove(cCtrl);
            if (bRelease) {
                cCtrl.release();
            }
            m_aCtrl = lstCtrl.ToArray();
        }
        //==========================================================================
        /*!CtrlIdを使って削除する.
            @brief	remove
        */
        public bool remove(uint mCtrlId, bool bRelease) {
            foreach (CWinCtrlBase ctrl in m_aCtrl) {
                if (ctrl.id == mCtrlId) {
                    remove(ctrl, bRelease);
                    return true;
                }
            }
            return false;
        }

        //==========================================================================
        /*!find
            @brief	find
        */
        public virtual WinCtrl find<WinCtrl>(uint mId, uint contentsId = 0) where WinCtrl : CWinCtrlBase {
            CWinCtrlBase cCtrl = find(mId, contentsId);
            return cCtrl as WinCtrl;
        }
        public CWinCtrlBase find(uint mId, uint contentsId = 0) {
            if (m_aCtrl.Length == 0) {
                return null;
            }
            if (contentsId == 0) {
                foreach (CWinCtrlBase ctrl in m_aCtrl) {
                    if (ctrl.id == mId) {
                        return ctrl;
                    }
                    CWinContents con = ctrl.getContents();
                    if (con != null) {
                        CWinCtrlBase cFind = con.find(mId, 0);
                        if (cFind != null) {
                            return cFind;
                        }
                    }
                }
            } else {
                foreach (CWinCtrlBase ctrl in m_aCtrl) {
                    if (ctrl.id == mId && ctrl.contentsId == contentsId) {
                        return ctrl;
                    }
                    int count = ctrl.count;
                    for (int i = 0; i < count; i++) {
                        CWinContents cContents = ctrl.getContentsFromIndex(i);
                        if (cContents != null) {
                            CWinCtrlBase cFind = cContents.find(mId, contentsId);
                            if (cFind != null) {
                                return cFind;
                            }
                        }
                    }
                }
            }
            return null;
        }
        //==========================================================================
        /*!sort.
            @brief	sort
        */
        public void sort() {
            m_bSort = true;
        }
        //==========================================================================
        /*!特定のコントロールのインデックスを取得する.
            @brief	getIndex
        */
        public int getIndex(uint ctrlId) {
            for (int i = 0; i < m_aCtrl.Length; ++i) {
                if (m_aCtrl[i].id == ctrlId) {
                    return i;
                }
            }
            return -1;
        }
        //==========================================================================
        /*!インデクサ.
            @brief	[]
        */
        public CWinCtrlBase this[int i] {
            get {
                if (i < 0 || i >= m_aCtrl.Length) {
                    return null;
                }
                return m_aCtrl[i];
            }
        }
        //==========================================================================
        /*!count
            @brief	count
        */
        public int count {
            get {
                return m_aCtrl.Length;
            }
        }
        //==========================================================================
        /*!クリップの状態を取得.
            @brief	clipState
        */
        public ClipRect.State clipState {
            get {
                return m_clipState;
            }
            set {
                m_clipState = value;
            }
        }
        public float width {
            get {
                float w = 0f;
                for (int i = 0; i < m_aCtrl.Length; ++i) {
                    CWinCtrlBase cCtrl = m_aCtrl[i];
                    if (cCtrl.isNeedToRender) {
                        w = Mathf.Max(w, cCtrl.width + cCtrl.position.x);
                    }
                }
                return w;
            }
        }
        public float height {
            get {
                float h = 0f;
                for (int i = 0; i < m_aCtrl.Length; ++i) {
                    CWinCtrlBase cCtrl = m_aCtrl[i];
                    if (cCtrl.isNeedToRender) {
                        h = Mathf.Max(h, cCtrl.height + cCtrl.position.y);
                    }
                }
                return h;
            }
        }
        public Vector2 size {
            get {
                Vector2 s;
                s.x = 0f;
                s.y = 0f;
                for (int i = 0; i < m_aCtrl.Length; ++i) {
                    CWinCtrlBase cCtrl = m_aCtrl[i];
                    if (cCtrl.isNeedToRender) {
                        s.x = Mathf.Max(s.x, cCtrl.width + cCtrl.position.x);
                        s.y = Mathf.Max(s.y, cCtrl.height + cCtrl.position.y);
                    }
                }
                return s;
            }
        }
        public Vector3 position {
            get {
                return m_position;
            }
        }
        public uint id {
            get {
                return m_id;
            }
            set {
                m_id = value;
            }
        }
        public bool disableRecursive {
            set {
                for (int i = 0; i < m_aCtrl.Length; ++i) {
                    CWinCtrlBase cCtrl = m_aCtrl[i];
                    cCtrl.disable = value;
                    if (cCtrl.count > 0) {
                        for (int idx = 0; idx < cCtrl.count; ++idx) {
                            cCtrl.getContentsFromIndex(idx).disableRecursive = value;
                        }
                    } else if (cCtrl.getContents() != null) {
                        cCtrl.getContents().disableRecursive = value;
                    }
                }
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
        public bool nohit {
            get {
                return m_bNohit;
            }
            set {
                m_bNohit = value;
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
        public CWinCtrlBase parent {
            get {
                return m_parent;
            }
        }
    }
}
