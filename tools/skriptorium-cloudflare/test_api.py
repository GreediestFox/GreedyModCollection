import json, sys, urllib.request, urllib.error

import os
KEY = os.environ.get("TKEY", "localtestkey123")
B = sys.argv[1] if len(sys.argv) > 1 else "http://127.0.0.1:8799"
fails = 0


def call(method, path, body=None, raw=False):
    data = json.dumps(body).encode() if body is not None else None
    req = urllib.request.Request(B + path, data=data, method=method, headers={"content-type": "application/json", "user-agent": "Mozilla/5.0"})
    try:
        r = urllib.request.urlopen(req, timeout=30)
        t = r.read().decode()
        return r.status, (t if raw else (json.loads(t) if t else {}))
    except urllib.error.HTTPError as e:
        t = e.read().decode()
        try:
            return e.code, (t if raw else json.loads(t))
        except Exception:
            return e.code, t


def check(name, cond, info=""):
    global fails
    print(("PASS " if cond else "FAIL ") + name, "" if cond else info)
    if not cond:
        fails += 1


# ---- letter
s, r = call("POST", "/api/letters", {"title": "", "ticket": "abcd1234efgh"})
check("letter create", s == 200 and r["id"].startswith("_ltr"), (s, r))
lid = r["id"]
s, r = call("PUT", f"/api/letters/{lid}/draft", {"html": '<div>Hallo <b>Welt</b><script>alert(1)</script><a href="x">link</a></div><p onclick="x()">zwei</p>', "title": "Mein Brief"})
check("letter draft", s == 200, (s, r))
s, r = call("GET", f"/api/letters/{lid}")
h = r["blocks"][0]["html"]
print("   sanitized:", h)
check("sanitize removes script/onclick/a", "script" not in h and "onclick" not in h and "<a" not in h and "alert" not in h and "<b>Welt</b>" in h and "<p>" in h, h)
s, r = call("GET", f"/api/ticket/abcd1234efgh", raw=True)
check("ticket 404 before finalize", s == 404)
s, r = call("POST", f"/api/letters/{lid}/finalize", {"sealed": True})
check("letter finalize sealed", s == 200, (s, r))
s, r = call("GET", f"/api/letters/{lid}")
check("sealed hides blocks", r["blocks"] is None and r["sealed"] is True, r)
s, r = call("POST", f"/api/letters/{lid}/break-seal", {})
check("break seal", s == 200 and r["blocks"][-1]["html"] == "<p><em>[Siegel gebrochen]</em></p>", (s, r))
s, r = call("POST", f"/api/letters/{lid}/append", {"html": "<p>Nachtrag</p>"})
check("append", s == 200 and r["seq"] == 2, (s, r))
s, r = call("POST", f"/api/letters/{lid}/seal", {})
check("seal again", s == 200, (s, r))
s, r = call("GET", f"/api/ticket/abcd1234efgh", raw=True)
check("ticket lookup", s == 200 and r.split("\n") == [lid, "Mein Brief"], (s, r))
s, r = call("GET", f"/api/title/{lid}", raw=True)
check("title lookup", s == 200 and r == "Mein Brief", (s, r))
s, r = call("GET", "/api/title/_ltrnotthere", raw=True)
check("title 404", s == 404)
s, r = call("PUT", f"/api/letters/{lid}/draft", {"html": "x"})
check("draft after finalize = 409", s == 409)

# ---- book
s, r = call("POST", "/api/books", {"title": "", "ticket": "bookticket99"})
bid = r["id"]
check("book create", s == 200 and bid.startswith("_bok"), (s, r))
s, r = call("PUT", f"/api/books/{bid}/draft", {"title": "Mein Buch", "author": "Ich", "coverColor": "#aa0000", "coverTextColor": "zzz",
    "pages": ['<span style="color:#ff0000;position:absolute;font-size:20px">rot</span><img src="javascript:alert(1)" alt="a"><img src="https://example.com/x.png" style="width:50%;height:9px" onerror="x">', "<p>Seite 2</p>"]})
check("book draft", s == 200, (s, r))
s, r = call("GET", f"/api/books/{bid}")
print("   pages:", r["pages"])
p0 = r["pages"][0]
check("book sanitize", "position" not in p0 and "color:#ff0000" in p0 and "font-size:20px" in p0 and "javascript" not in p0 and "onerror" not in p0 and 'src="https://example.com/x.png"' in p0 and "width:50%" in p0 and "height" not in p0, p0)
check("book cover", r["author"] == "Ich" and r["coverColor"] == "#aa0000" and r["coverTextColor"] == "" and len(r["pages"]) == 2, r)
s, r = call("POST", f"/api/books/{bid}/finalize", {"password": "ab"})
check("short password refused", s == 400, (s, r))
s, r = call("POST", f"/api/books/{bid}/finalize", {"password": "geheim"})
check("book finalize", s == 200, (s, r))
s, r = call("PUT", f"/api/books/{bid}", {"password": "falsch", "pages": ["x"]})
check("wrong password refused", s == 403, (s, r))
s, r = call("PUT", f"/api/books/{bid}", {"password": "geheim", "pages": ["<p>neu</p>"], "title": "Neuer Titel"})
check("edit with password", s == 200, (s, r))
s, r = call("GET", f"/api/books/{bid}")
check("book edited", r["pages"] == ["<p>neu</p>"] and r["title"] == "Neuer Titel", r)

# ---- tablet
s, r = call("POST", "/api/tablets", {"title": "T", "plainText": "Hallo Welt", "key": "wrong"})
check("tablet wrong key", s == 403, (s, r))
s, r = call("POST", "/api/tablets", {"title": "Tafel", "plainText": "Hallo Welt, gut", "key": KEY, "ticket": "tabletticket1"})
check("tablet create", s == 200 and r["id"].startswith("_tbl"), (s, r))
tid = r["id"]
s, r = call("GET", f"/api/tablets/{tid}")
check("tablet read", s == 200 and "Hallo" not in r["encryptedText"] and len(r["symbols"]) > 0, r)
sym = r["symbols"][0]["symbol"]
s, r = call("POST", f"/api/tablets/{tid}/bonus-reveal", {"count": 50, "excludeSymbols": []})
check("bonus reveal capped at 10", s == 200 and 0 < len(r["pairs"]) <= 10, (s, r))
pair = r["pairs"][0]
s, r = call("POST", f"/api/tablets/{tid}/guess", {"symbol": pair["symbol"], "letter": pair["letter"]})
check("guess correct", r.get("correct") is True, r)
s, r = call("POST", f"/api/tablets/{tid}/reveal", {"mapping": {}})
check("reveal incomplete refused", s == 403)

# ---- pages / routing
req = urllib.request.Request(B + "/open?tag=" + lid, method="GET", headers={"user-agent": "Mozilla/5.0"})


class NoRedirect(urllib.request.HTTPRedirectHandler):
    def redirect_request(self, *a, **k):
        return None


op = urllib.request.build_opener(NoRedirect)
try:
    op.open(req)
    check("open redirect", False)
except urllib.error.HTTPError as e:
    check("open redirect", e.code in (301, 302) and f"/letter.html?id={lid}" in e.headers.get("location", ""), (e.code, e.headers.get("location")))
s, r = call("GET", "/admin/tablet?key=bad", raw=True)
check("admin page needs key", s == 403)
s, r = call("GET", "/admin/tablet?key="+KEY+"", raw=True)
check("admin page with key", s == 200 and "<html" in r.lower(), s)
s, r = call("GET", "/", raw=True)
check("index served", s == 200 and "<html" in r.lower(), s)
s, r = call("GET", "/letter.html?id=x", raw=True)
check("letter.html served", s == 200, s)
s, r = call("GET", "/js/common.js", raw=True)
check("static js served", s == 200 and len(r) > 100, s)
print("FAILS:", fails)
