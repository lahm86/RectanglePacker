# RectanglePacker
RectanglePacker provides a solution to the bin-packing problem. Given a finite number of bins, and a collection of items to add to them, RectanglePacker attempts to fill the bins using as little space as possible. The solution currently supports packing of rectangles and squares only.

# Usage
This application uses a MIT license as described in the LICENSE file. Follow the steps below to download and use the application.

To use RectanglePacker in your own solution, after making a reference to RectanglePacker.dll:

* Implement `IRectangle` in your solution to use as the basis for the rectangles you wish to pack. Alternatively, simply use or extend `DefaultRectangle`.
* Implement `ITile` in your solution to use as the actual bins into which rectangles will be packed. Again, you can use or extend `DefaultTile`.
* `AbstractPacker` is the basis for the main packing algorithm. Extend this or use `DefaultPacker` if you are using `DefaultRectangle` and `DefaultTile`.

#### Packing Options
The following options can be configured to change the packing behaviour.

* `PackingFillMode`
  * `Horizontal` - rectangles will be added to tiles from left to right, top to bottom.
  * `Vertical` - rectangles will be added to tiles from top to bottom, left to right.
* `PackingGroupMode`
  * `None` - no grouping will be performed.
  * `Squares` - all squares will be grouped together and packed first.
* `PackingOrderMode` - rectangles will be sorted by comparing one of the following properties.
  * `Width`
  * `Height`
  * `Area`
  * `Perimiter`
* `PackingOrder` - works in combination with `PackingOrderMode`.
  * `Ascending`
  * `Descending`
* `PackingStartMethod`
  * `FirstTile` - packing will begin on the first tile.
  * `EndTile` - packing will begin on the last occupied tile - this and the following option are useful for speed if the tiles have pre-filled content.
  * `NewEndTile` - if there is available space for a new tile, this will be added and packing will commence here.
