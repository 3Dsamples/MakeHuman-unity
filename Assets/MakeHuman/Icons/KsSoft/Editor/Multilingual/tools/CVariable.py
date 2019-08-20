#!/usr/bin/env python
# -*- coding: utf-8 -*-
import re
from struct import *
from Vector import *

CheckFrameValue = re.compile('(\d+)[fF]$')
#
# WriteVariable
#
class CWriteVariable:
	buffer = []
	def __init__ (self):
		self.buffer = []
	def putBool(self,value):
		if (value is None):
			value = False
		if (value):
			value = 1
		else:
			value = 0
		self.buffer.append( pack('<b',value))
	def putS8(self,value):
		if (value is None):
			value = 0
		self.buffer.append( pack('<b',value))
	def putU8(self,value):
		if (value is None):
			value = 0
		self.buffer.append( pack('<B',value))
	def putS16(self,value):
		if (value is None):
			value = 0
		self.buffer.append( pack('<h',value))
	def putU16(self,value):
		if (value is None):
			value = 0
		self.buffer.append( pack('<H',value))
	def putS32(self,value):
		if (value is None):
			value = 0
		self.buffer.append( pack('<i',value))
	def putU32(self,value):
		if (value is None):
			value = 0
		self.buffer.append( pack('<I',value))
	def putS64(self,value):
		if (value is None):
			value = 0
		self.buffer.append( pack('<q',value))
	def putU64(self,value):
		if (value is None):
			value = 0
		self.buffer.append( pack('<Q',value))
	def putFloat(self,value):
		if (value is None):
			value = 0
		elif (type(value) is str or type(value) is str):
			m = CheckFrameValue.search(value)
			if (m is not None):
				value = int(m.group(1))/30.0
		self.buffer.append( pack('<f',value))
	def putVector2(self,value):
		if (value is None):
			value = Vector2()
		self.buffer.append( pack('<f',value.x))
		self.buffer.append( pack('<f',value.y))
	def putVector3(self,value):
		if (value is None):
			value = Vector3()
		self.buffer.append( pack('<f',value.x))
		self.buffer.append( pack('<f',value.y))
		self.buffer.append( pack('<f',value.z))
	def putVector4(self,value):
		if (value is None):
			value = Vector4()
		self.buffer.append( pack('<f',value.x))
		self.buffer.append( pack('<f',value.y))
		self.buffer.append( pack('<f',value.z))
		self.buffer.append( pack('<f',value.w))
	def putQuaternion(self,value):
		if (value is None):
			value = Quaternion()
		self.buffer.append( pack('<f',value.x))
		self.buffer.append( pack('<f',value.y))
		self.buffer.append( pack('<f',value.z))
		self.buffer.append( pack('<f',value.w))
	def putString(self,value,maxsize):
		if (value is None):
			value = ''
		if (type(value) is int or type(value) is float or type(value) is int):
			value = str(value)
		l = len(value)
		if (l > maxsize):
			l = maxsize
		fmt = ""
		if (maxsize < 256):
			fmt = '<B'
		elif (maxsize < 65536):
			fmt = '<H'
		else:
			fmt = '<I'
		self.buffer.append( pack(fmt,l))
		self.buffer.append( value.encode() )
	def error(self,msg):
		print(msg)
	def output(self,filename):
		try:
			f = open(filename,'wb')
			f.write(b''.join(self.buffer))
			f.close()
		except IOError:
			print('cannot open', filename)
			
	@staticmethod
	def isFrameValue(value):
		if (type(value) is str or type(value) is str):
			m = CheckFrameValue.search(value)
			if (m is not None):
				return True
		return False
	@staticmethod
	def getFrameValue(value):
		if (type(value) is str or type(value) is str):
			m = CheckFrameValue.search(value)
			if (m is not None):
				value = int(m.group(1))/30.0
		return value

#
# ReadVariable
#
class CReadVariable:
	buffer = []
	index = 0
	def __init__ (self,_buffer):
		self.buffer = _buffer
		self.index = 0
	def getBool(self):
		size = 1
		r = unpack('<b',self.buffer[self.index:self.index + size])
		self.index += size
		if (r[0] == 0):
			return False
		return True
	def getS8(self):
		size = 1
		r = unpack('<b',self.buffer[self.index:self.index + size])
		self.index += size
		return r[0]
	def getU8(self):
		size = 1
		r = unpack('<B',self.buffer[self.index:self.index + size])
		self.index += size
		return r[0]
	def getS16(self):
		size = 2
		r = unpack('<h',self.buffer[self.index:self.index + size])
		self.index += size
		return r[0]
	def getU16(self):
		size = 2
		r = unpack('<H',self.buffer[self.index:self.index + size])
		self.index += size
		return r[0]
	def getS32(self):
		size = 4
		r = unpack('<i',self.buffer[self.index:self.index + size])
		self.index += size
		return r[0]
	def getU32(self):
		size = 4
		r = unpack('<I',self.buffer[self.index:self.index + size])
		self.index += size
		return r[0]
	def getS64(self):
		size = 8
		r = unpack('<q',self.buffer[self.index:self.index + size])
		self.index += size
		return r[0]
	def getU64(self):
		size = 8
		r = unpack('<Q',self.buffer[self.index:self.index + size])
		self.index += size
		return r[0]
	def getFloat(self):
		size = 4
		r = unpack('<f',self.buffer[self.index:self.index + size])
		self.index += size
		return r[0]
	def getVector2(self):
		vec2 = Vector2()
		vec2.x = self.getFloat()
		vec2.y = self.getFloat()
		return vec2
	def getVector3(self):
		vec3 = Vector3()
		vec3.x = self.getFloat()
		vec3.y = self.getFloat()
		vec3.z = self.getFloat()
		return vec3
	def getVector4(self):
		vec4 = Vector4()
		vec4.x = self.getFloat()
		vec4.y = self.getFloat()
		vec4.z = self.getFloat()
		vec4.w = self.getFloat()
		return vec4
	def getQuaternion(self):
		quat = Quaternion()
		quat.x = self.getFloat()
		quat.y = self.getFloat()
		quat.z = self.getFloat()
		quat.w = self.getFloat()
		return quat
	def getString(self,maxsize):
		fmt = ""
		size = 0
		if (maxsize < 256):
			fmt = '<B'
			size = 1
		elif (maxsize < 65536):
			fmt = '<H'
			size = 2
		else:
			fmt = '<I'
			size = 4
		r = unpack(fmt,self.buffer[self.index:self.index + size])
		self.index += size
		l = r[0]
		if (l > maxsize):
			print("string size is too long:" + str(l) + "(" + str(maxsize) + ")")
		if (l == 0):
			return ""
		byte = self.count(l)
		r = unpack('<' + str(l) + 's',self.buffer[self.index:self.index + byte])
		self.index += byte
		return r[0]
	def error(self,msg):
		print(msg)
	def input(self,filename):
		try:
			f = open(filename,'rb')
			self.buffer = f.read()
			f.close()
		except IOError:
			print('cannot open', filename)
	def count(self,n):
		size = 0
		i = self.index
		begin = i
		l = len(self.buffer)
		while (i < l):
			if (i >= l):
				return 0
			ch = ord(self.buffer[i])
			i += 1
			if ((0x80 & ch) == 0):
				size += 1
			elif ((0xf0 & ch) == 0xc0):
				for j in range(1,2):
					if (i >= l):
						return -1
					ch = ord(self.buffer[i])
					i += 1
					if (ch == 0 or (ch & 0xc0) != 0x80):
						return -1
				size += 1
			elif ((0xf0 & ch) == 0xe0):
				for j in range(1,3):
					if (i >= l):
						return -2
					ch = ord(self.buffer[i])
					i += 1
					if (ch == 0 or (ch & 0xc0) != 0x80):
						return -2
				size += 1
			elif ((0xf0 & ch) == 0xf0):
				for j in range(1,4):
					if (i >= l):
						return -3
					ch = ord(self.buffer[i])
					i += 1
					if (ch == 0 or (ch & 0xc0) != 0x80):
						return -3
				size += 1
			else:
				return -4
			if (size == n):
				return i - begin
		return 0
