import base64

# Lue kuva binäärinä
with open("realLego2.jpg", "rb") as img_file:
    b64_string = base64.b64encode(img_file.read()).decode('utf-8')

# Tee JSON-payload
json_payload = '{\n  "img": "' + b64_string + '"\n}'

# Tallenna tiedostoon
with open("realLego2.txt", "w", encoding="utf-8") as f:
    f.write(json_payload)

print("Payload saved to realLego2.txt")
