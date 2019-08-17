//==============================================================================================
/*!
	@file  CResource
*/
//==============================================================================================
//#deinfe OUTPUT_LOG
//#define	USE_CALLBACK
using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
namespace KS {
    [Flags]
    public enum e_ResourceFlag {
        NONE = 0,
        REMAIN = (1 << 0),
        RESOURCE = (1 << 1),
        DOWNLOAD_DEFAULT = (0 << 2),
        DOWNLOAD_STREAMING = (1 << 2),
        DOWNLOAD_NETWORK = (2 << 2),
        DOWNLOAD_MASK = (3 << 2),
    };
    abstract public class CResourceBase {
        int m_cntRef = 0;
        CAssetBundle m_cAB;
        //==========================================================================
        /*!
            @brief	Constructor
        */
        public CResourceBase(uint id) {
            m_id = id;
        }
        abstract protected void DontDestroyOnLoad();
        abstract public void Destroy();
        //==========================================================================
        /*!release
            @brief	release
        */
        public void release() {
            if (state != e_State.LOADED) {
                return;
            }
            if (m_cntRef > 0) {
                m_cntRef--;
            }
            if (m_cntRef <= 0) {
                m_cntRef = 0;
                if (CAssetBundleMgr.Instance == null) {
                    Destroy();
                    return;
                }
                CAssetBundleMgr.Instance.release(this);
            }
        }
        //==========================================================================
        /*!getFlag
            @brief	フラグを取得する.
        */
        public e_ResourceFlag flag {
            set {
                m_eFlag = value;
                if (isRemain) {
                    DontDestroyOnLoad();
                }
            }
            get {
                return m_eFlag;
            }
        }
        //==========================================================================
        /*!getId
            @brief	オブジェクトidを取得する 
        */
        public uint id {
            get {
                return m_id;
            }
        }
        public static implicit operator uint(CResourceBase cResource) {
            return cResource.m_id;
        }
        //==========================================================================
        /*!state
            @brief	状態を取得する 
        */
        public e_State state {
            get {
                return m_eState;
            }
            set {
                m_eState = value;
            }
        }
        //==========================================================================
        /*!isLoaded
            @brief	読み込み中か判定する 
            @retval	bool	:true	読み込み済み 
            @retval	bool	:false	読み込みは完了していない 
        */
        public bool isLoaded {
            get {
                return (m_eState == e_State.LOADED);
            }
        }
        //==========================================================================
        /*!isRemain
            @brief	メモリに常駐するかチェック.
        */
        public bool isRemain {
            get {
                return ((m_eFlag & e_ResourceFlag.REMAIN) != 0);
            }
        }
        //==========================================================================
        /*!isResource
            @brief	リソースフォルダからよみこんだかどうか?
        */
        public bool isResource {
            get {
                return ((m_eFlag & e_ResourceFlag.RESOURCE) != 0);
            }
        }
        public bool isStreaming {
            get {
                switch ((m_eFlag & e_ResourceFlag.DOWNLOAD_MASK)) {
                case e_ResourceFlag.DOWNLOAD_DEFAULT:
                    return KsSoftConfig.UseStreaming;
                case e_ResourceFlag.DOWNLOAD_NETWORK:
                    return false;
                case e_ResourceFlag.DOWNLOAD_STREAMING:
                    return true;
                }
                return KsSoftConfig.UseStreaming;
            }
        }
        public bool isDownload {
            get {
                switch ((m_eFlag & e_ResourceFlag.DOWNLOAD_MASK)) {
                case e_ResourceFlag.DOWNLOAD_DEFAULT:
                    return KsSoftConfig.UseStreaming;
                case e_ResourceFlag.DOWNLOAD_NETWORK:
                    return true;
                case e_ResourceFlag.DOWNLOAD_STREAMING:
                    return false;
                }
                return !KsSoftConfig.UseStreaming;
            }
        }
        public int refcnt {
            get {
                return m_cntRef;
            }
            set {
                m_cntRef = value;
            }
        }
        public CAssetBundle asset {
            get {
                return m_cAB;
            }
            set {
                m_cAB = value;
            }
        }
        //==========================================================================
        /*!ToString
            @brief	文字列に変換.
        */
        override public string ToString() {
            return new MulId(m_id).ToString() + ".unity3d";
        }
        public enum e_State {
            NOEXIST = -1,
            NONE,
            LOADING,
            LOADED,
        };
        protected e_State m_eState = e_State.NONE;
        protected uint m_id = 0;
        protected e_ResourceFlag m_eFlag = e_ResourceFlag.NONE;
    }
    //==========================================================================
    /*!
        @brief	class CResource<Type>
    */
    public class CResource<Type> : CResourceBase where Type : UnityEngine.Object {
        //==========================================================================
        /*!
            @brief	Constructor
        */
        public CResource(uint id) : base(id) {
            m_Object = null;
        }
        override protected void DontDestroyOnLoad() {
            if (m_Object != null) {
                UnityEngine.Object.DontDestroyOnLoad(m_Object);
            }
        }
        override public void Destroy() {
            m_Object = null;
            if (asset != null) {
                asset.release();
            }
        }
        //==========================================================================
        /*!set
            @brief	Objectを設定する.
        */
        virtual public void set(Type cObject) {
            if (isRemain && m_Object != null) {
                Debug.LogError("already set object:" + new MulId(m_id));
                Destroy();
            }
            m_Object = cObject;
            if (isRemain) {
#if OUTPUT_LOG
			MulId	mId = new MulId(getId());
			Debug.Log("remain asset:" + mId);
#endif
                DontDestroyOnLoad();
            }
        }
        //==========================================================================
        /*!get
            @brief	オブジェクトを取得する 
        */
        public Type get() {
            return m_Object;
        }
        public static implicit operator Type(CResource<Type> cResource) {
            return cResource.m_Object;
        }
        protected Type m_Object;
    }
}
