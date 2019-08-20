//==============================================================================================
/*!乱数.
	@file  KsRandom
    @note   Xorshift
*/
//==============================================================================================
#define COUNT_DEBUG
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


namespace KS {
    public class KsRandom {
        public struct Seed {
            public uint x;
            public uint y;
            public uint z;
            public uint w;
        }
#if COUNT_DEBUG
        int m_count;
#endif
        Seed m_seed;
        public KsRandom() {
            m_seed.x = 123456789;
            m_seed.y = 362436069;
            m_seed.z = 521288629;
            m_seed.w = 88675123;
#if COUNT_DEBUG
            m_count = 0;
#endif
        }
        uint get() {
#if COUNT_DEBUG
            m_count++;
#endif
            uint tmp = m_seed.x ^ (m_seed.x << 11);
            m_seed.x = m_seed.y;
            m_seed.y = m_seed.z;
            m_seed.z = m_seed.w;
            m_seed.w = (m_seed.w ^ (m_seed.w >> 19)) ^ (tmp ^ (tmp >> 8));
            return m_seed.w;
        }
        public uint uvalue {
            get {
                return get();
            }
        }
        public int ivalue {
            get {
                return (int)get();
            }
        }
        public float fvalue {
            get {
                //            return int r = (int) ((uvalue >> 9) | 0x3f800000);
                return (float)(uvalue >> 9) * (1f / (float)0x800000);
            }
        }
        //[min,max - 1]
        public uint Range(uint min, uint max) {
            if (min >= max) {
                return min;
            }
            return min + (get() % (max - min));
        }
        //[min,max - 1]
        public int Range(int min, int max) {
            if (min >= max) {
                return min;
            }
            return min + (int)(get() % (max - min));
        }
        //[min,max)
        public float Range(float min, float max) {
            return min + (max - min) * fvalue;
        }
        public T Range<T>(T[] array) {
            return array[get() % array.Length];
        }
        public T Range<T>(List<T> array) {
            return array[(int)get() % array.Count];
        }
        public void initialize(uint seed) {
            m_seed.x = 123456789 ^ (seed);
            m_seed.y = 362436069 ^ ((seed >> 8) | ((seed & 0xff) << 24));
            m_seed.z = 521288629 ^ ((seed >> 24) | ((seed & 0xffffff) << 8));
            m_seed.w = 88675123 ^ ((seed >> 16) | ((seed & 0xffff) << 16));
#if COUNT_DEBUG
            m_count = 0;
#endif
        }
        public Seed seed {
            get {
                return m_seed;
            }
            set {
                m_seed = value;
#if COUNT_DEBUG
                m_count = 0;
#endif
            }
        }
        override public string ToString() {
#if COUNT_DEBUG
            return "random seed(" + m_count + "):" + m_seed.x + "," + m_seed.y + "," + m_seed.z + "," + m_seed.w;
#else
        return "random seed:" + m_seed.x + "," + m_seed.y + "," + m_seed.z + "," + m_seed.w;
#endif
        }
    }
}
