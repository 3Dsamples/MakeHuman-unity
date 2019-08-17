//==============================================================================================
/*!すべてのアセットバンドルデータをエキスポートする.
	@file  ExportAllAssetBundle

	(counter SJIS string 京.)
*/
//==============================================================================================
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using KS;
public class ExportAllAssetBundle
{
	const BuildTarget	m_eBaseBuildTarget = BuildTarget.StandaloneWindows;

    static public void exportNone() {
	}
	[MenuItem("Tools/KsSoft/Export All/Active Build Target")]
	static public void export() {
		export(EditorUserBuildSettings.activeBuildTarget);
	}
	[MenuItem("Tools/KsSoft/Export All/iOS")]
	static public void exportIOS() {
		export(BuildTarget.iOS);
	}
	[MenuItem("Tools/KsSoft/Export All/Android")]
	static public void exportAndroid() {
		export(BuildTarget.Android);
	}
	[MenuItem("Tools/KsSoft/Export All/Web")]
	static public void exportWeb() {
		export(BuildTarget.WebGL);
	}
	[MenuItem("Tools/KsSoft/Export All/Windows,Mac")]
	static public void exportWindows() {
		export(BuildTarget.StandaloneWindows);
    }
	static void export(BuildTarget eTarget) {
		AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
		string	sAssetPath = CreateAssetbundles.getAssetPath("",eTarget);
		if (!Directory.Exists(sAssetPath)) {
			Directory.CreateDirectory(sAssetPath);
		}
		// binary list
		ExportBinaryData.export(eTarget);

		// SE
		foreach (string dir in Directory.GetDirectories(KsSoftConfig.SEResourcePath)) {
			MulId	id = convertDir2Id(dir);
			if (id != 0 && isNeedToExport(eTarget,id.ToString(),dir)) {
				ExportSeData.export(eTarget,dir);
			}
		}
		// BGM
		List<string> lstTarget = EditorHelpers.CollectAllAssetPath<AudioClip>(KsSoftConfig.BgmResourcePath,true);
		foreach (string dir in lstTarget) {
			string sName = Path.GetFileNameWithoutExtension(dir);
			MulId	id = MulId.Normalize(sName);
			if (id != 0 && isNeedToExport(eTarget,id.ToString())) {
				Debug.Log (dir);
				ExportBgmData.export (eTarget,dir);
			}
		}
		// font
		ExportFontData(eTarget);

		// TextureResource
		foreach (string dir in Directory.GetDirectories(KsSoftConfig.TextureResourcePath)) {
			MulId	id = convertDir2Id(dir);
			if (id != 0 && isNeedToExport(eTarget,id.ToString(),dir)) {
				ExportTextureResource.export(eTarget,dir);
			}
		}
		// ウィンドウリソースデータ出力.
		if (isNeedToExport(eTarget,ExportWindowResource.id)) {
			ExportWindowResource.export(eTarget);
		}
        //----------------------------------------------
        // version.unity3dを出力.
        // この関数の一番最後で呼ぶようにしてください.
        // バージョンデータが正常に出力できなくなります.
        ExportVersion.export(eTarget);
    }
	static public void ExportFontData(BuildTarget eTarget) {
		foreach (string dir in Directory.GetDirectories(KsSoftConfig.FontPath)) {
			MulId id = convertDir2Id(dir);
			if (id != 0 && isNeedToExport(eTarget, id.ToString(), dir)) {
				ExportFonts.export(eTarget, dir);
			}
		}
		List<TextAsset> lstFont = EditorHelpers.CollectAll<TextAsset>(KsSoftConfig.FontResourcePath, true);
		foreach (TextAsset textasset in lstFont) {
			ExportFonts.export(eTarget, textasset);
		}
	}
	//==========================================================================
	/* !日付の更新情報を見て、出力する必要があるかどうか判定する.
		@brief	isNeedToExport
	*/
	static bool isNeedToExport(BuildTarget eTarget,string sName) {
		if (eTarget == BuildTarget.StandaloneWindows) {
			return true;
		}
		string	sTargetFile = CreateAssetbundles.getAssetPath(sName,eTarget);
        if (sTargetFile.Contains("StreamingAssets")) {
            return true;
        }
        // 出力対象が存在しないときは無条件で出力.
        if (!File.Exists(sTargetFile)) {
            return true;
        }
        string sBaseFile = CreateAssetbundles.getAssetPath(sName,m_eBaseBuildTarget);
        if (!File.Exists(sBaseFile)) {
            // ベースになるファイルが存在しないときは、作成を抑制.
            Debug.LogWarning("can't find base file:" + sBaseFile);
            if (File.Exists(sTargetFile)) {
                Debug.LogWarning("delete file:" + sTargetFile);
                File.Delete(sTargetFile);
            }
            return false;
        }
		// 更新された日付を確認し、出力対象ファイルが古ければ更新.
		if (File.GetLastWriteTime(sTargetFile) < File.GetLastWriteTime(sBaseFile)) {
			return true;
		}
		// 更新する必要なし.
		return false;
	}
	//==========================================================================
	/* !特定のパス直下にあるファイル達の日付の更新情報を見て、出力する必要があるかどうか判定する.
		@brief	isNeedToExport
	*/
	static bool isNeedToExport(BuildTarget eTarget,string sName,string sPath) {
		if (isNeedToExport (eTarget,sName)) {
			return true;
		}
		string	sBaseFile = CreateAssetbundles.getAssetPath(sName,m_eBaseBuildTarget);
		if (!File.Exists(sBaseFile)) {
			return false;
		}
		string	sTargetFile = CreateAssetbundles.getAssetPath(sName,eTarget);
		if (!Directory.Exists(sPath)) {
			// ベースになるディレクトリが存在しない.
			Debug.LogWarning("can't find path:" + sPath);
			if (File.Exists(sTargetFile)) {
				Debug.LogWarning("delete file:" + sTargetFile);
				File.Delete(sTargetFile);
			}
			return false;
		}
		List<string> lstPath = EditorHelpers.CollectAllAssetPath<UnityEngine.Object>(sPath,true);
		System.DateTime	dtTarget = File.GetLastWriteTime(sTargetFile);
		foreach (string path in lstPath) {
			// 更新された日付を確認し、出力対象ファイルが古ければ更新.
			if (dtTarget < File.GetLastWriteTime(path)) {
				return true;
			}
		}
		// 更新する必要なし.
		return false;
	}
	static MulId convertDir2Id(string dir) {
		if (dir.Contains("/include/")) {
			return MulId.zero;
		}
		string[] aPath = dir.Split(new char [] { '/','\\' });
		if (aPath.Length <= 0) return MulId.zero;
		string	sPath = aPath[aPath.Length - 1];
		return MulId.Normalize(sPath);
	}
}
