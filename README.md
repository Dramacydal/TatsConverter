# TatsConverter

Converts SlaveTats overlays to RaceMenu overlays

Note: you still need to compile result script.

```
TatsConverter.exe --help
Description:
  TatsConverter: convert SlaveTats to RaceMenu overlays

Usage:
  TatsConverter [options]

Options:
  --context <context> (REQUIRED)  Conversion context - the name of result plugin
  --out <out> (REQUIRED)          Output path where plugin will be created
  --format <esl|esp>              Output plugin format [default: esp]
  --skyrim <le|se|segog>          Skyrim version [default: se]
  --data-path <data-path>         Path to tats data directory (where "textures" directory is)
  --json-path <json-path>         Path to tats json directory (where *.json files are)
  --list                          Only list found tattoos, do not create anything
  --version                       Show version information
  -?, -h, --help                  Show help and usage information
```
