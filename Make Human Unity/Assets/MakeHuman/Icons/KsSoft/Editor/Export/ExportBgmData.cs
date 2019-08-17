 //==============================================================================================
/*!BGMエキスポート.
	@file  ExportBgmData
	
	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using KS;

public class ExportBgmData
{
	static public bool export(BuildTarget eTarget,string path) {
		string sAssetPath = CreateAssetbundles.getAssetPath("", eTarget);
		if (!Directory.Exists(sAssetPath)) {
			Directory.CreateDirectory(sAssetPath);
		}
		MulId	mId = MulId.Normalize(path);
		if (mId == 0) {
			Debug.LogError ("auido file name is illegal!:" + path);
			return false;
		}
		string	sPath = Path.GetDirectoryName(path) + "/";
		string	sExt = Path.GetExtension(path);
		string	sName = new MulId(mId).ToString();

		AssetBundleBuild[]	aABB = new AssetBundleBuild[1];
		aABB[0].assetBundleName = mId + KsSoftConfig.AssetbundleExt;
		List<string> lstPath = new List<string>();
		lstPath.Add (sPath + sName + sExt);
		if (!File.Exists(lstPath[0])) {
			return false;
		}
		// イントロ部分があるかチェック.
		string pathIntro = sPath + sName + ".intro" + sExt;
		if (File.Exists (pathIntro)) {
			lstPath.Add (pathIntro);
		}
		aABB[0].assetNames = lstPath.ToArray();
		Debug.Log(aABB[0].assetBundleName);
		// アセットをアセットバンドルにパックする.
		BuildPipeline.BuildAssetBundles(CreateAssetbundles.getAssetPath(eTarget),aABB,BuildAssetBundleOptions.ChunkBasedCompression,eTarget);
		return true;
	}
}
