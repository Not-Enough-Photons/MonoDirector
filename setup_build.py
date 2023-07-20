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

# Sets up the managed folder
# This makes it easier for other people to build and contribute to monodirector

import sys
import os

bl_path = None
while True:
    bl_path = input("Where is BONELAB installed? ")

    if os.path.exists(bl_path):
        break
    else:
        print("Path '" + bl_path + "' does not exist! Do you have permissions to read it? Did you enter the path wrong?")

# Does the user have a MelonLoader folder?
ml_path = os.path.join(bl_path, "MelonLoader")

if not os.path.exists(ml_path):
    print("MelonLoader folder was not found! Have you installed MelonLoader?")
    exit(1)

mod_path = os.path.join(bl_path, "Mods")

if not os.path.exists(ml_path):
    print("Mods folder was not found! Have you launched MelonLoader at least once?")
    exit(1)

# Then bridge these folders into a local "Links" folder with a hard link
print("Linking files and folders...")

if not os.path.exists("./Links"):
    os.mkdir("./Links")

os.symlink(mod_path, "./Links/Mods")
os.symlink(ml_path, "./Links/MelonLoader")
os.symlink(bl_path, "./Links/Game")

print("Finding BONELAB executable...")

for file in os.listdir(bl_path):
    if file.endswith(".exe") and file.startswith("BONELAB"):
        print("Found '" + file + "'")
        os.symlink(os.path.join(bl_path, file), "./Links/BONELAB.exe")
        break