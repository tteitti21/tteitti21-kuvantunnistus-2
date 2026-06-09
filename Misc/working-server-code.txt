import re
import io
import base64
import numpy as np
from PIL import Image
from flask import Flask, request, jsonify
import tensorflow as tf
from keras import layers
import traceback

# Load SavedModel using TFSMLayer
layer = tf.keras.layers.TFSMLayer("saved_model", call_endpoint="serving_default")

# Load labels.txt (format: "0 Banana", "1 Lego", etc.)
with open("labels.txt", "r") as f:
    class_names = []
    for line in f:
        parts = line.strip().split(maxsplit=1)
        if len(parts) == 2:
            class_names.append(parts[1])
        elif len(parts) == 1:
            class_names.append(parts[0]) 

app = Flask(__name__)

def preprocess_image(image_bytes):
    img = Image.open(io.BytesIO(image_bytes)).convert("RGB")
    img = img.resize((224, 224))  # resize to model input size
    img_array = np.array(img).astype("float32") / 255.0
    img_array = np.expand_dims(img_array, axis=0)  # add batch dimension
    return img_array

@app.route("/", methods=["POST"])
def image_rec():
    try:
        data = request.json
        img_data = base64.b64decode(data["img"])
        b64_str = data.get("img", "")
        b64_str = re.sub(r"^data:image/[^;]+;base64,", "", b64_str)
        b64_str = re.sub(r"\s+", "", b64_str).strip()
        b64_str = b64_str.replace(' ', '+')
        pad = len(b64_str) % 4
        if pad:
            b64_str += "=" * (4 - pad)

        img_data = base64.b64decode(b64_str)
        img_array = preprocess_image(img_data)

        # --- FIX: call TFSMLayer with tensor, not dict ---
        result = layer(img_array)
        probs = result["sequential_3"].numpy()[0]

        # --- best prediction only ---
        best_idx = int(np.argmax(probs))
        best_label = class_names[best_idx]
        best_conf = float(probs[best_idx] * 100.0)

        return jsonify({
            "label": best_label,
            "confidence": best_conf
        })

    except Exception as e:
        print("ERROR:", str(e))
        traceback.print_exc()
        return jsonify({"error": str(e)}), 500


if __name__ == "__main__":
    app.run(port=5001)
