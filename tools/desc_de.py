"""German descriptions for every item whose DE description was missing or still English (2026-10-04).
Keys are the exact English texts from the client objects_types.xml; applied to loc\de\data\objects_types_Description.xml."""
import json, re, shutil, os
L = r'F:\SteamLibrary\steamapps\common\Life is Feudal Your Own\data\loc\de\data\objects_types_Description.xml'
BK = r'E:\ClaudeScratch\backups\desc_de_20261004'
os.makedirs(BK, exist_ok=True)

T = {
 "Can be gathered from Forest Soil. Used in several cooking recipes.": "Kann auf Waldboden gesammelt werden. Wird in mehreren Kochrezepten verwendet.",
 "An alcoholic beverage made from fermented grapes.": "Ein alkoholisches Getränk aus vergorenen Trauben.",
 "Can be gathered from Apple Trees. Used in various cooking recipes and as animal feed.": "Kann von Apfelbäumen gesammelt werden. Wird in verschiedenen Kochrezepten und als Tierfutter verwendet.",
 "A crop that can be grown by Farmers. Used for various cooking recipes.": "Eine Feldfrucht, die von Bauern angebaut werden kann. Wird für verschiedene Kochrezepte verwendet.",
 "Collected by cleaning out coops, barns, or stables. Can be used as a fertilizer.": "Fällt beim Ausmisten von Hühnerställen, Scheunen oder Ställen an. Kann als Dünger verwendet werden.",
 "A grain. Ingredient for soups.": "Ein Getreide. Zutat für Suppen.",
 "A small fish.": "Ein kleiner Fisch.",
 "A freshwater fish.": "Ein Süßwasserfisch.",
 "A raw material used for cooking and brewing.": "Ein Rohstoff zum Kochen und Brauen.",
 "Fine flour used for baking.": "Feines Mehl zum Backen.",
 "Malted oats used for brewing.": "Gemälzter Hafer zum Brauen.",
 "Malted rye used for brewing.": "Gemälzter Roggen zum Brauen.",
 "Malted wheat used for brewing.": "Gemälzter Weizen zum Brauen.",
 "Churned butter used for baking.": "Geschlagene Butter zum Backen.",
 "Food (4 ingredients)": "Leckeres Essen (4 Zutaten)",
 "Made from Metal Band and Rocksalt": "Hergestellt aus Metallband und Steinsalz.",
 "Made from Metal Band and Brimstone": "Hergestellt aus Metallband und Schwefel.",
 "Made from Metal Sheet and Rocksalt": "Hergestellt aus Metallblech und Steinsalz.",
 "Made from Metal Sheet and Brimstone": "Hergestellt aus Metallblech und Schwefel.",
 "Made from Chainmail and Rocksalt. Used for manufacturing Chainmail Armor at a Workbench.": "Hergestellt aus Kettengeflecht und Steinsalz. Wird an einer Werkbank zur Herstellung von Kettenrüstungen verwendet.",
 "Made from Chainmail and Brimstone. Used for manufacturing Chainmail Armor at a Workbench.": "Hergestellt aus Kettengeflecht und Schwefel. Wird an einer Werkbank zur Herstellung von Kettenrüstungen verwendet.",
 "Made from Small metal plate and Rocksalt. Used for manufacturing Plate Armor, Chainmail Armor, Scale Armor at a Workbench.": "Hergestellt aus kleiner Metallplatte und Steinsalz. Wird an einer Werkbank zur Herstellung von Platten-, Ketten- und Schuppenrüstungen verwendet.",
 "Made from Small metal plate and Brimstone. Used for manufacturing Plate Armor, Chainmail Armor, Scale Armor at a Workbench.": "Hergestellt aus kleiner Metallplatte und Schwefel. Wird an einer Werkbank zur Herstellung von Platten-, Ketten- und Schuppenrüstungen verwendet.",
 "Made from Metal plate and Rocksalt. Used for manufacturing Plate Armor at a Workbench.": "Hergestellt aus Metallplatte und Steinsalz. Wird an einer Werkbank zur Herstellung von Plattenrüstungen verwendet.",
 "Made from Metal plate and Brimstone. Used for manufacturing Plate Armor at a Workbench.": "Hergestellt aus Metallplatte und Schwefel. Wird an einer Werkbank zur Herstellung von Plattenrüstungen verwendet.",
 "Made from Scale strip and Rocksalt. Used for manufacturing Scale Armor at a Workbench.": "Hergestellt aus Schuppenstreifen und Steinsalz. Wird an einer Werkbank zur Herstellung von Schuppenrüstungen verwendet.",
 "Made from Scale strip and Brimstone. Used for manufacturing Scale Armor at a Workbench.": "Hergestellt aus Schuppenstreifen und Schwefel. Wird an einer Werkbank zur Herstellung von Schuppenrüstungen verwendet.",
 "Mallet's Big Brother": "Der große Bruder des Holzhammers.",
 "Looks like a regular staff but weighs a little less": "Sieht aus wie ein gewöhnlicher Stab, wiegt aber etwas weniger.",
 "Used to craft high-quality weapons": "Wird zur Herstellung hochwertiger Waffen verwendet.",
 "Used to craft high-quality armor": "Wird zur Herstellung hochwertiger Rüstungen verwendet.",
 "A material used in the manufacture of leather clothes, outfits, weapons, and armor.": "Ein Material zur Herstellung von Lederkleidung, Trachten, Waffen und Rüstungen.",
 "A material used in the manufacture of leather clothes, outfits, weapon, and armor.": "Ein Material zur Herstellung von Lederkleidung, Trachten, Waffen und Rüstungen.",
 "A cloth that's used to craft decorated clothes and high level armor. Crafted from Hanks of Wool with a Loom.": "Stoff, der für verzierte Kleidung und hochwertige Rüstungen verwendet wird. Wird an einem Webstuhl aus Wollsträngen gewebt.",
 "A cloth that's used to craft decorated clothes and high level armor. Crafted from Hanks of Soft Wool with a Loom.": "Stoff, der für verzierte Kleidung und hochwertige Rüstungen verwendet wird. Wird an einem Webstuhl aus Strängen weicher Wolle gewebt.",
 "A building material required in complex structures": "Ein Baumaterial, das für aufwendige Bauwerke benötigt wird.",
 "Made from Steel Bar and Rocksalt. Used for forging weapons of high quality.": "Hergestellt aus Stahlstange und Steinsalz. Wird zum Schmieden hochwertiger Waffen verwendet.",
 "Made from Steel Bar and Brimstone. Used for forging weapons of high quality.": "Hergestellt aus Stahlstange und Schwefel. Wird zum Schmieden hochwertiger Waffen verwendet.",
 "Made from Iron Bar and Rocksalt. Used for forging weapons of medium quality.": "Hergestellt aus Eisenstange und Steinsalz. Wird zum Schmieden von Waffen mittlerer Qualität verwendet.",
 "Made from Iron Bar and Brimstone. Used for forging weapons of medium quality.": "Hergestellt aus Eisenstange und Schwefel. Wird zum Schmieden von Waffen mittlerer Qualität verwendet.",
 "Made from Vostaskus Bar and Rocksalt. Used for forging weapons of the best quality.": "Hergestellt aus Vostaskusstange und Steinsalz. Wird zum Schmieden von Waffen bester Qualität verwendet.",
 "Made from Vostaskus Bar and Brimstone. Used for forging weapons of the best quality.": "Hergestellt aus Vostaskusstange und Schwefel. Wird zum Schmieden von Waffen bester Qualität verwendet.",
 "Made from Steel Ingot and Rocksalt. Used for forging weapons of high quality.": "Hergestellt aus Stahlbarren und Steinsalz. Wird zum Schmieden hochwertiger Waffen verwendet.",
 "Made from Steel Ingot and Brimstone. Used for forging weapons of high quality.": "Hergestellt aus Stahlbarren und Schwefel. Wird zum Schmieden hochwertiger Waffen verwendet.",
 "Made from Iron Ingot and Rocksalt. Used for forging weapons of medium quality.": "Hergestellt aus Eisenbarren und Steinsalz. Wird zum Schmieden von Waffen mittlerer Qualität verwendet.",
 "Made from Iron Ingot and Brimstone. Used for forging weapons of medium quality.": "Hergestellt aus Eisenbarren und Schwefel. Wird zum Schmieden von Waffen mittlerer Qualität verwendet.",
 "Made form Vostaskus Ingot and Rocksalt. Used for forging weapons of the best quality.": "Hergestellt aus Vostaskusbarren und Steinsalz. Wird zum Schmieden von Waffen bester Qualität verwendet.",
 "Made form Vostaskus Ingot and Brimstone. Used for forging weapons of the best quality.": "Hergestellt aus Vostaskusbarren und Schwefel. Wird zum Schmieden von Waffen bester Qualität verwendet.",
 "Required to craft Wool Cloth. Crafted from Wool Packs with a Spinning Wheel.": "Wird für Wollstoff benötigt. Wird am Spinnrad aus Wollballen gesponnen.",
 "Required to craft Soft Wool Cloth. Crafted from Soft Wool with a Spinning Wheel.": "Wird für weichen Wollstoff benötigt. Wird am Spinnrad aus weicher Wolle gesponnen.",
 "Used to make Rough Thin Leather with a Tanning Tub.": "Wird in einer Gerbwanne zu grobem dünnem Leder verarbeitet.",
 "Used to make Soft Thin Leather with a Tanning Tub.": "Wird in einer Gerbwanne zu weichem dünnem Leder verarbeitet.",
 "Used to make Rough Thick Leather with a Tanning Tub.": "Wird in einer Gerbwanne zu grobem dickem Leder verarbeitet.",
 "Used to make Soft Thick Leather with a Tanning Tub.": "Wird in einer Gerbwanne zu weichem dickem Leder verarbeitet.",
 "Obtained from mature sheep in a barn. Can be refined into Hanks of Wool with a Spinning Wheel.": "Stammt von ausgewachsenen Schafen in der Scheune. Kann am Spinnrad zu Wollsträngen verarbeitet werden.",
 "Obtained from mature sheep in a barn. Can be refined into Hanks of Soft Wool with a Spinning Wheel.": "Stammt von ausgewachsenen Schafen in der Scheune. Kann am Spinnrad zu Strängen weicher Wolle verarbeitet werden.",
 "Used to create Siege Ladders. Crafted in a Siege Engineer&apos;s Workshop.": "Wird zum Bau von Belagerungsleitern verwendet. Wird in der Werkstatt des Belagerungsingenieurs hergestellt.",
 "This heraldic banner is worn only by the best warriors who do not cover their shiny armor with tabards.": "Dieses Wappenbanner tragen nur die besten Krieger, die ihre glänzende Rüstung nicht unter einem Wappenrock verbergen.",
 "This scarlet banner with a brocade border is the essential symbol of the light cavalry that was in use in the time of the Vulpiс Empire.": "Dieses scharlachrote Banner mit Brokatsaum ist das Wahrzeichen der leichten Reiterei aus der Zeit des Vulpischen Reiches.",
 "Every truly battle-hardened warrior knows this symbol: the head of a unicorn on red canvas has long been the mark of heavy cavalry.": "Jeder kampferprobte Krieger kennt dieses Zeichen: Der Kopf eines Einhorns auf rotem Tuch ist seit jeher das Abzeichen der schweren Reiterei.",
 "Pikemen love to show off their spears on their banners, however they are drawn. Be they artless silhouettes like arrowheads or elegant sharp-pointed complex shapes, the leaf-green background is constant.": "Pikeniere zeigen ihre Speere gern auf ihren Bannern, wie auch immer sie gezeichnet sind. Ob schlichte Umrisse wie Pfeilspitzen oder elegante, spitz zulaufende Formen – der blattgrüne Hintergrund bleibt stets gleich.",
 "A halberd is a serious weapon in principle, and its banner was designed to match: a sturdy bolt of emerald silk emblazoned with the image of the halberd embroidered in silver-white thread.": "Eine Hellebarde ist eine ernsthafte Waffe, und ihr Banner wurde passend gestaltet: kräftige smaragdgrüne Seide mit einer in silberweißem Faden gestickten Hellebarde.",
 "These light blue infantry banners are always found on the front lines. Under this standard march shieldbearers, protecting their brothers-in-arms the pikemen and archers from enemy arrows.": "Diese hellblauen Banner des Fußvolks findet man stets in der vordersten Reihe. Unter diesem Feldzeichen marschieren Schildträger, die ihre Waffenbrüder, die Pikeniere und Bogenschützen, vor feindlichen Pfeilen schützen.",
 "This piece of thick blue velvet is decorated with a simple, recognizable symbol: a full-height infantry shield.": "Dieses Stück dicken blauen Samts ziert ein schlichtes, gut erkennbares Zeichen: ein mannshoher Fußvolkschild.",
 "Paint from the northern shores gives this banner a pale but fast gold hue. Once, these flags with a white axe terrified the inhabitants of the coastal villages, suddenly appearing through a thick fog accompanied by the cries of warriors and the noise of arriving ships.": "Farbe von den nördlichen Küsten verleiht diesem Banner einen blassen, aber beständigen Goldton. Einst versetzten diese Flaggen mit der weißen Axt die Bewohner der Küstendörfer in Angst, wenn sie plötzlich aus dichtem Nebel auftauchten, begleitet vom Geschrei der Krieger und dem Lärm anlandender Schiffe.",
 "The shape of a two-handed sword is stitched into this pale yellow cloth. Such banners fly high over the fray of battle, sowing fear into the hearts of those who stand against them.": "Die Form eines Zweihänders ist in dieses blassgelbe Tuch gestickt. Solche Banner wehen hoch über dem Schlachtgetümmel und säen Furcht in die Herzen derer, die sich ihnen entgegenstellen.",
 "The bow in this embroidered complex design is barely visible on the violet banner. Some say that this symbol, like the bow itself, was invented by Aori Goldenhanded.": "Der Bogen in diesem kunstvoll gestickten Muster ist auf dem violetten Banner kaum zu erkennen. Manche sagen, dieses Zeichen sei, wie der Bogen selbst, von Aori Goldhand erfunden worden.",
 "This massive purple silk banner is marked with a white symbol that can be seen from afar. This mark looks like all the Slavard runes thrown together, represents the crossbow, as well as all warriors that use them.": "Dieses gewaltige Banner aus purpurner Seide trägt ein weißes, weithin sichtbares Zeichen. Es sieht aus wie alle slavardischen Runen zusammengewürfelt und steht für die Armbrust sowie für alle Krieger, die sie führen.",
 "A red clover on a white canvas is the traditional banner of all healers, who are indispensable on any battlefield.": "Ein roter Klee auf weißem Tuch ist das traditionelle Banner aller Heiler, die auf keinem Schlachtfeld fehlen dürfen.",
 "The sunlight playing on the simple pattern of gold thread gives life to the sparks, and the details shimmer when the wind blows. This proud, shining banner is imbued with a special strength: hundred, thousands of people are willing to follow it into the furious flames of battle...": "Das Sonnenlicht, das auf dem schlichten Muster aus Goldfaden spielt, lässt Funken aufleben, und die Einzelheiten schimmern im Wind. Dieses stolze, leuchtende Banner besitzt eine besondere Kraft: Hunderte, ja Tausende sind bereit, ihm in die tobenden Flammen der Schlacht zu folgen ...",
 "The short clipped ends of deer antlers that adorn this helm look so exquisite that they bring tears to the eyes of other northerners.": "Die kurz gestutzten Enden des Hirschgeweihs, die diesen Helm zieren, wirken so erlesen, dass sie anderen Nordmännern Tränen in die Augen treiben.",
 "The Gottlungs see the deer as a proud and noble animal. Its antlers symbolize the branches of the World Tree, on which the trembling worlds of the Sleeper rest.": "Die Gottlungen sehen im Hirsch ein stolzes und edles Tier. Sein Geweih steht für die Äste des Weltenbaums, auf denen die zitternden Welten des Schläfers ruhen.",
 "The commander of the Gottlung troops, wearing a gleaming gold helm decorated with the branching antlers of a young moose, is a truly majestic sight. And yet, for some reason, it causes some northerners to burst out laughing.": "Der Befehlshaber der gottlungischen Truppen mit seinem glänzenden Goldhelm, verziert mit dem verzweigten Geweih eines jungen Elchs, ist ein wahrhaft majestätischer Anblick. Und doch bringt er manche Nordmänner aus irgendeinem Grund zum Lachen.",
 "The thin, delicate antlers of a young steppe deer look like two Heavenly Sabers.": "Das dünne, zarte Geweih eines jungen Steppenhirschs sieht aus wie zwei himmlische Säbel.",
 "The more beautiful the wife of a Khoorsian warrior, the more ferocious his temper. The more terrifying and dangerous a warrior is in battle, the longer the horns that decorate his heavy helm.": "Je schöner die Frau eines khoorsischen Kriegers, desto wilder sein Gemüt. Je furchterregender und gefährlicher ein Krieger in der Schlacht ist, desto länger die Hörner, die seinen schweren Helm zieren.",
 "A sign of great honor: four long, curved horns decorating the dome of a khan&apos;s helm. The branches of the Tree, the Heavenly Blades, and the rays of Amate the Sun are all eternal symbols known to every Khoor.": "Ein Zeichen großer Ehre: vier lange, geschwungene Hörner auf der Kuppel eines Khanshelms. Die Äste des Baumes, die himmlischen Klingen und die Strahlen der Sonne Amate sind ewige Zeichen, die jeder Khoor kennt.",
 "A dispute arose at Konung Halvdan&apos;s feast as to whether a man could topple a bull with a single punch. Jarl Hakon the Anvil knocked one down, cut off its horns, and gifted them to the konung. Thus began the trend of decorating helms with animal horns.": "Beim Fest von Konung Halvdan entbrannte ein Streit, ob ein Mann einen Stier mit einem einzigen Faustschlag fällen könne. Jarl Hakon der Amboss streckte einen nieder, schnitt ihm die Hörner ab und schenkte sie dem Konung. So begann die Sitte, Helme mit Tierhörnern zu schmücken.",
 "A leather helm humbly adorned with the antlers of a young deer and worn by regular Gottlung troops.": "Ein Lederhelm, schlicht mit dem Geweih eines jungen Hirschs geschmückt, getragen von gewöhnlichen gottlungischen Truppen.",
 "A Gottlung helm to which branching deer antlers are attached using an iron mount. It is said the Gottlungs borrowed this tradition from either the northerners or the Khoors. Or maybe it was from the deer themselves.": "Ein gottlungischer Helm, an dem ein verzweigtes Hirschgeweih mit einer eisernen Halterung befestigt ist. Es heißt, die Gottlungen hätten diesen Brauch von den Nordmännern oder den Khoors übernommen. Oder vielleicht von den Hirschen selbst.",
 "Affixed to the band of this Slavard helm are cow horns, pointing up like a peasant&apos;s pitchfork. It is unknown how many foes its owner gored or if he had given up his blade, but these helms are now common on the battlefield, as warriors are fond of them.": "Am Reif dieses slavardischen Helms sind Kuhhörner befestigt, die wie eine Bauernforke nach oben ragen. Ob sein Besitzer viele Feinde aufspießte oder sein Schwert aufgegeben hatte, weiß niemand, doch solche Helme sind heute auf dem Schlachtfeld verbreitet, denn die Krieger mögen sie.",
 "At times, a lord will don the clothes of a laborer, put a tool on their belt, and walk around as if they were a stonecutter-except their apron is dyed with woad and embroidered with gold. The commoners, gray from head to toe, gaze at its beauty with envy.": "Mitunter legt ein Herr die Kleidung eines Arbeiters an, hängt sich ein Werkzeug an den Gürtel und geht umher, als wäre er ein Steinmetz – nur dass seine Schürze mit Waid gefärbt und mit Gold bestickt ist. Das gemeine Volk, grau von Kopf bis Fuß, betrachtet ihre Schönheit voller Neid.",
 "The dress of a distinguished herdsman. It is ill-suited for actual work, and chiefly worn as a display of splendor: it features an emerald tunic with red trim and an apron with shining silver animals, fit to pay respects to a lord.": "Die Tracht eines angesehenen Hirten. Für echte Arbeit taugt sie kaum und wird vor allem zur Schau getragen: ein smaragdgrüner Kittel mit rotem Saum und eine Schürze mit glänzenden silbernen Tieren, würdig, einem Herrn die Aufwartung zu machen.",
 "Object from Jorvik MOD pack": "Objekt aus dem Jorvik-Mod.",
 "A crop that can be grown by Farmers. Used for brewing.": "Eine Feldfrucht, die von Bauern angebaut werden kann. Wird zum Brauen verwendet.",
 "A board of amberwood.": "Ein Brett aus Bernsteinholz.",
 "A building block of amberwood.": "Ein Baustein aus Bernsteinholz.",
 "A board of whitewood.": "Ein Brett aus Weißholz.",
 "A building block of whitewood.": "Ein Baustein aus Weißholz.",
 "A large potato.": "Eine große Kartoffel.",
 "Plump wheat grains.": "Pralle Weizenkörner.",
 "A crop that can be grown by Farmers.": "Eine Feldfrucht, die von Bauern angebaut werden kann.",
 "A sweet carrot.": "Eine süße Karotte.",
 "A large onion.": "Eine große Zwiebel.",
 "An onion.": "Eine Zwiebel.",
 "Seeds for sowing.": "Saatgut zum Aussäen.",
 "A cutting of a wine grapevine.": "Ein Steckling einer Weinrebe.",
 "A cutting of a wild grapevine.": "Ein Steckling einer wilden Weinrebe.",
 "Fatty pork meat.": "Fettes Schweinefleisch.",
 "Marbled beef.": "Marmoriertes Rindfleisch.",
 "Lamb meat.": "Lammfleisch.",
 "Rough thick leather.": "Grobes dickes Leder.",
 "Soft thick leather.": "Weiches dickes Leder.",
 "A trap baited with rock salt.": "Eine mit Steinsalz beköderte Falle.",
 "Dark honey.": "Dunkler Honig.",
 "White honey.": "Weißer Honig.",
 "Yellow honey.": "Gelber Honig.",
 "An uncut gem.": "Ein ungeschliffener Edelstein.",
 "A roughly cut gem.": "Ein grob geschliffener Edelstein.",
 "A cracked gem.": "Ein gesprungener Edelstein.",
 "A gem.": "Ein Edelstein.",
 "A metal band.": "Ein Metallband.",
 "An amulet.": "Ein Amulett.",
 "A ring.": "Ein Ring.",
 "A set of precision tools.": "Ein Satz Präzisionswerkzeuge.",
 "A set of professional tools.": "Ein Satz Profiwerkzeuge.",
 "A set of tools.": "Ein Satz Werkzeuge.",
 "A set of seasonings.": "Ein Gewürzset.",
 "A set of sewing tools.": "Ein Nähset.",
 "A kit for building an armor fitting stand.": "Ein Bausatz für einen Rüstungsanpassungsständer.",
 "A kit.": "Ein Bausatz.",
 "A trophy.": "Eine Trophäe.",
 "A grisly ingredient.": "Eine grausige Zutat.",
 "A trinket.": "Ein Schmuckstück.",
 "A symbol.": "Ein Symbol.",
 "A mask.": "Eine Maske.",
 "A charm.": "Ein Talisman.",
 "Sacred coins.": "Heilige Münzen.",
 "A badge.": "Ein Abzeichen.",
 "A totem.": "Ein Totem.",
 "A pendulum.": "Ein Pendel.",
}

todo = json.load(open(r'E:\ClaudeScratch\daniel\desc_todo.json', encoding='utf-8'))
missing = [d for d, ids in todo if d not in T]
assert not missing, missing
shutil.copy2(L, os.path.join(BK, 'objects_types_Description.xml'))
raw = open(L, 'rb').read(); bom = raw.startswith(b'\xef\xbb\xbf'); t = raw.decode('utf-8-sig'); nl = '\r\n' if '\r\n' in t else '\n'


def esc(s): return s.replace('&apos;', "'").replace('&', '&amp;').replace('<', '&lt;').replace('>', '&gt;')


n = 0
for d, ids in todo:
    v = esc(T[d])
    for i in ids:
        if re.search(r'<string id="%d">' % i, t):
            t = re.sub(r'<string id="%d">[^<]*</string>' % i, lambda m: '<string id="%d">%s</string>' % (i, v), t)
        else:
            last = t.rfind('</string>') + len('</string>')
            t = t[:last] + nl + '<string id="%d">%s</string>' % (i, v) + t[last:]
        n += 1
open(L, 'wb').write((b'\xef\xbb\xbf' if bom else b'') + t.encode('utf-8'))
print('descriptions set:', n)
