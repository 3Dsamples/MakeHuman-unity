//==============================================================================================
/*!バージョンデータを更新する.
	@file  ExportVersion

	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using KS;

public class ExportVersion {
    class CAsset {
        public CAsset(t_AssetVersionOne _one) {
            one = _one;
        }
        public t_AssetVersionOne    one;
        public bool                 isDelete = true;
        public bool                 isExist = false;
        public bool isEqual(ulong uMd5a,ulong uMd5b) {
            if (one.m_uMd5a == uMd5a && one.m_uMd5b == uMd5b) {
                return true;
            }
            return false;
        }
        public void update(ulong uMd5a,ulong uMd5b,string path,byte[] buffer) {
            one.m_path = path;
            one.m_uMd5a = uMd5a;
            one.m_uMd5b = uMd5b;
            if (path.ToLower().EndsWith(".jpg") || path.ToLower().EndsWith(".png")) {
                Texture2D tex = new Texture2D(2,2);
                if (!tex.LoadImage(buffer)) {
                    Debug.LogError("this file is not image:" + path);
                    return;
                }
                one.m_width = (ushort) tex.width;
                one.m_height = (ushort) tex.height;
            }
        }
    }
	class CParameter {
		public int	 isRemain = 0;
        public int   downloadType = 0;
        public uint	flag {
			get {
				e_ResourceFlag	eFlag = e_ResourceFlag.NONE;
				if (isRemain != 0) {
					eFlag |= e_ResourceFlag.REMAIN;
				}
                if (downloadType == 1) {
                    eFlag |= e_ResourceFlag.DOWNLOAD_STREAMING;
                } else if (downloadType == 2) {
                    eFlag |= e_ResourceFlag.DOWNLOAD_NETWORK;
                }
                return (uint) eFlag;
			}
		}
	};
	//==========================================================================
	/*!export
		@brief	特定のターゲットに出力する.
	*/
	static public bool export(BuildTarget eTarget) {
		// 現在の時間からバージョンIDを取得.
		// version.defを読み込む.
		Dictionary<string,CParameter> dicParameter = readParameter("version.def");

		// アセットバンドルのパスを取得する.
        string path = CreateAssetbundles.getAssetPath(eTarget);
        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
		}
		// version.assetbundeが存在しているなら読み込む.
		CAssetVersion	cVersion = new CAssetVersion();
        string versionfile = CreateAssetbundles.getAssetPath("version", eTarget);
		Debug.Log (versionfile);
		string versiontxt = versionfile.Replace(KsSoftConfig.AssetbundleExt,".txt");
        byte[] oldBuffer = null;
		if (File.Exists(versionfile)) {
			AssetBundle	ab = AssetBundle.LoadFromFile(versionfile);
			if (ab == null) {
				Debug.LogError ("can't read " + versionfile);
				return false;
			}
			CSerializableScript cSS = ab.LoadAsset<CSerializableScript>("version");
			if (cSS != null) {
				if (cVersion.read(cSS.m_buffer) != 0) {
					cVersion = null;
				}
				oldBuffer = cSS.m_buffer;
			}
			ab.Unload(true);
		}
		uint versionAsset,versionClient;
		Dictionary<string,int>	dicVersion = readVersion(versiontxt,out versionAsset,out versionClient);

		int iVersion = (int) ((KsSoftUtility.getUnixTime() - 1000000000) & 0xffffffff);
		// 過去に存在していたアセットバンドルの辞書を作成する.
		Dictionary<string,CAsset>	dicAsset = new Dictionary<string, CAsset>();
		foreach (t_AssetVersionOne avo in cVersion.Versions) {
			dicAsset[avo.m_path] = new CAsset(avo);
		}
		// 確認用TXTファイルを開く.
		StreamWriter	swTxt = new StreamWriter(versiontxt);

        // アセットバンドルが格納されているディレクトリの内、*.unity3dを列挙する.
        List<string> lstFiles = new List<string>();
        getFiles(lstFiles,path,KsSoftConfig.AssetbundleExt);
        getFiles(lstFiles,KsSoftConfig.AssetbundlePath + "image/",".jpg");
        getFiles(lstFiles,KsSoftConfig.AssetbundlePath + "image/",".png");

        swTxt.Write("Asset  Version\t" + KsSoftConfig.VERSION_ASSET + "\n");
		swTxt.Write("Client Version\t" + KsSoftConfig.VERSION_CLIENT + "\n");
		// 各アセットのMD5を調べ更新が必要かチェック.
		int		num = 0;
		foreach (string file in lstFiles) {
            byte[] buffer = KsSoftUtility.load(file);
			MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
			byte[] md5hash = md5.ComputeHash(buffer);
			CReadVariable	cVariable = new CReadVariable(md5hash);
			ulong uMd5a	= cVariable.getU64();
			ulong uMd5b	= cVariable.getU64();
			char	mode = 'N';
            path = System.IO.Path.GetFileName(file);
            if (dicAsset.ContainsKey(path)) {
				// すでに存在している.
				CAsset	cAsset = dicAsset[path];
				cAsset.isDelete = false;
				cAsset.isExist = true;
				num++;
				if (!cAsset.isEqual(uMd5a,uMd5b)) {
                    // MD5が異なるときはバージョンを進める.
                    cAsset.update(uMd5a,uMd5b,path,buffer);
					if (dicVersion.ContainsKey(path)) {
						cAsset.one.m_iVersion = iVersion;
					} else {
						cAsset.one.m_iVersion = iVersion;
					}
					mode = 'U';
				} else {
					// バージョンの強制上書き.
					if (dicVersion.ContainsKey(path) && dicVersion[path] > cAsset.one.m_iVersion) {
						cAsset.one.m_iVersion = dicVersion[path];
						mode = 'F';
					}
				}
			} else {
				// 新規のアセット.
				t_AssetVersionOne avo = new t_AssetVersionOne();
                avo.m_mId = 0;
                string name = System.IO.Path.GetFileNameWithoutExtension(path);
                if (MulId.isMulId(name)) {
                    avo.m_mId = new MulId(name);
                }
				avo.m_uFlag = 0;
				if (dicVersion.ContainsKey(path)) {
					avo.m_iVersion = iVersion;
				} else {
					avo.m_iVersion = iVersion;
				}
				CAsset cAsset= new CAsset(avo);
                cAsset.update(uMd5a,uMd5b,path,buffer);
                cAsset.isDelete = false;
				cAsset.isExist = true;
				num++;
				dicAsset[path] = cAsset;
				mode = 'A';
			}
			t_AssetVersionOne target = dicAsset[path].one;
			// 設定データがあるなら反映させる.
			CParameter	cParam;
			if (dicParameter.TryGetValue(path,out cParam)) {
				if (target.m_uFlag != cParam.flag) {
					target.m_uFlag = cParam.flag;
				}
			} else {
                target.m_uFlag = 0;
            }
			// 確認用テキストに出力する.
//			string md5str = target.m_uMd5a.ToString("X8") + target.m_uMd5b.ToString("X8");
			swTxt.Write(mode.ToString() + '\t' + path + "\t" + target.m_uFlag + "\t" + target.m_iVersion + "\n");
		}
		swTxt.Close();
		// アセットを調べ、CAssestVersionに追加していく.
		// 削除されているものはメッセージを出力する.
		CAssetVersion	cNewVersion = new CAssetVersion();
		List<t_AssetVersionOne>	lstAVO = new List<t_AssetVersionOne>();
		foreach (CAsset cAsset in dicAsset.Values) {
			if (!cAsset.isExist) {
				Debug.Log("deleted " + cAsset.one.m_path + "(" + new MulId(cAsset.one.m_mId) + ")");
				continue;
			}
			lstAVO.Add (cAsset.one);
		}
		cNewVersion.Versions = lstAVO.ToArray();
		// アセットバンドル化用のインスタンス.
		CSerializableScript ss = ScriptableObject.CreateInstance<CSerializableScript>();

		// バッファからアセットを生成.
		cNewVersion.write(ref ss.m_buffer);
        // 生成されたversion情報が古いものと一致しているか調べる.
        if (oldBuffer != null && ss.m_buffer.Length == oldBuffer.Length) {
            bool bEqual = true;
            for (int i = 0;i < oldBuffer.Length;++i) {
                if (oldBuffer[i] != ss.m_buffer[i]) {
                    bEqual = false;
                    break;
                }
            }
            if (bEqual) {
                Debug.Log("skip then this folder is not updated yet");
                return true;
            }
        }
		string assetPath = "Assets/version.asset";
		AssetDatabase.CreateAsset(ss, assetPath);

		Debug.Log("************* export " + versionfile + " *****************");
		// アセットバンドルにパックする.
		AssetBundleBuild[]	aABB = new AssetBundleBuild[1];
		aABB[0].assetBundleName = "version" + KsSoftConfig.AssetbundleExt;
		aABB[0].assetNames = new string[] {	assetPath	};
		BuildPipeline.BuildAssetBundles(CreateAssetbundles.getAssetPath(eTarget),aABB,BuildAssetBundleOptions.ChunkBasedCompression,eTarget);

        // 削除.
		AssetDatabase.DeleteAsset(assetPath);
		return true;
	}
    //==========================================================================
    /*!ファイルを取得する.
		@brief	getFiles.
	*/
    static void getFiles(List<string> lstPath,string path,string ext) {
        if (!System.IO.Directory.Exists(path)) {
            return;
        }
        string[] files = Directory.GetFiles(path,"*" + ext);
        for (int j = 0;j < files.Length;++j) {
            if (files[j].EndsWith(ext)) {
                if (files[j].Contains("version")) {
                    continue;
                }
                    lstPath.Add(files[j]);
            }
        }
    }
    //==========================================================================
    /*!readParameter
		@brief	パラメータを読み込む.
	*/
    static Dictionary<string,CParameter> readParameter(string sDefine) {
		Dictionary<string,CParameter>	dcResult = new Dictionary<string,CParameter>();
		if (!File.Exists(sDefine)) {
			return dcResult;
		}
		Debug.Log("read " + sDefine);
		StreamReader sr;
		try {
			sr = new StreamReader(sDefine);
		}
	    catch( FileNotFoundException e) {
			Debug.Log(e);
			return  dcResult;
		}
		while (sr.EndOfStream == false) {
		    //一行読み込んで解析する.
			string	line = sr.ReadLine().Trim();
			// 空行若しくはコメント行.
			if (line.Length == 0 || line[0] == '#') {
				continue;
			}
			if (line.Contains("=")) {
				string[] aSplit = line.Split(new char[] {'=',' ','\t'},System.StringSplitOptions.RemoveEmptyEntries);
				// 空行.
				if (aSplit.Length == 0) {
					continue;
				}
				// フォーマットエラーチェック.
				if (aSplit.Length != 2) {
					Debug.LogError("format error:" + line);
					continue;
				}
				string path = aSplit[0];
				// 登録.
				string[] aArg = aSplit[1].Split(new char[] {',',';',' ','\t'},System.StringSplitOptions.RemoveEmptyEntries);
				CParameter	cParameter = new CParameter();
				foreach (string arg in aArg) {
					string[] aCmd = arg.Split(new char[] {':',' ','\t'},System.StringSplitOptions.RemoveEmptyEntries);
					if (aCmd[0] == "REMAIN") {
						if (!int.TryParse(aCmd[1],out cParameter.isRemain)) {
							Debug.LogError("format error:" + line);
						}
					} else if (aCmd[0] == "DOWNLOADTYPE") {
                        if (aCmd[1] == "D") {
                            cParameter.downloadType = 0;
                        } else if (aCmd[1] == "S") {
                            cParameter.downloadType = 1;
                        } else if (aCmd[1] == "N") {
                            cParameter.downloadType = 2;
                        } else {
                            Debug.LogError("DOWNLOADTYPE is illegal:D = Default,S = Streaming,N = Network");
                        }
                    }
				}
				dcResult[path] = cParameter;
			}
		}
		sr.Close();
		return dcResult;
	}
	static Dictionary<string,int>	readVersion(string file,out uint versionAsset,out uint versionClient) {
		StreamReader	sr;
		Dictionary<string,int>	dicVersion = new Dictionary<string,int>();
		versionAsset = 0;
		versionClient = 0;
        Debug.Log(file);
		try {
			sr = new StreamReader(file);
		}
	    catch( FileNotFoundException e) {
			Debug.Log(e);
			return dicVersion;
		}
		while (sr.EndOfStream == false) {
		    //一行読み込んで解析する.
			string	line = sr.ReadLine().Trim();
			// 空行若しくはコメント行.
			if (line.Length == 0 || line[0] == '#' || line[0] == '=' || line[0] == '>'  || line[0] == '<') {
				continue;
			}
			string[] aArg = line.Split(new char[] {'\t'});
			if (aArg[0] == "Asset  Version") {
				versionAsset = UInt32.Parse (aArg[1]);
			} else if (aArg[0] == "Client Version") {
				versionClient = UInt32.Parse (aArg[1]);
			} else {
				string path = System.IO.Path.GetFileNameWithoutExtension(aArg[1]);
				int  iVersion = Int32.Parse(aArg[3]);
				if (dicVersion.ContainsKey(path)) {
					if (dicVersion[path] < iVersion) {
						dicVersion[path] = iVersion;
					}
				} else {
					dicVersion[path] = iVersion;
				}
			}
		}
		sr.Close();
		return dicVersion;
	}
}
