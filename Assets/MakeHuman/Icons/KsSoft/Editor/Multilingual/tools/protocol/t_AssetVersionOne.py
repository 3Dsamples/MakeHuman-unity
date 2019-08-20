from CVariable import CReadVariable,CWriteVariable
from Vector import *
#/==========================================================================
#*!
#	@brief	t_AssetVersionOne
#/
class t_AssetVersionOne:
	#/==========================================================================
	#*!
	#	@brief	Member
	#/
	m_mId = 0	#U32
	m_path = ""	#std::string
	m_iVersion = 0	#S32
	m_uFlag = 0	#U32
	m_width = 0	#U16
	m_height = 0	#U16
	m_uMd5a = 0	#U64
	m_uMd5b = 0	#U64
	#/==========================================================================
	#*!
	#	@brief	Constructor
	#/
	def __init__(self):
		self.clear()

	#/==========================================================================
	#*!
	#	@brief	Accessor
	#/
	def clear(self):
		self.m_mId = 0
		self.m_path = ""
		self.m_iVersion = 0
		self.m_uFlag = 0
		self.m_width = 0
		self.m_height = 0
		self.m_uMd5a = 0
		self.m_uMd5b = 0

	def read(self,cVariable):
		self.m_mId = cVariable.getU32()
		self.m_path = cVariable.getString(255)
		self.m_iVersion = cVariable.getS32()
		self.m_uFlag = cVariable.getU32()
		self.m_width = cVariable.getU16()
		self.m_height = cVariable.getU16()
		self.m_uMd5a = cVariable.getU64()
		self.m_uMd5b = cVariable.getU64()
		return True

	def write(self,cVariable):
		cVariable.putU32(self.m_mId)
		cVariable.putString(self.m_path,255)
		cVariable.putS32(self.m_iVersion)
		cVariable.putU32(self.m_uFlag)
		cVariable.putU16(self.m_width)
		cVariable.putU16(self.m_height)
		cVariable.putU64(self.m_uMd5a)
		cVariable.putU64(self.m_uMd5b)
		return True

