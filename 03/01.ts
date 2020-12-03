const fs = require('fs');

enum Tile {
    Square = 1,
    Tree
}

type Playground = Record<number, Record<number, Tile>>;

const traverseInstructions = {
    down: 1,
    right: 3,
};

(function main() {
    const lines = inputToLines();

    const map = buildMap(lines);

    const count = traverseMap(map);

    console.log(count);
})();

function traverseMap(map: Playground) {
    let treeCount = 0;
    let row = 0;
    let col = 0;

    while (row < Object.keys(map).length) {
        col %= Object.keys(map[row]).length;

        if (map[row][col] === Tile.Tree) {
            treeCount += 1;
        }

        row += traverseInstructions.down;
        col += traverseInstructions.right;

        console.log(row, col);
    }

    return treeCount;
}

function buildMap(lines: string[]) {
    const map: Playground = {};

    for (let row = 0; row < lines.length; row += 1) {
        const line = lines[row];
        map[row] = {};

        for (let col = 0; col < line.length; col += 1) {
            map[row][col] = charToTile(line.charAt(col));
        }
    }

    return map;
}

function charToTile(char: string): Tile {
    switch (char) {
        case '#': return Tile.Tree;
        case '.': return Tile.Square;
        default: {
            throw new Error(`Unknown char ${char}`);
        }
    }
}

function inputToLines() {
    const lines = fs.readFileSync('./input.txt').toString('utf-8');

    return lines.split('\r\n');
}