//==============================================================================================
/*!
	@file  CSeResource
*/
//==============================================================================================
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
namespace KS {
    //==========================================================================
    /*!
        @brief	class CSeResource
    */
    public class CSeResource : CResource<AudioClip>, IWinSoundEffect {
        Dictionary<uint, CSoundEffect> m_dicSE = new Dictionary<uint, CSoundEffect>();
        AudioClip[] m_aAudioClip = null;
        public CSeResource(uint id) : base(id) {
        }
        //==========================================================================
        /*!set
            @brief	Objectを設定する.
        */
        public void set(t_SoundParam[] aParam, AudioClip[] aAudioClip) {
            if (aParam == null) {
                Debug.LogError("logic error:can't find sound param");
                return;
            }
            m_dicSE.Clear();
            if (m_aAudioClip != null) {
                Debug.LogError("already set object:" + new MulId(m_id));
                foreach (AudioClip ac in m_aAudioClip) {
                    UnityEngine.Object.DestroyImmediate(ac, true);
                }
                m_aAudioClip = null;
            }
            m_aAudioClip = aAudioClip;
            Dictionary<uint, AudioClip> dcAudioClip = new Dictionary<uint, AudioClip>();
            for (int i = 0; i < aAudioClip.Length; ++i) {
                AudioClip ac = aAudioClip[i];
                uint id = KsSoftUtility.ConvertId(ac.name);
                if (id == 0) {
                    Debug.LogError("audio clip name is illegal:" + ac.name);
                    continue;
                }
                dcAudioClip[id] = ac;
            }
            foreach (t_SoundParam tParam in aParam) {
                AudioClip ac;
                if (!dcAudioClip.TryGetValue(tParam.m_mClip, out ac)) {
                    Debug.LogError("can't find audio clip");
                    continue;
                }
                m_dicSE[tParam.m_id] = new CSoundEffect(tParam, ac);
                if (isRemain) {
                    UnityEngine.Object.DontDestroyOnLoad(ac);
                }
            }
        }
        //==========================================================================
        /*!find
            @brief	find CSoundEffect
        */
        public CSoundEffect find(uint id) {
            CSoundEffect se;
            if (!m_dicSE.TryGetValue(id, out se)) {
                Debug.LogError("can't find sound effect:" + new MulId(id) + ":" + new FiveCC(id));
                return null;
            }
            return se;
        }
        //==========================================================================
        /*!find
            @brief	find CSoundEffect
        */
        public void play(uint mSE) {
            CSoundEffect se;
            if (!m_dicSE.TryGetValue(mSE, out se)) {
                Debug.LogError("can't find sound effect:" + new MulId(id) + ":" + new FiveCC(id));
                return;
            }
            CSoundEffectMgr cSoundEffectMgr = CSoundEffectMgr.Instance;
            if (cSoundEffectMgr == null) {
                return;
            }
            cSoundEffectMgr.play(se);
        }
        //==========================================================================
        /*!release
            @brief	release
        */
        override public void Destroy() {
            m_dicSE.Clear();
            m_aAudioClip = null;
            base.Destroy();
        }
    };
}
