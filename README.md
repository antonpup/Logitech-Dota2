# Logitech/Corsair - Dota 2 game integration
This project enables game interaction between Logitech and Corsair RGB keyboards, such as [Logitech G910](http://gaming.logitech.com/en-us/product/rgb-gaming-keyboard-g910) or [Logitech G410](http://gaming.logitech.com/en-us/product/rgb-tenkeyless-gaming-keyboard-g410), and [Dota 2](http://store.steampowered.com/app/570/) via [Game Integration](https://developer.valvesoftware.com/wiki/Counter-Strike:_Global_Offensive_Game_State_Integration). Please keep in mind that this is still in development, there may be bugs and there will be new features added.

# Requirements
* Logitech Keyboard such as: [Logitech G910](http://gaming.logitech.com/en-us/product/rgb-gaming-keyboard-g910) keyboard or [Logitech G410](http://gaming.logitech.com/en-us/product/rgb-tenkeyless-gaming-keyboard-g410) keyboard. Or any Corsair SDK supported keyboard
* [Dota 2](http://store.steampowered.com/app/570/)
* Installed [Visual C++ Redistributable Packages for Visual Studio 2013](https://www.microsoft.com/en-us/download/details.aspx?id=40784)
* Installed [Microsoft .NET Framework 4.5](https://www.microsoft.com/en-us/download/details.aspx?id=30653)

# How to Install
1. First of all, make sure that "Allow games to control illumination" is enabled in Logitech Gaming Software.
2. Download the latest release from [here](https://github.com/antonpup/Logitech-Dota2/releases/latest).
  * If you're on a 32 bit system, download x32 version. If you're on 64 bit, download x64 version.
3. Extract the archive anywhere on your computer.
4. Copy the "gamestate_integration_logitech.cfg" into ".steamapps/common/dota 2 beta/game/dota/cfg/gamestate_integration/" folder
5. Run "Logitech-Dota2.exe" (Run as admin if you have any issues.)

## Run this program in the background at windows start
1. Go to the Startup folder. (For Windows 10, press Windows Key + R and enter "shell:startup")
2. Make a shortcut to the exe in that folder.
3. Edit the shortcut by right clicking it, going into properties, and add " -silent" at the end of "Target". It should look something like this: "...\Logitech-Dota2\Logitech-Dota2.exe -silent". Then press apply, and next time your windows will automatically start the program.

## Included effects
* Team-based background color
* Health indicator
* Mana indicator
* Kill streak lighting
* Respawning effects
* Ability keys light up based on availability.
* Inventory and Stash light up based on items
* Static keys (if you wish to keep some keys lit up at all times)

## Video demonstration
[![Demo 1](http://img.youtube.com/vi/KV-doX9VsXk/0.jpg)](http://www.youtube.com/watch?v=KV-doX9VsXk)

## Screenshots
![Background](http://puu.sh/n4vGy/a0d2434c7d.png)
![HP/Mana](http://puu.sh/n4vKz/237c19db9b.png)
![Items](http://puu.sh/n4vMZ/4b836385ce.png)

# F.A.Q.
* Q: Can this give me a VACation?

   A: No. This uses Valve's [Game Integration](https://developer.valvesoftware.com/wiki/Counter-Strike:_Global_Offensive_Game_State_Integration) for CSGO, which allows developers to read game information without accessing memory of the game.

* Q: Why are Logitech G910 and G410 only supported?

   A: Logitech G910 and G410 are the only keyboards from Logitech that allow for per-key RGB lighting effects.

* Q: How come Corsair devices are also supported? Isn't this a Logitech only project?

   A: Yes and no. My primary goal is to make it work with Logitech, and any other brand support is extra. :)

* Q: Some keys in the program state that they are not supported. What is this?

   A: It means that changes to those keys are not currently possible. This is present on Logitech keyboards with G-keys and logo.
   
* Q: I have found a bug. How do I report it?

   A: You can report bugs here, by creating a new Issue [here](https://github.com/antonpup/Logitech-Dota2/issues).

* Q: I wish to expand this, fixing and adding my own features.

   A: Feel free to fork this repo and make pull requests with your own code. I am open for suggestions for both features and optimization. :)
