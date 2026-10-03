"""Port MMO effects into free YO slots (2026-10-03):
  61 (was Iron Grip, MMO-only)          <- MMO 108 Exhausted: SPEED x0.7 fixed, no run/jump/operate
  84 (was Skills Drop Insurance, MMO)   <- MMO 132 Increased XP Gain: SKILL_GROW_BONUS_MULT +1.0 (all skills)
  64 StunFeets (= MMO Shackled Feet)    -> name 'Shackled Feet' + Root icon (flags unchanged)
Descriptions go into the reserved blank message slots 5161 / 5162. cm_effects.xml + cm_messages.xml stay identical
on server and client."""
import os, re, shutil
SRV = r'G:\Progam (Launcher)\steamapps\common\Life is Feudal Your Own Dedicated Server'
CL = r'F:\SteamLibrary\steamapps\common\Life is Feudal Your Own'
BK = r'E:\ClaudeScratch\backups\mmo_effects_20261003'
os.makedirs(BK, exist_ok=True)
FLAGS_X = '<flags CanMove="TRUE" CanRun="FALSE" CanJump="FALSE" CanRotate="TRUE" CanRotateHead="TRUE" CanOperate="FALSE" CanBeCanceled="FALSE" RemovedOnDeath="TRUE" />'
FLAGS_XP = '<flags CanMove="TRUE" CanRun="TRUE" CanJump="TRUE" CanRotate="TRUE" CanRotateHead="TRUE" CanOperate="TRUE" CanBeCanceled="FALSE" RemovedOnDeath="FALSE" />'


def block(i, name, flags, desc, params, icon, ind='    '):
    s = '%s<effect id="%d" name="%s">\r\n%s    %s\r\n%s    <description>%d</description>\r\n' % (ind, i, name, ind, flags, ind, desc)
    for p in params:
        s += '%s    %s\r\n' % (ind, p)
    s += '%s    <icon>%s</icon>\r\n%s</effect>' % (ind, icon, ind)
    return s


NEW = {
    61: block(61, 'Exhausted', FLAGS_X, 5162, ['<parameter type="SPEED" applytype="MULTIPLY" value="0.7" ignore_magnitude="1" />'], 'art/2D/Effects/Exhausted.png'),
    84: block(84, 'Increased XP Gain', FLAGS_XP, 5161, ['<parameter type="SKILL_GROW_BONUS_MULT" applytype="INCREASE_COEFF" filter="ALL" value="1" />'], 'art/2D/Effects/Increased_XP.png'),
}


def fix_effects(t):
    for i, b in NEW.items():
        m = re.search(r'[ \t]*<effect id="%d" name="[^"]*">.*?</effect>' % i, t, re.S)
        t = t[:m.start()] + '    <!-- LiFx 2026-10-03: ported from the MMO (slot was an unused MMO-only effect) -->\r\n' + b + t[m.end():]
    m = re.search(r'<effect id="64" name="StunFeets">.*?</effect>', t, re.S)
    seg = m.group(0).replace('name="StunFeets"', 'name="Shackled Feet"').replace('<icon></icon>', '<icon>art/2D/Effects/Root.png</icon>')
    return t[:m.start()] + seg + t[m.end():]


def setmsg(t, mid, text):
    t2, n = re.subn(r'<string id="%d"([^>]*)>[^<]*</string>' % mid, lambda m: '<string id="%d"%s>%s</string>' % (mid, m.group(1), text), t)
    if n == 0:
        last = t2.rfind('</string>') + len('</string>')
        t2 = t2[:last] + '\r\n<string id="%d">%s</string>' % (mid, text) + t2[last:]
    return t2


def rw(p, tag, fn):
    shutil.copy2(p, os.path.join(BK, tag))
    raw = open(p, 'rb').read(); bom = raw.startswith(b'\xef\xbb\xbf'); t = raw.decode('utf-8-sig')
    open(p, 'wb').write((b'\xef\xbb\xbf' if bom else b'') + fn(t).encode('utf-8'))
    print('updated', tag)


for root, tag in ((SRV, 'server'), (CL, 'client')):
    rw(root + r'\data\cm_effects.xml', tag + '_cm_effects.xml', fix_effects)
    rw(root + r'\data\cm_messages.xml', tag + '_cm_messages.xml',
       lambda t: setmsg(setmsg(t, 5161, 'You gain more XP.'), 5162, "You've spent all your strength and need a while to catch your breath."))
rw(CL + r'\data\loc\de\data\cm_messages.xml', 'de_cm_messages.xml',
   lambda t: setmsg(setmsg(t, 5161, 'Du profitierst von einem erhöhten EP-Erhalt.'), 5162, 'Du hast all deine Kraft verbraucht und brauchst eine Weile, um wieder zu Atem zu kommen.'))
rw(CL + r'\data\loc\de\data\cm_effects_name.xml', 'de_cm_effects_name.xml',
   lambda t: setmsg(setmsg(setmsg(t, 61, 'Erschöpft'), 84, 'Erhöhter EP-Zuwachs'), 64, 'Gefesselte Füße'))
for icon in ('Exhausted.png', 'Increased_XP.png'):
    dst = CL + r'\art\2D\Effects\\' + icon
    if not os.path.exists(dst):
        shutil.copy2(r'E:\ClaudeScratch\qbms\out\art_2D\Effects\\' + icon, dst); print('icon copied', icon)
