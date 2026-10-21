# Unity Spatial Audio Spaces

## Overview

Create a Unity spatial audio/video space using Agora RTC so users can move through a shared real-time scene.

Repository: [AgoraIO-Community/Agora_Spaces_Unity](https://github.com/AgoraIO-Community/Agora_Spaces_Unity)

## Prerequisites and Setup

1. Use Unity 2021.3 LTS as recommended by the README. Import the compatible Agora Video SDK (the guide uses v4.2.0) and NewtonSoft JSON package.
2. Set the App ID in `Assets/AgoraSpaces/_AppIDInput/AppIDInput` as described in the README. Supply a suitable token when your project requires authentication.
3. Add the scenes from `Assets/AgoraSpaces/Scenes` to build settings with `SpaceOffline` first, then build for the target platform. Add camera/microphone usage descriptions for macOS/iOS.
4. Run a Mirror host and client. Give clients the host/server address and use the same room name, which identifies the Agora channel.

## What To Check

- Verify both clients see synchronized characters and hear positional audio.
- Test camera visibility and movement-dependent audio.
- Disconnect a client and stop the host; confirm media and Mirror room state terminate.

## Integration Notes

Mirror handles world synchronization while Agora handles media. Both network paths must be reachable. Optional transcription has additional service requirements.

Keep App Certificates, customer secrets and privileged token generation on a trusted backend for production. Use only your own test credentials while configuring historical demos; do not commit them. Preserve the sample's existing dependency versions and lifecycle until a separate migration is tested.

## Source References

- [README.md](../../README.md)

## Validation Status

This recipe was checked against the repository's source and setup documents. The platform build and live service/device flow have not been run as part of this documentation change. Complete the checks above in an environment with the required SDKs, hardware and service access before treating it as a validated deployment.

## Implementation Prompt

```text
You are working in AgoraIO-Community/Agora_Spaces_Unity. Use docs/ai/RECIPE.md and its linked source files to adapt the Unity Spatial Audio Spaces reference to this project.

Start by inspecting the actual sample directory and configuration described above. Resolve its listed missing prerequisites before claiming it is runnable. Preserve the existing architecture and compatible dependency versions. Implement the recipe's feature-specific flow, permissions and resource cleanup. Use a trusted backend for privileged credentials and tokens in production.

Run the repository-specific build or launch flow, then perform each What To Check item. Report the commands run, the observed results and anything blocked by service access, hardware or incomplete sample code. Do not claim live validation from a documentation or catalog build alone.
```
