//==============================================================================================
/*!CCryptDES
	@file  CCryptDES
*/
//==============================================================================================
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
    public class CCryptDES : CCryptBase {

        protected DESCryptoServiceProvider m_des;
        protected byte[] m_aDefaultIV;

        public bool initialize(ulong uKeyIV, ulong uKeyData) {
            try {
                m_des = new DESCryptoServiceProvider();
                m_des.Key = BitConverter.GetBytes(uKeyData);
                m_des.IV = BitConverter.GetBytes(uKeyIV);
                m_des.Mode = CipherMode.CBC;
                m_aDefaultIV = m_des.IV;
            } catch (Exception e) {
                Debug.LogWarning("crypt initialize error:" + e);
                m_des = null;
                return false;
            }
            return true;
        }
        override protected bool setIV(byte[] aIV) {
            try {
                if (aIV == null) {
                    m_des.IV = m_aDefaultIV;
                } else {
                    m_des.IV = aIV;
                }
            } catch (Exception e) {
                Debug.LogWarning("can't set initialize vector:" + e);
                return false;
            }
            return true;
        }
        override protected ICryptoTransform decryptor() {
            return m_des.CreateDecryptor();
        }
        override protected ICryptoTransform encryptor() {
            return m_des.CreateEncryptor();
        }
    }
}
