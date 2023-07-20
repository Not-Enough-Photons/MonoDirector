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