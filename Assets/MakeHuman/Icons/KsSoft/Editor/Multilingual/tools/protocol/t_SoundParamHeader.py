from CVariable import CReadVariable,CWriteVariable
from Vector import *
from t_SoundParam import	t_SoundParam
#/==========================================================================
#*!
#	@brief	t_SoundParamHeader
#/
class t_SoundParamHeader:
	#/==========================================================================
	#*!
	#	@brief	Member
	#/
	MAGIC_NO = 4276948992
	m_magicNo = 4276948992	#U32
	m_aSoundParam = []	#t_SoundParam
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
		self.m_magicNo = 4276948992
		self.m_aSoundParam = []

	def read(self,cVariable):
		self.m_magicNo = cVariable.getU32()
		n = cVariable.getU16()
		if (n > 65535) :
			cVariable.error('array size error in m_aSoundParam')
			return False
		self.m_aSoundParam = [None]*n
		for i in range(n):
			self.m_aSoundParam[i] = t_SoundParam()
			if (not self.m_aSoundParam[i].read(cVariable)):
				return False
		return True

	def write(self,cVariable):
		cVariable.putU32(self.m_magicNo)
		if (len(self.m_aSoundParam) > 65535) :
			cVariable.error('array size error in m_aSoundParam')
			return False
		n = len(self.m_aSoundParam)
		cVariable.putU16(n)
		for i in range(n):
			if (not self.m_aSoundParam[i].write(cVariable)):
				return False
		return True

