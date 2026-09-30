# 05 - Cart capacity, well water, stables and other small engine features

## Idea (GreedyFox)
* The wood cart should carry more than the hard-coded 12 "places" of logs.
* Wells should give more than one Water per action.
* A custom storage building should hold only one kind of item (a rabbit cage), and custom drying frames/tubs should survive restarts.

## What was done / tried
* Changing the cart's container size in the database had no effect: the limit is not data-driven.
* The limit was found in the server executable and wrapped per cart type by a hook; the same lookup pattern was reused for wells and stables.

## Output taken into the server
* `cartPlaces` hook: the Wood Cart holds 30 places, horse carts stay at 12. The Wood Cart inventory was also raised to 150 stones by data (MaxContSize 150000, Length 3).
* `wellWater` hook: the stone well and the wooden well give 20 Water per use. Fresh Water (item 3180) from wells, salt water from the sea.
* `stableAlias` hook: custom buildings behave as coop/barn/stable with own capacity and an allowed-animals or single-item ("strict") rule; used for the Rabbit Cage.
* `greenhouseAlias` hook: restart persistence for the Big Drying Frame and Big Tanning Tub (see note 04).

## Technical notes
* Cart: `AbilityImp::PutMovableInCart::_onDoPerform` adds the places already used to the size of the item (byte at type record +0x1C, a log is 2) and refuses above 12 (`cmp r14d,0Ch ; jbe`); `PullMovableFromCart` and the message "Im Wagen sind %1 von %2 Plaetze belegt" (id 2678) print the maximum from a global int. The check ignores the cart's type, so the wrappers set the compare byte and displayed maximum per call from the targeted cart's type. A hitched cart is an `AttachedObject_Entity` whose type is not readable; it is matched by the `AttachedShapeData` datablock id (707 = harnessed wood cart) instead. The object-type lists of abilities 300/301 in `skill_types.xml` decide which carts accept logs.
* Well: `AbilityImp::GetWater::_onDoPerform` resolves the targeted entity and calls `Gathering::Manager::gather` with a one-element item range {204 Water} and the literal quantity 1; the GetWater wrapper records the wanted amount in thread-local storage and the gather wrapper replaces the quantity only for the Water item, so natural water and every other gathering ability are untouched.
* Stables: the breeding manager loads only completed unmovable objects whose type is in a hard-coded SQL `IN (133,134,143,144,517)` literal of fixed length, rewritten in memory (same length); a parameter table in `.data` keyed by stable type (max animals, harvest numbers, product item) is read by six tiny getters that are detoured so an aliased type is looked up as its `behavesLike` type. `checkContainerLimits` is wrapped to refuse animal species not in `allowAnimals` (or every non-listed item with `strict="1"`).
* The engine message helper takes only (connection, id) without parameters; parametrised messages are built client-side from events.
