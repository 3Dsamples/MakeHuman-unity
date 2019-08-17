from CVariable import CReadVariable,CWriteVariable
from Vector import *
from t_SpriteDataOne import	t_SpriteDataOne
#/==========================================================================
#*!
#	@brief	t_SpriteData
#/
class t_SpriteData:
	#/==========================================================================
	#*!
	#	@brief	Member
	#/
	m_id = 0	#U32
	m_sShader = ""	#std::string
	m_aData = []	#t_SpriteDataOne
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
		self.m_sShader = ""
		self.m_aData = []

	def read(self,cVariable):
		self.m_id = cVariable.getU32()
		self.m_sShader = cVariable.getString(255)
		n = cVariable.getU32()
		if (n > 65536) :
			cVariable.error('array size error in m_aData')
			return False
		self.m_aData = [None]*n
		for i in range(n):
			self.m_aData[i] = t_SpriteDataOne()
			if (not self.m_aData[i].read(cVariable)):
				return False
		return True

	def write(self,cVariable):
		cVariable.putU32(self.m_id)
		cVariable.putString(self.m_sShader,255)
		if (len(self.m_aData) > 65536) :
			cVariable.error('array size error in m_aData')
			return False
		n = len(self.m_aData)
		cVariable.putU32(n)
		for i in range(n):
			if (not self.m_aData[i].write(cVariable)):
				return False
		return True

