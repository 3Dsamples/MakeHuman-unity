//==============================================================================================
/*!CSoundEffect
	@file	CSoundEffect
	
	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System;
using System.Collections.Generic;

namespace KS {
    public class CSoundEffect {
        t_SoundParam m_param;
        AudioClip m_clip;
        public CSoundEffect(t_SoundParam param, AudioClip clip) {
            m_param = param;
            m_clip = clip;
        }
        public uint id {
            get {
                return m_param.m_id;
            }
        }
        public AudioClip clip {
            get {
                return m_clip;
            }
            set {
                m_clip = value;
            }
        }
        public float volume {
            get {
                return (float)m_param.m_volume * (1f / 255f);
            }
        }
        public byte priority {
            get {
                return m_param.m_priority;
            }
        }
        public byte group {
            get {
                return m_param.m_group;
            }
        }
        public byte maxPolyphony {
            get {
                return m_param.m_maxPolyphony;
            }
        }
        public float minDistance {
            get {
                if (m_param.m_minDistance < 0f) {
                    return 5f;
                }
                return m_param.m_minDistance;
            }
        }
        public float maxDistance {
            get {
                if (m_param.m_maxDistance < 0f) {
                    return 30f;
                }
                return m_param.m_maxDistance;
            }
        }
        // スコアが大きいものほどキャンセルされやすい.
        public float score(Vector3 posCamera, Vector3 posPlay) {
            return (posCamera - posPlay).sqrMagnitude + (float)(255 - priority) * 10000f;
        }
    }
}
