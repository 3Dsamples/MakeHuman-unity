//==============================================================================================
/*!Sprite Data
	@file  CSpriteData
	
*/
//==============================================================================================
using UnityEngine;

//==========================================================================
/*!スプライトデータ.
	@brief	class CSpriteData
*/
namespace KS {
    public class CSpriteDataOne {
        public CSpriteDataOne() {
        }
        public CSpriteDataOne(uint id) {
            m_id = id;
            m_ePatch = e_Patch.None;
            m_uv = new Vector4(0f, 0f, 1f, 1f);
            m_color = WinColor.white;
            m_aUV = new Vector4[1];
            m_aUV[0] = m_uv;
        }
        public uint m_id;
        public WinColor m_color;
        public e_Patch m_ePatch;
        public Vector4 m_uv;
        public Vector4[] m_aUV;
    };
    public class CSpriteData {
        public uint m_mId;
        public CSpriteDataOne[] m_aData;
        protected string m_sShader;

        public const string DefaultShader = "Custom UI/Alpha";
        public CSpriteData() {
        }
        public CSpriteData(CSerializableScript cSS) {
            if (cSS == null) {
                return;
            }
            CReadVariable cVariable = new CReadVariable(cSS.m_buffer);
            t_SpriteData tSD = new t_SpriteData();
            tSD.read(cVariable);

            m_mId = tSD.m_id;
            m_sShader = tSD.m_sShader;
            m_aData = new CSpriteDataOne[tSD.m_aData.Length];

            for (int i = 0; i < tSD.m_aData.Length; ++i) {
                CSpriteDataOne cSDO = new CSpriteDataOne();
                t_SpriteDataOne tSDO = tSD.m_aData[i];
                m_aData[i] = cSDO;
                cSDO.m_id = tSDO.m_id;
                cSDO.m_color = new WinColor(tSDO.m_color);
                cSDO.m_ePatch = (e_Patch)tSDO.m_ePatch;
                int n = tSDO.m_aUV.Length;
                cSDO.m_uv = new Vector4(tSDO.m_aUV[0].x, tSDO.m_aUV[0].y, tSDO.m_aUV[n - 1].z, tSDO.m_aUV[n - 1].w);
                cSDO.m_aUV = tSDO.m_aUV;
            }
        }
        public string shader {
            get {
                if (string.IsNullOrEmpty(m_sShader)) {
                    return DefaultShader;
                }
                return m_sShader;
            }
            set {
                m_sShader = value;
            }
        }
    }
}
