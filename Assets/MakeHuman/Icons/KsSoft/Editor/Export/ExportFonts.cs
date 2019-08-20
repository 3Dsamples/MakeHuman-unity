 //==============================================================================================
/*!フォント出力.
	@file  ExportFonts
	(counter SJIS string 京)
*/
//==============================================================================================
#define COMPRESS
using UnityEditor;
using UnityEngine;

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using KS;

public class ExportFonts 
{
    static public void export(BuildTarget eTarget,string sBasePath) {
        if (!System.IO.Directory.Exists(sBasePath)) {
            return;
        }
		string[] aName = sBasePath.Split(new char[] { '/', '\\' }, System.StringSplitOptions.RemoveEmptyEntries);
		if (aName.Length < 3) {
			Debug.LogError("name is illegal:" + sBasePath);
			return;
		}
		MulId mId = new MulId(aName[aName.Length - 1]);
		if (mId == 0) {
			Debug.LogError("this folder is not multi id!!:" + sBasePath);
			return;
		}
        List<string> lstFontPath = EditorHelpers.CollectAllAssetPath<TextAsset>(sBasePath,true);
		List<string> lstPath = new List<string>();
		List<t_SpriteFont> lstFont = new List<t_SpriteFont>();
		string assetPath;
		foreach (string name in lstFontPath) {
			string sPath = System.IO.Path.GetDirectoryName(name);
			string sId = System.IO.Path.GetFileNameWithoutExtension(name);
			TextAsset	asset = (TextAsset) AssetDatabase.LoadAssetAtPath(name, typeof(TextAsset));
			FontParser	fontParser = new FontParser(asset);
			t_SpriteFont font = fontParser.spriteFont;
			font.m_id = new FiveCC(sId);
			font.m_textureName = sId + "_tex";
			lstFont.Add (font);
			Texture2D	texture = new Texture2D(font.m_texWidth,font.m_texHeight,TextureFormat.ARGB32,false);
			
			Texture2D[]	aTexFont = new Texture2D[fontParser.pages.Count];
			for (int i = 0;i < fontParser.pages.Count;i++) {
				string file = sPath + "/" + fontParser.pages[i];
				aTexFont[i] = (Texture2D) AssetDatabase.LoadAssetAtPath(file,typeof(Texture2D));
				if (aTexFont[i] == null) {
					Debug.LogError ("can't find texture asset:" + file);
					continue;
				}
			}
			for (int y = 0;y < texture.height;y++) {
				for (int x = 0;x < texture.width;x++) {
					Color	bc = Color.black;
					for (int i = 0;i < aTexFont.Length;i++) {
						Color	c = aTexFont[i].GetPixel(x,y);
						switch (i) {
						  case 0:
							bc.r = c.a;
							break;
						  case 1:
							bc.g = c.a;
							break;
						  case 2:
							bc.b = c.a;
							break;
						  case 3:
							bc.a = c.a;
							break;
						}
					}
					texture.SetPixel(x,y,bc);
				}
			}
#if COMPRESS
            EditorUtility.CompressTexture(texture, TextureFormat.RGBA4444,
#if UNITY_2018_3_OR_NEWER
                UnityEditor.TextureCompressionQuality.Best);
#else
                UnityEngine.TextureCompressionQuality.Best);
#endif
#endif
            // テクスチャをアセット化.
            assetPath = "Assets/" + font.m_textureName + ".asset";
			AssetDatabase.CreateAsset(texture, assetPath);
			lstPath.Add(assetPath);
		}
		t_SpriteFonts tSpriteFonts = new t_SpriteFonts();
		tSpriteFonts.m_aFont = lstFont.ToArray();
		// フォントデータをシリアライズ.
		CWriteVariable	cVariable = new CWriteVariable(1024 * 1024);
		tSpriteFonts.write (cVariable);
			
		CSerializableScript cSS = ScriptableObject.CreateInstance<CSerializableScript>();
		// バッファからアセットを生成.
		cSS.m_buffer = cVariable.copybuffer();
		assetPath = "Assets/fonts.asset";
		AssetDatabase.CreateAsset(cSS, assetPath);
		lstPath.Add(assetPath);

		AssetBundleBuild[]	aABB = new AssetBundleBuild[1];
		
		aABB[0].assetBundleName = mId + KsSoftConfig.AssetbundleExt;
		aABB[0].assetNames = lstPath.ToArray();
		
		// アセットをアセットバンドルにパックする.
		BuildPipeline.BuildAssetBundles(CreateAssetbundles.getAssetPath(eTarget),aABB,BuildAssetBundleOptions.ChunkBasedCompression,eTarget);

		foreach (string sPath in lstPath) {
			AssetDatabase.DeleteAsset(sPath);
		}
	}
	static public void export(BuildTarget eTarget,TextAsset txtAsset) {
		string sName = AssetDatabase.GetAssetPath(txtAsset);

		string sPath = System.IO.Path.GetDirectoryName(sName);
		string sId = System.IO.Path.GetFileNameWithoutExtension(sName);

		FontParser	fontParser = new FontParser(txtAsset);

		t_SpriteFont font = fontParser.spriteFont;
		font.m_id = new FiveCC(sId);
		font.m_textureName = sId;
		Texture2D	texture = new Texture2D(font.m_texWidth,font.m_texHeight,TextureFormat.ARGB32,false);
			
		Texture2D[]	aTexFont = new Texture2D[fontParser.pages.Count];
		for (int i = 0;i < fontParser.pages.Count;i++) {
			string file = sPath + "/" + fontParser.pages[i];
			aTexFont[i] = (Texture2D) AssetDatabase.LoadAssetAtPath(file,typeof(Texture2D));
			if (aTexFont[i] == null) {
				Debug.LogError ("can't find texture asset:" + file);
				continue;
			}
		}
		for (int y = 0;y < texture.height;y++) {
			for (int x = 0;x < texture.width;x++) {
				Color	bc = Color.black;
				for (int i = 0;i < aTexFont.Length;i++) {
					Color	c = aTexFont[i].GetPixel(x,y);
					switch (i) {
					case 0:
						bc.r = c.a;
						break;
					case 1:
						bc.g = c.a;
						break;
					case 2:
						bc.b = c.a;
						break;
					case 3:
						bc.a = c.a;
						break;
					}
				}
				texture.SetPixel(x,y,bc);
			}
		}
#if COMPRESS
        EditorUtility.CompressTexture(texture, TextureFormat.RGBA4444,
#if UNITY_2018_3_OR_NEWER
                UnityEditor.TextureCompressionQuality.Best);
#else
                UnityEngine.TextureCompressionQuality.Best);
#endif
#endif
        // テクスチャをアセット化.
        string assetPath = KsSoftConfig.ResourcesPath + font.m_textureName + ".tex.asset";
		AssetDatabase.CreateAsset(texture, assetPath);
		// フォントデータをシリアライズ.
		CWriteVariable	cVariable = new CWriteVariable(1024 * 1024);
		font.write (cVariable);
		
		CSerializableScript cSS = ScriptableObject.CreateInstance<CSerializableScript>();
		// バッファからアセットを生成.
		cSS.m_buffer = cVariable.copybuffer();
		assetPath = KsSoftConfig.ResourcesPath + sId + ".font.asset";
		AssetDatabase.CreateAsset(cSS, assetPath);
	}
};

public class FontParser {
	protected delegate void ParserDel(string line);
	public TextAsset fontDef;

	protected t_SpriteFont	m_tSpriteFont = new t_SpriteFont();
	public t_SpriteFont spriteFont {
		get {
			return m_tSpriteFont;
		}
	}
    protected List<t_SpriteChar>    m_lstChar = new List<t_SpriteChar>();
    protected List<string>	m_lstPage = new List<string>();
	public List<string> pages {
		get {
			return m_lstPage;
		}
	}
	public FontParser(TextAsset def) 
	{
		Load(def);
	}
	public void Load(TextAsset def) 
	{
		if (def == null)
			return;

		int pos, c = 0;

		fontDef = def;

		string[] lines = fontDef.text.Split(new char[] { '\n' });

		pos = ParseSection("info", lines, HeaderParser, 0);
		pos = ParseSection("common", lines, CommonParser, pos);
		pos = PageIdParser(lines,pos);
		pos = ParseSection("chars count", lines, CharCountParser, pos);

		while (pos < lines.Length && c < m_tSpriteFont.m_aChar.Length) {
			CharParser(lines[pos++]);
		}
		m_tSpriteFont.m_aChar = m_lstChar.ToArray();

		int i = 0;
		foreach (t_SpriteChar ch in m_tSpriteFont.m_aChar) {
			if (ch == null) {
				Debug.LogWarning("null:" + i);
			}
			i++;
		}
		// Back up one line to be safe:
		--pos;
	}
	int ParseSection(string tag, string[] lines, ParserDel parser, int pos)
	{
		for (; pos < lines.Length; ++pos) {
			string line = lines[pos].Trim();

			if (line.Length < 1)
				continue;

			if (line.StartsWith(tag)) {
				parser(line);
				return ++pos;
			}
		}
		return pos;
	}
	int FindField(string label, string[] fields, int pos, bool logError)
	{
		for (; pos < fields.Length; ++pos) {
			if (label == fields[pos].Trim())
				return pos;
		}

		if (logError) {
			Debug.LogError("Missing \"" + label + "\" field in font definition file \"" + fontDef.name + "\". Please check the file or re-create it.");
			return pos;
		} else
			return -1;
	}

	int FindField(string label, string[] fields, int pos) 
	{
		return FindField(label, fields, pos, true);
	}

	int FindFieldOptional(string label, string[] fields, int pos)
	{
		return FindField(label, fields, pos, false);
	}
	void HeaderParser(string line)
	{
		string[] vals = line.Split(new char[] { ' ', '=' });

		int index = FindField("face", vals, 1);
		m_tSpriteFont.m_face =vals[index + 1].Trim(new char[] { '\"' });

		index = FindField("size", vals, index);
        int pxSize = int.Parse(vals[index + 1]);
        if (pxSize < 0) {
            pxSize = -pxSize;
        }
        m_tSpriteFont.m_pxSize = (ushort) pxSize;

		index = FindFieldOptional("charSpacing", vals, index);
		if (index != -1)
			m_tSpriteFont.m_charSpacing = sbyte.Parse(vals[index + 1]);
	}
	void CommonParser(string line)
	{
		string[] vals = line.Split(new char[] { ' ', '=' });

		int index = FindField("lineHeight", vals, 1);
		m_tSpriteFont.m_lineHeight = sbyte.Parse(vals[index + 1]);

		index = FindField("base", vals, index);
		m_tSpriteFont.m_baseHeight = sbyte.Parse(vals[index + 1]);

		index = FindField("scaleW", vals, index);
		m_tSpriteFont.m_texWidth = short.Parse(vals[index + 1]);

		index = FindField("scaleH", vals, index);
		m_tSpriteFont.m_texHeight = short.Parse(vals[index + 1]);

		index = FindField("pages", vals, index);
		m_tSpriteFont.m_pages = sbyte.Parse(vals[index + 1]);
	}
	int PageIdParser(string[] lines,int pos)
	{
		for (; pos < lines.Length; ++pos) {
			string line = lines[pos].Trim();

			if (line.Length < 1)
				continue;

			if (line.StartsWith("page id")) {
				string[] vals = line.Split(new char[] { '=',' '});
				int index = FindField("file", vals, 1);
				string	page = vals[index + 1].Trim(new char[] { '\"' });
				m_lstPage.Add(page);
				Debug.Log (page);
			} else if (m_lstPage.Count != 0) {
				break;
			}
		}
		return pos;
	}

	void CharCountParser(string line)
	{
		string[] vals = line.Split(new char[] { '=' });

		if (vals.Length < 2) {
			Debug.LogError("Malformed \"chars count\" line in font definition file \"" + fontDef.name + "\". Please check the file or re-create it.");
			return;
		}
		m_tSpriteFont.m_aChar = new t_SpriteChar[int.Parse(vals[1]) + 1];
	}
	bool CharParser(string line)
	{
		if (!line.StartsWith("char"))
			return false;
		float	texw = m_tSpriteFont.m_texWidth;
		float	texh = m_tSpriteFont.m_texHeight;
		float x, y, width, height;

		string[] vals = line.Split(new char[] { ' ', '=' });

		int index = FindField("id", vals, 1);
		t_SpriteChar tChar = new t_SpriteChar();
        int id = int.Parse(vals[index + 1]);
        if (id > 65535) {
            Debug.LogError("char code is illegal!!:" + (char) id + "(" + id + ") at " + line);
            return false;
        }
        tChar.id = (ushort) id;

		index = FindField("x", vals, index);
		x = float.Parse(vals[index + 1]);
	
		index = FindField("y", vals, index);
		y = texh - float.Parse(vals[index + 1]);

		index = FindField("width", vals, index);
		width = float.Parse(vals[index + 1]);
	
		index = FindField("height", vals, index);
		height = float.Parse(vals[index + 1]);
	
		index = FindField("xoffset", vals, index);
		tChar.xOffset = sbyte.Parse(vals[index + 1]);

		index = FindField("yoffset", vals, index);
		tChar.yOffset = (sbyte) -sbyte.Parse(vals[index + 1]);

		index = FindField("xadvance", vals, index);
		tChar.xAdvance = sbyte.Parse(vals[index + 1]);

		index = FindField("page", vals, index);
		tChar.page = sbyte.Parse(vals[index + 1]);

		// Build our character's UVs:
		tChar.uv = new Vector4(x/texw,(y - height)/texh,(x + width)/texw,y/texh);
		m_lstChar.Add(tChar);
		return true;
	}
}

