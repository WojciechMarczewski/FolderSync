# FolderSync

**FolderSync** is a command-line utility that compares the contents of two directories and synchronizes them by copying missing or changed files from the source to the destination.  

## 🔧 Features

- One-way folder synchronization (source → replica)
- File comparison based on MD5 hash evaluation
- Logging of copied, added, or deleted files
- Self-contained builds for Linux and Windows

## 🚀 Quick Start (No build required)

1. Go to the [Releases](https://github.com/WojciechMarczewski/FolderSync/releases) page.
2. Download the appropriate `.zip` file for your OS (Windows or Linux).
3. Extract the archive.
4. Run the executable from the commandline/terminal.

### Windows
```powershell
FolderSync.exe --source "C:\Path\To\Source" --replica "C:\Path\To\Destination" --interval 5 --log "C:\Path\To\logfile.txt"
```

### Linux
```bash
chmod +x FolderSync
./FolderSync --source /path/to/source --replica /path/to/destination --interval 5 --log /path/to/logfile.txt
```

## 📝 Arguments

| Argument     | Description                                           | Required |
| ------------ | ----------------------------------------------------- | -------- |
| `--source`   | Path to the source directory (monitored for changes). | ✅ Yes    |
| `--replica`  | Path to the replica directory (synchronized target).  | ✅ Yes    |
| `--interval` | Time interval (in seconds) between sync operations.   | ❌  No    |
| `--log`      | Path to the log file where changes will be recorded.  | ✅ Yes    |
