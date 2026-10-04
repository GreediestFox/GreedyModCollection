"""2026-10-04 (Rezepte 1.xlsx): replace the decorator-kit requirements of the listed recipes with new materials
and set the new skill levels. Recipes not in the sheet stay untouched; an empty skill level keeps the current one.
These recipes consisted ONLY of kit rows, so the sheet list becomes the full requirement list. Influence is split evenly (sum 90).
Targets: art+sql dump.sql (vanilla 1026-1040), mod.cs packs, client recipe.xml + recipe_requirement.xml."""
import os, re, glob, shutil, sys, openpyxl
S = r'G:\Progam (Launcher)\steamapps\common\Life is Feudal Your Own Dedicated Server'
C = r'F:\SteamLibrary\steamapps\common\Life is Feudal Your Own'
BK = r'E:\ClaudeScratch\backups\kit_recipes_20261004'
XLSX = r'E:\ClaudeScratch\kits\Rezepte_1.xlsx'
DRY = '--dry' in sys.argv
os.makedirs(BK, exist_ok=True)
KITS = {1634, 1635, 1636, 3042, 3043, 3044}

# ---------------- read sheet ----------------
plan = {}   # rid -> (reqs [(mat, qty)], lvl or None)
ws = openpyxl.load_workbook(XLSX, data_only=True).active
for row in ws.iter_rows(values_only=True):
    rid, change, lvl = row[0], row[3], row[6]
    if not isinstance(rid, int) or not change: continue
    reqs = {}
    order = []
    for q, m in re.findall(r'(\d+)\s*x\s*(\d+)', str(change)):
        q, m = int(q), int(m)
        if m not in reqs: order.append(m)
        reqs[m] = reqs.get(m, 0) + q          # 5922 lists 583 twice -> merged
    plan[rid] = ([(m, reqs[m]) for m in order], int(lvl) if isinstance(lvl, (int, float)) else None)


def influences(n):
    base = 90 // n
    return [base + (1 if i < 90 - base * n else 0) for i in range(n)]


def bk(p, tag):
    d = os.path.join(BK, tag)
    if not os.path.exists(d): shutil.copy2(p, d)


def rd(p): return open(p, 'rb').read().decode('utf-8')
def wr(p, t):
    if not DRY: open(p, 'wb').write(t.encode('utf-8'))


STR = r"'(?:[^'\\]|\\.|'')*'"


def set_lvl(t, rid, lvl):
    """recipe row: (ID,'Name','Desc',StartingTools,SkillType,SkillLvl,..."""
    pat = re.compile(r"(\(%d,%s,%s,\w+,\d+,)(\d+)(,)" % (rid, STR, STR))
    t2, n = pat.subn(lambda m: m.group(1) + str(lvl) + m.group(3), t)
    return t2, n


report = []
# ---------------- dump.sql ----------------
next_id = [4261]
used_dump_ids = set()
for sub in ('art', 'sql'):
    p = S + '\\' + sub + r'\dump.sql'; bk(p, sub + '_dump.sql'); t = rd(p)
    nid = 4261
    for rid, (reqs, lvl) in plan.items():
        rows = list(re.finditer(r'\n\((\d+),%d,(\d+),\d+,\d+,\d+,\d+\)([,;])' % rid, t))
        rows = [m for m in rows if int(m.group(2)) in KITS]
        if not rows: continue
        assert all(m.group(3) == ',' for m in rows), 'kit row is last row of block'
        ids = [int(m.group(1)) for m in rows]
        new_ids = ids[:len(reqs)]
        while len(new_ids) < len(reqs):
            new_ids.append(nid); nid += 1
        infl = influences(len(reqs))
        block = ''.join('\n(%d,%d,%d,0,%d,%d,0),' % (i, rid, m, inf, q) for i, (m, q), inf in zip(new_ids, reqs, infl))
        first = rows[0]
        # remove the other kit rows (back to front), then replace the first
        for m in reversed(rows[1:]):
            t = t[:m.start()] + t[m.end() - 1:] if False else t[:m.start()] + t[m.end():]
        t = t[:first.start()] + block + t[first.end():]
        if lvl is not None:
            t, n = set_lvl(t, rid, lvl); assert n >= 1, (sub, rid)
        if sub == 'art': report.append('dump.sql %d: %d rows, lvl %s' % (rid, len(reqs), lvl))
    wr(p, t)
for rid in plan:
    pass

# ---------------- mod.cs ----------------
mods = glob.glob(S + r'\mods\LiFx\*\mod.cs')
for p in mods:
    t = rd(p); orig = t; nl = '\r\n' if '\r\n' in t else '\n'
    for rid, (reqs, lvl) in plan.items():
        pat = re.compile(r'([ \t]*)dbi\.Update\("INSERT IGNORE INTO `recipe_requirement` VALUES \(NULL,\s*%d,\s*(\d+),[^\n]*\n' % rid)
        rows = [m for m in pat.finditer(t) if int(m.group(2)) in KITS]
        if not rows: continue
        ind = rows[0].group(1)
        infl = influences(len(reqs))
        block = ''.join('%sdbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,%d,%d,0,%d,%d,0)");%s' % (ind, rid, m, inf, q, nl)
                        for (m, q), inf in zip(reqs, infl))
        for m in reversed(rows[1:]):
            t = t[:m.start()] + t[m.end():]
        m0 = rows[0]
        t = t[:m0.start()] + block + t[m0.end():]
        if lvl is not None:
            t, n = set_lvl(t, rid, lvl); assert n >= 1, (p, rid)
        report.append('%s %d: %d rows, lvl %s' % (os.path.basename(os.path.dirname(p)), rid, len(reqs), lvl))
    if t != orig:
        bk(p, os.path.basename(os.path.dirname(p)) + '_mod.cs'); wr(p, t)

# ---------------- client recipe.xml (SkillLvl) ----------------
p = C + r'\data\recipe.xml'; bk(p, 'client_recipe.xml'); t = rd(p)
for rid, (reqs, lvl) in plan.items():
    if lvl is None: continue
    m = re.search(r'<row>\s*<ID>%d</ID>.*?</row>' % rid, t, re.S)
    seg = re.sub(r'<SkillLvl>\d+</SkillLvl>', '<SkillLvl>%d</SkillLvl>' % lvl, m.group(0))
    t = t[:m.start()] + seg + t[m.end():]
wr(p, t)

# ---------------- client recipe_requirement.xml ----------------
p = C + r'\data\recipe_requirement.xml'; bk(p, 'client_recipe_requirement.xml'); t = rd(p)
nid = max(int(x) for x in re.findall(r'<ID>(\d+)</ID>', t)) + 1
cnt = 0
for rid, (reqs, lvl) in plan.items():
    rows = [m for m in re.finditer(r'[ \t]*<row>\s*<ID>\d+</ID>\s*<RecipeID>%d</RecipeID>\s*<MaterialObjectTypeID>(\d+)</MaterialObjectTypeID>.*?</row>\r?\n' % rid, t, re.S)
            if int(m.group(1)) in KITS]
    if not rows:
        report.append('CLIENT: no kit rows for %d' % rid); continue
    infl = influences(len(reqs))
    block = ''
    for (m, q), inf in zip(reqs, infl):
        block += ('\t<row><ID>%d</ID><RecipeID>%d</RecipeID><MaterialObjectTypeID>%d</MaterialObjectTypeID><Quality>0</Quality>'
                  '<Influence>%d</Influence><Quantity>%d</Quantity><IsRegionItemRequired>0</IsRegionItemRequired></row>\r\n') % (nid, rid, m, inf, q)
        nid += 1; cnt += 1
    for m in reversed(rows[1:]):
        t = t[:m.start()] + t[m.end():]
    m0 = rows[0]
    t = t[:m0.start()] + block + t[m0.end():]
wr(p, t)

print('\n'.join(report))
print('recipes in sheet:', len(plan), '| client rows written:', cnt, '| DRY' if DRY else '')
missing = [rid for rid in plan if not any((' %d:' % rid) in r for r in report)]
print('not found anywhere:', missing)
