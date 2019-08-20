from CVariable import CReadVariable,CWriteVariable
from Vector import *
from t_Billboard import	t_Billboard
#/==========================================================================
#*!
#	@brief	t_Billboards
#/
class t_Billboards:
	#/==========================================================================
	#*!
	#	@brief	Member
	#/
	aBillboard = []	#t_Billboard
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
		self.aBillboard = []

	def read(self,cVariable):
		n = cVariable.getU32()
		if (n > 65536) :
			cVariable.error('array size error in aBillboard')
			return False
		self.aBillboard = [None]*n
		for i in range(n):
			self.aBillboard[i] = t_Billboard()
			if (not self.aBillboard[i].read(cVariable)):
				return False
		return True

	def write(self,cVariable):
		if (len(self.aBillboard) > 65536) :
			cVariable.error('array size error in aBillboard')
			return False
		n = len(self.aBillboard)
		cVariable.putU32(n)
		for i in range(n):
			if (not self.aBillboard[i].write(cVariable)):
				return False
		return True

