// Material definitions added for the ported models (all names start with LiFx_).
// Append to the client art/materials.cs, then rename art/materials.cs.dso (or delete it) so the edited .cs is compiled.
// Texture files are not part of this repo (game art).

//////////////////////////////////////////////////////////////////////////////////////////
// LiFx workshop materials (added 2026-09-19): the custom workshop models reference these
// material names but the base game never defined them ("Cannot map material" in the log).
// Texture files placed next to a model are NOT picked up by name; a Material is required.
//////////////////////////////////////////////////////////////////////////////////////////
singleton Material(LiFx_Leather90_DIFFUSE_mat)
{
   mapTo = "Leather90_DIFFUSE";
   diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_DIFFUSE.dds";
   diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_NORMALMAP.dds";
   diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_SPECULAR.dds";
   alphaTest = "1";
   alphaRef = "33";
   normal3DC = "1";
   doubleSided = "1";
};

singleton Material(LiFx_Chain60_Vik_mat)
{
   mapTo = "Chain60_Vik";
   diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_DIFFUSE.dds";
   diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_NORMALMAP.dds";
   diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_SPECULAR.dds";
   alphaTest = "1";
   alphaRef = "33";
   normal3DC = "1";
   doubleSided = "1";
};

singleton Material(LiFx_tableBenchest_blood_diff_mat)
{
   mapTo = "tableBenchest_blood_diff";
   diffuseMap[0] = "art/Textures/Atlas/tableBenchest_blood_diff.dds";
   diffuseMap[1] = "art/Textures/Atlas/tableBenchest_nm.dds";
   diffuseMap[2] = "art/Textures/Atlas/tableBenches_spec.dds";
};

singleton Material(LiFx_jewellery_01_diff_mat)
{
   mapTo = "jewellery_01_diff";
   diffuseMap[0] = "art/models/3d/construction/craftingbonus/jeweler_workshop/jewellery_01_diff.dds";
   diffuseMap[1] = "art/models/3d/construction/craftingbonus/jeweler_workshop/jewellery_01_norm.dds";
   diffuseMap[2] = "art/models/3d/construction/craftingbonus/jeweler_workshop/jewellery_01_spec.dds";
};

singleton Material(LiFx_jewellery_02_diff_mat)
{
   mapTo = "jewellery_02_diff";
   diffuseMap[0] = "art/models/3d/construction/craftingbonus/jeweler_workshop/jewellery_02_diff.dds";
   diffuseMap[1] = "art/models/3d/construction/craftingbonus/jeweler_workshop/jewellery_02_norm.dds";
   diffuseMap[2] = "art/models/3d/construction/craftingbonus/jeweler_workshop/jewellery_02_spec.dds";
};

// Big Tanning Tub (only user of tanningtub_diff): diffuse + ATI2 normal map + specular.
// 2026-10-09: doubleSided / alphaTest / alphaRef taken from the original material definition (materials.tscript):
// the DXT5 diffuse carries alpha cut-outs (hide edges, ropes) and the hides are single planes.
singleton Material(LiFx_tanningtub_diff_mat)
{
   mapTo = "tanningtub_diff";
   diffuseMap[0] = "art/Textures/TextureLib/TanningTub_DIFFUSE.dds";
   diffuseMap[1] = "art/Textures/TextureLib/TanningTub_NORMALMAP.dds";
   diffuseMap[2] = "art/Textures/TextureLib/TanningTub_SPECULAR.dds";
   normal3DC = "1";
   doubleSided = "1";
   alphaTest = "1";
   alphaRef = "111";
   useAnisotropic[0] = "1";
   useAnisotropic[1] = "1";
   useAnisotropic[2] = "1";
   streamable = "0";
};

// Stonemason's Workplace + Ore Washer (2026-09-21): the MMO's "Devices" atlas (found in the MMO client's TextureLib), replacing the wood placeholder.
// The normal map is ATI2 (3Dc), so normal3DC=1 is required (same as the Big Tanning Tub material).
singleton Material(LiFx_mason_and_orewasher_mat)
{
   mapTo = "mason_and_orewasher";
   diffuseMap[0] = "art/Textures/TextureLib/Masonry_OreWasher_DIFFUSE.dds";
   diffuseMap[1] = "art/Textures/TextureLib/Masonry_OreWasher_NORMALMAP.dds";
   diffuseMap[2] = "art/Textures/TextureLib/Masonry_OreWasher_SPECULAR.dds";
   normal3DC = "1";
   useAnisotropic[0] = "1";
   useAnisotropic[1] = "1";
   useAnisotropic[2] = "1";
   streamable = "0";
   doubleSided = "1";
};

// Large Herbal Garden model (2026-09-19): materials it asks for that the base game never mapped.
singleton Material(LiFx_CropsAtlas_mesh_mat)
{
   mapTo = "CropsAtlas_mesh";
   diffuseColor[0] = "0.8 0.8 0.8 1";
   diffuseMap[0] = "art/Textures/GroundCover/CropsAtlas.dds";
   diffuseMap[1] = "art/Textures/GroundCover/CropsAtlas_Normal.dds";
   diffuseMap[2] = "art/Textures/GroundCover/CropsAtlas_Specular.dds";
   specularPower[0] = "10";
   pixelSpecular[0] = "1";
   useAnisotropic[0] = "1";
   doubleSided = "1";
   alphaTest = "1";
   alphaRef = "111";
};

// Christmas tree: diffuse + specular only (its normal map is ATI2, held back).
singleton Material(LiFx_ChristmasTree_mat)
{
   mapTo = "ChristmasTree";
   diffuseMap[0] = "art/Textures/TextureLib/ChristmasTree.dds";
   diffuseMap[2] = "art/Textures/TextureLib/ChristmasTree_specular.dds";
   doubleSided = "1";
   alphaTest = "1";
   alphaRef = "100";
};

// Slavic fortification materials (2026-09-21): definitions for the Arden ZK_Slavard_* materials used by the Slav_* models. Textures copied from Arden's TextureLib.
singleton Material(LiFx_ZK_Slavard_Brick_Wall)
{
   mapTo = "ZK_Slavard_Brick_Wall";
   diffuseMap[0] = "art/Textures/TextureLib/ZK_Slavard_Brick_Wall_Diffuse.dds";
   diffuseMap[1] = "art/Textures/TextureLib/ZK_Slavard_Brick_Wall_Normal.dds";
   useAnisotropic[0] = "1";
   materialTag0 = "LiF";
};

singleton Material(LiFx_ZK_Slavard_Brick_Wall_001)
{
   mapTo = "ZK_Slavard_Brick_Wall.001";
   diffuseMap[0] = "art/Textures/TextureLib/ZK_Slavard_Brick_Wall_Diffuse.dds";
   diffuseMap[1] = "art/Textures/TextureLib/ZK_Slavard_Brick_Wall_Normal.dds";
   useAnisotropic[0] = "1";
   materialTag0 = "LiF";
};

singleton Material(LiFx_ZK_Slavard_Stone_Wall)
{
   mapTo = "ZK_Slavard_Stone_Wall";
   diffuseMap[0] = "art/Textures/TextureLib/ZK_Slavard_Stone_Wall_Diffuse.dds";
   diffuseMap[1] = "art/Textures/TextureLib/ZK_Slavard_Stone_Wall_Normal.dds";
   useAnisotropic[0] = "1";
   materialTag0 = "LiF";
};

singleton Material(LiFx_ZK_Slavard_Stone_Wall_001)
{
   mapTo = "ZK_Slavard_Stone_Wall.001";
   diffuseMap[0] = "art/Textures/TextureLib/ZK_Slavard_Stone_Wall_Diffuse.dds";
   diffuseMap[1] = "art/Textures/TextureLib/ZK_Slavard_Stone_Wall_Normal.dds";
   useAnisotropic[0] = "1";
   materialTag0 = "LiF";
};

singleton Material(LiFx_ZK_Slavard_Stone_Ornaments)
{
   mapTo = "ZK_Slavard_Stone_Ornaments";
   diffuseMap[0] = "art/Textures/TextureLib/ZK_Slavard_Stone_Ornaments_Diffuse.dds";
   diffuseMap[1] = "art/Textures/TextureLib/ZK_Slavard_Stone_Ornaments_Normal.dds";
   useAnisotropic[0] = "1";
   materialTag0 = "LiF";
};

singleton Material(LiFx_ZK_Slavard_Stone_Ornaments_001)
{
   mapTo = "ZK_Slavard_Stone_Ornaments.001";
   diffuseMap[0] = "art/Textures/TextureLib/ZK_Slavard_Stone_Ornaments_Diffuse.dds";
   diffuseMap[1] = "art/Textures/TextureLib/ZK_Slavard_Stone_Ornaments_Normal.dds";
   useAnisotropic[0] = "1";
   materialTag0 = "LiF";
};

singleton Material(LiFx_ZK_Slavard_Wood_01)
{
   mapTo = "ZK_Slavard_Wood_01";
   diffuseMap[0] = "art/Textures/TextureLib/ZK_Slavard_Wood_01_diffuse.dds";
   diffuseMap[1] = "art/Textures/TextureLib/ZK_Slavard_Wood_01_normal.dds";
   diffuseMap[2] = "art/Textures/TextureLib/ZK_Slavard_Wood_01_specular.dds";
   useAnisotropic[0] = "1";
   materialTag0 = "LiF";
   doubleSided = "1";
};

singleton Material(LiFx_ZK_Slavard_Wood_01_001)
{
   mapTo = "ZK_Slavard_Wood_01.001";
   diffuseMap[0] = "art/Textures/TextureLib/ZK_Slavard_Wood_01_diffuse.dds";
   diffuseMap[1] = "art/Textures/TextureLib/ZK_Slavard_Wood_01_normal.dds";
   diffuseMap[2] = "art/Textures/TextureLib/ZK_Slavard_Wood_01_specular.dds";
   useAnisotropic[0] = "1";
   materialTag0 = "LiF";
   doubleSided = "1";
};

singleton Material(LiFx_ZK_Slavard_Wood_02)
{
   mapTo = "ZK_Slavard_Wood_02";
   diffuseMap[0] = "art/Textures/TextureLib/ZK_Slavard_Wood_02_diffuse.dds";
   diffuseMap[1] = "art/Textures/TextureLib/ZK_Slavard_Wood_02_normal.dds";
   diffuseMap[2] = "art/Textures/TextureLib/ZK_Slavard_Wood_02_specular.dds";
   useAnisotropic[0] = "1";
   materialTag0 = "LiF";
   doubleSided = "1";
};

singleton Material(LiFx_ZK_Slavard_Wood_02_001)
{
   mapTo = "ZK_Slavard_Wood_02.001";
   diffuseMap[0] = "art/Textures/TextureLib/ZK_Slavard_Wood_02_diffuse.dds";
   diffuseMap[1] = "art/Textures/TextureLib/ZK_Slavard_Wood_02_normal.dds";
   diffuseMap[2] = "art/Textures/TextureLib/ZK_Slavard_Wood_02_specular.dds";
   useAnisotropic[0] = "1";
   materialTag0 = "LiF";
   doubleSided = "1";
};

singleton Material(LiFx_ZK_Slavard_Wood_03)
{
   mapTo = "ZK_Slavard_Wood_03";
   diffuseMap[0] = "art/Textures/TextureLib/ZK_Slavard_Wood_03_diffuse.dds";
   diffuseMap[1] = "art/Textures/TextureLib/ZK_Slavard_Wood_03_normal.dds";
   diffuseMap[2] = "art/Textures/TextureLib/ZK_Slavard_Wood_03_specular.dds";
   useAnisotropic[0] = "1";
   materialTag0 = "LiF";
   doubleSided = "1";
};

singleton Material(LiFx_ZK_Slavard_Wood_03_001)
{
   mapTo = "ZK_Slavard_Wood_03.001";
   diffuseMap[0] = "art/Textures/TextureLib/ZK_Slavard_Wood_03_diffuse.dds";
   diffuseMap[1] = "art/Textures/TextureLib/ZK_Slavard_Wood_03_normal.dds";
   diffuseMap[2] = "art/Textures/TextureLib/ZK_Slavard_Wood_03_specular.dds";
   useAnisotropic[0] = "1";
   materialTag0 = "LiF";
   doubleSided = "1";
};

singleton Material(LiFx_ZK_Slavard_Wood_Floor)
{
   mapTo = "ZK_Slavard_Wood_Floor";
   diffuseMap[0] = "art/Textures/TextureLib/ZK_Slavard_Wood_Floor_Diffuse.dds";
   diffuseMap[1] = "art/Textures/TextureLib/ZK_Slavard_Wood_Floor_Normal.dds";
   useAnisotropic[0] = "1";
   materialTag0 = "LiF";
   doubleSided = "1";
};

singleton Material(LiFx_ZK_Slavard_Wood_Floor_001)
{
   mapTo = "ZK_Slavard_Wood_Floor.001";
   diffuseMap[0] = "art/Textures/TextureLib/ZK_Slavard_Wood_Floor_Diffuse.dds";
   diffuseMap[1] = "art/Textures/TextureLib/ZK_Slavard_Wood_Floor_Normal.dds";
   useAnisotropic[0] = "1";
   materialTag0 = "LiF";
   doubleSided = "1";
};

singleton Material(LiFx_ZK_Slavard_Wood_Ornaments)
{
   mapTo = "ZK_Slavard_Wood_Ornaments";
   diffuseMap[0] = "art/Textures/TextureLib/ZK_Slavard_Wood_Ornaments_Diffuse.dds";
   diffuseMap[1] = "art/Textures/TextureLib/ZK_Slavard_Wood_Ornaments_Normal.dds";
   useAnisotropic[0] = "1";
   materialTag0 = "LiF";
   doubleSided = "1";
};

singleton Material(LiFx_ZK_Slavard_Wood_Ornaments_001)
{
   mapTo = "ZK_Slavard_Wood_Ornaments.001";
   diffuseMap[0] = "art/Textures/TextureLib/ZK_Slavard_Wood_Ornaments_Diffuse.dds";
   diffuseMap[1] = "art/Textures/TextureLib/ZK_Slavard_Wood_Ornaments_Normal.dds";
   useAnisotropic[0] = "1";
   materialTag0 = "LiF";
   doubleSided = "1";
};

singleton Material(LiFx_ZK_Slavard_Wood_Roof_01)
{
   mapTo = "ZK_Slavard_Wood_Roof_01";
   diffuseMap[0] = "art/Textures/TextureLib/ZK_Slavard_Wood_roof_01_diffuse.dds";
   useAnisotropic[0] = "1";
   materialTag0 = "LiF";
   doubleSided = "1";
};

singleton Material(LiFx_ZK_Slavard_Wood_Roof_01_001)
{
   mapTo = "ZK_Slavard_Wood_Roof_01.001";
   diffuseMap[0] = "art/Textures/TextureLib/ZK_Slavard_Wood_roof_01_diffuse.dds";
   useAnisotropic[0] = "1";
   materialTag0 = "LiF";
   doubleSided = "1";
};

// Compost Heap (2026-09-21): Arden's Manure_diff (the normal map manure_norm.dds has a DX10 header and is left unused).
singleton Material(LiFx_Manure_diff_mat)
{
   mapTo = "Manure_diff";
   diffuseMap[0] = "art/Textures/TextureLib/Manure_diff.dds";
   materialTag0 = "LiF";
};

// Grain / Ore Stockpile materials (2026-09-21). Arden defines these in scripts we can't read, so they are mapped by best guess to the matching textures.
singleton Material(LiFx_Stock_furniture_pack4)
{
   mapTo = "furniture_pack4";
   diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack4_DIFFUSE.dds";
   diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack4_NORMALMAP.dds";
   diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack4_SPECULAR.dds";
   normal3DC = "1";
   doubleSided = "1";
   materialTag0 = "LiF";
};

singleton Material(LiFx_Stock_TribeCAMP_Totem_Cloth_v3)
{
   mapTo = "TribeCAMP_Totem_Cloth_v3";
   diffuseMap[0] = "art/Textures/TextureLib/TribeCAMP_Totem_v3_Cloth_DIFFUSE.dds";
   diffuseMap[1] = "art/Textures/TextureLib/TribeCAMP_Totem_v3_Cloth_NORMALMAP.dds";
   diffuseMap[2] = "art/Textures/TextureLib/TribeCAMP_Totem_v3_Cloth_SPECULAR.dds";
   normal3DC = "1";
   doubleSided = "1";
   materialTag0 = "LiF";
};

singleton Material(LiFx_Stock_TribeCAMP_Totem_Stone_v3)
{
   mapTo = "TribeCAMP_Totem_Stone_v3";
   diffuseMap[0] = "art/Textures/TextureLib/TribeCAMP_Totem_v3_Stone_DIFFUSE.dds";
   diffuseMap[1] = "art/Textures/TextureLib/TribeCAMP_Totem_v3_Stone_NORMALMAP.dds";
   diffuseMap[2] = "art/Textures/TextureLib/TribeCAMP_Totem_v3_Stone_SPECULAR.dds";
   normal3DC = "1";
   materialTag0 = "LiF";
};

singleton Material(LiFx_Stock_TribeCamp_Cloth_v4)
{
   mapTo = "TribeCamp_Cloth_v4";
   diffuseMap[0] = "art/Textures/TextureLib/TribeCAMP_Totem_v4_Native_Cloth_DIFFUSE.dds";
   diffuseMap[1] = "art/Textures/TextureLib/TribeCAMP_Totem_v4_Native_Cloth_NORMALMAP.dds";
   diffuseMap[2] = "art/Textures/TextureLib/TribeCAMP_Totem_v4_Native_Cloth_SPECULAR.dds";
   normal3DC = "1";
   doubleSided = "1";
   materialTag0 = "LiF";
};

singleton Material(LiFx_Stock_iron_ore_frag_01_skin)
{
   mapTo = "iron_ore_frag_01_skin";
   diffuseMap[0] = "art/2D/Terrain/Substances/Iron_ore/Iron_Ore_diff.dds";
   diffuseMap[1] = "art/2D/Terrain/Substances/Rock_Fragments/Rock_Frag_nm.DDS";
   materialTag0 = "LiF";
};

singleton Material(LiFx_Stock_Pilaster)
{
   mapTo = "Pilaster";
   diffuseMap[0] = "art/Textures/TextureLib/PilasterB_diff.dds";
   diffuseMap[1] = "art/Textures/TextureLib/PilasterB_nm.dds";
   diffuseMap[2] = "art/Textures/TextureLib/PilasterB_spec.dds";
   materialTag0 = "LiF";
};

singleton Material(LiFx_Stock_Rivet)
{
   mapTo = "Rivet";
   diffuseMap[0] = "art/Textures/TextureLib/rivet_diff.dds";
   diffuseMap[1] = "art/Textures/TextureLib/rivet_nm.dds";
   materialTag0 = "LiF";
};

singleton Material(LiFx_Stock_Oil)
{
   mapTo = "Oil";
   diffuseMap[0] = "art/Textures/TextureLib/Oil_LinenOil_diffuse.dds";
   diffuseMap[1] = "art/Textures/TextureLib/Oil_LinenOil_Normal.dds";
   diffuseMap[2] = "art/Textures/TextureLib/Oil_LinenOil_Specular.dds";
   materialTag0 = "LiF";
};

singleton Material(LiFx_Stock_Rabbit_DIFFUSE_noskin)
{
   mapTo = "Rabbit_DIFFUSE_noskin";
   diffuseMap[0] = "art/Textures/Animals/hare/Rabbit_DIFFUSE.dds";
   diffuseMap[1] = "art/Textures/Animals/hare/Rabbit_NORMAL.dds";
   materialTag0 = "LiF";
};

singleton Material(LiFx_Stock_TribeCAMP_Totem_Detail_v3)
{
   mapTo = "TribeCAMP_Totem_Detail_v3";
   diffuseMap[0] = "art/Textures/TextureLib/TribeCAMP_Totem_v3_Detail_DIFFUSE.dds";
   diffuseMap[1] = "art/Textures/TextureLib/TribeCAMP_Totem_v3_Detail_NORMALMAP.dds";
   diffuseMap[2] = "art/Textures/TextureLib/TribeCAMP_Totem_v3_Detail_SPECULAR.dds";
   normal3DC = "1";
   doubleSided = "1";
   materialTag0 = "LiF";
};

singleton Material(LiFx_Stock_TribeCAMP_Totem_Tree_v3C)
{
   mapTo = "TribeCAMP_Totem_Tree_v3C";
   diffuseMap[0] = "art/Textures/TextureLib/TribeCAMP_Totem_v3_Tree_DIFFUSE.dds";
   diffuseMap[1] = "art/Textures/TextureLib/TribeCAMP_Totem_v3_Tree_NORMALMAP.dds";
   diffuseMap[2] = "art/Textures/TextureLib/TribeCAMP_Totem_v3_Tree_SPECULAR.dds";
   normal3DC = "1";
   materialTag0 = "LiF";
};

singleton Material(LiFx_Stock_TribeCamp_Detail_v4)
{
   mapTo = "TribeCamp_Detail_v4";
   diffuseMap[0] = "art/Textures/TextureLib/TribeCAMP_Totem_v4_Detail_DIFFUSE.dds";
   diffuseMap[1] = "art/Textures/TextureLib/TribeCAMP_Totem_v4_Detail_NORMALMAP.dds";
   diffuseMap[2] = "art/Textures/TextureLib/TribeCAMP_Totem_v4_Detail_SPECULAR.dds";
   normal3DC = "1";
   doubleSided = "1";
   materialTag0 = "LiF";
};

singleton Material(LiFx_Stock_TribeCamp_Hangman_v4)
{
   mapTo = "TribeCamp_Hangman_v4";
   diffuseMap[0] = "art/Textures/TextureLib/TribeCAMP_Totem_v4_Hangman_DIFFUSE.dds";
   diffuseMap[1] = "art/Textures/TextureLib/TribeCAMP_Totem_v4_Hangman_NORMALMAP.dds";
   diffuseMap[2] = "art/Textures/TextureLib/TribeCAMP_Totem_v4_Hangman_SPECULAR.dds";
   normal3DC = "1";
   doubleSided = "1";
   materialTag0 = "LiF";
};

singleton Material(LiFx_Stock_TribeCamp_Stone_v4)
{
   mapTo = "TribeCamp_Stone_v4";
   diffuseMap[0] = "art/Textures/TextureLib/TribeCAMP_Totem_v4_Native_Stone_DIFFUSE.dds";
   diffuseMap[1] = "art/Textures/TextureLib/TribeCAMP_Totem_v4_Native_Stone_NORMALMAP.dds";
   diffuseMap[2] = "art/Textures/TextureLib/TribeCAMP_Totem_v4_Native_Stone_SPECULAR.dds";
   normal3DC = "1";
   materialTag0 = "LiF";
};

singleton Material(LiFx_Stock_TribeCamp_Tree_v4C)
{
   mapTo = "TribeCamp_Tree_v4C";
   diffuseMap[0] = "art/Textures/TextureLib/TribeCAMP_Totem_v4_Native_Tree_DIFFUSE.dds";
   diffuseMap[1] = "art/Textures/TextureLib/TribeCAMP_Totem_v4_Native_Tree_NORMALMAP.dds";
   diffuseMap[2] = "art/Textures/TextureLib/TribeCAMP_Totem_v4_Native_Tree_SPECULAR.dds";
   normal3DC = "1";
   materialTag0 = "LiF";
};

singleton Material(LiFx_Stock_TribeCAMP_Totem_Tree_v3)
{
   mapTo = "TribeCAMP_Totem_Tree_v3";
   diffuseMap[0] = "art/Textures/TextureLib/TribeCAMP_Totem_v3_Tree_DIFFUSE.dds";
   diffuseMap[1] = "art/Textures/TextureLib/TribeCAMP_Totem_v3_Tree_NORMALMAP.dds";
   diffuseMap[2] = "art/Textures/TextureLib/TribeCAMP_Totem_v3_Tree_SPECULAR.dds";
   normal3DC = "1";
   materialTag0 = "LiF";
};

singleton Material(LiFx_Stock_TribeCamp_Tree_v4)
{
   mapTo = "TribeCamp_Tree_v4";
   diffuseMap[0] = "art/Textures/TextureLib/TribeCAMP_Totem_v4_Native_Tree_DIFFUSE.dds";
   diffuseMap[1] = "art/Textures/TextureLib/TribeCAMP_Totem_v4_Native_Tree_NORMALMAP.dds";
   diffuseMap[2] = "art/Textures/TextureLib/TribeCAMP_Totem_v4_Native_Tree_SPECULAR.dds";
   normal3DC = "1";
   materialTag0 = "LiF";
};

singleton Material(LiFx_AI_Altar_Statue_Ferryman)
{
    mapTo = "Altar_Statue_Ferryman";
    diffuseMap[0] = "art/Textures/TextureLib/Altar_Statues_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Altar_Statues_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Altar_Statues_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_AI_StoneFerryman)
{
    mapTo = "StoneFerryman";
    diffuseMap[0] = "art/Textures/TextureLib/Altar_Stones_colored_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Altar_Stones_colored_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Altar_Stones_colored_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_AI_Altar_Statue_Ferryman70)
{
    mapTo = "Altar_Statue_Ferryman70";
    diffuseMap[0] = "art/Textures/TextureLib/Altar_Statues_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Altar_Statues_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Altar_Statues_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_AI_StoneFerryman70)
{
    mapTo = "StoneFerryman70";
    diffuseMap[0] = "art/Textures/TextureLib/Altar_Stones_colored_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Altar_Stones_colored_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Altar_Stones_colored_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_AI_Altar_Statue_Ferryman40)
{
    mapTo = "Altar_Statue_Ferryman40";
    diffuseMap[0] = "art/Textures/TextureLib/Altar_Statues_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Altar_Statues_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Altar_Statues_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_AI_StoneFerryman40)
{
    mapTo = "StoneFerryman40";
    diffuseMap[0] = "art/Textures/TextureLib/Altar_Stones_colored_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Altar_Stones_colored_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Altar_Stones_colored_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_AI_Altar_Statue_Ferryman5)
{
    mapTo = "Altar_Statue_Ferryman5";
    diffuseMap[0] = "art/Textures/TextureLib/Altar_Statues_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Altar_Statues_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Altar_Statues_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_AI_StoneFerryman5)
{
    mapTo = "StoneFerryman5";
    diffuseMap[0] = "art/Textures/TextureLib/Altar_Stones_colored_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Altar_Stones_colored_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Altar_Stones_colored_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_AI_altar_statue_colored_packF)
{
    mapTo = "altar_statue_colored_packF";
    diffuseMap[0] = "art/Textures/TextureLib/Altar_Statues_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Altar_Statues_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Altar_Statues_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_AI_altar_stone_colored_packF)
{
    mapTo = "altar_stone_colored_packF";
    diffuseMap[0] = "art/Textures/TextureLib/Altar_Stones_Part2_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Altar_Stones_Part2_NORMAMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Altar_Stones_Part2_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_AI_Altar_Statue_Rob)
{
    mapTo = "Altar_Statue_Rob";
    diffuseMap[0] = "art/Textures/TextureLib/Altar_Statues_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Altar_Statues_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Altar_Statues_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_AI_StoneRob)
{
    mapTo = "StoneRob";
    diffuseMap[0] = "art/Textures/TextureLib/Altar_Stones_colored_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Altar_Stones_colored_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Altar_Stones_colored_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_AI_Altar_Statue_Rob70)
{
    mapTo = "Altar_Statue_Rob70";
    diffuseMap[0] = "art/Textures/TextureLib/Altar_Statues_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Altar_Statues_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Altar_Statues_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_AI_StoneRob70)
{
    mapTo = "StoneRob70";
    diffuseMap[0] = "art/Textures/TextureLib/Altar_Stones_colored_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Altar_Stones_colored_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Altar_Stones_colored_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_AI_Altar_Statue_Rob40)
{
    mapTo = "Altar_Statue_Rob40";
    diffuseMap[0] = "art/Textures/TextureLib/Altar_Statues_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Altar_Statues_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Altar_Statues_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_AI_StoneRob40)
{
    mapTo = "StoneRob40";
    diffuseMap[0] = "art/Textures/TextureLib/Altar_Stones_colored_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Altar_Stones_colored_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Altar_Stones_colored_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_AI_Altar_Statue_Rob5)
{
    mapTo = "Altar_Statue_Rob5";
    diffuseMap[0] = "art/Textures/TextureLib/Altar_Statues_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Altar_Statues_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Altar_Statues_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_AI_StoneRob5)
{
    mapTo = "StoneRob5";
    diffuseMap[0] = "art/Textures/TextureLib/Altar_Stones_colored_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Altar_Stones_colored_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Altar_Stones_colored_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_AI_altar_statue_colored_packR)
{
    mapTo = "altar_statue_colored_packR";
    diffuseMap[0] = "art/Textures/TextureLib/Altar_Statues_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Altar_Statues_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Altar_Statues_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_AI_altar_stone_colored_packR)
{
    mapTo = "altar_stone_colored_packR";
    diffuseMap[0] = "art/Textures/TextureLib/Altar_Stones_Part2_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Altar_Stones_Part2_NORMAMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Altar_Stones_Part2_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_AI_Altar_Statue_Sofapek)
{
    mapTo = "Altar_Statue_Sofapek";
    diffuseMap[0] = "art/Textures/TextureLib/Altar_Statues_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Altar_Statues_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Altar_Statues_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_AI_StoneSofapek)
{
    mapTo = "StoneSofapek";
    diffuseMap[0] = "art/Textures/TextureLib/Altar_Stones_colored_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Altar_Stones_colored_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Altar_Stones_colored_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_AI_Altar_Statue_Sofapek70)
{
    mapTo = "Altar_Statue_Sofapek70";
    diffuseMap[0] = "art/Textures/TextureLib/Altar_Statues_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Altar_Statues_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Altar_Statues_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_AI_StoneSofapek70)
{
    mapTo = "StoneSofapek70";
    diffuseMap[0] = "art/Textures/TextureLib/Altar_Stones_colored_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Altar_Stones_colored_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Altar_Stones_colored_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_AI_Altar_Statue_Sofapek40)
{
    mapTo = "Altar_Statue_Sofapek40";
    diffuseMap[0] = "art/Textures/TextureLib/Altar_Statues_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Altar_Statues_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Altar_Statues_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_AI_StoneSofapek40)
{
    mapTo = "StoneSofapek40";
    diffuseMap[0] = "art/Textures/TextureLib/Altar_Stones_colored_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Altar_Stones_colored_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Altar_Stones_colored_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_AI_Altar_Statue_Sofapek5)
{
    mapTo = "Altar_Statue_Sofapek5";
    diffuseMap[0] = "art/Textures/TextureLib/Altar_Statues_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Altar_Statues_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Altar_Statues_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_AI_StoneSofapek5)
{
    mapTo = "StoneSofapek5";
    diffuseMap[0] = "art/Textures/TextureLib/Altar_Stones_colored_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Altar_Stones_colored_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Altar_Stones_colored_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_AI_altar_statue_colored_packS)
{
    mapTo = "altar_statue_colored_packS";
    diffuseMap[0] = "art/Textures/TextureLib/Altar_Statues_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Altar_Statues_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Altar_Statues_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_AI_altar_stone_colored_packS)
{
    mapTo = "altar_stone_colored_packS";
    diffuseMap[0] = "art/Textures/TextureLib/Altar_Stones_Part2_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Altar_Stones_Part2_NORMAMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Altar_Stones_Part2_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_AI_Altar_Statue_Terskel)
{
    mapTo = "Altar_Statue_Terskel";
    diffuseMap[0] = "art/Textures/TextureLib/Altar_Statues_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Altar_Statues_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Altar_Statues_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_AI_StoneTerskel)
{
    mapTo = "StoneTerskel";
    diffuseMap[0] = "art/Textures/TextureLib/Altar_Stones_colored_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Altar_Stones_colored_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Altar_Stones_colored_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_AI_Altar_Statue_Terskel70)
{
    mapTo = "Altar_Statue_Terskel70";
    diffuseMap[0] = "art/Textures/TextureLib/Altar_Statues_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Altar_Statues_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Altar_Statues_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_AI_StoneTerskel70)
{
    mapTo = "StoneTerskel70";
    diffuseMap[0] = "art/Textures/TextureLib/Altar_Stones_colored_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Altar_Stones_colored_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Altar_Stones_colored_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_AI_Altar_Statue_Terskel40)
{
    mapTo = "Altar_Statue_Terskel40";
    diffuseMap[0] = "art/Textures/TextureLib/Altar_Statues_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Altar_Statues_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Altar_Statues_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_AI_StoneTerskel40)
{
    mapTo = "StoneTerskel40";
    diffuseMap[0] = "art/Textures/TextureLib/Altar_Stones_colored_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Altar_Stones_colored_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Altar_Stones_colored_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_AI_Altar_Statue_Terskel5)
{
    mapTo = "Altar_Statue_Terskel5";
    diffuseMap[0] = "art/Textures/TextureLib/Altar_Statues_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Altar_Statues_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Altar_Statues_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_AI_StoneTerskel5)
{
    mapTo = "StoneTerskel5";
    diffuseMap[0] = "art/Textures/TextureLib/Altar_Stones_colored_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Altar_Stones_colored_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Altar_Stones_colored_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_AI_altar_statue_colored_packT)
{
    mapTo = "altar_statue_colored_packT";
    diffuseMap[0] = "art/Textures/TextureLib/Altar_Statues_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Altar_Statues_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Altar_Statues_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_AI_altar_stone_colored_packT)
{
    mapTo = "altar_stone_colored_packT";
    diffuseMap[0] = "art/Textures/TextureLib/Altar_Stones_Part2_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Altar_Stones_Part2_NORMAMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Altar_Stones_Part2_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_Sofapek_Terskel_TEST)
{
    mapTo = "Sofapek_Terskel_TEST_DIFFUSE";
    diffuseMap[0] = "art/Textures/TextureLib/Sofapek_Terskel_TEST_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Sofapek_Terskel_TEST_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Sofapek_Terskel_TEST_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_RoadSign_A)
{
    mapTo = "RoadSign_A";
    diffuseMap[0] = "art/Textures/TextureLib/RoadSign_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/RoadSign_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/RoadSign_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_RoadSign_A5)
{
    mapTo = "RoadSign_A5";
    diffuseMap[0] = "art/Textures/TextureLib/RoadSign_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/RoadSign_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/RoadSign_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_RoadSign_A40)
{
    mapTo = "RoadSign_A40";
    diffuseMap[0] = "art/Textures/TextureLib/RoadSign_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/RoadSign_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/RoadSign_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_RoadSign_A70)
{
    mapTo = "RoadSign_A70";
    diffuseMap[0] = "art/Textures/TextureLib/RoadSign_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/RoadSign_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/RoadSign_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_RoadSign_A_Arrow)
{
    mapTo = "RoadSign_A_Arrow";
    diffuseMap[0] = "art/Textures/TextureLib/RoadSign_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/RoadSign_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/RoadSign_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_RoadSign_A_Arrow70)
{
    mapTo = "RoadSign_A_Arrow70";
    diffuseMap[0] = "art/Textures/TextureLib/RoadSign_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/RoadSign_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/RoadSign_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_RoadSign_B)
{
    mapTo = "RoadSign_B";
    diffuseMap[0] = "art/Textures/TextureLib/RoadSign_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/RoadSign_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/RoadSign_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_RoadSign_B5)
{
    mapTo = "RoadSign_B5";
    diffuseMap[0] = "art/Textures/TextureLib/RoadSign_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/RoadSign_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/RoadSign_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_RoadSign_B40)
{
    mapTo = "RoadSign_B40";
    diffuseMap[0] = "art/Textures/TextureLib/RoadSign_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/RoadSign_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/RoadSign_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_RoadSign_B70)
{
    mapTo = "RoadSign_B70";
    diffuseMap[0] = "art/Textures/TextureLib/RoadSign_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/RoadSign_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/RoadSign_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_RoadSign_B_Arrow)
{
    mapTo = "RoadSign_B_Arrow";
    diffuseMap[0] = "art/Textures/TextureLib/RoadSign_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/RoadSign_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/RoadSign_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_RoadSign_B_Arrow70)
{
    mapTo = "RoadSign_B_Arrow70";
    diffuseMap[0] = "art/Textures/TextureLib/RoadSign_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/RoadSign_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/RoadSign_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_RoadSign_C)
{
    mapTo = "RoadSign_C";
    diffuseMap[0] = "art/Textures/TextureLib/RoadSign_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/RoadSign_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/RoadSign_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_RoadSign_C5)
{
    mapTo = "RoadSign_C5";
    diffuseMap[0] = "art/Textures/TextureLib/RoadSign_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/RoadSign_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/RoadSign_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_RoadSign_C40)
{
    mapTo = "RoadSign_C40";
    diffuseMap[0] = "art/Textures/TextureLib/RoadSign_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/RoadSign_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/RoadSign_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_RoadSign_C70)
{
    mapTo = "RoadSign_C70";
    diffuseMap[0] = "art/Textures/TextureLib/RoadSign_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/RoadSign_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/RoadSign_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_RoadSign_C_Arrow)
{
    mapTo = "RoadSign_C_Arrow";
    diffuseMap[0] = "art/Textures/TextureLib/RoadSign_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/RoadSign_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/RoadSign_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_RoadSign_C_Arrow70)
{
    mapTo = "RoadSign_C_Arrow70";
    diffuseMap[0] = "art/Textures/TextureLib/RoadSign_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/RoadSign_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/RoadSign_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_RoadSign_D)
{
    mapTo = "RoadSign_D";
    diffuseMap[0] = "art/Textures/TextureLib/RoadSign_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/RoadSign_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/RoadSign_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_RoadSign_D5)
{
    mapTo = "RoadSign_D5";
    diffuseMap[0] = "art/Textures/TextureLib/RoadSign_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/RoadSign_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/RoadSign_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_RoadSign_D40)
{
    mapTo = "RoadSign_D40";
    diffuseMap[0] = "art/Textures/TextureLib/RoadSign_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/RoadSign_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/RoadSign_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_RoadSign_D70)
{
    mapTo = "RoadSign_D70";
    diffuseMap[0] = "art/Textures/TextureLib/RoadSign_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/RoadSign_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/RoadSign_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_RoadSign_D_Arrow)
{
    mapTo = "RoadSign_D_Arrow";
    diffuseMap[0] = "art/Textures/TextureLib/RoadSign_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/RoadSign_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/RoadSign_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_RoadSign_D_Arrow70)
{
    mapTo = "RoadSign_D_Arrow70";
    diffuseMap[0] = "art/Textures/TextureLib/RoadSign_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/RoadSign_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/RoadSign_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_RoadSign_E)
{
    mapTo = "RoadSign_E";
    diffuseMap[0] = "art/Textures/TextureLib/RoadSign_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/RoadSign_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/RoadSign_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_RoadSign_E5)
{
    mapTo = "RoadSign_E5";
    diffuseMap[0] = "art/Textures/TextureLib/RoadSign_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/RoadSign_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/RoadSign_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_RoadSign_E40)
{
    mapTo = "RoadSign_E40";
    diffuseMap[0] = "art/Textures/TextureLib/RoadSign_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/RoadSign_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/RoadSign_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_RoadSign_E70)
{
    mapTo = "RoadSign_E70";
    diffuseMap[0] = "art/Textures/TextureLib/RoadSign_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/RoadSign_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/RoadSign_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_RoadSign_E_Arrow)
{
    mapTo = "RoadSign_E_Arrow";
    diffuseMap[0] = "art/Textures/TextureLib/RoadSign_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/RoadSign_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/RoadSign_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_RoadSign_E_Arrow70)
{
    mapTo = "RoadSign_E_Arrow70";
    diffuseMap[0] = "art/Textures/TextureLib/RoadSign_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/RoadSign_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/RoadSign_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_RoadSign_DIFFUSE)
{
    mapTo = "RoadSign_DIFFUSE";
    diffuseMap[0] = "art/Textures/TextureLib/RoadSign_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/RoadSign_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/RoadSign_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Bardiche_base)
{
    mapTo = "Bardiche_base";
    diffuseMap[0] = "art/Textures/Weapons/Bardiche_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Bardiche_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Bardiche_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Bardiche_base10)
{
    mapTo = "Bardiche_base10";
    diffuseMap[0] = "art/Textures/Weapons/Bardiche_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Bardiche_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Bardiche_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Bardiche_base100)
{
    mapTo = "Bardiche_base100";
    diffuseMap[0] = "art/Textures/Weapons/Bardiche_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Bardiche_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Bardiche_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Bardiche_base200)
{
    mapTo = "Bardiche_base200";
    diffuseMap[0] = "art/Textures/Weapons/Bardiche_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Bardiche_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Bardiche_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Bardiche_base500)
{
    mapTo = "Bardiche_base500";
    diffuseMap[0] = "art/Textures/Weapons/Bardiche_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Bardiche_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Bardiche_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Bardiche_pack1_ns)
{
    mapTo = "Bardiche_pack1_ns";
    diffuseMap[0] = "art/Textures/Weapons/Bardiche_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Bardiche_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Bardiche_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Bardiche_SkinA)
{
    mapTo = "Bardiche_SkinA";
    diffuseMap[0] = "art/Textures/Weapons/Bardiche_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Bardiche_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Bardiche_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Bardiche_SkinA10)
{
    mapTo = "Bardiche_SkinA10";
    diffuseMap[0] = "art/Textures/Weapons/Bardiche_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Bardiche_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Bardiche_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Bardiche_SkinA100)
{
    mapTo = "Bardiche_SkinA100";
    diffuseMap[0] = "art/Textures/Weapons/Bardiche_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Bardiche_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Bardiche_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Bardiche_SkinA200)
{
    mapTo = "Bardiche_SkinA200";
    diffuseMap[0] = "art/Textures/Weapons/Bardiche_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Bardiche_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Bardiche_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Bardiche_SkinA500)
{
    mapTo = "Bardiche_SkinA500";
    diffuseMap[0] = "art/Textures/Weapons/Bardiche_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Bardiche_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Bardiche_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BattleAxe_Base)
{
    mapTo = "BattleAxe_Base";
    diffuseMap[0] = "art/Textures/Weapons/BattleAxe_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BattleAxe_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BattleAxe_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BattleAxe_Base10)
{
    mapTo = "BattleAxe_Base10";
    diffuseMap[0] = "art/Textures/Weapons/BattleAxe_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BattleAxe_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BattleAxe_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BattleAxe_Base100)
{
    mapTo = "BattleAxe_Base100";
    diffuseMap[0] = "art/Textures/Weapons/BattleAxe_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BattleAxe_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BattleAxe_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BattleAxe_Base200)
{
    mapTo = "BattleAxe_Base200";
    diffuseMap[0] = "art/Textures/Weapons/BattleAxe_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BattleAxe_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BattleAxe_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BattleAxe_Base500)
{
    mapTo = "BattleAxe_Base500";
    diffuseMap[0] = "art/Textures/Weapons/BattleAxe_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BattleAxe_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BattleAxe_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BattleAxe_BaseI)
{
    mapTo = "BattleAxe_BaseI";
    diffuseMap[0] = "art/Textures/Weapons/BattleAxe_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BattleAxe_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BattleAxe_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BattleAxe_BaseI10)
{
    mapTo = "BattleAxe_BaseI10";
    diffuseMap[0] = "art/Textures/Weapons/BattleAxe_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BattleAxe_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BattleAxe_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BattleAxe_BaseI100)
{
    mapTo = "BattleAxe_BaseI100";
    diffuseMap[0] = "art/Textures/Weapons/BattleAxe_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BattleAxe_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BattleAxe_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BattleAxe_BaseI200)
{
    mapTo = "BattleAxe_BaseI200";
    diffuseMap[0] = "art/Textures/Weapons/BattleAxe_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BattleAxe_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BattleAxe_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BattleAxe_BaseI500)
{
    mapTo = "BattleAxe_BaseI500";
    diffuseMap[0] = "art/Textures/Weapons/BattleAxe_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BattleAxe_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BattleAxe_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Battleaxe_Skin)
{
    mapTo = "Battleaxe_Skin";
    diffuseMap[0] = "art/Textures/Weapons/BattleAxe_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BattleAxe_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BattleAxe_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BattleAxe_SkinA)
{
    mapTo = "BattleAxe_SkinA";
    diffuseMap[0] = "art/Textures/Weapons/BattleAxe_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BattleAxe_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BattleAxe_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BattleAxe_SkinA10)
{
    mapTo = "BattleAxe_SkinA10";
    diffuseMap[0] = "art/Textures/Weapons/BattleAxe_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BattleAxe_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BattleAxe_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BattleAxe_SkinA100)
{
    mapTo = "BattleAxe_SkinA100";
    diffuseMap[0] = "art/Textures/Weapons/BattleAxe_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BattleAxe_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BattleAxe_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BattleAxe_SkinA200)
{
    mapTo = "BattleAxe_SkinA200";
    diffuseMap[0] = "art/Textures/Weapons/BattleAxe_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BattleAxe_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BattleAxe_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BattleAxe_SkinA500)
{
    mapTo = "BattleAxe_SkinA500";
    diffuseMap[0] = "art/Textures/Weapons/BattleAxe_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BattleAxe_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BattleAxe_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Battleaxe_SkinC)
{
    mapTo = "Battleaxe_SkinC";
    diffuseMap[0] = "art/Textures/Weapons/BattleAxe_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BattleAxe_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BattleAxe_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BattleAxe_SkinC10)
{
    mapTo = "BattleAxe_SkinC10";
    diffuseMap[0] = "art/Textures/Weapons/BattleAxe_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BattleAxe_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BattleAxe_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BattleAxe_SkinC100)
{
    mapTo = "BattleAxe_SkinC100";
    diffuseMap[0] = "art/Textures/Weapons/BattleAxe_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BattleAxe_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BattleAxe_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Battleaxe_SkinC200)
{
    mapTo = "Battleaxe_SkinC200";
    diffuseMap[0] = "art/Textures/Weapons/BattleAxe_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BattleAxe_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BattleAxe_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BattleAxe_SkinC500)
{
    mapTo = "BattleAxe_SkinC500";
    diffuseMap[0] = "art/Textures/Weapons/BattleAxe_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BattleAxe_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BattleAxe_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BattleAxe_Skins_diffC)
{
    mapTo = "BattleAxe_Skins_diffC";
    diffuseMap[0] = "art/Textures/Weapons/BattleAxe_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BattleAxe_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BattleAxe_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BigFalchion_SkinA)
{
    mapTo = "BigFalchion_SkinA";
    diffuseMap[0] = "art/Textures/Weapons/BigFalchion_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BigFalchion_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BigFalchion_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BigFalchion_SkinA10)
{
    mapTo = "BigFalchion_SkinA10";
    diffuseMap[0] = "art/Textures/Weapons/BigFalchion_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BigFalchion_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BigFalchion_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BigFalchion_SkinA100)
{
    mapTo = "BigFalchion_SkinA100";
    diffuseMap[0] = "art/Textures/Weapons/BigFalchion_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BigFalchion_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BigFalchion_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BigFalchion_SkinA200)
{
    mapTo = "BigFalchion_SkinA200";
    diffuseMap[0] = "art/Textures/Weapons/BigFalchion_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BigFalchion_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BigFalchion_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BigFalchion_SkinA500)
{
    mapTo = "BigFalchion_SkinA500";
    diffuseMap[0] = "art/Textures/Weapons/BigFalchion_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BigFalchion_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BigFalchion_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BoarSpear_Base)
{
    mapTo = "BoarSpear_Base";
    diffuseMap[0] = "art/Textures/Weapons/BoarSpear_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BoarSpear_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BoarSpear_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BoarSpear_Base10)
{
    mapTo = "BoarSpear_Base10";
    diffuseMap[0] = "art/Textures/Weapons/BoarSpear_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BoarSpear_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BoarSpear_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BoarSpear_Base100)
{
    mapTo = "BoarSpear_Base100";
    diffuseMap[0] = "art/Textures/Weapons/BoarSpear_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BoarSpear_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BoarSpear_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BoarSpear_Base200)
{
    mapTo = "BoarSpear_Base200";
    diffuseMap[0] = "art/Textures/Weapons/BoarSpear_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BoarSpear_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BoarSpear_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BoarSpear_Base500)
{
    mapTo = "BoarSpear_Base500";
    diffuseMap[0] = "art/Textures/Weapons/BoarSpear_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BoarSpear_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BoarSpear_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BoarSpear_BaseI)
{
    mapTo = "BoarSpear_BaseI";
    diffuseMap[0] = "art/Textures/Weapons/BoarSpear_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BoarSpear_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BoarSpear_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BoarSpear_BaseI10)
{
    mapTo = "BoarSpear_BaseI10";
    diffuseMap[0] = "art/Textures/Weapons/BoarSpear_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BoarSpear_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BoarSpear_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BoarSpear_BaseI100)
{
    mapTo = "BoarSpear_BaseI100";
    diffuseMap[0] = "art/Textures/Weapons/BoarSpear_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BoarSpear_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BoarSpear_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BoarSpear_BaseI200)
{
    mapTo = "BoarSpear_BaseI200";
    diffuseMap[0] = "art/Textures/Weapons/BoarSpear_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BoarSpear_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BoarSpear_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BoarSpear_BaseI500)
{
    mapTo = "BoarSpear_BaseI500";
    diffuseMap[0] = "art/Textures/Weapons/BoarSpear_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BoarSpear_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BoarSpear_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BoarSpear_diff_nobehave)
{
    mapTo = "BoarSpear_diff_nobehave";
    diffuseMap[0] = "art/Textures/Weapons/BoarSpear_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BoarSpear_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BoarSpear_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BoarSpear_diff_nobehaveC)
{
    mapTo = "BoarSpear_diff_nobehaveC";
    diffuseMap[0] = "art/Textures/Weapons/BoarSpear_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BoarSpear_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BoarSpear_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BoarSpear_diff_ns)
{
    mapTo = "BoarSpear_diff_ns";
    diffuseMap[0] = "art/Textures/Weapons/BoarSpear_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BoarSpear_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BoarSpear_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BoarSpear_SkinA)
{
    mapTo = "BoarSpear_SkinA";
    diffuseMap[0] = "art/Textures/Weapons/BoarSpear_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BoarSpear_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BoarSpear_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BoarSpear_SkinA10)
{
    mapTo = "BoarSpear_SkinA10";
    diffuseMap[0] = "art/Textures/Weapons/BoarSpear_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BoarSpear_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BoarSpear_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BoarSpear_SkinA100)
{
    mapTo = "BoarSpear_SkinA100";
    diffuseMap[0] = "art/Textures/Weapons/BoarSpear_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BoarSpear_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BoarSpear_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BoarSpear_SkinA200)
{
    mapTo = "BoarSpear_SkinA200";
    diffuseMap[0] = "art/Textures/Weapons/BoarSpear_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BoarSpear_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BoarSpear_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BoarSpear_SkinA500)
{
    mapTo = "BoarSpear_SkinA500";
    diffuseMap[0] = "art/Textures/Weapons/BoarSpear_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BoarSpear_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BoarSpear_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BoarSpear_SkinD)
{
    mapTo = "BoarSpear_SkinD";
    diffuseMap[0] = "art/Textures/Weapons/BoarSpear_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BoarSpear_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BoarSpear_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BoarSpear_SkinD10)
{
    mapTo = "BoarSpear_SkinD10";
    diffuseMap[0] = "art/Textures/Weapons/BoarSpear_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BoarSpear_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BoarSpear_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BoarSpear_SkinD100)
{
    mapTo = "BoarSpear_SkinD100";
    diffuseMap[0] = "art/Textures/Weapons/BoarSpear_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BoarSpear_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BoarSpear_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BoarSpear_SkinD200)
{
    mapTo = "BoarSpear_SkinD200";
    diffuseMap[0] = "art/Textures/Weapons/BoarSpear_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BoarSpear_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BoarSpear_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BoarSpear_SkinD500)
{
    mapTo = "BoarSpear_SkinD500";
    diffuseMap[0] = "art/Textures/Weapons/BoarSpear_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BoarSpear_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BoarSpear_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Broad_Axe_pack1_ns)
{
    mapTo = "Broad_Axe_pack1_ns";
    diffuseMap[0] = "art/Textures/Weapons/BroadAxe_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BroadAxe_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BroadAxe_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Broad_Axe_pack2C)
{
    mapTo = "Broad_Axe_pack2C";
    diffuseMap[0] = "art/Textures/Weapons/BroadAxe_Pack2_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BroadAxe_Pack2_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BroadAxe_Pack2_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BroadAxe_SkinA)
{
    mapTo = "BroadAxe_SkinA";
    diffuseMap[0] = "art/Textures/Weapons/BroadAxe_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BroadAxe_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BroadAxe_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BroadAxe_SkinA10)
{
    mapTo = "BroadAxe_SkinA10";
    diffuseMap[0] = "art/Textures/Weapons/BroadAxe_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BroadAxe_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BroadAxe_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BroadAxe_SkinA100)
{
    mapTo = "BroadAxe_SkinA100";
    diffuseMap[0] = "art/Textures/Weapons/BroadAxe_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BroadAxe_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BroadAxe_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BroadAxe_SkinA200)
{
    mapTo = "BroadAxe_SkinA200";
    diffuseMap[0] = "art/Textures/Weapons/BroadAxe_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BroadAxe_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BroadAxe_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BroadAxe_SkinA500)
{
    mapTo = "BroadAxe_SkinA500";
    diffuseMap[0] = "art/Textures/Weapons/BroadAxe_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BroadAxe_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BroadAxe_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BroadAxe_SkinD)
{
    mapTo = "BroadAxe_SkinD";
    diffuseMap[0] = "art/Textures/Weapons/BroadAxe_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BroadAxe_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BroadAxe_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BroadAxe_SkinD10)
{
    mapTo = "BroadAxe_SkinD10";
    diffuseMap[0] = "art/Textures/Weapons/BroadAxe_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BroadAxe_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BroadAxe_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BroadAxe_SkinD100)
{
    mapTo = "BroadAxe_SkinD100";
    diffuseMap[0] = "art/Textures/Weapons/BroadAxe_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BroadAxe_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BroadAxe_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BroadAxe_SkinD200)
{
    mapTo = "BroadAxe_SkinD200";
    diffuseMap[0] = "art/Textures/Weapons/BroadAxe_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BroadAxe_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BroadAxe_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BroadAxe_SkinD500)
{
    mapTo = "BroadAxe_SkinD500";
    diffuseMap[0] = "art/Textures/Weapons/BroadAxe_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BroadAxe_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BroadAxe_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BroadAxe_SkinDI)
{
    mapTo = "BroadAxe_SkinDI";
    diffuseMap[0] = "art/Textures/Weapons/BroadAxe_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BroadAxe_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BroadAxe_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BroadAxe_SkinDI10)
{
    mapTo = "BroadAxe_SkinDI10";
    diffuseMap[0] = "art/Textures/Weapons/BroadAxe_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BroadAxe_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BroadAxe_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BroadAxe_SkinDI100)
{
    mapTo = "BroadAxe_SkinDI100";
    diffuseMap[0] = "art/Textures/Weapons/BroadAxe_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BroadAxe_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BroadAxe_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BroadAxe_SkinDI200)
{
    mapTo = "BroadAxe_SkinDI200";
    diffuseMap[0] = "art/Textures/Weapons/BroadAxe_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BroadAxe_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BroadAxe_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BroadAxe_SkinDI500)
{
    mapTo = "BroadAxe_SkinDI500";
    diffuseMap[0] = "art/Textures/Weapons/BroadAxe_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BroadAxe_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BroadAxe_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BroadAxe_SkinE)
{
    mapTo = "BroadAxe_SkinE";
    diffuseMap[0] = "art/Textures/Weapons/BroadAxe_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BroadAxe_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BroadAxe_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BroadAxe_SkinE10)
{
    mapTo = "BroadAxe_SkinE10";
    diffuseMap[0] = "art/Textures/Weapons/BroadAxe_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BroadAxe_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BroadAxe_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BroadAxe_SkinE100)
{
    mapTo = "BroadAxe_SkinE100";
    diffuseMap[0] = "art/Textures/Weapons/BroadAxe_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BroadAxe_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BroadAxe_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BroadAxe_SkinE200)
{
    mapTo = "BroadAxe_SkinE200";
    diffuseMap[0] = "art/Textures/Weapons/BroadAxe_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BroadAxe_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BroadAxe_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_BroadAxe_SkinE500)
{
    mapTo = "BroadAxe_SkinE500";
    diffuseMap[0] = "art/Textures/Weapons/BroadAxe_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/BroadAxe_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/BroadAxe_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain60_Vik_Arm)
{
    mapTo = "Chain60_Vik_Arm";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain60_Vik_Arm10)
{
    mapTo = "Chain60_Vik_Arm10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain60_Vik_Arm100)
{
    mapTo = "Chain60_Vik_Arm100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain60_Vik_Arm200)
{
    mapTo = "Chain60_Vik_Arm200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain60_Vik_Arm500)
{
    mapTo = "Chain60_Vik_Arm500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain60_Vik_Body)
{
    mapTo = "Chain60_Vik_Body";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain60_Vik_Body10)
{
    mapTo = "Chain60_Vik_Body10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain60_Vik_Body100)
{
    mapTo = "Chain60_Vik_Body100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain60_Vik_Body200)
{
    mapTo = "Chain60_Vik_Body200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain60_Vik_Body500)
{
    mapTo = "Chain60_Vik_Body500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain60_Vik_Feet)
{
    mapTo = "Chain60_Vik_Feet";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain60_Vik_Feet10)
{
    mapTo = "Chain60_Vik_Feet10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain60_Vik_Feet100)
{
    mapTo = "Chain60_Vik_Feet100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain60_Vik_Feet200)
{
    mapTo = "Chain60_Vik_Feet200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain60_Vik_Feet500)
{
    mapTo = "Chain60_Vik_Feet500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain60_Vik_Forearm)
{
    mapTo = "Chain60_Vik_Forearm";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain60_Vik_Forearm10)
{
    mapTo = "Chain60_Vik_Forearm10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain60_Vik_Forearm100)
{
    mapTo = "Chain60_Vik_Forearm100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain60_Vik_Forearm200)
{
    mapTo = "Chain60_Vik_Forearm200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain60_Vik_Forearm500)
{
    mapTo = "Chain60_Vik_Forearm500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain60_Vik_Helmet)
{
    mapTo = "Chain60_Vik_Helmet";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain60_Vik_Helmet10)
{
    mapTo = "Chain60_Vik_Helmet10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain60_Vik_Helmet100)
{
    mapTo = "Chain60_Vik_Helmet100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain60_Vik_Helmet200)
{
    mapTo = "Chain60_Vik_Helmet200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain60_Vik_Helmet500)
{
    mapTo = "Chain60_Vik_Helmet500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain60_Vik_Legs)
{
    mapTo = "Chain60_Vik_Legs";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain60_Vik_Legs10)
{
    mapTo = "Chain60_Vik_Legs10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain60_Vik_Legs100)
{
    mapTo = "Chain60_Vik_Legs100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain60_Vik_Legs200)
{
    mapTo = "Chain60_Vik_Legs200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain60_Vik_Legs500)
{
    mapTo = "Chain60_Vik_Legs500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain60_Vik_no_skin)
{
    mapTo = "Chain60_Vik_no_skin";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain90_Vik_Arm)
{
    mapTo = "Chain90_Vik_Arm";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain90_Vik_Arm10)
{
    mapTo = "Chain90_Vik_Arm10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain90_Vik_Arm100)
{
    mapTo = "Chain90_Vik_Arm100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain90_Vik_Arm200)
{
    mapTo = "Chain90_Vik_Arm200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain90_Vik_Arm500)
{
    mapTo = "Chain90_Vik_Arm500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain90_Vik_Body)
{
    mapTo = "Chain90_Vik_Body";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain90_Vik_Body10)
{
    mapTo = "Chain90_Vik_Body10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain90_Vik_Body100)
{
    mapTo = "Chain90_Vik_Body100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain90_Vik_Body200)
{
    mapTo = "Chain90_Vik_Body200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain90_Vik_Body500)
{
    mapTo = "Chain90_Vik_Body500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain90_Vik_Feet)
{
    mapTo = "Chain90_Vik_Feet";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain90_Vik_Feet10)
{
    mapTo = "Chain90_Vik_Feet10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain90_Vik_Feet100)
{
    mapTo = "Chain90_Vik_Feet100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain90_Vik_Feet200)
{
    mapTo = "Chain90_Vik_Feet200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain90_Vik_Feet500)
{
    mapTo = "Chain90_Vik_Feet500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain90_Vik_Forearm)
{
    mapTo = "Chain90_Vik_Forearm";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain90_Vik_Forearm10)
{
    mapTo = "Chain90_Vik_Forearm10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain90_Vik_Forearm100)
{
    mapTo = "Chain90_Vik_Forearm100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain90_Vik_Forearm200)
{
    mapTo = "Chain90_Vik_Forearm200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain90_Vik_Forearm500)
{
    mapTo = "Chain90_Vik_Forearm500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain90_Vik_Helmet)
{
    mapTo = "Chain90_Vik_Helmet";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain90_Vik_Helmet_Add_Dw)
{
    mapTo = "Chain90_Vik_Helmet_Add_Dw";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain90_Vik_Helmet_Add_Dw10)
{
    mapTo = "Chain90_Vik_Helmet_Add_Dw10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain90_Vik_Helmet_Add_Dw100)
{
    mapTo = "Chain90_Vik_Helmet_Add_Dw100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain90_Vik_Helmet_Add_Dw200)
{
    mapTo = "Chain90_Vik_Helmet_Add_Dw200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain90_Vik_Helmet_Add_Dw500)
{
    mapTo = "Chain90_Vik_Helmet_Add_Dw500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain90_Vik_Helmet_Add_Up)
{
    mapTo = "Chain90_Vik_Helmet_Add_Up";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain90_Vik_Helmet_Add_Up10)
{
    mapTo = "Chain90_Vik_Helmet_Add_Up10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain90_Vik_Helmet_Add_Up100)
{
    mapTo = "Chain90_Vik_Helmet_Add_Up100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain90_Vik_Helmet_Add_Up200)
{
    mapTo = "Chain90_Vik_Helmet_Add_Up200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain90_Vik_Helmet_Add_Up500)
{
    mapTo = "Chain90_Vik_Helmet_Add_Up500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain90_Vik_Helmet10)
{
    mapTo = "Chain90_Vik_Helmet10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain90_Vik_Helmet100)
{
    mapTo = "Chain90_Vik_Helmet100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain90_Vik_Helmet200)
{
    mapTo = "Chain90_Vik_Helmet200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain90_Vik_Helmet500)
{
    mapTo = "Chain90_Vik_Helmet500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain90_Vik_Legs)
{
    mapTo = "Chain90_Vik_Legs";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain90_Vik_Legs10)
{
    mapTo = "Chain90_Vik_Legs10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain90_Vik_Legs100)
{
    mapTo = "Chain90_Vik_Legs100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain90_Vik_Legs200)
{
    mapTo = "Chain90_Vik_Legs200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain90_Vik_Legs500)
{
    mapTo = "Chain90_Vik_Legs500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Chain90_Vik_no_skin)
{
    mapTo = "Chain90_Vik_no_skin";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Chainmail/Chain_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Claymore_DIFFUSE_ns)
{
    mapTo = "Claymore_DIFFUSE_ns";
    diffuseMap[0] = "art/Textures/Weapons/Claymore_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Claymore_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Claymore_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Claymore_SkinE)
{
    mapTo = "Claymore_SkinE";
    diffuseMap[0] = "art/Textures/Weapons/Claymore_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Claymore_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Claymore_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Claymore_SkinE10)
{
    mapTo = "Claymore_SkinE10";
    diffuseMap[0] = "art/Textures/Weapons/Claymore_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Claymore_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Claymore_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Claymore_SkinE100)
{
    mapTo = "Claymore_SkinE100";
    diffuseMap[0] = "art/Textures/Weapons/Claymore_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Claymore_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Claymore_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Claymore_SkinE200)
{
    mapTo = "Claymore_SkinE200";
    diffuseMap[0] = "art/Textures/Weapons/Claymore_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Claymore_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Claymore_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Claymore_SkinE500)
{
    mapTo = "Claymore_SkinE500";
    diffuseMap[0] = "art/Textures/Weapons/Claymore_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Claymore_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Claymore_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Falchion_SkinB)
{
    mapTo = "Falchion_SkinB";
    diffuseMap[0] = "art/Textures/Weapons/Falchion_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Falchion_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Falchion_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Falchion_SkinB10)
{
    mapTo = "Falchion_SkinB10";
    diffuseMap[0] = "art/Textures/Weapons/Falchion_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Falchion_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Falchion_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Falchion_SkinB100)
{
    mapTo = "Falchion_SkinB100";
    diffuseMap[0] = "art/Textures/Weapons/Falchion_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Falchion_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Falchion_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Falchion_SkinB200)
{
    mapTo = "Falchion_SkinB200";
    diffuseMap[0] = "art/Textures/Weapons/Falchion_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Falchion_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Falchion_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Falchion_SkinB500)
{
    mapTo = "Falchion_SkinB500";
    diffuseMap[0] = "art/Textures/Weapons/Falchion_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Falchion_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Falchion_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Flamberge_pack1_diff_ns)
{
    mapTo = "Flamberge_pack1_diff_ns";
    diffuseMap[0] = "art/Textures/Weapons/Flamberge_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Flamberge_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Flamberge_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Flamberge_pack2_diff_nobehave)
{
    mapTo = "Flamberge_pack2_diff_nobehave";
    diffuseMap[0] = "art/Textures/Weapons/Flamberge_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Flamberge_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Flamberge_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Flamberge_pack2_diff_nobehaveC)
{
    mapTo = "Flamberge_pack2_diff_nobehaveC";
    diffuseMap[0] = "art/Textures/Weapons/Flamberge_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Flamberge_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Flamberge_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Flamberge_SkinA)
{
    mapTo = "Flamberge_SkinA";
    diffuseMap[0] = "art/Textures/Weapons/Flamberge_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Flamberge_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Flamberge_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Flamberge_SkinA10)
{
    mapTo = "Flamberge_SkinA10";
    diffuseMap[0] = "art/Textures/Weapons/Flamberge_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Flamberge_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Flamberge_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Flamberge_SkinA100)
{
    mapTo = "Flamberge_SkinA100";
    diffuseMap[0] = "art/Textures/Weapons/Flamberge_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Flamberge_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Flamberge_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Flamberge_SkinA200)
{
    mapTo = "Flamberge_SkinA200";
    diffuseMap[0] = "art/Textures/Weapons/Flamberge_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Flamberge_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Flamberge_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Flamberge_SkinA500)
{
    mapTo = "Flamberge_SkinA500";
    diffuseMap[0] = "art/Textures/Weapons/Flamberge_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Flamberge_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Flamberge_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Flamberge_SkinF)
{
    mapTo = "Flamberge_SkinF";
    diffuseMap[0] = "art/Textures/Weapons/Flamberge_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Flamberge_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Flamberge_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Flamberge_SkinF10)
{
    mapTo = "Flamberge_SkinF10";
    diffuseMap[0] = "art/Textures/Weapons/Flamberge_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Flamberge_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Flamberge_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Flamberge_SkinF100)
{
    mapTo = "Flamberge_SkinF100";
    diffuseMap[0] = "art/Textures/Weapons/Flamberge_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Flamberge_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Flamberge_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Flamberge_SkinF200)
{
    mapTo = "Flamberge_SkinF200";
    diffuseMap[0] = "art/Textures/Weapons/Flamberge_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Flamberge_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Flamberge_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Flamberge_SkinF500)
{
    mapTo = "Flamberge_SkinF500";
    diffuseMap[0] = "art/Textures/Weapons/Flamberge_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Flamberge_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Flamberge_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_furniture_pack3)
{
    mapTo = "furniture_pack3";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack3_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack3_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack3_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Glaive_SkinA)
{
    mapTo = "Glaive_SkinA";
    diffuseMap[0] = "art/Textures/Weapons/Glaive_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Glaive_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Glaive_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Glaive_SkinA10)
{
    mapTo = "Glaive_SkinA10";
    diffuseMap[0] = "art/Textures/Weapons/Glaive_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Glaive_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Glaive_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Glaive_SkinA100)
{
    mapTo = "Glaive_SkinA100";
    diffuseMap[0] = "art/Textures/Weapons/Glaive_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Glaive_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Glaive_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Glaive_SkinA200)
{
    mapTo = "Glaive_SkinA200";
    diffuseMap[0] = "art/Textures/Weapons/Glaive_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Glaive_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Glaive_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Glaive_SkinA500)
{
    mapTo = "Glaive_SkinA500";
    diffuseMap[0] = "art/Textures/Weapons/Glaive_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Glaive_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Glaive_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Glaive_SkinD)
{
    mapTo = "Glaive_SkinD";
    diffuseMap[0] = "art/Textures/Weapons/Glaive_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Glaive_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Glaive_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Glaive_SkinD10)
{
    mapTo = "Glaive_SkinD10";
    diffuseMap[0] = "art/Textures/Weapons/Glaive_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Glaive_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Glaive_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Glaive_SkinD100)
{
    mapTo = "Glaive_SkinD100";
    diffuseMap[0] = "art/Textures/Weapons/Glaive_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Glaive_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Glaive_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Glaive_SkinD200)
{
    mapTo = "Glaive_SkinD200";
    diffuseMap[0] = "art/Textures/Weapons/Glaive_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Glaive_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Glaive_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Glaive_SkinD500)
{
    mapTo = "Glaive_SkinD500";
    diffuseMap[0] = "art/Textures/Weapons/Glaive_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Glaive_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Glaive_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Grossmesser_Base)
{
    mapTo = "Grossmesser_Base";
    diffuseMap[0] = "art/Textures/Weapons/GrossMesser_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/GrossMesser_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/GrossMesser_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Grossmesser_Base250)
{
    mapTo = "Grossmesser_Base250";
    diffuseMap[0] = "art/Textures/Weapons/GrossMesser_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/GrossMesser_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/GrossMesser_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Grossmesser_Base40)
{
    mapTo = "Grossmesser_Base40";
    diffuseMap[0] = "art/Textures/Weapons/GrossMesser_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/GrossMesser_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/GrossMesser_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Grossmesser_Base5)
{
    mapTo = "Grossmesser_Base5";
    diffuseMap[0] = "art/Textures/Weapons/GrossMesser_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/GrossMesser_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/GrossMesser_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Grossmesser_Base80)
{
    mapTo = "Grossmesser_Base80";
    diffuseMap[0] = "art/Textures/Weapons/GrossMesser_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/GrossMesser_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/GrossMesser_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Grossmesser_diff_nobehave)
{
    mapTo = "Grossmesser_diff_nobehave";
    diffuseMap[0] = "art/Textures/Weapons/GrossMesser_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/GrossMesser_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/GrossMesser_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Grossmesser_Skin)
{
    mapTo = "Grossmesser_Skin";
    diffuseMap[0] = "art/Textures/Weapons/GrossMesser_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/GrossMesser_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/GrossMesser_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Grossmesser_SkinB)
{
    mapTo = "Grossmesser_SkinB";
    diffuseMap[0] = "art/Textures/Weapons/GrossMesser_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/GrossMesser_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/GrossMesser_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Grossmesser_SkinB10)
{
    mapTo = "Grossmesser_SkinB10";
    diffuseMap[0] = "art/Textures/Weapons/GrossMesser_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/GrossMesser_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/GrossMesser_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Grossmesser_SkinB100)
{
    mapTo = "Grossmesser_SkinB100";
    diffuseMap[0] = "art/Textures/Weapons/GrossMesser_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/GrossMesser_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/GrossMesser_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Grossmesser_SkinB200)
{
    mapTo = "Grossmesser_SkinB200";
    diffuseMap[0] = "art/Textures/Weapons/GrossMesser_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/GrossMesser_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/GrossMesser_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Grossmesser_SkinB500)
{
    mapTo = "Grossmesser_SkinB500";
    diffuseMap[0] = "art/Textures/Weapons/GrossMesser_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/GrossMesser_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/GrossMesser_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Grossmesser_SkinC)
{
    mapTo = "Grossmesser_SkinC";
    diffuseMap[0] = "art/Textures/Weapons/GrossMesser_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/GrossMesser_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/GrossMesser_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Grossmesser_SkinC250)
{
    mapTo = "Grossmesser_SkinC250";
    diffuseMap[0] = "art/Textures/Weapons/GrossMesser_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/GrossMesser_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/GrossMesser_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Grossmesser_SkinC40)
{
    mapTo = "Grossmesser_SkinC40";
    diffuseMap[0] = "art/Textures/Weapons/GrossMesser_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/GrossMesser_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/GrossMesser_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Grossmesser_SkinC5)
{
    mapTo = "Grossmesser_SkinC5";
    diffuseMap[0] = "art/Textures/Weapons/GrossMesser_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/GrossMesser_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/GrossMesser_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Grossmesser_SkinC80)
{
    mapTo = "Grossmesser_SkinC80";
    diffuseMap[0] = "art/Textures/Weapons/GrossMesser_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/GrossMesser_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/GrossMesser_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Grossmesser_SkinD)
{
    mapTo = "Grossmesser_SkinD";
    diffuseMap[0] = "art/Textures/Weapons/GrossMesser_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/GrossMesser_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/GrossMesser_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Grossmesser_SkinD250)
{
    mapTo = "Grossmesser_SkinD250";
    diffuseMap[0] = "art/Textures/Weapons/GrossMesser_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/GrossMesser_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/GrossMesser_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Grossmesser_SkinD40)
{
    mapTo = "Grossmesser_SkinD40";
    diffuseMap[0] = "art/Textures/Weapons/GrossMesser_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/GrossMesser_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/GrossMesser_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Grossmesser_SkinD5)
{
    mapTo = "Grossmesser_SkinD5";
    diffuseMap[0] = "art/Textures/Weapons/GrossMesser_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/GrossMesser_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/GrossMesser_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Grossmesser_SkinD80)
{
    mapTo = "Grossmesser_SkinD80";
    diffuseMap[0] = "art/Textures/Weapons/GrossMesser_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/GrossMesser_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/GrossMesser_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Grossmesser_SkinE)
{
    mapTo = "Grossmesser_SkinE";
    diffuseMap[0] = "art/Textures/Weapons/GrossMesser_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/GrossMesser_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/GrossMesser_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Grossmesser_SkinE250)
{
    mapTo = "Grossmesser_SkinE250";
    diffuseMap[0] = "art/Textures/Weapons/GrossMesser_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/GrossMesser_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/GrossMesser_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Grossmesser_SkinE40)
{
    mapTo = "Grossmesser_SkinE40";
    diffuseMap[0] = "art/Textures/Weapons/GrossMesser_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/GrossMesser_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/GrossMesser_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Grossmesser_SkinE5)
{
    mapTo = "Grossmesser_SkinE5";
    diffuseMap[0] = "art/Textures/Weapons/GrossMesser_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/GrossMesser_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/GrossMesser_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Grossmesser_SkinE80)
{
    mapTo = "Grossmesser_SkinE80";
    diffuseMap[0] = "art/Textures/Weapons/GrossMesser_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/GrossMesser_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/GrossMesser_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Heather_ShieldA_A)
{
    mapTo = "Heather_ShieldA_A";
    diffuseMap[0] = "art/Textures/Shields/Shield_HeaterA_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_HeaterA_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_HeaterA_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Heather_ShieldA_A10)
{
    mapTo = "Heather_ShieldA_A10";
    diffuseMap[0] = "art/Textures/Shields/Shield_HeaterA_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_HeaterA_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_HeaterA_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Heather_ShieldA_A100)
{
    mapTo = "Heather_ShieldA_A100";
    diffuseMap[0] = "art/Textures/Shields/Shield_HeaterA_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_HeaterA_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_HeaterA_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Heather_ShieldA_A200)
{
    mapTo = "Heather_ShieldA_A200";
    diffuseMap[0] = "art/Textures/Shields/Shield_HeaterA_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_HeaterA_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_HeaterA_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Heather_ShieldA_A500)
{
    mapTo = "Heather_ShieldA_A500";
    diffuseMap[0] = "art/Textures/Shields/Shield_HeaterA_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_HeaterA_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_HeaterA_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Heavy_Horse_base_no_skin)
{
    mapTo = "Heavy_Horse_base_no_skin";
    diffuseMap[0] = "art/Textures/Animals/Heavy_Horse1_DIFFUSE_01.dds";
    diffuseMap[2] = "art/Textures/Animals/Heavy_Horse1_SPECULAR_01.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Heavy_Horse2_DIFFUSE_01_no_skin)
{
    mapTo = "Heavy_Horse2_DIFFUSE_01_no_skin";
    diffuseMap[0] = "art/Textures/Animals/Heavy_Horse2_DIFFUSE_01.dds";
    diffuseMap[2] = "art/Textures/Animals/Heavy_Horse2_SPECULAR_01.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Heavy_Horse3_DIFFUSE_01_no_skin)
{
    mapTo = "Heavy_Horse3_DIFFUSE_01_no_skin";
    diffuseMap[0] = "art/Textures/Animals/Heavy_Horse3_DIFFUSE_01.dds";
    diffuseMap[2] = "art/Textures/Animals/Heavy_Horse3_SPECULAR_01.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_HeavyCrossbow_no_skin)
{
    mapTo = "HeavyCrossbow_no_skin";
    diffuseMap[0] = "art/Textures/Weapons/HeavyCrossbow_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/HeavyCrossbow_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/HeavyCrossbow_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_HeavyCrossbow_no_skinC)
{
    mapTo = "HeavyCrossbow_no_skinC";
    diffuseMap[0] = "art/Textures/Weapons/HeavyCrossbow_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/HeavyCrossbow_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/HeavyCrossbow_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_HeavyCrossbow_SkinA)
{
    mapTo = "HeavyCrossbow_SkinA";
    diffuseMap[0] = "art/Textures/Weapons/HeavyCrossbow_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/HeavyCrossbow_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/HeavyCrossbow_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_HeavyCrossbow_SkinA10)
{
    mapTo = "HeavyCrossbow_SkinA10";
    diffuseMap[0] = "art/Textures/Weapons/HeavyCrossbow_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/HeavyCrossbow_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/HeavyCrossbow_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_HeavyCrossbow_SkinA100)
{
    mapTo = "HeavyCrossbow_SkinA100";
    diffuseMap[0] = "art/Textures/Weapons/HeavyCrossbow_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/HeavyCrossbow_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/HeavyCrossbow_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_HeavyCrossbow_SkinA200)
{
    mapTo = "HeavyCrossbow_SkinA200";
    diffuseMap[0] = "art/Textures/Weapons/HeavyCrossbow_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/HeavyCrossbow_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/HeavyCrossbow_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_HeavyCrossbow_SkinA500)
{
    mapTo = "HeavyCrossbow_SkinA500";
    diffuseMap[0] = "art/Textures/Weapons/HeavyCrossbow_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/HeavyCrossbow_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/HeavyCrossbow_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_HeavyCrossbowBody_SkinA)
{
    mapTo = "HeavyCrossbowBody_SkinA";
    diffuseMap[0] = "art/Textures/Weapons/HeavyCrossbow_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/HeavyCrossbow_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/HeavyCrossbow_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_HeavyCrossbowBody_SkinA10)
{
    mapTo = "HeavyCrossbowBody_SkinA10";
    diffuseMap[0] = "art/Textures/Weapons/HeavyCrossbow_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/HeavyCrossbow_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/HeavyCrossbow_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_HeavyCrossbowBody_SkinA100)
{
    mapTo = "HeavyCrossbowBody_SkinA100";
    diffuseMap[0] = "art/Textures/Weapons/HeavyCrossbow_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/HeavyCrossbow_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/HeavyCrossbow_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_HeavyCrossbowBody_SkinA200)
{
    mapTo = "HeavyCrossbowBody_SkinA200";
    diffuseMap[0] = "art/Textures/Weapons/HeavyCrossbow_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/HeavyCrossbow_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/HeavyCrossbow_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_HeavyCrossbowBody_SkinA500)
{
    mapTo = "HeavyCrossbowBody_SkinA500";
    diffuseMap[0] = "art/Textures/Weapons/HeavyCrossbow_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/HeavyCrossbow_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/HeavyCrossbow_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Helmet_Eur_no_skin)
{
    mapTo = "Helmet_Eur_no_skin";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Helmet0_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Helmet0_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Helmet0_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Horse_DIFFUSE_BLACK_01_no_skin)
{
    mapTo = "Horse_DIFFUSE_BLACK_01_no_skin";
    diffuseMap[0] = "art/Textures/Animals/Horse_DIFFUSE_BLACK_01.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_JoustingLance_DIFFUSE_nobehave)
{
    mapTo = "JoustingLance_DIFFUSE_nobehave";
    diffuseMap[0] = "art/Textures/Weapons/JoustingLance_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/JoustingLance_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/JoustingLance_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_JoustingLance_DIFFUSE_nobehaveC)
{
    mapTo = "JoustingLance_DIFFUSE_nobehaveC";
    diffuseMap[0] = "art/Textures/Weapons/JoustingLance_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/JoustingLance_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/JoustingLance_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_JoustingLance_Skin)
{
    mapTo = "JoustingLance_Skin";
    diffuseMap[0] = "art/Textures/Weapons/JoustingLance_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/JoustingLance_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/JoustingLance_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_JoustingLance_SkinB)
{
    mapTo = "JoustingLance_SkinB";
    diffuseMap[0] = "art/Textures/Weapons/JoustingLance_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/JoustingLance_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/JoustingLance_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_JoustingLance_SkinB10)
{
    mapTo = "JoustingLance_SkinB10";
    diffuseMap[0] = "art/Textures/Weapons/JoustingLance_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/JoustingLance_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/JoustingLance_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_JoustingLance_SkinB100)
{
    mapTo = "JoustingLance_SkinB100";
    diffuseMap[0] = "art/Textures/Weapons/JoustingLance_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/JoustingLance_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/JoustingLance_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_JoustingLance_SkinB200)
{
    mapTo = "JoustingLance_SkinB200";
    diffuseMap[0] = "art/Textures/Weapons/JoustingLance_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/JoustingLance_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/JoustingLance_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_JoustingLance_SkinB500)
{
    mapTo = "JoustingLance_SkinB500";
    diffuseMap[0] = "art/Textures/Weapons/JoustingLance_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/JoustingLance_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/JoustingLance_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_JoustingLance_SkinC)
{
    mapTo = "JoustingLance_SkinC";
    diffuseMap[0] = "art/Textures/Weapons/JoustingLance_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/JoustingLance_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/JoustingLance_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_JoustingLance_SkinC10)
{
    mapTo = "JoustingLance_SkinC10";
    diffuseMap[0] = "art/Textures/Weapons/JoustingLance_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/JoustingLance_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/JoustingLance_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_JoustingLance_SkinC100)
{
    mapTo = "JoustingLance_SkinC100";
    diffuseMap[0] = "art/Textures/Weapons/JoustingLance_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/JoustingLance_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/JoustingLance_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_JoustingLance_SkinC200)
{
    mapTo = "JoustingLance_SkinC200";
    diffuseMap[0] = "art/Textures/Weapons/JoustingLance_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/JoustingLance_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/JoustingLance_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_JoustingLance_SkinC500)
{
    mapTo = "JoustingLance_SkinC500";
    diffuseMap[0] = "art/Textures/Weapons/JoustingLance_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/JoustingLance_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/JoustingLance_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_JoustingLance_SkinD)
{
    mapTo = "JoustingLance_SkinD";
    diffuseMap[0] = "art/Textures/Weapons/JoustingLance_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/JoustingLance_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/JoustingLance_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_JoustingLance_SkinD10)
{
    mapTo = "JoustingLance_SkinD10";
    diffuseMap[0] = "art/Textures/Weapons/JoustingLance_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/JoustingLance_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/JoustingLance_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_JoustingLance_SkinD100)
{
    mapTo = "JoustingLance_SkinD100";
    diffuseMap[0] = "art/Textures/Weapons/JoustingLance_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/JoustingLance_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/JoustingLance_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_JoustingLance_SkinD200)
{
    mapTo = "JoustingLance_SkinD200";
    diffuseMap[0] = "art/Textures/Weapons/JoustingLance_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/JoustingLance_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/JoustingLance_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_JoustingLance_SkinD500)
{
    mapTo = "JoustingLance_SkinD500";
    diffuseMap[0] = "art/Textures/Weapons/JoustingLance_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/JoustingLance_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/JoustingLance_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_knight_sword_nobehave)
{
    mapTo = "knight_sword_nobehave";
    diffuseMap[0] = "art/Textures/Weapons/KnightSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/KnightSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/KnightSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_knight_sword_nobehaveC)
{
    mapTo = "knight_sword_nobehaveC";
    diffuseMap[0] = "art/Textures/Weapons/KnightSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/KnightSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/KnightSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_knight_sword_ns)
{
    mapTo = "knight_sword_ns";
    diffuseMap[0] = "art/Textures/Weapons/KnightSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/KnightSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/KnightSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_knight_sword_nsC)
{
    mapTo = "knight_sword_nsC";
    diffuseMap[0] = "art/Textures/Weapons/KnightSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/KnightSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/KnightSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_KnightSword_Base)
{
    mapTo = "KnightSword_Base";
    diffuseMap[0] = "art/Textures/Weapons/KnightSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/KnightSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/KnightSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_KnightSword_Base250)
{
    mapTo = "KnightSword_Base250";
    diffuseMap[0] = "art/Textures/Weapons/KnightSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/KnightSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/KnightSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_KnightSword_Base40)
{
    mapTo = "KnightSword_Base40";
    diffuseMap[0] = "art/Textures/Weapons/KnightSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/KnightSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/KnightSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_KnightSword_Base5)
{
    mapTo = "KnightSword_Base5";
    diffuseMap[0] = "art/Textures/Weapons/KnightSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/KnightSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/KnightSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_KnightSword_Base80)
{
    mapTo = "KnightSword_Base80";
    diffuseMap[0] = "art/Textures/Weapons/KnightSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/KnightSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/KnightSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_KnightSword_Skin)
{
    mapTo = "KnightSword_Skin";
    diffuseMap[0] = "art/Textures/Weapons/KnightSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/KnightSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/KnightSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_KnightSword_SkinB)
{
    mapTo = "KnightSword_SkinB";
    diffuseMap[0] = "art/Textures/Weapons/KnightSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/KnightSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/KnightSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_KnightSword_SkinB10)
{
    mapTo = "KnightSword_SkinB10";
    diffuseMap[0] = "art/Textures/Weapons/KnightSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/KnightSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/KnightSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_KnightSword_SkinB100)
{
    mapTo = "KnightSword_SkinB100";
    diffuseMap[0] = "art/Textures/Weapons/KnightSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/KnightSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/KnightSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_KnightSword_SkinB200)
{
    mapTo = "KnightSword_SkinB200";
    diffuseMap[0] = "art/Textures/Weapons/KnightSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/KnightSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/KnightSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_KnightSword_SkinB500)
{
    mapTo = "KnightSword_SkinB500";
    diffuseMap[0] = "art/Textures/Weapons/KnightSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/KnightSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/KnightSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_KnightSword_SkinC)
{
    mapTo = "KnightSword_SkinC";
    diffuseMap[0] = "art/Textures/Weapons/KnightSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/KnightSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/KnightSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_KnightSword_SkinC10)
{
    mapTo = "KnightSword_SkinC10";
    diffuseMap[0] = "art/Textures/Weapons/KnightSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/KnightSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/KnightSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_KnightSword_SkinC100)
{
    mapTo = "KnightSword_SkinC100";
    diffuseMap[0] = "art/Textures/Weapons/KnightSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/KnightSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/KnightSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_KnightSword_SkinC200)
{
    mapTo = "KnightSword_SkinC200";
    diffuseMap[0] = "art/Textures/Weapons/KnightSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/KnightSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/KnightSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_KnightSword_SkinC500)
{
    mapTo = "KnightSword_SkinC500";
    diffuseMap[0] = "art/Textures/Weapons/KnightSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/KnightSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/KnightSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_KnightSword_SkinF)
{
    mapTo = "KnightSword_SkinF";
    diffuseMap[0] = "art/Textures/Weapons/KnightSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/KnightSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/KnightSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_KnightSword_SkinF250)
{
    mapTo = "KnightSword_SkinF250";
    diffuseMap[0] = "art/Textures/Weapons/KnightSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/KnightSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/KnightSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_KnightSword_SkinF40)
{
    mapTo = "KnightSword_SkinF40";
    diffuseMap[0] = "art/Textures/Weapons/KnightSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/KnightSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/KnightSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_KnightSword_SkinF5)
{
    mapTo = "KnightSword_SkinF5";
    diffuseMap[0] = "art/Textures/Weapons/KnightSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/KnightSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/KnightSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_KnightSword_SkinF80)
{
    mapTo = "KnightSword_SkinF80";
    diffuseMap[0] = "art/Textures/Weapons/KnightSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/KnightSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/KnightSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Lance_pack1_nobehave)
{
    mapTo = "Lance_pack1_nobehave";
    diffuseMap[0] = "art/Textures/Weapons/Lance_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Lance_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Lance_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Lance_pack1_nobehaveC)
{
    mapTo = "Lance_pack1_nobehaveC";
    diffuseMap[0] = "art/Textures/Weapons/Lance_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Lance_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Lance_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Lance_Skin)
{
    mapTo = "Lance_Skin";
    diffuseMap[0] = "art/Textures/Weapons/Lance_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Lance_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Lance_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Lance_SkinA)
{
    mapTo = "Lance_SkinA";
    diffuseMap[0] = "art/Textures/Weapons/Lance_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Lance_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Lance_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Lance_SkinA10)
{
    mapTo = "Lance_SkinA10";
    diffuseMap[0] = "art/Textures/Weapons/Lance_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Lance_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Lance_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Lance_SkinA100)
{
    mapTo = "Lance_SkinA100";
    diffuseMap[0] = "art/Textures/Weapons/Lance_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Lance_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Lance_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Lance_SkinA200)
{
    mapTo = "Lance_SkinA200";
    diffuseMap[0] = "art/Textures/Weapons/Lance_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Lance_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Lance_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Lance_SkinA500)
{
    mapTo = "Lance_SkinA500";
    diffuseMap[0] = "art/Textures/Weapons/Lance_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Lance_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Lance_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Lance_SkinB)
{
    mapTo = "Lance_SkinB";
    diffuseMap[0] = "art/Textures/Weapons/Lance_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Lance_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Lance_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Lance_SkinB10)
{
    mapTo = "Lance_SkinB10";
    diffuseMap[0] = "art/Textures/Weapons/Lance_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Lance_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Lance_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Lance_SkinB100)
{
    mapTo = "Lance_SkinB100";
    diffuseMap[0] = "art/Textures/Weapons/Lance_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Lance_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Lance_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Lance_SkinB200)
{
    mapTo = "Lance_SkinB200";
    diffuseMap[0] = "art/Textures/Weapons/Lance_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Lance_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Lance_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Lance_SkinB500)
{
    mapTo = "Lance_SkinB500";
    diffuseMap[0] = "art/Textures/Weapons/Lance_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Lance_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Lance_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Lance_SkinC)
{
    mapTo = "Lance_SkinC";
    diffuseMap[0] = "art/Textures/Weapons/Lance_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Lance_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Lance_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Lance_SkinC10)
{
    mapTo = "Lance_SkinC10";
    diffuseMap[0] = "art/Textures/Weapons/Lance_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Lance_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Lance_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Lance_SkinC100)
{
    mapTo = "Lance_SkinC100";
    diffuseMap[0] = "art/Textures/Weapons/Lance_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Lance_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Lance_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Lance_SkinC200)
{
    mapTo = "Lance_SkinC200";
    diffuseMap[0] = "art/Textures/Weapons/Lance_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Lance_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Lance_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Lance_SkinC500)
{
    mapTo = "Lance_SkinC500";
    diffuseMap[0] = "art/Textures/Weapons/Lance_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Lance_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Lance_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Lance_SkinD)
{
    mapTo = "Lance_SkinD";
    diffuseMap[0] = "art/Textures/Weapons/Lance_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Lance_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Lance_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Lance_SkinD10)
{
    mapTo = "Lance_SkinD10";
    diffuseMap[0] = "art/Textures/Weapons/Lance_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Lance_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Lance_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Lance_SkinD100)
{
    mapTo = "Lance_SkinD100";
    diffuseMap[0] = "art/Textures/Weapons/Lance_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Lance_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Lance_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Lance_SkinD200)
{
    mapTo = "Lance_SkinD200";
    diffuseMap[0] = "art/Textures/Weapons/Lance_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Lance_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Lance_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Lance_SkinD500)
{
    mapTo = "Lance_SkinD500";
    diffuseMap[0] = "art/Textures/Weapons/Lance_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Lance_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Lance_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather100_Eur_Arm)
{
    mapTo = "Leather100_Eur_Arm";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_100_Eur_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather100_Eur_Arm10)
{
    mapTo = "Leather100_Eur_Arm10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_100_Eur_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather100_Eur_Arm100)
{
    mapTo = "Leather100_Eur_Arm100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_100_Eur_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather100_Eur_Arm200)
{
    mapTo = "Leather100_Eur_Arm200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_100_Eur_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather100_Eur_Arm500)
{
    mapTo = "Leather100_Eur_Arm500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_100_Eur_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather100_Eur_Body)
{
    mapTo = "Leather100_Eur_Body";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_100_Eur_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather100_Eur_Body10)
{
    mapTo = "Leather100_Eur_Body10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_100_Eur_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather100_Eur_Body100)
{
    mapTo = "Leather100_Eur_Body100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_100_Eur_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather100_Eur_Body200)
{
    mapTo = "Leather100_Eur_Body200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_100_Eur_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather100_Eur_Body500)
{
    mapTo = "Leather100_Eur_Body500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_100_Eur_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather100_Eur_Feet)
{
    mapTo = "Leather100_Eur_Feet";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_100_Eur_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather100_Eur_Feet10)
{
    mapTo = "Leather100_Eur_Feet10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_100_Eur_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather100_Eur_Feet100)
{
    mapTo = "Leather100_Eur_Feet100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_100_Eur_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather100_Eur_Feet200)
{
    mapTo = "Leather100_Eur_Feet200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_100_Eur_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather100_Eur_Feet500)
{
    mapTo = "Leather100_Eur_Feet500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_100_Eur_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather100_Eur_Forearm)
{
    mapTo = "Leather100_Eur_Forearm";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_100_Eur_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather100_Eur_Forearm10)
{
    mapTo = "Leather100_Eur_Forearm10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_100_Eur_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather100_Eur_Forearm100)
{
    mapTo = "Leather100_Eur_Forearm100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_100_Eur_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather100_Eur_Forearm200)
{
    mapTo = "Leather100_Eur_Forearm200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_100_Eur_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather100_Eur_Forearm500)
{
    mapTo = "Leather100_Eur_Forearm500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_100_Eur_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather100_Eur_Helmet)
{
    mapTo = "Leather100_Eur_Helmet";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_100_Eur_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather100_Eur_Helmet10)
{
    mapTo = "Leather100_Eur_Helmet10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_100_Eur_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather100_Eur_Helmet100)
{
    mapTo = "Leather100_Eur_Helmet100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_100_Eur_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather100_Eur_Helmet200)
{
    mapTo = "Leather100_Eur_Helmet200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_100_Eur_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather100_Eur_Helmet500)
{
    mapTo = "Leather100_Eur_Helmet500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_100_Eur_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather100_Eur_Legs)
{
    mapTo = "Leather100_Eur_Legs";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_100_Eur_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather100_Eur_Legs10)
{
    mapTo = "Leather100_Eur_Legs10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_100_Eur_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather100_Eur_Legs100)
{
    mapTo = "Leather100_Eur_Legs100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_100_Eur_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather100_Eur_Legs200)
{
    mapTo = "Leather100_Eur_Legs200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_100_Eur_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather100_Eur_Legs500)
{
    mapTo = "Leather100_Eur_Legs500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_100_Eur_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather100_Eur_no_skin)
{
    mapTo = "Leather100_Eur_no_skin";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_100_Eur_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather90_Eur_Arm)
{
    mapTo = "Leather90_Eur_Arm";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather90_Eur_Arm10)
{
    mapTo = "Leather90_Eur_Arm10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather90_Eur_Arm100)
{
    mapTo = "Leather90_Eur_Arm100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather90_Eur_Arm200)
{
    mapTo = "Leather90_Eur_Arm200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather90_Eur_Arm500)
{
    mapTo = "Leather90_Eur_Arm500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather90_Eur_Body)
{
    mapTo = "Leather90_Eur_Body";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather90_Eur_Body10)
{
    mapTo = "Leather90_Eur_Body10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather90_Eur_Body100)
{
    mapTo = "Leather90_Eur_Body100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather90_Eur_Body200)
{
    mapTo = "Leather90_Eur_Body200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather90_Eur_Body500)
{
    mapTo = "Leather90_Eur_Body500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather90_Eur_Feet)
{
    mapTo = "Leather90_Eur_Feet";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather90_Eur_Feet10)
{
    mapTo = "Leather90_Eur_Feet10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather90_Eur_Feet100)
{
    mapTo = "Leather90_Eur_Feet100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather90_Eur_Feet200)
{
    mapTo = "Leather90_Eur_Feet200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather90_Eur_Feet500)
{
    mapTo = "Leather90_Eur_Feet500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather90_Eur_Forearm)
{
    mapTo = "Leather90_Eur_Forearm";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather90_Eur_Forearm10)
{
    mapTo = "Leather90_Eur_Forearm10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather90_Eur_Forearm100)
{
    mapTo = "Leather90_Eur_Forearm100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather90_Eur_Forearm200)
{
    mapTo = "Leather90_Eur_Forearm200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather90_Eur_Forearm500)
{
    mapTo = "Leather90_Eur_Forearm500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather90_Eur_Helmet)
{
    mapTo = "Leather90_Eur_Helmet";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather90_Eur_Helmet10)
{
    mapTo = "Leather90_Eur_Helmet10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather90_Eur_Helmet100)
{
    mapTo = "Leather90_Eur_Helmet100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather90_Eur_Helmet200)
{
    mapTo = "Leather90_Eur_Helmet200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather90_Eur_Helmet500)
{
    mapTo = "Leather90_Eur_Helmet500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather90_Eur_Legs)
{
    mapTo = "Leather90_Eur_Legs";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather90_Eur_Legs10)
{
    mapTo = "Leather90_Eur_Legs10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather90_Eur_Legs100)
{
    mapTo = "Leather90_Eur_Legs100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather90_Eur_Legs200)
{
    mapTo = "Leather90_Eur_Legs200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather90_Eur_Legs500)
{
    mapTo = "Leather90_Eur_Legs500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Leather90_Eur_no_skin)
{
    mapTo = "Leather90_Eur_no_skin";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Leather/Leather_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Maul_diff_ns)
{
    mapTo = "Maul_diff_ns";
    diffuseMap[0] = "art/Textures/Weapons/Maul_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Maul_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Maul_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Maul_pack2_diff_nobehave)
{
    mapTo = "Maul_pack2_diff_nobehave";
    diffuseMap[0] = "art/Textures/Weapons/Maul_Pack2_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Maul_Pack2_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Maul_Pack2_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Maul_pack2_diff_nobehaveC)
{
    mapTo = "Maul_pack2_diff_nobehaveC";
    diffuseMap[0] = "art/Textures/Weapons/Maul_Pack2_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Maul_Pack2_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Maul_Pack2_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Maul_Skin)
{
    mapTo = "Maul_Skin";
    diffuseMap[0] = "art/Textures/Weapons/Maul_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Maul_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Maul_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Maul_SkinA)
{
    mapTo = "Maul_SkinA";
    diffuseMap[0] = "art/Textures/Weapons/Maul_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Maul_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Maul_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Maul_SkinA10)
{
    mapTo = "Maul_SkinA10";
    diffuseMap[0] = "art/Textures/Weapons/Maul_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Maul_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Maul_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Maul_SkinA100)
{
    mapTo = "Maul_SkinA100";
    diffuseMap[0] = "art/Textures/Weapons/Maul_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Maul_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Maul_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Maul_SkinA200)
{
    mapTo = "Maul_SkinA200";
    diffuseMap[0] = "art/Textures/Weapons/Maul_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Maul_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Maul_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Maul_SkinA500)
{
    mapTo = "Maul_SkinA500";
    diffuseMap[0] = "art/Textures/Weapons/Maul_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Maul_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Maul_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Maul_SkinC)
{
    mapTo = "Maul_SkinC";
    diffuseMap[0] = "art/Textures/Weapons/Maul_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Maul_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Maul_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Maul_SkinC10)
{
    mapTo = "Maul_SkinC10";
    diffuseMap[0] = "art/Textures/Weapons/Maul_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Maul_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Maul_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Maul_SkinC100)
{
    mapTo = "Maul_SkinC100";
    diffuseMap[0] = "art/Textures/Weapons/Maul_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Maul_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Maul_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Maul_SkinC200)
{
    mapTo = "Maul_SkinC200";
    diffuseMap[0] = "art/Textures/Weapons/Maul_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Maul_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Maul_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Maul_SkinC500)
{
    mapTo = "Maul_SkinC500";
    diffuseMap[0] = "art/Textures/Weapons/Maul_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Maul_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Maul_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Maul_SkinE)
{
    mapTo = "Maul_SkinE";
    diffuseMap[0] = "art/Textures/Weapons/Maul_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Maul_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Maul_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Maul_SkinE10)
{
    mapTo = "Maul_SkinE10";
    diffuseMap[0] = "art/Textures/Weapons/Maul_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Maul_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Maul_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Maul_SkinE100)
{
    mapTo = "Maul_SkinE100";
    diffuseMap[0] = "art/Textures/Weapons/Maul_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Maul_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Maul_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Maul_SkinE200)
{
    mapTo = "Maul_SkinE200";
    diffuseMap[0] = "art/Textures/Weapons/Maul_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Maul_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Maul_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Maul_SkinE500)
{
    mapTo = "Maul_SkinE500";
    diffuseMap[0] = "art/Textures/Weapons/Maul_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Maul_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Maul_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_NordicSword_Base)
{
    mapTo = "NordicSword_Base";
    diffuseMap[0] = "art/Textures/Weapons/NordicSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/NordicSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/NordicSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_NordicSword_Base250)
{
    mapTo = "NordicSword_Base250";
    diffuseMap[0] = "art/Textures/Weapons/NordicSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/NordicSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/NordicSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_NordicSword_Base40)
{
    mapTo = "NordicSword_Base40";
    diffuseMap[0] = "art/Textures/Weapons/NordicSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/NordicSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/NordicSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_NordicSword_Base5)
{
    mapTo = "NordicSword_Base5";
    diffuseMap[0] = "art/Textures/Weapons/NordicSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/NordicSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/NordicSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_NordicSword_Base80)
{
    mapTo = "NordicSword_Base80";
    diffuseMap[0] = "art/Textures/Weapons/NordicSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/NordicSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/NordicSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_NordicSword_DIFFUSE_nobehave)
{
    mapTo = "NordicSword_DIFFUSE_nobehave";
    diffuseMap[0] = "art/Textures/Weapons/NordicSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/NordicSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/NordicSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_NordicSword_SkinB)
{
    mapTo = "NordicSword_SkinB";
    diffuseMap[0] = "art/Textures/Weapons/NordicSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/NordicSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/NordicSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_NordicSword_SkinB250)
{
    mapTo = "NordicSword_SkinB250";
    diffuseMap[0] = "art/Textures/Weapons/NordicSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/NordicSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/NordicSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_NordicSword_SkinB40)
{
    mapTo = "NordicSword_SkinB40";
    diffuseMap[0] = "art/Textures/Weapons/NordicSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/NordicSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/NordicSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_NordicSword_SkinB5)
{
    mapTo = "NordicSword_SkinB5";
    diffuseMap[0] = "art/Textures/Weapons/NordicSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/NordicSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/NordicSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_NordicSword_SkinB80)
{
    mapTo = "NordicSword_SkinB80";
    diffuseMap[0] = "art/Textures/Weapons/NordicSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/NordicSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/NordicSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_NordicSword_SkinD)
{
    mapTo = "NordicSword_SkinD";
    diffuseMap[0] = "art/Textures/Weapons/NordicSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/NordicSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/NordicSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_NordicSword_SkinD250)
{
    mapTo = "NordicSword_SkinD250";
    diffuseMap[0] = "art/Textures/Weapons/NordicSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/NordicSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/NordicSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_NordicSword_SkinD40)
{
    mapTo = "NordicSword_SkinD40";
    diffuseMap[0] = "art/Textures/Weapons/NordicSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/NordicSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/NordicSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_NordicSword_SkinD5)
{
    mapTo = "NordicSword_SkinD5";
    diffuseMap[0] = "art/Textures/Weapons/NordicSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/NordicSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/NordicSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_NordicSword_SkinD80)
{
    mapTo = "NordicSword_SkinD80";
    diffuseMap[0] = "art/Textures/Weapons/NordicSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/NordicSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/NordicSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_NordicSword_SkinE)
{
    mapTo = "NordicSword_SkinE";
    diffuseMap[0] = "art/Textures/Weapons/NordicSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/NordicSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/NordicSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_NordicSword_SkinE10)
{
    mapTo = "NordicSword_SkinE10";
    diffuseMap[0] = "art/Textures/Weapons/NordicSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/NordicSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/NordicSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_NordicSword_SkinE100)
{
    mapTo = "NordicSword_SkinE100";
    diffuseMap[0] = "art/Textures/Weapons/NordicSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/NordicSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/NordicSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_NordicSword_SkinE200)
{
    mapTo = "NordicSword_SkinE200";
    diffuseMap[0] = "art/Textures/Weapons/NordicSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/NordicSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/NordicSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_NordicSword_SkinE500)
{
    mapTo = "NordicSword_SkinE500";
    diffuseMap[0] = "art/Textures/Weapons/NordicSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/NordicSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/NordicSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_NordicSword_SkinF)
{
    mapTo = "NordicSword_SkinF";
    diffuseMap[0] = "art/Textures/Weapons/NordicSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/NordicSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/NordicSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_NordicSword_SkinF250)
{
    mapTo = "NordicSword_SkinF250";
    diffuseMap[0] = "art/Textures/Weapons/NordicSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/NordicSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/NordicSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_NordicSword_SkinF40)
{
    mapTo = "NordicSword_SkinF40";
    diffuseMap[0] = "art/Textures/Weapons/NordicSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/NordicSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/NordicSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_NordicSword_SkinF5)
{
    mapTo = "NordicSword_SkinF5";
    diffuseMap[0] = "art/Textures/Weapons/NordicSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/NordicSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/NordicSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_NordicSword_SkinF80)
{
    mapTo = "NordicSword_SkinF80";
    diffuseMap[0] = "art/Textures/Weapons/NordicSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/NordicSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/NordicSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_NordicSword_SkinH)
{
    mapTo = "NordicSword_SkinH";
    diffuseMap[0] = "art/Textures/Weapons/NordicSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/NordicSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/NordicSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_NordicSword_SkinH250)
{
    mapTo = "NordicSword_SkinH250";
    diffuseMap[0] = "art/Textures/Weapons/NordicSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/NordicSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/NordicSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_NordicSword_SkinH40)
{
    mapTo = "NordicSword_SkinH40";
    diffuseMap[0] = "art/Textures/Weapons/NordicSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/NordicSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/NordicSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_NordicSword_SkinH5)
{
    mapTo = "NordicSword_SkinH5";
    diffuseMap[0] = "art/Textures/Weapons/NordicSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/NordicSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/NordicSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_NordicSword_SkinH80)
{
    mapTo = "NordicSword_SkinH80";
    diffuseMap[0] = "art/Textures/Weapons/NordicSword_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/NordicSword_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/NordicSword_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Normal_Horse_base_no_skin)
{
    mapTo = "Normal_Horse_base_no_skin";
    diffuseMap[0] = "art/Textures/Animals/Horse_DIFFUSE_BROWN_01.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Normal_Horse_base_white_no_skin)
{
    mapTo = "Normal_Horse_base_white_no_skin";
    diffuseMap[0] = "art/Textures/Animals/Horse_DIFFUSE_WHITE_01.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded100_Vik_Arm)
{
    mapTo = "Padded100_Vik_Arm";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_100_Vik_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded100_Vik_Arm10)
{
    mapTo = "Padded100_Vik_Arm10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_100_Vik_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded100_Vik_Arm100)
{
    mapTo = "Padded100_Vik_Arm100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_100_Vik_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded100_Vik_Arm200)
{
    mapTo = "Padded100_Vik_Arm200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_100_Vik_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded100_Vik_Arm500)
{
    mapTo = "Padded100_Vik_Arm500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_100_Vik_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded100_Vik_Body)
{
    mapTo = "Padded100_Vik_Body";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_100_Vik_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded100_Vik_Body10)
{
    mapTo = "Padded100_Vik_Body10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_100_Vik_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded100_Vik_Body100)
{
    mapTo = "Padded100_Vik_Body100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_100_Vik_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded100_Vik_Body200)
{
    mapTo = "Padded100_Vik_Body200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_100_Vik_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded100_Vik_Body500)
{
    mapTo = "Padded100_Vik_Body500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_100_Vik_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded100_Vik_Feet)
{
    mapTo = "Padded100_Vik_Feet";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_100_Vik_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded100_Vik_Feet10)
{
    mapTo = "Padded100_Vik_Feet10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_100_Vik_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded100_Vik_Feet100)
{
    mapTo = "Padded100_Vik_Feet100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_100_Vik_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded100_Vik_Feet200)
{
    mapTo = "Padded100_Vik_Feet200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_100_Vik_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded100_Vik_Feet500)
{
    mapTo = "Padded100_Vik_Feet500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_100_Vik_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded100_Vik_Forearm)
{
    mapTo = "Padded100_Vik_Forearm";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_100_Vik_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded100_Vik_Forearm10)
{
    mapTo = "Padded100_Vik_Forearm10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_100_Vik_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded100_Vik_Forearm100)
{
    mapTo = "Padded100_Vik_Forearm100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_100_Vik_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded100_Vik_Forearm200)
{
    mapTo = "Padded100_Vik_Forearm200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_100_Vik_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded100_Vik_Forearm500)
{
    mapTo = "Padded100_Vik_Forearm500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_100_Vik_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded100_Vik_Helmet)
{
    mapTo = "Padded100_Vik_Helmet";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_100_Vik_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded100_Vik_Helmet10)
{
    mapTo = "Padded100_Vik_Helmet10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_100_Vik_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded100_Vik_Helmet100)
{
    mapTo = "Padded100_Vik_Helmet100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_100_Vik_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded100_Vik_Helmet200)
{
    mapTo = "Padded100_Vik_Helmet200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_100_Vik_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded100_Vik_Helmet500)
{
    mapTo = "Padded100_Vik_Helmet500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_100_Vik_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded100_Vik_Legs)
{
    mapTo = "Padded100_Vik_Legs";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_100_Vik_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded100_Vik_Legs10)
{
    mapTo = "Padded100_Vik_Legs10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_100_Vik_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded100_Vik_Legs100)
{
    mapTo = "Padded100_Vik_Legs100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_100_Vik_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded100_Vik_Legs200)
{
    mapTo = "Padded100_Vik_Legs200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_100_Vik_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded100_Vik_Legs500)
{
    mapTo = "Padded100_Vik_Legs500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_100_Vik_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded100_Vik_no_skin)
{
    mapTo = "Padded100_Vik_no_skin";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_100_Vik_DIFFUSE.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded60_Vik_Arm)
{
    mapTo = "Padded60_Vik_Arm";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded60_Vik_Arm10)
{
    mapTo = "Padded60_Vik_Arm10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded60_Vik_Arm100)
{
    mapTo = "Padded60_Vik_Arm100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded60_Vik_Arm200)
{
    mapTo = "Padded60_Vik_Arm200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded60_Vik_Arm500)
{
    mapTo = "Padded60_Vik_Arm500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded60_Vik_Body)
{
    mapTo = "Padded60_Vik_Body";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded60_Vik_Body10)
{
    mapTo = "Padded60_Vik_Body10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded60_Vik_Body100)
{
    mapTo = "Padded60_Vik_Body100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded60_Vik_Body200)
{
    mapTo = "Padded60_Vik_Body200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded60_Vik_Body500)
{
    mapTo = "Padded60_Vik_Body500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded60_Vik_Feet)
{
    mapTo = "Padded60_Vik_Feet";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded60_Vik_Feet10)
{
    mapTo = "Padded60_Vik_Feet10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded60_Vik_Feet100)
{
    mapTo = "Padded60_Vik_Feet100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded60_Vik_Feet200)
{
    mapTo = "Padded60_Vik_Feet200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded60_Vik_Feet500)
{
    mapTo = "Padded60_Vik_Feet500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded60_Vik_Forearm)
{
    mapTo = "Padded60_Vik_Forearm";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded60_Vik_Forearm10)
{
    mapTo = "Padded60_Vik_Forearm10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded60_Vik_Forearm100)
{
    mapTo = "Padded60_Vik_Forearm100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded60_Vik_Forearm200)
{
    mapTo = "Padded60_Vik_Forearm200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded60_Vik_Forearm500)
{
    mapTo = "Padded60_Vik_Forearm500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded60_Vik_HelmetAdd)
{
    mapTo = "Padded60_Vik_HelmetAdd";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded60_Vik_HelmetAdd10)
{
    mapTo = "Padded60_Vik_HelmetAdd10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded60_Vik_HelmetAdd100)
{
    mapTo = "Padded60_Vik_HelmetAdd100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded60_Vik_HelmetAdd200)
{
    mapTo = "Padded60_Vik_HelmetAdd200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded60_Vik_HelmetAdd500)
{
    mapTo = "Padded60_Vik_HelmetAdd500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded60_Vik_Legs)
{
    mapTo = "Padded60_Vik_Legs";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded60_Vik_Legs10)
{
    mapTo = "Padded60_Vik_Legs10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded60_Vik_Legs100)
{
    mapTo = "Padded60_Vik_Legs100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded60_Vik_Legs200)
{
    mapTo = "Padded60_Vik_Legs200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded60_Vik_Legs500)
{
    mapTo = "Padded60_Vik_Legs500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded60_Vik_no_skin)
{
    mapTo = "Padded60_Vik_no_skin";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_60_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded90_Vik_Arm)
{
    mapTo = "Padded90_Vik_Arm";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded90_Vik_Arm10)
{
    mapTo = "Padded90_Vik_Arm10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded90_Vik_Arm100)
{
    mapTo = "Padded90_Vik_Arm100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded90_Vik_Arm200)
{
    mapTo = "Padded90_Vik_Arm200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded90_Vik_Arm500)
{
    mapTo = "Padded90_Vik_Arm500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded90_Vik_Body)
{
    mapTo = "Padded90_Vik_Body";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded90_Vik_Body10)
{
    mapTo = "Padded90_Vik_Body10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded90_Vik_Body100)
{
    mapTo = "Padded90_Vik_Body100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded90_Vik_Body200)
{
    mapTo = "Padded90_Vik_Body200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded90_Vik_Body500)
{
    mapTo = "Padded90_Vik_Body500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded90_Vik_Feet)
{
    mapTo = "Padded90_Vik_Feet";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded90_Vik_Feet10)
{
    mapTo = "Padded90_Vik_Feet10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded90_Vik_Feet100)
{
    mapTo = "Padded90_Vik_Feet100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded90_Vik_Feet200)
{
    mapTo = "Padded90_Vik_Feet200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded90_Vik_Feet500)
{
    mapTo = "Padded90_Vik_Feet500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded90_Vik_Forearm)
{
    mapTo = "Padded90_Vik_Forearm";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded90_Vik_Forearm10)
{
    mapTo = "Padded90_Vik_Forearm10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded90_Vik_Forearm100)
{
    mapTo = "Padded90_Vik_Forearm100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded90_Vik_Forearm200)
{
    mapTo = "Padded90_Vik_Forearm200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded90_Vik_Forearm500)
{
    mapTo = "Padded90_Vik_Forearm500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded90_Vik_Helmet)
{
    mapTo = "Padded90_Vik_Helmet";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded90_Vik_Helmet10)
{
    mapTo = "Padded90_Vik_Helmet10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded90_Vik_Helmet100)
{
    mapTo = "Padded90_Vik_Helmet100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded90_Vik_Helmet200)
{
    mapTo = "Padded90_Vik_Helmet200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded90_Vik_Helmet500)
{
    mapTo = "Padded90_Vik_Helmet500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded90_Vik_Legs)
{
    mapTo = "Padded90_Vik_Legs";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded90_Vik_Legs10)
{
    mapTo = "Padded90_Vik_Legs10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded90_Vik_Legs100)
{
    mapTo = "Padded90_Vik_Legs100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded90_Vik_Legs200)
{
    mapTo = "Padded90_Vik_Legs200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded90_Vik_Legs500)
{
    mapTo = "Padded90_Vik_Legs500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Padded90_Vik_no_skin)
{
    mapTo = "Padded90_Vik_no_skin";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Padded/Padded_90_Vik_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate100_Eur_Arm)
{
    mapTo = "Plate100_Eur_Arm";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_DIFFUSE.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate100_Eur_Arm10)
{
    mapTo = "Plate100_Eur_Arm10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_DIFFUSE.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate100_Eur_Arm100)
{
    mapTo = "Plate100_Eur_Arm100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_DIFFUSE.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate100_Eur_Arm200)
{
    mapTo = "Plate100_Eur_Arm200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_DIFFUSE.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate100_Eur_Arm500)
{
    mapTo = "Plate100_Eur_Arm500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_DIFFUSE.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate100_Eur_Body)
{
    mapTo = "Plate100_Eur_Body";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_DIFFUSE.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate100_Eur_Body10)
{
    mapTo = "Plate100_Eur_Body10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_DIFFUSE.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate100_Eur_Body100)
{
    mapTo = "Plate100_Eur_Body100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_DIFFUSE.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate100_Eur_Body200)
{
    mapTo = "Plate100_Eur_Body200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_DIFFUSE.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate100_Eur_Body500)
{
    mapTo = "Plate100_Eur_Body500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_DIFFUSE.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate100_Eur_Feet)
{
    mapTo = "Plate100_Eur_Feet";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_DIFFUSE.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate100_Eur_Feet10)
{
    mapTo = "Plate100_Eur_Feet10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_DIFFUSE.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate100_Eur_Feet100)
{
    mapTo = "Plate100_Eur_Feet100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_DIFFUSE.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate100_Eur_Feet200)
{
    mapTo = "Plate100_Eur_Feet200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_DIFFUSE.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate100_Eur_Feet500)
{
    mapTo = "Plate100_Eur_Feet500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_DIFFUSE.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate100_Eur_Forearm)
{
    mapTo = "Plate100_Eur_Forearm";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_DIFFUSE.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate100_Eur_Forearm10)
{
    mapTo = "Plate100_Eur_Forearm10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_DIFFUSE.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate100_Eur_Forearm100)
{
    mapTo = "Plate100_Eur_Forearm100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_DIFFUSE.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate100_Eur_Forearm200)
{
    mapTo = "Plate100_Eur_Forearm200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_DIFFUSE.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate100_Eur_Forearm500)
{
    mapTo = "Plate100_Eur_Forearm500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_DIFFUSE.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate100_Eur_Helmet)
{
    mapTo = "Plate100_Eur_Helmet";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_DIFFUSE.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate100_Eur_Helmet10)
{
    mapTo = "Plate100_Eur_Helmet10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_DIFFUSE.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate100_Eur_Helmet100)
{
    mapTo = "Plate100_Eur_Helmet100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_DIFFUSE.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate100_Eur_Helmet200)
{
    mapTo = "Plate100_Eur_Helmet200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_DIFFUSE.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate100_Eur_Helmet500)
{
    mapTo = "Plate100_Eur_Helmet500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_DIFFUSE.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate100_Eur_Legs)
{
    mapTo = "Plate100_Eur_Legs";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_DIFFUSE.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate100_Eur_Legs10)
{
    mapTo = "Plate100_Eur_Legs10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_DIFFUSE.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate100_Eur_Legs100)
{
    mapTo = "Plate100_Eur_Legs100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_DIFFUSE.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate100_Eur_Legs200)
{
    mapTo = "Plate100_Eur_Legs200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_DIFFUSE.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate100_Eur_Legs500)
{
    mapTo = "Plate100_Eur_Legs500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_DIFFUSE.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate100_Eur_no_skin)
{
    mapTo = "Plate100_Eur_no_skin";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_DIFFUSE.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate100_Eur_Shield)
{
    mapTo = "Plate100_Eur_Shield";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_DIFFUSE.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate100_Eur_Shield10)
{
    mapTo = "Plate100_Eur_Shield10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_DIFFUSE.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate100_Eur_Shield100)
{
    mapTo = "Plate100_Eur_Shield100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_DIFFUSE.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate100_Eur_Shield200)
{
    mapTo = "Plate100_Eur_Shield200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_DIFFUSE.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate100_Eur_Shield500)
{
    mapTo = "Plate100_Eur_Shield500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_DIFFUSE.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_100_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate60_Eur_Arm)
{
    mapTo = "Plate60_Eur_Arm";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate60_Eur_Arm10)
{
    mapTo = "Plate60_Eur_Arm10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate60_Eur_Arm100)
{
    mapTo = "Plate60_Eur_Arm100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate60_Eur_Arm200)
{
    mapTo = "Plate60_Eur_Arm200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate60_Eur_Arm500)
{
    mapTo = "Plate60_Eur_Arm500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate60_Eur_Body)
{
    mapTo = "Plate60_Eur_Body";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate60_Eur_Body10)
{
    mapTo = "Plate60_Eur_Body10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate60_Eur_Body100)
{
    mapTo = "Plate60_Eur_Body100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate60_Eur_Body200)
{
    mapTo = "Plate60_Eur_Body200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate60_Eur_Body500)
{
    mapTo = "Plate60_Eur_Body500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate60_Eur_Feet)
{
    mapTo = "Plate60_Eur_Feet";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate60_Eur_Feet10)
{
    mapTo = "Plate60_Eur_Feet10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate60_Eur_Feet100)
{
    mapTo = "Plate60_Eur_Feet100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate60_Eur_Feet200)
{
    mapTo = "Plate60_Eur_Feet200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate60_Eur_Feet500)
{
    mapTo = "Plate60_Eur_Feet500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate60_Eur_Forearm)
{
    mapTo = "Plate60_Eur_Forearm";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate60_Eur_Forearm10)
{
    mapTo = "Plate60_Eur_Forearm10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate60_Eur_Forearm100)
{
    mapTo = "Plate60_Eur_Forearm100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate60_Eur_Forearm200)
{
    mapTo = "Plate60_Eur_Forearm200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate60_Eur_Forearm500)
{
    mapTo = "Plate60_Eur_Forearm500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate60_Eur_Helmet)
{
    mapTo = "Plate60_Eur_Helmet";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate60_Eur_Helmet10)
{
    mapTo = "Plate60_Eur_Helmet10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate60_Eur_Helmet100)
{
    mapTo = "Plate60_Eur_Helmet100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate60_Eur_Helmet200)
{
    mapTo = "Plate60_Eur_Helmet200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate60_Eur_Helmet500)
{
    mapTo = "Plate60_Eur_Helmet500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate60_Eur_Legs)
{
    mapTo = "Plate60_Eur_Legs";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate60_Eur_Legs10)
{
    mapTo = "Plate60_Eur_Legs10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate60_Eur_Legs100)
{
    mapTo = "Plate60_Eur_Legs100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate60_Eur_Legs200)
{
    mapTo = "Plate60_Eur_Legs200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate60_Eur_Legs500)
{
    mapTo = "Plate60_Eur_Legs500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate60_Eur_no_skin)
{
    mapTo = "Plate60_Eur_no_skin";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate60_Eur_Shield)
{
    mapTo = "Plate60_Eur_Shield";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate60_Eur_Shield10)
{
    mapTo = "Plate60_Eur_Shield10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate60_Eur_Shield100)
{
    mapTo = "Plate60_Eur_Shield100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate60_Eur_Shield200)
{
    mapTo = "Plate60_Eur_Shield200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate60_Eur_Shield500)
{
    mapTo = "Plate60_Eur_Shield500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_60_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate90_Eur_Arm)
{
    mapTo = "Plate90_Eur_Arm";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate90_Eur_Arm10)
{
    mapTo = "Plate90_Eur_Arm10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate90_Eur_Arm100)
{
    mapTo = "Plate90_Eur_Arm100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate90_Eur_Arm200)
{
    mapTo = "Plate90_Eur_Arm200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate90_Eur_Arm500)
{
    mapTo = "Plate90_Eur_Arm500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate90_Eur_Body)
{
    mapTo = "Plate90_Eur_Body";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate90_Eur_Body10)
{
    mapTo = "Plate90_Eur_Body10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate90_Eur_Body100)
{
    mapTo = "Plate90_Eur_Body100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate90_Eur_Body200)
{
    mapTo = "Plate90_Eur_Body200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate90_Eur_Body500)
{
    mapTo = "Plate90_Eur_Body500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate90_Eur_Feet)
{
    mapTo = "Plate90_Eur_Feet";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate90_Eur_Feet10)
{
    mapTo = "Plate90_Eur_Feet10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate90_Eur_Feet100)
{
    mapTo = "Plate90_Eur_Feet100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate90_Eur_Feet200)
{
    mapTo = "Plate90_Eur_Feet200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate90_Eur_Feet500)
{
    mapTo = "Plate90_Eur_Feet500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate90_Eur_Forearm)
{
    mapTo = "Plate90_Eur_Forearm";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate90_Eur_Forearm10)
{
    mapTo = "Plate90_Eur_Forearm10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate90_Eur_Forearm100)
{
    mapTo = "Plate90_Eur_Forearm100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate90_Eur_Forearm200)
{
    mapTo = "Plate90_Eur_Forearm200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate90_Eur_Forearm500)
{
    mapTo = "Plate90_Eur_Forearm500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate90_Eur_Helmet)
{
    mapTo = "Plate90_Eur_Helmet";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate90_Eur_Helmet10)
{
    mapTo = "Plate90_Eur_Helmet10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate90_Eur_Helmet100)
{
    mapTo = "Plate90_Eur_Helmet100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate90_Eur_Helmet200)
{
    mapTo = "Plate90_Eur_Helmet200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate90_Eur_Helmet500)
{
    mapTo = "Plate90_Eur_Helmet500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate90_Eur_Legs)
{
    mapTo = "Plate90_Eur_Legs";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate90_Eur_Legs10)
{
    mapTo = "Plate90_Eur_Legs10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate90_Eur_Legs100)
{
    mapTo = "Plate90_Eur_Legs100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate90_Eur_Legs200)
{
    mapTo = "Plate90_Eur_Legs200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate90_Eur_Legs500)
{
    mapTo = "Plate90_Eur_Legs500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate90_Eur_no_skin)
{
    mapTo = "Plate90_Eur_no_skin";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate90_Eur_Shield)
{
    mapTo = "Plate90_Eur_Shield";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate90_Eur_Shield10)
{
    mapTo = "Plate90_Eur_Shield10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate90_Eur_Shield100)
{
    mapTo = "Plate90_Eur_Shield100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate90_Eur_Shield200)
{
    mapTo = "Plate90_Eur_Shield200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Plate90_Eur_Shield500)
{
    mapTo = "Plate90_Eur_Shield500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Plate/Plate_90_Eur_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale60_Mon_Arm)
{
    mapTo = "Scale60_Mon_Arm";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale60_Mon_Arm10)
{
    mapTo = "Scale60_Mon_Arm10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale60_Mon_Arm100)
{
    mapTo = "Scale60_Mon_Arm100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale60_Mon_Arm200)
{
    mapTo = "Scale60_Mon_Arm200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale60_Mon_Arm500)
{
    mapTo = "Scale60_Mon_Arm500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale60_Mon_Body)
{
    mapTo = "Scale60_Mon_Body";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale60_Mon_Body10)
{
    mapTo = "Scale60_Mon_Body10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale60_Mon_Body100)
{
    mapTo = "Scale60_Mon_Body100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale60_Mon_Body200)
{
    mapTo = "Scale60_Mon_Body200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale60_Mon_Body500)
{
    mapTo = "Scale60_Mon_Body500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale60_Mon_Feet)
{
    mapTo = "Scale60_Mon_Feet";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale60_Mon_Feet10)
{
    mapTo = "Scale60_Mon_Feet10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale60_Mon_Feet100)
{
    mapTo = "Scale60_Mon_Feet100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale60_Mon_Feet200)
{
    mapTo = "Scale60_Mon_Feet200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale60_Mon_Feet500)
{
    mapTo = "Scale60_Mon_Feet500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale60_Mon_Forearm)
{
    mapTo = "Scale60_Mon_Forearm";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale60_Mon_Forearm10)
{
    mapTo = "Scale60_Mon_Forearm10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale60_Mon_Forearm100)
{
    mapTo = "Scale60_Mon_Forearm100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale60_Mon_Forearm200)
{
    mapTo = "Scale60_Mon_Forearm200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale60_Mon_Forearm500)
{
    mapTo = "Scale60_Mon_Forearm500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale60_Mon_Helmet)
{
    mapTo = "Scale60_Mon_Helmet";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale60_Mon_Helmet10)
{
    mapTo = "Scale60_Mon_Helmet10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale60_Mon_Helmet100)
{
    mapTo = "Scale60_Mon_Helmet100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale60_Mon_Helmet200)
{
    mapTo = "Scale60_Mon_Helmet200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale60_Mon_Helmet500)
{
    mapTo = "Scale60_Mon_Helmet500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale60_Mon_Legs)
{
    mapTo = "Scale60_Mon_Legs";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale60_Mon_Legs10)
{
    mapTo = "Scale60_Mon_Legs10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale60_Mon_Legs100)
{
    mapTo = "Scale60_Mon_Legs100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale60_Mon_Legs200)
{
    mapTo = "Scale60_Mon_Legs200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale60_Mon_Legs500)
{
    mapTo = "Scale60_Mon_Legs500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale60_Mon_no_skin)
{
    mapTo = "Scale60_Mon_no_skin";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_60_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale90_Mon_Arm)
{
    mapTo = "Scale90_Mon_Arm";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale90_Mon_Arm10)
{
    mapTo = "Scale90_Mon_Arm10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale90_Mon_Arm100)
{
    mapTo = "Scale90_Mon_Arm100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale90_Mon_Arm200)
{
    mapTo = "Scale90_Mon_Arm200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale90_Mon_Arm500)
{
    mapTo = "Scale90_Mon_Arm500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale90_Mon_Body)
{
    mapTo = "Scale90_Mon_Body";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale90_Mon_Body10)
{
    mapTo = "Scale90_Mon_Body10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale90_Mon_Body100)
{
    mapTo = "Scale90_Mon_Body100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale90_Mon_Body200)
{
    mapTo = "Scale90_Mon_Body200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale90_Mon_Body500)
{
    mapTo = "Scale90_Mon_Body500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale90_Mon_Feet)
{
    mapTo = "Scale90_Mon_Feet";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale90_Mon_Feet10)
{
    mapTo = "Scale90_Mon_Feet10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale90_Mon_Feet100)
{
    mapTo = "Scale90_Mon_Feet100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale90_Mon_Feet200)
{
    mapTo = "Scale90_Mon_Feet200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale90_Mon_Feet500)
{
    mapTo = "Scale90_Mon_Feet500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale90_Mon_Forearm)
{
    mapTo = "Scale90_Mon_Forearm";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale90_Mon_Forearm10)
{
    mapTo = "Scale90_Mon_Forearm10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale90_Mon_Forearm100)
{
    mapTo = "Scale90_Mon_Forearm100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale90_Mon_Forearm200)
{
    mapTo = "Scale90_Mon_Forearm200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale90_Mon_Forearm500)
{
    mapTo = "Scale90_Mon_Forearm500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale90_Mon_Helmet)
{
    mapTo = "Scale90_Mon_Helmet";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale90_Mon_Helmet10)
{
    mapTo = "Scale90_Mon_Helmet10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale90_Mon_Helmet100)
{
    mapTo = "Scale90_Mon_Helmet100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale90_Mon_Helmet200)
{
    mapTo = "Scale90_Mon_Helmet200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale90_Mon_Helmet500)
{
    mapTo = "Scale90_Mon_Helmet500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale90_Mon_Legs)
{
    mapTo = "Scale90_Mon_Legs";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale90_Mon_Legs10)
{
    mapTo = "Scale90_Mon_Legs10";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale90_Mon_Legs100)
{
    mapTo = "Scale90_Mon_Legs100";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale90_Mon_Legs200)
{
    mapTo = "Scale90_Mon_Legs200";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale90_Mon_Legs500)
{
    mapTo = "Scale90_Mon_Legs500";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scale90_Mon_no_skin)
{
    mapTo = "Scale90_Mon_no_skin";
    diffuseMap[0] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/CharacterTextures/Armors/Scale/Scale_90_Mon_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_scimitar_pack1_ns)
{
    mapTo = "scimitar_pack1_ns";
    diffuseMap[0] = "art/Textures/Weapons/Scimitar_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Scimitar_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Scimitar_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scimitar_SkinA)
{
    mapTo = "Scimitar_SkinA";
    diffuseMap[0] = "art/Textures/Weapons/Scimitar_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Scimitar_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Scimitar_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scimitar_SkinA10)
{
    mapTo = "Scimitar_SkinA10";
    diffuseMap[0] = "art/Textures/Weapons/Scimitar_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Scimitar_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Scimitar_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scimitar_SkinA100)
{
    mapTo = "Scimitar_SkinA100";
    diffuseMap[0] = "art/Textures/Weapons/Scimitar_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Scimitar_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Scimitar_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scimitar_SkinA200)
{
    mapTo = "Scimitar_SkinA200";
    diffuseMap[0] = "art/Textures/Weapons/Scimitar_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Scimitar_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Scimitar_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Scimitar_SkinA500)
{
    mapTo = "Scimitar_SkinA500";
    diffuseMap[0] = "art/Textures/Weapons/Scimitar_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Scimitar_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Scimitar_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shelf_A)
{
    mapTo = "Shelf_A";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack3_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack3_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack3_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shelf_A250)
{
    mapTo = "Shelf_A250";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack3_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack3_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack3_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shelf_A40)
{
    mapTo = "Shelf_A40";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack3_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack3_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack3_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shelf_A5)
{
    mapTo = "Shelf_A5";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack3_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack3_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack3_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shelf_A80)
{
    mapTo = "Shelf_A80";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack3_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack3_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack3_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_HeaterA_ns1)
{
    mapTo = "Shield_HeaterA_ns1";
    diffuseMap[0] = "art/Textures/Shields/Shield_HeaterA_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_HeaterA_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_HeaterA_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_HeaterA_ns1C)
{
    mapTo = "Shield_HeaterA_ns1C";
    diffuseMap[0] = "art/Textures/Shields/Shield_HeaterA_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_HeaterA_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_HeaterA_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_HeaterB_A)
{
    mapTo = "Shield_HeaterB_A";
    diffuseMap[0] = "art/Textures/Shields/Shield_HeaterB_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_HeaterB_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_HeaterB_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_HeaterB_A10)
{
    mapTo = "Shield_HeaterB_A10";
    diffuseMap[0] = "art/Textures/Shields/Shield_HeaterB_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_HeaterB_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_HeaterB_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_HeaterB_A100)
{
    mapTo = "Shield_HeaterB_A100";
    diffuseMap[0] = "art/Textures/Shields/Shield_HeaterB_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_HeaterB_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_HeaterB_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_HeaterB_A200)
{
    mapTo = "Shield_HeaterB_A200";
    diffuseMap[0] = "art/Textures/Shields/Shield_HeaterB_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_HeaterB_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_HeaterB_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_HeaterB_A500)
{
    mapTo = "Shield_HeaterB_A500";
    diffuseMap[0] = "art/Textures/Shields/Shield_HeaterB_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_HeaterB_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_HeaterB_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_HeaterB_ns)
{
    mapTo = "Shield_HeaterB_ns";
    diffuseMap[0] = "art/Textures/Shields/Shield_HeaterB_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_HeaterB_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_HeaterB_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_HeaterB_nsC)
{
    mapTo = "Shield_HeaterB_nsC";
    diffuseMap[0] = "art/Textures/Shields/Shield_HeaterB_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_HeaterB_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_HeaterB_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_HeaterREG_A)
{
    mapTo = "Shield_HeaterREG_A";
    diffuseMap[0] = "art/Textures/Shields/Shield_HeaterREG_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_HeaterREG_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_HeaterREG_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_HeaterREG_A10)
{
    mapTo = "Shield_HeaterREG_A10";
    diffuseMap[0] = "art/Textures/Shields/Shield_HeaterREG_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_HeaterREG_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_HeaterREG_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_HeaterREG_A100)
{
    mapTo = "Shield_HeaterREG_A100";
    diffuseMap[0] = "art/Textures/Shields/Shield_HeaterREG_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_HeaterREG_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_HeaterREG_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_HeaterREG_A200)
{
    mapTo = "Shield_HeaterREG_A200";
    diffuseMap[0] = "art/Textures/Shields/Shield_HeaterREG_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_HeaterREG_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_HeaterREG_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_HeaterREG_A500)
{
    mapTo = "Shield_HeaterREG_A500";
    diffuseMap[0] = "art/Textures/Shields/Shield_HeaterREG_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_HeaterREG_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_HeaterREG_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_HeaterREG_ns)
{
    mapTo = "Shield_HeaterREG_ns";
    diffuseMap[0] = "art/Textures/Shields/Shield_HeaterREG_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_HeaterREG_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_HeaterREG_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_HeaterREG_ns1)
{
    mapTo = "Shield_HeaterREG_ns1";
    diffuseMap[0] = "art/Textures/Shields/Shield_HeaterREG_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_HeaterREG_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_HeaterREG_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_HeaterREG_nsC)
{
    mapTo = "Shield_HeaterREG_nsC";
    diffuseMap[0] = "art/Textures/Shields/Shield_HeaterREG_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_HeaterREG_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_HeaterREG_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_Kite_LOD_A)
{
    mapTo = "Shield_Kite_LOD_A";
    diffuseMap[0] = "art/Textures/Shields/Shield_Kite_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_Kite_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_Kite_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_Kite_LOD_A10)
{
    mapTo = "Shield_Kite_LOD_A10";
    diffuseMap[0] = "art/Textures/Shields/Shield_Kite_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_Kite_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_Kite_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_Kite_LOD_A100)
{
    mapTo = "Shield_Kite_LOD_A100";
    diffuseMap[0] = "art/Textures/Shields/Shield_Kite_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_Kite_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_Kite_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_Kite_LOD_A200)
{
    mapTo = "Shield_Kite_LOD_A200";
    diffuseMap[0] = "art/Textures/Shields/Shield_Kite_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_Kite_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_Kite_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_Kite_LOD_A500)
{
    mapTo = "Shield_Kite_LOD_A500";
    diffuseMap[0] = "art/Textures/Shields/Shield_Kite_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_Kite_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_Kite_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_Kite_ns)
{
    mapTo = "Shield_Kite_ns";
    diffuseMap[0] = "art/Textures/Shields/Shield_Kite_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_Kite_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_Kite_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_Kite_nsC)
{
    mapTo = "Shield_Kite_nsC";
    diffuseMap[0] = "art/Textures/Shields/Shield_Kite_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_Kite_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_Kite_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_KiteREG_A)
{
    mapTo = "Shield_KiteREG_A";
    diffuseMap[0] = "art/Textures/Shields/Shield_KiteREG_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_KiteREG_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_KiteREG_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_KiteREG_A10)
{
    mapTo = "Shield_KiteREG_A10";
    diffuseMap[0] = "art/Textures/Shields/Shield_KiteREG_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_KiteREG_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_KiteREG_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_KiteREG_A100)
{
    mapTo = "Shield_KiteREG_A100";
    diffuseMap[0] = "art/Textures/Shields/Shield_KiteREG_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_KiteREG_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_KiteREG_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_KiteREG_A200)
{
    mapTo = "Shield_KiteREG_A200";
    diffuseMap[0] = "art/Textures/Shields/Shield_KiteREG_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_KiteREG_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_KiteREG_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_KiteREG_A500)
{
    mapTo = "Shield_KiteREG_A500";
    diffuseMap[0] = "art/Textures/Shields/Shield_KiteREG_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_KiteREG_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_KiteREG_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_KiteREG_ns)
{
    mapTo = "Shield_KiteREG_ns";
    diffuseMap[0] = "art/Textures/Shields/Shield_KiteREG_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_KiteREG_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_KiteREG_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_KiteREG_nsC)
{
    mapTo = "Shield_KiteREG_nsC";
    diffuseMap[0] = "art/Textures/Shields/Shield_KiteREG_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_KiteREG_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_KiteREG_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_SteelRound_A)
{
    mapTo = "Shield_SteelRound_A";
    diffuseMap[0] = "art/Textures/Shields/Shield_SteelRound_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_SteelRound_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_SteelRound_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_SteelRound_A10)
{
    mapTo = "Shield_SteelRound_A10";
    diffuseMap[0] = "art/Textures/Shields/Shield_SteelRound_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_SteelRound_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_SteelRound_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_SteelRound_A100)
{
    mapTo = "Shield_SteelRound_A100";
    diffuseMap[0] = "art/Textures/Shields/Shield_SteelRound_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_SteelRound_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_SteelRound_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_SteelRound_A200)
{
    mapTo = "Shield_SteelRound_A200";
    diffuseMap[0] = "art/Textures/Shields/Shield_SteelRound_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_SteelRound_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_SteelRound_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_SteelRound_A500)
{
    mapTo = "Shield_SteelRound_A500";
    diffuseMap[0] = "art/Textures/Shields/Shield_SteelRound_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_SteelRound_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_SteelRound_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_SteelRound_ns)
{
    mapTo = "Shield_SteelRound_ns";
    diffuseMap[0] = "art/Textures/Shields/Shield_SteelRound_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_SteelRound_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_SteelRound_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_SteelRound_nsC)
{
    mapTo = "Shield_SteelRound_nsC";
    diffuseMap[0] = "art/Textures/Shields/Shield_SteelRound_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_SteelRound_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_SteelRound_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_SteelRoundREG_no_customization)
{
    mapTo = "Shield_SteelRoundREG_no_customization";
    diffuseMap[0] = "art/Textures/Shields/Shield_SteelRoundREG_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_SteelRoundREG_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_SteelRoundREG_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_Tower_A)
{
    mapTo = "Shield_Tower_A";
    diffuseMap[0] = "art/Textures/Shields/Shield_Tower_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_Tower_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_Tower_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_Tower_A10)
{
    mapTo = "Shield_Tower_A10";
    diffuseMap[0] = "art/Textures/Shields/Shield_Tower_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_Tower_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_Tower_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_Tower_A100)
{
    mapTo = "Shield_Tower_A100";
    diffuseMap[0] = "art/Textures/Shields/Shield_Tower_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_Tower_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_Tower_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_Tower_A200)
{
    mapTo = "Shield_Tower_A200";
    diffuseMap[0] = "art/Textures/Shields/Shield_Tower_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_Tower_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_Tower_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_Tower_A500)
{
    mapTo = "Shield_Tower_A500";
    diffuseMap[0] = "art/Textures/Shields/Shield_Tower_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_Tower_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_Tower_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_Tower_ns)
{
    mapTo = "Shield_Tower_ns";
    diffuseMap[0] = "art/Textures/Shields/Shield_Tower_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_Tower_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_Tower_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_Tower_nsC)
{
    mapTo = "Shield_Tower_nsC";
    diffuseMap[0] = "art/Textures/Shields/Shield_Tower_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_Tower_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_Tower_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_WoodenRound_A)
{
    mapTo = "Shield_WoodenRound_A";
    diffuseMap[0] = "art/Textures/Shields/Shield_WoodenRound_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_WoodenRound_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_WoodenRound_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_WoodenRound_A10)
{
    mapTo = "Shield_WoodenRound_A10";
    diffuseMap[0] = "art/Textures/Shields/Shield_WoodenRound_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_WoodenRound_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_WoodenRound_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_WoodenRound_A100)
{
    mapTo = "Shield_WoodenRound_A100";
    diffuseMap[0] = "art/Textures/Shields/Shield_WoodenRound_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_WoodenRound_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_WoodenRound_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_WoodenRound_A200)
{
    mapTo = "Shield_WoodenRound_A200";
    diffuseMap[0] = "art/Textures/Shields/Shield_WoodenRound_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_WoodenRound_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_WoodenRound_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_WoodenRound_A500)
{
    mapTo = "Shield_WoodenRound_A500";
    diffuseMap[0] = "art/Textures/Shields/Shield_WoodenRound_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_WoodenRound_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_WoodenRound_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_WoodenRound_ns)
{
    mapTo = "Shield_WoodenRound_ns";
    diffuseMap[0] = "art/Textures/Shields/Shield_WoodenRound_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_WoodenRound_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_WoodenRound_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_WoodenRound_ns1)
{
    mapTo = "Shield_WoodenRound_ns1";
    diffuseMap[0] = "art/Textures/Shields/Shield_WoodenRound_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_WoodenRound_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_WoodenRound_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_WoodenRound_ns1C)
{
    mapTo = "Shield_WoodenRound_ns1C";
    diffuseMap[0] = "art/Textures/Shields/Shield_WoodenRound_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_WoodenRound_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_WoodenRound_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_WoodenRound_ns2)
{
    mapTo = "Shield_WoodenRound_ns2";
    diffuseMap[0] = "art/Textures/Shields/Shield_WoodenRound_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_WoodenRound_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_WoodenRound_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_WoodenRound_ns2C)
{
    mapTo = "Shield_WoodenRound_ns2C";
    diffuseMap[0] = "art/Textures/Shields/Shield_WoodenRound_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_WoodenRound_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_WoodenRound_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_WoodenRound_nsC)
{
    mapTo = "Shield_WoodenRound_nsC";
    diffuseMap[0] = "art/Textures/Shields/Shield_WoodenRound_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_WoodenRound_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_WoodenRound_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_WoodenRoundREG_A_ns)
{
    mapTo = "Shield_WoodenRoundREG_A_ns";
    diffuseMap[0] = "art/Textures/Shields/Shield_WoodenRoundREG_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_WoodenRoundREG_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_WoodenRoundREG_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_WoodenRoundREG_A_nsC)
{
    mapTo = "Shield_WoodenRoundREG_A_nsC";
    diffuseMap[0] = "art/Textures/Shields/Shield_WoodenRoundREG_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_WoodenRoundREG_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_WoodenRoundREG_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_WoodenRoundREG_A10)
{
    mapTo = "Shield_WoodenRoundREG_A10";
    diffuseMap[0] = "art/Textures/Shields/Shield_WoodenRoundREG_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_WoodenRoundREG_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_WoodenRoundREG_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_WoodenRoundREG_A100)
{
    mapTo = "Shield_WoodenRoundREG_A100";
    diffuseMap[0] = "art/Textures/Shields/Shield_WoodenRoundREG_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_WoodenRoundREG_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_WoodenRoundREG_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_WoodenRoundREG_A200)
{
    mapTo = "Shield_WoodenRoundREG_A200";
    diffuseMap[0] = "art/Textures/Shields/Shield_WoodenRoundREG_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_WoodenRoundREG_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_WoodenRoundREG_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Shield_WoodenRoundREG_A500)
{
    mapTo = "Shield_WoodenRoundREG_A500";
    diffuseMap[0] = "art/Textures/Shields/Shield_WoodenRoundREG_A_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Shields/Shield_WoodenRoundREG_A_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Shields/Shield_WoodenRoundREG_A_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Wood_Head)
{
    mapTo = "Wood_Head";
    diffuseMap[0] = "art/Textures/TextureLib/Dummy_Head_Diffuse.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Dummy_Head_Normal.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Wood_Head10)
{
    mapTo = "Wood_Head10";
    diffuseMap[0] = "art/Textures/TextureLib/Dummy_Head_Diffuse.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Dummy_Head_Normal.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Wood_Head100)
{
    mapTo = "Wood_Head100";
    diffuseMap[0] = "art/Textures/TextureLib/Dummy_Head_Diffuse.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Dummy_Head_Normal.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Wood_Head200)
{
    mapTo = "Wood_Head200";
    diffuseMap[0] = "art/Textures/TextureLib/Dummy_Head_Diffuse.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Dummy_Head_Normal.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Wood_Head500)
{
    mapTo = "Wood_Head500";
    diffuseMap[0] = "art/Textures/TextureLib/Dummy_Head_Diffuse.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Dummy_Head_Normal.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Wood_HeadC)
{
    mapTo = "Wood_HeadC";
    diffuseMap[0] = "art/Textures/TextureLib/Dummy_Head_Diffuse.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Dummy_Head_Normal.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Zweihander_DIFFUSEC)
{
    mapTo = "Zweihander_DIFFUSEC";
    diffuseMap[0] = "art/Textures/Weapons/Zweihander_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Zweihander_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Zweihander_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Zweihander_Skin)
{
    mapTo = "Zweihander_Skin";
    diffuseMap[0] = "art/Textures/Weapons/Zweihander_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Zweihander_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Zweihander_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Zweihander_SkinA)
{
    mapTo = "Zweihander_SkinA";
    diffuseMap[0] = "art/Textures/Weapons/Zweihander_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Zweihander_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Zweihander_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Zweihander_SkinA10)
{
    mapTo = "Zweihander_SkinA10";
    diffuseMap[0] = "art/Textures/Weapons/Zweihander_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Zweihander_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Zweihander_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Zweihander_SkinA100)
{
    mapTo = "Zweihander_SkinA100";
    diffuseMap[0] = "art/Textures/Weapons/Zweihander_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Zweihander_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Zweihander_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Zweihander_SkinA200)
{
    mapTo = "Zweihander_SkinA200";
    diffuseMap[0] = "art/Textures/Weapons/Zweihander_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Zweihander_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Zweihander_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Zweihander_SkinA500)
{
    mapTo = "Zweihander_SkinA500";
    diffuseMap[0] = "art/Textures/Weapons/Zweihander_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Zweihander_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Zweihander_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Zweihander_SkinC)
{
    mapTo = "Zweihander_SkinC";
    diffuseMap[0] = "art/Textures/Weapons/Zweihander_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Zweihander_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Zweihander_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Zweihander_SkinC10)
{
    mapTo = "Zweihander_SkinC10";
    diffuseMap[0] = "art/Textures/Weapons/Zweihander_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Zweihander_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Zweihander_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Zweihander_SkinC100)
{
    mapTo = "Zweihander_SkinC100";
    diffuseMap[0] = "art/Textures/Weapons/Zweihander_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Zweihander_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Zweihander_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Zweihander_SkinC200)
{
    mapTo = "Zweihander_SkinC200";
    diffuseMap[0] = "art/Textures/Weapons/Zweihander_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Zweihander_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Zweihander_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_D_Zweihander_SkinC500)
{
    mapTo = "Zweihander_SkinC500";
    diffuseMap[0] = "art/Textures/Weapons/Zweihander_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/Weapons/Zweihander_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/Weapons/Zweihander_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_F_Bed_A)
{
    mapTo = "Bed_A";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_F_Bed_A40)
{
    mapTo = "Bed_A40";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_F_Bed_A5)
{
    mapTo = "Bed_A5";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_F_Bed_A70)
{
    mapTo = "Bed_A70";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_F_Bench_A)
{
    mapTo = "Bench_A";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack3_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack3_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack3_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_F_Bench_A40)
{
    mapTo = "Bench_A40";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack3_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack3_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack3_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_F_Bench_A5)
{
    mapTo = "Bench_A5";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack3_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack3_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack3_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_F_Bench_A70)
{
    mapTo = "Bench_A70";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack3_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack3_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack3_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_F_Bench_B)
{
    mapTo = "Bench_B";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack3_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack3_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack3_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_F_Bench_B40)
{
    mapTo = "Bench_B40";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack3_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack3_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack3_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_F_Bench_B5)
{
    mapTo = "Bench_B5";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack3_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack3_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack3_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_F_Bench_B70)
{
    mapTo = "Bench_B70";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack3_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack3_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack3_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_F_Bench_C)
{
    mapTo = "Bench_C";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack2_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack2_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack2_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_F_Bench_C40)
{
    mapTo = "Bench_C40";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack2_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack2_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack2_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_F_Bench_C5)
{
    mapTo = "Bench_C5";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack2_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack2_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack2_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_F_Bench_C70)
{
    mapTo = "Bench_C70";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack2_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack2_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack2_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_F_furniture_pack1_diffC)
{
    mapTo = "furniture_pack1_diffC";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_F_furniture_pack1_diff)
{
    mapTo = "furniture_pack1_diff";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack1_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack1_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack1_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_F_furniture_pack2_diffC)
{
    mapTo = "furniture_pack2_diffC";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack2_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack2_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack2_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_F_furniture_pack2_diff)
{
    mapTo = "furniture_pack2_diff";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack2_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack2_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack2_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_F_sleepingbag)
{
    mapTo = "sleepingbag";
    diffuseMap[0] = "art/Textures/Atlas/bedSmall_diff.dds";
    diffuseMap[1] = "art/Textures/Atlas/bedSmall_nm.dds";
    diffuseMap[2] = "art/Textures/Atlas/bedSmall_spec.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_F_sleepingbag25)
{
    mapTo = "sleepingbag25";
    diffuseMap[0] = "art/Textures/Atlas/bedSmall_diff.dds";
    diffuseMap[1] = "art/Textures/Atlas/bedSmall_nm.dds";
    diffuseMap[2] = "art/Textures/Atlas/bedSmall_spec.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_F_SleepingBag3)
{
    mapTo = "SleepingBag3";
    diffuseMap[0] = "art/Textures/Atlas/bedSmall_diff.dds";
    diffuseMap[1] = "art/Textures/Atlas/bedSmall_nm.dds";
    diffuseMap[2] = "art/Textures/Atlas/bedSmall_spec.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_F_sleepingbag50)
{
    mapTo = "sleepingbag50";
    diffuseMap[0] = "art/Textures/Atlas/bedSmall_diff.dds";
    diffuseMap[1] = "art/Textures/Atlas/bedSmall_nm.dds";
    diffuseMap[2] = "art/Textures/Atlas/bedSmall_spec.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_F_Table_A)
{
    mapTo = "Table_A";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack2_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack2_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack2_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_F_Table_A40)
{
    mapTo = "Table_A40";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack2_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack2_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack2_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_F_Table_A5)
{
    mapTo = "Table_A5";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack2_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack2_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack2_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_F_Table_A70)
{
    mapTo = "Table_A70";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack2_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack2_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack2_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_F_Table_B)
{
    mapTo = "Table_B";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack2_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack2_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack2_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_F_Table_B40)
{
    mapTo = "Table_B40";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack2_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack2_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack2_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_F_Table_B5)
{
    mapTo = "Table_B5";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack2_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack2_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack2_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_F_Table_B70)
{
    mapTo = "Table_B70";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack2_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack2_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack2_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_F_Throne_A)
{
    mapTo = "Throne_A";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack3_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack3_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack3_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_F_Throne_A40)
{
    mapTo = "Throne_A40";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack3_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack3_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack3_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_F_Throne_A5)
{
    mapTo = "Throne_A5";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack3_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack3_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack3_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_F_Throne_A70)
{
    mapTo = "Throne_A70";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack3_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack3_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack3_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_S_Statue_TusgaalC)
{
    mapTo = "Statue_TusgaalC";
    diffuseMap[0] = "art/Textures/TextureLib/Statue_Tusgaal_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Statue_Tusgaal_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Statue_Tusgaal_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_S_Statue_Tusgaal10)
{
    mapTo = "Statue_Tusgaal10";
    diffuseMap[0] = "art/Textures/TextureLib/Statue_Tusgaal_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Statue_Tusgaal_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Statue_Tusgaal_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_S_Statue_Tusgaal100)
{
    mapTo = "Statue_Tusgaal100";
    diffuseMap[0] = "art/Textures/TextureLib/Statue_Tusgaal_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Statue_Tusgaal_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Statue_Tusgaal_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_S_Statue_Tusgaal250)
{
    mapTo = "Statue_Tusgaal250";
    diffuseMap[0] = "art/Textures/TextureLib/Statue_Tusgaal_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Statue_Tusgaal_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Statue_Tusgaal_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_S_Statue_Tusgaal500)
{
    mapTo = "Statue_Tusgaal500";
    diffuseMap[0] = "art/Textures/TextureLib/Statue_Tusgaal_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Statue_Tusgaal_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Statue_Tusgaal_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_S_Statue_McTirC)
{
    mapTo = "Statue_McTirC";
    diffuseMap[0] = "art/Textures/TextureLib/Statue_McTir_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Statue_McTir_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Statue_McTir_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_S_Statue_McTir10)
{
    mapTo = "Statue_McTir10";
    diffuseMap[0] = "art/Textures/TextureLib/Statue_McTir_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Statue_McTir_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Statue_McTir_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_S_Statue_McTir100)
{
    mapTo = "Statue_McTir100";
    diffuseMap[0] = "art/Textures/TextureLib/Statue_McTir_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Statue_McTir_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Statue_McTir_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_S_Statue_McTir250)
{
    mapTo = "Statue_McTir250";
    diffuseMap[0] = "art/Textures/TextureLib/Statue_McTir_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Statue_McTir_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Statue_McTir_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_S_Statue_McTir500)
{
    mapTo = "Statue_McTir500";
    diffuseMap[0] = "art/Textures/TextureLib/Statue_McTir_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Statue_McTir_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Statue_McTir_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_S_Statue_SovereignC)
{
    mapTo = "Statue_SovereignC";
    diffuseMap[0] = "art/Textures/TextureLib/Statue_Sovereign_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Statue_Sovereign_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Statue_Sovereign_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_S_Statue_Sovereign10)
{
    mapTo = "Statue_Sovereign10";
    diffuseMap[0] = "art/Textures/TextureLib/Statue_Sovereign_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Statue_Sovereign_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Statue_Sovereign_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_S_Statue_Sovereign100)
{
    mapTo = "Statue_Sovereign100";
    diffuseMap[0] = "art/Textures/TextureLib/Statue_Sovereign_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Statue_Sovereign_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Statue_Sovereign_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_S_Statue_Sovereign250)
{
    mapTo = "Statue_Sovereign250";
    diffuseMap[0] = "art/Textures/TextureLib/Statue_Sovereign_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Statue_Sovereign_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Statue_Sovereign_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_S_Statue_Sovereign500)
{
    mapTo = "Statue_Sovereign500";
    diffuseMap[0] = "art/Textures/TextureLib/Statue_Sovereign_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Statue_Sovereign_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Statue_Sovereign_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_S_Statue_Sovereign_man)
{
    mapTo = "Statue_Sovereign_man";
    diffuseMap[0] = "art/Textures/TextureLib/Statue_Sovereign_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Statue_Sovereign_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Statue_Sovereign_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_S_Statue_Sovereign_man500)
{
    mapTo = "Statue_Sovereign_man500";
    diffuseMap[0] = "art/Textures/TextureLib/Statue_Sovereign_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Statue_Sovereign_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Statue_Sovereign_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_S_Statue_Sovereign_man100)
{
    mapTo = "Statue_Sovereign_man100";
    diffuseMap[0] = "art/Textures/TextureLib/Statue_Sovereign_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Statue_Sovereign_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Statue_Sovereign_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_S_Statue_Sovereign_man10)
{
    mapTo = "Statue_Sovereign_man10";
    diffuseMap[0] = "art/Textures/TextureLib/Statue_Sovereign_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Statue_Sovereign_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Statue_Sovereign_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_S_Statue_Sovereign_man250)
{
    mapTo = "Statue_Sovereign_man250";
    diffuseMap[0] = "art/Textures/TextureLib/Statue_Sovereign_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Statue_Sovereign_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Statue_Sovereign_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_C_Chest_A)
{
    mapTo = "Chest_A";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack4_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack4_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack4_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_C_Chest_A40)
{
    mapTo = "Chest_A40";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack4_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack4_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack4_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_C_Chest_A5)
{
    mapTo = "Chest_A5";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack4_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack4_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack4_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_C_Chest_A70)
{
    mapTo = "Chest_A70";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack4_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack4_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack4_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_C_Chest_B)
{
    mapTo = "Chest_B";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack4_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack4_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack4_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_C_Chest_B40)
{
    mapTo = "Chest_B40";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack4_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack4_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack4_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_C_Chest_B5)
{
    mapTo = "Chest_B5";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack4_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack4_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack4_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_C_Chest_B70)
{
    mapTo = "Chest_B70";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack4_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack4_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack4_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_C_Chest_C)
{
    mapTo = "Chest_C";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack4_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack4_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack4_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_C_Chest_C40)
{
    mapTo = "Chest_C40";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack4_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack4_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack4_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_C_Chest_C5)
{
    mapTo = "Chest_C5";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack4_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack4_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack4_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_C_Chest_C70)
{
    mapTo = "Chest_C70";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack4_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack4_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack4_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_C_Chest_D)
{
    mapTo = "Chest_D";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack4_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack4_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack4_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_C_Chest_D40)
{
    mapTo = "Chest_D40";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack4_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack4_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack4_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_C_Chest_D5)
{
    mapTo = "Chest_D5";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack4_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack4_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack4_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_C_Chest_D70)
{
    mapTo = "Chest_D70";
    diffuseMap[0] = "art/Textures/TextureLib/Furniture_Pack4_DIFFUSE.dds";
    diffuseMap[1] = "art/Textures/TextureLib/Furniture_Pack4_NORMALMAP.dds";
    diffuseMap[2] = "art/Textures/TextureLib/Furniture_Pack4_SPECULAR.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_Outfit_miner_diff)
{
   mapTo = "miner_diff";
   diffuseMap[0] = "art/Textures/CharacterTextures/Outfits/miner/Miner_Male_Diffuse.dds";
   diffuseMap[1] = "art/Textures/CharacterTextures/Outfits/miner/Miner_Male_Normal.dds";
   diffuseMap[2] = "art/Textures/CharacterTextures/Outfits/miner/Miner_Male_Specular.dds";
   materialTag0 = "LiF";
   normal3DC = "1";
   doubleSided = "1";
   alphaTest = "1";
   alphaRef = "110";
   skinned = true;
};

singleton Material(LiFx_Outfit_miner_female_diff)
{
   mapTo = "miner_female_diff";
   diffuseMap[0] = "art/Textures/CharacterTextures/Outfits/miner/Miner_Female_Diffuse.dds";
   diffuseMap[1] = "art/Textures/CharacterTextures/Outfits/miner/Miner_Female_Normal.dds";
   diffuseMap[2] = "art/Textures/CharacterTextures/Outfits/miner/Miner_Female_Specular.dds";
   materialTag0 = "LiF";
   normal3DC = "1";
   doubleSided = "1";
   alphaTest = "1";
   alphaRef = "110";
   skinned = true;
};

singleton Material(LiFx_Outfit_cattleman_diff)
{
   mapTo = "cattleman_diff";
   diffuseMap[0] = "art/Textures/CharacterTextures/Outfits/cattleman/Cattleman_Male_Diffuse.dds";
   diffuseMap[1] = "art/Textures/CharacterTextures/Outfits/cattleman/Cattleman_Male_Normal.dds";
   diffuseMap[2] = "art/Textures/CharacterTextures/Outfits/cattleman/Cattleman_Male_Specular.dds";
   materialTag0 = "LiF";
   normal3DC = "1";
   doubleSided = "1";
   alphaTest = "1";
   alphaRef = "110";
   skinned = true;
};

singleton Material(LiFx_Outfit_cattleman_female_diff)
{
   mapTo = "cattleman_female_diff";
   diffuseMap[0] = "art/Textures/CharacterTextures/Outfits/cattleman/Cattleman_Female_diffuse.dds";
   diffuseMap[1] = "art/Textures/CharacterTextures/Outfits/cattleman/Cattleman_Female_Normal.dds";
   diffuseMap[2] = "art/Textures/CharacterTextures/Outfits/cattleman/Cattleman_Female_Specular.dds";
   materialTag0 = "LiF";
   normal3DC = "1";
   doubleSided = "1";
   alphaTest = "1";
   alphaRef = "110";
   skinned = true;
};

singleton Material(LiFx_King_Male_King_Outfit_A)
{
   mapTo = "Male_King_Outfit_A";
   diffuseMap[0] = "art/Textures/CharacterTextures/Outfits/king/Male_KingA_DIFFUSE.dds";
   diffuseMap[1] = "art/Textures/CharacterTextures/Outfits/king/Male_KingA_NORMALMAP.dds";
   diffuseMap[2] = "art/Textures/CharacterTextures/Outfits/king/Male_KingA_SPECULAR.dds";
   materialTag0 = "LiF";
   normal3DC = "1";
   doubleSided = "1";
   alphaTest = "1";
   alphaRef = "110";
   skinned = true;
};

singleton Material(LiFx_King_Male_King_Outfit_A_SkinA)
{
   mapTo = "Male_King_Outfit_A_SkinA";
   diffuseMap[0] = "art/Textures/CharacterTextures/Outfits/king/Male_KingA_SkinA_DIFFUSE.dds";
   diffuseMap[1] = "art/Textures/CharacterTextures/Outfits/king/Male_KingA_NORMALMAP.dds";
   diffuseMap[2] = "art/Textures/CharacterTextures/Outfits/king/Male_KingA_SkinA_SPECULAR.dds";
   materialTag0 = "LiF";
   normal3DC = "1";
   doubleSided = "1";
   alphaTest = "1";
   alphaRef = "110";
   skinned = true;
};

singleton Material(LiFx_King_Male_King_Outfit_A_SkinB)
{
   mapTo = "Male_King_Outfit_A_SkinB";
   diffuseMap[0] = "art/Textures/CharacterTextures/Outfits/king/Male_KingA_SkinB_DIFFUSE.dds";
   diffuseMap[1] = "art/Textures/CharacterTextures/Outfits/king/Male_KingA_NORMALMAP.dds";
   diffuseMap[2] = "art/Textures/CharacterTextures/Outfits/king/Male_KingA_SkinB_SPECULAR.dds";
   materialTag0 = "LiF";
   normal3DC = "1";
   doubleSided = "1";
   alphaTest = "1";
   alphaRef = "110";
   skinned = true;
};

singleton Material(LiFx_King_Male_King_Outfit_B)
{
   mapTo = "Male_King_Outfit_B";
   diffuseMap[0] = "art/Textures/CharacterTextures/Outfits/king/Male_KingB_DIFFUSE.dds";
   diffuseMap[1] = "art/Textures/CharacterTextures/Outfits/king/Male_KingB_NORMALMAP.dds";
   diffuseMap[2] = "art/Textures/CharacterTextures/Outfits/king/Male_KingB_SPECULAR.dds";
   materialTag0 = "LiF";
   normal3DC = "1";
   doubleSided = "1";
   alphaTest = "1";
   alphaRef = "110";
   skinned = true;
};

singleton Material(LiFx_King_Male_King_Outfit_B_SkinA)
{
   mapTo = "Male_King_Outfit_B_SkinA";
   diffuseMap[0] = "art/Textures/CharacterTextures/Outfits/king/Male_KingB_SkinA_DIFFUSE.dds";
   diffuseMap[1] = "art/Textures/CharacterTextures/Outfits/king/Male_KingB_NORMALMAP.dds";
   diffuseMap[2] = "art/Textures/CharacterTextures/Outfits/king/Male_KingB_SkinA_SPECULAR.dds";
   materialTag0 = "LiF";
   normal3DC = "1";
   doubleSided = "1";
   alphaTest = "1";
   alphaRef = "110";
   skinned = true;
};

singleton Material(LiFx_King_Male_King_Outfit_B_SkinB)
{
   mapTo = "Male_King_Outfit_B_SkinB";
   diffuseMap[0] = "art/Textures/CharacterTextures/Outfits/king/Male_KingB_SkinB_DIFFUSE.dds";
   diffuseMap[1] = "art/Textures/CharacterTextures/Outfits/king/Male_KingB_NORMALMAP.dds";
   diffuseMap[2] = "art/Textures/CharacterTextures/Outfits/king/Male_KingB_SkinB_SPECULAR.dds";
   materialTag0 = "LiF";
   normal3DC = "1";
   doubleSided = "1";
   alphaTest = "1";
   alphaRef = "110";
   skinned = true;
};

singleton Material(LiFx_King_Female_King_Outfit_A)
{
   mapTo = "Female_King_Outfit_A";
   diffuseMap[0] = "art/Textures/CharacterTextures/Outfits/king/Female_KingA_DIFFUSE.dds";
   diffuseMap[1] = "art/Textures/CharacterTextures/Outfits/king/Female_KingA_NORMALMAP.dds";
   diffuseMap[2] = "art/Textures/CharacterTextures/Outfits/king/Female_KingA_SPECULAR.dds";
   materialTag0 = "LiF";
   normal3DC = "1";
   doubleSided = "1";
   alphaTest = "1";
   alphaRef = "110";
   skinned = true;
};

singleton Material(LiFx_King_Female_King_Outfit_A_SkinA)
{
   mapTo = "Female_King_Outfit_A_SkinA";
   diffuseMap[0] = "art/Textures/CharacterTextures/Outfits/king/Female_KingA_SkinA_DIFFUSE.dds";
   diffuseMap[1] = "art/Textures/CharacterTextures/Outfits/king/Female_KingA_NORMALMAP.dds";
   diffuseMap[2] = "art/Textures/CharacterTextures/Outfits/king/Female_KingA_SkinA_SPECULAR.dds";
   materialTag0 = "LiF";
   normal3DC = "1";
   doubleSided = "1";
   alphaTest = "1";
   alphaRef = "110";
   skinned = true;
};

singleton Material(LiFx_King_Female_King_Outfit_A_SkinB)
{
   mapTo = "Female_King_Outfit_A_SkinB";
   diffuseMap[0] = "art/Textures/CharacterTextures/Outfits/king/Female_KingA_SkinB_DIFFUSE.dds";
   diffuseMap[1] = "art/Textures/CharacterTextures/Outfits/king/Female_KingA_NORMALMAP.dds";
   diffuseMap[2] = "art/Textures/CharacterTextures/Outfits/king/Female_KingA_SkinB_SPECULAR.dds";
   materialTag0 = "LiF";
   normal3DC = "1";
   doubleSided = "1";
   alphaTest = "1";
   alphaRef = "110";
   skinned = true;
};

singleton Material(LiFx_King_Female_King_Outfit_B)
{
   mapTo = "Female_King_Outfit_B";
   diffuseMap[0] = "art/Textures/CharacterTextures/Outfits/king/Female_KingB_DIFFUSE.dds";
   diffuseMap[1] = "art/Textures/CharacterTextures/Outfits/king/Female_KingB_NORMALMAP.dds";
   diffuseMap[2] = "art/Textures/CharacterTextures/Outfits/king/Female_KingB_SPECULAR.dds";
   materialTag0 = "LiF";
   normal3DC = "1";
   doubleSided = "1";
   alphaTest = "1";
   alphaRef = "110";
   skinned = true;
};

singleton Material(LiFx_King_Female_King_Outfit_B_SkinA)
{
   mapTo = "Female_King_Outfit_B_SkinA";
   diffuseMap[0] = "art/Textures/CharacterTextures/Outfits/king/Female_KingB_SkinA_DIFFUSE.dds";
   diffuseMap[1] = "art/Textures/CharacterTextures/Outfits/king/Female_KingB_NORMALMAP.dds";
   diffuseMap[2] = "art/Textures/CharacterTextures/Outfits/king/Female_KingB_SkinA_SPECULAR.dds";
   materialTag0 = "LiF";
   normal3DC = "1";
   doubleSided = "1";
   alphaTest = "1";
   alphaRef = "110";
   skinned = true;
};

singleton Material(LiFx_King_Female_King_Outfit_B_SkinB)
{
   mapTo = "Female_King_Outfit_B_SkinB";
   diffuseMap[0] = "art/Textures/CharacterTextures/Outfits/king/Female_KingB_SkinB_DIFFUSE.dds";
   diffuseMap[1] = "art/Textures/CharacterTextures/Outfits/king/Female_KingB_NORMALMAP.dds";
   diffuseMap[2] = "art/Textures/CharacterTextures/Outfits/king/Female_KingB_SkinB_SPECULAR.dds";
   materialTag0 = "LiF";
   normal3DC = "1";
   doubleSided = "1";
   alphaTest = "1";
   alphaRef = "110";
   skinned = true;
};

singleton Material(LiFx_King_HatCrown_King_Outfit)
{
   mapTo = "HatCrown_King_Outfit";
   diffuseMap[0] = "art/Textures/CharacterTextures/Outfits/king/Crowns_diffuse.dds";
   diffuseMap[1] = "art/Textures/CharacterTextures/Outfits/king/Crowns_normal.dds";
   diffuseMap[2] = "art/Textures/CharacterTextures/Outfits/king/Crowns_specular.dds";
   materialTag0 = "LiF";
   normal3DC = "1";
   doubleSided = "1";
   alphaTest = "1";
   alphaRef = "110";
   skinned = true;
};

singleton Material(LiFx_King_Male_Tatters_v1_Eur_DIFFUSE)
{
   mapTo = "Male_Tatters_v1_Eur_DIFFUSE";
   diffuseMap[0] = "art/Textures/CharacterTextures/Customization/male/Male_Tatters_Eur_v1_DIFFUSE.dds";
   diffuseMap[1] = "art/Textures/CharacterTextures/Customization/male/Male_Tatters_Eur_NORMALMAP.dds";
   diffuseMap[2] = "art/Textures/CharacterTextures/Customization/male/Male_Tatters_Eur_v1_SPECULAR.dds";
   materialTag0 = "LiF";
   normal3DC = "1";
   doubleSided = "1";
   alphaTest = "1";
   alphaRef = "110";
   skinned = true;
};

singleton Material(LiFx_King_Female_Tatters_v1_Eur_DIFFUSE)
{
   mapTo = "Female_Tatters_v1_Eur_DIFFUSE";
   diffuseMap[0] = "art/Textures/CharacterTextures/Customization/Female/Female_Tatters_Eur_v1_DIFFUSE.dds";
   diffuseMap[1] = "art/Textures/CharacterTextures/Customization/Female/Female_Tatters_Eur_NORMALMAP.dds";
   diffuseMap[2] = "art/Textures/CharacterTextures/Customization/Female/Female_Tatters_Eur_v1_SPECULAR.dds";
   materialTag0 = "LiF";
   normal3DC = "1";
   doubleSided = "1";
   alphaTest = "1";
   alphaRef = "110";
   skinned = true;
};

singleton Material(LiFx_King_Male_Tatters_v1_Mon_DIFFUSE)
{
   mapTo = "Male_Tatters_v1_Mon_DIFFUSE";
   diffuseMap[0] = "art/Textures/CharacterTextures/Customization/male/Male_Tatters_Mon_v1_DIFFUSE.dds";
   diffuseMap[1] = "art/Textures/CharacterTextures/Customization/male/Male_Tatters_Mon_NORMALMAP.dds";
   diffuseMap[2] = "art/Textures/CharacterTextures/Customization/male/Male_Tatters_Mon_v1_SPECULAR.dds";
   materialTag0 = "LiF";
   normal3DC = "1";
   doubleSided = "1";
   alphaTest = "1";
   alphaRef = "110";
   skinned = true;
};

singleton Material(LiFx_King_Female_Tatters_v1_Mon_DIFFUSE)
{
   mapTo = "Female_Tatters_v1_Mon_DIFFUSE";
   diffuseMap[0] = "art/Textures/CharacterTextures/Customization/Female/Female_Tatters_Mon_v1_DIFFUSE.dds";
   diffuseMap[1] = "art/Textures/CharacterTextures/Customization/Female/Female_Tatters_Mon_NORMALMAP.dds";
   diffuseMap[2] = "art/Textures/CharacterTextures/Customization/Female/Female_Tatters_Mon_v1_SPECULAR.dds";
   materialTag0 = "LiF";
   normal3DC = "1";
   doubleSided = "1";
   alphaTest = "1";
   alphaRef = "110";
   skinned = true;
};

singleton Material(LiFx_King_Male_Tatters_v1_Vik_DIFFUSE)
{
   mapTo = "Male_Tatters_v1_Vik_DIFFUSE";
   diffuseMap[0] = "art/Textures/CharacterTextures/Customization/male/Male_Tatters_Vik_v1_DIFFUSE.dds";
   diffuseMap[1] = "art/Textures/CharacterTextures/Customization/male/Male_Tatters_Vik_NORMALMAP.dds";
   diffuseMap[2] = "art/Textures/CharacterTextures/Customization/male/Male_Tatters_Vik_v1_SPECULAR.dds";
   materialTag0 = "LiF";
   normal3DC = "1";
   doubleSided = "1";
   alphaTest = "1";
   alphaRef = "110";
   skinned = true;
};

singleton Material(LiFx_King_Female_Tatters_v1_Vik_DIFFUSE)
{
   mapTo = "Female_Tatters_v1_Vik_DIFFUSE";
   diffuseMap[0] = "art/Textures/CharacterTextures/Customization/Female/Female_Tatters_Vik_v1_DIFFUSE.dds";
   diffuseMap[1] = "art/Textures/CharacterTextures/Customization/Female/Female_Tatters_Vik_NORMALMAP.dds";
   diffuseMap[2] = "art/Textures/CharacterTextures/Customization/Female/Female_Tatters_Vik_v1_SPECULAR.dds";
   materialTag0 = "LiF";
   normal3DC = "1";
   doubleSided = "1";
   alphaTest = "1";
   alphaRef = "110";
   skinned = true;
};

singleton Material(LiFx_Surcoat_Pink)
{
   mapTo = "Surcoat_armor_Pink_DIFFUSE";
   diffuseMap[0] = "art/Textures/CharacterTextures/Outfits/Surcoat/Surcoat_armorA_DIFFUSE.dds";
   diffuseMap[1] = "art/Textures/CharacterTextures/Outfits/Surcoat/Surcoat_armor_NORMALMAP.dds";
   diffuseMap[2] = "art/Textures/CharacterTextures/Outfits/Surcoat/Surcoat_armorA_SPECULAR.dds";
   alphaTest = "1";
   alphaRef = "100";
   useAnisotropic[0] = "1";
   isTabard = true;
   skinned = true;
   doubleSided = "1";
   heraldicCustomizationData = Surcoat_armor_Heraldry;
   normal3DC="1";
   heraldyOffset = "0 0 2 1";
};

singleton Material(LiFx_Surcoat_Red)
{
   mapTo = "Surcoat_armor_Red_DIFFUSE";
   diffuseMap[0] = "art/Textures/CharacterTextures/Outfits/Surcoat/Surcoat_armorB_DIFFUSE.dds";
   diffuseMap[1] = "art/Textures/CharacterTextures/Outfits/Surcoat/Surcoat_armor_NORMALMAP.dds";
   diffuseMap[2] = "art/Textures/CharacterTextures/Outfits/Surcoat/Surcoat_armorB_SPECULAR.dds";
   alphaTest = "1";
   alphaRef = "100";
   useAnisotropic[0] = "1";
   isTabard = true;
   skinned = true;
   doubleSided = "1";
   heraldicCustomizationData = Surcoat_armor_Heraldry;
   normal3DC="1";
   heraldyOffset = "0 0 2 1";
};

singleton Material(LiFx_Surcoat_Blue)
{
   mapTo = "Surcoat_armor_Blue_DIFFUSE";
   diffuseMap[0] = "art/Textures/CharacterTextures/Outfits/Surcoat/Surcoat_armorC_DIFFUSE.dds";
   diffuseMap[1] = "art/Textures/CharacterTextures/Outfits/Surcoat/Surcoat_armor_NORMALMAP.dds";
   diffuseMap[2] = "art/Textures/CharacterTextures/Outfits/Surcoat/Surcoat_armorC_SPECULAR.dds";
   alphaTest = "1";
   alphaRef = "100";
   useAnisotropic[0] = "1";
   isTabard = true;
   skinned = true;
   doubleSided = "1";
   heraldicCustomizationData = Surcoat_armor_Heraldry;
   normal3DC="1";
   heraldyOffset = "0 0 2 1";
};

singleton Material(LiFx_Surcoat_Orange)
{
   mapTo = "Surcoat_armor_Orange_DIFFUSE";
   diffuseMap[0] = "art/Textures/CharacterTextures/Outfits/Surcoat/Surcoat_armorD_DIFFUSE.dds";
   diffuseMap[1] = "art/Textures/CharacterTextures/Outfits/Surcoat/Surcoat_armor_NORMALMAP.dds";
   diffuseMap[2] = "art/Textures/CharacterTextures/Outfits/Surcoat/Surcoat_armorD_SPECULAR.dds";
   alphaTest = "1";
   alphaRef = "100";
   useAnisotropic[0] = "1";
   isTabard = true;
   skinned = true;
   doubleSided = "1";
   heraldicCustomizationData = Surcoat_armor_Heraldry;
   normal3DC="1";
   heraldyOffset = "0 0 2 1";
};

singleton Material(LiFx_FlagOfficer_H_a)
{
   mapTo = "FlagOfficer_H_a_DIFFUSE";
   diffuseMap[0] = "art/Textures/CharacterTextures/FlagOfficer/FlagOfficer_H_a_DIFFUSE.dds";
   diffuseMap[1] = "art/Textures/CharacterTextures/FlagOfficer/FlagOfficer_G_NORMALMAP.dds";
   diffuseMap[2] = "art/Textures/CharacterTextures/FlagOfficer/FlagOfficer_G_SPECULAR.dds";
   alphaTest = "1";
   alphaRef = "20";
   isHeraldic = true;
   skinned = true;
   doubleSided = "1";
   normal3DC="1";
   heraldicCustomizationData = FlagOfficer_G_Heraldry_OPACITY;
   heraldyOffset = "0.27 -0.22 2 1";
};

singleton Material(LiFx_Skin_Male_Craft_Miner_SkinA_DIFFUSE)
{
   mapTo = "Male_Craft_Miner_SkinA_DIFFUSE";
   diffuseMap[0] = "art/Textures/CharacterTextures/Outfits/miner/Male_Craft_Miner_SkinA_DIFFUSE.dds";
   diffuseMap[1] = "art/Textures/CharacterTextures/Outfits/miner/Miner_Male_Normal.dds";
   diffuseMap[2] = "art/Textures/CharacterTextures/Outfits/miner/Male_Craft_Miner_SkinA_SPECULAR.dds";
   materialTag0 = "LiF";
   normal3DC = "1";
   doubleSided = "1";
   alphaTest = "1";
   alphaRef = "110";
   skinned = true;
};

singleton Material(LiFx_Skin_Female_Craft_Miner_SkinA_DIFFUSE)
{
   mapTo = "Female_Craft_Miner_SkinA_DIFFUSE";
   diffuseMap[0] = "art/Textures/CharacterTextures/Outfits/miner/Female_Craft_Miner_SkinA_DIFFUSE.dds";
   diffuseMap[1] = "art/Textures/CharacterTextures/Outfits/miner/Miner_Female_Normal.dds";
   diffuseMap[2] = "art/Textures/CharacterTextures/Outfits/miner/Female_Craft_Miner_SkinA_SPECULAR.dds";
   materialTag0 = "LiF";
   normal3DC = "1";
   doubleSided = "1";
   alphaTest = "1";
   alphaRef = "110";
   skinned = true;
};

singleton Material(LiFx_Skin_Male_Craft_Cattleman_SkinA_DIFFUSE)
{
   mapTo = "Male_Craft_Cattleman_SkinA_DIFFUSE";
   diffuseMap[0] = "art/Textures/CharacterTextures/Outfits/cattleman/Male_Craft_Cattleman_SkinA_DIFFUSE.dds";
   diffuseMap[1] = "art/Textures/CharacterTextures/Outfits/cattleman/Cattleman_Male_Normal.dds";
   diffuseMap[2] = "art/Textures/CharacterTextures/Outfits/cattleman/Male_Craft_Cattleman_SkinA_SPECULAR.dds";
   materialTag0 = "LiF";
   normal3DC = "1";
   doubleSided = "1";
   alphaTest = "1";
   alphaRef = "110";
   skinned = true;
};

singleton Material(LiFx_Skin_Female_Cattleman_SkinA_DIFFUSE)
{
   mapTo = "Female_Cattleman_SkinA_DIFFUSE";
   diffuseMap[0] = "art/Textures/CharacterTextures/Outfits/cattleman/Female_Craft_Cattleman_SkinA_DIFFUSE.dds";
   diffuseMap[1] = "art/Textures/CharacterTextures/Outfits/cattleman/Female_Craft_Cattleman_SkinA_NORMAL.dds";
   diffuseMap[2] = "art/Textures/CharacterTextures/Outfits/cattleman/Female_Craft_Cattleman_SkinA_SPECULAR.dds";
   materialTag0 = "LiF";
   normal3DC = "1";
   doubleSided = "1";
   alphaTest = "1";
   alphaRef = "110";
   skinned = true;
};

singleton Material(LiFx_Recolor_Male_Peasant_SkinB)
{
   mapTo = "Male_Peasant_SkinB_DIFFUSE";
   diffuseMap[0] = "art/Textures/CharacterTextures/Outfits/Simple/Male_Peasant_Blue_DIFFUSE.dds";
   diffuseMap[1] = "art/Textures/CharacterTextures/Outfits/Simple/Male_Peasant_Blue_NORMALMAP.dds";
   diffuseMap[2] = "art/Textures/CharacterTextures/Outfits/Simple/Male_Peasant_Blue_SPECULAR.dds";
   normal3DC="1";
   skinned = true;
   doubleSided = "1";
};

singleton Material(LiFx_Sg_Fence)
{
    mapTo = "Fence";
    diffuseMap[0] = "art/Textures/Atlas/Fence_diff.dds";
    diffuseMap[1] = "art/Textures/Atlas/Fence_nm.dds";
    diffuseMap[2] = "art/Textures/Atlas/Fence_spec.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_Sg_fence02clean)
{
    mapTo = "fence02clean";
    diffuseMap[0] = "art/Textures/Atlas/fence_02_clean_diff.dds";
    diffuseMap[1] = "art/Textures/Atlas/fence_02_clean_nm.dds";
    diffuseMap[2] = "art/Textures/Atlas/fence_02_clean_spec.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_Sg_fence02clean200)
{
    mapTo = "fence02clean200";
    diffuseMap[0] = "art/Textures/Atlas/fence_02_clean_diff.dds";
    diffuseMap[1] = "art/Textures/Atlas/fence_02_clean_nm.dds";
    diffuseMap[2] = "art/Textures/Atlas/fence_02_clean_spec.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_Sg_fence02clean500)
{
    mapTo = "fence02clean500";
    diffuseMap[0] = "art/Textures/Atlas/fence_02_clean_diff.dds";
    diffuseMap[1] = "art/Textures/Atlas/fence_02_clean_nm.dds";
    diffuseMap[2] = "art/Textures/Atlas/fence_02_clean_spec.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_Sg_fence02clean75)
{
    mapTo = "fence02clean75";
    diffuseMap[0] = "art/Textures/Atlas/fence_02_clean_diff.dds";
    diffuseMap[1] = "art/Textures/Atlas/fence_02_clean_nm.dds";
    diffuseMap[2] = "art/Textures/Atlas/fence_02_clean_spec.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_Sg_Fence200)
{
    mapTo = "Fence200";
    diffuseMap[0] = "art/Textures/Atlas/Fence_diff.dds";
    diffuseMap[1] = "art/Textures/Atlas/Fence_nm.dds";
    diffuseMap[2] = "art/Textures/Atlas/Fence_spec.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_Sg_Fence500)
{
    mapTo = "Fence500";
    diffuseMap[0] = "art/Textures/Atlas/Fence_diff.dds";
    diffuseMap[1] = "art/Textures/Atlas/Fence_nm.dds";
    diffuseMap[2] = "art/Textures/Atlas/Fence_spec.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};

singleton Material(LiFx_Sg_Fence75)
{
    mapTo = "Fence75";
    diffuseMap[0] = "art/Textures/Atlas/Fence_diff.dds";
    diffuseMap[1] = "art/Textures/Atlas/Fence_nm.dds";
    diffuseMap[2] = "art/Textures/Atlas/Fence_spec.dds";
    materialTag0 = "LiF";
    normal3DC = "1";
};
