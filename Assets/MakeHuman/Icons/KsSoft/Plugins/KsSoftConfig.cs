//==============================================================================================
/*!KsSoftConfig.
	@file  KsSoftConfig.cs
*/
//==============================================================================================
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace KS {
    public class KsSoftConfig : KsSoftConfigBase {
        static public void initialize() {
            m_UseStreaming = false;
            m_assetbundlePath = "Assets/KsSoft/Assetbundle/";
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
            m_sDebugDataPath = m_assetbundlePath + "Windows/";
#endif
            m_resourcePath = "Assets/KsSoft/Resources/";
            m_windowResourcePath = "Assets/KsSoft/WindowResource/";
            m_WindowResourceBinaryPath = m_windowResourcePath + "wrb/";
            m_textureResourcePath = "Assets/KsSoft/TextureResource/";
            m_SEResourcePath = "Assets/KsSoft/SE/";
            m_BgmResourcePath = "Assets/KsSoft/Bgm/";
            m_FontPath = "Assets/KsSoft/Fonts/";
            m_FontResourcePath = "Assets/KsSoft/FontResource/";
        }
        static public string BINARY_BASE_PATH = "Assets/KsSoft/Editor/Multilingual/";
    }
}
