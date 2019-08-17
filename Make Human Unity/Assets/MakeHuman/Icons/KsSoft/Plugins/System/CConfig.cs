//==============================================================================================
/*!設定ファイル読み込み.
	@file  CConfig
*/
//==============================================================================================
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace KS {
    public class CConfig {
        Dictionary<string, string> m_dcKey = new Dictionary<string, string>();
        //==========================================================================
        /*!Constructor
         * @brief	CConfigLoader
        */
        public CConfig() {
            if (m_instance != null) {
                Debug.LogError("already exist configloader");
                return;
            }
            m_instance = this;
            initialize();
        }
        //==========================================================================
        /*!初期化を行う.
         * @brief	intialize
        */
        public void initialize() {
            m_dcKey.Clear();
        }
        //==========================================================================
        /*!設定ファイルを読み込む.
         * @brief	load
        */
        public bool load(string sConfig) {
            if (!File.Exists(sConfig)) {
                return false;
            }
            using (StreamReader sr = new StreamReader(sConfig)) {
                string line;
                while ((line = sr.ReadLine()) != null) {
                    line = line.TrimStart();
                    if (line.Length == 0 || line[0] == '#' || line.Length == 0) continue;
                    string[] aCmd = line.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                    if (aCmd.Length == 2) {
                        string[] aVal = aCmd[1].Split(new char[] { ' ', '\t', ';' }, StringSplitOptions.RemoveEmptyEntries);
                        string key = aCmd[0].TrimEnd();
                        m_dcKey[key] = aVal[0];
                    }
                }
            }
            return true;
        }
        //==========================================================================
        /*!設定データを文字列として取得する.
         * @brief	get
        */
        public string get(string sKey, string sDefault) {
            string result;
            if (m_dcKey.TryGetValue(sKey, out result)) {
                return result;
            }
            return sDefault;
        }
        //==========================================================================
        /*!設定データを整数として取得する.
         * @brief	get
        */
        public int get(string sKey, int nDefault) {
            string result;
            if (m_dcKey.TryGetValue(sKey, out result)) {
                int n;
                Int32.TryParse(result, out n);
                return n;
            }
            return nDefault;
        }
        //==========================================================================
        /*!設定データを符号なし整数として取得する.
         * @brief	get
        */
        public uint get(string sKey, uint nDefault) {
            string result;
            if (m_dcKey.TryGetValue(sKey, out result)) {
                uint n;
                UInt32.TryParse(result, out n);
                return n;
            }
            return nDefault;
        }
        //==========================================================================
        /*!設定データを浮動小数点として取得する.
         * @brief	get
        */
        public float get(string sKey, float fDefault) {
            string result;
            if (m_dcKey.TryGetValue(sKey, out result)) {
                float f;
                Single.TryParse(result, out f);
                return f;
            }
            return fDefault;
        }
        //==========================================================================
        /*!キーが含まれるかどうか判定する.
         * @brief	containsKey
        */
        public bool containsKey(string sKey) {
            return m_dcKey.ContainsKey(sKey);
        }
        //==========================================================================
        /*!Instance.
            @brief	Instance.
        */
        static protected CConfig m_instance = null;
        public static CConfig Instance {
            get {
                return m_instance;
            }
        }
    }
}
