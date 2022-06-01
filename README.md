# backpackhero_mods
Unofficial mods for the [Backpack Hero](https://store.steampowered.com/app/1970580/Backpack_Hero/ game


Dependencies: 
  * [Backpack Hero](https://store.steampowered.com/app/1970580/Backpack_Hero/)
  * [MelonLoader](https://github.com/LavaGang/MelonLoader)




Directory Structure: 
{ BackPack Hero Installation Directory }
   * |-- Backpack Hero_Data : the game data - the csprojs has assembly references to this folder
   * |-- MelonLoader : the prerequisite of this project - the csprojs has assembly references to this folder
   * |-- Mods : where MelonLoader loads mods from - the csprojs automatically copy the output files here
   * |-- UserLibs : dependent dlls must be copied here for MelonLoader 
   * |-- {repo} : this repository
       * |-- src : where the code is
       * |-- docs : documentations
       * |-- LICENSE : GNU GPL 3.0 
       * |-- README.md : this file
      



