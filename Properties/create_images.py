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
COMPOSITE_MODE = False

# True  = reverse order so "No LOD" is on the left (high detail on the right)
# False = default order: 50% -> 100% -> 500% -> No LOD
REVERSE_ORDER = False

if REVERSE_ORDER:
    paths = paths[::-1]
    labels = labels[::-1]

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


def get_crop(img, index, seg_w):
    """Return a seg_w x h crop from the image. In composite mode each image
    contributes its own spatial quarter; the center seg_w columns of that
    quarter are taken so we never distort (crop, don't stretch)."""
    if COMPOSITE_MODE:
        # full quarter boundaries
        q_x0 = index * (w // 4)
        q_x1 = q_x0 + (w // 4) if index < 3 else w
        q_w = q_x1 - q_x0
        if seg_w >= q_w:
            # banner mode: seg_w == q_w, take the whole quarter
            return img.crop((q_x0, 0, q_x1, h))
        else:
            # square mode: seg_w < q_w, center-crop within the quarter
            cx = q_x0 + q_w // 2
            return img.crop((cx - seg_w // 2, 0, cx + seg_w // 2, h))
    else:
        cx = w // 2
        return img.crop((cx - seg_w // 2, 0, cx + seg_w // 2, h))


# --- Banner ---
seg_w_banner = h if not COMPOSITE_MODE else w // 4  # 1080px or 480px per segment
banner = Image.new("RGB", (seg_w_banner * 4, h))
for i, img in enumerate(imgs):
    crop = get_crop(img, i, seg_w_banner)
    font_size = 52 if COMPOSITE_MODE else 72
    labeled = add_label(crop, labels[i], font_size=font_size, margin_bottom=120)
    banner.paste(labeled, (i * seg_w_banner, 0))

draw_b = ImageDraw.Draw(banner)
for i in range(1, 4):
    draw_b.line([(i * seg_w_banner, 0), (i * seg_w_banner, h)], fill=(255, 255, 255, 80), width=2)

banner.save("lodcontrol_banner_labeled.jpg", quality=95)
print(f"Banner saved: {banner.size}")

# --- Square ---
seg_sq = h // 4  # 270px per segment -> 1080x1080 total
square = Image.new("RGB", (h, h))
for i, img in enumerate(imgs):
    crop = get_crop(img, i, seg_sq)
    labeled = add_label(crop, labels[i], font_size=28, margin_bottom=50)
    square.paste(labeled, (i * seg_sq, 0))

draw_s = ImageDraw.Draw(square)
for i in range(1, 4):
    draw_s.line([(i * seg_sq, 0), (i * seg_sq, h)], fill=(255, 255, 255, 80), width=1)

square.save("lodcontrol_square_labeled.jpg", quality=95)
print(f"Square saved: {square.size}")
print("Done")