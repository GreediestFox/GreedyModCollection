import os
import re, os, glob, csv, subprocess, collections

S = r'G:\Progam (Launcher)\steamapps\common\Life is Feudal Your Own Dedicated Server'
C = r'F:\SteamLibrary\steamapps\common\Life is Feudal Your Own'
DE = C + r'\data\loc\de\data'
MYSQL = r'C:\Program Files\MariaDB 12.0\bin\mysql.exe'


def q(sql):
    r = subprocess.run([MYSQL, '--skip-ssl', '--host=127.0.0.1', '-uroot', '-p' + os.environ.get('LIF_DB_PASSWORD', ''), 'lif_1', '-N', '-B', '-e', sql], capture_output=True, text=True, encoding='utf-8')
    return [l.split('\t') for l in r.stdout.splitlines()]


def loc(fn):
    t = open(DE + '\\' + fn, encoding='utf-8', errors='replace').read()
    return {int(i): v for i, v in re.findall(r'<string id="(\d+)">(.*?)</string>', t, re.S)}


de_on, de_od, de_rn, de_rd = loc('objects_types_Name.xml'), loc('objects_types_Description.xml'), loc('recipe_Name.xml'), loc('recipe_Description.xml')
skills = {int(a): b for a, b in q('select ID,Name from skill_types')} if False else {}
try:
    skills = {int(a[0]): a[1] for a in q('select ID,Name from skill_types')}
except Exception:
    pass
parents = {int(a[0]): (int(a[1]) if a[1] != 'NULL' else 0, a[2]) for a in q('select ID,ParentID,Name from objects_types')}


def split_sql(s):
    parts, cur, qq, i = [], '', False, 0
    while i < len(s):
        ch = s[i]
        if qq and ch == '\\':
            j = i
            while j < len(s) and s[j] == '\\':
                j += 1
            if j < len(s):
                cur += s[j]
            i = j + 1
            continue
        if ch == "'":
            if qq and i + 1 < len(s) and s[i + 1] == "'":
                cur += "'"
                i += 2
                continue
            qq = not qq
            i += 1
            continue
        if ch == ',' and not qq:
            parts.append(cur.strip())
            cur = ''
        else:
            cur += ch
        i += 1
    parts.append(cur.strip())
    return parts


def root_cat(pid):
    chain = []
    while pid in parents and pid not in chain:
        chain.append(pid)
        pid = parents[pid][0]
    return ' > '.join(parents[x][1] for x in reversed(chain[-3:]))


objs, recs = [], []
for mp in sorted(glob.glob(S + r'\mods\LiFx\*\mod.cs')):
    pack = os.path.basename(os.path.dirname(mp))
    for ln in open(mp, encoding='utf-8', errors='replace'):
        commented = ln.lstrip().startswith('//')
        body = ln.lstrip('/ \t')
        m = re.search(r'INSERT IGNORE INTO `objects_types` VALUES \((.*)\)"\)', body)
        if m:
            p = split_sql(m.group(1))
            g = lambda k: p[k] if len(p) > k else '0'
            try:
                objs.append(dict(pack=pack, id=int(p[0]), parent=int(p[1]), name=p[2], mov=g(4), unmov=g(5), dev=g(7), icon=p[21] if len(p) > 21 else '', commented=commented))
            except ValueError:
                print('skip obj line', pack, ln[:100])
            continue
            continue
        m = re.search(r'INSERT IGNORE INTO `recipe` VALUES \((.*)\)"\)', body)
        if m:
            p = split_sql(m.group(1))
            try:
                recs.append(dict(pack=pack, id=int(p[0]), name=p[1], skill=int(p[4]), lvl=p[5], result=int(p[6]), commented=commented))
            except (ValueError, IndexError):
                print('skip rec line', pack, ln.strip()[:140])

# live DB state for recipes (skill/level as currently served, which reflects dump.sql edits) - only for ids present
db_rec = {int(a[0]): a for a in q('select ID,Name,SkillTypeID,SkillLvl,ResultObjectTypeID from recipe')}
db_obj = {int(a[0]): a for a in q('select ID,Name,ParentID,IsMovableObject,IsUnmovableobject,IsDevice from objects_types')}

# dedupe (same id defined in several lines/packs: last wins, but keep first pack)
oo = {}
for o in objs:
    oo.setdefault(o['id'], o)
rr = {}
for r in recs:
    rr.setdefault(r['id'], r)


def kind(o):
    d = db_obj.get(o['id'])
    if d:
        mov, unm, dev = d[3], d[4], d[5]
    else:
        mov, unm, dev = o['mov'], o['unmov'], o['dev']
    if d and d[2] == '1902':
        return 'Carried twin of a movable'
    if dev == '1':
        return 'Device/workshop'
    if mov == '1':
        return 'Movable (furniture/decoration/siege)'
    if unm == '1':
        return 'Building'
    return 'Item'


out_o = []
for i in sorted(oo):
    o = oo[i]
    d = db_obj.get(i)
    name = d[1] if d else o['name']
    de = de_on.get(i)
    out_o.append([i, name, o['pack'], kind(o), root_cat(d and int(d[2]) or o['parent']), 'yes' if d else 'NOT IN DB', 'commented' if o['commented'] else '',
                  'OK' if de and de.strip() and de != name else ('MISSING' if not de else 'same-as-English'), de or '', 'yes' if i in de_od else 'no'])
out_r = []
for i in sorted(rr):
    r = rr[i]
    d = db_rec.get(i)
    name = d[1] if d else r['name']
    skill = int(d[2]) if d else r['skill']
    lvl = d[3] if d else r['lvl']
    res = int(d[4]) if d else r['result']
    de = de_rn.get(i)
    out_r.append([i, name, r['pack'], res, (db_obj.get(res) or ['', '?'])[1], skill, skills.get(skill, ''), lvl, 'yes' if d else 'NOT IN DB', 'commented' if r['commented'] else '',
                  'OK' if de and de.strip() and de != name else ('MISSING' if not de else 'same-as-English'), de or ''])

w = lambda fn, hdr, rows: (lambda f: (csv.writer(f, delimiter=';').writerow(hdr), csv.writer(f, delimiter=';').writerows(rows)))(open(fn, 'w', encoding='utf-8-sig', newline=''))
OUT = os.path.join(os.path.dirname(os.path.abspath(__file__)), '..', 'docs', 'reports')
w(OUT + r'\LiFx_ModObjects_Translation.csv', ['ObjectID', 'Name (EN)', 'Pack', 'Kind', 'Category', 'In DB', 'Note', 'German name status', 'German name now', 'German description present'], out_o)
w(OUT + r'\LiFx_ModRecipes_Translation.csv', ['RecipeID', 'Name (EN)', 'Pack', 'ResultID', 'Result name', 'SkillID', 'Skill', 'Level', 'In DB', 'Note', 'German name status', 'German name now'], out_r)

print('objects', len(out_o), 'recipes', len(out_r))
print('objects by pack/status:')
c = collections.Counter((r[2], r[7]) for r in out_o)
for k in sorted(c):
    print('  ', k, c[k])
print('recipes by pack/status:')
c = collections.Counter((r[2], r[10]) for r in out_r)
for k in sorted(c):
    print('  ', k, c[k])
print('not in DB obj:', sum(1 for r in out_o if r[5] != 'yes'), 'rec:', sum(1 for r in out_r if r[8] != 'yes'))
