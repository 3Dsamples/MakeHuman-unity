//==============================================================================================
/*!ユーティリティ.
	@file  KsSoftUtility
*/
//==============================================================================================
using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace KS {
    public enum e_ResolutionPriority {
        Auto,
        Width,
        Height,
    };

    public class KsSoftUtility {
        static CMessageDataSheet m_cMessageDataSheet;
        static Vector2 m_defaultResolution;
        static char[] m_aSqlInjection = new char[] {
        '-',
        ';',
        ':',
        '\'',
        '\"',
        '%',
        '=',
        '(',
        ')',
        '\\',
        (char) 0x00a5,
    };
        // UNIXエポックを表すDateTimeオブジェクトを取得.
        private static System.DateTime UNIX_EPOCH = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);

        public static void Swap<T>(ref T lhs, ref T rhs) {
            T temp;
            temp = lhs;
            lhs = rhs;
            rhs = temp;
        }
        static public Color U32toColor(uint uColor) {
            Color color;
            color.r = (float)((uColor >> 16) & 0xff) * (1.0f / 255.0f);
            color.g = (float)((uColor >> 8) & 0xff) * (1.0f / 255.0f);
            color.b = (float)((uColor) & 0xff) * (1.0f / 255.0f);
            color.a = (float)((uColor >> 24) & 0xff) * (1.0f / 255.0f);
            return color;
        }
        static public uint ColorToU32(Color cColor) {
            uint r = (uint)Mathf.Clamp((int)(cColor.r * 255f), 0, 255);
            uint g = (uint)Mathf.Clamp((int)(cColor.g * 255f), 0, 255);
            uint b = (uint)Mathf.Clamp((int)(cColor.b * 255f), 0, 255);
            uint a = (uint)Mathf.Clamp((int)(cColor.a * 255f), 0, 255);
            return (uint)((a << 24) | (r << 16) | (g << 8) | b);
        }
        static public Color32 U32toColor32(uint uColor) {
            Color32 color = new Color32(
                (byte)((uColor >> 16) & 0xff),
                (byte)((uColor >> 8) & 0xff),
                (byte)((uColor) & 0xff),
                (byte)((uColor >> 24) & 0xff));
            return color;
        }
        static public Color32 Color32Mag(Color32 color, float mag) {
            uint k = (uint)(Mathf.Clamp01(mag) * 256f);
            color.r = (byte)((k * color.r) >> 8);
            color.g = (byte)((k * color.g) >> 8);
            color.b = (byte)((k * color.b) >> 8);
            return color;
        }
        static public Color32 Color32Mul(Color32 a, Color32 b) {
            a.r = (byte)((a.r * b.r) >> 8);
            a.g = (byte)((a.g * b.g) >> 8);
            a.b = (byte)((a.b * b.b) >> 8);
            a.a = (byte)((a.a * b.a) >> 8);
            return a;
        }

        static KsSoftUtility() {
            m_defaultResolution.x = Screen.width;
            m_defaultResolution.y = Screen.height;
        }
        //==========================================================================
        /*!初期化.
         * @brief	initialize
        */
        static public void initalize() {
            if (CMessageDataSheetMgr.Instance != null) {
                m_cMessageDataSheet = CMessageDataSheetMgr.Instance.find(new FiveCC("WNDW"));
            }
        }
        //==========================================================================
        /*!名前が一致するゲームオブジェクトを探す.
         * @brief	getGameComponent
        */
        static public Type getGameComponent<Type>(string sName) where Type : MonoBehaviour {
            GameObject go = GameObject.Find(sName);
            if (go == null) {
                return null;
            }
            return go.GetComponentInChildren<Type>() as Type;
        }




        //==========================================================================
        /*!コンポーネントを探す.
         * @brief	getGameComponents
        */
        static public Type[] getGameComponents<Type>() where Type : MonoBehaviour {
            return (Type[])UnityEngine.Object.FindObjectsOfType(typeof(Type));
        }




        //==========================================================================
        /*!指定した範囲に値を丸める.
         * @brief	Clamp
        */
        static public T Clamp<T>(T value, T min, T max) where T : System.IComparable<T> {
            T result = value;
            if (value.CompareTo(min) < 0) {
                result = min;
            }
            if (value.CompareTo(max) > 0) {
                result = max;
            }
            return result;
        }




        //==========================================================================
        /*!指定の名前のTransformを探す.
         * @brief	findTransform
        */
        static public Transform findTransform(Transform root, string target) {
            if (root.name == target) {
                return root;
            }
            foreach (Transform t in root) {
                Transform trans = findTransform(t, target);
                if (trans != null) {
                    return trans;
                }
            }
            return null;
        }




        //==========================================================================
        /*!大きい値を返す.
         * @brief	Max
        */
        static public T Max<T>(T a, T b) where T : System.IComparable<T> {
            if (a.CompareTo(b) > 0) {
                return a;
            }
            return b;
        }
        //==========================================================================
        /*!ゲームオブジェクトについているすべてのマテリアルを取得する.
         * @brief	getMaterials
        */
        static public Material[] getMaterials(GameObject go) {
            List<Material> lstMaterial = new List<Material>();
            Renderer[] aRenderer = go.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer r in aRenderer) {
                lstMaterial.AddRange(r.materials);
            }
            return lstMaterial.ToArray();
        }




        //==========================================================================
        /*!ゲームオブジェクトのレイヤーを再帰的に設定する.
         * @brief	setLayer
        */
        static public void setLayer(GameObject go, e_LayerId eLayer, bool bRecursive = true) {
            if (go == null) {
                return;
            }
            if (!bRecursive) {
                return;
            }
            go.layer = (int)eLayer;
            foreach (Transform transChild in go.transform) {
                GameObject child = transChild.gameObject;
                child.layer = (int)eLayer;
                setLayer(child, eLayer, true);
            }
        }




        //==========================================================================
        /*!データセーブ用ルートディレクトリ取得.
         * @brief	getSaveRootPath
        */
        static public string getSaveRootPath() {
            return Application.persistentDataPath;
        }
        //==========================================================================
        /*!経過した時間をUtc基準で取得する.
         * @brief	getPastTimeSpan
        */
        static public System.TimeSpan getPastTimeSpan(uint utime) {
            System.DateTime dateTime = UNIX_EPOCH.AddSeconds(utime); // mail.CreatedTime = 1354844896
            return System.DateTime.UtcNow - dateTime;
        }




        //==========================================================================
        /*!Unixタイムを取得する.
         * @brief	getUnixTime
        */
        public static long getUnixTime(System.DateTime targetTime) {
            // UTC時間に変換.
            targetTime = targetTime.ToUniversalTime();

            // UNIXエポックからの経過時間を取得.
            System.TimeSpan elapsedTime = targetTime - UNIX_EPOCH;

            // 経過秒数に変換.
            return (long)elapsedTime.TotalSeconds;
        }
        public static long getUnixTime() {
            return getUnixTime(System.DateTime.UtcNow);
        }




        //==========================================================================
        /*!Unixタイムから時間を取得する.
         * @brief	getDateTime
        */
        public static System.DateTime getDateTime(long unixtime) {
            return UNIX_EPOCH.AddSeconds(unixtime).ToLocalTime();
        }




        //==========================================================================
        /*!Unixタイムから、経過時間の文字列を取得する.
         * @brief	getSpanTime
        */
        public static string getSpanTime(uint utime) {
            if (m_cMessageDataSheet == null) {
                return "";
            }
            string strDateTime = "";
            System.DateTime dateTime = UNIX_EPOCH.AddSeconds(utime);
            System.TimeSpan span = System.DateTime.UtcNow - dateTime;
            if (span.TotalSeconds < 60) {
                strDateTime = span.Minutes + " " + m_cMessageDataSheet.find(new MulId(0, 0, 60));
            } else if (span.TotalSeconds < 60 * 60) {
                strDateTime = span.Minutes + " " + m_cMessageDataSheet.find(new MulId(0, 0, 60));
            } else if (span.TotalSeconds < 60 * 60 * 60) {
                strDateTime = span.Hours + " " + m_cMessageDataSheet.find(new MulId(0, 0, 70));
            } else {
                strDateTime = span.Days + " " + m_cMessageDataSheet.find(new MulId(0, 0, 80));
            }
            return strDateTime;
        }





        //==========================================================================
        /*!SQLインジェクションを調べる.
         * @brief	isValidSQL
        */
        public static bool isValidSQL(string str) {
            if (str.IndexOfAny(m_aSqlInjection) < 0) {
                return true;
            }
            return false;
        }




        //==========================================================================
        /*!SQLインジェクションを調べる.
         * @brief	isInvalidSQL
        */
        public static bool isInvalidSQL(string str) {
            return !isValidSQL(str);
        }




        //==========================================================================
        /*!バックスラッシュを外す.
         * @brief	stripslashes
        */
        public static string stripslashes(string str) {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(str.Length + 1);
            for (int i = 0; i < str.Length; ++i) {
                char ch = str[i];
                if (ch == '\\') {
                    ++i;
                    if (i >= str.Length) {
                        break;
                    }
                    ch = str[i];
                    switch (ch) {
                    case 'n':
                        ch = '\n';
                        break;
                    case 't':
                        ch = '\t';
                        break;
                    case 'r':
                        ch = '\r';
                        break;
                    }
                }
                sb.Append(ch);
            }
            return sb.ToString();
        }




        //==========================================================================
        /*!バックシュラッシュをescapeする(Richtext表示用).
         * @brief	escapestring
        */
        public static string escapestring(string str) {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(str.Length * 2 + 1);
            for (int i = 0; i < str.Length; ++i) {
                char ch = str[i];
                if (ch == '\\') {
                    sb.Append('\\');
                }
                sb.Append(ch);
            }
            return sb.ToString();
        }
        //==========================================================================
        /*!文字列の長さを全角:2,半角:1としてカウントした場合の、maxCharを超えないサイズを計算する.
         * 文字が既定の長さより短い場合は-1を返す。
            @brief	clampZenHan.
        */
        static public int clampZenHan(string str, int maxChar) {
            if (str.Length <= maxChar / 2) {
                return -1;
            }
            int n = 0;
            for (int i = 0; i < str.Length; ++i) {
                char c = str[i];
                if (c < 0x100) {
                    ++n;
                } else {
                    n += 2;
                }
                if (n > maxChar) {
                    return i;
                }
            }
            return -1;
        }
        //==========================================================================
        /*!UI用のカメラコンポーネントを追加する.
            * @brief	addUICamera
        */
        static public Camera addUICamera(GameObject go, e_Layer eLayer, float depth) {
            Camera camera = go.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.Nothing;
            camera.cullingMask = (int)eLayer;
            camera.orthographic = true;
            camera.orthographicSize = 100f;
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 1000f;
            camera.depth = depth;
            camera.useOcclusionCulling = false;
            camera.allowHDR = false;
            camera.allowMSAA = false;
            if (camera != null) {
                camera.orthographicSize = Screen.height / 2;
            }
            return camera;
        }




        //==========================================================================
        /*!2D素材がボケないように座標を補正する.
         * @brief	pixelPerfect
        */
        static public Vector3 pixelPerfect(Vector3 pos) {
            pos.x = Mathf.Floor(pos.x);
            pos.y = Mathf.Floor(pos.y);
            return pos;
        }
        static public Vector2 pixelPerfect(Vector2 pos) {
            pos.x = Mathf.Floor(pos.x);
            pos.y = Mathf.Floor(pos.y);
            return pos;
        }
        static public Vector2 pixelPerfect(float x, float y) {
            Vector2 pos;
            pos.x = Mathf.Floor(x);
            pos.y = Mathf.Floor(y);
            return pos;
        }




        //==========================================================================
        /*!特定のレイヤーに属するコンポーネントを列挙する.
         * @brief getComponentsInGameObject
        */
        static public Type[] getComponentsInGameObject<Type>(GameObject go, e_LayerId eLayerId) where Type : Component {
            Type[] componets = go.GetComponentsInChildren<Type>();
            List<Type> lstResult = new List<Type>();
            for (int i = 0; i < componets.Length; ++i) {
                if (componets[i].gameObject.layer == (int)eLayerId) {
                    lstResult.Add(componets[i]);
                }
            }
            return lstResult.ToArray();
        }




        //==========================================================================
        /*!特定のTypeを除くコンポーネントを列挙する.
         * @brief getComponentsWithoutType
        */
        static public Component[] getComponentsWithoutType<Type>(GameObject go) where Type : Component {
            Component[] componets = go.GetComponentsInChildren<Component>();
            List<Component> lstResult = new List<Component>();
            for (int i = 0; i < componets.Length; ++i) {
                if (componets[i] is Type) {
                    continue;
                }
                lstResult.Add(componets[i]);
            }
            return lstResult.ToArray();
        }




        //==========================================================================
        /*!コンポーネントのenableの状態を設定する.
         * @brief setComponetsState
        */
        static public void setComponetsState(Component[] aComponent, bool enable) {
            for (int i = 0; i < aComponent.Length; ++i) {
                Component com = aComponent[i];
                MonoBehaviour mb = com as MonoBehaviour;
                if (mb != null) {
                    mb.enabled = enable;
                    continue;
                }
                Renderer r = com as Renderer;
                if (r != null) {
                    r.enabled = enable;
                    continue;
                }
                Collider col = com as Collider;
                if (col != null) {
                    col.enabled = enable;
                    continue;
                }
            }
        }




        //==========================================================================
        /*!スクリーン上のタッチ座標からヒットする対象のColliderを取得する.
            * @brief getCollider
        */
        static public Collider getCollider(Vector3 posTouch, e_Layer layerMask) {
            RaycastHit hit;
            Camera camera = Camera.main;
            posTouch.x = (posTouch.x + 1f) * Screen.width * 0.5f;
            posTouch.y = (posTouch.y + 1f) * Screen.height * 0.5f;

            Ray ray = camera.ScreenPointToRay(posTouch);
            if (!Physics.Raycast(ray, out hit, Mathf.Infinity, (int)layerMask)) {
                return null;
            }
            return hit.collider;
        }
        static public Collider getCollider(out Vector3 rPos, Vector3 posTouch, e_Layer layerMask) {
            RaycastHit hit;
            Camera camera = Camera.main;
            posTouch.x = (posTouch.x + 1f) * Screen.width * 0.5f;
            posTouch.y = (posTouch.y + 1f) * Screen.height * 0.5f;

            Ray ray = camera.ScreenPointToRay(posTouch);
            if (!Physics.Raycast(ray, out hit, Mathf.Infinity, (int)layerMask)) {
                rPos = Vector3.zero;
                return null;
            }
            rPos = hit.point;
            return hit.collider;
        }
        //==========================================================================
        /*!MulID,FiveCCどちらかに該当する文字列をID化する.
         * @brief ConvertId
        */
        static char[] chSeps = new char[] {
        ' ','('
    };
        static public uint ConvertId(string sId) {
            int idx = sId.IndexOfAny(chSeps);
            if (idx >= 0) {
                sId = sId.Remove(idx);
            }
            if (MulId.isMulId(sId)) {
                return MulId.Id(sId);
            }
            if (FiveCC.isFiveCC(sId)) {
                return FiveCC.Id(sId);
            }
            return 0;
        }




        //==========================================================================
        /*!ファイルを読み込んで、CReadVariableを返す.
            @brief	read
        */
        public static CReadVariable read(string path) {
            byte[] buffer = load(path);
            return new CReadVariable(buffer);
        }




        //==========================================================================
        /*!ファイルを読み込んで、そのバイト配列を返す.
            @brief	load
        */
        public static byte[] load(string path) {


#if !UNITY_WEBPLAYER
            if (!File.Exists(path)) {
                Debug.LogError("can't find file:" + path);
                return null;
            }
            return File.ReadAllBytes(path);







#else
		Debug.LogError("you can not load this file on web player:" + path);
		return null;

#endif
        }
        public static Material buildMaterial(t_Material tMaterial) {
            Material mat = new Material(Shader.Find(tMaterial.m_shader));
            CReadVariable cVariable = new CReadVariable(tMaterial.m_aBuffer);
            string propname = "";
            for (int i = 0; i < tMaterial.m_nProperty; ++i) {
                cVariable.getString(ref propname, 255);
                switch (cVariable.getU8()) {
                case t_Material.PropertyType_Color:
                    mat.SetColor(propname, KsSoftUtility.U32toColor(cVariable.getU32()));
                    break;
                case t_Material.PropertyType_Vector:
                    mat.SetVector(propname, cVariable.getVector4());
                    break;
                case t_Material.PropertyType_Float:
                    mat.SetFloat(propname, cVariable.getFloat());
                    break;
                case t_Material.PropertyType_Texture:
                    string texpath = "";
                    cVariable.getString(ref texpath, 255);
                    Texture tex = Resources.Load<Texture>("material/Texture/" + texpath);
                    if (tex == null) {
                        Debug.Log("can't find texture:" + texpath);
                        continue;
                    }
                    mat.SetTexture(propname, tex);
                    break;
                }
            }
            return mat;
        }
        public static void setResolution(int w, int h, bool bFullscreen, e_ResolutionPriority ePriority = e_ResolutionPriority.Auto) {
#if UNITY_ANDROID || UNITY_IOS || UNITY_EDITOR
            float ow = m_defaultResolution.x;
            float oh = m_defaultResolution.y;
            switch (ePriority) {
            case e_ResolutionPriority.Auto:
                float w0 = w;
                float h0 = oh * w0 / ow;

                float h1 = h;
                float w1 = ow * h1 / oh;

                if (w0 * h0 > w1 * h1) {
                    w = (int)w0;
                    h = (int)h0;
                } else {
                    w = (int)w1;
                    h = (int)h1;
                }
                break;
            case e_ResolutionPriority.Width:
                h = (int)(oh * w / ow);
                break;
            case e_ResolutionPriority.Height:
                w = (int)(ow * h / oh);
                break;
            }
#endif
            Screen.SetResolution(w, h, bFullscreen);
            Debug.Log("x:" + w + "  y:" + h + ":" + ePriority);
        }
        public static void setResolution(int minW, int maxW, int minH, int maxH, bool bFullscreen, e_ResolutionPriority ePriority = e_ResolutionPriority.Auto) {
            int w = Mathf.Clamp(Screen.width, minW, maxW);
            int h = Mathf.Clamp(Screen.height, minH, maxH);
            setResolution(w, h, bFullscreen, ePriority);
        }
        public static Vector3 getColliderCenter(Collider col) {
            if (col is SphereCollider) {
                SphereCollider sc = col as SphereCollider;
                return sc.center;
            }
            if (col is CapsuleCollider) {
                CapsuleCollider cc = col as CapsuleCollider;
                return cc.center;
            }
            if (col is BoxCollider) {
                BoxCollider bc = col as BoxCollider;
                return bc.center;
            }
            if (col is MeshCollider) {
                Debug.LogError("can't get center:mesh collider:" + col);
            }
            return Vector3.zero;
        }
        //==========================================================================
        /*!特定の座標が画面から外れているとき、その点と画面の中央との線分と、画面のフレームとの交点を求める.
            @brief	外れているときはtrueを返す.
        */
        public static bool calcOutsidePosition(ref Vector3 res, Vector3 pos, Camera camera, float frameWidth = 0.0f) {
            Vector2 vpos = camera.WorldToViewportPoint(pos);
            if (vpos.x >= 0 && vpos.x <= 1f && vpos.y >= 0 && vpos.y <= 1f) {
                // 内側に入っている.
                res.x *= camera.pixelWidth;
                res.y = (1.0f - res.y) * camera.pixelHeight;
                return false;
            }
            Vector2 cpos = new Vector2(0.5f, 0.5f);
            Vector2 dpos = vpos - cpos;
            if (Mathf.Abs(dpos.x) < 0.001f) {
                if (vpos.y < 0.0f) {
                    float k = -cpos.y / dpos.y;
                    res.y = 0.0f;
                    res.x = k * dpos.x + cpos.x;
                } else {
                    float k = (1.0f - cpos.y) / dpos.y;
                    res.y = 1.0f;
                    res.x = k * dpos.x + cpos.x;
                }
            } else if (Mathf.Abs(dpos.y) < 0.001f) {
                if (vpos.x < 0.0f) {
                    float k = -cpos.x / dpos.x;
                    res.x = 0.0f;
                    res.y = k * dpos.y + cpos.y;
                } else {
                    float k = (1.0f - cpos.x) / dpos.x;
                    res.x = 1.0f;
                    res.y = k * dpos.y + cpos.y;
                }
            } else if (vpos.x < 0.0f) {
                // 左チェック.
                float k = -cpos.x / dpos.x;
                res.y = k * dpos.y + cpos.y;
                if (res.y < 0.0f) {
                    //上面チェック.
                    k = -cpos.y / dpos.y;
                    res.x = k * dpos.x + cpos.x;
                    res.y = 0.0f;
                } else if (res.y > 1.0f) {
                    //下面チェック.
                    k = (1.0f - cpos.y) / dpos.y;
                    res.x = k * dpos.x + cpos.x;
                    res.y = 1.0f;
                } else {
                    res.x = 0.0f;
                }
            } else if (vpos.x > 1.0f) {
                // 右チェック.
                float k = (1.0f - cpos.x) / dpos.x;
                res.y = k * dpos.y + cpos.y;
                if (res.y < 0.0f) {
                    //上面チェック.
                    k = -cpos.y / dpos.y;
                    res.x = k * dpos.x + cpos.x;
                    res.y = 0.0f;
                } else if (res.y > 1.0f) {
                    //下面チェック.
                    k = (1.0f - cpos.y) / dpos.y;
                    res.x = k * dpos.x + cpos.x;
                    res.y = 1.0f;
                } else {
                    res.x = 1.0f;
                }
            } else if (vpos.y < 0.0f) {
                // 上面チェック.
                float k = -cpos.y / dpos.y;
                res.x = k * dpos.x + cpos.x;
                if (res.x < 0.0f) {
                    //左チェック.
                    k = -cpos.x / dpos.x;
                    res.y = k * dpos.y + cpos.y;
                    res.x = 0.0f;
                } else if (res.x > 1.0f) {
                    //右チェック.
                    k = (1.0f - cpos.x) / dpos.x;
                    res.y = k * dpos.y + cpos.y;
                    res.x = 1.0f;
                } else {
                    res.y = 0.0f;
                }
            } else if (vpos.y > 1.0f) {
                // 下面チェック.
                float k = (1.0f - cpos.y) / dpos.y;
                res.x = k * dpos.x + cpos.x;
                if (res.x < 0.0f) {
                    //左チェック.
                    k = -cpos.x / dpos.x;
                    res.y = k * dpos.y + cpos.y;
                    res.x = 0.0f;
                } else if (res.x > 1.0f) {
                    //右チェック.
                    k = (1.0f - cpos.x) / dpos.x;
                    res.y = k * dpos.y + cpos.y;
                    res.x = 1.0f;
                } else {
                    res.y = 1.0f;
                }
            }
            res.x *= (camera.pixelWidth - frameWidth * 2.0f);
            res.y = (1.0f - res.y) * (camera.pixelHeight - frameWidth * 2.0f);
            return true;
        }
#if UNITY_EDITOR
        public static void DrawSphere(Vector3 center, float r, Color32 col) {
            Vector2 oldsc;
            oldsc.x = 0.0f;
            oldsc.y = r;
            for (int ang = Angle.Deg2Ang(30); ang < Angle.Deg2Ang(350); ang += Angle.Deg2Ang(30)) {
                Vector2 sc = Angle.SinCos(ang) * r;
                Vector3 pos0 = center;
                Vector3 pos1 = center;
                pos0.x += oldsc.x;
                pos0.y += oldsc.y;
                pos1.x += sc.x;
                pos1.y += sc.y;
                Debug.DrawLine(pos0, pos1, col);
                pos0 = center;
                pos1 = center;
                pos0.x += oldsc.x;
                pos0.z += oldsc.y;
                pos1.x += sc.x;
                pos1.z += sc.y;
                Debug.DrawLine(pos0, pos1, col);
                pos0 = center;
                pos1 = center;
                pos0.y += oldsc.x;
                pos0.z += oldsc.y;
                pos1.y += sc.x;
                pos1.z += sc.y;
                Debug.DrawLine(pos0, pos1, col);
                oldsc = sc;
            }
        }
#endif
        //==========================================================================
        /*!スクリーンショットを取る.
            @brief	screenShot.
        */
        static public void screenShot(RenderTexture rt, string path) {
            RenderTexture oldRT = RenderTexture.active;

            Texture2D tex = new Texture2D(rt.width, rt.height);
            RenderTexture.active = rt;
            tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            tex.Apply();

            File.WriteAllBytes(path, tex.EncodeToPNG());
            RenderTexture.active = oldRT;
            UnityEngine.Object.DestroyImmediate(tex);
        }
        static public Vector2 defaultResolution {
            get {
                return m_defaultResolution;
            }
        }
    }
}
