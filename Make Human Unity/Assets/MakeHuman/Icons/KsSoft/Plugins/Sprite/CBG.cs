//==============================================================================================
/*!バックグラウンド.
	@file  CBG

	(counter SJIS string ??)
*/
//==============================================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KS {
    public class CBG : MonoBehaviour {
        public Material m_material;
        protected Vector2 m_size;
        protected Camera m_camera;
        protected Mesh m_mesh;
        protected MeshFilter m_meshFilter;
        protected Vector3[] m_vertices = new Vector3[4];
        protected Vector2[] m_uvs = new Vector2[4];
        protected Color32[] m_colors = new Color32[4];
        protected Color32 m_color = new Color(0.5f, 0.5f, 0.5f, 1f);
        protected int[] m_triangles = new int[6];
        protected Bounds m_bounds;
        protected e_LayerId m_eLayerId = e_LayerId.BG;
        protected float m_depth = 0f;
        protected Color m_colMaterial;
        //==========================================================================
        /*!Awake
            @brief	Unity Callback
        */
        protected void Awake() {
            m_camera = KsSoftUtility.addUICamera(gameObject, (e_Layer)(1 << (int)m_eLayerId), m_depth);
            initialize((float)Screen.width, (float)Screen.height, m_material);
            transform.position = new Vector3(0f, 0f, -100f);

        }
        //==========================================================================
        /*!Update
         * @brief	Unity Callback
        */
        protected void Update() {
            CWindowMgr cWindowMgr = CWindowMgr.Instance;
            if (cWindowMgr == null) {
                return;
            }
            m_camera.orthographicSize = Screen.height / 2;
            CWindowBase cWindow = cWindowMgr.topWindow;
            if (cWindow != null && (cWindow.priorityStyle == e_WinStyle.POPUP || cWindow.priorityStyle == e_WinStyle.TOPMOST)) {
                Color col = m_colMaterial;
                col.r *= 0.5f;
                col.g *= 0.5f;
                col.b *= 0.5f;
                m_material.color = col;
            } else {
                m_material.color = m_colMaterial;
            }
            float width = (float)Screen.width;
            float height = (float)Screen.height;

            if (m_size.x != width || m_size.y != height) {
                createMesh(width, height);
            }
        }

        protected void initialize(float width, float height, Texture2D texture, string sShader) {
            Material material = new Material(Shader.Find(sShader));
            material.color = Color.white;
            material.mainTexture = texture;
            initialize(width, height, material);
        }
        protected void initialize(float width, float height, Material material) {
            MeshRenderer mr = gameObject.AddComponent<MeshRenderer>();
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            mr.receiveShadows = false;
            m_meshFilter = gameObject.AddComponent<MeshFilter>();
            m_mesh = new Mesh();

            m_material = material;

            m_meshFilter.mesh = m_mesh;

            mr.material = m_material;

            createMesh(width, height);
        }
        protected virtual void createMesh(float width, float height) {
            m_size.x = width;
            m_size.y = height;

            m_vertices[0] = new Vector3(-width * 0.5f - 1f, height * 0.5f + 1f, 50.0f);
            m_vertices[1] = new Vector3(width * 0.5f, height * 0.5f + 1f, 50.0f);
            m_vertices[2] = new Vector3(-width * 0.5f - 1f, -height * 0.5f, 50.0f);
            m_vertices[3] = new Vector3(width * 0.5f, -height * 0.5f, 50.0f);

            m_uvs[0] = new Vector2(0.0f, 1.0f);
            m_uvs[1] = new Vector2(1.0f, 1.0f);
            m_uvs[2] = new Vector2(0.0f, 0.0f);
            m_uvs[3] = new Vector2(1.0f, 0.0f);

            m_colors[0] = m_color;
            m_colors[1] = m_color;
            m_colors[2] = m_color;
            m_colors[3] = m_color;

            m_triangles[0] = 0;
            m_triangles[1] = 1;
            m_triangles[2] = 2;
            m_triangles[3] = 1;
            m_triangles[4] = 3;
            m_triangles[5] = 2;

            m_bounds = new Bounds(Vector3.zero, new Vector3(65536f, 65536f, 65536f));

            m_mesh.vertices = m_vertices;
            m_mesh.uv = m_uvs;
            m_mesh.colors32 = m_colors;
            m_mesh.triangles = m_triangles;
            m_mesh.bounds = m_bounds;

            gameObject.layer = (int)m_eLayerId;
            transform.localRotation = Quaternion.identity;
        }
        public virtual Color color {
            get {
                return m_color;
            }
            set {
                if (m_color == value) {
                    return;
                }
                m_color = value;
                createMesh(m_size.x, m_size.y);
            }
        }
        public Color materialColor {
            get {
                return m_colMaterial;
            }
            set {
                m_colMaterial = value;
            }
        }
    };
}
