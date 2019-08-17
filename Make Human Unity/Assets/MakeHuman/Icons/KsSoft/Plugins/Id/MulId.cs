//==============================================================================================
/*!MulId
	@file  MulId

*/
//==============================================================================================
using UnityEngine;
using System;
using System.Collections;
namespace KS {
    //==========================================================================
    /*!
        @brief	class CMulId
        @note	int型をマルチIDとして操作するための関数をパック、主にこちらを使用.
        @ref	CMultiId
    */
    public struct MulId {
        uint m_value;
        static public MulId zero = new MulId(0);
        public MulId(string sId) {
            m_value = 0;
            set(sId);
        }
        public MulId(uint upper, uint middle, uint lower) {
            m_value = 0;
            set(upper, middle, lower);
        }
        public MulId(uint hi, uint low) {
            m_value = 0;
            set(hi, low);
        }
        public MulId(uint val) {
            m_value = val;
        }
        //==========================================================================
        /*!文字列をMulIdに変換.
            @brief	Normalize
        */
        public static MulId Normalize(string sId) {
            int index = sId.LastIndexOf('/');
            if (index > 0) {
                sId = sId.Substring(index + 1);
            }
            index = sId.LastIndexOf('\\');
            if (index > 0) {
                sId = sId.Substring(index + 1);
            }

            string[] aSep = sId.Split(new Char[] { '_', '.', '(', '@' });
            if (aSep.Length < 3) {
                Debug.Log("Multi id format error:" + sId);
                return zero;
            }
            return get(aSep);
        }
        //==========================================================================
        /*!Id
            @brief	Int形式に変換.
        */
        public void set(uint val) {
            m_value = val;
        }
        public void set(string sId) {
            m_value = Id(sId);
        }
        public void set(uint upper, uint middle, uint lower) {
            if (upper < 0 || upper > 255 ||
                middle < 0 || middle > 255 ||
                lower < 0 || lower > 65535) {
                Debug.Log("Multi id format error:" + upper + "_" + middle + "_" + lower);
                return;
            }
            m_value = Id(upper, middle, lower);
        }
        public void set(uint hi, uint low) {
            if (hi < 0 || hi > 65535 ||
                low < 0 || low > 65535) {
                Debug.Log("Multi id format error:" + hi + "_" + low);
                return;
            }
            m_value = Id(hi, low);
        }

        static public uint Id(string sId) {
            string[] aSep = sId.Split('_');
            if (aSep.Length != 3) {
                Debug.Log("Multi id format error:" + sId);
                return 0;
            }
            return get(aSep);
        }
        static public uint Id(uint upper, uint middle, uint lower) {
            return (uint)((upper << 24) | (middle << 16) | (lower));
        }
        static public uint Id(uint hi, uint low) {
            return (uint)((hi << 16) | (low));
        }
        static public string ToString(uint mId) {
            return new MulId(mId).ToString();
        }
        static MulId get(string[] aSep) {
            uint upper;
            if (!uint.TryParse(aSep[0], out upper)) {
                Debug.LogError("multi id format error:" + aSep[0]);
                return zero;
            }
            uint middle;
            if (!uint.TryParse(aSep[1], out middle)) {
                Debug.LogError("multi id format error:" + aSep[1]);
                return zero;
            }
            uint lower;
            if (!uint.TryParse(aSep[2], out lower)) {
                Debug.LogError("multi id format error:" + aSep[2]);
                return zero;
            }
            return new MulId(upper, middle, lower);
        }
        //==========================================================================
        /*!IdのUpper取得.
            @brief	Multi IdのUpperを取得.
        */
        public uint Upper {
            get {
                return (m_value >> 24) & 0xff;
            }
            set {
                m_value = (m_value & 0xffffff) | ((value & 0xff) << 24);
            }
        }
        static public uint upper(uint mId) {
            return (mId >> 24) & 0xFF;
        }
        //==========================================================================
        /*!IdのMiddleを取得.
            @brief	Multi IdのMiddleを取得.
        */
        public uint Middle {
            get {
                return (m_value >> 16) & 0xff;
            }
            set {
                m_value = ((uint)m_value & (uint)0xff00ffff) | (uint)((value & 0xff) << 16);
            }
        }
        static public uint middle(uint mId) {
            return (mId >> 16) & 0xFF;
        }
        //==========================================================================
        /*!IdのLowerを取得.
            @brief	Multi IdのLowerを取得.
        */
        public uint Lower {
            get {
                return m_value & 0xffff;
            }
            set {
                m_value = ((uint)m_value & (uint)0xffff0000) | (uint)(value & 0xffff);
            }
        }
        static public uint lower(uint mId) {
            return mId & 0xFFFF;
        }
        //==========================================================================
        /*!ToString
            @brief	Multi Id文字列に変換.
        */
        override public string ToString() {
            return ((m_value >> 24) & 0xff).ToString("000_") +
                    ((m_value >> 16) & 0xff).ToString("000_") +
                    (m_value & 0xffff).ToString("00000");
        }
        static public implicit operator uint(MulId mulId) {
            return mulId.m_value;
        }
        static public bool isMulId(string sId) {
            string[] aSep = sId.Split(new char[] { '_', '.' });
            if (aSep.Length != 3) {
                return false;
            }
            uint upper;
            if (!uint.TryParse(aSep[0], out upper)) {
                return false;
            }
            if (upper > 255) {
                return false;
            }
            uint middle;
            if (!uint.TryParse(aSep[1], out middle)) {
                Debug.LogError("multi id format error:" + sId);
                return false;
            }
            if (middle > 255) {
                return false;
            }
            uint lower;
            if (!uint.TryParse(aSep[2], out lower)) {
                Debug.LogError("multi id format error:" + sId);
                return false;
            }
            if (middle > 65535) {
                return false;
            }
            return true;
        }
    }
}
