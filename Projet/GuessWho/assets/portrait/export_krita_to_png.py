from krita import Krita, InfoObject
import os

# Dossier courant (où est situé le script)
script_dir = os.path.dirname(os.path.abspath(__file__))

# Sous-dossier contenant les .kra
input_folder = os.path.join(script_dir, "krita")

# Dossier de sortie = même que le script
output_folder = script_dir

app = Krita.instance()

for filename in os.listdir(input_folder):
    if filename.endswith(".kra"):
        doc_path = os.path.join(input_folder, filename)
        doc = app.openDocument(doc_path)
        app.setActiveDocument(doc)

        name = os.path.splitext(filename)[0]
        output_path = os.path.join(output_folder, name + ".png")

        # Créer les options d’export PNG
        export_info = InfoObject()
        export_info.setProperty("compression", "1")  # Compression minimale (sans perte)
        export_info.setProperty("forceSRGB", "true")  # Pour éviter les surprises de profil
        export_info.setProperty("alpha", "true")      # Préserver la transparence si présente

        doc.exportImage(output_path, export_info)
        doc.close()
