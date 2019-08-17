from CVariable import CReadVariable,CWriteVariable
from Vector import *
from t_AssetVersionOne import	t_AssetVersionOne
#/==========================================================================
#*!
#	@brief	t_AssetVersion
#/
class t_AssetVersion:
	#/==========================================================================
	#*!
	#	@brief	Member
	#/
	m_verionAsset = 0	#U32
	m_versionClient = 0	#U32
	m_uMd5a = 0	#U64
	m_uMd5b = 0	#U64
	m_aVersion = []	#t_AssetVersionOne
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
		self.m_verionAsset = 0
		self.m_versionClient = 0
		self.m_uMd5a = 0
		self.m_uMd5b = 0
		self.m_aVersion = []

	def read(self,cVariable):
		self.m_verionAsset = cVariable.getU32()
		self.m_versionClient = cVariable.getU32()
		self.m_uMd5a = cVariable.getU64()
		self.m_uMd5b = cVariable.getU64()
		n = cVariable.getU16()
		if (n > 65535) :
			cVariable.error('array size error in m_aVersion')
			return False
		self.m_aVersion = [None]*n
		for i in range(n):
			self.m_aVersion[i] = t_AssetVersionOne()
			if (not self.m_aVersion[i].read(cVariable)):
				return False
		return True

	def write(self,cVariable):
		cVariable.putU32(self.m_verionAsset)
		cVariable.putU32(self.m_versionClient)
		cVariable.putU64(self.m_uMd5a)
		cVariable.putU64(self.m_uMd5b)
		if (len(self.m_aVersion) > 65535) :
			cVariable.error('array size error in m_aVersion')
			return False
		n = len(self.m_aVersion)
		cVariable.putU16(n)
		for i in range(n):
			if (not self.m_aVersion[i].write(cVariable)):
				return False
		return True

