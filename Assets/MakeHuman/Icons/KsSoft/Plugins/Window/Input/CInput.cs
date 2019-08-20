//==============================================================================================
/*!CInput
	@file  CInput
	@brief	複数の入力を保持し管理する.
*/
//==============================================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KS {
    public class CInput : MonoBehaviour {
        public enum e_State {
            Press,          //押した瞬間.
            On,             //押している.
            OnMove,         //押しているが、移動している.
            Release,        //移動せずに離した.
            ReleaseMove,    //移動して離した.
            Off,
        };
        public enum e_Type {
            RelativeCurrent,    //現在の位置からの相対.
            RelativeStart,      //タッチ開始からの相対.
        };
        e_Type m_eType = e_Type.RelativeStart;

        const int MultiTouchMax = 4;
        CStick[] m_aStick = new CStick[MultiTouchMax];
        //==========================================================================
        /*!Awake
            @brief	Unity Callback
        */
        void Awake() {
            if (m_instance != null) {
                Debug.LogError("already exist input behaviour");
                return;
            }
            m_instance = this;
            for (int i = 0; i < m_aStick.Length; ++i) {
                m_aStick[i] = new CStick();
            }
            for (int i = 0; i < m_aStick.Length; ++i) {
                m_aStick[i].clear();
            }
        }
        //==========================================================================
        /*!Update
            @brief	Unity Callback
        */
        void Update() {
            int iCount = 0;
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
            if (Input.GetMouseButton(0) || Input.GetMouseButtonUp(0)) {
                iCount = 1;
            }
#else
		iCount = Input.touchCount;
#endif
            // 前回フレームでリリース状態のものはクリアする.
            for (int i = 0; i < m_aStick.Length; ++i) {
                CStick cStick = m_aStick[i];
                if (cStick.state == CInput.e_State.Release || cStick.state == CInput.e_State.ReleaseMove) {
                    cStick.clear();
                }
            }
            // 各タッチを検討し情報を取得する.
            if (iCount > 0) {
                int iFingerId = CStick.Unuse;
                for (int i = 0; i < iCount; i++) {
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
                    iFingerId = 0;
                    Vector2 vTouchPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
#else
				Touch cTouch = Input.GetTouch(i);
				Vector2	vTouchPos = cTouch.position;
				iFingerId = cTouch.fingerId;

#endif
                    vTouchPos.x = 2.0f * vTouchPos.x / CWindowMgr.screenWidth - 1.0f;
                    vTouchPos.y = 2.0f * vTouchPos.y / CWindowMgr.screenHeight - 1.0f;

                    Vector2 vWindowTouchPos; ;
                    vWindowTouchPos.x = vTouchPos.x * CWindowMgr.width * 0.5f;
                    vWindowTouchPos.y = vTouchPos.y * CWindowMgr.height * -0.5f;

                    CStick cStick = find(iFingerId);
                    if (cStick == null) {
                        cStick = create(iFingerId, vTouchPos, vWindowTouchPos);
                        if (cStick == null) {
                            continue;
                        }
                    }
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
                    if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) {
                        cStick.state = e_State.Press;
                    } else {
                        if (cStick.state != e_State.OnMove) {
                            if (cStick.isMoved) {
                                cStick.state = e_State.OnMove;
                            } else {
                                cStick.state = e_State.On;
                            }
                        }
                    }
                    if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)) {
                        if (cStick.state == e_State.OnMove) {
                            cStick.state = e_State.ReleaseMove;
                        } else {
                            cStick.state = e_State.Release;
                        }
                    }
#else
				switch (cTouch.phase) {
				  case TouchPhase.Began:
					cStick.state = e_State.Press;
					break;
				  case TouchPhase.Stationary:
					if (cStick.state == e_State.Press) {
						cStick.state = e_State.On;
					}
					break;
				  case TouchPhase.Moved:
					if (cStick.isMoved) {
						cStick.state = e_State.OnMove;
					}
					break;
				  case TouchPhase.Canceled:
					goto case TouchPhase.Ended;
				  case TouchPhase.Ended:
					if (cStick.state == e_State.OnMove) {
						cStick.state = e_State.ReleaseMove;
					} else {
						if ((vTouchPos - cStick.startPos).sqrMagnitude > CStick.marginSq) {
							cStick.state = e_State.ReleaseMove;
						} else {
							cStick.state = e_State.Release;
						}
					}
					break;
				}
#endif
                    cStick.update(vTouchPos, vWindowTouchPos, m_eType);
                }
            }
        }
        CStick create(int iFingerId, Vector2 touchPos, Vector2 screenTouchPos) {
            CStick cStick = find(CStick.Unuse);
            if (cStick != null) {
                cStick.initialize(iFingerId, touchPos, screenTouchPos);
                return cStick;
            }
            return null;
        }
        public CStick find(int iFingerId) {
            for (int i = 0; i < m_aStick.Length; ++i) {
                if (m_aStick[i].id == iFingerId) {
                    return m_aStick[i];
                }
            }
            return null;
        }
        public CStick[] sticks {
            get {
                return m_aStick;
            }
        }
        public e_Type type {
            get {
                return m_eType;
            }
            set {
                m_eType = value;
            }
        }
        static CInput m_instance = null;
        static public CInput Instance {
            get {
                return m_instance;
            }
        }
    }
}
