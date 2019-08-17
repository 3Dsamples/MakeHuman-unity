using UnityEngine;
using System.Collections;
namespace KS {
    public class Parts {
        public struct Part {
            public Vector4 vert;
            public Vector4 uv;
        };
        e_Patch m_ePatch;
        Vector4 m_uv;
        WinColor m_color;
        Part[] m_aPart;

        public Parts(Vector4 vert, Vector4 uv) {
            m_ePatch = e_Patch.None;
            m_uv = uv;
            m_aPart = new Part[1];
            m_aPart[0].vert = vert;
            m_aPart[0].uv = uv;
            m_color = WinColor.white;
        }
        public Parts(Parts src) {
            m_ePatch = src.m_ePatch;
            m_uv = src.m_uv;
            m_color = src.m_color;
            m_aPart = new Part[src.m_aPart.Length];
            for (int i = 0; i < m_aPart.Length; ++i) {
                m_aPart[i] = src.m_aPart[i];
            }
        }
        public Parts(CSpriteDataOne cSDO) {
            m_ePatch = cSDO.m_ePatch;
            m_aPart = new Part[cSDO.m_aUV.Length];
            m_uv = cSDO.m_uv;
            m_color = cSDO.m_color;
            for (int i = 0; i < m_aPart.Length; ++i) {
                Part part;
                part.uv = cSDO.m_aUV[i];
                part.vert = Vector4.zero;
                m_aPart[i] = part;
            }
        }
        public Part[] part {
            get {
                return m_aPart;
            }
        }
        public e_Patch patch {
            get {
                return m_ePatch;
            }
        }
        public int Length {
            get {
                return m_aPart.Length;
            }
        }
        public Vector4 uv {
            get {
                return m_uv;
            }
        }
        public WinColor color {
            get {
                return m_color;
            }
            set {
                m_color = value;
            }
        }
        public float width {
            get {
                return m_aPart[m_aPart.Length - 1].vert.z - m_aPart[0].vert.x;
            }
        }
        public float minwidth {
            get {
                switch (m_ePatch) {
                case e_Patch.H3:
                    return m_aPart[0].vert.z - m_aPart[0].vert.x + m_aPart[2].vert.z - m_aPart[2].vert.x;
                case e_Patch.V3:
                    return m_aPart[0].vert.z - m_aPart[0].vert.x;
                case e_Patch.HV9:
                    return m_aPart[0].vert.z - m_aPart[0].vert.x + m_aPart[8].vert.z - m_aPart[8].vert.x;
                }
                return 0f;
            }
        }
        public float height {
            get {
                return m_aPart[m_aPart.Length - 1].vert.w - m_aPart[0].vert.y;
            }
        }
        public float minheight {
            get {
                switch (m_ePatch) {
                case e_Patch.H3:
                    return m_aPart[0].vert.w - m_aPart[0].vert.y + m_aPart[2].vert.w - m_aPart[2].vert.y;
                case e_Patch.V3:
                    return m_aPart[0].vert.w - m_aPart[0].vert.y;
                case e_Patch.HV9:
                    return m_aPart[0].vert.w - m_aPart[0].vert.y + m_aPart[8].vert.w - m_aPart[8].vert.y;
                }
                return 0f;
            }
        }
    }

    public class CSprite : CSpriteBase {
        protected uint m_id;
        public struct Vertex {
            public Vector2 pos;
            public Vector2 uv;
            public Vertex(float x, float y, Vector2 uv) {
                this.pos.x = x;
                this.pos.y = y;
                this.uv = uv;
            }
        };
        protected delegate Vertex DelIntersect(Vertex vBegin, Vertex vEnd, float intersect);
        protected delegate bool DelCheckIntersect(Vector2 pos, float intersect);
        public const int Flag_UVRotateMask = 3;
        public const int Flag_UVRotate0 = 0;
        public const int Flag_UVRotate90 = 1;
        public const int Flag_UVRotate180 = 2;
        public const int Flag_UVRotate270 = 3;
        public const int Flag_UVReverseH = 4;
        public const int Flag_UVReverseV = 8;
        const float forceZero = -65536f;
        //==========================================================================
        /*!スプライトレンダリングオブジェクトを生成する.
         * @brief	create
        */
        static public CSprite create(CTextureResource cResource, float zoffset = -1f / 65536f) {
            GameObject go = new GameObject("spr:" + new MulId(cResource.id));
            go.transform.localPosition = new Vector3(0f, 0f, zoffset);
            // メッシュをアタッチ.
            CSprite cSpr = go.AddComponent<CSprite>();
            cSpr.m_id = cResource.id;
            cSpr.initialize(cResource);
            return cSpr;
        }
        //==========================================================================
        /*!Awake
            @brief	Unity Callback
        */
        public void Awake() {
            format = e_Format.UV | e_Format.Colors;
            e_LayerId eLayerId = e_LayerId.Window;
            addRenderer(eLayerId);
        }
        //==========================================================================
        /*!Start
         * @brief	Unity Callback
        */
        IEnumerator Start() {
            if (m_cResource == null) {
                yield break;
            }
            while (!m_cResource.isLoaded) {
                yield return 0;
            }
            if (!m_isLoaded) {
                initialize(m_cResource);
            }
        }
        //==========================================================================
        /*!スプライト情報を一つ生成する.
            @brief	create
        */
        public Parts create(uint id, Vector2 size, e_Anchor eAnchor = e_Anchor.LeftTop) {
            CSpriteDataOne cSDO = referenceSDO(id);
            if (cSDO == null) {
                return null;
            }
            Parts parts = new Parts(cSDO);
            resize(parts, size, eAnchor);
            return parts;
        }
        //==========================================================================
        /*!スプライト情報のサイズを変更する.
            @brief	resize
        */
        public void resize(Parts parts, Vector2 size, e_Anchor eAnchor = e_Anchor.LeftTop) {
            Parts.Part[] aPart = parts.part;
            switch (parts.patch) {
            case e_Patch.None:
                setPart(ref aPart[0], size);
                break;
            case e_Patch.H3: {
                Vector2 sz = new Vector2(0f, size.y);
                setPart(ref aPart[0], sz);
                setPart(ref aPart[2], sz);
                float w = aPart[0].vert.z + aPart[2].vert.z;
                if (size.x < w) {
                    size.x = forceZero;
                } else {
                    size.x = size.x - w;
                }
                setPart(ref aPart[1], size);
                aPart[1].vert.x += aPart[0].vert.z;
                aPart[1].vert.z += aPart[0].vert.z;
                aPart[2].vert.x += aPart[1].vert.z;
                aPart[2].vert.z += aPart[1].vert.z;
            }
            break;
            case e_Patch.V3: {
                Vector2 sz = new Vector2(size.x, 0f);
                setPart(ref aPart[0], sz);
                setPart(ref aPart[2], sz);
                float h = aPart[0].vert.w + aPart[2].vert.w;
                if (size.y < h) {
                    size.y = forceZero;
                } else {
                    size.y = size.y - h;
                }
                setPart(ref aPart[1], size);
                aPart[1].vert.y += aPart[2].vert.w;
                aPart[1].vert.w += aPart[2].vert.w;
                aPart[0].vert.y += aPart[1].vert.w;
                aPart[0].vert.w += aPart[1].vert.w;
            }
            break;
            case e_Patch.HV9: {
                // top
                setPart(ref aPart[0], Vector2.zero);
                setPart(ref aPart[2], Vector2.zero);
                float w = size.x - (aPart[0].vert.z + aPart[2].vert.z);
                if (w < 0f) {
                    w = 0f;
                }
                setPart(ref aPart[1], new Vector2(w, 0f));
                // bottom
                setPart(ref aPart[6], Vector2.zero);
                setPart(ref aPart[8], Vector2.zero);
                w = size.x - (aPart[6].vert.z + aPart[8].vert.z);
                if (w < 0f) {
                    w = forceZero;
                }
                setPart(ref aPart[7], new Vector2(w, 0f));

                // center
                float h = size.y - (aPart[1].vert.w + aPart[7].vert.w);
                if (h < 0f) {
                    h = forceZero;
                }
                setPart(ref aPart[3], new Vector2(0f, h));
                setPart(ref aPart[5], new Vector2(0f, h));
                w = size.x - (aPart[3].vert.z + aPart[5].vert.z);
                if (w < 0f) {
                    w = forceZero;
                }
                setPart(ref aPart[4], new Vector2(w, h));

                aPart[1].vert.x += aPart[0].vert.z;
                aPart[1].vert.z += aPart[0].vert.z;
                aPart[2].vert.x += aPart[1].vert.z;
                aPart[2].vert.z += aPart[1].vert.z;
                float ofty = aPart[0].vert.w;
                aPart[3].vert.y += ofty;
                aPart[3].vert.w += ofty;
                aPart[4].vert.y += ofty;
                aPart[4].vert.w += ofty;
                aPart[5].vert.y += ofty;
                aPart[5].vert.w += ofty;
                aPart[4].vert.x += aPart[3].vert.z;
                aPart[4].vert.z += aPart[3].vert.z;
                aPart[5].vert.x += aPart[4].vert.z;
                aPart[5].vert.z += aPart[4].vert.z;

                ofty = aPart[3].vert.w;
                aPart[6].vert.y += ofty;
                aPart[6].vert.w += ofty;
                aPart[7].vert.y += ofty;
                aPart[7].vert.w += ofty;
                aPart[8].vert.y += ofty;
                aPart[8].vert.w += ofty;
                aPart[7].vert.x += aPart[6].vert.z;
                aPart[7].vert.z += aPart[6].vert.z;
                aPart[8].vert.x += aPart[7].vert.z;
                aPart[8].vert.z += aPart[7].vert.z;
            }
            break;
            }
            if (eAnchor != e_Anchor.LeftTop) {
                float dx = 0f;
                float dy = 0f;
                if (eAnchor == e_Anchor.Center || eAnchor == e_Anchor.Top || eAnchor == e_Anchor.Bottom) {
                    dx = -0.5f * parts.width;
                } else if (eAnchor == e_Anchor.RightCenter || eAnchor == e_Anchor.RightTop || eAnchor == e_Anchor.RightBottom) {
                    dx = -parts.width;
                }
                if (eAnchor == e_Anchor.Center || eAnchor == e_Anchor.LeftCenter || eAnchor == e_Anchor.RightCenter) {
                    dy = -parts.height * 0.5f;
                } else if (eAnchor == e_Anchor.Bottom || eAnchor == e_Anchor.LeftBottom || eAnchor == e_Anchor.RightBottom) {
                    dy = -parts.height;
                }
                for (int i = 0; i < aPart.Length; ++i) {
                    aPart[i].vert.x += dx;
                    aPart[i].vert.y += dy;
                    aPart[i].vert.z += dx;
                    aPart[i].vert.w += dy;
                }
            }
        }
        void setPart(ref Parts.Part part, Vector2 size) {
            if (size.x == 0f) {
                size.x = m_size.x * (part.uv.z - part.uv.x);
            } else if (size.x == forceZero) {
                size.x = 0f;
            }
            if (size.y == 0f) {
                size.y = m_size.y * (part.uv.w - part.uv.y);
            } else if (size.y == forceZero) {
                size.y = 0f;
            }
            part.vert.x = 0f;
            part.vert.z = size.x;
            part.vert.y = 0f;
            part.vert.w = size.y;
        }
        //==========================================================================
        /*!request
         * @brief	レンダリングを要求する.
        */
        public int request(Parts parts, Vector3 pos, WinColor color, int flag, ClipRect clip = null) {
            if (m_mesh == null || parts == null) {
                return -1;
            }
            Parts.Part[] aPart = parts.part;
            color = color * parts.color;
            if (color.a == 0) {
                return 0;
            }
            if (m_index + aPart.Length * 4 >= m_vertices.Length) {
                expand();
            }
            for (int i = 0; i < aPart.Length; ++i) {
                Vector4 vert = aPart[i].vert;
                vert.x += pos.x;
                vert.z += pos.x;
                vert.y += pos.y;
                vert.w += pos.y;
                request(vert, pos.z, aPart[i].uv, color, flag, clip);
            }
            return 0;
        }
        //==========================================================================
        /*!request
         * @brief	レンダリングを要求する.
        */
        public int request(Parts parts, Vector3 pos, Vector2 scale, WinColor color, int flag, ClipRect clip = null) {
            if (m_mesh == null || parts == null) {
                return -1;
            }
            Parts.Part[] aPart = parts.part;
            if (m_index + aPart.Length * 4 >= m_vertices.Length) {
                expand();
            }
            color = color * parts.color;
            for (int i = 0; i < aPart.Length; ++i) {
                Vector4 vert = aPart[i].vert;
                vert.x = vert.x * scale.x + pos.x;
                vert.y = vert.y * scale.y + pos.y;
                vert.z = vert.z * scale.x + pos.x;
                vert.w = vert.w * scale.y + pos.y;
                request(vert, pos.z, aPart[i].uv, color, flag, clip);
            }
            return 0;
        }
        //==========================================================================
        /*!内側かどうかチェックする.
         * @brief	checkClippedXXX
        */
        protected bool checkClippedLeft(Vector2 pos, float clip) {
            if (pos.x >= clip) {
                return true;
            }
            return false;
        }
        protected bool checkClippedRight(Vector2 pos, float clip) {
            if (pos.x <= clip) {
                return true;
            }
            return false;
        }
        protected bool checkClippedTop(Vector2 pos, float clip) {
            if (pos.y >= clip) {
                return true;
            }
            return false;
        }
        protected bool checkClippedBottom(Vector2 pos, float clip) {
            if (pos.y <= clip) {
                return true;
            }
            return false;
        }
        //==========================================================================
        /*!Y軸でクリッピングする.
         * @brief	intersectY
        */
        protected Vertex intersectY(Vertex vBegin, Vertex vEnd, float cx) {
            float k = (vEnd.pos.x - vBegin.pos.x);
            if (Mathf.Abs(k) < 0.00001f) {
                return vBegin;
            }
            k = (cx - vBegin.pos.x) / k;
            float cy = (vEnd.pos.y - vBegin.pos.y) * k + vBegin.pos.y;
            Vertex vResult;
            vResult.pos.x = cx;
            vResult.pos.y = cy;
            vResult.uv = (vEnd.uv - vBegin.uv) * k + vBegin.uv;
            return vResult;
        }
        //==========================================================================
        /*!X軸でクリッピングする.
         * @brief	intersectX
        */
        protected Vertex intersectX(Vertex vBegin, Vertex vEnd, float cy) {
            float k = (vEnd.pos.y - vBegin.pos.y);
            if (Mathf.Abs(k) < 0.00001f) {
                return vBegin;
            }
            k = (cy - vBegin.pos.y) / k;
            float cx = (vEnd.pos.x - vBegin.pos.x) * k + vBegin.pos.x;
            Vertex vResult;
            vResult.pos.x = cx;
            vResult.pos.y = cy;
            vResult.uv = (vEnd.uv - vBegin.uv) * k + vBegin.uv;
            return vResult;
        }
        //==========================================================================
        /*!クリップされる座標を計算する.
         * @brief	clipped
        */
        protected int clipping(Vertex[] aClipped, Vertex[] aVertex, int n, float clip, DelCheckIntersect delCheck, DelIntersect delIntersect) {
            int iIndex = 0;
            for (int i = 0; i < n; ++i) {
                int j = i + 1;
                if (j == n) j = 0;
                bool bInBegin = delCheck(aVertex[i].pos, clip);
                bool bInEnd = delCheck(aVertex[j].pos, clip);
                if (bInBegin) {
                    if (bInEnd) {
                        aClipped[iIndex++] = aVertex[j];    //inside,inside
                    } else {
                        aClipped[iIndex++] = delIntersect(aVertex[i], aVertex[j], clip);    //inside,outside
                    }
                } else {
                    if (bInEnd) {   // outside,inside
                        aClipped[iIndex++] = delIntersect(aVertex[i], aVertex[j], clip);
                        aClipped[iIndex++] = aVertex[j];
                    } else {    // outside,outside

                    }
                }
            }
            return iIndex;
        }
        //==========================================================================
        /*!自由な4点を指定してのクリッピング付レンダリング.
         * @brief	request
        */
        public int request(Vertex[] aVertex, float z, WinColor color, ClipRect clip) {
            if (m_mesh == null) {
                return -1;
            }
            int n = aVertex.Length;
            if (n != 4) {
                Debug.LogError("this vertex is not square");
                return -2;
            }
            bool bClipped = false;
            if (clip != null) {
                for (int i = 0; i < n; ++i) {
                    if (aVertex[i].pos.x < clip.xMin || aVertex[i].pos.x > clip.xMax) {
                        bClipped = true;
                        break;
                    }
                    if (aVertex[i].pos.y < clip.yMin || aVertex[i].pos.y > clip.yMax) {
                        bClipped = true;
                        break;
                    }
                }
            }
            if (!bClipped) {
                // 完全に中に入っている.
                if (m_index + 4 >= m_vertices.Length) {
                    expand();
                }
                m_vertices[m_index + 0] = aVertex[0].pos;
                m_vertices[m_index + 1] = aVertex[1].pos;
                m_vertices[m_index + 2] = aVertex[3].pos;
                m_vertices[m_index + 3] = aVertex[2].pos;

                m_uv[m_index + 0] = aVertex[0].uv;
                m_uv[m_index + 1] = aVertex[1].uv;
                m_uv[m_index + 2] = aVertex[3].uv;
                m_uv[m_index + 3] = aVertex[2].uv;

                m_colors[m_index + 0] = color;
                m_colors[m_index + 1] = color;
                m_colors[m_index + 2] = color;
                m_colors[m_index + 3] = color;
                m_index += 4;
            } else {
                Vertex[] aClippingA = new Vertex[n * 2 + 1];
                Vertex[] aClippingB = new Vertex[n * 2 + 1];
                // 左側.
                n = clipping(aClippingA, aVertex, n, clip.xMin, checkClippedLeft, intersectY);
                // 右側.
                n = clipping(aClippingB, aClippingA, n, clip.xMax, checkClippedRight, intersectY);
                // 上側.
                n = clipping(aClippingA, aClippingB, n, clip.yMin, checkClippedTop, intersectX);
                // 下側.
                n = clipping(aClippingB, aClippingA, n, clip.yMax, checkClippedBottom, intersectX);

                if (n < 3) {
                    // 外に完全に外れた.
                    return 0;
                }
                if (m_index + 4 * (n - 2) >= m_vertices.Length) {
                    expand();
                }
                // 縮退ポリゴンを使って、TriangleFanをエミュレートする.
                Vector3 vRoot = new Vector3(aClippingB[0].pos.x, aClippingB[0].pos.y, z);
                Vector2 uvRoot = aClippingB[0].uv;
                Vector3 vOld = new Vector3(aClippingB[1].pos.x, aClippingB[1].pos.y, z);
                Vector2 uvOld = aClippingB[1].uv;
                for (int i = 2; i < n; i++) {
                    Vector3 vPos = new Vector3(aClippingB[i].pos.x, aClippingB[i].pos.y, z);
                    Vector2 uv = aClippingB[i].uv;
                    m_vertices[m_index + 0] = vRoot;
                    m_vertices[m_index + 1] = vOld;
                    m_vertices[m_index + 2] = vPos;
                    m_vertices[m_index + 3] = vPos;

                    m_uv[m_index + 0] = uvRoot;
                    m_uv[m_index + 1] = uvOld;
                    m_uv[m_index + 2] = uv;
                    m_uv[m_index + 3] = uv;

                    m_colors[m_index + 0] = color;
                    m_colors[m_index + 1] = color;
                    m_colors[m_index + 2] = color;
                    m_colors[m_index + 3] = color;

                    vOld = vPos;
                    uvOld = uv;
                    m_index += 4;
                }
            }
            return 0;
        }
        protected int request(Vertex[] aVertex, Vector4 uv, Vector2 pos0, Vector2 pos1, Vector2 vNorm, float z, WinColor color, ClipRect clip = null) {
            Vector2 pos = pos0 + vNorm;
            aVertex[0].pos = new Vector3(pos.x, pos.y, z);
            aVertex[0].uv = new Vector2(uv.x, uv.y);
            pos = pos0 - vNorm;
            aVertex[1].pos = new Vector3(pos.x, pos.y, z);
            aVertex[1].uv = new Vector2(uv.x, uv.w);
            pos = pos1 - vNorm;
            aVertex[2].pos = new Vector3(pos.x, pos.y, z);
            aVertex[2].uv = new Vector2(uv.z, uv.w);
            pos = pos1 + vNorm;
            aVertex[3].pos = new Vector3(pos.x, pos.y, z);
            aVertex[3].uv = new Vector2(uv.z, uv.y);
            return request(aVertex, z, color, clip);
        }
        //==========================================================================
        /*!request
         * @brief	直線のレンダリングを要求する.
        */
        public int request(Parts parts, Vector3 posBegin, Vector3 posEnd, float width, WinColor color, ClipRect clip = null) {
            if (m_mesh == null || parts == null) {
                return -1;
            }
            color = color * parts.color;
            Vertex[] aVertex = new Vertex[4];
            Vector2 vDir = (posEnd - posBegin);
            float ln = vDir.magnitude;
            vDir = vDir.normalized;
            Vector2 vNorm;
            vNorm.x = vDir.y;
            vNorm.y = -vDir.x;
            if (width == 0f) {
                width = parts.height;
            }
            vNorm *= width * 0.5f;
            float z = posBegin.z;
            switch (parts.patch) {
            default:
                goto case e_Patch.None;
            case e_Patch.None:
                return request(aVertex, parts.uv, posBegin, posEnd, vNorm, z, color, clip);
            case e_Patch.H3: {
                float l0 = parts.part[0].vert.z;
                float l2 = parts.part[2].vert.z - parts.part[2].vert.x;
                float l1 = ln - (l0 + l2);
                Vector2 pos0 = posBegin;
                Vector2 pos1 = pos0 + l0 * vDir;
                Vector2 pos2 = pos1 + l1 * vDir;
                Vector2 pos3 = posEnd;
                request(aVertex, parts.part[0].uv, pos0, pos1, vNorm, z, color, clip);
                request(aVertex, parts.part[1].uv, pos1, pos2, vNorm, z, color, clip);
                request(aVertex, parts.part[2].uv, pos2, pos3, vNorm, z, color, clip);
            }
            return 0;
            }
        }
        //==========================================================================
        /*!request
         * @brief	回転/スケール付きレンダリングを要求する.
        */
        public int request(Parts parts, Vector3 pos, Vector2 scale, Vector2 offset, int angle, WinColor color, ClipRect clip = null) {
            Parts.Part[] aPart = parts.part;

            Vector2 sc = Angle.SinCos(angle);

            Vertex[] aVertex = new Vertex[4];

            WinColor col = color * parts.color;
            for (int i = 0; i < aPart.Length; ++i) {
                Vector4 uv = aPart[i].uv;
                Vector4 vert = aPart[i].vert;
                vert.x = vert.x * scale.x + offset.x;
                vert.y = vert.y * scale.y + offset.y;
                vert.z = vert.z * scale.x + offset.x;
                vert.w = vert.w * scale.y + offset.y;
                if (angle == 0) {
                    vert.x += pos.x;
                    vert.y += pos.y;
                    vert.z += pos.x;
                    vert.w += pos.y;
                    request(vert, pos.z, uv, col, clip);
                } else {
                    aVertex[0].pos.x = sc.y * vert.x - sc.x * vert.y + pos.x;
                    aVertex[0].pos.y = sc.x * vert.x + sc.y * vert.y + pos.y;
                    aVertex[0].uv = new Vector2(uv.x, uv.y);

                    aVertex[1].pos.x = sc.y * vert.x - sc.x * vert.w + pos.x;
                    aVertex[1].pos.y = sc.x * vert.x + sc.y * vert.w + pos.y;
                    aVertex[1].uv = new Vector2(uv.x, uv.w);

                    aVertex[2].pos.x = sc.y * vert.z - sc.x * vert.w + pos.x;
                    aVertex[2].pos.y = sc.x * vert.z + sc.y * vert.w + pos.y;
                    aVertex[2].uv = new Vector2(uv.z, uv.w);

                    aVertex[3].pos.x = sc.y * vert.z - sc.x * vert.y + pos.x;
                    aVertex[3].pos.y = sc.x * vert.z + sc.y * vert.y + pos.y;
                    aVertex[3].uv = new Vector2(uv.z, uv.y);

                    request(aVertex, pos.z, col, clip);
                }
            }
            return 0;
        }
        //==========================================================================
        /*!request
         * @brief	レンダリングを要求する.
        */
        public int request(Vector3 pos, WinColor color, Vector4 uv, Vector2 size, int flag, ClipRect clip = null) {
            if (m_mesh == null) {
                return -1;
            }
            if (m_index + 4 >= m_vertices.Length) {
                expand();
            }
            Vector4 vert;
            vert.x = pos.x;
            vert.y = pos.y;
            vert.z = pos.x + size.x;
            vert.w = pos.y + size.y;
            if ((flag & Flag_UVReverseH) != 0) {
                float u = uv.x;
                uv.x = uv.z;
                uv.z = u;
            }
            if ((flag & Flag_UVReverseV) != 0) {
                float v = uv.y;
                uv.y = uv.w;
                uv.w = v;
            }
            switch (flag & Flag_UVRotateMask) {
            case Flag_UVRotate0:
                return request(vert, pos.z, uv, color, clip);
            case Flag_UVRotate90:
                return requestR(vert, pos.z, uv, color, clip);
            case Flag_UVRotate180: {
                float u = uv.x;
                uv.x = uv.z;
                uv.z = u;
                float v = uv.y;
                uv.y = uv.w;
                uv.w = v;
            }
            return request(vert, pos.z, uv, color, clip);
            case Flag_UVRotate270: {
                float u = uv.x;
                uv.x = uv.z;
                uv.z = u;
                float v = uv.y;
                uv.y = uv.w;
                uv.w = v;
            }
            return requestR(vert, pos.z, uv, color, clip);
            }
            return 0;
        }
        //==========================================================================
        /*!request
         * @brief	レンダリングを要求する.
        */
        public int request(Vector4 vert, float z, Vector4 uv, WinColor color, int flag, ClipRect clip = null) {
            if (m_mesh == null) {
                return -1;
            }
            if (m_index + 4 >= m_vertices.Length) {
                expand();
            }
            if ((flag & Flag_UVReverseH) != 0) {
                float u = uv.x;
                uv.x = uv.z;
                uv.z = u;
            }
            if ((flag & Flag_UVReverseV) != 0) {
                float v = uv.y;
                uv.y = uv.w;
                uv.w = v;
            }
            switch (flag & Flag_UVRotateMask) {
            case Flag_UVRotate0:
                return request(vert, z, uv, color, clip);
            case Flag_UVRotate90:
                return requestR(vert, z, uv, color, clip);
            case Flag_UVRotate180: {
                float u = uv.x;
                uv.x = uv.z;
                uv.z = u;
                float v = uv.y;
                uv.y = uv.w;
                uv.w = v;
            }
            return request(vert, z, uv, color, clip);
            case Flag_UVRotate270: {
                float u = uv.x;
                uv.x = uv.z;
                uv.z = u;
                float v = uv.y;
                uv.y = uv.w;
                uv.w = v;
            }
            return requestR(vert, z, uv, color, clip);
            }
            return 0;
        }
        //==========================================================================
        /*!request
         * @brief	レンダリングを要求する.
        */
        protected int request(Vector4 vert, float z, Vector4 uv, Color32 color, ClipRect clip = null) {
            Vector4 clipeduv = uv;
            Vector4 clipedvert = vert;
            if (clip != null) {
                float w = clipedvert.z - clipedvert.x;
                float uvw = clipeduv.z - clipeduv.x;
                if (clip.xMin > clipedvert.x) {
                    // 左辺クリップ.
                    clipeduv.x = uv.x + uvw * (clip.xMin - vert.x) / w;
                    clipedvert.x = clip.xMin;
                }
                if (clip.xMax < clipedvert.z) {
                    // 右辺クリップ.
                    clipeduv.z = uv.x + uvw * (clip.xMax - vert.x) / w;
                    clipedvert.z = clip.xMax;
                }
                // クリッピングから外れた.
                if (clipedvert.x >= clipedvert.z) {
                    return -1;
                }
                float h = clipedvert.w - clipedvert.y;
                float uvh = clipeduv.w - clipeduv.y;
                if (clip.yMin > clipedvert.y) {
                    // 下辺クリップ.
                    clipeduv.w = uv.w - uvh * (clip.yMin - vert.y) / h;
                    clipedvert.y = clip.yMin;
                }
                if (clip.yMax < clipedvert.w) {
                    // 上辺クリップ.
                    clipeduv.y = uv.w - uvh * (clip.yMax - vert.y) / h;
                    clipedvert.w = clip.yMax;
                }
                // クリッピングから外れた.
                if (clipedvert.y >= clipedvert.w) {
                    return -1;
                }
            }
            int i = m_index;
            m_vertices[i + 0] = new Vector3(clipedvert.x, clipedvert.y, z);
            m_vertices[i + 1] = new Vector3(clipedvert.z, clipedvert.y, z);
            m_vertices[i + 2] = new Vector3(clipedvert.x, clipedvert.w, z);
            m_vertices[i + 3] = new Vector3(clipedvert.z, clipedvert.w, z);
            // UV.
            m_uv[i + 0].x = clipeduv.x;
            m_uv[i + 0].y = clipeduv.w;
            m_uv[i + 1].x = clipeduv.z;
            m_uv[i + 1].y = clipeduv.w;
            m_uv[i + 2].x = clipeduv.x;
            m_uv[i + 2].y = clipeduv.y;
            m_uv[i + 3].x = clipeduv.z;
            m_uv[i + 3].y = clipeduv.y;

            // カラー.
            m_colors[i + 0] = color;
            m_colors[i + 1] = color;
            m_colors[i + 2] = color;
            m_colors[i + 3] = color;

            m_index += 4;
            return i;
        }
        //==========================================================================
        /*!requestR
         * @brief	レンダリングを要求する(UVを90度回転させたもの).
        */
        protected int requestR(Vector4 vert, float z, Vector4 uv, Color32 color, ClipRect clip = null) {
            Vector4 clipeduv = uv;
            Vector4 clipedvert = vert;
            if (clip != null) {
                float w = clipedvert.z - clipedvert.x;
                float uvw = clipeduv.w - clipeduv.y;
                if (clip.xMin > clipedvert.x) {
                    // 左辺クリップ.
                    clipeduv.y = uv.x + uvw * (clip.xMin - vert.x) / w;
                    clipedvert.x = clip.xMin;
                }
                if (clip.xMax < clipedvert.z) {
                    // 右辺クリップ.
                    clipeduv.w = uv.y + uvw * (clip.xMax - vert.x) / w;
                    clipedvert.z = clip.xMax;
                }
                // クリッピングから外れた.
                if (clipedvert.x >= clipedvert.z) {
                    return -1;
                }
                float h = clipedvert.w - clipedvert.y;
                float uvh = clipeduv.z - clipeduv.x;
                if (clip.yMin > clipedvert.y) {
                    // 下辺クリップ.
                    clipeduv.x = uv.x + uvh * (clip.yMin - vert.y) / h;
                    clipedvert.y = clip.yMin;
                }
                if (clip.yMax < clipedvert.w) {
                    // 上辺クリップ.
                    clipeduv.z = uv.z + uvh * (clip.yMax - vert.y) / h;
                    clipedvert.w = clip.yMax;
                }
                // クリッピングから外れた.
                if (clipedvert.y >= clipedvert.w) {
                    return -1;
                }
            }
            int i = m_index;
            m_vertices[i + 0] = new Vector3(clipedvert.x, clipedvert.y, z);
            m_vertices[i + 1] = new Vector3(clipedvert.z, clipedvert.y, z);
            m_vertices[i + 2] = new Vector3(clipedvert.x, clipedvert.w, z);
            m_vertices[i + 3] = new Vector3(clipedvert.z, clipedvert.w, z);
            // UV.
            m_uv[i + 0].x = clipeduv.z;
            m_uv[i + 0].y = clipeduv.w;
            m_uv[i + 1].x = clipeduv.z;
            m_uv[i + 1].y = clipeduv.y;
            m_uv[i + 2].x = clipeduv.x;
            m_uv[i + 2].y = clipeduv.w;
            m_uv[i + 3].x = clipeduv.x;
            m_uv[i + 3].y = clipeduv.y;

            // カラー.
            m_colors[i + 0] = color;
            m_colors[i + 1] = color;
            m_colors[i + 2] = color;
            m_colors[i + 3] = color;

            m_index += 4;
            return i;
        }
        public uint id {
            get {
                return m_id;
            }
        }
    }
}
