#!/usr/bin/env bash
# Nightly auto-stop for the agent-workbench VM (docs/gcp-setup.md, Step 7 safety net).
# Stops the VM at 20:00 Toronto time every day. It never starts the VM.
set -euo pipefail

PROJECT=agent-workbench-tray
PROJECT_NUMBER=733926430833
REGION=us-central1
ZONE=us-central1-a
POLICY=workbench-nightly-stop

# 1. A minimal custom role: only the two permissions the scheduler uses.
gcloud iam roles create vmScheduler --project="$PROJECT" \
  --title="VM scheduler (start/stop only)" \
  --permissions=compute.instances.start,compute.instances.stop

# 2. Give that role to Google's Compute Engine service agent, which runs schedules.
#    This is a Google-managed account; nothing on the VM can use it.
gcloud projects add-iam-policy-binding "$PROJECT" \
  --member="serviceAccount:service-${PROJECT_NUMBER}@compute-system.iam.gserviceaccount.com" \
  --role="projects/${PROJECT}/roles/vmScheduler" \
  --condition=None

# 3. The schedule itself: 20:00 every day, Toronto time (handles daylight saving).
gcloud compute resource-policies create instance-schedule "$POLICY" \
  --region="$REGION" \
  --description="Stop agent-workbench nightly at 20:00 Toronto" \
  --vm-stop-schedule="0 20 * * *" \
  --timezone="America/Toronto"

# 4. Attach it to the VM. New permissions can take a minute to apply, so wait first.
echo "Waiting 60s for the new permission to take effect..."
sleep 60
gcloud compute instances add-resource-policies agent-workbench \
  --zone="$ZONE" \
  --resource-policies="$POLICY"

echo "Done: agent-workbench will stop at 20:00 America/Toronto every day."
