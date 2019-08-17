//==============================================================================================
/*!SEデータ出力.
	@file  Export SeData.cs
*/
//==============================================================================================
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using KS;

public class ExportSeData
{
	static private string sDef			 = "@se.def";


	static public bool export(BuildTarget eTarget) {
		Object[] aObject = Selection.GetFiltered(typeof (Object), SelectionMode.DeepAssets);
		if (aObject.Length != 1) {
			return false;
		}
		string sPath = AssetDatabase.GetAssetPath(aObject[0]);
		return export(eTarget,Path.GetDirectoryName(sPath));
	}
	//==========================================================================
	/*!SEエキスポート.
		@brief	export
	*/
	static public bool export( BuildTarget eTarget,string sBasePath) {
		string sAssetPath = CreateAssetbundles.getAssetPath("", eTarget);
		if (!Directory.Exists(sAssetPath)) {
			Directory.CreateDirectory(sAssetPath);
		}
		// テクスチャパーツの情報を列挙する.
		List<string> lstSePath = EditorHelpers.CollectAllAssetPath<AudioClip>(sBasePath,true);
		// 全てのObjectをリストの中にいれる.
		List<string> lstInclude = new List<string>();
		List<t_SoundParam> lstParam = new List<t_SoundParam>();

		MulId	mId = MulId.zero;
		string path;
		for (int i = 0;i < lstSePath.Count;++i) {
			path = lstSePath[i];
			string[] aName = path.Split(new char [] {'/','\\','.'});
			if (aName.Length < 3) {
				Debug.LogError("name is illegal:" + path);
				return false;
			}
			if (mId == 0) {
				mId = new MulId(aName[aName.Length - 3]);
			} else {
				MulId	id = new MulId(aName[aName.Length - 3]);
				if (id != mId) {
					Debug.LogError("selection error");
					return false;
				}
			}
			uint	acId = EditorHelpers.ConvertId(aName[aName.Length - 2]);
			if (acId == 0) {
				Debug.LogError ("id format is illegal:" + path);
				continue;
			}
			lstInclude.Add(path);
			t_SoundParam	tParam = new t_SoundParam();
			tParam.m_id = acId;
			tParam.m_mClip = acId;
			tParam.m_minDistance = -1f;
			tParam.m_maxDistance = -1f;
			tParam.m_group = 1;
			tParam.m_priority = 1;
			tParam.m_volume = 255;
			tParam.m_maxPolyphony = 1;
			lstParam.Add (tParam);
		}
		// 設定ファイル読み込み.
		path = sBasePath + "/" + sDef;
		bool	bResource;
		Dictionary<uint,t_SoundParam>	dcParam = readSeDef(out bResource,path,lstParam);
		if (dcParam == null) {
			return false;
		}
		t_SoundParamHeader	tHeader = new t_SoundParamHeader();
		tHeader.m_aSoundParam = new t_SoundParam[dcParam.Count];
		int idx = 0;
		foreach (t_SoundParam tParam in dcParam.Values) {
			tHeader.m_aSoundParam[idx++] = tParam;
		}
		CWriteVariable	cVariable = new CWriteVariable(4096);
		tHeader.write (cVariable);

		if (bResource) {
			CSeSerializableScript cSeSS = ScriptableObject.CreateInstance<CSeSerializableScript>();
			cSeSS.m_buffer = cVariable.buffer();
			cSeSS.m_aAC = new AudioClip[lstInclude.Count];
			for (int i = 0;i < cSeSS.m_aAC.Length;++i) {
				cSeSS.m_aAC[i] = AssetDatabase.LoadAssetAtPath<AudioClip>(lstInclude[i]);
			}
			AssetDatabase.CreateAsset(cSeSS,KsSoftConfig.ResourcesPath + mId + ".se.asset");

			Debug.Log("******* export resource data for sound effect resource: " + mId.ToString() + " *******");
		} else {
			CSerializableScript cSS = ScriptableObject.CreateInstance<CSerializableScript>();
			cSS.m_buffer = cVariable.copybuffer();
			// se設定データをアセット化.
			path = "Assets/sedata.asset";
			AssetDatabase.CreateAsset(cSS, path);
			lstInclude.Add(path);

			// アセットバンドル化.
			AssetBundleBuild[]	aABB = new AssetBundleBuild[1];

			aABB[0].assetBundleName = mId + KsSoftConfig.AssetbundleExt;
			aABB[0].assetNames = lstInclude.ToArray();

			// アセットをアセットバンドルにパックする.
			BuildPipeline.BuildAssetBundles(CreateAssetbundles.getAssetPath(eTarget),aABB,BuildAssetBundleOptions.ChunkBasedCompression,eTarget);
			AssetDatabase.DeleteAsset(path);
			Debug.Log("******* export assetbundles for sound effect resource: " + mId.ToString() + " *******");
		}
		return true;
	}
	//==========================================================================
	/*!設定データを読み込む.
		@brief	readSeDef
	*/
	static 	Dictionary<uint,t_SoundParam>	readSeDef(out bool bResource,string path,List<t_SoundParam> lstParam) {
		bResource = false;
		Dictionary<uint,t_SoundParam> dcParam = new Dictionary<uint, t_SoundParam>();
		// 設定ファイルがあるかチェック.
		if (!File.Exists(path)) {
			foreach (t_SoundParam tParam in lstParam) {
				dcParam[tParam.m_id] = tParam;
			}
			return dcParam;
		}
		Debug.Log ("find " + sDef + " at " + path);
		CSeParser.Parser parser = CSeParser.Parser.compile(path);
		if (parser == null) {
			Debug.LogError("compile failure:" + path);
			return null;
		}
		bool	bError = false;
		for (CSeParserDef	cDef = parser.topDef;cDef != null;cDef = cDef.next) {
			if (cDef.id == 0) {
				continue;
			}
			t_SoundParam	tParam = lstParam.Find (p => p.m_id == cDef.idbase);
			if (tParam == null) {
				bError = true;
				Debug.LogError("can't find audio clip:" + new FiveCC(cDef.idbase) + " or " + new MulId(cDef.idbase));
				continue;
			}
			tParam = new t_SoundParam();
			tParam.m_id = cDef.id;
			tParam.m_mClip = cDef.idbase;
			tParam.m_minDistance = -1f;
			tParam.m_maxDistance = -1f;
			tParam.m_group = 1;
			tParam.m_priority = 1;
			tParam.m_volume = 255;
			tParam.m_maxPolyphony = 1;
			for (CSeParserProperty prop = cDef.property;prop != null;prop = prop.next) {
				switch (prop.property) {
				case e_SeProperty.NONE:
					break;
				case e_SeProperty.VOLUME:
					tParam.m_volume = (byte) prop.Value;;
					break;
				case e_SeProperty.PRIORITY:
					tParam.m_priority = (byte) prop.Value;
					break;
				case e_SeProperty.GROUP:
					tParam.m_group = (byte) prop.Value;
					break;
				case e_SeProperty.POLYPHONY:
					tParam.m_maxPolyphony = (byte) prop.Value;
					break;
				case e_SeProperty.DISTANCE:
					tParam.m_minDistance = prop.getVector2.x;
					tParam.m_maxDistance = prop.getVector2.y;
					break;
				}
			}
			if (dcParam.ContainsKey(tParam.m_id)) {
				Debug.LogError ("already defined parts id:" + new FiveCC(cDef.id) + " or " + new MulId(cDef.id));
				bError = true;
				continue;
			}
			dcParam[cDef.id] = tParam;
		}
		if (bError) {
			return null;
		}
		// 残りを回収.
		foreach (t_SoundParam param in lstParam) {
			if (dcParam.ContainsKey(param.m_id)) {
				continue;
			}
			dcParam[param.m_id] = param;
		}
		return dcParam;
	}
}
