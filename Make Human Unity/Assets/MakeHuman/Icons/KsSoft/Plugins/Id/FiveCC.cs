//==============================================================================================
/*!FiveCC
	@file  FiveCC
	
*/
//==============================================================================================
using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections;
namespace KS {
    //==========================================================================
    /*!
        @brief	class CFiveCC
        @note	4文字ID.
    */
    public struct FiveCC {
        public uint m_value;
        const string _idchar = " 0123456789_abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        static uint[] _idtable = new uint[128];

        static FiveCC() {
            if (_idchar.Length != 64) {
                Debug.LogError("id char length is illegal:" + _idchar.Length);
            }
            for (int i = 0; i < _idtable.Length; ++i) {
                _idtable[i] = 0xffff;
            }
            for (int i = 1; i < _idchar.Length; ++i) {
                _idtable[(int)_idchar[i]] = (uint)i;
            }
            _idtable[(int)'*'] = 11;
            _idtable[(int)'?'] = 11;
            _idtable[0] = 0;
        }
        public FiveCC(uint val) {
            m_value = val;
        }
        public FiveCC(string sCC) {
            m_value = Id(sCC);
        }
        override public string ToString() {
            return ToString(m_value);
        }
        static public uint Id(string sCC) {
            uint val = 0;

            if (!isFiveCC(sCC)) {
                Debug.Log("can't convert FiveCC:" + sCC);
                return 0;
            }
            for (int i = 0; i < sCC.Length; i++) {
                int ch = sCC[i];
                val |= _idtable[ch] << (i * 6);
            }
            return val;
        }
        static public string ToString(uint val) {
            string sResult = "";
            for (int i = 0; i < 5; i++) {
                uint id = (val >> (i * 6)) & 63;
                if (id == 0) {
                    break;
                }
                sResult += _idchar[(int)id];
            }
            return sResult;
        }
        public char this[int index] {
            get {
                index *= 6;
                uint id = (m_value >> index) & 63;
                return _idchar[(int)id];
            }
            set {
                if (value > 128 || _idtable[value] == 0xffff) {
                    Debug.Log("can't use character code for  FiveCC:" + value);
                    return;
                }
                index *= 6;
                m_value = (m_value & (uint)~(63 << index)) | (_idtable[value] << index);
            }
        }
        static public implicit operator uint(FiveCC FiveCC) {
            return FiveCC.m_value;
        }
        static public bool isFiveCC(string sId) {
            if (sId.Length > 5) {
                return false;
            }
            for (int i = 0; i < sId.Length; i++) {
                uint ch = sId[i];
                if (ch > 128 || _idtable[ch] == 0xffff) {
                    Debug.Log("can't use character code for  FiveCC:" + sId[i]);
                    return false;
                }
            }
            return true;
        }
    }
}
