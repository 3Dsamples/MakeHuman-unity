//==============================================================================================
/*!テクスチャリソースをエキスポートする.
	@file  ExportTextureResource
	(counter SJIS string 京.)
*/
//==============================================================================================
//#define EXPORT_SPR_BIN      //外部ファイルとして、sprのバイナリデータを出力する.
//#define	EXPORT_PNG
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using KS;


public class ExportTextureResource
{
#if EXPORT_SPR_BIN
    static string  sExportSprBinPath = "bin/";
#endif
    static string sPartsDef		 = "@parts.def";
	static TextureFormat[]	aSquareTextureFormat = new TextureFormat[] {
		TextureFormat.PVRTC_RGB2,
		TextureFormat.PVRTC_RGB4,
		TextureFormat.PVRTC_RGBA2,
		TextureFormat.PVRTC_RGBA4,
	};
    static public bool export(BuildTarget eTarget) {
		Object[] aObject = Selection.GetFiltered(typeof (Object), SelectionMode.DeepAssets);
		if (aObject.Length != 1) {
			return false;
		}
		string sPath = AssetDatabase.GetAssetPath(aObject[0]);
		return export(eTarget,Path.GetDirectoryName(sPath));
	}
	struct t_TexturePart {
		public uint			id;
		public Texture2D	texture;
		public Color32		color;
		public e_Patch		ePatch;
		public Vector4		uv;
		public Vector4		divide;
		public int			dither;
	}
	//==========================================================================
	/*!テクスチャデータエキスポート.
		@brief	export
	*/
	static public bool export( BuildTarget eTarget,string sBasePath) {
		string sAssetPath = CreateAssetbundles.getAssetPath("", eTarget);
		if (!Directory.Exists(sAssetPath)) {
			Directory.CreateDirectory(sAssetPath);
		}
        // 出力IDを取得する.
        string[] aName = sBasePath.Split(new char [] {'/','\\'},System.StringSplitOptions.RemoveEmptyEntries);
        if (aName.Length < 3) {
            Debug.LogError("name is illegal:" + sBasePath);
            return false;
        }
        MulId   mId = new MulId(aName[aName.Length - 1]);
        if (mId == 0) {
            Debug.LogError("this folder is not multi id!!:" + sBasePath);
            return false;
        }
        // ベースパスに存在するテクスチャを列挙する.
        List<string> lstTexturePath = EditorHelpers.CollectAllAssetPath<Texture2D>(sBasePath,true);
        // 設定ファイル読み込み.
        bool    bResource = false;
        string  sShader = "";
        TextureFormat   format = TextureFormat.RGBA4444;
        FilterMode      filterMode = FilterMode.Bilinear;
        int exportType = 0;
        string path = sBasePath + "/" + sPartsDef;
        CTexParser.Parser   parser = null;
        if (File.Exists(path)) {
            parser = readPartsDef(path);
            if (parser == null) {
                return false;
            }
            bResource = parser.isResource;
            sShader = parser.shader;
            exportType = parser.exportType;
            format = parser.format;
            filterMode = parser.filter;
            foreach (string ip in parser.importpath) {
                string importpath = sBasePath + "/" + ip;
                string[] files = Directory.GetFiles(Path.GetDirectoryName(importpath), Path.GetFileName(importpath));
                for (int i = 0; i < files.Length; ++i) {
                    string file = Path.GetFullPath(files[i]);
                    file = file.Remove(0, file.IndexOf("\\Assets\\TextureResource\\") + 1);
                    Texture2D asset = (Texture2D) AssetDatabase.LoadAssetAtPath(file, typeof(Texture2D));
                    if (asset == null) {
                        continue;
                    }
                    lstTexturePath.Add(file);
                }
            }
        }
        // テクスチャパーツの情報を列挙する.
		List<t_TexturePart> lstTexturePart = new List<t_TexturePart>();
		List<Texture2D>	lstTexture = new List<Texture2D>();

		for (int i = 0;i < lstTexturePath.Count;++i) {
			t_TexturePart	tPart;
			path = lstTexturePath[i];
			tPart.id = EditorHelpers.ConvertId(Path.GetFileNameWithoutExtension(path));
			if (tPart.id == 0) {
				Debug.LogError ("id format is illegal:" + path);
				continue;
			}
			tPart.texture = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
			tPart.ePatch = e_Patch.None;
			tPart.uv = Vector4.zero;
			tPart.divide = Vector4.zero;
			tPart.divide.x = tPart.texture.width;
			tPart.divide.y = tPart.texture.height;
			tPart.dither = 1;
			tPart.color = new Color32(128,128,128,255);
			lstTexturePart.Add (tPart);
			lstTexture.Add (tPart.texture);
		}
		List<string> lstPath = new List<string>();
		// アトラス化.
		bool	bForceSquare = false;
		if (System.Array.IndexOf(aSquareTextureFormat,format) >= 0) {
			bForceSquare = true;
		}
		TextureAtlas.Result	atlas = TextureAtlas.packTexture(lstTexture,8,2048,bForceSquare,true);
		// アトラス化した結果のUVを格納.
		float	w = (float) atlas.texture.width;
		float	h = (float) atlas.texture.height;
        bool[,] aDitherMask = new bool[atlas.texture.width,atlas.texture.height];
        for (int i = 0;i < lstTexturePart.Count;++i) {
            Rect r = atlas.aRect[i];
            t_TexturePart tPart = lstTexturePart[i];
            tPart.uv = new Vector4(r.x,r.y,r.x + r.width,r.y + r.height);
            int rx = (int) (r.width * w);
            int ry = (int) (r.height * h);

            if (rx != tPart.texture.width || ry != tPart.texture.height) {
                Debug.LogError("texture was resized why texture part is too much!!:" + w + "," + h + ":" + new FiveCC(tPart.id) + "(" + rx + "," + ry + ")-(" + tPart.texture.width + "," + tPart.texture.height + ")");
                return false;
            }
            lstTexturePart[i] = tPart;
        }
        // 設定ファイルがあるなら読み込む.
        Dictionary<uint,t_TexturePart> dcPart = analyzePartsDef(parser,lstTexturePart,aDitherMask);
        if (dcPart == null) {
            Debug.LogError("can't read parts datas");
            return false;
        }
        // ディザーが必要かどうかチェック.
        bool bNeedDither = false;
        foreach (t_TexturePart tPart in dcPart.Values) {
            if (tPart.dither > 0) {
                bNeedDither = true;
                break;
            }
        }
        // ディザー実行.
        if (bNeedDither) {
            atlas.texture = TextureAtlas.dither(atlas.texture,aDitherMask);
        }
        if (atlas.texture.format != format) {
			EditorUtility.CompressTexture(atlas.texture,format,
#if UNITY_2018_3_OR_NEWER
                UnityEditor.TextureCompressionQuality.Best);
#else
                UnityEngine.TextureCompressionQuality.Best);
#endif
		}
		atlas.texture.filterMode = filterMode;
		CSerializableScript cSS = ScriptableObject.CreateInstance<CSerializableScript>();
		// 設定データから構築.
		t_SpriteData	tSD = new t_SpriteData();
		tSD.m_id = mId;
		tSD.m_sShader = sShader;
		tSD.m_aData = new t_SpriteDataOne[dcPart.Count];
		int	partindex = 0;
		foreach (t_TexturePart tPart in dcPart.Values) {
			t_SpriteDataOne	tSDO = new t_SpriteDataOne();
			tSD.m_aData[partindex++] = tSDO;
			tSDO.m_id = tPart.id;
			tSDO.m_ePatch = (sbyte) tPart.ePatch;
			tSDO.m_color = (uint) (((int) tPart.color.a << 24)|((int) tPart.color.r << 16)|((int) tPart.color.g << 8)|(tPart.color.b));
			switch (tPart.ePatch) {
			  case e_Patch.None:
				tSDO.m_aUV = new Vector4[1];
				tSDO.m_aUV[0] = tPart.uv;
				break;
			  case e_Patch.H3:
				tSDO.m_aUV = new Vector4[3];
				tSDO.m_aUV[0].x = tPart.uv.x;
				tSDO.m_aUV[0].y = tPart.uv.y;
				tSDO.m_aUV[0].z = tPart.divide.x;
				tSDO.m_aUV[0].w = tPart.uv.w;

				tSDO.m_aUV[1].x = tPart.divide.x;
				tSDO.m_aUV[1].y = tPart.uv.y;
				tSDO.m_aUV[1].z = tPart.divide.z;
				tSDO.m_aUV[1].w = tPart.uv.w;

				tSDO.m_aUV[2].x = tPart.divide.z;
				tSDO.m_aUV[2].y = tPart.uv.y;
				tSDO.m_aUV[2].z = tPart.uv.z;
				tSDO.m_aUV[2].w = tPart.uv.w;
				break;
			  case e_Patch.V3:
				tSDO.m_aUV = new Vector4[3];
				tSDO.m_aUV[0].x = tPart.uv.x;
				tSDO.m_aUV[0].y = tPart.uv.y;
				tSDO.m_aUV[0].z = tPart.uv.z;
				tSDO.m_aUV[0].w = tPart.divide.y;

				tSDO.m_aUV[1].x = tPart.uv.x;
				tSDO.m_aUV[1].y = tPart.divide.y;
				tSDO.m_aUV[1].z = tPart.uv.z;
				tSDO.m_aUV[1].w = tPart.divide.w;

				tSDO.m_aUV[2].x = tPart.uv.x;
				tSDO.m_aUV[2].y = tPart.divide.w;
				tSDO.m_aUV[2].z = tPart.uv.z;
				tSDO.m_aUV[2].w = tPart.uv.w;
				break;
			  case e_Patch.HV9:
				tSDO.m_aUV = new Vector4[9];
				// bottom
				tSDO.m_aUV[6].x = tPart.uv.x;
				tSDO.m_aUV[6].y = tPart.uv.y;
				tSDO.m_aUV[6].z = tPart.divide.x;
				tSDO.m_aUV[6].w = tPart.divide.y;

				tSDO.m_aUV[7].x = tPart.divide.x;
				tSDO.m_aUV[7].y = tPart.uv.y;
				tSDO.m_aUV[7].z = tPart.divide.z;
				tSDO.m_aUV[7].w = tPart.divide.y;

				tSDO.m_aUV[8].x = tPart.divide.z;
				tSDO.m_aUV[8].y = tPart.uv.y;
				tSDO.m_aUV[8].z = tPart.uv.z;
				tSDO.m_aUV[8].w = tPart.divide.y;
				// center
				tSDO.m_aUV[3].x = tPart.uv.x;
				tSDO.m_aUV[3].y = tPart.divide.y;
				tSDO.m_aUV[3].z = tPart.divide.x;
				tSDO.m_aUV[3].w = tPart.divide.w;

				tSDO.m_aUV[4].x = tPart.divide.x;
				tSDO.m_aUV[4].y = tPart.divide.y;
				tSDO.m_aUV[4].z = tPart.divide.z;
				tSDO.m_aUV[4].w = tPart.divide.w;

				tSDO.m_aUV[5].x = tPart.divide.z;
				tSDO.m_aUV[5].y = tPart.divide.y;
				tSDO.m_aUV[5].z = tPart.uv.z;
				tSDO.m_aUV[5].w = tPart.divide.w;

				// top
				tSDO.m_aUV[0].x = tPart.uv.x;
				tSDO.m_aUV[0].y = tPart.divide.w;
				tSDO.m_aUV[0].z = tPart.divide.x;
				tSDO.m_aUV[0].w = tPart.uv.w;

				tSDO.m_aUV[1].x = tPart.divide.x;
				tSDO.m_aUV[1].y = tPart.divide.w;
				tSDO.m_aUV[1].z = tPart.divide.z;
				tSDO.m_aUV[1].w = tPart.uv.w;

				tSDO.m_aUV[2].x = tPart.divide.z;
				tSDO.m_aUV[2].y = tPart.divide.w;
				tSDO.m_aUV[2].z = tPart.uv.z;
				tSDO.m_aUV[2].w = tPart.uv.w;
				break;
			}
		}
		CWriteVariable	cVariable = new CWriteVariable(4096);
		tSD.write (cVariable);
		cSS.m_buffer = cVariable.copybuffer();
#if EXPORT_SPR_BIN
        EditorHelpers.write(sExportSprBinPath + mId + ".bin",cVariable);
#endif
        atlas.texture.filterMode = filterMode;
		if (bResource) {
			path = KsSoftConfig.ResourcesPath + mId;
			if (exportType == 1) {
				// PNGファイルとして出力.
				CSerializableScript	cImageSS = ScriptableObject.CreateInstance<CSerializableScript>();
				cImageSS.m_buffer = atlas.texture.EncodeToPNG();
				AssetDatabase.CreateAsset(cImageSS,path + ".img.asset");
			} else if (exportType == 2) {
				// JPGファイルとして出力.
				CSerializableScript	cImageSS = ScriptableObject.CreateInstance<CSerializableScript>();
				cImageSS.m_buffer = atlas.texture.EncodeToJPG();
				AssetDatabase.CreateAsset(cImageSS,path + ".img.asset");
			} else {
				// RAWファイルとして出力.
				AssetDatabase.CreateAsset(atlas.texture,path + ".tex.asset");
			}
			AssetDatabase.CreateAsset(cSS,path + ".spr.asset");
			Debug.Log("******* export resource data for texture resource: " + mId.ToString() + " *******");
		} else {
			// テクスチャをアセット化.
			string assetPath = "Assets/texture.asset";
			if (exportType == 1) {
				// PNGファイルとして出力.
				assetPath = "Assets/image.asset";
				CSerializableScript	cImageSS = ScriptableObject.CreateInstance<CSerializableScript>();
				cImageSS.m_buffer = atlas.texture.EncodeToPNG();
				AssetDatabase.CreateAsset(cImageSS,assetPath);
			} else if (exportType == 2) {
				// JPGファイルとして出力.
				assetPath = "Assets/image.asset";
				CSerializableScript	cImageSS = ScriptableObject.CreateInstance<CSerializableScript>();
				cImageSS.m_buffer = atlas.texture.EncodeToJPG();
				AssetDatabase.CreateAsset(cImageSS,assetPath);
			} else {
				// RAWファイルとして出力.
				AssetDatabase.CreateAsset(atlas.texture, assetPath);
			}
			lstPath.Add(assetPath);
	
	
			assetPath = "Assets/sprite.asset";
			AssetDatabase.CreateAsset(cSS, assetPath);
			lstPath.Add(assetPath);

			// アセットバンドル化.
			AssetBundleBuild[]	aABB = new AssetBundleBuild[1];
			
			aABB[0].assetBundleName = mId + KsSoftConfig.AssetbundleExt;
			aABB[0].assetNames = lstPath.ToArray();

			// アセットをアセットバンドルにパックする.
			BuildPipeline.BuildAssetBundles(CreateAssetbundles.getAssetPath(eTarget),aABB,BuildAssetBundleOptions.ChunkBasedCompression,eTarget);

            CreateAssetbundles.DeleteAssets(lstPath.ToArray());
			Debug.Log("******* export assetbundles for texture resource: " + mId.ToString() + " *******");
		}
    	return true;
    }

	//==========================================================================
	/*!パーツ情報を読み込む.
		@brief	readPartsDef
	*/
	static 	CTexParser.Parser	readPartsDef(string path) {
		if (!File.Exists(path)) {
			return null;
		}
		Debug.Log ("find " + sPartsDef + " at " + path);
		CTexParser.Parser parser = CTexParser.Parser.compile(path);
		if (parser == null) {
			Debug.LogError("compile failure:" + path);
			return null;
		}
		return parser;
	}
	//==========================================================================
	/*!パーツ情報を解析する.
		@brief	analyzePartsDef
	*/
	static 	Dictionary<uint,t_TexturePart>	analyzePartsDef(CTexParser.Parser parser,List<t_TexturePart> lstPart,bool[,] aDitherMask) {
		Dictionary<uint,t_TexturePart> dcPart = new Dictionary<uint, t_TexturePart>();
		// 設定ファイルがあるかチェック.
		if (parser == null) {
			foreach (t_TexturePart tPart in lstPart) {
				dcPart[tPart.id] = tPart;
			}
			return dcPart;
		}
		float	width = (float) aDitherMask.GetLength(0);
		float	height = (float) aDitherMask.GetLength(1);
		bool	bError = false;
		for (CTexParserPart	cPart = parser.topPart;cPart != null;cPart = cPart.next) {
			t_TexturePart	tPart;
			if (cPart.id == 0) {
				continue;
			}
			if (!find(out tPart,cPart.texid,lstPart)) {
				bError = true;
				Debug.LogError("can't find texture part:" + new FiveCC(cPart.texid));
				continue;
			}
			Vector4		div = Vector4.zero;
			Color32		color = new Color32(128,128,128,255);
			int dither = (parser.isDither)? 1:0;
			e_Patch	ePatch = e_Patch.None;
			for (CTexParserProperty prop = cPart.property;prop != null;prop = prop.next) {
				switch (prop.property) {
				  case e_TexProperty.NONE:
					break;
				  case e_TexProperty.NODIVIDE:
					ePatch = e_Patch.None;
					div = Vector4.zero;
					break;
				  case e_TexProperty.DIVIDE3H:
					ePatch = e_Patch.H3;
					div = new Vector4(tPart.uv.x + prop.getVector2.x/width,0f,tPart.uv.z - prop.getVector2.y/width,0f);
					break;
				  case e_TexProperty.DIVIDE3V:
					ePatch = e_Patch.V3;
					div = new Vector4(0f,tPart.uv.y + prop.getVector2.x/height,0f,tPart.uv.w - prop.getVector2.y/height);
					break;
				  case e_TexProperty.DIVIDE9:
					ePatch = e_Patch.HV9;
					div = prop.getVector4;
					div.x /= width;
					div.y /= height;
					div.z /= width;
					div.w /= height;
					float y = div.y;
					div.y = div.w;
					div.w = y;
					div.x += tPart.uv.x;
					div.y += tPart.uv.y;
					div.z = tPart.uv.z - div.z;
					div.w = tPart.uv.w - div.w;
					break;
				  case e_TexProperty.DITHER:
					dither = prop.Value;
					break;
				  case e_TexProperty.COLOR:
					Vector4	col = prop.getVector4;
					color = new Color32((byte) Mathf.Clamp(col.x,0f,255f),(byte) Mathf.Clamp(col.y,0f,255f),(byte) Mathf.Clamp(col.z,0f,255f),(byte) Mathf.Clamp(col.w,0f,255f));
					break;
				}
			}
			tPart.id = cPart.id;
			tPart.ePatch = ePatch;
			tPart.divide = div;
			tPart.dither = dither;
			tPart.color = color;
			if (dcPart.ContainsKey(cPart.id)) {
				Debug.LogError ("already defined parts id:" + new FiveCC(cPart.id));
				bError = true;
				continue;
			}
			dcPart[cPart.id] = tPart;
			
		}
		if (bError) {
			return null;
		}
		// 残りを回収.
		foreach (t_TexturePart part in lstPart) {
			t_TexturePart	tPart;
			if (dcPart.ContainsKey(part.id)) {
				continue;
			}
			tPart = part;
			tPart.divide = Vector4.zero;
			tPart.dither = (parser.isDither)? 1:0;
			dcPart[part.id] = tPart;
		}
		foreach (t_TexturePart part in dcPart.Values) {
			if (part.dither == 0) {
				continue;
			}
			int	sx = (int) (part.uv.x * width);
			int sy = (int) (part.uv.y * height);
			int	ex = (int) (part.uv.z * width);
			int ey = (int) (part.uv.w * height);
			
			for (int y = sy;y < ey;++y) {
				for (int x = sx;x < ex;++x) {
					aDitherMask[x,y] = true;
				}
			}
		}
		return dcPart;
	}
	static bool	find(out t_TexturePart rPart,uint id,List<t_TexturePart> lstPart) {
		foreach (t_TexturePart tPart in lstPart) {
			if (tPart.id == id) {
				rPart = tPart;
				return true;
			}
		}
		rPart.ePatch = e_Patch.None;
		rPart.id = 0;
		rPart.texture = null;
		rPart.uv = Vector4.zero;
		rPart.divide = Vector4.zero;
		rPart.dither = 0;
		rPart.color = new Color32(128,128,128,255);
		return false;
	}
}

