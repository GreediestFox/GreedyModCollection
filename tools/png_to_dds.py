"""Write an RGBA image as an uncompressed 32-bit DDS (A8R8G8B8) with a full mip chain (box filter).
Usage: python png_to_dds.py <in.png> <out.dds>"""
import sys, struct
import numpy as np
from PIL import Image


def write_dds(img, path):
    a = np.asarray(img.convert('RGBA')).astype(np.float32)
    h, w = a.shape[:2]
    levels = [a]
    while w > 1 or h > 1:
        nh, nw = max(1, h // 2), max(1, w // 2)
        cur = levels[-1][:nh * 2 if h > 1 else 1, :nw * 2 if w > 1 else 1]
        if h > 1: cur = (cur[0::2] + cur[1::2]) / 2
        if w > 1: cur = (cur[:, 0::2] + cur[:, 1::2]) / 2
        levels.append(cur); h, w = nh, nw
    H, W = levels[0].shape[:2]
    DDSD = 0x1 | 0x2 | 0x4 | 0x8 | 0x1000 | 0x20000  # caps|height|width|pitch|pixelformat|mipmapcount
    hdr = struct.pack('<4sIIIIIII', b'DDS ', 124, DDSD, H, W, W * 4, 0, len(levels))
    hdr += b'\0' * 44
    hdr += struct.pack('<II4sIIIII', 32, 0x41, b'\0\0\0\0', 32, 0x00FF0000, 0x0000FF00, 0x000000FF, 0xFF000000)
    hdr += struct.pack('<IIIII', 0x1000 | 0x8 | 0x400000, 0, 0, 0, 0)  # texture|complex|mipmap
    with open(path, 'wb') as f:
        f.write(hdr)
        for lv in levels:
            px = np.clip(lv + 0.5, 0, 255).astype(np.uint8)
            f.write(px[..., [2, 1, 0, 3]].tobytes())  # BGRA
    print('wrote', path, '%dx%d' % (W, H), 'mips', len(levels))


if __name__ == '__main__':
    write_dds(Image.open(sys.argv[1]), sys.argv[2])
