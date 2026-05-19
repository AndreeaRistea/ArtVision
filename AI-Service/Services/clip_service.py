import os
import json
import numpy as np
from PIL import Image
import torch
from transformers import CLIPProcessor, CLIPModel


class CLIPService:
    def __init__(self, model_name="openai/clip-vit-base-patch32"):
        self.device = "cuda" if torch.cuda.is_available() else "cpu"
        self.model = CLIPModel.from_pretrained(model_name).to(self.device)
        self.processor = CLIPProcessor.from_pretrained(model_name)

        base_dir = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
        self.embeddings_path = os.path.join(base_dir, "embeddings", "image_embeddings.npy")
        self.mapping_path = os.path.join(base_dir, "embeddings", "image_mapping.json")
        self.dataset_dir = os.path.join(base_dir, "data", "art_dataset")

        self._load_embeddings()

    def _load_embeddings(self):
        self.embeddings = np.load(self.embeddings_path)
        with open(self.mapping_path, "r") as f:
            self.image_paths = json.load(f)

    def _encode_pil_image(self, image: Image.Image) -> np.ndarray:
        inputs = self.processor(images=image, return_tensors="pt").to(self.device)
        with torch.no_grad():
            outputs = self.model.get_image_features(**inputs)
        if isinstance(outputs, torch.Tensor):
            embedding = outputs
        elif hasattr(outputs, "pooler_output"):
            embedding = outputs.pooler_output
        else:
            embedding = outputs[0]
        return embedding.cpu().numpy().flatten()

    def find_similar(self, query_embedding: np.ndarray, top_k: int = 10) -> list:
        norms = np.linalg.norm(self.embeddings, axis=1)
        query_norm = np.linalg.norm(query_embedding)
        similarities = np.dot(self.embeddings, query_embedding) / (norms * query_norm + 1e-8)

        top_indices = np.argsort(similarities)[::-1][:top_k]
        results = []
        for idx in top_indices:
            full_path = self.image_paths[idx].replace("\\", "/")
            relative_path = full_path.replace("data/art_dataset/", "")
            results.append({
                "file_path": relative_path,
                "similarity_score": float(similarities[idx])
            })
        return results

    def find_similar_from_image(self, image_input, top_k: int = 10) -> list:
        if isinstance(image_input, str):
            image = Image.open(image_input).convert("RGB")
            query_emb = self._encode_pil_image(image)
        elif isinstance(image_input, Image.Image):
            query_emb = self._encode_pil_image(image_input.convert("RGB"))
        else:
            image = Image.open(image_input).convert("RGB")
            query_emb = self._encode_pil_image(image)

        return self.find_similar(query_emb, top_k)

    def generate_image_embeddings(self):
        metadata_path = os.path.join(self.dataset_dir, "metadata.json")
        with open(metadata_path, "r") as f:
            metadata = json.load(f)

        image_paths = []
        embeddings = []

        for item in metadata:
            rel_path = item["file_path"]
            full_path = os.path.join(self.dataset_dir, rel_path)
            if not os.path.exists(full_path):
                print(f"Skipping {full_path}, not found")
                continue

            try:
                image = Image.open(full_path).convert("RGB")
                emb = self._encode_pil_image(image)
                embeddings.append(emb)
                image_paths.append(f"data/art_dataset/{rel_path.replace(os.sep, '/')}")
                print(f"Embedded: {rel_path}")
            except Exception as e:
                print(f"Error processing {rel_path}: {e}")

        embeddings_arr = np.array(embeddings)
        os.makedirs(os.path.dirname(self.embeddings_path), exist_ok=True)
        np.save(self.embeddings_path, embeddings_arr)

        with open(self.mapping_path, "w") as f:
            json.dump(image_paths, f, indent=4)

        print(f"Generated {len(image_paths)} embeddings, saved to {self.embeddings_path}")
