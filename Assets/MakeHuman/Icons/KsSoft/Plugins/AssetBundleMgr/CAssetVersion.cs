//==============================================================================================
/*!
	@file  AssetVersion
*/
//==============================================================================================
using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
namespace KS {
    //==========================================================================
    /*!
        @brief	class CAssetVersion
    */
    public class CAssetVersion : t_AssetVersion {
        bool m_isLoaded = false;
        //==========================================================================
        /*!
            @brief	class CHashOne
        */
        const int hashdataoffset = 8;
        const int size = 4 + 4 + 8 + 8;
        //==========================================================================
        /*!Constructor
            @brief	
        */
        public CAssetVersion() {
            m_isLoaded = false;
            m_verionAsset = KsSoftConfig.VERSION_ASSET;
            m_versionClient = KsSoftConfig.VERSION_CLIENT;
        }
        //==========================================================================
        /*!MD5が一致するかチェック.
            @brief	isEqual
        */
        public bool isEqual(ulong uMd5a, ulong uMd5b) {
            if (m_uMd5a == uMd5a && m_uMd5b == uMd5b) {
                return true;
            }
            return false;
        }
        //==========================================================================
        /*!MD5取得.
            @brief	md5a,md5b
        */
        public ulong md5a {
            get {
                return m_uMd5a;
            }
        }
        public ulong md5b {
            get {
                return m_uMd5b;
            }
        }
        //==========================================================================
        /*!バージョン情報取得.
            @brief	[]
        */
        public t_AssetVersionOne this[uint id] {
            get {
                if (id == 0) {
                    return null;
                }
                foreach (t_AssetVersionOne o in m_aVersion) {
                    if (o.m_mId == id) {
                        return o;
                    }
                }
                return null;
            }
        }
        public t_AssetVersionOne this[string path] {
            get {
                foreach (t_AssetVersionOne o in m_aVersion) {
                    if (o.m_path == path) {
                        return o;
                    }
                }
                return null;
            }
        }
        //==========================================================================
        /*!バージョン情報辞書取得.
            @brief	dicVersion
        */
        public t_AssetVersionOne[] Versions {
            get {
                return m_aVersion;
            }
            set {
                m_aVersion = value;
            }
        }
        //==========================================================================
        /*!バイトストリームから読み込み生成する.
            @brief	read
            @param	byte[]	bytes.
        */
        public int read(byte[] bytes) {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] md5hash = md5.ComputeHash(bytes, size, bytes.Length - size);

            for (int i = 0; i < md5hash.Length; i++) {
                if (bytes[i + hashdataoffset] != md5hash[i]) {
                    Debug.LogError("version hash is illegal");
                    return -2;
                }
            }
            CReadVariable cVariable = new CReadVariable(bytes);
            if (!read(cVariable)) {
                return -3;
            }
            m_isLoaded = true;
            return 0;
        }
        //==========================================================================
        /*!write
            @brief	バイナリデータとして出力する.
        */
        public bool write(ref byte[] aResult) {
            CWriteVariable cVariable = new CWriteVariable(2048);
            write(cVariable);

            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] md5hash = md5.ComputeHash(cVariable.buffer(), size, cVariable.size() - size);
            aResult = cVariable.copybuffer();
            md5hash.CopyTo(aResult, hashdataoffset);
            return true;
        }
        //==========================================================================
        /*!クライアントのバージョンが正しいものかチェック.
            @brief	isValidClientVersion
        */
        public bool isValidClientVersion {
            get {
                if (m_versionClient == KsSoftConfig.VERSION_CLIENT) {
                    return true;
                }
                return false;
            }
        }
        //==========================================================================
        /*!アセットのバージョンを返す.
            @brief	version
        */
        public uint version {
            get {
                return m_verionAsset;
            }
        }
        public bool isLoaded {
            get {
                return m_isLoaded;
            }
        }
    }
}
