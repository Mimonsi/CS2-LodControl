from PIL import Image, ImageDraw, ImageFont

paths = [
    "1.jpg",
    "2.jpg",
    "3.jpg",
    "4.jpg",
]
labels = ["50%", "100%", "500%", "No LOD"]
font_path = "Poppins-Bold.ttf"

imgs = [Image.open(p) for p in paths]
w, h = imgs[0].size  # 1920x1080
seg_w = h  # 1080 — each segment is square (center crop)
cx = w // 2
left = cx - seg_w // 2
right = cx + seg_w // 2

def add_label(img_crop, label, font_size=72, margin_bottom=120):
    img = img_crop.copy().convert("RGBA")
    overlay = Image.new("RGBA", img.size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(overlay)
    font = ImageFont.truetype(font_path, font_size)

    padding_x, padding_y = 28, 18
    bbox = draw.textbbox((0, 0), label, font=font)
    # bbox may have a top offset (e.g. bbox[1] != 0) — account for it
    text_x_offset = bbox[0]
    text_y_offset = bbox[1]
    tw = bbox[2] - bbox[0]
    th = bbox[3] - bbox[1]

    pill_w = tw + padding_x * 2
    pill_h = th + padding_y * 2
    pill_x0 = (img.width - pill_w) // 2
    pill_x1 = pill_x0 + pill_w
    pill_y1 = img.height - margin_bottom
    pill_y0 = pill_y1 - pill_h

    draw.rounded_rectangle([pill_x0, pill_y0, pill_x1, pill_y1], radius=18, fill=(10, 20, 30, 190))
    # subtract the bbox offsets so text sits exactly centered in the pill
    draw.text(
        (pill_x0 + padding_x - text_x_offset, pill_y0 + padding_y - text_y_offset),
        label, font=font, fill=(255, 255, 255, 255)
    )

    return Image.alpha_composite(img, overlay).convert("RGB")

# --- Banner (2x2) ---
banner = Image.new("RGB", (seg_w * 2, h * 2))
for i, img in enumerate(imgs):
    crop = img.crop((left, 0, right, h))
    labeled = add_label(crop, labels[i], font_size=72, margin_bottom=120)
    col = i % 2
    row = i // 2
    banner.paste(labeled, (col * seg_w, row * h))

draw_b = ImageDraw.Draw(banner)
draw_b.line([(seg_w, 0), (seg_w, h * 2)], fill=(255, 255, 255, 80), width=2)
draw_b.line([(0, h), (seg_w * 2, h)], fill=(255, 255, 255, 80), width=2)

banner.save("lodcontrol_banner_labeled.jpg", quality=95)
print(f"Banner saved: {banner.size}")

# --- Square (1x4) ---
seg_sq = h // 4
square = Image.new("RGB", (h, h))
for i, img in enumerate(imgs):
    l2 = cx - seg_sq // 2
    r2 = cx + seg_sq // 2
    crop = img.crop((l2, 0, r2, h))
    labeled = add_label(crop, labels[i], font_size=28, margin_bottom=50)
    square.paste(labeled, (i * seg_sq, 0))

draw_s = ImageDraw.Draw(square)
for i in range(1, 4):
    draw_s.line([(i * seg_sq, 0), (i * seg_sq, h)], fill=(255, 255, 255, 80), width=1)

square.save("lodcontrol_square_labeled.jpg", quality=95)
print(f"Square saved: {square.size}")
print("Done")