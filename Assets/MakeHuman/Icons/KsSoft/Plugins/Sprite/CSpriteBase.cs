//==============================================================================================
/*!テクスチャアトラスによって一つにまとまったスプライトをまとめてレンダリングする.
	@file  Billboard Effect

	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace KS {
    public class CSpriteBase : MonoBehaviour {
        public int m_maxSpriteNum = 256;
        [System.Flags]
        public enum e_Format {
            Vertices = 1 << 0,
            Normals = 1 << 1,
            Tangents = 1 << 2,
            Colors = 1 << 3,
            UV = 1 << 4,
            UV2 = 1 << 5,
        };
        protected Mesh m_mesh = null;
        protected MeshRenderer m_renderer = null;
        protected int m_index;
        protected Material m_material = null;
        protected Vector3[] m_vertices = null;
        protected Vector3[] m_normals = null;
        protected Vector4[] m_tangents = null;
        protected Color32[] m_colors = null;
        protected Vector2[] m_uv = null;
        protected Vector2[] m_uv2 = null;
        protected int[] m_triangles = null;
        protected Vector2 m_size = Vector2.zero;
        protected Vector4 m_uvNull = Vector4.zero;  //パーツが見つからないとき返すUV値.
        protected Dictionary<uint, CSpriteDataOne> m_dicSDO = new Dictionary<uint, CSpriteDataOne>();
        protected CTextureResource m_cResource;
        protected bool m_isLoaded = false;
        protected bool m_invertY = false;

        static protected Vector4 m_uvCenter;
        static protected Vector4 m_uvTop;
        static protected Vector4 m_uvBottom;
        static protected Vector4 m_uvLeftTop;
        static private Bounds m_bounds;
        static protected uint m_fcNull;

        static CSpriteBase() {
            m_uvCenter = new Vector4(-0.5f, -0.5f, 0.5f, 0.5f);
            m_uvTop = new Vector4(-0.5f, -1.0f, 0.5f, 0.0f);
            m_uvBottom = new Vector4(-0.5f, 0.0f, 0.5f, 1.0f);

            m_uvLeftTop = new Vector4(0f, 0, 1f, 1f);

            m_bounds = new Bounds(Vector3.zero, new Vector3(65536f, 65536f, 65536f));

            m_fcNull = FiveCC.Id("NULL");
        }
        //===========================================================ex===============
        /*!isLoaded
         * @brief	読み込み中かどうか判定する.
        */
        public bool isLoaded {
            get {
                return m_isLoaded;
            }
            set {
                m_isLoaded = value;
            }
        }
        //==========================================================================
        /*!頂点フォーマットを設定する.
            @brief	format
        */
        public e_Format format {
            set {
                e_Format eFormat = value | e_Format.Vertices;
                int vnum = m_maxSpriteNum * 4;
                if (m_vertices == null) {
                    m_vertices = new Vector3[vnum];
                }
                if (m_normals == null) {
                    if ((eFormat & e_Format.Normals) != 0) {
                        m_normals = new Vector3[vnum];
                    }
                }
                if (m_tangents == null) {
                    if ((eFormat & e_Format.Tangents) != 0) {
                        m_tangents = new Vector4[vnum];
                    }
                }
                if (m_colors == null) {
                    if ((eFormat & e_Format.Colors) != 0) {
                        m_colors = new Color32[vnum];
                    }
                } else {
                    if ((eFormat & e_Format.Colors) == 0) {
                        m_colors = null;
                    }
                }
                if (m_uv == null) {
                    if ((eFormat & (e_Format.UV | e_Format.UV2)) != 0) {
                        m_uv = new Vector2[vnum];
                    }
                } else {
                    if ((eFormat & (e_Format.UV | e_Format.UV2)) == 0) {
                        m_uv = null;
                    }
                }
                if (m_uv2 == null) {
                    if ((eFormat & e_Format.UV2) != 0) {
                        m_uv2 = new Vector2[vnum];
                    }
                } else {
                    if ((eFormat & e_Format.UV2) == 0) {
                        m_uv2 = null;
                    }
                }
                if (m_triangles == null) {
                    m_triangles = new int[m_maxSpriteNum * 6];
                    for (int i = 0; i < m_maxSpriteNum; i++) {
                        int tri = i * 6;
                        int v = i * 4;
                        m_triangles[tri++] = v;
                        m_triangles[tri++] = v + 1;
                        m_triangles[tri++] = v + 2;
                        m_triangles[tri++] = v + 1;
                        m_triangles[tri++] = v + 3;
                        m_triangles[tri++] = v + 2;
                    }
                }
            }
        }
        //==========================================================================
        /*!LateUpdate
            @brief	Unity Callback
        */
        protected void LateUpdate() {
            update();
            buildMesh();
        }
        //==========================================================================
        /*!Release
            @brief	release
        */
        public virtual void release() {
            m_mesh = null;
            m_vertices = null;
            m_normals = null;
            m_tangents = null;
            m_colors = null;
            m_uv = null;
            m_uv2 = null;
            m_triangles = null;
            m_dicSDO.Clear();
        }
        //==========================================================================
        /*!Initialize
            @brief	initialize
        */
        virtual public int initialize(Material material, CSpriteDataOne[] aData) {
            initialize(material);
            // アニメーションデータ格納.
            if (aData != null) {
                foreach (CSpriteDataOne cSDO in aData) {
                    m_dicSDO[(uint)cSDO.m_id] = cSDO;
                    if (m_fcNull == cSDO.m_id) {
                        m_uvNull = cSDO.m_uv;
                    }
                }
            }
            return 0;
        }
        //==========================================================================
        /*!Initialize
            @brief	initialize
        */
        virtual public int initialize(CSpriteBase src) {
            initialize(src.m_material);
            // アニメーションデータ格納.
            m_dicSDO = src.m_dicSDO;
            return 0;
        }
        //==========================================================================
        /*!initialize
         * @brief	initialize
        */
        protected bool initialize(CTextureResource cTR) {
            // 一枚絵の場合はリソースを複製する.
            if (!cTR.isLoaded) {
                return false;
            }
            m_cResource = cTR;
            Material material = m_cResource.get();
            if (m_cResource.spriteData == null) {
                material = new Material(material);
                material.mainTexture = GameObject.Instantiate(material.mainTexture) as Texture2D;
            }
            initialize(material, m_cResource.spriteData);
            m_isLoaded = true;
            return true;
        }
        //==========================================================================
        /*!Initialize
            @brief	initialize
        */
        virtual public int initialize(Material material) {
            if (material == null) {
                Debug.LogError("materil == null");
                return -1;
            }
            m_mesh = new Mesh();

            MeshFilter mf = GetComponent<MeshFilter>();
            mf.mesh = m_mesh;

            // マテリアル割り当て.
            m_material = material;

            m_renderer = GetComponent<MeshRenderer>();

            if (m_renderer != null) {
                m_renderer.material = material;
            }
            if (material.mainTexture == null) {
                Debug.LogError("materil.mainTexture == null");
                return -1;
            }

            m_size.x = (float)material.mainTexture.width;
            m_size.y = (float)material.mainTexture.height;
            return 0;
        }
        //==========================================================================
        /*!レンダラーコンポーネントを追加する.
            @brief	addRenderer
        */
        protected void addRenderer(e_LayerId eLayer) {
            gameObject.AddComponent<MeshFilter>();
            MeshRenderer mr = gameObject.AddComponent<MeshRenderer>();
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            mr.receiveShadows = false;
            gameObject.layer = (int)eLayer;
        }
        //==========================================================================
        /*!UVを参照する.
            @brief	reference
        */
        public Vector4 reference(uint id) {
            CSpriteDataOne cSDO;
            if (id == 0) {
                return m_uvNull;
            }
            if (m_dicSDO.TryGetValue(id, out cSDO)) {
                return cSDO.m_uv;
            }
            Debug.LogWarning("can't find part id:" + new FiveCC(id));
            return m_uvNull;
        }
        public CSpriteDataOne referenceSDO(uint id) {
            CSpriteDataOne cSDO;
            if (id == 0) {
                return null;
            }
            if (m_dicSDO.TryGetValue(id, out cSDO)) {
                return cSDO;
            }
            Debug.LogWarning("can't find part id:" + new FiveCC(id));

            if (m_dicSDO.TryGetValue(m_fcNull, out cSDO)) {
                return cSDO;
            }
            return null;
        }
        //==========================================================================
        /*!Update.
            @brief	update
        */
        virtual protected void update() {
        }
        //==========================================================================
        /*!頂点バッファサイズを拡張する.
            @brief	expand
        */
        public void expand(int n = 0) {
            int nExpand = n + m_maxSpriteNum;
            int size = m_vertices.Length + nExpand * 4;
            int oldTri = m_triangles.Length / 6;
            int newTri = m_triangles.Length / 6 + nExpand;
            Array.Resize(ref m_triangles, newTri * 6);
            for (int i = oldTri; i < newTri; i++) {
                int tri = i * 6;
                int v = i * 4;
                m_triangles[tri++] = v;
                m_triangles[tri++] = v + 1;
                m_triangles[tri++] = v + 2;
                m_triangles[tri++] = v + 1;
                m_triangles[tri++] = v + 3;
                m_triangles[tri++] = v + 2;
            }

            if (m_vertices != null) {
                Array.Resize(ref m_vertices, size);
            }
            if (m_normals != null) {
                Array.Resize(ref m_normals, size);
            }
            if (m_tangents != null) {
                Array.Resize(ref m_tangents, size);
            }
            if (m_uv != null) {
                Array.Resize(ref m_uv, size);
            }
            if (m_uv2 != null) {
                Array.Resize(ref m_uv2, size);
            }
            if (m_colors != null) {
                Array.Resize(ref m_colors, size);
            }
        }
        //==========================================================================
        /*!頂点を一つ追加する.
            @brief	addVert
        */
        public void addVert(Vector3 vert) {
            if (m_index + 1 >= m_vertices.Length) {
                expand();
            }
            m_vertices[m_index] = vert;
            m_index++;
        }
        public void addVert(Vector3 vert, Color32 color) {
            if (m_index + 1 >= m_vertices.Length) {
                expand();
            }
            m_vertices[m_index] = vert;
            m_colors[m_index] = color;
            m_index++;
        }
        public void addVert(Vector3 vert, Vector2 uv) {
            if (m_index + 1 >= m_vertices.Length) {
                expand();
            }
            m_vertices[m_index] = vert;
            m_uv[m_index] = uv;
            m_index++;
        }
        public void addVert(Vector3 vert, Color32 color, Vector2 uv) {
            if (m_index + 1 >= m_vertices.Length) {
                expand();
            }
            m_vertices[m_index] = vert;
            m_colors[m_index] = color;
            m_uv[m_index] = uv;
            m_index++;
        }
        public void addVert(Vector3 vert, Color32 color, Vector2 uv, Vector2 uv2) {
            if (m_index + 1 >= m_vertices.Length) {
                expand();
            }
            m_vertices[m_index] = vert;
            m_colors[m_index] = color;
            m_uv[m_index] = uv;
            m_uv2[m_index] = uv2;
            m_index++;
        }
        //==========================================================================
        /*!ビルボードを追加する.
            @brief	addBillboard
        */
        public void addBillboard(e_BillboardOrigin eOrigin, Vector3 pos, Color32 color, Vector4 uv, int angle, Vector2 size) {
            if (m_index + 4 >= m_vertices.Length) {
                expand();
            }
            int i = m_index;
            // カラー.
            m_colors[i + 0] = color;
            m_colors[i + 1] = color;
            m_colors[i + 2] = color;
            m_colors[i + 3] = color;

            addBillboard(eOrigin, pos, uv, angle, size);
        }
        //==========================================================================
        /*!ビルボードを追加する.
            @brief	addBillboard
        */
        public void addBillboard(e_BillboardOrigin eOrigin, Vector3 pos, Vector4 uv, int angle, Vector2 size) {
            if (m_index + 4 >= m_vertices.Length) {
                expand();
            }

            int i = m_index;
            // 頂点.
            m_vertices[i + 0] = pos;
            m_vertices[i + 1] = pos;
            m_vertices[i + 2] = pos;
            m_vertices[i + 3] = pos;
            // UV.
            m_uv[i + 0].x = uv.x;
            m_uv[i + 0].y = uv.y;
            m_uv[i + 1].x = uv.z;
            m_uv[i + 1].y = uv.y;
            m_uv[i + 2].x = uv.x;
            m_uv[i + 2].y = uv.w;
            m_uv[i + 3].x = uv.z;
            m_uv[i + 3].y = uv.w;
            // ビルボード用UV.
            Vector4 uvOffset = getUVOffset(eOrigin);
            if (angle == 0) {
                m_uv2[i + 0].x = uvOffset.x * size.x;
                m_uv2[i + 0].y = uvOffset.y * size.y;
                m_uv2[i + 1].x = uvOffset.z * size.x;
                m_uv2[i + 1].y = uvOffset.y * size.y;
                m_uv2[i + 2].x = uvOffset.x * size.x;
                m_uv2[i + 2].y = uvOffset.w * size.y;
                m_uv2[i + 3].x = uvOffset.z * size.x;
                m_uv2[i + 3].y = uvOffset.w * size.y;
            } else {
                Vector2 sc = Angle.SinCos(angle);

                Vector2 uvorg;
                uvorg.x = uvOffset.x * size.x;
                uvorg.y = uvOffset.y * size.y;
                m_uv2[i + 0].x = uvorg.x * sc.y - uvorg.y * sc.x;
                m_uv2[i + 0].y = uvorg.x * sc.x + uvorg.y * sc.y;

                uvorg.x = uvOffset.z * size.x;
                uvorg.y = uvOffset.y * size.y;
                m_uv2[i + 1].x = uvorg.x * sc.y - uvorg.y * sc.x;
                m_uv2[i + 1].y = uvorg.x * sc.x + uvorg.y * sc.y;

                uvorg.x = uvOffset.x * size.x;
                uvorg.y = uvOffset.w * size.y;
                m_uv2[i + 2].x = uvorg.x * sc.y - uvorg.y * sc.x;
                m_uv2[i + 2].y = uvorg.x * sc.x + uvorg.y * sc.y;

                uvorg.x = uvOffset.z * size.x;
                uvorg.y = uvOffset.w * size.y;
                m_uv2[i + 3].x = uvorg.x * sc.y - uvorg.y * sc.x;
                m_uv2[i + 3].y = uvorg.x * sc.x + uvorg.y * sc.y;
            }
            m_index += 4;
        }
        //==========================================================================
        /*!ビルボードの中心位置を取得.
            @brief	getUVOffset
        */
        protected virtual Vector4 getUVOffset(e_BillboardOrigin eOrigin) {
            switch (eOrigin) {
            case e_BillboardOrigin.Top:
                return m_uvTop;
            case e_BillboardOrigin.Bottom:
                return m_uvBottom;
            }
            return m_uvCenter;
        }
        //==========================================================================
        /*!メッシュ生成.
            @brief	buildMesh
        */
        protected virtual bool buildMesh() {
            if (m_mesh == null) {
                return false;
            }
            m_mesh.triangles = null;
            if (m_index == 0) {
                m_renderer.enabled = false;
                return false;
            }
            m_renderer.enabled = true;
            if (m_vertices != null) {
                Vector3[] vertices = new Vector3[m_index];
                if (m_invertY) {
                    for (int i = 0; i < vertices.Length; ++i) {
                        Vector3 v = m_vertices[i];
                        v.y = -v.y;
                        vertices[i] = v;
                    }
                } else {
                    System.Array.Copy(m_vertices, vertices, m_index);
                }
                //			Buffer.BlockCopy(m_vertices,0,vertices,0,m_index);
                m_mesh.vertices = vertices;
            }
            if (m_normals != null) {
                Vector3[] normals = new Vector3[m_index];
                System.Array.Copy(m_normals, normals, m_index);
                //			Buffer.BlockCopy(m_vertices,0,vertices,0,m_index);
                m_mesh.normals = normals;
            }
            if (m_tangents != null) {
                Vector4[] tangents = new Vector4[m_index];
                System.Array.Copy(m_tangents, tangents, m_index);
                //			Buffer.BlockCopy(m_tangents,0,tangents,0,m_index);
                m_mesh.tangents = tangents;
            }
            if (m_colors != null) {
                Color32[] colors = new Color32[m_index];
                System.Array.Copy(m_colors, colors, m_index);
                //			Buffer.BlockCopy(m_colors,0,colors,0,m_index);
                m_mesh.colors32 = colors;
            }
            if (m_uv != null) {
                Vector2[] uv = new Vector2[m_index];
                System.Array.Copy(m_uv, uv, m_index);
                //			Buffer.BlockCopy(m_uv,0,uv,0,m_index);
                m_mesh.uv = uv;
            }
            if (m_uv2 != null) {
                Vector2[] uv2 = new Vector2[m_index];
                System.Array.Copy(m_uv2, uv2, m_index);
                //			Buffer.BlockCopy(m_uv2,0,uv2,0,m_index);
                m_mesh.uv2 = uv2;
            }
            int trinum = m_index * 6 / 4;
            int[] triangles = new int[trinum];
            //		Buffer.BlockCopy(m_triangles,0,triangles,0,trinum * 4);
            System.Array.Copy(m_triangles, triangles, trinum);
            m_mesh.triangles = triangles;

            m_mesh.bounds = m_bounds;

            m_index = 0;
            return true;
        }
        //==========================================================================
        /*!テクスチャサイズ.
            @brief	texSize
        */
        public Vector2 texSize {
            get {
                return m_size;
            }
        }
        public Material material {
            get {
                return m_material;
            }
        }
        //==========================================================================
        /*!直アクセス用プロパティ.
         * @note	使用する際は気を付けてください.
        */
        public int maxindex {
            get {
                return m_vertices.Length;
            }
        }
        public int index {
            get {
                return m_index;
            }
            set {
                m_index = value;
            }
        }
        public Vector3[] vertices {
            get {
                return m_vertices;
            }
        }
        public Vector3[] normals {
            get {
                return m_normals;
            }
        }
        public Color32[] colors {
            get {
                return m_colors;
            }
        }
        public Vector2[] uvs {
            get {
                return m_uv;
            }
        }
        public CTextureResource resource {
            get {
                return m_cResource;
            }
        }
        public MeshRenderer render {
            get {
                return m_renderer;
            }
        }
        public bool isInvert {
            get {
                return m_invertY;
            }
            set {
                m_invertY = value;
            }
        }
    };
}
