//==============================================================================================
/*!
	@file  CBgmResource
*/
//==============================================================================================
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
namespace KS {
    //==========================================================================
    /*!
        @brief	class CBgmResource
    */
    public class CBgmResource : CResource<AudioClip> {
        AudioClip m_acIntro = null;
        AudioClip m_acLoop = null;
        public CBgmResource(uint id) : base(id) {
        }
        //==========================================================================
        /*!set
            @brief	Objectを設定する.
        */
        public void set(AudioClip acLoop, AudioClip acIntro) {
            if (isRemain && m_acLoop != null) {
                Debug.LogError("already set object:" + new MulId(m_id));
                UnityEngine.Object.DestroyImmediate(m_acLoop, true);
                if (m_acIntro != null) {
                    UnityEngine.Object.DestroyImmediate(m_acIntro, true);
                }
            }
            m_acLoop = acLoop;
            m_acIntro = acIntro;
            if (isRemain) {
                UnityEngine.Object.DontDestroyOnLoad(m_acLoop);
                if (m_acIntro != null) {
                    UnityEngine.Object.DontDestroyOnLoad(m_acIntro);
                }
            }
        }
        override public void Destroy() {
            if (m_acLoop != null) {
                m_acLoop = null;
            }
            if (m_acIntro != null) {
                m_acIntro = null;
            }
            base.Destroy();
        }
        public AudioClip loopClip {
            get {
                return m_acLoop;
            }
        }
        public AudioClip introClip {
            get {
                return m_acIntro;
            }
        }
    };
}
