# gcloud notes

A personal cheat sheet for this Google Cloud setup. It grows as you learn:
when you figure out something useful, ask cc to add it here.

Commands are written for **PowerShell on Windows** (the backtick `` ` `` at
the end of a line means "this command continues on the next line").

## My setup

| Thing | Value |
|-------|-------|
| Project ID | `agent-workbench-tray` |
| Zone | `us-central1-a` |
| VM name | `agent-workbench` |
| VM size | `e2-small` (2 GB RAM) |
| Network | `workbench-net` (SSH through IAP only) |
| Budget alert | $10/month, emails at 50%, 90%, 100% |
| Auto-stop | `workbench-nightly-stop`: stops the VM at 20:00 Toronto time daily (never starts it). Set up by `scripts/setup-autostop.sh` |

## Everyday commands

```powershell
# Start the VM (starts billing for the VM, ~$0.017/hr)
gcloud compute instances start agent-workbench --zone=us-central1-a

# Connect to it
gcloud compute ssh agent-workbench --zone=us-central1-a --tunnel-through-iap

# Stop it when done (stops the hourly VM charge; the disk still costs ~$1.20/mo)
gcloud compute instances stop agent-workbench --zone=us-central1-a

# Is it running or stopped?
gcloud compute instances list
```

On the VM, `tmux` keeps work running after you disconnect:

| Do this | Command |
|---------|---------|
| Start a session | `tmux new -s work` |
| Leave it running and disconnect | `Ctrl+B`, then `D` |
| Come back to it | `tmux attach -t work` |

## Checking where you are

```powershell
gcloud auth list              # which Google account is signed in
gcloud config list            # current project, account, and default settings
gcloud config set project agent-workbench-tray   # switch to this project
```

## Checking spending

gcloud can't show a simple "how much have I spent" number. Use the console:

- **Credits left and expiration:** console.cloud.google.com → Billing → Credits
- **What cost what:** Billing → Reports (filter by project `agent-workbench-tray`)
- **Budget alert settings:** Billing → Budgets & alerts

## Getting help

| Need | Where to look |
|------|---------------|
| What a command does and its options | `gcloud <command> --help`, e.g. `gcloud compute instances start --help` |
| Common commands on one page | `gcloud cheat-sheet` |
| Full command reference | cloud.google.com/sdk/gcloud/reference |
| VMs (Compute Engine) | cloud.google.com/compute/docs |
| Translation API | cloud.google.com/translate/docs |
| "How much would this cost?" | cloud.google.com/products/calculator |

## Words you'll see

| Word | Meaning |
|------|---------|
| **Project** | A folder that holds all your cloud resources and their bill |
| **VM / instance** | A virtual computer you rent |
| **Zone** | Which data center the VM lives in |
| **IAP** | Identity-Aware Proxy: Google's secure tunnel, used here for SSH |
| **Service account** | A login for a machine instead of a person (our VM has none, on purpose) |
| **API key** | A password-like string that lets a program use one Google API |
| **Quota** | A usage limit you can set so a bug can't spend too much |

## Things I've learned

<!-- Add notes here as you go. Example:
- 2026-10-05: If ssh hangs, check the VM is started with `gcloud compute instances list`.
-->
- 2026-10-03: **Pasting long commands into cc's `!` prompt** can wrap onto two lines,
  which runs them as two broken commands. Keep long commands in a script
  (e.g. `bash scripts/create-vm.sh`) or paste them in shorter pieces.
- 2026-10-03: **In PuTTY, right-click pastes** (Ctrl+V doesn't).
- 2026-10-03: **gcloud commands go on the PC, not the VM.** The VM has no Google
  Cloud login or permissions on purpose, so gcloud there just errors. Never run
  `gcloud auth login` on the VM. If the prompt says `...@agent-workbench`, you're on the VM.
- 2026-10-06: **Translation daily cap isn't editable in the console.** "v2 and v3 general
  model characters per day" shows Unlimited / Adjustable: No. **TODO before lesson 4**
  (before the key goes into Tampermonkey): set it to 50,000/day with a quota override
  via `gcloud alpha services quota` (needs `gcloud components install alpha`).
- 2026-10-06: API key `thorium-translator` lives in KeePassXC only. It's restricted to
  `translate.googleapis.com`. Copy it to the clipboard without showing it:
  `gcloud services api-keys get-key-string <name> --format="value(keyString)" | Set-Clipboard`
- 2026-10-03: A stopped VM shows as **`TERMINATED`** in `gcloud compute instances list`.
  That just means stopped; nothing is deleted.

### Later: SSH from Windows Terminal instead of PuTTY

Windows has `ssh` built in, and gcloud already made the key. Run
`notepad $HOME\.ssh\config`, paste this, and save:

```
Host workbench
    HostName agent-workbench
    User samashukur_gmail_com
    IdentityFile ~/.ssh/google_compute_engine
    ProxyCommand "C:\Users\yachiru\AppData\Local\Google\Cloud SDK\google-cloud-sdk\bin\gcloud.cmd" compute start-iap-tunnel %h %p --listen-on-stdin --project=agent-workbench-tray --zone=us-central1-a --verbosity=warning
    ServerAliveInterval 60
```

Then connect with `ssh workbench` (start the VM first). It still goes through
the IAP tunnel, so nothing new is opened to the internet.
