#!/usr/bin/env python3
r"""
toggle_client_substance_alias.py - switch the client's substance-alias jump (patch_client_substance_alias.py)
OFF or back ON in place, without restoring an old exe backup (which would undo every later client patch).

At RVA 0xa6bc10 the alias patch wrote: E9 <rel32> 90 90 90  (jmp into the .subal cave + 3 NOPs)
The original 8 bytes there are:        4C 8B 49 10 44 0F B6 C2  (mov r9,[rcx+0x10] ; movzx r8d,dl)

Usage: python toggle_client_substance_alias.py <yo_cm_client.exe> status|off|on
  off : writes the original 8 bytes (alias disabled; the .subal section stays in the file, unused)
  on  : re-writes the jmp to the start of the .subal section (alias enabled again)
A timestamped copy of the exe is saved to <client>/subal_toggle_backups before any write.
"""
import sys, os, struct, shutil, datetime

TARGET_RVA = 0xa6bc10
ORIG = bytes.fromhex('4c8b4910440fb6c2')


def sections(d):
    lf = struct.unpack_from('<I', d, 0x3c)[0]
    nsec, = struct.unpack_from('<H', d, lf + 6)
    optsz, = struct.unpack_from('<H', d, lf + 20)
    sec0 = lf + 24 + optsz
    out = []
    for i in range(nsec):
        o = sec0 + 40 * i
        name = bytes(d[o:o + 8]).rstrip(b'\0').decode(errors='replace')
        vs, va, rs, rp = struct.unpack_from('<IIII', d, o + 8)
        out.append((name, va, vs, rp, rs))
    return out


def r2o(secs, rva):
    for name, va, vs, rp, rs in secs:
        if va <= rva < va + max(vs, rs):
            return rp + (rva - va)
    raise SystemExit('rva %x not in any section' % rva)


def main():
    if len(sys.argv) != 3 or sys.argv[2] not in ('status', 'off', 'on'):
        print(__doc__); sys.exit(1)
    exe, mode = sys.argv[1], sys.argv[2]
    d = bytearray(open(exe, 'rb').read())
    secs = sections(d)
    off = r2o(secs, TARGET_RVA)
    cur = bytes(d[off:off + 8])
    subal = [s for s in secs if s[0] == '.subal']
    jmp = None
    if subal:
        cave = subal[0][1]
        jmp = b'\xE9' + struct.pack('<i', cave - (TARGET_RVA + 5)) + b'\x90\x90\x90'
    state = 'OFF (original bytes)' if cur == ORIG else ('ON (jmp to .subal)' if cur == jmp else 'UNKNOWN ' + cur.hex())
    print('alias site', hex(TARGET_RVA), 'file offset', hex(off), '->', state, '| .subal section:', 'yes' if subal else 'no')
    if mode == 'status':
        return
    if state.startswith('UNKNOWN'):
        raise SystemExit('unexpected bytes - refusing to write')
    want = ORIG if mode == 'off' else jmp
    if want is None:
        raise SystemExit('no .subal section - cannot switch ON')
    if cur == want:
        print('already', mode); return
    bk = os.path.join(os.path.dirname(os.path.abspath(exe)), 'subal_toggle_backups')
    os.makedirs(bk, exist_ok=True)
    dst = os.path.join(bk, 'yo_cm_client.exe.before_%s_%s' % (mode, datetime.datetime.now().strftime('%Y%m%d_%H%M%S')))
    shutil.copy2(exe, dst)
    d[off:off + 8] = want
    open(exe, 'wb').write(d)
    print('backup:', dst)
    print('alias switched', mode.upper())


if __name__ == '__main__':
    main()
