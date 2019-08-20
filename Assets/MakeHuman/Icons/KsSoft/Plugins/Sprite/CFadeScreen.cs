//==============================================================================================
/*!フェードイン/アウト.
	@file  CFadeScreen
	
	(counter SJIS string ??)
*/
//==============================================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KS {
    public class CFadeScreen : CBG {
        bool m_bIsFade = false;
        Color m_colFade;
        //==========================================================================
        /*!Awake
            @brief	Unity Callback
        */
        new void Awake() {
            m_depth = 2f;
            m_eLayerId = e_LayerId.Fade;
            base.Awake();

            gameObject.SetActive(false);
        }
        //==========================================================================
        /*!Start
            @brief	Unity Callback
        */
        IEnumerator Start() {
            Vector3 pos = transform.position;
            pos.z = -100f;
            transform.position = pos;
            //------------------------------------------------------------
            // 消されないように設定.
            while (CMainSystemBase.Instance == null) {
                yield return 0;
            }
            CMainSystemBase.Instance.regist(gameObject);
        }
        //==========================================================================
        /*!フェードインを開始する.
            @brief	startFadeIn
        */
        public bool startFadeIn(float spd = 1.0f) {
            gameObject.SetActive(true);
            m_colFade = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            if (m_bIsFade) {
                StopCoroutine("doFadeCR");
            }
            m_bIsFade = true;
            StartCoroutine("doFadeCR", spd);
            return true;
        }
        //==========================================================================
        /*!フェードアウトを開始する.
            @brief	startFadeOut
        */
        public bool startFadeOut(float spd = 0.5f) {
            gameObject.SetActive(true);
            m_colFade = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            if (m_bIsFade) {
                StopCoroutine("doFadeCR");
            }
            m_bIsFade = true;
            StartCoroutine("doFadeCR", spd);
            return true;
        }
        //==========================================================================
        /*!フェードを開始する.
            @brief	startFade
        */
        public bool startFade(float alpha, float spd = 0.5f) {
            gameObject.SetActive(true);
            m_colFade = new Color(1.0f, 1.0f, 1.0f, alpha);
            if (m_bIsFade) {
                StopCoroutine("doFadeCR");
            }
            m_bIsFade = true;
            StartCoroutine("doFadeCR", spd);
            return true;
        }
        IEnumerator doFadeCR(float speed) {
            int ang = Angle.InitLerpFactor();
            Color colBegin = materialColor;
            while (ang > 0) {
                materialColor = Color.Lerp(colBegin, m_colFade, Angle.LerpFactor(ref ang, speed));
                yield return 0;
            }
            m_bIsFade = false;
            materialColor = m_colFade;
            if (m_colFade.a == 0f) {
                gameObject.SetActive(false);
            }
        }
        //==========================================================================
        /*!フェード中かどうか判定する.
            @brief	isFade
        */
        public bool isFade {
            get {
                return m_bIsFade;
            }
        }
        //==========================================================================
        /*!フェードアウトしているかどうか判定する.
            @brief	isFadeOut
        */
        public bool isFadeOut {
            get {
                if (m_colFade.a == 1f) {
                    return true;
                }
                return false;
            }
        }
        //==========================================================================
        /*!フェードインしているかどうか判定する.
            @brief	isFadeOut
        */
        public bool isFadeIn {
            get {
                if (m_colFade.a == 0f) {
                    return true;
                }
                return false;
            }
        }
        //==========================================================================
        /*!フェードアウトオブジェクトが覆っているかどうか.
            @brief	isActive
        */
        public bool isActive {
            get {
                return gameObject.activeSelf;
            }
        }

    };
}
