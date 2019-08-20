from CVariable import CReadVariable,CWriteVariable
from Vector import *
from t_ParticleEffect import	t_ParticleEffect
#/==========================================================================
#*!
#	@brief	t_ParticleEffectHeader
#/
class t_ParticleEffectHeader:
	#/==========================================================================
	#*!
	#	@brief	Member
	#/
	MAGIC_NO = 4276944901
	m_magicNo = 4276944901	#U32
	m_aParticleEffect = []	#t_ParticleEffect
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
		self.m_magicNo = 4276944901
		self.m_aParticleEffect = []

	def read(self,cVariable):
		self.m_magicNo = cVariable.getU32()
		n = cVariable.getU16()
		if (n > 65535) :
			cVariable.error('array size error in m_aParticleEffect')
			return False
		self.m_aParticleEffect = [None]*n
		for i in range(n):
			self.m_aParticleEffect[i] = t_ParticleEffect()
			if (not self.m_aParticleEffect[i].read(cVariable)):
				return False
		return True

	def write(self,cVariable):
		cVariable.putU32(self.m_magicNo)
		if (len(self.m_aParticleEffect) > 65535) :
			cVariable.error('array size error in m_aParticleEffect')
			return False
		n = len(self.m_aParticleEffect)
		cVariable.putU16(n)
		for i in range(n):
			if (not self.m_aParticleEffect[i].write(cVariable)):
				return False
		return True

