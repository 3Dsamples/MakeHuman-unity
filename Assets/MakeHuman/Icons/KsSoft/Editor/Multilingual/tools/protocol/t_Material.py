from CVariable import CReadVariable,CWriteVariable
from Vector import *
#/==========================================================================
#*!
#	@brief	t_Material
#/
class t_Material:
	#/==========================================================================
	#*!
	#	@brief	Member
	#/
	PropertyType_Color = 0
	PropertyType_Vector = 1
	PropertyType_Float = 2
	PropertyType_Texture = 3
	m_mId = 0	#U32
	m_shader = ""	#std::string
	m_nProperty = 0	#U16
	m_aBuffer = []	#U8
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
		self.m_shader = ""
		self.m_nProperty = 0
		self.m_aBuffer = []

	def read(self,cVariable):
		self.m_mId = cVariable.getU32()
		self.m_shader = cVariable.getString(255)
		self.m_nProperty = cVariable.getU16()
		n = cVariable.getU16()
		if (n > 65535) :
			cVariable.error('array size error in m_aBuffer')
			return False
		self.m_aBuffer = [0]*n
		for i in range(n):
			self.m_aBuffer[i] = cVariable.getU8()
		return True

	def write(self,cVariable):
		cVariable.putU32(self.m_mId)
		cVariable.putString(self.m_shader,255)
		cVariable.putU16(self.m_nProperty)
		if (len(self.m_aBuffer) > 65535) :
			cVariable.error('array size error in m_aBuffer')
			return False
		n = len(self.m_aBuffer)
		cVariable.putU16(n)
		for i in range(n):
			cVariable.putU8(self.m_aBuffer[i])
		return True

