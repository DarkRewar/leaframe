# Leaframe Changelog

## 0.8.0

### Changes

- Upgrading web CSS/JS framework to an Unity package and using it in UI Toolkit.

### Improvements

- Add `Container` custom control to handle once `.first-child`, `.last-child`, `.even-child` and `.odd-child` ;
- Add abstract `ChildManipulator` to handle children manipulation in `VisualElement` ;
- Add `ChildManipulator` inherited class `FirstChildManipulator` to determine which `VisualElement` is the `.first-child` ;
- Add `ChildManipulator` inherited class `LastChildManipulator` to determine which `VisualElement` is the `.last-child` ;
- Add `ChildManipulator` inherited class `NthChildManipulator` to allow specific child selection ;
- Add `ChildManipulator` inherited class `OnlyChildManipulator` to determine which `VisualElement` is the `.only-child` ;
- Add `NthChildManipulator` inherited class `EvenChildManipulator` to determine which `VisualElement` is the `.even-child` ;
- Add `NthChildManipulator` inherited class `OddChildManipulator` to determine which `VisualElement` is the `.odd-child` ;
- Add `EmptyManipulator` to determine which `VisualElement` is the `.empty` (has no children) ;

## 0.7.0 (web-only)

### Improvements

- Add "normal" color (grey) ;
- Add progress bars.

### Fixes

- Button color correction ;
- Tab panel overflow ;
- Link size in lists (ul, ol) to 100% ;
- Messages font-weight, size and padding.

## 0.6.7 (web-only)

### Fixes

- Dropdowns ;
- 
Modification des couleurs pour un design "flat"
Modification des messages