//==============================================================================================
/*!UID
	@file  UID

*/
//==============================================================================================
using UnityEngine;
using System;
using System.Collections;
namespace KS {
    //==========================================================================
    /*!
        @brief	class UID
        @note	.
        @ref	UID
    */
    public struct UID {
        ulong m_value;

        static ulong m_ulUIDCounter = 0;
        static byte m_uWorldNumber = 255;
        static byte m_uSvNumber = 255;

        static public ulong create() {
            UID uid = new UID((uint)KsSoftUtility.getUnixTime(), (ushort)++m_ulUIDCounter, m_uSvNumber, m_uWorldNumber);
            return uid.m_value;
        }
        static public void initialize(byte uWorldNumber, byte uSvNumber) {
            m_uSvNumber = uSvNumber;
            m_uWorldNumber = uWorldNumber;
        }
        public UID(uint date, ushort counter, byte svNo, byte worldNo) {
            m_value = 0;
            set(date, counter, svNo, worldNo);
        }
        public UID(ulong val) {
            m_value = val;
        }
        //==========================================================================
        /*!Id
            @brief	Int形式に変換.
        */
        public void set(ulong val) {
            m_value = val;
        }
        //==========================================================================
        /*!Id
            @brief	Int形式に変換.
        */
        public void set(uint date, ushort counter, byte svNo, byte worldNo) {
            m_value = ((ulong)date) |
                     ((ulong)counter << 32) |
                     ((ulong)svNo << 48) |
                     ((ulong)worldNo << 56)
                    ;
        }
        public uint date {
            get {
                return (uint)(m_value & 0xffffffff);
            }
        }
        static public uint getDate(ulong uid) {
            return (uint)(uid & 0xffffffff);
        }
        public ushort counter {
            get {
                return (ushort)((m_value >> 32) & 0xffff);
            }
        }
        static public ushort getCounter(ulong uid) {
            return (ushort)((uid >> 32) & 0xffff);
        }
        public byte svNo {
            get {
                return (byte)((m_value >> 48) & 0xff);
            }
        }
        static public byte getSvNo(ulong uid) {
            return (byte)((uid >> 48) & 0xff);
        }
        public byte worldNo {
            get {
                return (byte)((m_value >> 56) & 0xff);
            }
        }
        static public byte getWorldNo(ulong uid) {
            return (byte)((uid >> 56) & 0xff);
        }
        override public string ToString() {
            return m_value.ToString();
        }
        static public implicit operator ulong(UID uid) {
            return uid.m_value;
        }
    }
}
