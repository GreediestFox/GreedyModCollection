import sys, pefile
from capstone import Cs, CS_ARCH_X86, CS_MODE_64
exe, start, end = sys.argv[1], int(sys.argv[2], 16), int(sys.argv[3], 16)
pe = pefile.PE(exe)
data = pe.get_memory_mapped_image()
md = Cs(CS_ARCH_X86, CS_MODE_64)
for i in md.disasm(data[start:end], start):
    print('%08x  %-24s %s %s' % (i.address, i.bytes.hex(' '), i.mnemonic, i.op_str))
