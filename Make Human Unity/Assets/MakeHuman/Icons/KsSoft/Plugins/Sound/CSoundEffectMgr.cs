//==============================================================================================
/*!CSoundEffectMgr
	@file	CSoundEffectMgr

	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;

namespace KS {
    public class BgmSource {
        AudioSource intro;
        AudioSource loop;
        const double delay = 0.1f;
        public BgmSource(GameObject go) {
            intro = go.AddComponent<AudioSource>() as AudioSource;
            intro.spatialBlend = 0f;
            intro.priority = 0;
            intro.loop = false;
            intro.playOnAwake = false;
            loop = go.AddComponent<AudioSource>() as AudioSource;
            loop.spatialBlend = 0f;
            loop.priority = 0;
            loop.loop = true;
            loop.playOnAwake = false;
        }
        public void clear() {
            intro.clip = null;
            loop.clip = null;
        }
        public void play(CBgmResource res, float vol, float _delay, bool _loop = true) {
            double tmSchedule = AudioSettings.dspTime + delay + _delay;
            if (res.introClip != null) {
                intro.clip = res.introClip;
                intro.volume = vol;
                intro.PlayScheduled(tmSchedule);
                tmSchedule += res.introClip.length;
            }
            if (res.loopClip != null) {
                loop.clip = res.loopClip;
                loop.volume = vol;
                loop.loop = _loop;
                loop.PlayScheduled(tmSchedule);
            }
        }
        public void stop() {
            intro.Stop();
            loop.Stop();
        }
        public float volume {
            get {
                return intro.volume;
            }
            set {
                intro.volume = value;
                loop.volume = value;
            }
        }
        public bool isPlaying {
            get {
                return intro.isPlaying | loop.isPlaying;
            }
        }
    }

    public class CSoundEffectMgr : MonoBehaviour {
        const int concurrentnum2d = 4;
        const int concurrentnum3d = 8;
        BgmSource[] m_aBGM = new BgmSource[2];
        uint m_mBGM = 0;
        int m_pageBGM = 0;
        bool m_loopBGM = true;
        float m_volBGM = 1f;
        float m_volPlayBGM = 1f;
        float m_volSE = 1f;
        Coroutine m_coFadeOut;
        CAudioSource[] m_a3dAS = new CAudioSource[concurrentnum3d];
        CAudioSource[] m_a2dAS = new CAudioSource[concurrentnum2d];
        Transform m_transCamera = null;
        CBgmResource m_cBgmResource = null;
        Vector3 m_posAudience;
        const float BgmFadeTime = 1f;
        //==========================================================================
        /*!Awake
         * @brief	Unity Callback
        */
        void Awake() {
            SceneManager.sceneLoaded += onLevelWasLoaded;
            if (m_instance != null) {
                Debug.LogError("already exist instance");
            }
            m_instance = this;
            for (int i = 0; i < m_aBGM.Length; ++i) {
                m_aBGM[i] = new BgmSource(gameObject);
            }
            for (int i = 0; i < m_a3dAS.Length; ++i) {
                m_a3dAS[i] = new CAudioSource(transform);
            }
            for (int i = 0; i < m_a2dAS.Length; ++i) {
                m_a2dAS[i] = new CAudioSource(transform);
            }
        }
        //==========================================================================
        /*!OnDestroy
         * @brief	Unity Callback
        */
        void OnDestroy() {
            m_instance = null;
        }
        //==========================================================================
        /*!Update
         * @brief	Unity Callback
        */
        void Update() {
            if (m_transCamera == null) {
                Camera cCamera = Camera.main;
                if (cCamera == null) {
                    return;
                }
                m_transCamera = cCamera.transform;
                m_posAudience = m_transCamera.position;
            }
            // BGMを鳴らす準備ができているなら鳴らす.
            if (m_cBgmResource != null) {
                playBgm();
            }
        }
        //==========================================================================
        /*!onLevelWasLoaded
         * @brief	Unity Callback
        */
        void onLevelWasLoaded(Scene scenename, LoadSceneMode mode) {
            release();
        }
        //==========================================================================
        /*!Release
         * @brief	release
        */
        public void release() {
            m_cBgmResource = null;
            m_mBGM = 0;
            for (int i = 0; i < m_aBGM.Length; ++i) {
                m_aBGM[i].clear();
            }
            for (int i = 0; i < m_a2dAS.Length; ++i) {
                m_a2dAS[i].stop();
            }
            for (int i = 0; i < m_a3dAS.Length; ++i) {
                m_a3dAS[i].stop();
            }
            m_transCamera = null;
        }
        //==========================================================================
        /*!playBgm
            @brief	BGMを再生する.
        */
        public void playBgm(CBgmResource cBR, bool loop = true, float volume = 1f) {
            if (cBR == null) {
                fadeout();
            }
            if (cBR.id == m_mBGM) {
                return;
            }
            m_cBgmResource = cBR;
            m_loopBGM = loop;
            m_volPlayBGM = volume;
        }
        public void playBgm(uint id, bool loop = true, float volume = 1f) {
            CBgmResourceMgr cMgr = CBgmResourceMgr.Instance;
            if (id == 0) {
                m_cBgmResource = null;
                fadeout();
            } else if (id == m_mBGM) {
                return;
            } else {
                m_cBgmResource = cMgr.reference(id);
                m_loopBGM = loop;
            }
            m_volPlayBGM = volume;
        }
        public void stopBgm() {

        }
        protected void playBgm() {
            if (m_cBgmResource == null || !m_cBgmResource.isLoaded) {
                return;
            }
            float delay = 0f;
            m_aBGM[m_pageBGM].play(m_cBgmResource, Mathf.Clamp01(m_volBGM * m_volPlayBGM), delay, m_loopBGM);
            m_mBGM = m_cBgmResource.id;
            m_cBgmResource = null;
        }
        bool fadeout() {
            if (m_coFadeOut != null) {
                StopCoroutine(m_coFadeOut);
            }
            if (m_aBGM[m_pageBGM].isPlaying) {
                m_coFadeOut = StartCoroutine(FadeOut(m_aBGM[m_pageBGM]));
                m_pageBGM ^= 1;
                return true;
            }
            return false;
        }
        IEnumerator FadeOut(BgmSource bs) {
            if (!bs.isPlaying) {
                m_coFadeOut = null;
                yield break;
            }
            float lastTime = BgmFadeTime;
            float baseVolume = bs.volume;
            while (lastTime > 0f) {
                bs.volume = baseVolume * lastTime / BgmFadeTime;
                lastTime -= Time.deltaTime;
                yield return 0;
            }
            bs.stop();
            bs.clear();
            m_coFadeOut = null;
        }
        //==========================================================================
        /*!play
            @brief	SEを再生する.
        */
        public int play(uint mAssetBundle, uint id) {
            CSeResourceMgr cMgr = CSeResourceMgr.Instance;
            if (cMgr == null) {
                return -1;
            }
            CSoundEffect cSE = cMgr.find(mAssetBundle, id);
            if (cSE == null) {
                return -2;
            }
            return play(cSE);
        }
        public int play(CSoundEffect cSE) {
            if (cSE == null) {
                return -2;
            }
            CAudioSource cAS = findAudioSource(cSE);
            if (cAS == null) {
                return 1;
            }
            cAS.play(cSE);
            return 0;
        }
        public void play(uint mAssetBundle, uint id, IEffectEmitter emitter) {
            if (emitter == null) {
                return;
            }
            emitter.addRefEmitter();
            play(mAssetBundle, id, emitter.getTransform(0));
            emitter.releaseEmitter();
        }
        public void play(CSoundEffect cSE, IEffectEmitter emitter) {
            if (emitter == null) {
                return;
            }
            emitter.addRefEmitter();
            play(cSE, emitter.getTransform(0));
            emitter.releaseEmitter();
        }
        public void play(uint mAssetBundle, uint id, Transform trans) {
            if (trans == null) {
                return;
            }
            play(mAssetBundle, id, trans.position);
        }
        public void play(CSoundEffect cSE, Transform trans) {
            if (trans == null) {
                return;
            }
            play(cSE, trans.position);
        }
        public int play(uint mAssetBundle, uint id, Vector3 position) {
            CSeResourceMgr cMgr = CSeResourceMgr.Instance;
            if (cMgr == null) {
                return -1;
            }
            CSoundEffect cSE = cMgr.find(mAssetBundle, id);
            if (cSE == null) {
                return -2;
            }
            return play(cSE, position);
        }
        public int play(CSoundEffect cSE, Vector3 position) {
            if (cSE == null) {
                return -2;
            }
            if (m_transCamera == null) {
                Camera cCamera = Camera.main;
                if (cCamera == null) {
                    return -1;
                }
                m_transCamera = cCamera.transform;
            }
            CAudioSource cAS = findAudioSource(cSE, position, m_posAudience);
            if (cAS != null) {
                cAS.play(cSE, position);
            }
            return 0;
        }
        //==========================================================================
        /*!findAudioSource
            @brief	2D用の空いているAudioSourceを取得する.
            @note	同じ音が2重でならないように防ぐ.
        */
        protected CAudioSource findAudioSource(CSoundEffect cSE) {
            if (cSE == null) {
                return null;
            }
            CAudioSource cStopAS = null;
            for (int i = 0; i < m_a2dAS.Length; ++i) {
                CAudioSource cAS = m_a2dAS[i];
                CSoundEffect cPlayedSE = cAS.se;
                /*			if (cPlayedSE == cSE) {
                                return cAS;
                            }
                 */
                if (cPlayedSE == null) {
                    cStopAS = cAS;
                }
            }
            return cStopAS;
        }
        //==========================================================================
        /*!findAudioSource
            @brief	3D用の空いているAudioSourceを取得する.
        */
        protected CAudioSource findAudioSource(CSoundEffect cSE, Vector3 posSE, Vector3 posBase) {
            int idx = -1;
            float score = cSE.score(posBase, posSE);
            float maxScore = score;     // 同じグループの再生数を調べる.
            byte group = cSE.group;
            int nPolyphony = 0;
            bool bNeedOverride = false;
            for (int i = 0; i < m_a3dAS.Length; ++i) {
                if (m_a3dAS[i].group == group) {
                    ++nPolyphony;
                    if (nPolyphony >= cSE.maxPolyphony) {
                        bNeedOverride = true;
                        break;
                    }
                }
            }
            if (bNeedOverride) {
                // 既存のなっている音を上書きする必要がある.
                for (int i = 0; i < m_a3dAS.Length; ++i) {
                    CAudioSource cAS = m_a3dAS[i];
                    if (m_a3dAS[i].group == group) {
                        score = cAS.score(posBase);
                        if (maxScore < score) {
                            maxScore = score;
                            idx = i;
                        }
                    }
                }
            } else {
                // 純粋にスコアで判定する.
                for (int i = 0; i < m_a3dAS.Length; ++i) {
                    CAudioSource cAS = m_a3dAS[i];
                    score = cAS.score(posBase);
                    if (maxScore < score) {
                        maxScore = score;
                        idx = i;
                    }
                }
            }
            if (idx >= 0) {
                return m_a3dAS[idx];
            }
            return null;
        }
        public float volumeBGM {
            get {
                return m_volBGM;
            }
            set {
                m_volBGM = value;
                for (int i = 0; i < m_aBGM.Length; ++i) {
                    if (m_aBGM[i] == null) {
                        continue;
                    }
                    m_aBGM[i].volume = Mathf.Clamp01(m_volBGM * m_volPlayBGM);
                }
            }
        }
        public float volumeSE {
            get {
                return m_volSE;
            }
            set {
                m_volSE = value;
                for (int i = 0; i < m_a3dAS.Length; ++i) {
                    if (m_a3dAS[i] == null) {
                        continue;
                    }
                    m_a3dAS[i].volume = value;
                }
                for (int i = 0; i < m_a2dAS.Length; ++i) {
                    if (m_a2dAS[i] == null) {
                        continue;
                    }
                    m_a2dAS[i].volume = value;
                }
            }
        }
        public uint bgm {
            get {
                return m_mBGM;
            }
        }
        public Vector3 audiencePosition {
            set {
                m_posAudience = value;
            }
        }
        static protected CSoundEffectMgr m_instance;
        public static CSoundEffectMgr Instance {
            get {
                return m_instance;
            }
        }
    }
}
