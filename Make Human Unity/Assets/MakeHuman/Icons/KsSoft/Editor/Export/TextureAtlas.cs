//==============================================================================================
/*!テクスチャアトラス汎用関数.
	@file  Texture Atlas
	
	(counter SJIS string 京)
*/
//==============================================================================================
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using KS;

class TextureAtlas {
	public struct Result {
		public Texture2D	texture;
		public Rect[]		aRect;
	}
	//==========================================================================
	/*!指定されたテクスチャを各チャンネルに割り振り一枚のテクスチャにする.
		@brief	packChannel
	*/
	[MenuItem("Assets/packChannel")]
	static public bool packChannel() {
		Object[] aObject = Selection.GetFiltered(typeof(Texture2D), SelectionMode.Assets);
		Texture2D	texR = null;
		Texture2D	texG = null;
		Texture2D	texB = null;
		Texture2D	texA = null;
		string		path = null;
		string		name = null;
		foreach (Object obj in aObject) {
			Texture2D	tex = obj as Texture2D;
			if (tex == null) {
				continue;
			}
			string assetPath = AssetDatabase.GetAssetPath(tex);
			if (assetPath.IndexOf(".r.") > 0) {
				texR = tex;
			} else if (assetPath.IndexOf(".g.") > 0) {
				texG = tex;
			} else if (assetPath.IndexOf(".b.") > 0) {
				texB = tex;
			} else if (assetPath.IndexOf(".a.") > 0) {
				texA = tex;
			} else {
				Debug.LogWarning("this texture name is illegal:" + tex.name);
				continue;
			}
			if (path == null) {
				name = Path.GetFileName(assetPath).Split('.')[0] + ".png";
				path = Path.GetDirectoryName(assetPath);
			}
		}
		if (path == null) {
			return false;
		}
		Texture2D	res = packChannel(texR,texG,texB,texA);
		if (res == null) {
			return false;
		}
		byte[]	pngData = res.EncodeToPNG();
		string filePath = EditorUtility.SaveFilePanel("Save Texture",path,name, "png");
		if (string.IsNullOrEmpty(filePath)) {
			return false;
		}
		File.WriteAllBytes(filePath,pngData);
		AssetDatabase.Refresh();
		return true;
	}
	//==========================================================================
	/*!指定されたテクスチャを各チャンネルに割り振り一枚のテクスチャにする.
		@brief	packChannel
		@note	各テクスチャのR成分をコピーします.
	*/
	static public Texture2D packChannel(Texture2D texR,Texture2D texG,Texture2D texB,Texture2D texA) {
		Texture2D[]	aTexture = new Texture2D[4];
		TextureFormat	fmt = TextureFormat.ARGB32;
		if (texA == null) {
			fmt = TextureFormat.RGB24;
		}
		aTexture[0] = texR;
		aTexture[1] = texG;
		aTexture[2] = texB;
		aTexture[3] = texA;
		int w = 0;
		int h = 0;
		string name = null;
		foreach (Texture2D tx in aTexture) {
			if (tx == null) {
				continue;
			}
			string path = AssetDatabase.GetAssetPath(tx);
			TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
			if (textureImporter.isReadable == false) {
				textureImporter.isReadable = true;
				AssetDatabase.ImportAsset(path);
			}
			if (w == 0) {
				w = tx.width;
				h = tx.height;
			}
			if (name == null) {
				name = tx.name.Split('.')[0];
			}
			if (w != tx.width || h != tx.height) {
				Debug.LogError ("texture size is illegal");
				return null;
			}
		}
		Texture2D tempTex = new Texture2D(w,h,fmt,false);
		tempTex.name = name;
		tempTex.Resize(w,h, fmt, false);
		Color	c,tmp;
		for (int y = 0;y < h;++y) {
			for (int x = 0;x < w;++x) {
				if (texR != null) {
					tmp = texR.GetPixel(x,y);
					c.r = tmp.r;
				} else {
					c.r = 0f;
				}
				if (texG != null) {
					tmp = texG.GetPixel(x,y);
					c.g = tmp.r;
				} else {
					c.g = 0f;
				}
				if (texB != null) {
					tmp = texB.GetPixel(x,y);
					c.b = tmp.r;
				} else {
					c.b = 0f;
				}
				if (texA != null) {
					tmp = texA.GetPixel(x,y);
					c.a = tmp.r;
				} else {
					c.a = 1f;
				}
				tempTex.SetPixel(x,y,c);
			}
		}
		tempTex.wrapMode = TextureWrapMode.Clamp;
		return tempTex;
	}
	//==========================================================================
	/*!テクスチャをパックする.
		@brief	packTexture
	*/
	static public Result packTexture(List<Texture2D> lstTexture,int padding,int maxAtlasSize,bool forceSquare,bool expand = false) {
		foreach (Texture2D tx in lstTexture) {
			string path = AssetDatabase.GetAssetPath(tx);
			TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
			if (textureImporter.isReadable == false) {
				textureImporter.isReadable = true;
				AssetDatabase.ImportAsset(path);
			}
		}

		Texture2D	atlas = new Texture2D(4,4,TextureFormat.ARGB32,false);
		Rect[] aRect = atlas.PackTextures((Texture2D[])lstTexture.ToArray(), padding, maxAtlasSize,false);

		foreach (Texture2D tx in lstTexture) {
			if (AssetDatabase.GetAssetPath(tx).Length < 1)
				Object.DestroyImmediate(tx);
		}
		lstTexture.Clear();
		
		// 拡大しても崩れないようにパディング分押し広げる.
		if (expand && padding/2 > 0) {
			// Create a square texture:
			Texture2D tempTex = (Texture2D)Object.Instantiate(atlas);
			tempTex.name = atlas.name;
			
			int ph = padding/2;
			for (int i = 0;i < aRect.Length;i++) {
				int sx = (int) (aRect[i].x * (float) atlas.width);
				int sy = (int) (aRect[i].y * (float) atlas.height);
				int w = (int) (aRect[i].width  * (float) atlas.width);
				int h = (int) (aRect[i].height * (float) atlas.height);
				int ex = sx + w - 1;
				int ey = sy + h - 1;
				
				int x,y;
				for (int j = 1;j <= ph;j++) {
					y = sy - j;
					if (y >= 0) {
						for (x = sx - j;x < sx + w + j;x++) {
							if (x < 0 || x >= atlas.width) continue;
							Color	c;
							if (x < sx) {
								c = atlas.GetPixel(sx,sy);
							} else if (x >= sx + w) {
								c = atlas.GetPixel(ex,sy);
							} else {
								c = atlas.GetPixel(x,sy);
							}
							tempTex.SetPixel(x,y,c);
						}
					}
					y = ey + j;
					if (y < atlas.height) {
						for (x = sx - j;x < sx + w + j;x++) {
							if (x < 0 || x >= atlas.width) continue;
							Color	c;
							if (x < sx) {
								c = atlas.GetPixel(sx,ey);
							} else if (x >= sx + w) {
								c = atlas.GetPixel(ex,ey);
							} else {
								c = atlas.GetPixel(x,ey);
							}
							tempTex.SetPixel(x,y,c);
						}
					}
					x = sx - j;
					if (x >= 0) {
						for (y = sy - j;y < sy + h + j;y++) {
							if (y < 0 || y >= atlas.height) continue;
							Color	c;
							if (y < sy) {
								c = atlas.GetPixel(sx,sy);
							} else if (y >= sy + h) {
								c = atlas.GetPixel(sx,ey);
							} else {
								c = atlas.GetPixel(sx,y);
							}
							tempTex.SetPixel(x,y,c);
						}
					}
					x = ex + j;
					if (x < atlas.width) {
						for (y = sy - j;y < sy + h + j;y++) {
							if (y < 0 || y >= atlas.height) continue;
							Color	c;
							if (y < sy) {
								c = atlas.GetPixel(ex,sy);
							} else if (y >= sy + h) {
								c = atlas.GetPixel(ex,ey);
							} else {
								c = atlas.GetPixel(ex,y);
							}
							tempTex.SetPixel(x,y,c);
						}
					}
				}	
			}
			atlas = tempTex;
		}

		// See if the texture needs to be made square:
		if (forceSquare && atlas.width != atlas.height) {
			int size = Mathf.Max(atlas.width, atlas.height);
			
			// Create a square texture:
			Texture2D tempTex = (Texture2D)Object.Instantiate(atlas);
			tempTex.name = atlas.name;
			tempTex.Resize(size, size, TextureFormat.ARGB32, false);
			
			// Copy the contents:
			tempTex.SetPixels(0, 0, atlas.width, atlas.height, atlas.GetPixels(0), 0);
			tempTex.Apply(false);
			
			// Scale the UVs to account for this:
			for (int j = 0; j < aRect.Length; ++j) {
				// See which side we expanded:
				if (atlas.width > atlas.height) {
					aRect[j].y = aRect[j].y * 0.5f;
					aRect[j].yMax = aRect[j].y + (aRect[j].height * 0.5f);
				} else {
					aRect[j].x = aRect[j].x * 0.5f;
					aRect[j].xMax = aRect[j].x + (aRect[j].width * 0.5f);
				}
			}

			if (atlas != tempTex)
				Object.DestroyImmediate(atlas);
			atlas = tempTex;
		} else {
			Texture2D tempTex = (Texture2D)Object.Instantiate(atlas);
			tempTex.name = atlas.name;
			tempTex.Resize(atlas.width, atlas.height, TextureFormat.ARGB32, false);
			
			// Copy the contents:
			tempTex.SetPixels(0, 0, atlas.width, atlas.height, atlas.GetPixels(0), 0);
			tempTex.Apply(false);			
			if (atlas != tempTex)
				Object.DestroyImmediate(atlas);
			atlas = tempTex;
		}
		if (!forceSquare) {
			Texture2D tempTex = trimAtlas(atlas);

			float widthFactor = ((float)atlas.width) / ((float)tempTex.width);
			float heightFactor = ((float)atlas.height) / ((float)tempTex.height);

			//check region
			bool bTrim = true;
			if (widthFactor != 1f || heightFactor != 1f) {
				for (int j = 0;j < aRect.Length;++j) {
					if (aRect[j].xMax * widthFactor > 1f)  {
						bTrim = false;
						break;
					}
					if (aRect[j].yMax * heightFactor > 1f)  {
						bTrim = false;
						break;
					}
				}
			}
			if (bTrim) {
				for (int k = 0; k < aRect.Length; ++k) {
					aRect[k].x *= widthFactor;
					aRect[k].y *= heightFactor;
					aRect[k].width *= widthFactor;
					aRect[k].height *= heightFactor;
				}
				if (atlas != tempTex)
					Object.DestroyImmediate(atlas);
				atlas = tempTex;
			} else {
				Object.DestroyImmediate(tempTex);
			}
		}
		Result	res;
		atlas.wrapMode = TextureWrapMode.Clamp;	//アトラス化したテクスチャはClampのほうが都合がいい.
		res.texture = atlas;
		res.aRect = aRect;
		return res;
	}
	static Texture2D trimAtlas(Texture2D tex) {
		int x, y;
		int firstX, firstY;
		int newWidth, newHeight;
		Texture2D newTex;
		Color[] pixels;

		pixels = tex.GetPixels(0);

		firstX = tex.width;
		firstY = tex.height;

		for (x = tex.width - 1; x >= 0; --x) {
			for (y = 0; y < tex.height; ++y){
				if (pixels[y * tex.width + x].a != 0f) {
					firstX = x;
					x = 0;
					break;
				}
			}
		}

		for (y = tex.height - 1; y >= 0; --y) {
			for (x = 0; x < tex.width; ++x) {
				if (pixels[y * tex.width + x].a != 0f) {
					firstY = y;
					y = 0;
					break;
				}
			}
		}

		newWidth = nextPowerOf2(firstX + 1);
		newHeight = nextPowerOf2(firstY + 1);

		if (newWidth == tex.width && newHeight == tex.height)
			return tex;

		newTex = new Texture2D(newWidth, newHeight, TextureFormat.ARGB32, false);

		newTex.SetPixels(tex.GetPixels(0, 0, newWidth, newHeight, 0));
		newTex.Apply(false);
		newTex.name = tex.name;

		return newTex;
	}
	static int nextPowerOf2(int val) {
		int newVal = Mathf.ClosestPowerOfTwo(val);

		while (newVal < val) {
			newVal <<= 1;
		}
		return newVal;
	}
	//==========================================================================
	/*!ディザリング処理をしつつ、16bitカラーに変換.
		@brief	dither
	*/
	struct t_Color {
		public int r;
		public int g;
		public int b;
		public int a;
		public void clear() {
			r = 0;
			g = 0;
			b = 0;
			a = 0;
		}
	};
	static public Texture2D dither(Texture2D src,bool[,] aDitherMask) {
		Texture2D	tex2d = jjndither(src,aDitherMask,false);
		tex2d.wrapMode = TextureWrapMode.Clamp;
		return tex2d;
	}
	//==========================================================================
	/*!Jarvis,Judice & Ninke型誤差分散.
		@brief	jjndither
	*/
	const int m_patx = 5;		//配列err_patの横幅.
	const int m_paty = 3;		//配列err_patの縦幅.
	static protected Texture2D jjndither(Texture2D src,bool[,] aDitherMask,bool bAlphaDither) {
		// 以下３つは誤差分散パターンによって決まる定数.
		int d_area = 2;		//誤差分散を行う画素の範囲.

		// 誤差分散パターン
		int[] err_pat = new int[m_patx * m_paty] {
			0,0,0,7,5,
			3,5,7,5,3,
			1,3,5,3,1
		};
	
		int mx = (src.width) + d_area * 2;	// バッファの横サイズ（横幅＋両端に余計にとる）.
		int sum=mx * m_paty;				// バッファサイズ.
		
		t_Color[]	aColorErr = new t_Color[sum];
		// バッファ初期化.
		for(int i = 0;i < aColorErr.Length;++i) {
			aColorErr[i].clear ();
		}

		Texture2D	tex2d = new Texture2D(src.width,src.height,TextureFormat.ARGB32,false);
		// 分散パターンの合計値.
		int pat_sum = 0;
		for(int i = 0;i < err_pat.Length;i++) {
			pat_sum += err_pat[i];
		}
	
		for(int y = 0; y < src.height;y++) {
			for(int x = 0;x < src.width;x++) {
				Color32	col = src.GetPixel(x,y);	//画像上の画素情報を取得.
				int adr = (x + d_area);
				// 他から分散された誤差を加算.
				int rr = col.r + (aColorErr[adr].r/pat_sum);
				int gg = col.g + (aColorErr[adr].g/pat_sum);
				int bb = col.b + (aColorErr[adr].b/pat_sum);
				int aa = col.a + (aColorErr[adr].a/pat_sum);
				// 16ビットカラーへ減色.
				int rss = (rr & (~15));
	            int gss = (gg & (~15));
				int bss = (bb & (~15));
				int ass = (aa & (~15));
				// 処理画素で生じた誤差を計算.
				int re = rr - rss;
				int ge = gg - gss;
				int be = bb - bss;
				int ae = aa - ass;
				// 誤差分散.
				adr -= d_area;
				for(int iy = 0;iy < m_paty;iy++) {
					for(int ix = 0;ix < m_patx;ix++) {
						aColorErr[adr + ix].r += (re * err_pat[ix + iy * m_patx]);
						aColorErr[adr + ix].g += (ge * err_pat[ix + iy * m_patx]);
						aColorErr[adr + ix].b += (be * err_pat[ix + iy * m_patx]);
						aColorErr[adr + ix].a += (ae * err_pat[ix + iy * m_patx]);
					}
					adr += mx;
				}
				if (!aDitherMask[x,y]) {
					// ディザなし.
					col.r = (byte) (col.r & (~15));
					col.g = (byte) (col.g & (~15));
					col.b = (byte) (col.b & (~15));
				} else {
					col.r = (byte) Mathf.Min (rss,255);
					col.g = (byte) Mathf.Min (gss,255);
					col.b = (byte) Mathf.Min (bss,255);
					if (bAlphaDither) {
						col.a = (byte) Mathf.Min (ass,255);
					}
				}
				tex2d.SetPixel(x,y,col);	// 画像に値をセットする.
			}
			// バッファのずらし処理.
			for(int i = 0;i < mx;i++) {
				for(int iy = 0;iy < m_paty - 1;iy++) {
					aColorErr[i + iy * mx] = aColorErr[i + (iy + 1) * mx];
				}
				aColorErr[i + (m_paty - 1) * mx].clear();
			}
		}
		return tex2d;		
	}
	//==========================================================================
	/*!Floyd & Steinberg型誤差分散.
		@brief	fsdither
	*/
	static protected Texture2D fsdither(Texture2D src,bool bAlphaDither) {
		int mx = src.width + 2;		// バッファの横サイズ（横幅＋両端に２画素分余計にとる）.
		int sum = mx * 2;			// ２列分なので２倍.
		t_Color[] aColErr = new t_Color[sum];
		// バッファ初期化.
		for(int i = 0;i < aColErr.Length;++i) {
			aColErr[i].clear();
		}
		Texture2D	tex2d = new Texture2D(src.width,src.height,TextureFormat.ARGB32,false);

		for(int y = 0;y < src.height;y++) {
			for(int x = 0;x < src.width;x++) {
				Color32 col = src.GetPixel(x,y);	//画像上の画素情報を取得.
				int adr = x + 1;
				// 他から分散された誤差を加算.
				int rr = col.r + aColErr[adr].r/16;
				int gg = col.g + aColErr[adr].g/16;
				int bb = col.b + aColErr[adr].b/16;
				int aa = col.a + aColErr[adr].a/16;
				// 16bitカラーへ減色.
				int rss = (rr & (~15));
	            int gss = (gg & (~15));
				int bss = (bb & (~15));
				int ass = (aa & (~15));
				// 処理画素で生じた誤差を計算.
				int re = rr - rss;
				int ge = gg - gss;
				int be = bb - bss;
				int ae = aa - ass;
				// 誤差分散.
				// 右へ.
				aColErr[adr + 1].r += re * 7;
				aColErr[adr + 1].g += ge * 7;
				aColErr[adr + 1].b += be * 7;
				aColErr[adr + 1].a += ae * 7;

				// 左下へ.
				aColErr[adr + mx - 1].r += re * 3;
				aColErr[adr + mx - 1].g += ge * 3;
				aColErr[adr + mx - 1].b += be * 3;
				aColErr[adr + mx - 1].a += ae * 3;

				// 下へ.
				aColErr[adr + mx].r += re * 5;
				aColErr[adr + mx].g += ge * 5;
				aColErr[adr + mx].b += be * 5;
				aColErr[adr + mx].a += ae * 5;

				// 右下へ.
				aColErr[adr + mx + 1].r += re * 1;
				aColErr[adr + mx + 1].g += ge * 1;
				aColErr[adr + mx + 1].b += be * 1;
				aColErr[adr + mx + 1].a += ae * 1;

				col.r = (byte) Mathf.Min (rss,255);
				col.g = (byte) Mathf.Min (gss,255);
				col.b = (byte) Mathf.Min (bss,255);
				if (bAlphaDither) {
					col.a = (byte) Mathf.Min (ass,255);
				}
				tex2d.SetPixel(x,y,col);	// 画像に値をセットする.
			}
			// バッファのずらし処理.
			for(int i = 0;i < mx;i++) {
				aColErr[i] = aColErr[i + mx];
				aColErr[i + mx].clear();
			}
		}
		return tex2d;
	}
};
