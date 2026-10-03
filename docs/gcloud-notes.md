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
