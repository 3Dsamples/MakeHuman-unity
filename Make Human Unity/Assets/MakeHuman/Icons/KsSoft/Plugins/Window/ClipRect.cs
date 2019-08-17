//==============================================================================================
/*!クリップ用矩形.
	@file  CClipRect
	
	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KS {
    public class ClipRect {
        public enum State {
            Inside,     //クリップの必要なしにレンダリング.
            Clipped,    //クリッピングが必要.
            Outside,    //完全に外に出ている.
        };
        public ClipRect() {
            m_x = 0;
            m_y = 0;
            m_width = 0;
            m_height = 0;
        }
        public ClipRect(float x, float y, float width, float height) {
            m_x = x;
            m_y = y - height;
            m_width = width;
            m_height = height;
        }
        public ClipRect(ClipRect cr) {
            m_x = cr.m_x;
            m_y = cr.m_y - height;
            m_width = cr.m_width;
            m_height = cr.m_height;
        }
        protected float m_x;
        protected float m_y;
        protected float m_width;
        protected float m_height;

        public float x {
            get {
                return m_x;
            }
            set {
                m_x = value;
            }
        }
        public float y {
            get {
                return m_y;
            }
            set {
                m_y = value;
            }
        }
        public float ex {
            get {
                return m_x + m_width;
            }
            set {
                width = value - m_x;
            }
        }
        public float ey {
            get {
                return m_y + m_height;
            }
            set {
                m_height = value - m_y;
            }
        }
        public float width {
            get {
                return m_width;
            }
            set {
                m_width = value;
            }
        }
        public float height {
            get {
                return m_height;
            }
            set {
                m_height = value;
            }
        }
        public float xMin {
            get {
                return m_x;
            }
        }
        public float xMax {
            get {
                return m_x + m_width;
            }
        }
        public float yMin {
            get {
                return m_y;
            }
        }
        public float yMax {
            get {
                return m_y + m_height;
            }
        }
        public State check(Vector4 vert) {
            if (xMax < vert.x || xMin > vert.z || yMax < vert.y || yMin > vert.w) {
                return State.Outside;
            }
            // 完全に内側にあるかチェック.
            if (xMin <= vert.x && vert.z <= xMax && yMin <= vert.y && vert.w <= yMax) {
                return State.Inside;
            }
            // クリッピングに引っかかっている.
            return State.Clipped;
        }
        public State check(Vector3 pos, Vector2 size) {
            Vector4 vert;
            vert.x = pos.x;
            vert.y = pos.y;
            vert.z = pos.x + size.x;
            vert.w = pos.y + size.y;
            return check(vert);
        }
        public bool check(Vector2 vert) {
            if (xMax < vert.x || xMin > vert.x || yMax < vert.y || yMin > vert.y) {
                return false;
            }
            return true;
        }
    };
}
