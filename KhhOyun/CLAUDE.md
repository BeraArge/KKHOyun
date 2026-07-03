# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

KKHOyun is a Unity 6 educational game for children preparing for hospital visits/surgery. The game is in Turkish and teaches kids what to pack in a hospital bag and what behaviors to follow pre-surgery. It uses a stage-based structure with a scoring system.

## Unity Version & Tools

- **Engine**: Unity 6000.4.11f1
- **Render Pipeline**: Universal Render Pipeline (URP) 17.4.0
- **Input**: Unity Input System 1.19.0
- **UI Text**: TextMesh Pro (via `com.unity.ugui`)

Open and run the project through the Unity Editor — there are no CLI build commands for day-to-day development.

## Code Architecture

All custom scripts live under `Assets/Script/`, organized by stage:

```
Assets/Script/
  Core/                   — Reusable components shared across stages
    ClickableItem.cs      — IPointerClickHandler that delegates to a RoomManager
    ItemFlyAnimator.cs    — Arc-parabola fly animation (UI Image → slot target)
    WarningPopupUI.cs     — Animated popup (scale + alpha, EaseOutBack open, SmoothStep close)
  Asama1/                 — Stage 1 logic
    Stage1RoomManager.cs  — Orchestrates stage 1: item selection, scoring, completion
```

**Data flow for a click:**
1. `ClickableItem` (on each room object) receives the pointer click and calls `Stage1RoomManager.SelectItem(itemName, flyImage)`.
2. `Stage1RoomManager` evaluates the item name, updates score, triggers `ItemFlyAnimator.FlyToSlot()` with a callback, and shows `WarningPopupUI` for wrong choices.
3. `WarningPopupUI.ShowAndWaitForClose()` is a coroutine yielded by the stage manager for sequenced narrative flow.

**Item names used in Stage 1** (string keys in `SelectItem`):
- Required (correct): `"pijama"`, `"disfircasi"`
- Optional bonus: `"ayicik"` (teddy bear, +10 score, gets removed from bag before surgery in the narrative)
- Penalized: `"cips"` (-10), `"soda"` (-10), `"oyuncaklar"` (-5)
- Stage-restart triggers: `"yemek"`, `"su"` (food/water, forbidden pre-surgery)

**Scoring**: `+10` per required item, `+10` for optional toy, deductions for wrong items. Stage completes when `currentRequiredCount >= requiredCount` (default 2).

## Third-Party Assets

- `Assets/Layer Lab/GUI Pro-CasualGame/` — UI prefab library (buttons, panels); use these prefabs for new UI elements rather than building from scratch.
- `Assets/_Creepy_Cat/3D Games Effects Pack Free/` — particle effect prefabs.

## Adding New Stages

Follow the `Asama1` pattern:
1. Create a `StageNRoomManager.cs` under `Assets/Script/AsamaN/` inheriting `MonoBehaviour`.
2. Wire `ClickableItem` components to the new manager via the Inspector.
3. Reuse `ItemFlyAnimator` and `WarningPopupUI` from Core — assign them in the scene.

## Language Note

All in-game strings are Turkish. String literals in `SelectItem` switch cases (e.g., `"pijama"`, `"disfircasi"`) are the canonical item identifiers — keep them consistent between scene GameObjects and code.
