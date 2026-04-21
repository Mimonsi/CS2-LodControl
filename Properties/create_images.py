from PIL import Image, ImageDraw, ImageFont

paths = [
    "1.jpg",
    "2.jpg",
    "3.jpg",
    "4.jpg",
]
labels = ["50%", "100%", "500%", "No LOD"]
font_path = "Poppins-Bold.ttf"

# True  = each image contributes its own quarter of the full frame (composite)
# False = center crop of each image repeated across all segments
COMPOSITE_MODE = True

imgs = [Image.open(p) for p in paths]
w, h = imgs[0].size  # 1920x1080


def add_label(img_crop, label, font_size=72, margin_bottom=120):
    img = img_crop.copy().convert("RGBA")
    overlay = Image.new("RGBA", img.size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(overlay)
    font = ImageFont.truetype(font_path, font_size)

    padding_x, padding_y = 28, 18
    bbox = draw.textbbox((0, 0), label, font=font)
    tw = bbox[2] - bbox[0]
    th = bbox[3] - bbox[1]

    pill_x0 = 36
    pill_y1 = img.height - margin_bottom
    pill_y0 = pill_y1 - th - padding_y * 2
    pill_x1 = pill_x0 + tw + padding_x * 2

    draw.rounded_rectangle([pill_x0, pill_y0, pill_x1, pill_y1], radius=18, fill=(10, 20, 30, 190))
    draw.text((pill_x0 + padding_x, pill_y0 + padding_y), label, font=font, fill=(255, 255, 255, 255))

    return Image.alpha_composite(img, overlay).convert("RGB")


def get_crop(img, index, seg_w, mode_composite):
    if mode_composite:
        x0 = index * seg_w
        x1 = x0 + seg_w if index < 3 else w
        return img.crop((x0, 0, x1, h))
    else:
        cx = w // 2
        return img.crop((cx - seg_w // 2, 0, cx + seg_w // 2, h))


# --- Banner ---
seg_w_banner = h  # 1080px per segment → 4320×1080 total
banner_w = seg_w_banner * 4 if not COMPOSITE_MODE else w
if COMPOSITE_MODE:
    seg_w_banner = w // 4  # 480px per segment → 1920×1080 total

banner = Image.new("RGB", (seg_w_banner * 4, h))
for i, img in enumerate(imgs):
    crop = get_crop(img, i, seg_w_banner, COMPOSITE_MODE)
    font_size = 52 if COMPOSITE_MODE else 72
    labeled = add_label(crop, labels[i], font_size=font_size, margin_bottom=120)
    banner.paste(labeled, (i * seg_w_banner, 0))

draw_b = ImageDraw.Draw(banner)
for i in range(1, 4):
    draw_b.line([(i * seg_w_banner, 0), (i * seg_w_banner, h)], fill=(255, 255, 255, 80), width=2)

banner.save("lodcontrol_banner_labeled.jpg", quality=95)
print(f"Banner saved: {banner.size}")

# --- Square ---
seg_sq = h // 4  # 270px per segment → 1080×1080 total
square = Image.new("RGB", (h, h))
for i, img in enumerate(imgs):
    if COMPOSITE_MODE:
        # map the square segment back to the corresponding horizontal slice of the full image
        src_x0 = int(i * w / 4)
        src_x1 = int((i + 1) * w / 4)
        crop = img.crop((src_x0, 0, src_x1, h)).resize((seg_sq, h), Image.LANCZOS)
    else:
        cx = w // 2
        crop = img.crop((cx - seg_sq // 2, 0, cx + seg_sq // 2, h))
    labeled = add_label(crop, labels[i], font_size=28, margin_bottom=50)
    square.paste(labeled, (i * seg_sq, 0))

draw_s = ImageDraw.Draw(square)
for i in range(1, 4):
    draw_s.line([(i * seg_sq, 0), (i * seg_sq, h)], fill=(255, 255, 255, 80), width=1)

square.save("lodcontrol_square_labeled.jpg", quality=95)
print(f"Square saved: {square.size}")
print("Done")