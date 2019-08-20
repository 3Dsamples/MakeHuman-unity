//==============================================================================================
/*!CStick
	@file  CStick
	@brief	入力の一つの情報を保持する.
*/
//==============================================================================================
using UnityEngine;
using System.Collections;

namespace KS {
    public class CStick {
        public const int Unuse = -1;
        int m_iId;
        Vector2 m_startPos;
        Vector2 m_windowStartPos;
        Vector2 m_oldPos;
        Vector2 m_windowPos;
        Vector2 m_pos;
        Vector2 m_axisRelative; //単位はmmで返す.
        Vector2 m_axisAbsolute; //単位はmmで返す.
        float m_inputTime = 0.0f;
        float m_startTime = 0.0f;
        CInput.e_State m_eState;
        bool m_isUseWindow = false;
        bool m_isUseUser = false;
        bool m_isUseListbox = false;
        bool m_isMoved = false;
        float m_dotPerMM;
        Vector2 m_magDPI;
        public const float marginSq = 8f * 8f;
        public const float fps = 60f;
        public const float sensitiveSq = (6f) * (6f);

        //==========================================================================
        /*!clear
            @brief	Clear
        */
        public void clear() {
            m_dotPerMM = Screen.dpi / 25.4f;    //1mmに対してのドット数.
#if UNITY_EDITOR
            m_magDPI.x = Screen.width / m_dotPerMM;
            m_magDPI.y = Screen.height / m_dotPerMM;
#else
		m_magDPI.x = KsSoftUtility.defaultResolution.x / m_dotPerMM;
		m_magDPI.y = KsSoftUtility.defaultResolution.y / m_dotPerMM;
#endif
            m_iId = Unuse;
            m_startPos = Vector2.zero;
            m_pos = Vector2.zero;
            m_axisRelative = Vector2.zero;
            m_axisAbsolute = Vector2.zero;
            m_inputTime = 0.0f;
            m_startTime = 0.0f;
            m_eState = CInput.e_State.Off;
            m_isUseWindow = false;
            m_isUseListbox = false;
            m_isUseUser = false;
            m_isMoved = false;
        }
        //==========================================================================
        /*!initialize
            @brief	initialize
        */
        public void initialize(int iId, Vector2 startPos, Vector2 windowPos) {
            m_iId = iId;
            m_axisRelative = Vector2.zero;
            m_axisAbsolute = Vector2.zero;
            m_startPos = startPos;
            m_pos = m_startPos;
            m_windowStartPos = windowPos;
            m_inputTime = 0.0f;
            m_startTime = Time.time;
            m_eState = CInput.e_State.Off;
            m_isUseWindow = false;
            m_isUseListbox = false;
            m_isUseUser = false;
            m_isMoved = false;
        }
        //==========================================================================
        /*!update
            @brief	update
        */
        public void update(Vector2 touchPos, Vector2 windowPos, CInput.e_Type eType) {
            float dtTm = Time.deltaTime;
            m_inputTime += dtTm;

            Vector2 d = Vector2.Scale(touchPos - m_startPos, m_magDPI);
            if (d.sqrMagnitude >= marginSq) {
                m_isMoved = true;
            }
            m_windowPos = windowPos;
            m_oldPos = m_pos;
            m_axisRelative = Vector2.Scale(touchPos - m_oldPos, m_magDPI);
            switch (eType) {
            case CInput.e_Type.RelativeCurrent:
                if (m_axisRelative.sqrMagnitude >= sensitiveSq && Vector2.Angle(m_axisAbsolute, m_axisRelative) > 20f) {
                    m_axisAbsolute = m_axisRelative.normalized * d.magnitude;
                    d.x = m_axisAbsolute.x / m_magDPI.x;
                    d.y = m_axisAbsolute.y / m_magDPI.y;
                    m_startPos = touchPos - d;
                } else {
                    m_axisAbsolute = d;
                }
                break;
            case CInput.e_Type.RelativeStart:
                m_axisAbsolute = d;
                break;
            }
            m_pos = touchPos;
        }
        static public float ddd = 1f;
        public int id {
            get {
                return m_iId;
            }
        }
        public float startTime {
            get {
                return m_startTime;
            }
        }
        public float inputTime {
            get {
                return m_inputTime;
            }
        }
        public Vector2 axisRelative {
            get {
                return m_axisRelative;
            }
        }
        public Vector2 axisAbsolute {
            get {
                return m_axisAbsolute;
            }
        }
        public Vector2 touchPos {
            get {
                return m_pos;
            }
        }
        public Vector2 touchOldPos {
            get {
                return m_oldPos;
            }
        }
        public Vector2 windowPos {
            get {
                return m_windowPos;
            }
        }
        public Vector2 startPos {
            get {
                return m_startPos;
            }
        }
        public Vector2 windowStartPos {
            get {
                return m_windowStartPos;
            }
        }
        public bool isMoved {
            get {
                return m_isMoved;
            }
        }
        public CInput.e_State state {
            get {
                return m_eState;
            }
            set {
                m_eState = value;
            }
        }
        public bool isUseWindow {
            get {
                return m_isUseWindow;
            }
            set {
                m_isUseWindow = value; ;
            }
        }
        public bool isUseListbox {
            get {
                return m_isUseListbox;
            }
            set {
                m_isUseListbox = value; ;
            }
        }
        public bool isUseUser {
            get {
                return m_isUseUser;
            }
            set {
                m_isUseUser = value;
            }
        }
        public bool isUnused {
            get {
                if (m_iId == CStick.Unuse) {
                    return true;
                }
                return false;
            }
        }
    }
}
