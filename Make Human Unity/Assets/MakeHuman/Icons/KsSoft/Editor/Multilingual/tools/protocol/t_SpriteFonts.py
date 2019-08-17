from CVariable import CReadVariable,CWriteVariable
from Vector import *
from t_SpriteFont import	t_SpriteFont
#/==========================================================================
#*!
#	@brief	t_SpriteFonts
#/
class t_SpriteFonts:
	#/==========================================================================
	#*!
	#	@brief	Member
	#/
	m_aFont = []	#t_SpriteFont
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
		self.m_aFont = []

	def read(self,cVariable):
		n = cVariable.getU8()
		if (n > 255) :
			cVariable.error('array size error in m_aFont')
			return False
		self.m_aFont = [None]*n
		for i in range(n):
			self.m_aFont[i] = t_SpriteFont()
			if (not self.m_aFont[i].read(cVariable)):
				return False
		return True

	def write(self,cVariable):
		if (len(self.m_aFont) > 255) :
			cVariable.error('array size error in m_aFont')
			return False
		n = len(self.m_aFont)
		cVariable.putU8(n)
		for i in range(n):
			if (not self.m_aFont[i].write(cVariable)):
				return False
		return True

