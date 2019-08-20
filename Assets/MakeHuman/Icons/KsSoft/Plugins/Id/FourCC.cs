//==============================================================================================
/*!FourCC
	@file  FourCC
	
*/
//==============================================================================================
using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections;
namespace KS {
    //==========================================================================
    /*!
        @brief	class FourCC
        @note	4文字ID
    */
    public struct FourCC {
        public uint m_value;
        public FourCC(uint val) {
            m_value = val;
        }
        public FourCC(string sCC) {
            m_value = Id(sCC);
        }
        override public string ToString() {
            string sResult = "";
            for (int i = 0; i < 4; i++) {
                char c = (char)((m_value >> (i * 8)) & 0xff);
                if (c == 0) {
                    break;
                }
                sResult += c;
            }
            return sResult;
        }
        static public uint Id(string sCC) {
            uint val = 0;

            if (sCC.Length > 4) {
                Debug.Log("can't convert FourCC:" + sCC);
                return 0;
            }
            for (int i = 0; i < sCC.Length; i++) {
                if (sCC[i] > 128) {
                    Debug.Log("can't use character code for  FourCC:" + sCC);
                    return 0;
                }
            }
            for (int i = 0; i < sCC.Length; i++) {
                val |= (uint)(sCC[i] << (i * 8));
            }
            return val;
        }
        public char this[int index] {
            get {
                index *= 8;
                return (char)((m_value >> index) & 0xff);
            }
            set {
                index *= 8;
                m_value = (m_value & (uint)~(0xff << index)) | (uint)((value & 0xff) << index);
            }
        }
        static public implicit operator uint(FourCC fourCC) {
            return fourCC.m_value;
        }
    }
}
