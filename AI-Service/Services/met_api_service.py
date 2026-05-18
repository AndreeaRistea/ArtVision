import requests
import os
import json
import time


class MetAPIService:
    BASE_URL = "https://collectionapi.metmuseum.org/public/collection/v1"

    def __init__(self, save_folder="data/art_dataset"):
        self.save_folder = save_folder
        os.makedirs(self.save_folder, exist_ok=True)
        self.metadata_path = os.path.join(self.save_folder, "metadata.json")

    def _load_existing_metadata(self):
        if os.path.exists(self.metadata_path):
            try:
                with open(self.metadata_path, "r", encoding="utf-8") as f:
                    return json.load(f)
            except Exception:
                return []
        return []

    def search_objects(self, query="painting", has_images=True):
        url = f"{self.BASE_URL}/search"
        params = {
            "q": query,
            "hasImages": has_images
        }
        response = requests.get(url, params=params)
        response.raise_for_status()
        data = response.json()
        return data.get("objectIDs", [])

    def get_object_details(self, object_id):
        url = f"{self.BASE_URL}/objects/{object_id}"
        response = requests.get(url)
        return response.json()

    def download_dataset(self, query="painting", max_images=50):
        print(f"Searching for '{query}'...")
        category_folder = os.path.join(self.save_folder, query.replace(" ", "_"))
        os.makedirs(category_folder, exist_ok=True)
        object_ids = self.search_objects(query)[:max_images]
        print(f"Found {len(object_ids)} objects")

        all_metadata = self._load_existing_metadata()
        existing_ids = {item["object_id"] for item in all_metadata}

        count = 0

        for obj_id in object_ids:
            if obj_id in existing_ids:
                print(f"Skip {obj_id},already exist in metadata.")
                continue

            try:
                obj = self.get_object_details(obj_id)
                image_url = obj.get("primaryImageSmall")

                if not image_url:
                    continue

                img_data = requests.get(image_url).content

                filename = f"art_{obj_id}.jpg"
                filepath = os.path.join(category_folder, filename)

                with open(filepath, "wb") as f:
                    f.write(img_data)

                all_metadata.append({
                    "category": query,
                    "object_id": obj_id,
                    "file_path": os.path.join(query.replace(" ", "_"), filename),
                    "title": obj.get("title"),
                    "artist": obj.get("artistDisplayName"),
                    "date": obj.get("objectDate")
                })

                print(f"Download: {filename} ({query})")
                count += 1
                time.sleep(0.1)

            except Exception as e:
                print(f"Error for {obj_id}: {e}")

        with open(self.metadata_path, "w", encoding="utf-8") as f:
            json.dump(all_metadata, f, indent=4, ensure_ascii=False)

        print(f"Done! {count} new images add for '{query}'.")