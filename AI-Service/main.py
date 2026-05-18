from Services.met_api_service import MetAPIService
from Services.clip_service import CLIPService

if __name__ == "__main__":
    # service = MetAPIService()
    #
    # service.download_dataset(
    #     query="landscape painting",
    #     max_images=100
    # )
    # service.download_dataset(
    #     "portrait",
    #     50
    # )
    # service.download_dataset(
    #     "sculpture",
    #     50
    # )
    clip_service = CLIPService()

    clip_service.generate_image_embeddings()
