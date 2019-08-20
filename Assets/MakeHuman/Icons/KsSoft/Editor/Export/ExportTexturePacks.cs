//==============================================================================================
/*!テクスチャをシンプルにパックする.
	@file  ExportTexturePacks
	(counter SJIS string 京.)
*/
//==============================================================================================
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using KS;

public class ExportTexturePacks {
    //==========================================================================
    /*!テクスチャデータエキスポート.
		@brief	export
	*/
    static public bool export(BuildTarget eTarget,string sBasePath) {
        string sAssetPath = CreateAssetbundles.getAssetPath("",eTarget);
        if (!Directory.Exists(sAssetPath)) {
            Directory.CreateDirectory(sAssetPath);
        }
        // テクスチャパーツの情報を列挙する.
        List<string> lstTexturePath = EditorHelpers.CollectAllAssetPath<Texture2D>(sBasePath,true);
        List<Texture2D> lstTexture = new List<Texture2D>();
        List<string> lstPath = new List<string>();
        MulId mId = MulId.zero;
        string path;
        for (int i = 0;i < lstTexturePath.Count;++i) {
            path = lstTexturePath[i];
            string[] aName = path.Split(new char[] { '/','\\','.' });
            if (aName.Length < 3) {
                Debug.LogError("name is illegal:" + path);
                return false;
            }
            if (mId == 0) {
                mId = new MulId(aName[aName.Length - 3]);
            } else {
                MulId id = new MulId(aName[aName.Length - 3]);
                if (id != mId) {
                    Debug.LogError("selection error");
                    return false;
                }
            }
            uint texid = EditorHelpers.ConvertId(aName[aName.Length - 2]);
            if (texid == 0) {
                Debug.LogError("id format is illegal:" + path);
                continue;
            }
            Texture2D texture = AssetDatabase.LoadAssetAtPath(path,typeof(Texture2D)) as Texture2D;
            lstTexture.Add(texture);
            lstPath.Add(path);
        }
        AssetBundleBuild[] aABB = new AssetBundleBuild[1];
        aABB[0].assetBundleName = mId + KsSoftConfig.AssetbundleExt;
        aABB[0].assetNames = lstPath.ToArray();
        // アセットをアセットバンドルにパックする.
        BuildPipeline.BuildAssetBundles(CreateAssetbundles.getAssetPath(eTarget),aABB,BuildAssetBundleOptions.ChunkBasedCompression,eTarget);

        Debug.Log("******* packed texture resource: " + mId.ToString() + " *******");
        return true;
    }
}
