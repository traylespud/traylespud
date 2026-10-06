#!/usr/bin/env bash
# Creates the agent-workbench VM exactly as described in docs/gcp-setup.md (Step 5).
# Kept as a script so the long command can't be broken by line wrapping when pasted.
set -euo pipefail

gcloud compute instances create agent-workbench \
  --zone=us-central1-a \
  --machine-type=e2-small \
  --network=workbench-net \
  --image-family=debian-12 --image-project=debian-cloud \
  --boot-disk-size=30GB --boot-disk-type=pd-standard \
  --shielded-secure-boot \
  --no-service-account --no-scopes \
  --metadata=enable-oslogin=TRUE
