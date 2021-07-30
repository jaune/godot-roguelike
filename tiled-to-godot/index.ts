/// <reference types="@mapeditor/tiled-api" />


const computeGodotTileModifier = (cell: cell) => {
  /**
   * no rotation or flips
   * cell.cell.flippedHorizontally is false and
   * cell.cell.flippedVertically is false
   * cell.cell.flippedAntiDiagonally is false
   */
  let secondParam = 0;

  /**
   * rotated 1x left or
   * rotated 3x right
   */
  if (
      cell.flippedHorizontally === false &&
      cell.flippedVertically === true &&
      cell.flippedAntiDiagonally === true
  ) {
      secondParam = -1073741824;
  }

  /**
   * rotated 2x left or 2x right or
   * vertical and horizontal flip
   */
  if (
      cell.flippedHorizontally === true &&
      cell.flippedVertically === true &&
      cell.flippedAntiDiagonally === false
  ) {
      secondParam = 1610612736;
  }

  /**
   * rotated 3x left or
   * rotated 1x right
   */
  if (
      cell.flippedHorizontally === true &&
      cell.flippedVertically === false &&
      cell.flippedAntiDiagonally === true
  ) {
      secondParam = -1610612736;
  }

  /**
   * flipped horizontal or
   * flipped vertical and 2x times rotated left/right
   */
  if (
      cell.flippedHorizontally === true &&
      cell.flippedVertically === false &&
      cell.flippedAntiDiagonally === false
  ) {
      secondParam = 536870912;
  }

  /**
   * flipped horizontal and 1x rotated left or
   * flipped vertical and 1x time rotated right
   */
  if (
      cell.flippedHorizontally === false &&
      cell.flippedVertically === false &&
      cell.flippedAntiDiagonally === true
  ) {
      secondParam = -2147483648;
  }

  /**
   * flipped horizontal and 2x times rotated left/right or
   * flipped vertically
   */
  if (
      cell.flippedHorizontally === false &&
      cell.flippedVertically === true &&
      cell.flippedAntiDiagonally === false
  ) {
      secondParam = 1073741824;
  }

  /**
   * flipped horizontal and 3x rotated left or
   * flipped vertically and 1x rotated left or
   * flipped horizontal and 1x rotated right or
   * flipped vertically and 3x rotated right
   */
  if (
      cell.flippedHorizontally === true &&
      cell.flippedVertically === true &&
      cell.flippedAntiDiagonally === true
  ) {
      secondParam = -536870912;
  }

  return secondParam;
}

const FIRST_PARAM_MAGIC_NUMBER = 65536


const buildTileMapNodeString = (layer: TileLayer) => {
  const data: Array<number> = []
  const rect = layer.region().boundingRect

  for (let i = 0; i <= rect.width; i++) {
    for (let j = 0; j <= rect.height; j++) {
      const x = rect.x + i
      const y = rect.y + j
      const cell = layer.cellAt(x, y)

      if (cell.empty) {
        // ignore empty cell
        continue
      }

      const tile = layer.tileAt(x, y)

      if (!tile) {
        continue
      }


      // TODO: get right id
      const godotTileId = tile.id

      const xValue = x
      const yValue = (xValue < 0) ? y + 1 : y
      const firstParam = xValue + (yValue * FIRST_PARAM_MAGIC_NUMBER)
      const secondParam = computeGodotTileModifier(cell)

      data.push(firstParam, godotTileId, secondParam)
    }
  }

  return [
    `[node name="${layer.name}" type="TileMap" parent="."]`,
    `tile_set = ExtResource( 1 )`,
    `cell_size = Vector2( ${layer.map.tileWidth}, ${layer.map.tileHeight} )`,
    `format = 1`,
    `tile_data = PoolIntArray( ${data.join(', ')} )`,
  ].join('\n')
}

const buildTileMapSceneString = (map: TileMap) => {
  if (map.orientation !== TileMap.Orthogonal) {
    throw new Error(`Unsupported orientation, should be orthogonal`)
  }

  const scene = [
    '[gd_scene load_steps=2 format=2]',
    '',
    '[ext_resource path="res://maps/Test/tileset.tres" type="TileSet" id=1]',
    '',
    '[node name="TileMapChunk" type="Node2D"]',
    '',
  ]

  for (let layerIndex = 0; layerIndex < map.layerCount; layerIndex++) {
    const layer = map.layerAt(layerIndex);

    if (layer.isTileLayer && layer.visible) {
      scene.push(buildTileMapNodeString(layer as TileLayer))
    }
  }

  return scene.join('\n')
}

const visitCell = (layer: TileLayer, it: (x: number, y: number, cell: cell, tile: Tile) => void) => {
  const rect = layer.region().boundingRect

  for (let i = 0; i <= rect.width; i++) {
    for (let j = 0; j <= rect.height; j++) {
      const x = rect.x + i
      const y = rect.y + j
      const cell = layer.cellAt(x, y)

      if (cell.empty) {
        // ignore empty cell
        continue
      }

      const tile = layer.tileAt(x, y)

      if (!tile) {
        continue
      }

      it(x, y, cell, tile)
    }
  }
}

const visitObjectLayer = (map: TileMap, it: (layer: ObjectGroup, index: number) => void ) => {
  if (map.orientation !== TileMap.Orthogonal) {
    throw new Error(`Unsupported orientation, should be orthogonal`)
  }

  for (let layerIndex = 0; layerIndex < map.layerCount; layerIndex++) {
    const layer = map.layerAt(layerIndex)

    if (layer.isObjectLayer) {
      it(layer as ObjectGroup, layerIndex)
    }
  }
}


const visitTileLayer = (map: TileMap, it: (layer: TileLayer, index: number) => void ) => {
  if (map.orientation !== TileMap.Orthogonal) {
    throw new Error(`Unsupported orientation, should be orthogonal`)
  }

  for (let layerIndex = 0; layerIndex < map.layerCount; layerIndex++) {
    const layer = map.layerAt(layerIndex)

    if (layer.isTileLayer) {
      it(layer as TileLayer, layerIndex)
    }
  }
}

interface Box {
  top: number
  right: number
  bottom: number
  left: number
}


interface Chunk {
  Position: {
    x: number
    y: number
  }
  Walkable: Array<number>
}

const buildSimulationData = (map: TileMap) => {
  const chunkWidth = 32
  const chunkHeight = 32

  const defaultValue = Walkable.UNDEFINED

  const chunks: Record<string, Chunk> = {}

  visitTileLayer(map, (layer) => {
    visitCell(layer, (x, y, c, tile) => {
      const chunkX = Math.floor(x / chunkWidth) * chunkWidth
      const chunkY = Math.floor(y / chunkHeight) * chunkHeight
      const chunkKey = `${chunkX} ${chunkY}`

      const xInChunk = x - chunkX
      const yInChunk = y - chunkY
      const indexInChunk = xInChunk + (yInChunk * chunkWidth)
      const dataLength = chunkWidth * chunkHeight

      if ((xInChunk >= chunkWidth) || (xInChunk < 0)) {
        throw new Error(`Out of bounds: x = ${xInChunk} [0, ${chunkWidth-1}]`)
      }

      if ((yInChunk >= chunkHeight) || (yInChunk < 0)) {
        throw new Error(`Out of bounds: y = ${yInChunk} [0, ${chunkHeight-1}]`)
      }

      if ((indexInChunk >= dataLength) || (indexInChunk < 0)) {
        throw new Error(`Out of bounds: index = ${indexInChunk} [0, ${dataLength-1}]`)
      }

      let chunk

      const value = getWalkableFromTile(tile)

      if (chunks[chunkKey]) {
        chunk = chunks[chunkKey]
      }
      else {
        if (value === defaultValue) {
          // NOTE: Avoid empty chunk
          return
        }

        chunk = {
          Position: {
            x: chunkX,
            y: chunkY,
          },
          Walkable: Array(dataLength).fill(defaultValue)
        }
        chunks[chunkKey] = chunk
      }

      chunk.Walkable[indexInChunk] = value;
    })
  })

  console.log(Object.values(chunks).map(({ Position }) => (`${Position.x} ${Position.y} -- `)));

  let DefaultPlayerSpawn: null | { x: number, y: number }  = null;

  visitObjectLayer(map, (layer) => {
    if (!DefaultPlayerSpawn) {
      const obj = layer.objects.find((obj) => (
        (obj.shape === MapObject.Point) &&
        (obj.property('kind:player-spawn') === true)
      ))

      if (obj) {
        DefaultPlayerSpawn = {
          x: Math.floor(obj.x / map.tileWidth),
          y: Math.floor(obj.y / map.tileHeight),
        }
      }
    }
  })

  return {
    DisplayName: map.property('name') || map.fileName || 'undefined',
    DefaultPlayerSpawn,
    ChunkSize: {
      x: chunkWidth,
      y: chunkHeight,
    },
    Chunks: Object.values(chunks),
  }
}

enum Walkable {
  UNDEFINED = 0,
  NO = 1,
  YES = 2
}

const getWalkableFromTile = (tile: Tile) => {
  if (tile.property('walkable:no') === true) {
    return Walkable.NO
  }

  if (tile.property('walkable:yes') === true) {
    return Walkable.YES
  }

  return Walkable.UNDEFINED
}

const writeTextFile = (path: string, data: string) => {
  const file = new TextFile(path, TextFile.WriteOnly)

  file.write(data)
  file.commit()
}

tiled.registerMapFormat('GodotRoguelike', {
  name: 'Godot roguelike format',
  extension: 'tscn',
  write: (map: TileMap, scenePath: string) => {
    try {
      if (map.orientation !== TileMap.Orthogonal) {
        throw new Error(`Unsupported orientation, should be orthogonal`)
      }

      writeTextFile(scenePath, buildTileMapSceneString(map))

      // @ts-ignore
      const extension = FileInfo.suffix(scenePath) as string
      const dataPath = scenePath.substring(0, scenePath.length - extension.length) + 'simulation.json'

      writeTextFile(dataPath, JSON.stringify(buildSimulationData(map)))
    }
    catch (error) {
      return error.message
    }
    return ''
  }
})
