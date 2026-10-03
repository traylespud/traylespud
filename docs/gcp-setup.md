# Google Cloud workbench setup

Step-by-step plan for **cc on the user's Windows PC** to set up the Google
Cloud "agent workbench". Do the steps in order. The user is a beginner: explain
each step in plain language before running it, and **ask before any step that
costs money** (marked 💲).

## What we're building

| Piece | Purpose | Approx. cost |
|-------|---------|--------------|
| Project `agent-workbench-tray` | Container for everything below | Free |
| Budget alert ($10/month) | Email warning before credits drain | Free |
| VM `agent-workbench` (`e2-small`, `us-central1-a`) | Linux workbench where agents work on the repos | ~$0.017/hr **only while running**, plus ~$1.20/mo disk |
| Translation API key | Used by the Pornolab translator userscript (in Tampermonkey) | 500k characters/month free, then from credits |

The VM is **started only when we're working** and stopped afterwards.

## Privacy rules (must follow)

- The VM **never visits Pornolab** and never uses the user's Pornolab login.
- Only text from the **homepage and picture-free guide threads** is sent to the
  Translation API. No images are ever sent anywhere.
- The API key is **never committed** to any repo. It goes into Tampermonkey's
  storage (`GM_setValue`) during lesson 4 of `learning/`.

---

## Step 0: Install and sign in (user does this once)

```powershell
winget install Google.CloudSDK
```

Close and reopen the terminal, then:

```powershell
gcloud auth login
```

Thorium opens; the user clicks **Allow**. Check with `gcloud auth list`.

Before going further, have the user open **console.cloud.google.com →
Billing → Credits** and read out the credit amount and **expiration date**.

## Step 1: Create the project and link billing

Project ID chosen by the user: `agent-workbench-tray` (if taken, use `agent-workbench-tray1`).

```powershell
gcloud projects create agent-workbench-tray --name="agent-workbench"
gcloud config set project agent-workbench-tray
gcloud billing accounts list
gcloud billing projects link agent-workbench-tray --billing-account=BILLING_ACCOUNT_ID
```

## Step 2: Budget alert (do this BEFORE creating anything billable)

```powershell
gcloud services enable billingbudgets.googleapis.com
gcloud billing budgets create `
  --billing-account=BILLING_ACCOUNT_ID `
  --display-name="workbench-monthly-10" `
  --budget-amount=10USD `
  --credit-types-treatment=exclude-all-credits `
  --threshold-rule=percent=0.5 `
  --threshold-rule=percent=0.9 `
  --threshold-rule=percent=1.0
```

`exclude-all-credits` makes the alert measure real usage. Otherwise credits
would hide the spending and the alert would never fire.

Note: a budget **warns**, it does not stop spending.

## Step 3: Turn on the APIs

```powershell
gcloud services enable compute.googleapis.com iap.googleapis.com translate.googleapis.com apikeys.googleapis.com
```

## Step 4: Private network (SSH only, through Google's IAP)

A dedicated network with **one** firewall rule: SSH allowed only from Google's
IAP range (Identity-Aware Proxy, Google's secure tunnel). Nothing else on the
internet can reach the VM.

```powershell
gcloud compute networks create workbench-net --subnet-mode=auto
gcloud compute firewall-rules create workbench-allow-iap-ssh `
  --network=workbench-net --direction=INGRESS --action=allow `
  --rules=tcp:22 --source-ranges=35.235.240.0/20
```

## Step 5: 💲 Create the VM (ask the user first)

```powershell
gcloud compute instances create agent-workbench `
  --zone=us-central1-a `
  --machine-type=e2-small `
  --network=workbench-net `
  --image-family=debian-12 --image-project=debian-cloud `
  --boot-disk-size=30GB --boot-disk-type=pd-standard `
  --shielded-secure-boot `
  --no-service-account --no-scopes `
  --metadata=enable-oslogin=TRUE
```

- `--no-service-account --no-scopes`: the VM gets **no** Google Cloud
  permissions, so an agent on it can't create or delete cloud resources.
- `enable-oslogin`: SSH access is tied to the user's Google account.

## Step 6: Connect and install tools on the VM

```powershell
gcloud compute ssh agent-workbench --zone=us-central1-a --tunnel-through-iap
```

(On Windows, gcloud may offer to set up PuTTY or generate an SSH key the first
time. Say yes.)

On the VM:

```bash
sudo apt-get update && sudo apt-get install -y git tmux curl ca-certificates
# Node.js 22
curl -fsSL https://deb.nodesource.com/setup_22.x | sudo -E bash -
sudo apt-get install -y nodejs
# Claude Code
curl -fsSL https://claude.ai/install.sh | bash
# Headless Chromium for userscript testing (Playwright)
npx -y playwright install --with-deps chromium
```

Then clone the repos (`traylespud/traylespud`, `traylespud/userscripts`) and
run `claude` to sign in.

Teach the user `tmux`: `tmux new -s work` starts a session; `Ctrl+B` then
`D` detaches (it keeps running); `tmux attach -t work` reconnects.

## Step 7: Stop it when done

```powershell
gcloud compute instances stop agent-workbench --zone=us-central1-a
```

Daily use:

```powershell
gcloud compute instances start agent-workbench --zone=us-central1-a
gcloud compute ssh agent-workbench --zone=us-central1-a --tunnel-through-iap
gcloud compute instances stop agent-workbench --zone=us-central1-a
```

Optional safety net: a nightly auto-stop schedule (an instance schedule
resource policy). Ask the user for their time zone first.

## Step 8: Translation API key

```powershell
gcloud services api-keys create --display-name="thorium-translator" `
  --api-target=service=translate.googleapis.com
```

The key only works for the Translation API. Then, in the console, go to
**APIs & Services → Cloud Translation API → Quotas** and set a **daily
character cap** (e.g. 50,000/day) so a bug can't burn through credits.

Show the user where the key is, but **don't save it in any file in a repo**.
It gets pasted into Tampermonkey in lesson 4.

## Step 9 (optional): Extra Gmail accounts

Keep one Owner. Add others with limited roles:

```powershell
gcloud projects add-iam-policy-binding agent-workbench-tray `
  --member=user:OTHER@gmail.com --role=roles/editor
```

## When finished

Update this file with the real project ID and anything that differed, and
commit it (no keys, no billing account IDs).
