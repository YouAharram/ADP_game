#!/bin/bash

# Check if the user has root privileges
if [ "$EUID" -ne 0 ]; then
  echo "Error: Run this script with sudo! (sudo ./firewallProtection.sh)"
  exit 1
fi

echo "[FIREWALL] Applying network isolation rules for user 'python-runner'..."

# Check if the rule already exists
iptables -C OUTPUT -m owner --uid-owner python-runner -j DROP &>/dev/null

if [ $? -ne 0 ]; then
    # If it does not exist, we add it
    iptables -A OUTPUT -m owner --uid-owner python-runner -j DROP
    echo "Network protection ACTIVATED successfully!"
    echo "Remember: this rule is volatile. If you restart the Linux server, you will need to run this script again."
else
    echo "Network protection is already active. No changes needed."
fi
