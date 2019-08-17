from CVariable import CReadVariable,CWriteVariable
from Vector import *
from t_EffectAttribute import	t_EffectAttribute
#/==========================================================================
#*!
#	@brief	t_EffectAttributeHeader
#/
class t_EffectAttributeHeader:
	#/==========================================================================
	#*!
	#	@brief	Member
	#/
	MAGIC_NO = 4276944900
	m_magicNo = 4276944900	#U32
	m_aEffectAttribute = []	#t_EffectAttribute
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
		self.m_magicNo = 4276944900
		self.m_aEffectAttribute = []

	def read(self,cVariable):
		self.m_magicNo = cVariable.getU32()
		n = cVariable.getU16()
		if (n > 65535) :
			cVariable.error('array size error in m_aEffectAttribute')
			return False
		self.m_aEffectAttribute = [None]*n
		for i in range(n):
			self.m_aEffectAttribute[i] = t_EffectAttribute()
			if (not self.m_aEffectAttribute[i].read(cVariable)):
				return False
		return True

	def write(self,cVariable):
		cVariable.putU32(self.m_magicNo)
		if (len(self.m_aEffectAttribute) > 65535) :
			cVariable.error('array size error in m_aEffectAttribute')
			return False
		n = len(self.m_aEffectAttribute)
		cVariable.putU16(n)
		for i in range(n):
			if (not self.m_aEffectAttribute[i].write(cVariable)):
				return False
		return True

