//==============================================================================================
/*!バイナリ出力汎用関数.
	@file ExportBinaryData
	
	(counter SJIS string 京.)
*/
//==============================================================================================
#define RESOURCE
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using KS;

public class ExportBinaryData {

    const string COMPARE_PATH     = "bin/";
    static Dictionary<uint,string>  m_dicBinaryExport = new Dictionary<uint,string>();

    static ExportBinaryData() {
        Dictionary<string, SystemLanguage> dcMessageDataId = KsSoftConfig.MessageDataId;
        foreach (string nm in dcMessageDataId.Keys) {
            string path = "messagedata." + nm + ".bin";
            if (File.Exists(KsSoftConfig.BINARY_BASE_PATH + path)) {
                m_dicBinaryExport[(uint)dcMessageDataId[nm] + AssetBundleId.MessageData] = path;
            }
        }
    }
    static public void export(BuildTarget eTarget) {
        foreach (uint mId in m_dicBinaryExport.Keys) {
            export(eTarget,mId);
        }
    }
    static public void export(BuildTarget eTarget,uint mId) {
        byte[] buffer = isNeedUpdate(eTarget,mId);
        if (buffer == null) {
            return;
        }
        Debug.Log("******* export assetbundles: " + MulId.ToString(mId) + "(" + getBinaryPath(mId) + ") *******");
        // アセットバンドル化用のインスタンス.
        CSerializableScript cSS = ScriptableObject.CreateInstance<CSerializableScript>();

        // バッファからアセットを生成.
        cSS.m_buffer = buffer;
        string assetPath = "Assets/binary.asset";
        AssetDatabase.CreateAsset(cSS,assetPath);

        AssetBundleBuild[] aABB = new AssetBundleBuild[1];

        aABB[0].assetBundleName = MulId.ToString(mId) + KsSoftConfig.AssetbundleExt;
        aABB[0].assetNames = new string[] { assetPath };

        // アセットをアセットバンドルにパックする.
        BuildPipeline.BuildAssetBundles(CreateAssetbundles.getAssetPath(eTarget),aABB,BuildAssetBundleOptions.ChunkBasedCompression,eTarget);

        // 一時的なアセットを削除.
        AssetDatabase.DeleteAsset(assetPath);
        commit(eTarget,mId);
    }
    static void commit(BuildTarget eTarget,uint mId) {
        byte[] buffer = isNeedUpdate(eTarget,mId);
        if (buffer == null) {
            return;
        }
        string sPath = comparePath(eTarget);
        if (!Directory.Exists(sPath)) {
            Directory.CreateDirectory(sPath);
        }
        sPath = getComparePath(eTarget,mId);
        FileStream fs = new FileStream(sPath,FileMode.Create);
        fs.Write(buffer,(int) 0,(int) buffer.Length);
        fs.Close();
    }

    // 内容を比較し、更新が必要がかどうかを返す.
    static byte[] isNeedUpdate(BuildTarget eTarget,uint mId) {
        string createPath = CreateAssetbundles.getAssetPath(MulId.ToString(mId),eTarget);
        string compPath = getComparePath(eTarget,mId);
        string basePath = getBinaryPath(mId);
        if (compPath == null || basePath == null) {
            Debug.LogError("this id is not binary asset!!:" + new MulId(mId));
            return null;
        }
        if (File.Exists(basePath) == false) {
            Debug.LogError("can't find base binary path:" + basePath);
            return null;
        }
        // バイナリファイルをバッファに読み込む.
        byte[] aBase = read(basePath);
        if (!File.Exists(compPath) || !File.Exists(createPath)) {
            return aBase;
        }
        // 比較する.
        byte[] aComp = read(compPath);
        if (aComp.Length == aBase.Length) {
            for (int i = 0;i < aComp.Length;++i) {
                if (aComp[i] != aBase[i]) {
                    return aBase;
                }
            }
            return null;
        }
        return aBase;
    }

    static byte[] read(string path) {
        FileInfo fi = new FileInfo(path);
        if (!fi.Exists) {
            Debug.LogError("can't open file:" + path);
            return null;
        }
        FileStream fs = new FileStream(path,FileMode.Open);
        byte[] buffer = new byte[fs.Length];
        fs.Read(buffer,(int) 0,(int) fs.Length);
        fs.Close();
        return buffer;
    }
    static string getBinaryPath(uint mId) {
        string name;
        if (!m_dicBinaryExport.TryGetValue(mId,out name)) {
            return null;
        }
        return KsSoftConfig.BINARY_BASE_PATH + name;
    }
    static string getComparePath(BuildTarget eTarget,uint mId) {
        string name;
        if (!m_dicBinaryExport.TryGetValue(mId,out name)) {
            return null;
        }
        return comparePath(eTarget) + name;
    }
    static string comparePath(BuildTarget eTarget) {
        string sTarget = COMPARE_PATH;
        switch (eTarget) {
        case BuildTarget.StandaloneWindows:
            sTarget += "Windows/";
            break;
        case BuildTarget.StandaloneWindows64:
            sTarget += "Windows/";
            break; ;
        case BuildTarget.WebGL:
            sTarget += "Web/";
            break;
        case BuildTarget.StandaloneOSX:
            sTarget += "OSX/";
            break;
        case BuildTarget.iOS:
            sTarget += "iOS/";
            break;
        case BuildTarget.Android:
            sTarget += "Android/";
            break;
        default:
            goto case BuildTarget.StandaloneWindows;
        }
        return sTarget;
    }
}

