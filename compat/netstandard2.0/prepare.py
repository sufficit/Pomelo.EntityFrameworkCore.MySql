#!/usr/bin/env python3
"""Build the legacy EF Core provider from a pinned source revision, not patched IL."""
import io
from pathlib import Path
import shutil
import subprocess
import sys
import tarfile

ROOT = Path(__file__).resolve().parents[2]
REVISION = "520f4dbc7733ca5f97122fdf0dddfcf77e1b6cef"


def prepare(output):
    output = output.resolve()
    marker = output / ".pomelo-compat-profile"
    if output.exists():
        if not marker.is_file():
            raise RuntimeError("Refusing to replace an unowned output directory")
        shutil.rmtree(output)
    output.mkdir(parents=True)
    marker.write_text(REVISION)
    if subprocess.run(["git", "-C", str(ROOT), "cat-file", "-e", REVISION], capture_output=True).returncode:
        subprocess.run(["git", "-C", str(ROOT), "fetch", "--no-tags", "--depth=1", "origin", REVISION], check=True)
    archive = subprocess.check_output([
        "git", "-C", str(ROOT), "archive", REVISION, "src/EFCore.MySql", "src/Shared"
    ])
    with tarfile.open(fileobj=io.BytesIO(archive)) as source:
        source.extractall(output, filter="data")
    for file in (output / "src").rglob("*.cs"):
        text = file.read_text(encoding="utf-8-sig")
        text = text.replace("MySql.Data.MySqlClient", "MySqlConnector")
        file.write_text(text)


if __name__ == "__main__":
    prepare(Path(sys.argv[1]))
