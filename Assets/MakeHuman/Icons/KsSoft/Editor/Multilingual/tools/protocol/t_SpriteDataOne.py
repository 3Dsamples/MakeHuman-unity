from CVariable import CReadVariable,CWriteVariable
from Vector import *
#/==========================================================================
#*!
#	@brief	t_SpriteDataOne
#/
class t_SpriteDataOne:
	#/==========================================================================
	#*!
	#	@brief	Member
	#/
	m_id = 0	#U32
	m_color = 0	#U32
	m_ePatch = 0	#S8
	m_aUV = []	#Vector4
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
		self.m_id = 0
		self.m_color = 0
		self.m_ePatch = 0
		self.m_aUV = []

	def read(self,cVariable):
		self.m_id = cVariable.getU32()
		self.m_color = cVariable.getU32()
		self.m_ePatch = cVariable.getS8()
		n = cVariable.getU8()
		if (n > 9) :
			cVariable.error('array size error in m_aUV')
			return False
		self.m_aUV = [0]*n
		for i in range(n):
			self.m_aUV[i] = cVariable.getVector4()
		return True

	def write(self,cVariable):
		cVariable.putU32(self.m_id)
		cVariable.putU32(self.m_color)
		cVariable.putS8(self.m_ePatch)
		if (len(self.m_aUV) > 9) :
			cVariable.error('array size error in m_aUV')
			return False
		n = len(self.m_aUV)
		cVariable.putU8(n)
		for i in range(n):
			cVariable.putVector4(self.m_aUV[i])
		return True

