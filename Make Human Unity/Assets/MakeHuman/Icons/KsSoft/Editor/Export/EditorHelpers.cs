//==============================================================================================
/*!エディタ用ヘルパー関数.
	@file  EditorHelpers

	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using Object=UnityEngine.Object;
using KS;

class EditorHelpers
{
    //==========================================================================
    /*!プレファブをアセットとして保存する.
		@brief	SaveAsPrefabAsset
	*/
    public static Object SaveAsPrefabAsset(GameObject go,string path) {
#if UNITY_2018_3_NEWER
        return PrefabUtility.SaveAsPrefabAsset(go,path);
#else
        Object tempPrefab = PrefabUtility.CreateEmptyPrefab(path);
        return PrefabUtility.ReplacePrefab(go, tempPrefab);
#endif
    }

    //==========================================================================
    /*!ファイルを読み込んで、デシリアライズする.
		@brief	read
	*/
    public static CReadVariable read(string path) {
		//------------------------------------------
		// ビルボードソースデータを読み込む.
		FileInfo	fi = new FileInfo(path);
		if (!fi.Exists) {
			Debug.LogError("can't find file:" + path);
			return null;
		}
		byte[]	buffer = new byte[fi.Length];
		FileStream	fs = new FileStream(path,FileMode.Open,FileAccess.Read);
		fs.Read(buffer,0,buffer.Length);
		fs.Close();
		return new CReadVariable(buffer);
	}
    //==========================================================================
    /*!ファイルを読み込んで、デシリアライズする.
		@brief	write
	*/
    public static void write(string path,CWriteVariable cVariable) {
        int size = cVariable.size();
        byte[] buffer = cVariable.buffer();
        FileStream fs = new FileStream(path,FileMode.Create,FileAccess.Write);
        fs.Write(buffer,0,size);
        fs.Close();
    }
    public static void write(string path,ISerializable iSerializable) {
        CWriteVariable cVariable = new CWriteVariable(2048);
        iSerializable.write(cVariable);
        write(path,cVariable);
    }
    //==========================================================================
    /*!指定したゲームオブジェクトから名前が一致するコンポーネントを取得する.
		@brief	CollectAll
	*/
    public static T findComponent<T>(GameObject go,string name) where T : Component {
        T[] aComponent = go.GetComponentsInChildren<T>();
        foreach (T component in aComponent) {
            if (component.name == name) {
                return component;
            }
        }
        return null;
    }
    public static T findComponent<T>(GameObject go,T src) where T : Component {
        T[] aComponent = go.GetComponentsInChildren<T>();
        foreach (T component in aComponent) {
            if (component.name == src.name) {
                return component;
            }
        }
        return null;
    }
    public static T[] findComponents<T>(GameObject go,string[] names) where T : Component {
        T[] aComponent = go.GetComponentsInChildren<T>();
        T[] aResult = new T[names.Length];
        for (int i = 0;i < names.Length;++i) {
            string name = names[i];
            foreach (T component in aComponent) {
                if (component.name == name) {
                    aResult[i] = component;
                    break;
                }
            }
            if (aResult[i] == null) {
                Debug.LogError("can't find component:" + name);
            }
        }
        return aResult;
    }
    public static T[] findComponents<T>(GameObject go,T[] srcs) where T : Component {
        T[] aComponent = go.GetComponentsInChildren<T>();
        T[] aResult = new T[srcs.Length];
        for (int i = 0;i < srcs.Length;++i) {
            string name = srcs[i].name;
            foreach (T component in aComponent) {
                if (component.name == name) {
                    aResult[i] = component;
                    break;
                }
            }
            if (aResult[i] == null) {
                Debug.LogError("can't find component:" + name);
            }
        }
        return aResult;
    }
    //==========================================================================
    /*!指定したフォルダ以下の該当するアセットを全て取得する.
		@brief	CollectAll
	*/
    public static List<T> CollectAll<T>(string path,bool bDeep) where T : Object {
		List<T> result = new List<T>();
		CollectAll<T>(result,path,bDeep);
		return result;
    }
    static void CollectAll<T>(List<T> result,string path,bool bDeep) where T : Object {
        string[] files = Directory.GetFiles(path);

        foreach (string file in files) {
            if (file.Contains(".meta")) continue;
            if (file.Contains(".bin")) continue;
            if (file.Contains(".h")) continue;
            T asset = (T) AssetDatabase.LoadAssetAtPath(file, typeof(T));
            if (asset == null) {
				continue;
			}
            result.Add(asset);
        }

		if (bDeep) {
			foreach (string dir in Directory.GetDirectories(path)) {
				if (dir.Contains(".svn")) continue;
				CollectAll<T>(result,dir,bDeep);
			}
		}
    }
	//==========================================================================
	/*!指定したフォルダ以下の該当するアセットのパスを全て取得する.
		@brief	CollectAllAssetPath
	*/
    public static List<string> CollectAllAssetPath<T>(string path,bool bDeep) where T : Object {
		List<string> result = new List<string>();
		CollectAllAssetPath<T>(result,path,bDeep);
		return result;
    }
    static void CollectAllAssetPath<T>(List<string> result,string path,bool bDeep) where T : Object {
        string[] files = Directory.GetFiles(path);

        foreach (string file in files) {
            if (file.Contains(".meta")) continue;
            if (file.Contains(".bin")) continue;
            T asset = (T) AssetDatabase.LoadAssetAtPath(file, typeof(T));
            if (asset == null) {
				continue;
			}
			result.Add(file);
        }

		if (bDeep) {
			foreach (string dir in Directory.GetDirectories(path)) {
				if (dir.Contains(".svn")) continue;
				CollectAllAssetPath<T>(result,dir,bDeep);
			}
		}
    }
	static public uint ConvertId(string sId,bool bNormalize = false) {
		if (bNormalize) {
			string[] aName = sId.Split(new char [] {'/','\\','.'});
			if (aName.Length < 2) {
				return 0;
			}
			sId = aName[aName.Length - 2];
		}
		if (MulId.isMulId(sId)) {
			return new MulId(sId);
		}
		if (FiveCC.isFiveCC(sId)) {
			return new FiveCC(sId);
		}
		return 0;
	}
	static public uint Hash(string str) {
		uint hash = 0;
		for (int i = 0;i < str.Length;++i) {
			hash += ((uint) str[i]) << (i % 25);
		}
		return hash;
	}
	static public bool isDirectory(UnityEngine.Object obj,bool checkMulId = true) {
		string path = AssetDatabase.GetAssetPath(obj);
		return isDirectory(path,checkMulId);
	}
	static public bool isDirectory(string path,bool checkMulId = true) {
		if (Directory.Exists(path)) {
			if (checkMulId) {
				string[] aPath = path.Split(new char [] { '/','\\' });
				if (aPath.Length <= 0) return false;
				string	sPath = aPath[aPath.Length - 1];
				if (MulId.isMulId (sPath)) {
					return true;
				}
				return false;
			}
			return true;
		}
		return false;
	}
	//==========================================================================
	/*!マテリアル情報を格納する.
		@brief	getMaterialProperty
	*/
	public static t_Material getMaterialProperty(Material material,List<string> lstInclude = null) {
		t_Material	tMaterial = new t_Material();
		string	name = material.name;
		tMaterial.m_mId = (uint) EditorHelpers.ConvertId(name,true);
		Shader	shader = material.shader;
		tMaterial.m_shader = shader.name;
		tMaterial.m_nProperty = (byte) ShaderUtil.GetPropertyCount(shader);
		CWriteVariable	cVariable = new CWriteVariable(4096);
		for (int i = 0;i < tMaterial.m_nProperty;++i) {
			string propname = ShaderUtil.GetPropertyName(shader,i);
			cVariable.put (ref propname,255);
			switch (ShaderUtil.GetPropertyType(shader,i)) {
			  case ShaderUtil.ShaderPropertyType.Color:
				cVariable.put (t_Material.PropertyType_Color);
				cVariable.put (KsSoftUtility.ColorToU32(material.GetColor (propname)));
				break;
			  case ShaderUtil.ShaderPropertyType.Vector:
				cVariable.put (t_Material.PropertyType_Vector);
				cVariable.put (material.GetVector(propname));
				break;
			  case ShaderUtil.ShaderPropertyType.Float:
				cVariable.put (t_Material.PropertyType_Float);
				cVariable.put (material.GetFloat(propname));
				break;
			  case ShaderUtil.ShaderPropertyType.Range:
				cVariable.put (t_Material.PropertyType_Float);
				cVariable.put (material.GetFloat(propname));
				break;
			  case ShaderUtil.ShaderPropertyType.TexEnv:
				cVariable.put (t_Material.PropertyType_Texture);
				Texture	tex = material.GetTexture(propname);
				string texpath = AssetDatabase.GetAssetPath(tex);
				if (lstInclude != null) {
					lstInclude.Add(texpath);
				}
                string texname = tex.name;
				cVariable.put (ref texname,255);
				break;
			}
		}
		tMaterial.m_aBuffer = cVariable.copybuffer();
		return tMaterial;
	}
}
