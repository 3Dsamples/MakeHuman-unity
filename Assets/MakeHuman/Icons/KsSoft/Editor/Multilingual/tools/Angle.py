#!/usr/bin/env python
# -*- coding: utf-8 -*-

def Deg2Ang(deg):
	if (deg is None):
		return 0
	return (int) ((deg * 65536.0)/360.0)

def Deg2Ang16(deg):
	if (deg is None):
		return 0
	while (deg >= 180):
		deg -= 360
	while (deg < -180):
		deg += 360
	return int(max(min(((deg * 65536.0)/360.0),32767),-32768))

def Deg2AngU16(deg):
	if (deg is None):
		return 0
	while (deg >= 180):
		deg -= 360
	while (deg < -180):
		deg += 360
	return int((deg * 65536.0)/360.0) & 0xffff

def Ang2Deg(ang):
	if (ang is None):
		return 0
	return ang * 360.0/65536.0

