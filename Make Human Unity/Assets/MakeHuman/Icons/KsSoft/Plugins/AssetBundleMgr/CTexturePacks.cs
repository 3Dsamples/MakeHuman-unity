//==============================================================================================
/*!
	@file  CTexturePacks
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
    public class CTexturePacks : CResourceBase {
        protected Dictionary<uint, Texture2D> m_dcTexture = null;
        protected Texture2D[] m_aTextures = null;
        public CTexturePacks(uint id) : base(id) {
        }
        override protected void DontDestroyOnLoad() {
            if (m_aTextures != null) {
                for (int i = 0; i < m_aTextures.Length; ++i) {
                    UnityEngine.Object.DontDestroyOnLoad(m_aTextures[i]);
                }
            }
        }
        override public void Destroy() {
            m_aTextures = null;
            if (asset != null) {
                asset.release();
            }
        }
        //==========================================================================
        /*!reference
            @brief	reference
        */
        public Texture2D reference(uint mId) {
            if (m_aTextures == null) {
                return null;
            }
            if (m_dcTexture == null) {
                m_dcTexture = new Dictionary<uint, Texture2D>();
                for (int i = 0; i < m_aTextures.Length; ++i) {
                    Texture2D tex = m_aTextures[i];
                    uint id = KsSoftUtility.ConvertId(tex.name);
                    if (id == 0) {
                        Debug.LogError("this texture name is not id!!:" + tex.name);
                        continue;
                    }
                    m_dcTexture[id] = tex;
                }
            }
            Texture2D tex2d;
            if (!m_dcTexture.TryGetValue(mId, out tex2d)) {
                return null;
            }
            return tex2d;
        }
        public Texture2D[] textures {
            get {
                return m_aTextures;
            }
            set {
                m_aTextures = value;
            }
        }
    };
}
