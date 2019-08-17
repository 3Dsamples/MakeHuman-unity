//==============================================================================================
/*!
	@file  CTextureResource
*/
//==============================================================================================
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
namespace KS {
    //==========================================================================
    /*!
        @brief	class CTextureResource
    */
    public class CTextureResource : CResource<Material> {
        protected CSpriteDataOne[] m_aSDO = null;
        protected string m_sShader;
        protected Dictionary<uint, CSpriteDataOne> m_dcSDO = null;

        public CTextureResource(uint id) : base(id) {
        }
        public CSpriteDataOne[] spriteData {
            get {
                return m_aSDO;
            }
            set {
                if (m_aSDO == value) {
                    return;
                }
                m_aSDO = value;
                m_dcSDO = null;
            }
        }
        protected override void DontDestroyOnLoad() {
            if (m_Object == null) {
                m_Object = null;
                return;
            }
            base.DontDestroyOnLoad();
            if (m_Object.mainTexture != null) {
                Texture tex = UnityEngine.Object.Instantiate(m_Object.mainTexture);
                m_Object.mainTexture = tex;
                UnityEngine.Object.DontDestroyOnLoad(tex);
            }
        }
        //==========================================================================
        /*!reference
            @brief	reference
        */
        public CSpriteDataOne reference(uint mId) {
            if (m_aSDO == null) {
                return null;
            }
            CSpriteDataOne cSDO;
            if (m_dcSDO == null) {
                m_dcSDO = new Dictionary<uint, CSpriteDataOne>();
                for (int i = 0; i < m_aSDO.Length; ++i) {
                    cSDO = m_aSDO[i];
                    m_dcSDO[cSDO.m_id] = cSDO;
                }
            }
            if (!m_dcSDO.TryGetValue(mId, out cSDO)) {
                return null;
            }
            return cSDO;

        }

    };
}
