# UnityTilemapTileBitmask
An implementation of a Tile Bitmask feature intended to be used as a Tile type for Unity's built-in Tilemap/Tile feature-set.

Will include 4-bit and 8-bit masks for single terrain tiles.

Will include 5-bit and 9-bit masks for 2 terrain tiles.

Other features could include: 
 - Dynamically set bit depth for more complex terrain.
 - Dynamically set amount of terrain types in a given set with dynamically set bit-depth to cover all the complexities of single terrain sets.
 
 - Apply to seperately developed procedural terrain maps: 
   - Color or string identifier to mark multiple terrain types.
   - Layer number to set terrain type interactions (i.e. water and land on seperate layers that don't effect each other when placing tiles, water and land on same layer and part of same bitmask to allow them to interact when placing tiles in neighboring cells).
