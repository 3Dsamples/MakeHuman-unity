//==============================================================================================
/*!アセットバンドルを生成する.
	@file  CreateAssetBundles
*/
//==============================================================================================

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

using KS;

class CreateAssetbundles
{
	struct t_Asset {
		public Object obj;
		public string path;
		public t_Asset(Object _obj,string _path) {
			obj = _obj;
			path = _path;
		}
	};
    static CreateAssetbundles() {
        KsSoftConfig.initialize();
    }
	[MenuItem("Assets/Export")]
	static public void Export() {
		uint	mOldBgmId = 0;
		uint	mBgmId = 0;
		BuildTarget eBuildTarget = EditorUserBuildSettings.activeBuildTarget;

		Object[] aObject = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
		List<t_Asset> lstPath = new List<t_Asset>();
		foreach (Object obj in aObject) {
			string path = AssetDatabase.GetAssetPath(obj);
			// Hierarchy
			if (path.Contains("/Hierarchy/") && path.Contains(".fbx")) {
				lstPath.Add(new t_Asset(obj,path));
				continue;
			}
			// FontResource.
			if (path.Contains("/FontResource/")) {
				if (obj as TextAsset) {
					lstPath.Add(new t_Asset(obj, path));
					continue;
				}
			}
			// Fonts
			if (path.Contains("/Fonts/")) {
				string fontpath = Path.GetDirectoryName(path);
				string[] paths = fontpath.Split(new char[] { '/', '\\' });
				fontpath = paths[0];
				for (int i = 1;i < paths.Length;++i) {
					fontpath += "/" + paths[i];
					if (MulId.isMulId(paths[i])) {
						break;
					}
				}
				if (lstPath.Exists((s) => s.path == fontpath)) {
					continue;
				}
				lstPath.Add(new t_Asset(null, fontpath));
				continue;
			}
			// Compile Window Resource
			if (path.Contains("/WindowResource/") && path.Contains("wra")) {
				lstPath.Add(new t_Asset(obj, path));
				continue;
			}
			// Export Sound Effect
			if (path.Contains("/Se/")) {
				if (!EditorHelpers.isDirectory(obj)) {
					continue;
				}
				lstPath.Add(new t_Asset(obj, path));
				continue;
			}
			// Export BGM
			if (path.Contains("/Bgm/")) {
				mOldBgmId = mBgmId;
				mBgmId = MulId.Normalize(path);
				if (mOldBgmId == mBgmId) {
					continue;
				}
				lstPath.Add(new t_Asset(obj, path));
				continue;
			}
			// Export TextureResource
			if (path.Contains("/TextureResource/") && EditorHelpers.isDirectory(obj)) {
				lstPath.Add(new t_Asset(obj, path));
				continue;
			}
		}
		bool bExportVersion = false;
		for (int i = 0;i < lstPath.Count;++i) {
			float progress = (float) i/lstPath.Count;
			string path = lstPath[i].path;
			if (path == null) {
				continue;
			}
			Debug.Log(path);
			Object obj = lstPath[i].obj;
			// FontResource.
			if (path.Contains("/FontResource/")) {
				if (obj as TextAsset) {
					EditorUtility.DisplayProgressBar(
						"FontResource:" + i + "/" + lstPath.Count,
						path,
						progress
					);
					ExportFonts.export (eBuildTarget,obj as TextAsset);
					continue;
				}
			}
            // Fonts
			if (path.Contains("/Fonts/")) {
				EditorUtility.DisplayProgressBar(
					"Fonts:" + i + "/" + lstPath.Count,
					path,
					progress
				);
				ExportFonts.export (eBuildTarget,path);
			}
			// Compile Window Resource
			if (path.Contains("/WindowResource/") && path.Contains("wra")) {
				EditorUtility.DisplayProgressBar(
					"WindowResource:" + i + "/" + lstPath.Count,
					path,
					progress
				);
				ExportWindowResource.compile(obj);
				continue;
			}
            // Export Sound Effect
            if (path.Contains("/Se/")) {
				if (!EditorHelpers.isDirectory(obj)) {
					continue;
				}
				EditorUtility.DisplayProgressBar(
					"Se:" + i + "/" + lstPath.Count,
					path,
					progress
				);
				ExportSeData.export(eBuildTarget,path);
				bExportVersion = true;
				continue;
			}
			// Export BGM
			if (path.Contains("/Bgm/")) {
				mOldBgmId = mBgmId;
				mBgmId = MulId.Normalize(path);
				if (mOldBgmId == mBgmId) {
					Debug.Log ("skip");
					continue;
				}
				EditorUtility.DisplayProgressBar(
					"Bgm:" + i + "/" + lstPath.Count,
					path,
					progress
				);
				ExportBgmData.export(eBuildTarget,path);
				bExportVersion = true;
				continue;
			}
			// Export TextureResource
			if (path.Contains("/TextureResource/") && EditorHelpers.isDirectory(obj)) {
				EditorUtility.DisplayProgressBar(
					"TextureResource:" + i + "/" + lstPath.Count,
					path,
					progress
				);
				ExportTextureResource.export(eBuildTarget,path);
				bExportVersion = true;
				continue;
			}
		}
		if (bExportVersion) {
			exportFinish();
		}
		EditorUtility.ClearProgressBar();
	}
	[MenuItem("Tools/KsSoft/Export Fonts")]
    static public void exportFonts() {
        BuildTarget eBuildTarget = EditorUserBuildSettings.activeBuildTarget;
		// font
		ExportAllAssetBundle.ExportFontData(eBuildTarget);
		exportFinish();
	}
	[MenuItem("Tools/KsSoft/Export Window Resource")]
	static public void exportWindowResource() {
		BuildTarget eBuildTarget = EditorUserBuildSettings.activeBuildTarget;
		if (ExportWindowResource.export(eBuildTarget)) {
			exportFinish();
		}
	}
    [MenuItem("Tools/KsSoft/Export Binary Data")]
	static void exportBinary() {
		BuildTarget eBuildTarget = EditorUserBuildSettings.activeBuildTarget;
		ExportBinaryData.export(eBuildTarget);
		exportFinish();
	}
    [MenuItem("Tools/KsSoft/Re-Export Version")]
	static public void ExportFinish() {
		exportFinish();
	}

	//==========================================================================
	/*!ファイル出力後の後処理.
	 *	1)version.unity3dを更新する.
	 *	2)ビルドターゲットをWindowsに切り替える.
	*/
	static void exportFinish() {
		BuildTarget eBuildTarget = EditorUserBuildSettings.activeBuildTarget;
		ExportVersion.export(eBuildTarget);
	}

	//==========================================================================
	/*!アセット出力パスを決定する.
		@brief	getAssetPath
	*/
	public static string getAssetPath(string sBaseName,BuildTarget eTarget) {
		if (sBaseName == "") {
            return KsSoftConfig.getPlatformPath(eTarget);
        } else {
			string[] aName = sBaseName.Split('.');
			return KsSoftConfig.getPlatformPath(eTarget) + aName[0] + KsSoftConfig.AssetbundleExt;
		}
	}
	public static string getAssetPath(BuildTarget eTarget) {
		return getAssetPath("",eTarget);
	}

	//==========================================================================
	/* !拡張子が適切かどうかファイル名から判定する.
		@brief	CheckName
	*/
	public static int	CheckName(ref string sName,string sExt) {
		string[] aName = sName.Split('.');
		MulId	mId = new MulId(aName[0]);
		if (aName.Length != 2 || !aName[1].Contains(sExt) || mId == 0) {
			return 0;
		}
		sName = mId.ToString() + "." + sExt;
		return (int) ((uint) mId);
	}
	//==========================================================================
	/* !プレファブを生成する.
		@brief	GetPrefab
	*/
    public static Object GetPrefab(GameObject go, string name,bool bDestroy) {
        if (!name.Contains("Assets/")) {
            name = "Assets/" + name;
        }
        Object tempPrefab = PrefabUtility.CreateEmptyPrefab(name + ".prefab");
        tempPrefab = PrefabUtility.ReplacePrefab(go, tempPrefab);
		if (bDestroy) {
	        Object.DestroyImmediate(go);
		}
        return tempPrefab;
    }
	public static void setStatic(GameObject gameObject) {
		gameObject.isStatic = true;
		Transform	transform = gameObject.transform;
		foreach (Transform tf in transform) {
			if (tf.gameObject != null) {
				tf.gameObject.isStatic = true;
			}
		}
	}
    public static void DeleteAssets(string[] prefabs) {
        foreach (string path in prefabs) {
            AssetDatabase.DeleteAsset(path);
        }
    }
}
