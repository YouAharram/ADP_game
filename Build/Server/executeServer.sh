source .venv/bin/activate
uvicorn master_server:app --host 192.168.1.166 --port 8000 --reload
