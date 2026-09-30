#!/usr/bin/env python3
"""
make_dxt1.py - convert any image to a DXT1 (BC1) .dds with a full mipmap chain, the format vanilla LiF:YO
diffuse textures use (Pillow writes only uncompressed 24-bit RGB DDS, which D3D9 can't sample -> renders black/grey).

Usage: python make_dxt1.py <input image> <output.dds> [size] [tile]   (size: square power of two, default 512;
       the input is center-cropped to a square first; tile N repeats it NxN (edge-blended, seamless) for models whose UVs span
       the whole building once - e.g. the EKR Lehm houses)
"""
import sys, struct
import numpy as np
from PIL import Image


def to565(c):
    c = c.astype(np.int32)
    return ((c[..., 0] >> 3) << 11) | ((c[..., 1] >> 2) << 5) | (c[..., 2] >> 3)


def from565(v):
    r = (v >> 11) & 31
    g = (v >> 5) & 63
    b = v & 31
    return np.stack([(r << 3) | (r >> 2), (g << 2) | (g >> 4), (b << 3) | (b >> 2)], -1).astype(np.int32)


def encode_level(rgb):
    h, w, _ = rgb.shape
    bw, bh = max(1, (w + 3) // 4), max(1, (h + 3) // 4)
    pad = np.zeros((bh * 4, bw * 4, 3), np.uint8)
    pad[:h, :w] = rgb
    if h < 4 or w < 4:  # replicate edge pixels for tiny mips
        pad[:, w:] = pad[:, w - 1:w]
        pad[h:, :] = pad[h - 1:h, :]
    blocks = pad.reshape(bh, 4, bw, 4, 3).transpose(0, 2, 1, 3, 4).reshape(-1, 16, 3).astype(np.int32)

    # endpoints: extremes along the principal axis of each block
    mean = blocks.mean(1, keepdims=True)
    cen = blocks - mean
    cov = np.einsum('bni,bnj->bij', cen, cen)
    axis = np.ones((len(blocks), 3))
    for _ in range(8):  # power iteration
        axis = np.einsum('bij,bj->bi', cov, axis)
        axis /= np.linalg.norm(axis, axis=1, keepdims=True) + 1e-9
    proj = np.einsum('bni,bi->bn', cen, axis)
    c0 = blocks[np.arange(len(blocks)), proj.argmax(1)]
    c1 = blocks[np.arange(len(blocks)), proj.argmin(1)]
    v0, v1 = to565(c0), to565(c1)
    swap = v0 < v1
    v0, v1 = np.where(swap, v1, v0), np.where(swap, v0, v1)
    same = v0 == v1
    v0 = np.where(same & (v0 < 0xFFFF), v0 + 1, v0)  # keep 4-colour mode (v0 > v1)
    v1 = np.where(same & (v0 == 0xFFFF), v1 - 1, v1)
    p0, p1 = from565(v0), from565(v1)
    pal = np.stack([p0, p1, (2 * p0 + p1) // 3, (p0 + 2 * p1) // 3], 1)  # (b,4,3)
    d = ((blocks[:, :, None, :] - pal[:, None, :, :]) ** 2).sum(-1)  # (b,16,4)
    idx = d.argmin(-1).astype(np.uint32)
    bits = (idx << (2 * np.arange(16, dtype=np.uint32))).sum(1).astype(np.uint32)
    out = np.zeros((len(blocks), 8), np.uint8)
    out[:, 0:2] = v0.astype('<u2').view(np.uint8).reshape(-1, 2)
    out[:, 2:4] = v1.astype('<u2').view(np.uint8).reshape(-1, 2)
    out[:, 4:8] = bits.astype('<u4').view(np.uint8).reshape(-1, 4)
    return out.tobytes()


def main():
    src, dst = sys.argv[1], sys.argv[2]
    size = int(sys.argv[3]) if len(sys.argv) > 3 else 512
    tile = int(sys.argv[4]) if len(sys.argv) > 4 else 1
    img = Image.open(src).convert('RGB')
    w, h = img.size
    s = min(w, h)
    img = img.crop(((w - s) // 2, (h - s) // 2, (w - s) // 2 + s, (h - s) // 2 + s))
    if tile > 1:  # repeat tile x tile; edges cross-faded with a half-offset copy so the joins are seamless
        t = size // tile
        a = np.asarray(img.resize((t, t), Image.LANCZOS), np.float32)
        r = np.roll(a, (t // 2, t // 2), (0, 1))
        ramp = np.minimum(1.0, np.minimum(np.arange(t), t - 1 - np.arange(t)) / (t * 0.25))
        wgt = np.minimum.outer(ramp, ramp)[..., None]  # 1 in the middle, 0 at every edge
        a = (wgt * a + (1 - wgt) * r).clip(0, 255).astype(np.uint8)
        img = Image.fromarray(np.tile(a, (tile, tile, 1)))
    else:
        img = img.resize((size, size), Image.LANCZOS)
    levels, data, cur = 0, b'', img
    while True:
        data += encode_level(np.asarray(cur, np.uint8))
        levels += 1
        if cur.size[0] == 1:
            break
        cur = cur.resize((max(1, cur.size[0] // 2), max(1, cur.size[1] // 2)), Image.BOX)
    # magic, size, flags, height, width, linearSize, depth, mips, reserved[11],
    # pixelformat{size, flags=FOURCC, fourCC, rgbBits, R, G, B, A masks}, caps, caps2, caps3, caps4, reserved2
    hdr = struct.pack('<4sIIIIIII44sII4s5I5I', b'DDS ', 124, 0x000A1007, size, size, size * size // 2, 0, levels,
                      b'\0' * 44, 32, 0x4, b'DXT1', 0, 0, 0, 0, 0, 0x401008, 0, 0, 0, 0)
    assert len(hdr) == 128
    open(dst, 'wb').write(hdr + data)
    print('wrote %s: %dx%d DXT1, %d mips, %d bytes' % (dst, size, size, levels, len(hdr) + len(data)))


if __name__ == '__main__':
    main()
