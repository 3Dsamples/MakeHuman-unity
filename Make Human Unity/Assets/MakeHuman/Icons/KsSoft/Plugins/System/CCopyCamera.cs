//==============================================================================================
/*!特定のカメラをコピーし追随する.
	@file	CCopyCamera

	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System.Collections;

namespace KS {
    abstract public class CCopyCamera : MonoBehaviour {
        protected Camera m_camera;
        protected Transform m_transform;
        protected Camera m_copyCamera = null;
        protected Transform m_transformCopyCamera;

        //==========================================================================
        /*!initialize
            @brief	initialize
        */
        protected abstract void initialize(Camera camera);

        //==========================================================================
        /*!OnPreRender
            @brief	Unity Callback
        */
        void OnPreRender() {
            // カメラを更新.
            UpdateCamera();
        }
        //==========================================================================
        /*!UpdateCamera
            @brief	Update Camera
        */
        protected virtual void UpdateCamera() {
            if (m_camera == null || m_copyCamera == null) {
                return;
            }
            // カメラを更新.
            m_transform.position = m_transformCopyCamera.position;
            m_transform.rotation = m_transformCopyCamera.rotation;

            m_camera.rect = m_copyCamera.rect;
            m_camera.fieldOfView = m_copyCamera.fieldOfView;
            m_camera.nearClipPlane = m_copyCamera.nearClipPlane;
            m_camera.farClipPlane = m_copyCamera.farClipPlane;
        }
        //==========================================================================
        /*!copy
            @brief	コピーするカメラを設定する.
        */
        public Camera parent {
            set {
                m_transform = transform;
                m_camera = GetComponent<Camera>();
                if (m_camera == null) {
                    m_camera = gameObject.AddComponent<Camera>();
                }
                m_copyCamera = value;
                if (m_copyCamera == null) {
                    return;
                }
                m_transformCopyCamera = m_copyCamera.transform;
                m_camera.CopyFrom(m_copyCamera);
                initialize(m_camera);
            }
            get {
                return m_copyCamera;
            }
        }
        public Camera Camera {
            get {
                return m_camera;
            }
        }
    }
}
