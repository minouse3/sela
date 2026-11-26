"""Offline compile check for SELA runtime scripts using Unity's bundled Roslyn.

The Unity Editor holds the project lock, so this builds the same assembly the
Editor would, outside the Editor, to catch compile errors before opening it.
"""
import os
import subprocess
import sys
import tempfile

PROJECT = r"b:\SELA\sela"
EDITOR = r"C:\Program Files\Unity\Hub\Editor\6000.3.27f2\Editor"
CSC = os.path.join(EDITOR, "Data", "DotNetSdkRoslyn", "csc.dll")
MANAGED = os.path.join(EDITOR, "Data", "Managed")
SCRIPT_ASSEMBLIES = os.path.join(PROJECT, "Library", "ScriptAssemblies")
NETSTANDARD = os.path.join(EDITOR, "Data", "NetStandard", "ref", "2.1.0")

REFERENCES = [
    os.path.join(MANAGED, "UnityEngine", "UnityEngine.dll"),
    os.path.join(MANAGED, "UnityEngine", "UnityEngine.CoreModule.dll"),
    os.path.join(MANAGED, "UnityEngine", "UnityEngine.IMGUIModule.dll"),
    os.path.join(MANAGED, "UnityEngine", "UnityEngine.InputLegacyModule.dll"),
    os.path.join(MANAGED, "UnityEngine", "UnityEngine.PhysicsModule.dll"),
    os.path.join(MANAGED, "UnityEngine", "UnityEngine.AudioModule.dll"),
    os.path.join(MANAGED, "UnityEngine", "UnityEngine.UIModule.dll"),
    os.path.join(MANAGED, "UnityEngine", "UnityEngine.TextRenderingModule.dll"),
    os.path.join(MANAGED, "UnityEngine", "UnityEngine.AnimationModule.dll"),
    os.path.join(MANAGED, "UnityEngine", "UnityEngine.JSONSerializeModule.dll"),
    os.path.join(MANAGED, "UnityEngine", "UnityEngine.TextCoreFontEngineModule.dll"),
    os.path.join(MANAGED, "UnityEngine", "UnityEngine.TextCoreTextEngineModule.dll"),
    os.path.join(SCRIPT_ASSEMBLIES, "UnityEngine.UI.dll"),
    os.path.join(SCRIPT_ASSEMBLIES, "Unity.TextMeshPro.dll"),
    os.path.join(SCRIPT_ASSEMBLIES, "Unity.InputSystem.dll"),
    os.path.join(SCRIPT_ASSEMBLIES, "Unity.InputSystem.ForUI.dll"),
]


def sources(include_editor=False):
    root = os.path.join(PROJECT, "Assets", "_SELA", "Scripts")
    found = []
    for base, _, files in os.walk(root):
        for name in files:
            if not name.endswith(".cs"):
                continue
            path = os.path.join(base, name)
            is_editor = os.sep + "Editor" + os.sep in path
            if is_editor and not include_editor:
                continue
            found.append(path)
    return sorted(found)


def main():
    out_dir = os.path.join(tempfile.gettempdir(), "sela-check")
    os.makedirs(out_dir, exist_ok=True)

    failures = 0

    failures += build(out_dir, "sela-check.dll", sources(), [])
    failures += build(out_dir, "sela-check-editor.dll", sources(include_editor=True),
                      [os.path.join(MANAGED, "UnityEditor.dll")])
    return 1 if failures else 0


def build(out_dir, out_name, files, extra_refs):
    out_dll = os.path.join(out_dir, out_name)

    rsp = os.path.join(out_dir, out_name + ".rsp")
    lines = []
    for ref in REFERENCES + list(extra_refs):
        if not os.path.exists(ref):
            print("MISSING REFERENCE:", ref)
            return 1
        lines.append('-r:"{}"'.format(ref))
    if os.path.isdir(NETSTANDARD):
        for name in sorted(os.listdir(NETSTANDARD)):
            if name.endswith(".dll"):
                lines.append('-r:"{}"'.format(os.path.join(NETSTANDARD, name)))

    for path in files:
        lines.append('"{}"'.format(path))

    lines.append('-target:library')
    lines.append('-out:"{}"'.format(out_dll))
    lines.append('-nostdlib+')
    lines.append('-noconfig')
    lines.append('-langversion:9.0')
    lines.append('-warnaserror+')
    lines.append('-define:UNITY_EDITOR;UNITY_6000_3_OR_NEWER;UNITY_STANDALONE_WIN;UNITY_STANDALONE')
    lines.append('-nowarn:CS1701')

    with open(rsp, "w", encoding="utf-8") as handle:
        handle.write("\n".join(lines))

    cmd = ["dotnet", CSC, "@" + rsp, "-nologo"]
    result = subprocess.run(cmd, capture_output=True, text=True)
    output = (result.stdout or "") + (result.stderr or "")
    errors = [ln for ln in output.splitlines() if ": error " in ln]
    warnings = [ln for ln in output.splitlines() if ": warning " in ln]

    print(out_name, "sources:", len(files), "errors:", len(errors), "warnings:", len(warnings))
    for line in errors[:40]:
        print(" ", line)
    for line in warnings[:20]:
        print(" ", line)
    if result.returncode != 0 and not errors:
        print(output[:4000])
    return 1 if errors else 0


if __name__ == "__main__":
    sys.exit(main())
