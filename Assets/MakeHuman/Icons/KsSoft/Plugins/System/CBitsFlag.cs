//==============================================================================================
/*!ビットフラグ.
	@file  CBitsFlag	
*/
//==============================================================================================
using UnityEngine;

namespace KS {
    public class CBitsFlag {
        int[] m_flags;
        int m_length;
        public CBitsFlag(int[] flags) {
            m_length = flags.Length * 32;
        }
        public CBitsFlag(int length) {
            int l = (length + 31) >> 5;
            m_length = length;
            m_flags = new int[l];
        }
        public bool this[int index] {
            get {
                if (index < 0 || index >= m_length) {
                    Debug.LogError("index error:" + index + "(" + m_length + ")");
                    return false;
                }
                if ((m_flags[index >> 5] & (1 << (index & 31))) != 0) {
                    return true;
                }
                return false;
            }
            set {
                if (index < 0 || index >= m_length) {
                    Debug.LogError("index error:" + index + "(" + m_length + ")");
                    return;
                }
                int bits = 1 << (index & 31);
                int idx = index >> 5;
                if (value) {
                    m_flags[idx] |= bits;
                } else {
                    m_flags[idx] &= ~bits;
                }
            }
        }
        public int[] flags {
            get {
                return m_flags;
            }
        }
        public int Length {
            get {
                return m_length;
            }
        }
    };
}
