from CVariable import CReadVariable,CWriteVariable
from Vector import *
from t_SpriteChar import	t_SpriteChar
#/==========================================================================
#*!
#	@brief	t_SpriteFont
#/
class t_SpriteFont:
	#/==========================================================================
	#*!
	#	@brief	Member
	#/
	m_id = 0	#U32
	m_face = ""	#std::string
	m_textureName = ""	#std::string
	m_pxSize = 0	#U16
	m_charSpacing = 1	#S8
	m_lineHeight = 0	#S8
	m_baseHeight = 0	#S8
	m_texWidth = 0	#S16
	m_texHeight = 0	#S16
	m_pages = 0	#S8
	m_aChar = []	#t_SpriteChar
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
		self.m_face = ""
		self.m_textureName = ""
		self.m_pxSize = 0
		self.m_charSpacing = 1
		self.m_lineHeight = 0
		self.m_baseHeight = 0
		self.m_texWidth = 0
		self.m_texHeight = 0
		self.m_pages = 0
		self.m_aChar = []

	def read(self,cVariable):
		self.m_id = cVariable.getU32()
		self.m_face = cVariable.getString(255)
		self.m_textureName = cVariable.getString(255)
		self.m_pxSize = cVariable.getU16()
		self.m_charSpacing = cVariable.getS8()
		self.m_lineHeight = cVariable.getS8()
		self.m_baseHeight = cVariable.getS8()
		self.m_texWidth = cVariable.getS16()
		self.m_texHeight = cVariable.getS16()
		self.m_pages = cVariable.getS8()
		n = cVariable.getU32()
		if (n > 65536) :
			cVariable.error('array size error in m_aChar')
			return False
		self.m_aChar = [None]*n
		for i in range(n):
			self.m_aChar[i] = t_SpriteChar()
			if (not self.m_aChar[i].read(cVariable)):
				return False
		return True

	def write(self,cVariable):
		cVariable.putU32(self.m_id)
		cVariable.putString(self.m_face,255)
		cVariable.putString(self.m_textureName,255)
		cVariable.putU16(self.m_pxSize)
		cVariable.putS8(self.m_charSpacing)
		cVariable.putS8(self.m_lineHeight)
		cVariable.putS8(self.m_baseHeight)
		cVariable.putS16(self.m_texWidth)
		cVariable.putS16(self.m_texHeight)
		cVariable.putS8(self.m_pages)
		if (len(self.m_aChar) > 65536) :
			cVariable.error('array size error in m_aChar')
			return False
		n = len(self.m_aChar)
		cVariable.putU32(n)
		for i in range(n):
			if (not self.m_aChar[i].write(cVariable)):
				return False
		return True

