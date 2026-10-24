import sqlite3

SRC = r"E:\ClaudeScratch\books_registry\src_zip_20261010\BookWebsite\data\skriptorium.sqlite"
OUT = r"E:\ClaudeScratch\books_registry\cloudflare\migrate_old_data.sql"
c = sqlite3.connect(SRC)


def q(v):
    if v is None:
        return "NULL"
    if isinstance(v, int):
        return str(v)
    return "'" + str(v).replace("'", "''") + "'"


lines = []
for table in ("documents", "blocks"):
    cols = [r[1] for r in c.execute("pragma table_info(%s)" % table)]
    for row in c.execute("select * from %s" % table):
        lines.append("INSERT OR IGNORE INTO %s (%s) VALUES (%s);" % (table, ",".join(cols), ",".join(q(v) for v in row)))
open(OUT, "w", encoding="utf-8").write("\n".join(lines) + "\n")
print(len(lines), "statements")
