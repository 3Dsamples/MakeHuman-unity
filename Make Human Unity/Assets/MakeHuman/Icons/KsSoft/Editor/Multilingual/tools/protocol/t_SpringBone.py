from CVariable import CReadVariable,CWriteVariable
from Vector import *
#/==========================================================================
#*!
#	@brief	t_SpringBone
#/
class t_SpringBone:
	#/==========================================================================
	#*!
	#	@brief	Member
	#/
	boneIndex = 0	#U8
	childIndex = 0	#U8
	axis = Vector3()
	radius = 0.0	#float
	stiffnessForce = 0.0	#float
	dragForce = 0.0	#float
	m_aColliderIndex = []	#U8
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
		self.childIndex = 0
		self.axis = Vector3()
		self.radius = 0.0
		self.stiffnessForce = 0.0
		self.dragForce = 0.0
		self.m_aColliderIndex = []

	def read(self,cVariable):
		self.boneIndex = cVariable.getU8()
		self.childIndex = cVariable.getU8()
		self.axis = cVariable.getVector3()
		self.radius = cVariable.getFloat()
		self.stiffnessForce = cVariable.getFloat()
		self.dragForce = cVariable.getFloat()
		n = cVariable.getU8()
		if (n > 255) :
			cVariable.error('array size error in m_aColliderIndex')
			return False
		self.m_aColliderIndex = [0]*n
		for i in range(n):
			self.m_aColliderIndex[i] = cVariable.getU8()
		return True

	def write(self,cVariable):
		cVariable.putU8(self.boneIndex)
		cVariable.putU8(self.childIndex)
		cVariable.putVector3(self.axis)
		cVariable.putFloat(self.radius)
		cVariable.putFloat(self.stiffnessForce)
		cVariable.putFloat(self.dragForce)
		if (len(self.m_aColliderIndex) > 255) :
			cVariable.error('array size error in m_aColliderIndex')
			return False
		n = len(self.m_aColliderIndex)
		cVariable.putU8(n)
		for i in range(n):
			cVariable.putU8(self.m_aColliderIndex[i])
		return True

