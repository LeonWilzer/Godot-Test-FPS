# Godot Test FPS
## About
These are the project files for a first person shooter made with the Godot game engine. At the current stage it is mostly a port of the official Godot FPS tutorial (from GDScript to C#). Extending the game past official tutorials is definitely planned.
## Licensing
The source code is licensed under GPL v3.0 or later.

The licensing for assets and other files is still pending as it is planned to replace the assets provided by the Godot tutorial with custom-made ones. So please refer to the official Godot FPS tutorial for more information about the used assets.
https://docs.godotengine.org/en/3.1/tutorials/3d/fps_tutorial/part_one.html

**Contact:** leon.wilzer@protonmail.com, if you believe that your property is being misused here.

## How to play
### Stable (easy)
1. Download the game's binaries here:
https://github.com/LeonWilzer/Godot-Test-FPS/releases
Please file an issue (with the label "enhancement"), if your platform is not available:
https://github.com/LeonWilzer/Godot-Test-FPS/issues

1. Open the .zip file.
1. Extract all the files into an empty folder.
1. Start the executable
    1. **For Windows:** You should be able to start it by simply double clicking the `Windows.exe` file.
   
    2. **For Linux:** Make the `Linux.x86_64` executable with this command: `$ chmod +x Linux.x86_64`
    Now execute the file:
        1. `$ ./Linux.x86_64`, if you are in the game's directory)
        2. _Alternatively:_ `$ x86_64 <Where "Linux.x86_64" is located>` For example: `$ x86_64 ~/Games/Godot-FPS/Linux.x86_64`

1. How to close the game

    You can't, you have to kill it with Alt + F4 on Windows or _unlock the mouse with the `ESC`_  and `$ killall Linux.x86_64` on Linux. I am sure there are also other ways of accomplishing that on both platforms.

### Experimental (requires Godot)

See below for information on how to build an experimental version from source.

## How to develop

1. Clone this repository:
`$ git clone https://github.com/LeonWilzer/Godot-Test-FPS.git`

2. Install Godot (Mono Version):
    https://godotengine.org/download/linux

3. Open Godot and import the project.
4. Open the project if it is not already opened.
5. Start the game with the play button on the top right corner.
6. Refer to Godot docs and tutorials on how to develop a game.

**Tip:** See `Editor > Editor Settings > Mono > Editor` to set a third-party IDE as your preferred editor.