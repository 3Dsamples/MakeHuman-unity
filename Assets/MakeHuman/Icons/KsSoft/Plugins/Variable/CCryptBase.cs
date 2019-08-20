//==============================================================================================
/*!CCryptBase
	@file  CCryptBase
*/
//==============================================================================================
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
namespace KS {
    //==========================================================================
    /*!CCryptBase
        @brief	class CCryptBase
    */
    abstract public class CCryptBase {

        abstract protected ICryptoTransform decryptor();
        abstract protected ICryptoTransform encryptor();

        abstract protected bool setIV(byte[] aIV);
        //==========================================================================
        /*!decrypt
         * @brief	decrypt
        */
        virtual public byte[] decrypt(byte[] aIV, byte[] aBuffer, int length = 0) {
            setIV(aIV);
            return transform(aBuffer, length, decryptor());
        }
        //==========================================================================
        /*!encrypt
         * @brief	encrypt
        */
        virtual public byte[] encrypt(byte[] aIV, byte[] aBuffer, int length = 0) {
            setIV(aIV);
            return transform(aBuffer, length, encryptor());
        }
        //==========================================================================
        /*!transform
         * @brief	transform
        */
        protected byte[] transform(byte[] aBuffer, int length, ICryptoTransform transform) {
            if (transform == null || aBuffer == null) {
                return null;
            }
            if (length == 0) {
                length = aBuffer.Length;
            }
            try {
                byte[] aResult = transform.TransformFinalBlock(aBuffer, 0, length);
                transform.Dispose();
                return aResult;
            } catch (Exception e) {
                Debug.LogWarning("can't decrypt!:" + e);
                if (transform != null) {
                    transform.Dispose();
                }
                return null;
            }
        }
        //==========================================================================
        /*!dump
         * @brief	dump
        */
        public void dump(byte[] aBuffer) {
            string line = "";
            for (int i = 0; i < aBuffer.Length; ++i) {
                line += aBuffer[i].ToString("x02") + ",";
                if ((i & 7) == 7) {
                    Debug.Log(line);
                    line = "";
                }
            }
            if (line != "") {
                Debug.Log(line);
            }
        }
    };
}
