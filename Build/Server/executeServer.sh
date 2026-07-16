#!/bin/bash

# Nome della cartella del virtual environment
VENV_DIR="venv"

echo "[START] Avvio del processo di controllo e configurazione..."

# 1. Verifica e installazione di python3-venv (se su sistema Debian/Ubuntu)
if command -v apt-get &> /dev/null; then
    if ! dpkg -s python3-venv &> /dev/null; then
        echo "[Sistema] python3-venv non è presente. Tento l'installazione..."
        echo "Inserisci la password se richiesto:"
        sudo apt-get update -y && sudo apt-get install -y python3-venv
        
        if [ $? -ne 0 ]; then
            echo "[ERRORE] Impossibile installare python3-venv. Lo script potrebbe fallire."
        fi
    fi
fi

# 2. Controllo e creazione dell'ambiente virtuale (venv)
if [ ! -d "$VENV_DIR" ]; then
    echo "[VENV] Creazione dell'ambiente virtuale in corso..."
    python3 -m venv "$VENV_DIR"
    if [ $? -ne 0 ]; then
        echo "[ERRORE] Impossibile creare l'ambiente virtuale. Esco."
        exit 1
    fi
    echo "[VENV] Ambiente creato con successo."
fi

# 3. Attivazione del virtual environment
echo "[VENV] Attivazione in corso..."
source "$VENV_DIR/bin/activate"

# 4. Aggiornamento preventivo di pip
pip install --upgrade pip -q

# 5. Verifica e installazione dei pacchetti Python necessari
for pkg in fastapi uvicorn; do
    if ! pip show "$pkg" > /dev/null 2>&1; then
        echo "[Dipendenze] Installazione di $pkg in corso..."
        pip install "$pkg"
        if [ $? -ne 0 ]; then
            echo "[ERRORE] Installazione di $pkg fallita. Esco."
            exit 1
        fi
    fi
done

echo "[OK] Tutte le dipendenze sono verificate e installate!"

# 6. Avvio sicuro del server FastAPI con Uvicorn su 0.0.0.0
# Ascoltando su 0.0.0.0, Uvicorn risponderà su qualsiasi IP della macchina.
# Il tuo master_server.py comunicherà comunque l'IP corretto a Unity tramite get_local_ip().
PORT=8000
echo "[RUN] Avvio di Uvicorn su tutte le interfacce (porta $PORT)..."
uvicorn master_server:app --host 127.0.0.1 --port $PORT --reload
