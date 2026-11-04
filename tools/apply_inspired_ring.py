"""Gold Sapphire Ring (item type 496): Inspired Work (cm effect 88, DB effect row 58) instead of Intelligence. Byte-exact replacements, nothing else touched."""
import sys

S = r'G:\Progam (Launcher)\steamapps\common\Life is Feudal Your Own Dedicated Server'
C = r'F:\SteamLibrary\steamapps\common\Life is Feudal Your Own'


def edit(path, old, new, label):
    d = open(path, 'rb').read()
    o = old.encode('utf-8'); n = new.encode('utf-8')
    if n in d and o not in d:
        print(label, ': already applied'); return
    assert d.count(o) == 1, (label, d.count(o))
    open(path, 'wb').write(d.replace(o, n))
    print(label, ': ok')


def nl(path):
    return '\r\n' if b'\r\n' in open(path, 'rb').read() else '\n'


p = S + r'\data\item_effects.xml'
q = nl(p)
edit(p, '<item id="496"> <!-- Gold Sapphire Ring -->' + q + '        <effect id="10">1 10</effect>',
     '<item id="496"> <!-- Gold Sapphire Ring: Inspired Work (was Int) -->' + q + '        <effect id="58">1 1</effect>', 'item_effects 496')

p = S + r'\art\dump.sql'
q = nl(p)
anchor = "INSERT IGNORE INTO `effects` (`ID`,`Effect_name`,`ResultPreparationID`,`ResultPotionID`,`PlayerEffectID`) VALUES (57,'Invulnerable',NULL,NULL,45);"
add = (anchor + q + '-- Gold Sapphire Ring: Inspired Work (cm effect 88, -20% ability time) (2026-11-04)' + q +
       "INSERT IGNORE INTO `effects` (`ID`,`Effect_name`,`ResultPreparationID`,`ResultPotionID`,`PlayerEffectID`) VALUES (58,'Inspired Work',NULL,NULL,88);")
edit(p, anchor, add, 'dump.sql effects row 58')

en = 'You are full of energy, thanks to the delicious meal! Time to perform abilities reduced by 20%%'
en2 = 'You feel inspired! Time to perform abilities reduced by 20%%'
for p, lab in ((S + r'\data\cm_messages.xml', 'server messages'), (C + r'\data\cm_messages.xml', 'client messages')):
    edit(p, en, en2, lab)
de = 'Dank dieser vorzüglichen Mahlzeit bist du voller Energie! Du benötigst 20%% weniger Zeit, um Fähigkeiten durchzuführen.'
de2 = 'Du fühlst dich inspiriert! Du benötigst 20%% weniger Zeit, um Fähigkeiten durchzuführen.'
edit(C + r'\data\loc\de\data\cm_messages.xml', de, de2, 'client de messages')
