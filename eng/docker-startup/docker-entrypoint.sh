#!/bin/bash
set -e

cp -R /tmp/.ssh/ /root/.ssh/
chmod 700 /root/.ssh
chmod 600 /root/.ssh/id_*
chmod 644 /root/.ssh/id_*.pub

exec "$@"
