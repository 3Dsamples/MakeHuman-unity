from CVariable import CReadVariable,CWriteVariable
from Vector import *
#/==========================================================================
#*!
#	@brief	t_TransformData
#/
class t_TransformData:
	#/==========================================================================
	#*!
	#	@brief	Member
	#/
	boneIndex = 0	#U8
	parentIndex = 0	#U8
	position = Vector3()
	rotation = Quaternion()
	scale = Vector3()
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
		self.parentIndex = 0
		self.position = Vector3()
		self.rotation = Quaternion()
		self.scale = Vector3()

	def read(self,cVariable):
		self.boneIndex = cVariable.getU8()
		self.parentIndex = cVariable.getU8()
		self.position = cVariable.getVector3()
		self.rotation = cVariable.getQuaternion()
		self.scale = cVariable.getVector3()
		return True

	def write(self,cVariable):
		cVariable.putU8(self.boneIndex)
		cVariable.putU8(self.parentIndex)
		cVariable.putVector3(self.position)
		cVariable.putQuaternion(self.rotation)
		cVariable.putVector3(self.scale)
		return True

