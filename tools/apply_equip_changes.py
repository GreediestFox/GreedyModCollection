"""cm_equipTypes.xml (client and server must be identical):
 1. every armor piece (a type whose only slot is 1..6) and all arrows/bolts: required skill = Slinger (47), amount 0  -> equippable by everybody
 2. hand-and-a-half swords (object category 56 'Anderthalbhaender'): slots 12,13 -> 10,11,12,13 (also the hip slots)
Usage: python -I apply_equip_changes.py <cm_equipTypes.xml> [--dry]
Byte-exact: only the changed values are rewritten; line endings and everything else stay."""
import re, sys, shutil, os
import xml.etree.ElementTree as ET

SLINGER = '47'
AMMO = [656, 657, 658, 659, 660, 662, 663, 664, 1096, 1339, 1340, 1582, 1583]
HALFHANDERS = [557, 561, 563, 564, 3230, 3231, 3232, 3233, 3236, 3237]

path = sys.argv[1]
dry = '--dry' in sys.argv
d = open(path, 'rb').read()
txt = d.decode('utf-8')
nl = '\r\n' if '\r\n' in txt else '\n'
stats = dict(armor=0, armor_changed=0, ammo=0, ammo_changed=0, half=0, half_changed=0)


def fix_req(body):
    b2 = re.sub(r'<skillID>\d+</skillID>', '<skillID>%s</skillID>' % SLINGER, body, count=1)
    b2 = re.sub(r'<skillAmount>\d+</skillAmount>', '<skillAmount>0</skillAmount>', b2, count=1)
    return b2


def repl(m):
    oid = int(m.group(1))
    body = m.group(2)
    slots = [int(x) for x in re.findall(r'<slot>(\d+)</slot>', body.split('<skins>')[0])]
    new = body
    if len(slots) == 1 and 1 <= slots[0] <= 6:
        stats['armor'] += 1
        new = fix_req(new)
        if new != body: stats['armor_changed'] += 1
    elif oid in AMMO:
        stats['ammo'] += 1
        new = fix_req(new)
        if new != body: stats['ammo_changed'] += 1
    if oid in HALFHANDERS:
        stats['half'] += 1
        if sorted(slots) == [12, 13]:
            m2 = re.search(r'<slots>(.*?)</slots>', new, re.S)
            ind = re.search(r'([ \t]*)<slot>', m2.group(1)).group(1)
            lines = ''.join(nl + ind + '<slot>%d</slot>' % s for s in (10, 11, 12, 13))
            tail = re.search(r'(\s*)$', m2.group(1)).group(1)
            new = new.replace(m2.group(0), '<slots>' + lines + tail + '</slots>', 1)
            stats['half_changed'] += 1
    return '<object id="%d">%s</object>' % (oid, new)


out = re.sub(r'<object id="(\d+)">(.*?)</object>', repl, txt, flags=re.S)
ET.fromstring(out.encode('utf-8'))   # still well-formed
print(os.path.basename(path), stats, 'bytes %d -> %d' % (len(d), len(out.encode('utf-8'))))
if not dry:
    open(path, 'wb').write(out.encode('utf-8'))
    print('written')
