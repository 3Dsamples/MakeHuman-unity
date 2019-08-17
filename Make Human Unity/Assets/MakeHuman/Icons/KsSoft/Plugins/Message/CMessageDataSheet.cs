//==============================================================================================
/*!CMessageDataSheet
	@file  CMessageDataSheet

	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System.Collections.Generic;
namespace KS {
    public class CMessageDataSheet : IWinCaptionData {
        // key:id
        protected Dictionary<uint, string> m_dicMessage = new Dictionary<uint, string>();
        protected uint m_type;
        static System.Text.StringBuilder m_stringbuilder;
        static CMessageDataSheet() {
            m_stringbuilder = new System.Text.StringBuilder(2048);
        }
        //==========================================================================
        /*!
            @brief	Constructor.
        */
        public CMessageDataSheet(t_MessageDataSheet sheet) {
            m_type = sheet.m_type;
            foreach (t_MessageDataOne one in sheet.m_aContents) {
                m_dicMessage[one.m_id] = one.m_value;
            }
        }
        public void validate(CWinCtrlRichText rt) {
#if UNITY_EDITOR
            foreach (uint id in m_dicMessage.Keys) {
                if (!rt.validate(m_dicMessage[id])) {
                    Debug.LogError("at " + new FiveCC(m_type) + ":" + new MulId(id));
                }
            }
#endif
        }
        //==========================================================================
        /*!
            @brief	find
        */
        public string find(uint id) {
            string sTarget;
            if (!m_dicMessage.TryGetValue(id, out sTarget)) {
                Debug.LogWarning("can't find message content:" + new MulId(id) + " at " + new FiveCC(m_type));
                return null;
            }
            return sTarget;
        }
        public string tryGetValue(uint id) {
            string sTarget;
            if (!m_dicMessage.TryGetValue(id, out sTarget)) {
                return null;
            }
            return sTarget;
        }
        //==========================================================================
        /*!
            @brief	format
        */
        public string format(uint id, params object[] args) {
            string value = find(id);
            if (value == null) {
                return null;
            }
            m_stringbuilder.Length = 0;
            m_stringbuilder.AppendFormat(value, args);
            return m_stringbuilder.ToString();
        }
        //==========================================================================
        /*!複数形に対応したフォーマット
            @brief	n > 1の時、idを選択
                    n < 2の時、id + 1を選択

        */
        public string formatPlual(uint id, int n) {
            if (n <= 1) {
                id += 1;
            }
            string value = find(id);
            if (value == null) {
                return null;
            }
            m_stringbuilder.Length = 0;
            m_stringbuilder.AppendFormat(value, n);
            return m_stringbuilder.ToString();
        }
        //==========================================================================
        /*!
            @brief	type
        */
        public uint type {
            get { return m_type; }
        }
        public Dictionary<uint, string> messages {
            get {
                return m_dicMessage;
            }
        }
        public override string ToString() {
            return FiveCC.ToString(m_type);
        }
        public System.Text.StringBuilder stringbuilder {
            get {
                return m_stringbuilder;
            }
        }
    }
}
