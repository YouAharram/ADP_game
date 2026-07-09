import asyncio
import os
import random
import socket  # <--- Inserito per rilevare l'IP
from fastapi import FastAPI, HTTPException
from pydantic import BaseModel

app = FastAPI()

# Database temporaneo in-memory dei party attivi
active_parties = {}

def get_local_ip():
    """Ritorna l'indirizzo IP locale del server nella rete corrente."""
    s = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    try:
        # Non effettua una vera connessione, serve solo a determinare l'interfaccia di rete attiva
        s.connect(("8.8.8.8", 80))
        ip = s.getsockname()[0]
    except Exception:
        ip = "127.0.0.1"
    finally:
        s.close()
    return ip

# Memorizziamo l'IP all'avvio del server per non calcolarlo a ogni richiesta
SERVER_IP = get_local_ip()


async def log_stream(stream, party_code):
    """Legge lo stream di output del server Unity in parallelo e lo stampa nel terminale."""
    while True:
        line = await stream.readline()
        if not line:
            break
        decoded_line = line.decode('utf-8', errors='ignore').strip()
        if decoded_line:
            print(f"[{party_code}-UNITY] {decoded_line}")


# Modelli per la validazione dei dati in ingresso
class CreatePartyRequest(BaseModel):
    playerName: str  

class JoinPartyRequest(BaseModel):
    playerName: str
    code: str        


@app.post("/createParty")
async def create_party(request: CreatePartyRequest):
    party_code = "".join(random.choices("abcdefghijklmnopqrstuvwxyz0123456789", k=6)).upper()
    
    assigned_port = random.randint(7777, 7899)
    while any(party["port"] == assigned_port for party in active_parties.values()):
        assigned_port = random.randint(7777, 7899)
    
    script_dir = os.path.dirname(os.path.abspath(__file__))
    server_executable = os.path.join(script_dir, "builds", "server", "lobbyserver.exe")
    
    if not os.path.exists(server_executable):
        server_executable = os.path.join(script_dir, "builds", "server", "lobbyserver")
    if not os.path.exists(server_executable):
        server_executable = os.path.join(script_dir, "Server_build.x86_64")

    if not os.path.exists(server_executable):
        raise HTTPException(status_code=500, detail=f"Unity executable not found. Path: {server_executable}")

    try:
        print(f"[MASTER] Lancio in parallelo: {server_executable} sulla porta {assigned_port}")
        
        process = await asyncio.create_subprocess_exec(
            server_executable,
            "-batchmode",
            "-nographics",
            "-port", str(assigned_port),
            "-partyCode", party_code,
            "-logFile", "-", 
            stdout=asyncio.subprocess.PIPE,
            stderr=asyncio.subprocess.STDOUT 
        )
        
        asyncio.create_task(log_stream(process.stdout, party_code))
        
        active_parties[party_code] = {
            "port": assigned_port,
            "pid": process.pid,
            "host_player": request.playerName
        }
        
        return {
            "code": party_code,
            "ip": SERVER_IP,  # <--- Ora usa l'IP rilevato dinamicamente
            "port": assigned_port
        }
        
    except Exception as e:
        print(f"[ERRORE CRITICO MASTER SERVER]: {str(e)}")
        raise HTTPException(status_code=500, detail=f"Failed to launch unity server: {str(e)}")


@app.post("/joinParty")
async def join_party(request: JoinPartyRequest):
    party_code = request.code.upper() 
    
    if party_code not in active_parties:
        raise HTTPException(status_code=404, detail="Party code not found")
    
    party_info = active_parties[party_code]
    
    print(f"[MASTER] Player {request.playerName} si sta unendo alla lobby {party_code}")
    
    return {
        "code": party_code,
        "ip": SERVER_IP,  # <--- Ora usa l'IP rilevato dinamicamente
        "port": party_info["port"]
    }
