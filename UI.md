# PanaM UI — Design Guide

This document describes the look, feel, and architecture of the PanaM menu's **dark frosted-glass UI**.

---

## Overview

The menu is drawn with Unity IMGUI but is fully custom-themed: every window, button, toggle, and scrollbar is rendered with procedurally generated textures at runtime. The game world behind the menu is **really blurred** (not just darkened), producing a true frosted-glass effect.

| Layer | What it is |
|---|---|
| Backdrop blur | Live capture of the game view behind the window, downsampled into a soft blur |
| Glass panel | Semi-transparent dark rounded panel with baked-in grain (the "frost") |
| Hairline border | 1px white @ ~8% opacity outline that separates the glass from the game |
| Content | Sidebar navigation + content cards on top of the glass |

## Color system

All colors live in `src/UI/Utilities/Theme.cs`.

- **Glass base** — near-black `#0D1117` at configurable opacity (`glassOpacity`, default ~86%). The blurred game shows through.
- **Surfaces** — cards and panels are translucent white overlays (3–6%) stacked on the glass, so they read as "elevated" without hard edges.
- **Hairlines** — 1px borders in `white @ 8%` for structure; section dividers are `white @ 6%`.
- **Text** — primary `#E8EAF0`, secondary `#9AA3B5`, muted `#6B7280`.
- **Accent** — comes from the existing `PanaM.GUI.Color` config (`menuHtmlColor`). If unset, defaults to `#4F8CFF`. Used for: selected tab pill, toggle switches, search focus ring, primary buttons, slider fills.
- **Semantic colors** — success `#3FB950`, danger `#F85149`, warning `#D29922`.
- **RGB Mode** — cycles the *accent* hue instead of flooding the whole window with color. The glass stays neutral.

## Windows

Every window (Menu, Overload, Console, Roles, Protect, Doors, Tasks) shares the same treatment:

- Rounded corners (12px) via generated 9-slice texture
- Frosted translucent body over the blurred backdrop
- Custom slim title bar: title text left-aligned, drag handle across the full header area
- Drop shadow: a soft dark halo drawn behind the window

The main Menu window layout:

```
+--------------------------------------------------------------+
|  PanaM                                              v3.x     |  <- title bar (drag)
|  [ Search cheats...                                    x ]   |  <- search field
+------------+-------------------------------------------------+
| Movement   |  Tab Title                                      |
| ESP        |                                                 |
| Roles    <-|  +------------------------------------------+   |
| Ship       |  | Section                                  |   |
| Chat       |  |  [switch] Cheat name                     |   |
| ...        |  |  [switch] Another cheat                  |   |
|            |  +------------------------------------------+   |
|            |  +------------------------------------------+   |
|            |  | Camera                                   |   |
|            |  |  ...                                     |   |
|            |  +------------------------------------------+   |
+------------+-------------------------------------------------+
     ^ sidebar                ^ content area (cards)
```

### Title bar
- "PanaM" wordmark + version badge (accent-tinted pill) on the right
- Entire top strip is a drag handle
- Height ~36px, separated from the body by a hairline

### Search bar
- Sits directly under the title bar, spans the window width
- Real text field behavior preserved: click to focus, type to filter, Backspace deletes, Enter/Escape or clicking outside defocuses, `x` clears
- Placeholder text "Search cheats…" in muted gray; blinking caret while focused; accent focus ring when active
- While searching, results from **all tabs** are shown grouped under clickable tab headers; each result is its own switch row. Clicking a group header jumps to that tab.

### Sidebar
- Left rail (~15% width) listing all tabs
- Idle items: secondary text color, transparent background
- Hover: subtle white overlay
- Selected: accent-tinted pill with a small accent bar on the left edge and brighter text

### Content area
- Tab title (large, bold) followed by sections
- Each section ("General", "Camera", "Tracers", …) renders as a **card**: rounded translucent surface, hairline border, section label in small caps-style bold heading inside the card
- Toggles are rows inside the card

## Controls (`Widgets`)

### Toggle switch
Replaces the checkbox everywhere:
- iOS-style pill track (36x18): off = `white @ 10%`, on = accent
- White knob circle that slides between ends
- Row hover highlights the whole line subtly

### Buttons
- Rounded rect (radius 8), `white @ 6%` idle, `@ 10%` hover, `@ 14%` pressed
- Primary variant filled with the accent color
- Danger variant filled red (used by Overload STOP)

### Sliders
- Thin rounded track with accent fill up to the thumb, round knob

### Scrollbars
- Slim (4px) translucent thumbs that brighten on hover/while dragging

### Labels
- `TabTitle`: 20px bold
- `TabSubtitle`: 13px bold uppercase-feel section headers
- Body labels: 14px primary text

## Architecture

New files:

| File | Role |
|---|---|
| `src/UI/Utilities/Theme.cs` | Palette, config accent parsing, procedural texture generation, cached GUIStyles |
| `src/UI/Utilities/Widgets.cs` | Stateless IMGUI controls: `Toggle`, `Button`, `BeginSection`/`EndSection`, `SearchField`, `Slider` |
| `src/UI/Utilities/BackdropBlur.cs` | MonoBehaviour hooked into the camera's `OnRenderImage`; builds the blur chain |

### Backdrop blur
- While the menu is open, the main camera's `OnRenderImage` blits the frame through a progressive downsample chain using plain `Graphics.Blit` (no custom shaders needed). The final tiny RT is sampled bilinearly → smooth gaussian-like blur.
- Runs only when the menu is visible; always finishes with `Blit(source, destination)` so normal rendering is unaffected.
- Recreates render textures on resolution change and re-binds `Camera.main` on scene changes.
- **Fallback:** if no blur frames arrive within a grace period (unsupported pipeline / hook failure), windows automatically use simulated frost (higher-opacity glass + dimmed overlay) so the UI still looks correct.

### Theming rules
- `GUIStylePreset` keeps its old API (`NormalButton`, `TabTitle`, …) but now returns themed styles, so existing call sites stay valid.
- Tabs call `Widgets.Toggle(ref CheatToggles.x, "Label")` instead of raw `GUILayout.Toggle`. Field names and labels are unchanged, so keybinds, profiles, and the search reflection map keep working untouched.
- `UIHelpers.ApplyUIColor()` now manages accent state (config parse + RGB hue cycling) rather than tinting everything globally.

## Config additions (`PanaM.GUI`)

| Entry | Default | Purpose |
|---|---|---|
| `Color` | *(empty → `#4F8CFF`)* | Accent color (existing entry, reused) |
| `BackdropBlur` | `true` | Enables real blur; off/failure falls back to simulated frost |
| `GlassOpacity` | `0.86` | How opaque the glass panels are (0.5–1.0) |
