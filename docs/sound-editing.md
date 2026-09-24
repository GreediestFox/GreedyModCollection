# Editing sounds (client side)

- The client audio backend is FMOD. Sound profiles live in `art/datablocks/audioProfiles.cs` (`datablock SFXProfile(name) { local = 1; filename = "art/sound/..."; description = AudioObjectsCloseLoop3D; }`).
- World objects play sounds through `<soundEmitters><emitter><profile>NAME</profile>...` inside an object *state* in `data/cm_objects.xml`. An object whose emitter is only defined in the `Working` state is silent while merely `Complete`. Keep the client and server copies of the file in sync.
- Use mono, 44.1 kHz `.wav` files. In testing, Vorbis files encoded with libsndfile and an `SFXPlayList` datablock did not play on object emitters.
- The description `AudioObjectsCloseLoop3D` (objects channel, 3 m reference distance, 30 m max) was audible; the environment-channel description was not.
- For "random" sounds, pre-mix the clips with silent gaps into one long looping wav.
- Loops: trim to a steady section and crossfade the tail into the head (equal-power) so the loop point does not click.
- Sounds are client-side: every player needs the same files and profile edits.
- Torque caches compiled scripts as `.dso`; delete stale ones after editing `.cs` files (there is none for `audioProfiles.cs` on this client).