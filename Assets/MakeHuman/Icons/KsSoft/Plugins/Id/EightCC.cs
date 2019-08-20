//==============================================================================================
/*!EightCC
	@file  EightCC
	
*/
//==============================================================================================
using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections;
namespace KS {
    //==========================================================================
    /*!
        @brief	class CEightCC
        @note	8文字ID
    */
    public struct EightCC {
        public ulong m_value;
        public EightCC(ulong val) {
            m_value = val;
        }
        public EightCC(string sCC) {
            m_value = Id(sCC);
        }
        override public string ToString() {
            string sResult = "";
            for (int i = 0; i < 8; i++) {
                char c = (char)((m_value >> (i * 8)) & 0xff);
                if (c == 0) {
                    break;
                }
                sResult += c;
            }
            return sResult;
        }
        static public ulong Id(string sCC) {
            ulong val = 0;

            if (sCC.Length > 8) {
                Debug.Log("can't convert EightCC:" + sCC);
                return 0;
            }
            for (int i = 0; i < sCC.Length; i++) {
                if (sCC[i] > 128) {
                    Debug.Log("can't use character code for  EightCC:" + sCC);
                    return 0;
                }
            }
            for (int i = 0; i < sCC.Length; i++) {
                val |= (ulong)((ulong)sCC[i] << (i * 8));
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
                m_value = (m_value & (ulong)~((ulong)0xff << index)) | (ulong)((value & (ulong)0xff) << index);
            }
        }
        static public implicit operator ulong(EightCC eightCC) {
            return eightCC.m_value;
        }
    }
}
