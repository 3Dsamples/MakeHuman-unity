from CVariable import CReadVariable,CWriteVariable
from Vector import *
from t_Material import	t_Material
from t_TransformData import	t_TransformData
from t_SpringCollider import	t_SpringCollider
from t_SpringBone import	t_SpringBone
#/==========================================================================
#*!
#	@brief	t_MeshInfo
#/
class t_MeshInfo:
	#/==========================================================================
	#*!
	#	@brief	Member
	#/
	m_mMesh = 0	#U32
	m_mHierarchy = 0	#U32
	m_tMaterial = t_Material()	#t_Material
	m_aBonename = []	#std::string
	m_aBoneIndex = []	#U8
	m_aTransformData = []	#t_TransformData
	m_aSpringCollider = []	#t_SpringCollider
	m_aSpringBone = []	#t_SpringBone
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
		self.m_mMesh = 0
		self.m_mHierarchy = 0
		self.m_tMaterial = t_Material()
		self.m_aBonename = []
		self.m_aBoneIndex = []
		self.m_aTransformData = []
		self.m_aSpringCollider = []
		self.m_aSpringBone = []

	def read(self,cVariable):
		self.m_mMesh = cVariable.getU32()
		self.m_mHierarchy = cVariable.getU32()
		if (not self.m_tMaterial.read(cVariable)):
			return False
		n = cVariable.getU8()
		if (n > 255) :
			cVariable.error('array size error in m_aBonename')
			return False
		self.m_aBonename = [0]*n
		for i in range(n):
			self.m_aBonename[i] = cVariable.getString(255)
		n = cVariable.getU8()
		if (n > 255) :
			cVariable.error('array size error in m_aBoneIndex')
			return False
		self.m_aBoneIndex = [0]*n
		for i in range(n):
			self.m_aBoneIndex[i] = cVariable.getU8()
		n = cVariable.getU8()
		if (n > 255) :
			cVariable.error('array size error in m_aTransformData')
			return False
		self.m_aTransformData = [None]*n
		for i in range(n):
			self.m_aTransformData[i] = t_TransformData()
			if (not self.m_aTransformData[i].read(cVariable)):
				return False
		n = cVariable.getU8()
		if (n > 255) :
			cVariable.error('array size error in m_aSpringCollider')
			return False
		self.m_aSpringCollider = [None]*n
		for i in range(n):
			self.m_aSpringCollider[i] = t_SpringCollider()
			if (not self.m_aSpringCollider[i].read(cVariable)):
				return False
		n = cVariable.getU8()
		if (n > 255) :
			cVariable.error('array size error in m_aSpringBone')
			return False
		self.m_aSpringBone = [None]*n
		for i in range(n):
			self.m_aSpringBone[i] = t_SpringBone()
			if (not self.m_aSpringBone[i].read(cVariable)):
				return False
		return True

	def write(self,cVariable):
		cVariable.putU32(self.m_mMesh)
		cVariable.putU32(self.m_mHierarchy)
		if (not self.m_tMaterial.write(cVariable)):
			return False
		if (len(self.m_aBonename) > 255) :
			cVariable.error('array size error in m_aBonename')
			return False
		n = len(self.m_aBonename)
		cVariable.putU8(n)
		for i in range(n):
			cVariable.putString(self.m_aBonename[i],255)
		if (len(self.m_aBoneIndex) > 255) :
			cVariable.error('array size error in m_aBoneIndex')
			return False
		n = len(self.m_aBoneIndex)
		cVariable.putU8(n)
		for i in range(n):
			cVariable.putU8(self.m_aBoneIndex[i])
		if (len(self.m_aTransformData) > 255) :
			cVariable.error('array size error in m_aTransformData')
			return False
		n = len(self.m_aTransformData)
		cVariable.putU8(n)
		for i in range(n):
			if (not self.m_aTransformData[i].write(cVariable)):
				return False
		if (len(self.m_aSpringCollider) > 255) :
			cVariable.error('array size error in m_aSpringCollider')
			return False
		n = len(self.m_aSpringCollider)
		cVariable.putU8(n)
		for i in range(n):
			if (not self.m_aSpringCollider[i].write(cVariable)):
				return False
		if (len(self.m_aSpringBone) > 255) :
			cVariable.error('array size error in m_aSpringBone')
			return False
		n = len(self.m_aSpringBone)
		cVariable.putU8(n)
		for i in range(n):
			if (not self.m_aSpringBone[i].write(cVariable)):
				return False
		return True

