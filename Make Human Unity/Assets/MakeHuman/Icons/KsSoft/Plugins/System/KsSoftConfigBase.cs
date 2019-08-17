//==============================================================================================
/*!KsSoftConfig.
	@file  KsSoftConfig.cs
*/
//==============================================================================================
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace KS {
    public class KsSoftConfigBase {
        static KsSoftConfigBase() {
            m_UseStreaming = true;
            VERSION_ASSET = 1504030101;   //YYMMDDVVvv	V = メジャーバージョン,v = マイナーバージョン
            VERSION_CLIENT = 1504030101;   //YYMMDDVVvv
            CRYPT_CODE = 1504030101;
            CRYPT_DATA = 1234567890987654321;
            m_dicStrMessageDataMulId = new Dictionary<string, SystemLanguage>() {
            {"AF",SystemLanguage.Afrikaans },
            {"AR",SystemLanguage.Arabic },
            {"EU",SystemLanguage.Basque },
            {"BE",SystemLanguage.Belarusian },// ベラルーシ語
			{"BG",SystemLanguage.Bulgarian },// ブルガリア語
			{"CA",SystemLanguage.Catalan },// カタロニア語
			{"ZH",SystemLanguage.Chinese },// 中国語
			{"CS",SystemLanguage.Czech },// チェコ語
			{"DA",SystemLanguage.Danish },// デンマーク語
			{"NL",SystemLanguage.Dutch },// オランダ語
			{"EN",SystemLanguage.English },// 英語
			{"ET",SystemLanguage.Estonian },// エストニア語
			{"FI",SystemLanguage.Finnish },// フィンランド語
			{"FR",SystemLanguage.French },// フランス語
			{"DE",SystemLanguage.German },// ドイツ語
			{"EL",SystemLanguage.Greek },// ギリシャ語
			{"HE",SystemLanguage.Hebrew },// ヘブライ語
			{"IS",SystemLanguage.Icelandic },// アイスランド語
			{"ID",SystemLanguage.Indonesian },// インドネシア語
			{"IT",SystemLanguage.Italian },// イタリア語
			{"JA",SystemLanguage.Japanese },// 日本語
			{"KO",SystemLanguage.Korean },// 韓国語
			{"LV",SystemLanguage.Latvian },// ラトビア語
			{"LT",SystemLanguage.Lithuanian },// リトアニア語
			{"NO",SystemLanguage.Norwegian },// ノルウェー語
			{"PL",SystemLanguage.Polish },// ポーランド語
			{"PT",SystemLanguage.Portuguese },//  ポルトガル語
			{"RO",SystemLanguage.Romanian },// ルーマニア語
			{"RU",SystemLanguage.Russian },// ロシア語
			{"HR",SystemLanguage.SerboCroatian },// セルビアクロアチア語
			{"SL",SystemLanguage.Slovenian },// スロベニア語
			{"ES",SystemLanguage.Spanish },// スペイン語
			{"SV",SystemLanguage.Swedish },// スウェーデン語
			{"TH",SystemLanguage.Thai },// タイ語
			{"TR",SystemLanguage.Turkish },// トルコ語
			{"UK",SystemLanguage.Ukrainian },// ウクライナ語
			{"VE",SystemLanguage.Vietnamese },// ベトナム語
			{"ZS",SystemLanguage.ChineseSimplified },// 中国語簡体字(simplified)
			{"ZT",SystemLanguage.ChineseTraditional },// 中国語繁体字(traditional)
			{"HU",SystemLanguage.Hungarian }// ハンガリー語
		};
            m_httpserver = "http://xxx.xxx.xxx.xxx/";
            m_assetbundlePath = "assetbundles/";
            m_assetbundleExt = ".unity3d";
            m_resourcePath = "Assets/Resources/";
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
            m_sDebugDataPath = m_assetbundlePath + "Windows/";
#endif
            m_windowResourcePath = "Assets/WindowResource/";
            m_WindowResourceBinaryPath = "wrb/";
            m_textureResourcePath = "Assets/TextureResource/";
            m_FontPath = "Assets/Fonts/";
            m_FontResourcePath = "Assets/FontResource/";
            m_SEResourcePath = "Assets/SE/";
            m_BgmResourcePath = "Assets/Bgm/";

            m_defaultFontKind = FiveCC.Id("fn30");
            m_defaultTextureId = MulId.Id(100, 0, 1);

            m_preprocessor = "gcc";
            m_mFontsAssetbundleId = AssetBundleId.Fonts;
            m_httpserver = "http://xxx.xxx.xxx.xxx/";
#if UNITY_EDITOR
            m_safeArea = new Rect(32, 32, Screen.width - 64, Screen.height - 64);
#else
		m_safeArea = Screen.safeArea;
#endif
        }
        protected static string CurrentPath {
            get {
                string[] aPath = System.IO.Directory.GetCurrentDirectory().Split(new char[] { '/', '\\' }, System.StringSplitOptions.RemoveEmptyEntries);
                if (aPath.Length >= 1) {
                    if (!aPath[0].Contains(":")) {
                        aPath[0] = '/' + aPath[0];
                    }
                }
                return string.Join("/", aPath) + "/";

            }
        }
        protected static bool m_UseStreaming;
        public static bool UseStreaming {
            get {
                return m_UseStreaming;
            }
        }
        static public uint VERSION_ASSET;
        static public uint VERSION_CLIENT;
        static public uint CRYPT_CODE;
        static public ulong CRYPT_DATA;

        //==========================================================================
        /*!Assetbundle
        */
        static protected string m_assetbundlePath;
        public static string AssetbundlePath {
            get {
                if (m_assetbundlePath.IndexOf("Assets/") == 0) {
                    return CurrentPath + m_assetbundlePath;
                }
                return m_assetbundlePath;
            }
        }
#if UNITY_EDITOR
        public static string getPlatformPath(BuildTarget eTarget) {
            switch (eTarget) {
            case BuildTarget.StandaloneWindows:
                return AssetbundlePath + "Windows/";
            case BuildTarget.StandaloneOSX:
                goto case BuildTarget.StandaloneWindows;
            case BuildTarget.WebGL:
                return AssetbundlePath + "Web/";
            case BuildTarget.iOS:
                return AssetbundlePath + "iOS/";
            case BuildTarget.PS4:
                return AssetbundlePath + "PS4/";
            case BuildTarget.XboxOne:
                return AssetbundlePath + "XboxOne/";
            case BuildTarget.Android:
                return AssetbundlePath + "Android/";
            case BuildTarget.StandaloneLinux:
                goto case BuildTarget.StandaloneWindows;
            case BuildTarget.StandaloneWindows64:
                goto case BuildTarget.StandaloneWindows;
            case BuildTarget.WSAPlayer:
                goto case BuildTarget.StandaloneWindows;
            case BuildTarget.StandaloneLinux64:
                goto case BuildTarget.StandaloneWindows;
            case BuildTarget.StandaloneLinuxUniversal:
                goto case BuildTarget.StandaloneWindows;
            default:
                return AssetbundlePath + "Unknown/";
            }
        }
#endif
        public static string PlatformPath(bool UseStreaming) {
#if UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE
            if (UseStreaming) {
                return "file://" + Application.dataPath + "/StreamingAssets/";
            }
            return AssetbundlePath + "Windows/";
#elif UNITY_WEBPLAYER || UNITY_WEBGL
		return AssetbundlePath + "Web/";
#elif UNITY_WII
		return AssetbundlePath + "Wii/";
#elif UNITY_IOS
        if (UseStreaming) {
#if UNITY_EDITOR
            if (System.IO.Directory.Exists(Application.streamingAssetsPath)) {
                return "file://" + Application.streamingAssetsPath + "/";
            }
#endif
    		return "file://" + Application.streamingAssetsPath + "/";
        }
        return AssetbundlePath + "iOS/";
#elif UNITY_ANDROID
        if (UseStreaming) {
#if UNITY_EDITOR
            if (System.IO.Directory.Exists(Application.streamingAssetsPath)) {
                return "file://" + Application.streamingAssetsPath + "/";
            }
#endif
            return "jar:file://" + Application.dataPath + "!/assets/";
        }
        return AssetbundlePath + "Android/";
#elif UNITY_PS3
        return AssetbundlePath + "PS3/";
#elif UNITY_PS4
		return AssetbundlePath + "PS4/";
#elif UNITY_XBOX360
		return AssetbundlePath + "XBOX360/";
#elif UNITY_XBOXONE
		return AssetbundlePath + "XBOXONE/";
#elif UNITY_BLACKBERRY
		return AssetbundlePath + "BlackBerry/";
#elif UNITY_TIZEN
		return AssetbundlePath + "Tizen/";
#elif UNITY_WP8 || UNITY_WP8_1
        if (UseStreaming) {
            return Application.dataPath + "/StreamingAssets";
        }
        return AssetbundlePath + "Windows/";
#elif UNITY_WSA || UNITY_WSA_8_0 || UNITY_WSA_8_1 || UNITY_WSA_10_0
        if (UseStreaming) {
            return Application.dataPath + "/StreamingAssets";
        }
        return AssetbundlePath + "Windows/";
#elif UNITY_EDITOR
        if (UseStreaming && System.IO.Directory.Exists(Application.streamingAssetsPath)) {
            return "file://" + Application.streamingAssetsPath + "/";
        }
        return "file://" + AssetbundlePath + "Windows/";
#else
        if (UseStreaming) {
            return Application.dataPath + "/StreamingAssets";
        }
        return AssetbundlePath + "Windows/";
#endif
        }
        static protected string m_assetbundleExt;
        public static string AssetbundleExt {
            get { return m_assetbundleExt; }
        }
        static protected string m_resourcePath;
        public static string ResourcesPath {
            get { return m_resourcePath; }
        }
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
        static protected string m_sDebugDataPath;
        static public string debugPath {
            get {
                if (m_sDebugDataPath.Contains(":") || m_sDebugDataPath[0] == '/' || m_sDebugDataPath[0] == '\\') {
                    return m_sDebugDataPath;
                }
                return CurrentPath + m_sDebugDataPath;
            }
            set {
                m_sDebugDataPath = value;
            }
        }
#endif
        //==========================================================================
        /*!safeArea
        */
        static protected Rect m_safeArea;
        static public Rect safeArea {
            get {
                return m_safeArea;
            }
            set {
                m_safeArea = value;
            }
        }
        //==========================================================================
        /*!WindowMgr
        */
        static public uint WindowAssetbundleId {
            get {
                return AssetBundleId.WindowResourceData;
            }
        }
        static protected string m_windowResourcePath;
        static public string WindowResourceSourcePath {
            get {
                return m_windowResourcePath;
            }
        }
        static protected string m_WindowResourceBinaryPath;
        static public string WindowResourceBinaryPath {
            get {
                return m_WindowResourceBinaryPath;
            }
        }
        static public uint m_defaultFontKind;
        static public uint DefaultFontKind {
            get {
                return m_defaultFontKind;
            }
        }
        static protected uint m_defaultTextureId;
        static public uint DefaultTextureId {
            get {
                return m_defaultTextureId;
            }
        }
        //==========================================================================
        /*!TextureResource
        */
        static protected string m_textureResourcePath;
        static public string TextureResourcePath {
            get {
                return m_textureResourcePath;
            }
        }
        //==========================================================================
        /*!SE Resource
        */
        static protected string m_SEResourcePath;
        static public string SEResourcePath {
            get {
                return m_SEResourcePath;
            }
        }
        //==========================================================================
        /*!BGM Resource
        */
        static protected string m_BgmResourcePath;
        static public string BgmResourcePath {
            get {
                return m_BgmResourcePath;
            }
        }
        //==========================================================================
        /*!Font Resource
        */
        static protected string m_FontResourcePath;
        static public string FontResourcePath {
            get {
                return m_FontResourcePath;
            }
        }
        static protected string m_FontPath;
        static public string FontPath {
            get {
                return m_FontPath;
            }
        }
        //==========================================================================
        /*!Preprocessor for compilation.
        */
        static protected string m_preprocessor;
        static public string Preprocessor {
            get {
#if UNITY_EDITOR_WIN
                string gccpath = CurrentPath + "Assets/KsSoft/Editor/bin/";
                gccpath = gccpath.Replace('/', '\\');
                if (System.IO.Directory.Exists(gccpath)) {
                    string prpath = System.Environment.GetEnvironmentVariable("PATH");
                    if (!string.IsNullOrEmpty(prpath) && !prpath.Contains(gccpath)) {
                        System.Environment.SetEnvironmentVariable("PATH", prpath + ";" + gccpath, System.EnvironmentVariableTarget.Process);
                        Debug.Log("add:" + System.Environment.GetEnvironmentVariable("PATH"));
                    } else {
                        Debug.Log("exists:" + System.Environment.GetEnvironmentVariable("PATH"));
                    }
                    Debug.Log(gccpath);
                    return gccpath + "gcc.exe";

                }
#endif
                return m_preprocessor;
            }
        }
#if UNITY_EDITOR
        static protected string m_windowResourcePreprocessorArguments;
        static public string WindowResourcePreprocessorArguments {
            get {
                return " -x c -E -I " + WindowResourceSourcePath + "include -include wr.h " + m_windowResourcePreprocessorArguments + " -D " + EditorUserBuildSettings.activeBuildTarget;
            }
        }
        static protected string m_textureResourcePreprocessorArguments;
        static public string TextureResourcePreprocessorArguments {
            get {
                return " -x c -E -I " + TextureResourcePath + "include -include tr.h " + m_textureResourcePreprocessorArguments + " -D " + EditorUserBuildSettings.activeBuildTarget;
            }
        }
        static protected string m_SEResourcePreprocessorArguments;
        static public string SEResourcePreprocessorArguments {
            get {
                return " -x c -E -I " + SEResourcePath + "include -include se.h " + m_SEResourcePreprocessorArguments + " -D " + EditorUserBuildSettings.activeBuildTarget;
            }
        }
#endif
        //==========================================================================
        /*!Font Assetbundle ID.
        */
        static protected uint m_mFontsAssetbundleId;
        static public uint FontsAssetbundleId {
            get {
                return m_mFontsAssetbundleId;
            }
        }

        //==========================================================================
        /*!MessageDataSheet
        */
        static SystemLanguage m_language = SystemLanguage.Unknown;
        static public SystemLanguage language {
            get {
                if (m_language == SystemLanguage.Unknown) {
                    return Application.systemLanguage;
                }
                return m_language;
            }
            set {
                m_language = value;
            }
        }

        static protected Dictionary<string, SystemLanguage> m_dicStrMessageDataMulId;
        static public Dictionary<string, SystemLanguage> MessageDataId {
            get {
                return m_dicStrMessageDataMulId;
            }
        }
        //==========================================================================
        /*!HTTP Address/IP.
        */
        static protected string m_httpserver;
        static public string httpserver {
            get {
                return m_httpserver;
            }
            set {
                m_httpserver = value;
            }
        }
    }
}
