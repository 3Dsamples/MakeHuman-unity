//==============================================================================================
/*!ウィンドウ用カラー.
	@file  WinColor
	@note	127が白として定義(255は、色が２倍になっている.)
*/
//==============================================================================================
using UnityEngine;

namespace KS {
    public struct WinColor {
        public byte a;
        public byte b;
        public byte g;
        public byte r;

        public enum e_Id {
            white,
            aqua,
            purple,
            blue,
            yellow,
            green,
            red,
            pink,
            orange,
            skyblue,
            black,
            num
        };
        // カラー.
        public static WinColor white = new WinColor(127, 127, 127, 255);
        public static WinColor aqua = new WinColor(64, 127, 127, 255);
        public static WinColor purple = new WinColor(100, 60, 127, 255);
        public static WinColor blue = new WinColor(64, 64, 127, 255);
        public static WinColor yellow = new WinColor(127, 128, 35, 255);
        public static WinColor green = new WinColor(75, 120, 30, 255);
        public static WinColor red = new WinColor(127, 40, 40, 255);
        public static WinColor pink = new WinColor(127, 80, 80, 255);
        public static WinColor orange = new WinColor(127, 70, 20, 255);
        public static WinColor skyblue = new WinColor(80, 100, 127, 255);
        public static WinColor black = new WinColor(32, 32, 32, 255);

        static WinColor[] m_aColor = new WinColor[(int)e_Id.num] {
        white,
        aqua,
        purple,
        blue,
        yellow,
        green,
        red,
        pink,
        orange,
        skyblue,
        black,
    };

        public WinColor(byte r, byte g, byte b, byte a) {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }
        public WinColor(uint uColor) {
            r = (byte)((uColor >> 16) & 0xff);
            g = (byte)((uColor >> 8) & 0xff);
            b = (byte)((uColor) & 0xff);
            a = (byte)((uColor >> 24) & 0xff);
        }
        public static WinColor operator *(WinColor a, WinColor b) {
            a.r = (byte)((a.r * b.r) >> 7);
            a.g = (byte)((a.g * b.g) >> 7);
            a.b = (byte)((a.b * b.b) >> 7);
            a.a = (byte)((a.a * b.a) >> 8);
            return a;
        }
        public static WinColor operator *(WinColor color, float mag) {
            uint k = (uint)(mag * 128f);
            color.r = (byte)Mathf.Clamp((k * color.r) >> 7, 0, 255);
            color.g = (byte)Mathf.Clamp((k * color.g) >> 7, 0, 255);
            color.b = (byte)Mathf.Clamp((k * color.b) >> 7, 0, 255);
            return color;
        }
        public static implicit operator Color32(WinColor c) {
            Color32 c32 = new Color32(c.r, c.g, c.b, c.a);
            return c32;
        }
        public static implicit operator Color(WinColor col) {
            Color c;
            c.r = col.r * (1f / 128f);
            c.g = col.g * (1f / 128f);
            c.b = col.b * (1f / 128f);
            c.a = col.a * (1f / 256f);
            return c;
        }
        public static implicit operator uint(WinColor col) {
            return (uint)(col.a << 24 | col.r << 16 | col.g << 8 | col.b);
        }
        public static WinColor Lerp(WinColor s, WinColor e, float k) {
            int r = s.r;
            s.r = (byte)Mathf.Clamp(r + ((int)e.r - r) * k, 0, 255);
            int g = s.g;
            s.g = (byte)Mathf.Clamp(g + ((int)e.g - g) * k, 0, 255);
            int b = s.b;
            s.b = (byte)Mathf.Clamp(b + ((int)e.b - b) * k, 0, 255);
            int a = s.a;
            s.a = (byte)Mathf.Clamp(a + ((int)e.a - a) * k, 0, 255);
            return s;
        }
        //==========================================================================
        /*!カラーを取得する.
            @brief	getColor
        */
        public static void registColor(int idx, WinColor color) {
            if (m_aColor.Length > idx) {
                m_aColor[idx] = color;
                return;
            }
            System.Array.Resize(ref m_aColor, idx + 1);
            m_aColor[idx] = color;
        }
        //==========================================================================
        /*!カラーを取得する.
            @brief	getColor
        */
        static public WinColor getColor(e_Id eColor) {
            int iIndex = (int)eColor;
            if (eColor >= e_Id.num) {
                iIndex = 0;
            }
            return m_aColor[iIndex];
        }
        //==========================================================================
        /*!カラーを取得する.
            @brief	getColor
        */
        static public WinColor getColor(int index) {
            if (index >= m_aColor.Length) {
                index = 0;
            }
            return m_aColor[index];
        }
    };
}
