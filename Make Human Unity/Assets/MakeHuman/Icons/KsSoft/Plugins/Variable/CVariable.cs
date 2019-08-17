//==============================================================================================
/*!CVariable
	@file  CVariable
*/
//==============================================================================================
//#define	SHORT_BUFFER_WARNING

using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace KS {
    //==========================================================================
    /*!CVarialbe
        @brief	class CVarialbe
    */
    public class CVariable {
        static protected Encoding m_cUtf8Encode = Encoding.GetEncoding("utf-8");
        static protected CCryptDES m_cryptDES = null;

        //==========================================================================
        /*!暗号化キーを設定する.
            @brief	crypt
        */
        static public bool crypt(ulong uIV, ulong uKey) {
            m_cryptDES = new CCryptDES();
            if (m_cryptDES.initialize(uIV, uKey)) {
                return true;
            }
            m_cryptDES = null;
            return false;
        }
        //==========================================================================
        /*!UTF8として、n文字が占める文字列のバイト数を計算する.
            @brief	count
        */
        static public int count(byte[] aBuffer, int begin, int n) {
            int size = 0;
            int i;
            for (i = begin; i < aBuffer.Length;) {
                if (i >= aBuffer.Length) {
                    return 0;
                }
                byte ch = aBuffer[i++];
                //0xxx xxxx	1バイト文字を意味するビットパターン.
                //110x xxxx	2バイト文字の先頭バイトを意味するビットパターン.
                //1110 xxxx 3バイト文字の先頭バイトを意味するビットパターン.
                //10xx xxxx 2バイト以上の文字中の、継続バイト(第2バイト以降)を意味するビットパターン.
                if ((0x80 & ch) == 0) {
                    size++;
                } else if ((0xe0 & ch) == 0xc0) {
                    for (int j = 1; j < 2; ++j) {
                        if (i >= aBuffer.Length) {
                            return -1;
                        }
                        ch = aBuffer[i++];
                        if (ch == 0 || (ch & 0xc0) != 0x80) {
                            return -1;
                        }
                    }
                    size++;
                } else if ((0xf0 & ch) == 0xe0) {
                    for (int j = 1; j < 3; ++j) {
                        if (i >= aBuffer.Length) {
                            return -2;
                        }
                        ch = aBuffer[i++];
                        if (ch == 0 || (ch & 0xc0) != 0x80) {
                            return -2;
                        }
                    }
                    size++;
                } else if ((0xf8 & ch) == 0xf0) {
                    for (int j = 1; j < 4; ++j) {
                        if (i >= aBuffer.Length) {
                            return -3;
                        }
                        ch = aBuffer[i++];
                        if (ch == 0 || (ch & 0xc0) != 0x80) {
                            return -3;
                        }
                    }
                    size++;
                } else {
                    Debug.LogError("char code error:" + (int)ch);
                    return -4;
                }
                if (size == n) {
                    return i - begin;
                }
            }
            return 0;
        }

        static public Encoding utf8Encode {
            get {
                return m_cUtf8Encode;
            }
        }
    };

    //==========================================================================
    /*!CReadVariable
        @brief	class CReadVariable
    */
    public class CReadVariable : CVariable {
        public CReadVariable(byte[] aBuffer, int index = 0) {
            m_aBuffer = aBuffer;
            m_iIndex = index;
        }
        public byte[] buffer() {
            return m_aBuffer;
        }
        public void setBuffer(byte[] buffer, int idx = 0) {
            m_aBuffer = buffer;
            m_iIndex = idx;
        }
        public void clear() {
            m_iIndex = 0;
        }
        public int size() {
            return m_aBuffer.Length;
        }
        public int index {
            get {
                return m_iIndex;
            }
            set {
                m_iIndex = value;
            }
        }
        public void error(string sError) {
            Debug.LogError(sError);
        }
        public bool getBool() {
            if (m_iIndex + 1 > m_aBuffer.Length) {
                throw new IndexOutOfRangeException();
            }
            return (m_aBuffer[m_iIndex++] != 0) ? true : false;
        }
        public sbyte getS8() {
            if (m_iIndex + 1 > m_aBuffer.Length) {
                throw new IndexOutOfRangeException();
            }
            return (sbyte)m_aBuffer[m_iIndex++];
        }
        public byte getU8() {
            if (m_iIndex + 1 > m_aBuffer.Length) {
                throw new IndexOutOfRangeException();
            }
            return m_aBuffer[m_iIndex++];
        }
        public short getS16() {
            if (m_iIndex + 2 > m_aBuffer.Length) {
                throw new IndexOutOfRangeException();
            }
            short iResult = BitConverter.ToInt16(m_aBuffer, m_iIndex);
            m_iIndex += 2;
            return iResult;
        }
        public ushort getU16() {
            if (m_iIndex + 2 > m_aBuffer.Length) {
                throw new IndexOutOfRangeException();
            }
            ushort uResult = BitConverter.ToUInt16(m_aBuffer, m_iIndex);
            m_iIndex += 2;
            return uResult;
        }
        public int getS32() {
            if (m_iIndex + 4 > m_aBuffer.Length) {
                throw new IndexOutOfRangeException();
            }
            int iResult = BitConverter.ToInt32(m_aBuffer, m_iIndex);
            m_iIndex += 4;
            return iResult;
        }
        public uint getU32() {
            if (m_iIndex + 4 > m_aBuffer.Length) {
                throw new IndexOutOfRangeException();
            }
            uint uResult = BitConverter.ToUInt32(m_aBuffer, m_iIndex);
            m_iIndex += 4;
            return uResult;
        }
        public float getFloat() {
            if (m_iIndex + 4 > m_aBuffer.Length) {
                throw new IndexOutOfRangeException();
            }
            float fResult = BitConverter.ToSingle(m_aBuffer, m_iIndex);
            m_iIndex += 4;
            return fResult;
        }
        public Vector2 getVector2() {
            if (m_iIndex + 8 > m_aBuffer.Length) {
                throw new IndexOutOfRangeException();
            }
            Vector2 vec2;
            vec2.x = BitConverter.ToSingle(m_aBuffer, m_iIndex);
            m_iIndex += 4;
            vec2.y = BitConverter.ToSingle(m_aBuffer, m_iIndex);
            m_iIndex += 4;
            return vec2;
        }
        public Vector3 getVector3() {
            if (m_iIndex + 12 > m_aBuffer.Length) {
                throw new IndexOutOfRangeException();
            }
            Vector3 vec3;
            vec3.x = BitConverter.ToSingle(m_aBuffer, m_iIndex);
            m_iIndex += 4;
            vec3.y = BitConverter.ToSingle(m_aBuffer, m_iIndex);
            m_iIndex += 4;
            vec3.z = BitConverter.ToSingle(m_aBuffer, m_iIndex);
            m_iIndex += 4;
            return vec3;
        }
        public Vector4 getVector4() {
            if (m_iIndex + 16 > m_aBuffer.Length) {
                throw new IndexOutOfRangeException();
            }
            Vector4 vec4;
            vec4.x = BitConverter.ToSingle(m_aBuffer, m_iIndex);
            m_iIndex += 4;
            vec4.y = BitConverter.ToSingle(m_aBuffer, m_iIndex);
            m_iIndex += 4;
            vec4.z = BitConverter.ToSingle(m_aBuffer, m_iIndex);
            m_iIndex += 4;
            vec4.w = BitConverter.ToSingle(m_aBuffer, m_iIndex);
            m_iIndex += 4;
            return vec4;
        }
        public Quaternion getQuaternion() {
            if (m_iIndex + 16 > m_aBuffer.Length) {
                throw new IndexOutOfRangeException();
            }
            Quaternion quat;
            quat.x = BitConverter.ToSingle(m_aBuffer, m_iIndex);
            m_iIndex += 4;
            quat.y = BitConverter.ToSingle(m_aBuffer, m_iIndex);
            m_iIndex += 4;
            quat.z = BitConverter.ToSingle(m_aBuffer, m_iIndex);
            m_iIndex += 4;
            quat.w = BitConverter.ToSingle(m_aBuffer, m_iIndex);
            m_iIndex += 4;
            return quat;
        }
        public long getS64() {
            if (m_iIndex + 8 > m_aBuffer.Length) {
                throw new IndexOutOfRangeException();
            }
            long iResult = BitConverter.ToInt64(m_aBuffer, m_iIndex);
            m_iIndex += 8;
            return iResult;
        }
        public ulong getU64() {
            if (m_iIndex + 8 > m_aBuffer.Length) {
                throw new IndexOutOfRangeException();
            }
            ulong uResult = BitConverter.ToUInt64(m_aBuffer, m_iIndex);
            m_iIndex += 8;
            return uResult;
        }
        public int getString(ref String rsValue, int iMaxLength) {
            int iNum;
            if (iMaxLength < 256) {
                iNum = getU8();
            } else if (iMaxLength < 65536) {
                iNum = getU16();
            } else {
                iNum = getS32();
            }
            if (iNum > iMaxLength) {
                Debug.LogError(iNum + "index error");
                throw new IndexOutOfRangeException();
            }
            if (iNum == 0) {
                rsValue = "";
                return 0;
            }
            int iByte = count(m_aBuffer, m_iIndex, iNum);
            if (iByte == 0) {
                rsValue = "";
                return 0;
            }
            if (iByte < 0) {
                throw new IndexOutOfRangeException();
            }
            rsValue = m_cUtf8Encode.GetString(m_aBuffer, m_iIndex, iByte);
            m_iIndex += iByte;
            return rsValue.Length;
        }
        public byte[] getBinary(int iByte) {
            if (m_iIndex + iByte > m_aBuffer.Length) {
                throw new IndexOutOfRangeException();
            }
            byte[] aBinary = new byte[iByte];
            Array.Copy(m_aBuffer, m_iIndex, aBinary, 0, iByte);
            m_iIndex += iByte;
            return aBinary;
        }
        //==========================================================================
        /*!保持しているシリアライズデータを復号化する.
            @brief	decrpyt
        */
        public bool decrypt(byte[] iv = null) {
            if (m_cryptDES == null) {
                return false;
            }
            byte[] aDecrypt = m_cryptDES.decrypt(iv, m_aBuffer, m_aBuffer.Length);
            if (aDecrypt == null) {
                return false;
            }
            m_aBuffer = aDecrypt;
            return true;
        }
        protected byte[] m_aBuffer;
        protected int m_iIndex;
    };

    //==========================================================================
    /*!CWriteVariable
        @brief	class CWriteVariable
    */
    public class CWriteVariable : CVariable {
        public CWriteVariable(uint uSize) {
            if (uSize < 8) {
                uSize = 8;
            }
            m_aBuffer = new byte[uSize];
            m_uIndex = 0;
        }
        public byte[] buffer() {
            return m_aBuffer;
        }
        public byte[] copybuffer() {
            byte[] buffer = new byte[m_uIndex];
            Array.Copy(m_aBuffer, buffer, m_uIndex);
            return buffer;
        }
        public int size() {
            return m_uIndex;
        }
        public void clear() {
            m_uIndex = 0;
        }
        public void error(string sError) {
            Debug.LogError(sError);
        }
        public void put(bool bVal) {
            if (m_uIndex + 1 > m_aBuffer.Length) {
#if SHORT_BUFFER_WARNING
			Debug.Log("write buffer is too short!!");
#endif
                Array.Resize(ref m_aBuffer, m_aBuffer.Length * 2);
            }
            m_aBuffer[m_uIndex++] = (bVal) ? (byte)1 : (byte)0;
        }
        public void put(sbyte iVal) {
            if (m_uIndex + 1 > m_aBuffer.Length) {
#if SHORT_BUFFER_WARNING
			Debug.Log("write buffer is too short!!");
#endif
                Array.Resize(ref m_aBuffer, m_aBuffer.Length * 2);
            }
            m_aBuffer[m_uIndex++] = (byte)iVal;
        }
        public void put(byte uVal) {
            if (m_uIndex + 1 > m_aBuffer.Length) {
#if SHORT_BUFFER_WARNING
			Debug.Log("write buffer is too short!!");
#endif
                Array.Resize(ref m_aBuffer, m_aBuffer.Length * 2);
            }
            m_aBuffer[m_uIndex++] = uVal;
        }
        public void put(short iVal) {
            if (m_uIndex + 2 > m_aBuffer.Length) {
#if SHORT_BUFFER_WARNING
			Debug.Log("write buffer is too short!!");
#endif
                Array.Resize(ref m_aBuffer, m_aBuffer.Length * 2);
            }
            byte[] aBuffer = BitConverter.GetBytes(iVal);
            aBuffer.CopyTo(m_aBuffer, m_uIndex);
            m_uIndex += 2;
        }
        public void put(ushort uVal) {
            if (m_uIndex + 2 > m_aBuffer.Length) {
#if SHORT_BUFFER_WARNING
			Debug.Log("write buffer is too short!!");
#endif
                Array.Resize(ref m_aBuffer, m_aBuffer.Length * 2);
            }
            byte[] aBuffer = BitConverter.GetBytes(uVal);
            aBuffer.CopyTo(m_aBuffer, m_uIndex);
            m_uIndex += 2;
        }
        public void put(int iVal) {
            if (m_uIndex + 4 > m_aBuffer.Length) {
#if SHORT_BUFFER_WARNING
			Debug.Log("write buffer is too short!!");
#endif
                Array.Resize(ref m_aBuffer, m_aBuffer.Length * 2);
            }
            byte[] aBuffer = BitConverter.GetBytes(iVal);
            aBuffer.CopyTo(m_aBuffer, m_uIndex);
            m_uIndex += 4;
        }
        public void put(uint uVal) {
            if (m_uIndex + 4 > m_aBuffer.Length) {
#if SHORT_BUFFER_WARNING
			Debug.Log("write buffer is too short!!");
#endif
                Array.Resize(ref m_aBuffer, m_aBuffer.Length * 2);
            }
            byte[] aBuffer = BitConverter.GetBytes(uVal);
            aBuffer.CopyTo(m_aBuffer, m_uIndex);
            m_uIndex += 4;
        }
        public void put(float fVal) {
            if (m_uIndex + 4 > m_aBuffer.Length) {
#if SHORT_BUFFER_WARNING
			Debug.Log("write buffer is too short!!");
#endif
                Array.Resize(ref m_aBuffer, m_aBuffer.Length * 2);
            }
            byte[] aBuffer = BitConverter.GetBytes(fVal);
            aBuffer.CopyTo(m_aBuffer, m_uIndex);
            m_uIndex += 4;
        }
        public void put(Vector2 vec2) {
            if (m_uIndex + 8 > m_aBuffer.Length) {
#if SHORT_BUFFER_WARNING
			Debug.Log("write buffer is too short!!");
#endif
                Array.Resize(ref m_aBuffer, m_aBuffer.Length * 2);
            }
            byte[] aBuffer = BitConverter.GetBytes(vec2.x);
            aBuffer.CopyTo(m_aBuffer, m_uIndex);
            m_uIndex += 4;
            aBuffer = BitConverter.GetBytes(vec2.y);
            aBuffer.CopyTo(m_aBuffer, m_uIndex);
            m_uIndex += 4;
        }
        public void put(Vector3 vec3) {
            if (m_uIndex + 12 > m_aBuffer.Length) {
#if SHORT_BUFFER_WARNING
			Debug.Log("write buffer is too short!!");
#endif
                Array.Resize(ref m_aBuffer, m_aBuffer.Length * 2);
            }
            byte[] aBuffer = BitConverter.GetBytes(vec3.x);
            aBuffer.CopyTo(m_aBuffer, m_uIndex);
            m_uIndex += 4;
            aBuffer = BitConverter.GetBytes(vec3.y);
            aBuffer.CopyTo(m_aBuffer, m_uIndex);
            m_uIndex += 4;
            aBuffer = BitConverter.GetBytes(vec3.z);
            aBuffer.CopyTo(m_aBuffer, m_uIndex);
            m_uIndex += 4;
        }
        public void put(Vector4 vec4) {
            if (m_uIndex + 16 > m_aBuffer.Length) {
#if SHORT_BUFFER_WARNING
			Debug.Log("write buffer is too short!!");
#endif
                Array.Resize(ref m_aBuffer, m_aBuffer.Length * 2);
            }
            byte[] aBuffer = BitConverter.GetBytes(vec4.x);
            aBuffer.CopyTo(m_aBuffer, m_uIndex);
            m_uIndex += 4;
            aBuffer = BitConverter.GetBytes(vec4.y);
            aBuffer.CopyTo(m_aBuffer, m_uIndex);
            m_uIndex += 4;
            aBuffer = BitConverter.GetBytes(vec4.z);
            aBuffer.CopyTo(m_aBuffer, m_uIndex);
            m_uIndex += 4;
            aBuffer = BitConverter.GetBytes(vec4.w);
            aBuffer.CopyTo(m_aBuffer, m_uIndex);
            m_uIndex += 4;
        }
        public void put(Quaternion quat) {
            if (m_uIndex + 16 > m_aBuffer.Length) {
#if SHORT_BUFFER_WARNING
			Debug.Log("write buffer is too short!!");
#endif
                Array.Resize(ref m_aBuffer, m_aBuffer.Length * 2);
            }
            byte[] aBuffer = BitConverter.GetBytes(quat.x);
            aBuffer.CopyTo(m_aBuffer, m_uIndex);
            m_uIndex += 4;
            aBuffer = BitConverter.GetBytes(quat.y);
            aBuffer.CopyTo(m_aBuffer, m_uIndex);
            m_uIndex += 4;
            aBuffer = BitConverter.GetBytes(quat.z);
            aBuffer.CopyTo(m_aBuffer, m_uIndex);
            m_uIndex += 4;
            aBuffer = BitConverter.GetBytes(quat.w);
            aBuffer.CopyTo(m_aBuffer, m_uIndex);
            m_uIndex += 4;
        }
        public void put(long iVal) {
            if (m_uIndex + 8 > m_aBuffer.Length) {
#if SHORT_BUFFER_WARNING
			Debug.Log("write buffer is too short!!");
#endif
                Array.Resize(ref m_aBuffer, m_aBuffer.Length * 2);
            }
            byte[] aBuffer = BitConverter.GetBytes(iVal);
            aBuffer.CopyTo(m_aBuffer, m_uIndex);
            m_uIndex += 8;
        }
        public void put(ulong uVal) {
            if (m_uIndex + 8 > m_aBuffer.Length) {
#if SHORT_BUFFER_WARNING
			Debug.Log("write buffer is too short!!");
#endif
                Array.Resize(ref m_aBuffer, m_aBuffer.Length * 2);
            }
            byte[] aBuffer = BitConverter.GetBytes(uVal);
            aBuffer.CopyTo(m_aBuffer, m_uIndex);
            m_uIndex += 8;
        }
        public void put(ref string sVal, int iMaxLength) {
            int iLenByte;
            if (iMaxLength < 256) {
                iLenByte = 1;
            } else if (iMaxLength < 65536) {
                iLenByte = 2;
            } else {
                iLenByte = 4;
            }
            if (sVal == null) {
                sVal = "";
            }
            if (sVal.Length > iMaxLength) {
                Debug.Log("string length is too long");
                sVal = sVal.Substring(0, iMaxLength);
            }
            int iByte = m_cUtf8Encode.GetByteCount(sVal);
            // + 2:for put((ushort) iByteLen) size
            if (m_uIndex + iByte + iLenByte > m_aBuffer.Length) {
#if SHORT_BUFFER_WARNING
			Debug.Log("write buffer is too short!!");
#endif
                Array.Resize(ref m_aBuffer, m_aBuffer.Length * 2 + iByte + iLenByte);
            }
            if (iMaxLength < 256) {
                put((byte)sVal.Length);
            } else if (iMaxLength < 65536) {
                put((ushort)sVal.Length);
            } else {
                put(sVal.Length);
            }
            m_cUtf8Encode.GetBytes(sVal, 0, sVal.Length, m_aBuffer, m_uIndex);
            m_uIndex += iByte;
        }
        //==========================================================================
        /*!保持しているシリアライズデータを暗号化する.
            @brief	encrpyt
        */
        public byte[] encrypt(byte[] iv = null) {
            if (m_cryptDES == null) {
                return null;
            }
            return m_cryptDES.encrypt(iv, m_aBuffer, m_uIndex);
        }
        protected byte[] m_aBuffer = null;
        protected int m_uIndex;
    };
}
