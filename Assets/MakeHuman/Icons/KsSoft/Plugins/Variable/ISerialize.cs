//==============================================================================================
/*!シリアライズ可能なオブジェクトのインターフェース.
	@file  ISerializable	
*/
//==============================================================================================
using UnityEngine;


namespace KS {
    //==========================================================================
    /*!
        @brief	ISerializable
    */
    public interface ISerializable {
        bool read(CReadVariable cVariable);
        bool write(CWriteVariable cVariable);
    };
}
