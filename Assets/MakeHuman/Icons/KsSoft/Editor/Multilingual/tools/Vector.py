#!/usr/bin/env python
# -*- coding: utf-8 -*-
from struct import *
import math
import operator

#
# Vector2
#
class Vector2:
	x = 0.0
	y = 0.0
	def __init__ (self):
		self.x = 0.0
		self.y = 0.0

	def set (self,x,y):
		self.x = x
		self.y = y
		return self
	
	def __add__(self,b):
		r = Vector2()
		r.x = self.x + b.x
		r.y = self.y + b.y
		return r

	def __sub__(self,b):
		r = Vector2()
		r.x = self.x - b.x
		r.y = self.y - b.y
		return r

	def __mul__(self,b):
		r = Vector2()
		r.x = self.x * b.x
		r.y = self.y * b.y
		return r
	
	def length(self):
		return math.sqrt(self.lengthSq())

	def lengthSq(self):
		return self.x ** 2 + self.y ** 2
	
	def outer(self,b):
		return self.x * b.y - self.y * b.x
	
	def normalized(self):
		l = self.length()
		if (l > 0):
			self.x = self.x/l
			self.y = self.y/l
		return self
	
	def toString(self):
		print("(" + str(self.x) + "," + str(self.y) + ")")

#
# Vector3
#
class Vector3:
	x = 0.0
	y = 0.0
	z = 0.0
	def __init__ (self):
		self.x = 0.0
		self.y = 0.0
		self.z = 0.0

	def set (self,x,y,z):
		self.x = x
		self.y = y
		self.z = z
		return self

	def __add__(self,b):
		r = Vector3()
		r.x = self.x + b.x
		r.y = self.y + b.y
		r.z = self.z + b.z
		return r

	def __sub__(self,b):
		r = Vector3()
		r.x = self.x - b.x
		r.y = self.y - b.y
		r.z = self.z - b.z
		return r

	def __mul__(self,b):
		r = Vector3()
		r.x = self.x * b.x
		r.y = self.y * b.y
		r.z = self.z * b.z
		return r
	
	def length(self):
		return math.sqrt(self.lengthSq())

	def lengthSq(self):
		return self.x ** 2 + self.y ** 2 + self.z ** 2
	
	def outer(self,b):
		r = Vector3()
		r.x = self.y * b.z - self.z * b.y
		r.y = self.z * b.x - self.x * b.z
		r.z = self.x * b.y - self.y * b.x
		return r

	def normalized(self):
		l = self.length()
		if (l > 0):
			self.x = self.x/l
			self.y = self.y/l
			self.z = self.z/l
		return self

	def toString(self):
		print("(" + str(self.x) + "," + str(self.y) + ",",str(self.z) + ")")


#
# Vector4
#
class Vector4:
	x = 0.0
	y = 0.0
	z = 0.0
	w = 0.0
	def __init__ (self):
		self.x = 0.0
		self.y = 0.0
		self.z = 0.0
		self.w = 0.0
		
	def set (self,x,y,z,w):
		self.x = x
		self.y = y
		self.z = z
		self.w = w
		return self

	def __add__(self,b):
		r = Vector4()
		r.x = self.x + b.x
		r.y = self.y + b.y
		r.z = self.z + b.z
		r.w = self.w + b.w
		return r

	def __sub__(self,b):
		r = Vector4()
		r.x = self.x - b.x
		r.y = self.y - b.y
		r.z = self.z - b.z
		r.w = self.w - b.w
		return r

	def __mul__(self,b):
		r = Vector4()
		r.x = self.x * b.x
		r.y = self.y * b.y
		r.z = self.z * b.z
		r.w = self.w * b.w
		return r
	
	def length(self):
		return math.sqrt(self.lengthSq())

	def lengthSq(self):
		return self.x ** 2 + self.y ** 2 + self.z ** 2 + self.w ** 2

	def normalized(self):
		l = self.length()
		if (l > 0):
			self.x = self.x/l
			self.y = self.y/l
			self.z = self.z/l
			self.w = self.w/l
		return self

	def toString(self):
		print("(" + str(self.x) + "," + str(self.y) + ",",str(self.z) + "," + str(self.w) + ")")

#
# Quaternion
#
class Quaternion:
	x = 0.0
	y = 0.0
	z = 0.0
	w = 0.0
	def __init__ (self):
		self.x = 0.0
		self.y = 0.0
		self.z = 0.0
		self.w = 0.0

	def set (self,x,y,z,w):
		self.x = x
		self.y = y
		self.z = z
		self.w = w
		return self

	def toString(self):
		print("(" + str(self.x) + "," + str(self.y) + ",",str(self.z) + "," + str(self.w) + ")")

