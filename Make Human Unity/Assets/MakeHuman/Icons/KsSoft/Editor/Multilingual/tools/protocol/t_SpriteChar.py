from CVariable import CReadVariable,CWriteVariable
from Vector import *
#/==========================================================================
#*!
#	@brief	t_SpriteChar
#/
class t_SpriteChar:
	#/==========================================================================
	#*!
	#	@brief	Member
	#/
	id = 0	#U16
	uv = Vector4()
	page = 0	#S8
	xOffset = 0	#S8
	yOffset = 0	#S8
	xAdvance = 0	#S8
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
		self.id = 0
		self.uv = Vector4()
		self.page = 0
		self.xOffset = 0
		self.yOffset = 0
		self.xAdvance = 0

	def read(self,cVariable):
		self.id = cVariable.getU16()
		self.uv = cVariable.getVector4()
		self.page = cVariable.getS8()
		self.xOffset = cVariable.getS8()
		self.yOffset = cVariable.getS8()
		self.xAdvance = cVariable.getS8()
		return True

	def write(self,cVariable):
		cVariable.putU16(self.id)
		cVariable.putVector4(self.uv)
		cVariable.putS8(self.page)
		cVariable.putS8(self.xOffset)
		cVariable.putS8(self.yOffset)
		cVariable.putS8(self.xAdvance)
		return True

