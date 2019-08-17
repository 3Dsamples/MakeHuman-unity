------------------------------------------
The features of Window System for Unity
------------------------------------------
・Window-based system
  By the window-based system, such as the screen transition is made easier.
  Since it can be stuck to the window together, it becomes easier to have return to the state before opening the window.

・The system that does not collapse even if your application is developed by more than developers.
  When your application is developed by more than developers,you use a version control software (git or svn) in many cases.
  In this case, many problems occur in the merge.The window system is able to solve these problems.

・The separation of the window data and program logics.

・Powerful and easy / simple window description language.
  Script for describing the window definition can be easy and simple to write.
  It can be described radio buttons,tab controls,listbox,and etc.
  This script is able to write a window animation at open/close.

・Support a flexible layout not collapse even if the screen size is changed.

・Development efficient texture atlas.
  Texture Atlas of the window system support is turned to what was one step evolution texture atlas with the Unity.
  You can specify a patch how individually w hen texture atlas. The patch is a scaling method of the texture.
  You can specify to extend only the middle and fix the both ends.
  In addition, when performing the Texture Atlas, it is possible to automatically color reduction to 16bit textures from 32bit.
  In this case,it is possible to set whether a simple color reduction or dithering color individually texture.

・Optimized rendering.
  It has been to render together multiple controls in one of the mesh.
  Optimization of the draw call is automatically performed. Even a complex screen, it can provide a comfortable GUI.

・Multilingual possibility for the window caption
  The separation of the caption data and application.
  So the window system is able to make multilingual applications.

iOS, Android, MS-Windows, and Mac OS X confirmed.

More detail available here.

http://unity-window-resource.github.io/

