from CVariable import CReadVariable,CWriteVariable
from Vector import *
from t_SoundEffect import	t_SoundEffect
#/==========================================================================
#*!
#	@brief	t_SoundEffectHeader
#/
class t_SoundEffectHeader:
	#/==========================================================================
	#*!
	#	@brief	Member
	#/
	MAGIC_NO = 4276949008
	m_magicNo = 4276949008	#U32
	m_aData = []	#t_SoundEffect
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
		self.m_magicNo = 4276949008
		self.m_aData = []

	def read(self,cVariable):
		self.m_magicNo = cVariable.getU32()
		n = cVariable.getU16()
		if (n > 65535) :
			cVariable.error('array size error in m_aData')
			return False
		self.m_aData = [None]*n
		for i in range(n):
			self.m_aData[i] = t_SoundEffect()
			if (not self.m_aData[i].read(cVariable)):
				return False
		return True

	def write(self,cVariable):
		cVariable.putU32(self.m_magicNo)
		if (len(self.m_aData) > 65535) :
			cVariable.error('array size error in m_aData')
			return False
		n = len(self.m_aData)
		cVariable.putU16(n)
		for i in range(n):
			if (not self.m_aData[i].write(cVariable)):
				return False
		return True

