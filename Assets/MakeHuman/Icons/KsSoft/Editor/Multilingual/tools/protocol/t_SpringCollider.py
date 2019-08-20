from CVariable import CReadVariable,CWriteVariable
from Vector import *
#/==========================================================================
#*!
#	@brief	t_SpringCollider
#/
class t_SpringCollider:
	#/==========================================================================
	#*!
	#	@brief	Member
	#/
	boneIndex = 0	#U8
	radius = 0.0	#float
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
		self.boneIndex = 0
		self.radius = 0.0

	def read(self,cVariable):
		self.boneIndex = cVariable.getU8()
		self.radius = cVariable.getFloat()
		return True

	def write(self,cVariable):
		cVariable.putU8(self.boneIndex)
		cVariable.putFloat(self.radius)
		return True

