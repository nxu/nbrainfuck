About nbrainfuck
==========

What's this?
----------

nbrainfuck is a collection of free and open source softwares. They are created to make coding in brainfuck language easier. They are written in C# therefore .NET Framework 4 has to be installed on the host computer. Most parts work with mono as well, however the GUI of the BFIDE is written in WPF which is not supported (and not planned to be supported) by mono.

http://www.nxu.hu

Components
----------
###BFInterpreter
 A simple code library for interpreting and debugging standard brainfuck code. 
###BFIDE
An Integrated Development Environment for coding in brainfuck language. Multiple input sources (binary / text file, direct text input), syntax highlighting, output window, character table. Debugger for easier programming: step-by-step debugging, breakpoints ('#' character) and a debug window that shows the instruction pointer, 'The Pointer' and the values of each memory cell. The current command is also highlighted in the code editor. 
###nbrainfuck 
A lightweight brainfuck interpreter.

Working on
----------
###nbrainloller 
A simple yet powerful brainfuck↔brainloller converter. Supports multiple image formats. Integrated to BFIDE (importing brainfuck code from and exporting it to image files).

To do
----------
###bfcompiler
A compiler that fully supports the language. Flexible input sources. Compiled .NET assembly as result.

License
----------
nbrainfuck is released under The MIT License. It is free as in free beer and free as in free speech.
