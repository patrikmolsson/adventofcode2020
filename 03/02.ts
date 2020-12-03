const fs = require('fs');

enum Tile {
    Square = 1,
    Tree
}

type Playground = Record<number, Record<number, Tile>>;
type Instruction = {
    down: number;
    right: number;
};

const traverseInstructions: Instruction[] = [
    {
        right: 1,
        down: 1,
    },
    {
        right: 3,
        down: 1,
    },
    {
        right: 5,
        down: 1,
    },
    {
        right: 7,
        down: 1,
    },
    {
        right: 1,
        down: 2,
    }
];

(function main() {
    const lines = inputToLines();

    const map = buildMap(lines);

    const counts = traverseInstructions.map(instruction => traverseMap(map, instruction));

    const product = counts.reduce((acc, curr) => acc * curr);

    console.log(counts, product);
})();

function traverseMap(map: Playground, traverseInstruction: Instruction) {
    let treeCount = 0;
    let row = 0;
    let col = 0;

    while (row < Object.keys(map).length) {
        col %= Object.keys(map[row]).length;

        if (map[row][col] === Tile.Tree) {
            treeCount += 1;
        }

        row += traverseInstruction.down;
        col += traverseInstruction.right;
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
        case '#':
            return Tile.Tree;
        case '.':
            return Tile.Square;
        default: {
            throw new Error(`Unknown char ${char}`);
        }
    }
}

function inputToLines() {
    const lines = fs.readFileSync('./input.txt').toString('utf-8');

    return lines.split('\r\n');
}