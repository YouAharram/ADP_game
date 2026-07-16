# Guida all'avvio del progetto

## Dipendenze

Assicurarsi di avere installato:

- Python 3
- FastAPI
- Uvicorn

Installazione delle dipendenze:

```bash
pip install fastapi uvicorn
```

---

## Configurazione del firewall

Se si utilizza `ufw`, abilitare le porte necessarie:

```bash
sudo ufw allow 8000/tcp
sudo ufw allow 7777:7899/tcp
```

---

## Avvio del Master Server

Spostarsi nella cartella:

```text
Build/Server
```

Avviare il server in locale con il comando:

```bash
uvicorn master_server:app --host 127.0.0.1 --port 8000 --reload
```

Il server sarà raggiungibile all'indirizzo:

```text
http://127.0.0.1:8000
```

---

## Avvio del Client

Il client del gioco si trova nella cartella:

```text
Build/Client
```

Da qui è possibile avviare il gioco.

---

# Test Multiplayer

Per testare il multiplayer tra più computer nella stessa rete (o tramite un IP raggiungibile):

## 1. Avviare il server con il proprio indirizzo IP

Individuare l'indirizzo IP del computer che ospita il server e avviarlo con:

```bash
uvicorn master_server:app --host <PROPRIO_IP> --port 8000 --reload
```

Ad esempio:

```bash
uvicorn master_server:app --host 192.168.1.100 --port 8000 --reload
```

---

## 2. Configurare il client

Aprire il file:

```text
LobbyUIController.cs
```

e sostituire l'indirizzo IP del server con quello del computer che ospita il Master Server.

---

## 3. Rigenerare il progetto

Dopo aver modificato `LobbyUIController.cs`, effettuare una nuova build del:
- Server
- Client

---
