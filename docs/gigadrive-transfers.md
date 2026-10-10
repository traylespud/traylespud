# Moving big files between my GigaDrive and my Windows PC

My setup: **GigaDrive Gold G10** (GigaRapid). Files up to about 20 GB, in both
directions. The full list of apps I can install is in
[`reference/gigadrive-apps-by-plan.md`](../reference/gigadrive-apps-by-plan.md).

## The short version

| Direction | How | Encrypted? | Counts toward my quota? |
|---|---|---|---|
| Seedbox → PC (download) | **Advanced Browser + IDM** | Yes (`https://`) | **No**, Advanced Browser is exempt |
| PC → Seedbox (upload) | **WinSCP + SFTP** (OpenSSH Debian app) | Yes | Usually no. Most plans only count outgoing traffic, but I still need to check (see "Still to check") |

## Rules I follow

- **Only use encrypted connections:** SFTP, FTPS or `https://`. Never plain
  FTP, and never Windows' "Map network drive" with an `ftp://` address.
  Plain FTP sends my password and files unprotected.
- **Exempt apps** (these don't count toward my bandwidth quota): Default FTP
  Server, Simple Browser, Advanced Browser, vsftpd. Everything else counts,
  including SFTPGo and OpenSSH.
- **If I go over the quota**, my speed drops to 20 Mbps down / 10 Mbps up
  until the monthly reset. At that speed a 20 GB file takes more than 2 hours.
- **Never share** passwords, the IDM config file or my private key (`.ppk`).
  Blur them out of screenshots.

## (a) Downloads: Advanced Browser + IDM (already set up)

I already use Advanced Browser with the IDM (Internet Download Manager)
config file from GigaRapid, and the link starts with `https://`. It's one of
the download managers GigaRapid's own FAQ recommends.

- **Keep the IDM config file private.** It probably contains my Advanced
  Browser login.
- **If downloads get slow**, IDM can split a file into more parts:
  **Options → Connection → Default max. conn. number**, about 8.
- **If downloads are still slow**, check Windows' network auto-tuning (this
  tip is from GigaRapid's FAQ):
  1. Open Command Prompt as Administrator and run
     `netsh interface tcp show global`.
  2. If "Receive Window Auto-Tuning Level" says **Disabled**, run
     `netsh interface tcp set global autotuning=normal`, then reboot.
- **To find my real maximum speed**, install the **LibreSpeed** app and run
  its speed test. It shows Mbps, while download programs show MB/s
  (1 MB/s = 8 Mbps).

## (b) Uploads: WinSCP + SFTP (to set up)

1. **Install the SSH app.** Dashboard → **Application Manager** → install
   **OpenSSH Debian**. Wait until its status shows **Ready**.
2. **Get the login details.** Click the app's **Access** button and write
   down the hostname, port and username.
3. **Install WinSCP** from winscp.net.
4. **Make an SSH key** (skip this if I already have a `.ppk` file).
   1. Open **PuTTYgen**, which comes with PuTTY, and click **Generate**.
      Move the mouse around until it finishes.
   2. Type a passphrase, then click **Save private key**. Keep that `.ppk`
      file safe.
5. **Load the key.** Open **Pageant**, add the `.ppk` file and type the
   passphrase once.
6. **Create the WinSCP connection.**
   1. Click **New Site** and choose File protocol **SFTP**.
   2. Enter the host, port and username from step 2.
   3. Click **Advanced → SSH → Authentication → Tools → Install Public Key
      into Server** and pick the `.ppk` file. WinSCP asks for my password one
      time. After that, logins use the key through Pageant.
   4. Click **Save**.
7. **One-time WinSCP settings** (Options → Preferences):
   - **Transfer → Endurance → Enable transfer resume: "All files"**. If a big
     upload is interrupted, it continues where it stopped instead of starting
     over.
   - **Transfer → Background**: allow parallel connections for large files.
8. **Upload** by dragging files from the left pane (my PC) to the right pane
   (the seedbox).

## Checking that a big file arrived intact

This works after either a download or an upload. It compares a "fingerprint"
(SHA-256 checksum) of the file on both ends. Hashing 20 GB takes a few
minutes.

- **On the seedbox:** in WinSCP, use **Commands → Open Terminal** and run:
  ```
  sha256sum "path/to/file"
  ```
- **On Windows:** in PowerShell, run:
  ```
  Get-FileHash "C:\path\to\file"
  ```
  It uses SHA-256 by default.

The two long codes must match exactly. If they don't, transfer the file again.

## Still to check

- [ ] **Bandwidth Status page:** does my quota count **outgoing only**, or
      **both directions**? If both, uploads count too, and the vsftpd test
      below becomes worth doing.
- [ ] **Optional vsftpd test:** vsftpd doesn't count toward my quota. In
      WinSCP, try connecting with protocol **FTP** and **TLS/SSL Explicit
      encryption**, using the details from vsftpd's FTP configuration.
      - If it connects and shows a certificate, the connection is encrypted
        and I can use it as an extra quota-free route.
      - If it only works with "No encryption", **don't use it**.
- [ ] Once WinSCP is working, note its upload speed here so cc can help if it
      seems slow.

## Troubleshooting

- **"421 X users (the maximum) are already logged in"** (FTP apps only:
  vsftpd, PureFTPd, SFTPGo). The app allows 6 connections by default. Fix:
  reinstall the app with **Clear Appdata** on and **Max Concurrent
  Connections** set a bit higher (about 8–10), then download the new FTP
  configuration. If the install fails, retry once or twice, and keep the
  number as low as possible.
- **App shows 502 Bad Gateway right after installing:** wait 5–10 minutes
  until its status shows **Ready**.
- **The GigaDrive "FileZilla" app** runs *on the seedbox*. It's for copying
  between servers, not for transfers to my PC.

## Sources

GigaRapid's FAQ ("What is counted towards bandwidth quota?", "How to diagnose
slow transfer speeds?", "How to fix the error 421…"), the GigaDrive Supported
Apps page, and my Bandwidth Status page's Exempted Apps list (October 2026).
