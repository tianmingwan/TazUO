---
title: ApiItem
description:  Represents a Python-accessible item in the game world.  Inherits entity and positional data from `ApiEntity` . 
---

## Class Description
 Represents a Python-accessible item in the game world.
 Inherits entity and positional data from `ApiEntity` .


## Properties
### `Amount`

**Type:** `int`

### `Opened`

**Type:** `bool`

### `Container`

**Type:** `uint`

### `RootContainer`

**Type:** `uint`

### `OnGround`

**Type:** `bool`

### `RootEntity`

**Type:** `ApiEntity`

### `__class__`

**Type:** `string`

 The Python-visible class name of this object.
 Accessible in Python as `obj.__class__` .



### `IsCorpse`

**Type:** `bool`

### `IsContainer`

**Type:** `bool`

 Check if this item is a container(Bag, chest, etc)


### `MatchingHighlightName`

**Type:** `string`

 If this item matches a grid highlight rule, this is the rule name it matched against


### `MatchesHighlight`

**Type:** `bool`

 True/False if this matches a grid highlight config



## Enums
*No enums found.*

## Methods
### GetItemData

 Get the items ItemData


**Return Type:** `ApiItemData`

---

### GetContainerGump

 If this item is a container ( item.IsContainer ) and is open, this will return the grid container or container gump for it.


**Return Type:** `ApiUiBaseControl`

---

### NameAndProps
`(wait, timeout, englishOnly)`
 Gets the item name and properties (tooltip text).
 This returns the name and properties in a single string. You can split it by newline if you want to separate them.


**Parameters:**

| Name | Type | Optional | Description |
| --- | --- | --- | --- |
| `wait` | `bool` | ✅ Yes | True or false to wait for name and props |
| `timeout` | `int` | ✅ Yes | Timeout in seconds |
| `englishOnly` | `bool` | ✅ Yes | When True, returns the original English (Cliloc.enu) name and properties instead of the localized text. Use this when matching against hard-coded English strings so scripts keep working under any language. |

**Return Type:** `string`

---

