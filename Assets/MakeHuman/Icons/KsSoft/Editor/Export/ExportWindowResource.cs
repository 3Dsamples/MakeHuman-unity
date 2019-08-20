 //==============================================================================================
/*!ウィンドウリソースデータを出力する.
	@file  ExportWindowResource

	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using Object=UnityEngine.Object;
using KS;

public class ExportWindowResource
{
	const	uint	MAGIC_NO	= 0xFEEDBCAB;
	const	uint	VERSION		= 20140701;
	static 	string	srcpath 	= KsSoftConfig.WindowResourceSourcePath;
	static  string	wrbpath		= KsSoftConfig.WindowResourceBinaryPath;

	static public string id {
		get {
			return new MulId(KsSoftConfig.WindowAssetbundleId).ToString();
		}
	}
    static public bool export(BuildTarget eTarget ) {
		// アセットバンドルのパスを取得する.
        string path = CreateAssetbundles.getAssetPath("", eTarget);
        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
		}
		string[] srcfiles = Directory.GetFiles(srcpath,"*.wra",SearchOption.AllDirectories);
		List<string> lstWRA = new List<string>();
		foreach (string file in srcfiles) {
			if (!file.Contains(".wra")) continue;

			string name = System.IO.Path.GetFileNameWithoutExtension(file);

			lstWRA.Add (name);
		}
		// wrbファイルを列挙する.
		string[] binfiles = Directory.GetFiles(wrbpath,"*.wrb");
		List<string>	lstWRB = new List<string>();
		foreach (string file in binfiles) {
			if (Path.GetExtension(file) != ".wrb") continue;
			string name = System.IO.Path.GetFileNameWithoutExtension(file);
			if (lstWRA.IndexOf(name) < 0) {
				Debug.LogWarning ("delete " + file + ",can't find "  + name + ".wra");
				File.Delete(file);
				if (File.Exists(file + ".meta")) {
					File.Delete(file + ".meta");
				}
				continue;
			}
			lstWRB.Add(file);
		}
		// エキスポート先に応じてヘッダを生成する.
		Dictionary<uint,t_WindowDataHeader>	dcAssetBundle = new Dictionary<uint, t_WindowDataHeader>();
		Dictionary<string,t_WindowDataHeader> dcAsset = new Dictionary<string, t_WindowDataHeader>();

		Dictionary<uint,string> dcCheckDuplicateId = new Dictionary<uint, string>();
		bool	bError = false;
		foreach (string file in lstWRB) {
			CReadVariable	cVariable = KsSoftUtility.read(file);
			if (cVariable == null) {
				continue;
			}
			t_WindowBin	tBin	= new t_WindowBin();
			tBin.read (cVariable);
			uint winid = tBin.m_data.m_mId;
			if (dcCheckDuplicateId.ContainsKey(winid)) {
				Debug.LogError ("WindowID(" + new MulId(winid) +") was duplicated:" + dcCheckDuplicateId[winid] +"," + tBin.m_name);
				bError = true;
				continue;
			}
			dcCheckDuplicateId[winid] = tBin.m_name;
			t_WindowDataHeader	tDataHeader = null;
			if (tBin.m_mAssetbundle == 0 && string.IsNullOrEmpty(tBin.m_exportpath)) {
				tBin.m_mAssetbundle = KsSoftConfig.WindowAssetbundleId;
			}
			if (tBin.m_mAssetbundle != 0) {
				if (!dcAssetBundle.TryGetValue(tBin.m_mAssetbundle,out tDataHeader)) {
					tDataHeader = new t_WindowDataHeader();
					dcAssetBundle[tBin.m_mAssetbundle] = tDataHeader;
					tDataHeader.m_uMagicNo = MAGIC_NO;
					tDataHeader.m_uVersion = VERSION;
				}
			} else {
				if(!dcAsset.TryGetValue(tBin.m_exportpath,out tDataHeader)) {
					tDataHeader = new t_WindowDataHeader();
					dcAsset[tBin.m_exportpath] = tDataHeader;
					tDataHeader.m_uMagicNo = MAGIC_NO;
					tDataHeader.m_uVersion = VERSION;
				}
			}
			if (tDataHeader != null) {
				int ln = tDataHeader.m_aWindowData.Length;
				Array.Resize(ref tDataHeader.m_aWindowData,ln + 1);
				tDataHeader.m_aWindowData[ln] = tBin.m_data;
			}
		}
		if (bError) {
			Debug.LogError("can't export window resource binary data:assetbundle(" + dcAssetBundle.Count + "):asset(" + dcAsset.Count + ")");
			return false;
		}
		//--------------------------------------------------
		// アセットバンドルを出力.
		//--------------------------------------------------
		foreach (uint mId in dcAssetBundle.Keys) {
			t_WindowDataHeader	tHeader = dcAssetBundle[mId];
			CSerializableScript cSS = ScriptableObject.CreateInstance<CSerializableScript>();

			CWriteVariable	cVariable = new CWriteVariable(65536 *16);
			tHeader.write (cVariable);
			// バッファからアセットを生成.
			cSS.m_buffer = cVariable.copybuffer();

			string assetPath = "Assets/windows.asset";
			AssetDatabase.CreateAsset(cSS, assetPath);

			// アセットバンドル化.
			AssetBundleBuild[]	aABB = new AssetBundleBuild[1];

			aABB[0].assetBundleName = new MulId(mId) + KsSoftConfig.AssetbundleExt;
			aABB[0].assetNames = new string[] { assetPath	};

			// アセットをアセットバンドルにパックする.
			BuildPipeline.BuildAssetBundles(CreateAssetbundles.getAssetPath(eTarget),aABB,BuildAssetBundleOptions.ChunkBasedCompression,eTarget);

			// 一時的なアセットを削除.
			AssetDatabase.DeleteAsset(assetPath);
		}
		//--------------------------------------------------
		// Resourcesの下のwindows.assetを生成する.
		//--------------------------------------------------
		foreach (string exportpath in dcAsset.Keys) {
			t_WindowDataHeader	tHeader = dcAsset[exportpath];

			CWriteVariable	cVariable = new CWriteVariable(65536 *16);
			tHeader.write (cVariable);
			// バッファからアセットを生成.
			CSerializableScript cSS = ScriptableObject.CreateInstance<CSerializableScript>();
			cSS.m_buffer = cVariable.copybuffer();

			AssetDatabase.CreateAsset(cSS, exportpath + ".asset");
		}
		Debug.Log("export window resource binary data:assetbundle(" + dcAssetBundle.Count + "):asset(" + dcAsset.Count + ")");
		return true;
	}
	static public void compile() {
    	foreach (Object o in Selection.GetFiltered(typeof (Object), SelectionMode.DeepAssets)) {
        	compile(o);
        }
	}
	//==============================================================================================
	/*!compile
		@brief	compile
		@note
	*/
	//==============================================================================================
    static public bool compile(Object o ) {
		int	nError = 0;
		string path = AssetDatabase.GetAssetPath(o);
		if (!path.Contains(".wra")) {
			Debug.LogError("this file is not window resource data:" + path);
			nError++;
			return false;
		}
		string name = System.IO.Path.GetFileNameWithoutExtension(path);
		CWinParser.Parser parser = CWinParser.Parser.compile(path,wrbpath + "base/");
		if (parser == null || parser.ErrorNum > 0) {
			string err = "compile failure:" + path;
			if (parser != null) {
				err += " error num = " + parser.ErrorNum;
			}
			Debug.LogError (err);
			return false;
		}
		if (parser.id == 0) {
			Debug.LogError("Window Resource Id Error:" + parser);
			nError++;
			return false;
		}


		//------------------------------------
		// ヘッダ生成.
		CWriteVariable	cVariable = new CWriteVariable(4096);

		t_WindowBin		tWindowBin = new t_WindowBin();

		t_WindowData	tWindowData =tWindowBin.m_data;
		tWindowData.m_mId = parser.id;
		if (parser.property != null) {
			tWindowData.m_propertyNum = (uint) parser.property.size + 1;	//IDプロパティが自動生成されるので.
		}
		expandCtrl(parser.topCtrl);
		if (parser.topCtrl != null) {
			tWindowData.m_ctrlNum = (uint) parser.topCtrl.size;
		}
		// ウィンドウプロパティ書き込み.
		string	sPath = parser.path;
		string	sBaseClass = parser.baseclass;
		if (parser.property != null) {
			if (writeProperty(parser.id,cVariable,parser.property,null,name) < 0) {
				Debug.LogError("compile failure:" + parser);
				nError++;
				return false;
			}
		}
		// コントロールのIDテーブルを生成する.
		Dictionary<string,uint>	dicCtrlId = new Dictionary<string, uint>();
		Dictionary<uint,string>	dicCtrl = new Dictionary<uint, string>();
		if (parser.topCtrl != null) {
			for (CWinParserCtrl cCtrl = parser.topCtrl;cCtrl != null;cCtrl = cCtrl.next) {
				if (cCtrl.kind == e_WinCtrlKind.NONE) {
					continue;
				}
				string key = cCtrl.ToString ();
				if (dicCtrlId.ContainsKey(key)) {
					Debug.LogError ("already exist ctrl name:" + key + "(" + new MulId(id) + ")");
					nError++;
				}
				dicCtrlId[key] = searchCtrlId(cCtrl,dicCtrl);;
			}
			// コントロール書き込み.
			for (CWinParserCtrl cCtrl = parser.topCtrl;cCtrl != null;cCtrl = cCtrl.next) {
				if (cCtrl.kind == e_WinCtrlKind.NONE) {
					continue;
				}
				if (writeCtrl(cVariable,cCtrl,dicCtrlId) == 0) {
					Debug.LogError("compile failure:" + parser);
					nError++;
					return false;
				}
			}
		}
		tWindowData.m_aData = cVariable.copybuffer();
		cVariable.clear ();
		tWindowBin.m_exportpath = parser.exportpath;
		tWindowBin.m_mAssetbundle = parser.assetbundle;
		tWindowBin.m_name = parser.name;
		tWindowBin.write (cVariable);
		//-----------------------------------
		// ファイル書き出し.
		if (!Directory.Exists(wrbpath)) {
			Directory.CreateDirectory(wrbpath);
		}
		string sFile = wrbpath + name + ".wrb";
		FileStream	fs = new FileStream(sFile,FileMode.Create);
		fs.Write(cVariable.buffer(),0,cVariable.size());
		fs.Close();
		//-----------------------------------
		// C#コード出力.
		if (!sPath.EndsWith("/") && !sPath.EndsWith("\\")) {
			sPath += "/";
		}
		if (!Directory.Exists(sPath)) {
			Directory.CreateDirectory(sPath);
		}
		sFile = sPath + parser.name + "Base.cs";
		StreamWriter	sw = new StreamWriter(sFile);
        sw.Write("using KS;\n\n");
		sw.Write("public class " + parser.name + "Base : " + sBaseClass + " {\n");
		sw.Write("\tpublic\tconst uint\twindowId\t=\t" + parser.id + ";\t// " + new MulId(parser.id) + "\n");
		sw.Write("\tstatic public " + parser.name  + " create(CWindowBase cParent = null) {" + "\n");
		sw.Write("\t\treturn CWindowMgr.Instance.create<" + parser.name + ">(windowId,cParent);" + "\n");
		sw.Write("\t}" + "\n");
		if (parser.topCtrl != null) {
			for (CWinParserCtrl cCtrl = parser.topCtrl;cCtrl != null;cCtrl = cCtrl.next) {
				if (cCtrl.kind == e_WinCtrlKind.NONE) {
					continue;
				}
				string key = cCtrl.ToString ();
				uint	ctrlId = dicCtrlId[key];
				sw.Write("\tpublic\tconst uint\t" + cCtrl.kind + "_" + cCtrl.id + "\t=\t" + ctrlId + ";\t// " + new MulId(ctrlId) + "\n");
			}
		}
		sw.Write("};" + "\n");
		sw.Close();

		if (nError > 0) {
			Debug.LogError("compile failure:" + parser + " error num = " + nError);
		} else {
			if (parser.WarningNum == 0) {
				Debug.Log("success:compiled window resource:" + parser);
			} else {
				Debug.LogWarning("success:compiled window resource:" + parser + " warning num = " + parser.WarningNum);
			}
		}
		return true;
	}
	//==============================================================================================
	/*!コントロールを書き込む.
		@brief	writeCtrl
		@note
	*/
	//==============================================================================================
	static uint writeCtrl(CWriteVariable cVariable,CWinParserCtrl cCtrl,Dictionary<string,uint>	dicCtrlId) {
		if (cCtrl.kind == e_WinCtrlKind.NONE) {
			return 0;
		}
		t_WinCtrlData	tWinCtrlData = new t_WinCtrlData();
		tWinCtrlData.m_kind = (uint) cCtrl.kind;
		if (cCtrl.property != null) {
			tWinCtrlData.m_propertyNum = (uint) cCtrl.property.size;
		}
		tWinCtrlData.write(cVariable);
		uint mId = 0;
		if (cCtrl.property != null) {
			string key = cCtrl.ToString ();
			mId = dicCtrlId[key];
			if (writeProperty(mId,cVariable,cCtrl.property,dicCtrlId,cCtrl.ToString()) < 0) {
				return 0;
			}
		}
		return mId;
	}
	//==============================================================================================
	/*!コントロールのIDを探す.
		@brief	searchCtrlId
		@note	存在しないときは、ハッシュ値を生成する.
	*/
	//==============================================================================================
	static uint searchCtrlId(CWinParserCtrl cCtrl,Dictionary<uint,string> dicCtrl) {
		uint id = searchId(cCtrl.property);
		string key = cCtrl.ToString();
		if (id == 0) {
			id = EditorHelpers.Hash(key);
			while (dicCtrl.ContainsKey(id)) {
				++id;
			}
			CWinParserProperty	propID = new CWinParserProperty();
			propID.set (e_WinProperty.ID,(int) id);
			cCtrl.property.next = propID;
		}
		dicCtrl[id] = key;
		return id;
	}
	static uint searchId(CWinParserProperty prop) {
		uint id = 0;
		for (;prop != null;prop = prop.next) {
			if (prop.property == e_WinProperty.NONE) {
				continue;
			}
			if (prop.property == e_WinProperty.ID) {
				id = (uint) prop.getValue;
				break;
			}
		}
		return id;
	}
	//==============================================================================================
	/*!プロパティを書き込む.
		@brief	writeProperty
		@note
	*/
	//==============================================================================================
	static int writeProperty(uint mId,CWriteVariable cVariable,CWinParserProperty prop,Dictionary<string,uint>	dicCtrlId,string name) {
		int iResult = 0;
		CWinFactory	cFactory = CWinFactory.Instance;
		if (cFactory == null) {
			Debug.LogError("CWinFactory == null");
			return -1;
		}
		ulong	uFlag = 0;

		cVariable.put(cFactory.getChunk(e_WinProperty.ID));
		t_WinProperty tWinProp = new t_WinProperty();
		tWinProp.m_value = (int) mId;
		tWinProp.write(cVariable);

		for (;prop != null;prop = prop.next) {
			if (prop.property == e_WinProperty.NONE) {
				continue;
			}
			if (prop.property == e_WinProperty.ID) {
				if (mId != (uint) prop.getValue) {
					Debug.LogError ("id property is illegal:" + new MulId(mId) + ":" + name);
                    iResult =  -3;
				}
				continue;
			}
			uint chunk = cFactory.getChunk(prop.property);
			if (chunk == 0) {
				Debug.LogError("can't find chunk:" + prop.property);
				iResult =  -1;
			}
			// テクスチャZOffsetは複数持てるプロパティ.
			if (prop.property != e_WinProperty.TEXTURE_ZOFFSET) {
				ulong uAddFlag = (ulong) ((ulong) 1 << (int) prop.property);
				if ((uAddFlag & uFlag) != 0) {
					// 二重で登録されている.
					Debug.LogError("duplicate property:" + new MulId(mId) + ":" + prop.property);
					iResult =  -2;
				}
				uFlag |= uAddFlag;
			}

			cVariable.put(chunk);
			switch (prop.property) {
			  //---------------------------------------------
			  // 値の一つのプロパティ.
			  //---------------------------------------------
			  case e_WinProperty.STYLE:
				{
					t_WinProperty tProperty = new t_WinProperty();
					tProperty.m_value = prop.getValue;
					tProperty.write(cVariable);
				}
				break;
			  case e_WinProperty.CAPTION_COLOR:
				goto case e_WinProperty.STYLE;
			  case e_WinProperty.COLOR0:
				goto case e_WinProperty.STYLE;
			  case e_WinProperty.COLOR1:
				goto case e_WinProperty.STYLE;
			  case e_WinProperty.COLOR2:
				goto case e_WinProperty.STYLE;
			  case e_WinProperty.COLOR3:
				goto case e_WinProperty.STYLE;
			  case e_WinProperty.COLOR4:
				goto case e_WinProperty.STYLE;
			  case e_WinProperty.COLOR5:
				goto case e_WinProperty.STYLE;
			  case e_WinProperty.COLOR6:
				goto case e_WinProperty.STYLE;
			  case e_WinProperty.COLOR7:
				goto case e_WinProperty.STYLE;
			  case e_WinProperty.SE_ID:
				goto case e_WinProperty.STYLE;
		  	  case e_WinProperty.RELATION_ID:
				goto case e_WinProperty.STYLE;
			  case e_WinProperty.HELP_ID:
				goto case e_WinProperty.STYLE;
		 	  case e_WinProperty.FONT_KIND:
				goto case e_WinProperty.STYLE;
			  case e_WinProperty.SLIDEMAX:
				goto case e_WinProperty.STYLE;
			  case e_WinProperty.PRIORITY:
				goto case e_WinProperty.STYLE;
			  case e_WinProperty.CAPTION:
				goto case e_WinProperty.STYLE;
			  case e_WinProperty.TOOLTIP:
				goto case e_WinProperty.STYLE;
			  case e_WinProperty.LINE_SPACE:
				goto case e_WinProperty.STYLE;
			  case e_WinProperty.LINE_FEED_OFFSET:
				goto case e_WinProperty.STYLE;
			  case e_WinProperty.CAPTION_STR:
				{
					t_WinPropertyString tProperty = new t_WinPropertyString();
					tProperty.m_string = prop.getString;
					tProperty.write(cVariable);
				}
				break;
				//---------------------------------------------
			  // 値が４つのプロパティ.
			  //---------------------------------------------
			  case e_WinProperty.POSITION:
				{
                    Vector3[] pairratio = prop.getPairRatio;
                    cVariable.put(pairratio[0]);
                    cVariable.put(pairratio[1]);
				}
				break;
			  case e_WinProperty.POSITION2:
				goto case e_WinProperty.POSITION;
			  case e_WinProperty.CLOSE_POSITION:
				goto case e_WinProperty.POSITION;
			  case e_WinProperty.CAPTION_OFFSET:
				goto case e_WinProperty.POSITION;
			  case e_WinProperty.TEXTURE_OFFSET0:
				goto case e_WinProperty.POSITION;
			  case e_WinProperty.TEXTURE_OFFSET1:
				goto case e_WinProperty.POSITION;
			  case e_WinProperty.TEXTURE_OFFSET2:
				goto case e_WinProperty.POSITION;
			  case e_WinProperty.TEXTURE_OFFSET3:
				goto case e_WinProperty.POSITION;
			  case e_WinProperty.TEXTURE_OFFSET4:
				goto case e_WinProperty.POSITION;
			  case e_WinProperty.TEXTURE_OFFSET5:
				goto case e_WinProperty.POSITION;
			  case e_WinProperty.TEXTURE_OFFSET6:
				goto case e_WinProperty.POSITION;
			  case e_WinProperty.TEXTURE_OFFSET7:
				goto case e_WinProperty.POSITION;
			  case e_WinProperty.TEXTURE_SIZE1:
				goto case e_WinProperty.POSITION;
			  case e_WinProperty.TEXTURE_SIZE2:
				goto case e_WinProperty.POSITION;
			  case e_WinProperty.TEXTURE_SIZE3:
				goto case e_WinProperty.POSITION;
			  case e_WinProperty.TEXTURE_SIZE4:
				goto case e_WinProperty.POSITION;
			  case e_WinProperty.TEXTURE_SIZE5:
				goto case e_WinProperty.POSITION;
			  case e_WinProperty.TEXTURE_SIZE6:
				goto case e_WinProperty.POSITION;
			  case e_WinProperty.TEXTURE_SIZE7:
				goto case e_WinProperty.POSITION;
			  case e_WinProperty.SIZE:
				goto case e_WinProperty.POSITION;
			  case e_WinProperty.SAFEAREA:
				goto case e_WinProperty.SCREEN;
			  case e_WinProperty.CONTENTS_SIZE:
				goto case e_WinProperty.POSITION;
			//---------------------------------------------
			// Screen
			//---------------------------------------------
			  case e_WinProperty.SCREEN:
                {
                    Vector3[] screen = prop.getScreen;
                    cVariable.put(screen[0]);
                    cVariable.put(screen[1]);
                    cVariable.put(screen[2]);
                    cVariable.put(screen[3]);
                }
                break;
              case e_WinProperty.CLOSE_SCALE:
                {
                    cVariable.put(prop.getVector4);
                }
                break;
            //---------------------------------------------
            // テクスチャID.
            //---------------------------------------------
              case e_WinProperty.TEX_ID0:
				{
					t_WinPropertyTexId	tProperty = new t_WinPropertyTexId();
					tProperty.m_texId = (uint) prop.getValue;
					tProperty.m_partId = (uint) prop.getPartsId;
					tProperty.write(cVariable);
				}
				break;
			  case e_WinProperty.TEX_ID1:
				goto case e_WinProperty.TEX_ID0;
			  case e_WinProperty.TEX_ID2:
				goto case e_WinProperty.TEX_ID0;
			  case e_WinProperty.TEX_ID3:
				goto case e_WinProperty.TEX_ID0;
			  case e_WinProperty.TEX_ID4:
				goto case e_WinProperty.TEX_ID0;
			  case e_WinProperty.TEX_ID5:
				goto case e_WinProperty.TEX_ID0;
			  case e_WinProperty.TEX_ID6:
				goto case e_WinProperty.TEX_ID0;
			  case e_WinProperty.TEX_ID7:
				goto case e_WinProperty.TEX_ID0;
			  case e_WinProperty.TEXTURE_ZOFFSET: {
                    t_WinPropertyTextureZOffset tProperty = new t_WinPropertyTextureZOffset();
                    tProperty.m_texId = prop.getPartsId;
                    tProperty.m_zoffset = prop.getValue;
                    tProperty.write(cVariable);
                }
                break;
			  case e_WinProperty.EDIT:
				goto case e_WinProperty.TEX_ID0;
			  //---------------------------------------------
			  // グループ/コンテンツ.
			  //---------------------------------------------
			  case e_WinProperty.CONTENTS:
				{
					CWinNumberList	cNumberList = prop.getNumberList;
					int size = 0;
					if (cNumberList != null) {
						size = cNumberList.size;
					}
					t_WinPropertyArray tProperty = new t_WinPropertyArray();
					tProperty.m_aValue = new uint[size];
					for (int i = 0;i < size;i++) {
						uint id = cNumberList.getValue(dicCtrlId);
						if (id == 0) {
							iResult = -3;
							Debug.LogError("can't find ctlr id:" + cNumberList);
						}
						tProperty.m_aValue[i] = id;

						cNumberList = cNumberList.next;
					}
					tProperty.write(cVariable);
				}
				break;
			  case e_WinProperty.GROUP:
				goto case e_WinProperty.CONTENTS;
			}
		}
		return iResult;
    }
	static void expandCtrl(CWinParserCtrl	cTop) {
		List<CWinParserCtrl> lstCtrl = new List<CWinParserCtrl>();
		for (CWinParserCtrl cCtrl = cTop;cCtrl != null;cCtrl = cCtrl.next) {
			lstCtrl.Add (cCtrl);
		}
		foreach (CWinParserCtrl cCtrl in lstCtrl) {
			if (cCtrl.kind == e_WinCtrlKind.NONE) {
				continue;
			}
			for (CWinParserProperty	prop = cCtrl.property;prop != null;prop = prop.next) {
				if (prop.getContentsList != null) {
					expandCtrl (prop.getContentsList);
					cCtrl.next = prop.getContentsList;
				}
			}
		}
	}

}
