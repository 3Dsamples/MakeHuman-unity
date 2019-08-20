//==============================================================================================
/*!角度.
	@file  Angle
*/
//==============================================================================================
using UnityEngine;
using System.Collections;

namespace KS {
    public class Angle {
        public const int zero = 0;
        public const int quatPI = 0x2000;   //45
        public const int halfPI = 0x4000;   //90
        public const int PI = 0x8000;       //180
        public const int PI2 = 0x10000;     //360

        static public int Deg2Ang(float degree) {
            return (int)(degree * (65536.0f / 360.0f));
        }
        static public int Deg2Ang(int degree) {
            return degree * 65536 / 360;
        }
        static public float Ang2Deg(int angle) {
            return (float)angle * (360.0f / 65536.0f);
        }
        static public float Ang2Rad(int angle) {
            short ang = (short)angle;
            return (float)ang * (Mathf.PI / 32768.0f);
        }
        static public float Deg2Rad(float degree) {
            return (float)degree * (Mathf.PI / 180.0f);
        }
        static public int Rad2Ang(float rad) {
            return (int)(rad * (32768.0f / Mathf.PI));
        }
        static public float Rad2Deg(float rad) {
            return (int)(rad * (180f / Mathf.PI));
        }
        static public int Normalize(int angle) {
            return (int)((short)(angle & 0xffff));
        }
        static public float NormalizeDeg(float deg) {
            return Ang2Deg(Normalize(Deg2Ang(deg)));
        }
        static public float Sin(int angle) {
            return Mathf.Sin(Ang2Rad(angle));
        }
        static public float Cos(int angle) {
            return Mathf.Cos(Ang2Rad(angle));
        }
        static public float Tan(int angle) {
            return Mathf.Tan(Ang2Rad(angle));
        }
        static public Vector2 SinCos(int angle) {
            Vector2 v;
            float rad = Ang2Rad(angle);
            v.x = Mathf.Sin(rad);
            v.y = Mathf.Cos(rad);
            return v;
        }
        static public void SinCos(ref Vector3 rV, int angle) {
            float rad = Ang2Rad(angle);
            rV.x = Mathf.Sin(rad);
            rV.z = Mathf.Cos(rad);
        }
        static public int Atan(float y, float x) {
            return Rad2Ang(Mathf.Atan2(y, x));
        }
        static public int Atan(Vector3 dir) {
            return Rad2Ang(Mathf.Atan2(dir.x, dir.z));
        }
        static public Quaternion LookRotation(int angle) {
            return Quaternion.LookRotation(LookVector(angle));
        }
        static public Vector3 LookVector(int angle) {
            Vector3 look;
            // Z軸が、forwardであることに気を付けること.
            look.x = Sin(angle);
            look.y = 0f;
            look.z = Cos(angle);
            return look;
        }
        static public Vector3 LookVector(Quaternion q) {
            return LookVector(getRotateY(q));
        }
        static public int getRotateY(Quaternion q) {
            // Z軸が、forwardであることに気を付けること.
            float x = 1f - 2f * q.y * q.y - 2f * q.z * q.z;
            float y = 2f * q.x * q.z + 2f * q.w * q.y;
            return Atan(y, x);
        }
        static public Vector3 rotateY(Vector3 src, int ang) {
            // Z軸が、forwardであることに気を付けること.
            Vector3 res;
            Vector2 sc = SinCos(ang);
            res.x = src.x * sc.y + src.z * sc.x;
            res.y = src.y;
            res.z = src.x * -sc.x + src.z * sc.y;
            return res;
        }
        static public int LerpAng(int angOrg, int angTgt, float k) {
            angTgt = findNearAngle(angTgt, angOrg);
            return angOrg + (int)((float)(angTgt - angTgt) * k);
        }
        static public float LerpDeg(float degOrg, float degTgt, float k) {
            return Mathf.LerpAngle(degOrg, degTgt, k);
        }
        //==========================================================================
        /*!余弦補完値を取得.
            @brief	最初早くて徐々に速度が落ちるタイプの補完.
            @param	rAngle	:0になったら終了.
                             初期値は halfPI(= 90°)にする.
            @param	speed:speed secで補完が完了するように計算する.
        */
        static public int InitLerpFactor() {
            return halfPI;
        }
        static public float LerpFactor(ref int rAngle, float speed) {
            if (speed < 0.001f) {
                rAngle = 0;
                return 1.0f;
            }
            int spd = (int)(16384f * Time.deltaTime / speed);
            rAngle -= spd;
            if (rAngle < 0) {
                rAngle = 0;
                return 1.0f;
            }
            return Cos(rAngle);
        }
        static public float LerpFactorDelta(ref int rAngle, float speed) {
            float old = Cos(rAngle);
            if (speed < 0.001f) {
                rAngle = 0;
                return 1.0f - old;
            }
            int spd = (int)(16384f * Time.deltaTime / speed);
            rAngle -= spd;
            if (rAngle < 0) {
                rAngle = 0;
                return 1.0f - old;
            }
            return Cos(rAngle) - old;
        }
        //==========================================================================
        /*!線形補完値を取得.
            @param	rAngle	:0になったら終了.
                             初期値は halfPI(= 90°)にする.
            @param	speed:speed secで補完が完了するように計算する.
        */
        static public float LinearLerpFactor(ref int rAngle, float speed) {
            if (speed < 0.001f) {
                rAngle = 0;
                return 1.0f;
            }
            int spd = (int)(16384f * Time.deltaTime / speed);
            rAngle -= spd;
            if (rAngle < 0) {
                rAngle = 0;
                return 1.0f;
            }
            return 1f - (float)rAngle * (1f / 16384f);
        }
        //==========================================================================
        /*!余弦補完値を取得.
            @brief	最初遅く、真ん中で最速になった後、徐々に速度が落ちるタイプの補完.
            @param	rAngle	:0になったら終了.
                             初期値は halfPI(= 90°)にする.
            @param	speed:speed secで補完が完了するように計算する.
        */
        static public float LerpFactor2(ref int rAngle, float speed) {
            if (speed < 0.001f) {
                rAngle = 0;
                return 1.0f;
            }
            int spd = (int)(16384f * Time.deltaTime / speed);
            rAngle -= spd;
            if (rAngle < 0) {
                rAngle = 0;
                return 1.0f;
            }
            return (Cos(2 * rAngle) + 1f) * 0.5f;
        }
        //==========================================================================
        /*!src -> dstに補完する際、abs(dst - src)が最小になるようなdstを探す.
            例:	src = 0度,dst = -190度の時、dstを170として返す.
        */
        static public int findNearAngle(int dst, int src) {
            dst = (short)dst;
            src = (short)src;
            int result = dst;
            int dAng = dst - src;
            if (dAng > Angle.PI) {
                result = result - Angle.PI2;
            } else if (dAng < -Angle.PI) {
                result = result + Angle.PI2;
            }
            return result;
        }
        //==========================================================================
        /*!src -> dst間の最小となる角度を探す.
        */
        static public int deltaAngle(int dst, int src) {
            dst = (short)dst;
            src = (short)src;
            int d = dst - src;
            if (d > Angle.PI) {
                while (d > Angle.PI) {
                    d -= Angle.PI2;
                }
            } else if (d < -Angle.PI) {
                while (d < -Angle.PI) {
                    d += Angle.PI2;
                }
            }
            return d;
        }
        //==========================================================================
        /*!3点の角度求める.
            [param]
                p0,p1,p2
            [retval]
                (p2 - p0) <= (p1 - p0)の角度.
        */
        static public int calcAngle(Vector3 p0, Vector3 p1, Vector3 p2) {
            return deltaAngle(Atan(p2 - p0), Atan(p1 - p0));
        }
        //==========================================================================
        /*!p0 - p1の線分とpointまでの距離を求める.
            [param]
                point,p0,p1
            [retval]
                (p2 - p0) <= (p1 - p0)の角度.
        */
        static public float distancePointAndLine(Vector3 point, Vector3 p0, Vector3 p1, bool isSegment = true) {
            Vector3 ab, ap;
            ab = p1 - p0;
            ap = point - p0;
            float d = Vector3.Cross(ab, ap).magnitude;
            float h = d / ab.magnitude;
            if (isSegment) {
                float h0 = (p0 - point).sqrMagnitude;
                float h1 = (p1 - point).sqrMagnitude;
                if (h0 < h1) {
                    if (h0 < h * h) {
                        return Mathf.Sqrt(h0);
                    }
                    return h;
                } else {
                    if (h1 < h * h) {
                        return Mathf.Sqrt(h1);
                    }
                    return h;
                }
            }
            return h;
        }
    }
}
