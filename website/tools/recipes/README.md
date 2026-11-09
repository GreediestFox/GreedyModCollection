# Recipe book: how to update it from the Google sheet (tools/recipes)

1. Export the sheet's FIRST tab ("Rezepte Stand 2026") as CSV and save it as  tools\recipes\work\tab0.csv
   (File -> Download -> Comma-separated values, or the public link .../export?format=csv&gid=0 when the sheet is shared).
2. Names come from your own game + database (read-only):
   - German: client  F:\SteamLibrary\steamapps\common\Life is Feudal Your Own\data\loc\de\data\  (recipe_Name.xml, objects_types_Name.xml, skill_types.xml)
   - English: database lif_1 -> work\db_recipe_names.tsv, db_object_names.tsv, db_skill_names.tsv (columns ID<TAB>Name from tables recipe, objects_types, skill_type)
3. Run, from the project folder:
       node tools\recipes\parse.mjs      (sheet blocks -> work\parsed.json)
       node tools\recipes\build.mjs      (-> public\data\recipes.json and public\data\i18n-recipes-en.json)
4. Deploy (Website-veroeffentlichen.bat).

Ingredient lines are taken from the sheet author's comments ("Holzbretter x4" -> "4 × Holzbretter", "Säge Haltbarkeit 10" -> "Säge (Haltbarkeit 10)").
A few typos in the sheet are corrected automatically (see line() in build.mjs).

## Update 2026-10-17: the recipe book now comes from the live game database
The sheet is no longer the source. After the recipe revision the recipe book is built from the live database + client files:
1. Export the DB tables to work\ (db_recipe_live.tsv, db_recipe_requirement_live.tsv, db_object_names.tsv, db_object_flags.tsv, db_recipe_names.tsv, db_skill_names.tsv; see the mysql commands in the session notes).
2. python tools\recipes\fromdb_parsed.py  (needs the plan/model files from E:\ClaudeScratch\project_archive\Session_2026-10-14\RecipeRevision\gen; it writes work\parsed.json: same recipe scope as before, workshop duplicates and placeholder recipes left out)
3. node tools\recipes\build.mjs, then deploy.

## Static recipe pages for search engines (2026-10-17)
After build.mjs run:  node tools\recipes\static-pages.mjs  - it (re)creates public\rezepte\ (overview, 28 skill pages, one page per recipe), adds them to sitemap.xml and the English texts to data\i18n-en.json. Then deploy. Re-run it whenever recipes.json changes.
