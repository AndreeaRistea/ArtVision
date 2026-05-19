import os
import tempfile
from contextlib import asynccontextmanager

from fastapi import FastAPI, UploadFile, File, Form
from fastapi.middleware.cors import CORSMiddleware

from Services.clip_service import CLIPService

clip_service: CLIPService | None = None


@asynccontextmanager
async def lifespan(app: FastAPI):
    global clip_service
    clip_service = CLIPService()
    yield


app = FastAPI(title="ArtVision AI Service", lifespan=lifespan)

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)


@app.get("/api/health")
def health():
    return {"status": "ok"}


@app.post("/api/similar")
async def find_similar(file: UploadFile = File(...), top_k: int = Form(10)):
    contents = await file.read()
    suffix = os.path.splitext(file.filename or "image.jpg")[1] or ".jpg"
    with tempfile.NamedTemporaryFile(delete=False, suffix=suffix) as tmp:
        tmp.write(contents)
        tmp_path = tmp.name

    try:
        results = clip_service.find_similar_from_image(tmp_path, top_k)
        return {"results": results}
    finally:
        os.unlink(tmp_path)
