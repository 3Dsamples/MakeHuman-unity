#!/usr/bin/env python
# -*- coding: utf-8 -*-
from CVariable import *
from Angle import *
from Id import *
from CError import *
from CExcel import *

err = CError()
excel = CExcel("test.xlsx")
print(excel.sheetnames)

data = excel.convert(excel.getSheetByName("テスト"),err)
print(data)
