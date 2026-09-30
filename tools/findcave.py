import sys, pefile
exe = sys.argv[1]; need = int(sys.argv[2])
pe = pefile.PE(exe)
for s in pe.sections:
    print(s.Name.rstrip(b'\0').decode(), 'VA %x VSize %x raw %x rawsize %x chars %x' % (s.VirtualAddress, s.Misc_VirtualSize, s.PointerToRawData, s.SizeOfRawData, s.Characteristics))
data = open(exe, 'rb').read()
text = [s for s in pe.sections if s.Name.startswith(b'.text')][0]
raw = data[text.PointerToRawData:text.PointerToRawData + text.SizeOfRawData]
best = []
i = 0; n = len(raw)
while i < n:
    if raw[i] in (0xCC, 0x00):
        j = i
        while j < n and raw[j] == raw[i]: j += 1
        if j - i >= need: best.append((i, j - i, raw[i]))
        i = j
    else:
        i += 1
print('runs >= %d bytes in .text:' % need)
for off, ln, b in best[:10]:
    print('  RVA %x  len %d  byte %02x  (file off %x)' % (text.VirtualAddress + off, ln, b, text.PointerToRawData + off))
print('text raw end slack: rawsize %x vsize %x' % (text.SizeOfRawData, text.Misc_VirtualSize))
