#!/usr/bin/env python
# -*- coding: utf-8 -*-
import re
from struct import *

#
# class CMulId
#
class CMulId:
	value = 0
	def	__init__(self,value,error):
		if (value == None):
			return
		if (isinstance(value,int)):
			self.value = value
		elif (isinstance(value,float)):
			self.value = int(value)
		else:
			self.set(value,error)

	def set(self,string,error):
		if (string.isdigit()):
			self.value = int(string)
			return
		r = re.compile(r"\s*(\d+)_(\d+)_(\d+)\s*")
		m = r.match(string)
		self.value = 0
		if (m is None):
			if (error != None):
				error.add("error:multi id format error:" + string)
			return
		grp = m.groups()
		if (len(grp) != 3):
			if (error != None):
				error.add("@error:multi id format error:" + string)
			return
		upper = int(m.group(1))
		if (upper > 255):
			if (error != None):
				error.add("error:upper num is too large:" + str(upper))
			return
		middle = int(m.group(2))
		if (middle > 255):
			if (error != None):
				error.add("error:middle num is too large:" + str(middle))
			return
		lower = int(m.group(3))
		if (lower > 65535):
			if (error != None):
				error.add("error:lower num is too large:" + str(lower))
			return
		self.value = (upper << 24)|(middle << 16)|lower
		return self.value

	@staticmethod
	def isMulId(val):
		r = re.compile(r"\s*(\d+)_(\d+)_(\d+)\s*")
		m = r.match(val)
		if (m is None):
			return False
		return True
	
	def toString(self):
		return '%(#1)03d_%(#2)03d_%(#3)05d' % {"#1":self.upper(),"#2":self.middle(),"#3":self.lower()}

	def upper(self):
		return (self.value >> 24) & 255
	def setUpper(self,val):
		self.value = (self.value & 0xffffff) | ((val & 0xff) << 24)

	def middle(self):
		return (self.value >> 16) & 255
	def setMiddle(self,val):
		self.value = (self.value & 0xff00ffff) | ((val & 0xff) << 16)

	def lower(self):
		return self.value & 65535
	def setLower(self,val):
		self.value = (self.value & 0xffff0000) | (val & 0xffff)
		
#
# initilaize table for CFiveCC
#
_fiveccidchar = " 0123456789_abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"
_fiveccidtable = {}
for i in range(128):
	_fiveccidtable[i] = 0xffff
for i in range(1,64):
	_fiveccidtable[ord(_fiveccidchar[i])] = i
_fiveccidtable[0] = 0
_fiveccidtable[63] = 11		#'?'
_fiveccidtable[42] = 11		#'*'

#
# class CFiveCC
#
class CFiveCC:
	value = 0
	def __init__(self,value,error):
		if (value == None):
			return
		if (isinstance(value,int) or isinstance(value,int)):
			self.value = value
		else:
			self.set(value,error)

	def set(self,string,error):
		l = len(string)
		if (l > 5):
			if (error != None):
				error.add("error:five cc is too long:" + string + "(" + str(len(string)) + ")")
			return 0
		self.value = 0
		for i in range(l):
			ch = ord(string[i])
			if (ch > 128 or _fiveccidtable[ch] == 0xffff):
				if (error != None):
					error.add("error:five cc illegal code:" + string + "(" + string[i] + ")")
				ch = 0
			self.value |= _fiveccidtable[ch] << (i * 6)
		return self.value

	def toString(self):
		r = ''
		for i in range(5):
			id = (self.value >> (i * 6)) & 63
			if (id == 0):
				break
			print(id)
			r += _fiveccidchar[id]
		return r
#
# MulId,FiveCCいずれかのIDを数値化する.
#
def ConvertId(value,error):
	if (value == None):
		return 0
	mulId = CMulId(value,None)
	if (mulId.value != 0):
		return mulId.value
	fcId = CFiveCC(value,None)
	if (fcId.value != 0):
		return fcId.value
	if (error != None):
		error.add("error:this id is not multi id or fivecc:" + value)
	return 0

class CColor:
	r = 1.0
	g = 1.0
	b = 1.0
	a = 1.0
	def __init__(self,value,error):
		self.set(value,error)

	def set(self,value,error):
		if (value == None):
			self.r = 1.0
			self.g = 1.0
			self.b = 1.0
			self.a = 1.0
			return
		elif (isinstance(value,int)):
			self.r = ((value >> 16) & 255) / 255.0; 
			self.g = ((value >>  8) & 255) / 255.0; 
			self.b = ((value >>  0) & 255) / 255.0; 
			self.a = ((value >> 24) & 255) / 255.0; 
			return
		if (',' in value):
			v = value.split(",")
		elif (':' in value):
			v = value.split(":")
		elif (';' in value):
			v = value.split(";")
		else:
			v = value
		if (len(v) == 4):
			self.r = float(v[0])
			self.g = float(v[1])
			self.b = float(v[2])
			self.a = float(v[3])
		elif (len(v) == 3):
			self.r = float(v[0])
			self.g = float(v[1])
			self.b = float(v[2])
			self.a = 1.0
		else:
			error.add("color format error:" + value)

	def getU32(self,error):
		a = int(self.a * 255.0)
		r = int(self.r * 255.0)
		g = int(self.g * 255.0)
		b = int(self.b * 255.0)
		if (a > 255):
			a = int(self.a)
			r = int(self.r)
			g = int(self.g)
			b = int(self.b)
		if (a > 255):
			error.add("warning:alpha element is too large:" + str(self.a))
			a = 255
		if (r > 255):
			error.add("warning:red element is too large:" + str(self.r))
			r = 255
		if (g > 255):
			error.add("warning:green element is too large:" + str(self.g))
			g = 255
		if (b > 255):
			error.add("warning:blue element is too large:" + str(self.b))
			b = 255
		return (a << 24)|(r << 16)|(g << 8)|b
	def getW32(self,error):
		return self.getU32(error)

class CTime:
	value = 0
	def __init__(self,value,error):
		if (value == None):
			return
		if (isinstance(value,int) or isinstance(value,int)):
			self.value = value
		else:
			self.set(value,error)

	def set(self,string,error):
		month = 0
		day = 0
		hour = 0
		min = 0
		r = re.compile(r"(\d+):(\d+):(\d+):(\d+)")
		m = r.match(string)
		if (m is None):
			r = re.compile(r"([^:]+):(\d+):(\d+)")
			m = r.match(string)
			if (m is None):
				error.add("error:date format error:" + string)
			else:
				dicWeek = {'SU':0,'MO':1,'TU':2,'WE':3,'TH':4,'FR':5,'ST':6,
						   '日':0,'月':1,'火':2,'水':3,'木':4,'金':5,'土':6}
				week = m.group(1).upper()
				if (week in dicWeek):
					day = dicWeek[week]
				else:
					error.add("warning:day of week is illegal range:" + str(week))
				hour = int(m.group(2))
				min = int(m.group(3))
		else:
			month = int(m.group(1))
			if (month < 1 or month > 12):
				error.add("warning:month is illegal range[1..12]:" + str(month))
				month = 1
			day = int(m.group(2))
			if (day < 1 or day > 31):
				error.add("warning:day is illegal range[1..31]:" + str(day))
				day = 1
			hour = int(m.group(3))
			min = int(m.group(4))
		if (hour < 0 or hour > 23):
			error.add("warning:hour is illegal range[0..23]:" + str(hour))
			hour = 0
		if (min < 0 or min > 59):
			error.add("warning:min is illegal range[0..59]:" + str(min))
			min = 0
		self.value = (month << 24)|(day << 16)|(hour << 8)|min
		return self.value

	def toString(self):
		month = self.value >> 24;
		day = (self.value >> 16) & 255;
		hour = (self.value >> 8) & 255;
		min = self.value & 255;
		s = ''
		if (month == 0):
			aWeek = ['Su','Mo','Tu','We','Th','Fr','St']
			s += str(aWeek[day])
		else:
			s += str(month) + ':' + str(day)
		s += ':' + str(hour) + ':' + str(min)
		return s
