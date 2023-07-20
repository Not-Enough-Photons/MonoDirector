# -------------------------------------------------------------------------------
# MIT License
# 
# Copyright (c) 2023 Not Enough Photons & adamdev
# 
# Permission is hereby granted, free of charge, to any person obtaining a copy
# of this software and associated documentation files (the "Software"), to deal
# in the Software without restriction, including without limitation the rights
# to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
# copies of the Software, and to permit persons to whom the Software is
# furnished to do so, subject to the following conditions:
# 
# The above copyright notice and this permission notice shall be included in all
# copies or substantial portions of the Software.
# 
# THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
# IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
# FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
# AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
# LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
# OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
# SOFTWARE.
# -------------------------------------------------------------------------------

import os
import sys
import subprocess

if not os.path.exists("Links"):
    print("Links folder does not exist! Have you ran setup_build.py before running this script?")
    exit(1)

debug_build = "--debug" in sys.argv
run_after = False

if "clean" in sys.argv:
    command = "dotnet clean"

    if debug_build:
        command += " -c Debug"
    else:
        command += " -c Release"

    os.system(command)
    exit(0)

if "run" in sys.argv:
    print("[Build] Will run BONELAB after building...")
    run_after = True

command = "dotnet build"

if debug_build:
    print("[Build] Building Debug configuration...")
    command += " -c Debug"
    debug_build = True
else:
    command += " -c Release"

if os.system(command) != 0:
    print("[Build] Failed with non-zero return code!")
    exit(1)

# Copy the resulting output if we want to
out_path = "./Output/"

if debug_build:
    out_path += "Debug/"
else:
    out_path += "Release/"

out_path += "MonoDirector.dll"

print("[Build] Output is located at '" + out_path + "'")

if run_after:
    # Forces an overwrite of the existing file
    with open(out_path, "rb") as out_file:
        with open("./Links/Mods/MonoDirector.dll", "wb") as dst_file:
            dst_file.write(out_file.read())

    print("[Build] Copied MonoDirector.dll! Launching game...")

    # Get the real exe name and real game path
    game_path = os.readlink("./Links/Game").removeprefix("\\\\?\\")
    real_exe = os.readlink("./Links/BONELAB.exe").removeprefix("\\\\?\\")
    
    os.chdir(game_path)
    os.startfile(real_exe)