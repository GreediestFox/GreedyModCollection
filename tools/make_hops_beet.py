"""Hops (grape model) with green cones and Sugar Beet (carrot model) with a white root.
Copies grape*/carrot* .stbin to hops*/sugarbeet*.stbin with the baked texture name 'CropsAtlas' replaced by a
same-length name ('CropsHopsA' / 'CropsBeetA'), and writes recoloured copies of the crops atlas under those names.
Vanilla grape/carrot keep the original atlas.
The game only streams .dds: convert the two PNGs with png_to_dds.py and put them, plus copies of
cropsatlas_normal.dds / cropsatlas_winter.dds as <name>_Normal.dds / <name>_winter.dds, into cropsspeedtree/textures/."""
import os, argparse
import numpy as np
from PIL import Image

ap = argparse.ArgumentParser()
ap.add_argument('--client', required=True, help='Life is Feudal Your Own client folder (read only)')
ap.add_argument('--stage', required=True, help='output folder')
args = ap.parse_args()
D = os.path.join(args.client, r'art\models\3d\environment\cropsspeedtree')
T = D + r'\textures'
STAGE = args.stage
os.makedirs(STAGE, exist_ok=True)

atlas = np.asarray(Image.open(T + r'\cropsatlas.dds').convert('RGBA')).astype(np.float32) / 255
rgb, al = atlas[..., :3], atlas[..., 3]
mx = rgb.max(-1); mn = rgb.min(-1); d = mx - mn + 1e-6
r, g, b = rgb[..., 0], rgb[..., 1], rgb[..., 2]
hue = np.where(mx == r, ((g - b) / d) % 6, np.where(mx == g, (b - r) / d + 2, (r - g) / d + 4)) * 60
sat = d / (mx + 1e-6)
luma = 0.299 * r + 0.587 * g + 0.114 * b
H, W = al.shape
yy, xx = np.mgrid[0:H, 0:W]


def save(arr, name):
    Image.fromarray((np.clip(arr, 0, 1) * 255).astype(np.uint8), 'RGBA').save(os.path.join(STAGE, name))
    print('wrote', name)


# --- Hops: grape bunches (purple) -> light hop-cone green, only inside the grape-bunch area of the atlas
hop = atlas.copy()
region = (xx >= 760) & (xx <= 900) & (yy >= 1290) & (yy <= 1520)
m = region & ((hue > 200) | (hue < 15)) & (sat > 0.06) & (al > 0.02)
v = luma[m] / max(luma[m].max(), 1e-6)
shade = 0.45 + 0.55 * v                       # dark grapes -> mid/light green, keep the shading
hop_rgb = np.stack([0.62 * shade, 0.78 * shade, 0.30 * shade], -1)
hop[..., :3][m] = hop_rgb
print('hops pixels recoloured:', int(m.sum()))
save(hop, 'CropsHopsA.png')

# --- Sugar beet: orange carrot root -> creamy white, only inside the carrot-root area
beet = atlas.copy()
region = (xx >= 0) & (xx <= 110) & (yy >= 1150) & (yy <= 1540)
m = region & ((hue < 55) | (hue > 330)) & (sat > 0.12) & (al > 0.02)
v = luma[m] / max(luma[m].max(), 1e-6)
shade = 0.62 + 0.38 * v
beet_rgb = np.stack([0.97 * shade, 0.95 * shade, 0.88 * shade], -1)
beet[..., :3][m] = beet_rgb
print('beet pixels recoloured:', int(m.sum()))
save(beet, 'CropsBeetA.png')

# previews of the changed areas (2x)
Image.open(os.path.join(STAGE, 'CropsHopsA.png')).crop((740, 1270, 920, 1540)).resize((360, 540)).save(os.path.join(STAGE, 'preview_hops.png'))
Image.open(os.path.join(STAGE, 'CropsBeetA.png')).crop((0, 1130, 130, 1560)).resize((260, 860)).save(os.path.join(STAGE, 'preview_beet.png'))

# --- model copies with the texture name swapped (same length, so the file layout is untouched)
for src_pfx, dst_pfx, newname in (('grape', 'hops', b'CropsHopsA'), ('carrot', 'sugarbeet', b'CropsBeetA')):
    for st in ('small01', 'small02', 'big01', 'big02'):
        data = open(os.path.join(D, src_pfx + st + '.stbin'), 'rb').read()
        n = data.count(b'CropsAtlas')
        out = data.replace(b'CropsAtlas', newname)
        assert len(out) == len(data)
        open(os.path.join(STAGE, dst_pfx + st + '.stbin'), 'wb').write(out)
        print('%s%s.stbin: %d texture refs renamed' % (dst_pfx, st, n))
