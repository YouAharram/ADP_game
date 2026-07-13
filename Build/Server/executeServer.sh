source .venv/bin/activate
uvicorn master_server:app --host 127.0.0.1 --port 8000 --reload
