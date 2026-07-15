#!/bin/bash

# Check if the user has root privileges
if [ "$EUID" -ne 0 ]; then
  echo "Error: Run this script with sudo! (sudo ./setup.sh)"
  exit 1
fi

echo "[SETUP] Configuring the security environment for the server..."

# 1. Creation of the restricted user if it doesn't exist already
if id "python-runner" &>/dev/null; then
    echo "The user 'python-runner' already exists."
else
    useradd -r -s /usr/sbin/nologin python-runner
    echo "User 'python-runner' created successfully."
fi

# 2. Creation of the sandboxed temporary folder
mkdir -p /tmp/unity-sandbox
# Assign the folder to the runner user's group and grant read/execute permissions
chown :python-runner /tmp/unity-sandbox
chmod 770 /tmp/unity-sandbox
echo "Temporary folder /tmp/unity-sandbox configured."

# 3. Sudoers configuration (allows ANYONE running the game to launch python as python-runner)
SUDOERS_FILE="/etc/sudoers.d/unity-python-runner"
if [ ! -f "$SUDOERS_FILE" ]; then
    # Allows all users in the 'sudo' group (or the current user) to use sudo -u python-runner without a password
    echo "ALL ALL=(python-runner) NOPASSWD: /usr/bin/python3" > "$SUDOERS_FILE"
    chmod 0440 "$SUDOERS_FILE"
    echo "Sudoers configuration completed."
else
    echo "Sudoers configuration already present."
fi

echo "[SETUP COMPLETED] You can now start the game server!"
