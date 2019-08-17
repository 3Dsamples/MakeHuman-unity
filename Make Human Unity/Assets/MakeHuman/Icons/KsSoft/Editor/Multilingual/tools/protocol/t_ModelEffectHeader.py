from CVariable import CReadVariable,CWriteVariable
from Vector import *
from t_ModelEffect import	t_ModelEffect
#/==========================================================================
#*!
#	@brief	t_ModelEffectHeader
#/
class t_ModelEffectHeader:
	#/==========================================================================
	#*!
	#	@brief	Member
	#/
	MAGIC_NO = 4276944903
	m_magicNo = 4276944903	#U32
	m_aModelEffect = []	#t_ModelEffect
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
		self.m_magicNo = 4276944903
		self.m_aModelEffect = []

	def read(self,cVariable):
		self.m_magicNo = cVariable.getU32()
		n = cVariable.getU16()
		if (n > 65535) :
			cVariable.error('array size error in m_aModelEffect')
			return False
		self.m_aModelEffect = [None]*n
		for i in range(n):
			self.m_aModelEffect[i] = t_ModelEffect()
			if (not self.m_aModelEffect[i].read(cVariable)):
				return False
		return True

	def write(self,cVariable):
		cVariable.putU32(self.m_magicNo)
		if (len(self.m_aModelEffect) > 65535) :
			cVariable.error('array size error in m_aModelEffect')
			return False
		n = len(self.m_aModelEffect)
		cVariable.putU16(n)
		for i in range(n):
			if (not self.m_aModelEffect[i].write(cVariable)):
				return False
		return True

