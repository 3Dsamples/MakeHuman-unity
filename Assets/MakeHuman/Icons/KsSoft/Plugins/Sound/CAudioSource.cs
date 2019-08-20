//==============================================================================================
/*!CAudioSrc.
	@file  音源を管理.

	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
namespace KS {
    public class CAudioSource {
        protected GameObject m_go;
        protected AudioSource m_as;
        protected CSoundEffect m_se;
        protected float m_volume = 1f;
        protected Transform m_transform;
        //==========================================================================
        /*!Constructor
            @brief	Constructor
        */
        public CAudioSource(Transform parent) {
            m_go = new GameObject("audio src");
            m_as = m_go.AddComponent<AudioSource>();
            m_transform = m_go.transform;

            m_transform.parent = parent;
        }
        //==========================================================================
        /*!play
            @brief	SEを再生する.
        */
        public void play(CSoundEffect se) {
            play(se, 0f);
        }
        public void play(CSoundEffect se, Vector3 position) {
            transform.position = position;
            play(se, 1f);
        }
        protected void play(CSoundEffect se, float panLevel) {
            m_as.clip = se.clip;
            if (m_as.clip == null) {
                return;
            }
            m_as.spatialBlend = panLevel;
            m_as.volume = se.volume * m_volume;
            m_as.minDistance = se.minDistance;
            m_as.maxDistance = se.maxDistance;
            m_as.Play();
            m_se = se;
        }
        //==========================================================================
        /*!stop
            @brief	再生中のSEを止める.
        */
        public void stop() {
            if (m_as.clip == null) {
                return;
            }
            if (m_as.isPlaying) {
                m_as.Stop();
            }
            m_as.clip = null;
            m_se = null;
        }
        //==========================================================================
        /*!isPlay
            @brief	再生中かどうか?.
        */
        public bool isPlay {
            get {
                return m_as.isPlaying;
            }
        }
        public CSoundEffect se {
            get {
                if (m_se == null || !isPlay) {
                    return null;
                }
                return m_se;
            }
        }
        public float volume {
            get {
                return m_volume;
            }
            set {
                m_volume = value;
                if (m_se != null) {
                    m_as.volume = value * m_se.volume;
                }
            }
        }
        public byte group {
            get {
                if (m_se == null || !isPlay) {
                    return 0;
                }
                return m_se.group;
            }
        }
        public Transform transform {
            get {
                return m_transform;
            }
        }
        public float score(Vector3 posCamera) {
            if (m_se == null) {
                return float.MaxValue;
            }
            if (!isPlay) {
                m_se = null;
                return float.MaxValue;
            }
            return m_se.score(posCamera, m_transform.position);
        }
    }
}
